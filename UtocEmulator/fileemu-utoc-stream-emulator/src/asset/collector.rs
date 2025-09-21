use std::cell::{RefCell, RefMut};
use crossbeam_deque::Injector;
use crate::{
    log,
    platform::Metadata,
    serial::chunk::id::{ ChunkId, ChunkIdImpl },
};
use std::sync::{ Arc, Condvar, Mutex, OnceLock };
use std::collections::HashMap;
use std::hash::{Hash, Hasher};
use std::ops::{Deref, DerefMut};
use walkdir::{DirEntry, WalkDir };
use crate::asset::node::{File, Node};

// Not implemented: .m.bulk (Memory Mapped Bulk Data)
static SUPPORTED_EXTENSIONS: [&str; 4] = ["uasset", "ubulk", "uptnl", "umap"];

fn asset_supported(entry: &DirEntry) -> bool {
    let ext = match entry.path().extension() {
        Some(x) => x.to_str().unwrap(),
        None => return false
    };
    if entry.metadata().is_err() {
        return false;
    }
    SUPPORTED_EXTENSIONS.contains(&ext)
}

#[derive(Debug, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum CollectorSignal {
    Waiting,
    Collecting((String, u64)),
    Ending
}

#[derive(Debug)]
pub struct Collector {
    thread: std::thread::JoinHandle<Node>,
    signal: Arc<(Mutex<CollectorSignal>, Condvar)>
}

impl Collector {
    pub(crate) fn new() -> Self {
        let v = Arc::new((Mutex::new(CollectorSignal::Waiting), Condvar::new()));
        let vx = Arc::clone(&v);
        let thread = std::thread::spawn(move || {
            let (state, cvar) = &*vx;
            let root = Node::Dir(HashMap::new()); // (Game)
            loop {
                let mut can_collect = state.lock().unwrap();
                while *can_collect == CollectorSignal::Waiting {
                    can_collect = cvar.wait(can_collect).unwrap();
                }
                match &*can_collect {
                    CollectorSignal::Waiting => continue,
                    CollectorSignal::Collecting((path, prio)) => {
                        Self::_file_thread_collect_assets(path, &root, *prio);
                        *can_collect = CollectorSignal::Waiting;
                    },
                    CollectorSignal::Ending => break
                }
            }
            root
        });
        Self { thread, signal: v }
    }

    fn _file_thread_collect_assets(path: &str, root: &Node, prio: u64) {
        for entry in WalkDir::new(path).into_iter()
            .filter_map(|e| e.ok()
                .and_then(|v| match asset_supported(&v) { true => Some(v), false => None }
                )) {
            let full_path = entry.path().to_str().unwrap().to_owned();
            // use unix separator char for OS - fine to replace in-place since they are both one byte
            for (i, _) in full_path.match_indices(|c| c == std::path::MAIN_SEPARATOR) {
                #[allow(mutable_transmutes)]
                unsafe { *(std::mem::transmute::<_, &mut u8>(&full_path.as_bytes()[i])) = b'/' };
            }
            let path = &full_path[path.len() + 1..]; // relative to base directory
            /*
            match ChunkId::from_os_path(path).and_then(|hash| {
                Ok(hash)
            }) {
                _ => ()
            }
            */
            match ChunkId::from_os_path(path) {
                Ok(hash) => {
                    let size = Metadata::get_object_size(&entry);
                    // generate package summary
                    let dirs = path.split('/').collect::<Vec<_>>();
                    // ignore dirs[0], that's root object (Game)
                    let mut curr = root;
                    for dir in &dirs.as_slice()[1..dirs.len() - 1] {
                        match curr.get_dir(*dir) {
                            Some(ch) => curr = ch,
                            None => {
                                let curr_mut = unsafe { &mut *(&raw const *curr as *mut Node) };
                                curr_mut.insert(Node::new_dir(*dir));
                                curr = curr.get_dir(*dir).unwrap();
                            }
                        }
                    }
                    let curr_mut = unsafe { &mut *(&raw const *curr as *mut Node) };
                    curr_mut.insert(Node::new_file(*dirs.last().unwrap(), hash, size, path, prio));
                },
                Err(e) => {
                    log!(Warning, "Could not add file {}. Reason: {}", path, e);
                }
            }
        }
    }

    pub(crate) fn add_folders(&mut self, path: String, prio: u64) {
        let (state, cvar) = &*self.signal;
        *state.lock().unwrap() = CollectorSignal::Collecting((path, prio));
        cvar.notify_one();
    }

    pub(crate) fn close_thread(self) -> Node {
        let (state, cvar) = &*self.signal;
        *state.lock().unwrap() = CollectorSignal::Ending;
        cvar.notify_one();
        self.thread.join().unwrap()
    }
}

type InjectorType = Injector<Collector>;

static TASK_INJECTOR: OnceLock<InjectorType> = OnceLock::new();

fn get_injector() -> &'static InjectorType {
    TASK_INJECTOR.get().unwrap_or_else(|| {
        let _ = TASK_INJECTOR.set(InjectorType::new());
        TASK_INJECTOR.get().unwrap()
    })
}

static TASK_COUNT: OnceLock<usize> = OnceLock::new();

#[no_mangle]
pub unsafe extern "C" fn setup_folder_threads() {
    let injector = get_injector();
    // let _ = TASK_COUNT.set(2);
    let _ = TASK_COUNT.set(std::thread::available_parallelism()
        .map_or(1, |v| (v.get() - 1).max(1)));
    let task_count = *TASK_COUNT.get().unwrap();
    log!(Debug, "Spawning {} collector tasks", task_count);
    for _ in 0..task_count {
        injector.push(Collector::new());
    }
}

static ASSET_PRIORITY: Mutex<u64> = Mutex::new(0);

#[no_mangle]
pub unsafe extern "C" fn add_from_folders(mod_path: *const u16, path_len: usize) {
    add_from_folders_impl(String::from_utf16(std::slice::from_raw_parts(mod_path, path_len)).unwrap());
}

fn add_from_folders_impl(mod_path: String) {
    let id = *ASSET_PRIORITY.lock().unwrap();
    log!(Information, "{} (priority: {})", &mod_path, id);
    *ASSET_PRIORITY.lock().unwrap() += 1;
    let injector = get_injector();
    let mut task = None;
    while task.is_none() {
        task = injector.steal().success();
        if task.is_none() {
            log!(Debug, "{}: Ran out of tasks, waiting 10 ms", id);
            std::thread::sleep(std::time::Duration::from_millis(10));
        }
    }
    let mut task = task.unwrap();
    task.add_folders(mod_path, id);
    injector.push(task);
}

#[no_mangle]
pub unsafe extern "C" fn close_folder_threads() {
    let injector = get_injector();
    let mut tasks_remaining = *TASK_COUNT.get().unwrap();
    // let root = Node::Dir(HashMap::new()); // (Game)
    while tasks_remaining > 0 {
        match injector.steal().success() {
            Some(task) => {
                let root = task.close_thread();
                let mut dir = vec![];
                if !root.is_empty() {
                    for (k, n) in &root {
                        match n {
                            Node::File(f) => {
                                dir.push(k);
                                log!(Debug, "{:?}", dir.join("/"));
                                dir.pop();
                            },
                            Node::Dir(_) => {
                                dir.push(k);
                                log!(Debug, "{:?}", dir.join("/"));
                            },
                            Node::Return => {
                                dir.pop();
                            }
                        }
                    }
                }
                tasks_remaining -= 1;
            },
            None => {
                log!(Debug, "close_folder_threads: waiting 10 ms");
                std::thread::sleep(std::time::Duration::from_millis(10));
            }
        }
    }
}


/// # Important!
/// Since the crate uses global state shared between threads, tests must only be run single-threaded
/// (append -- --test-threads=1 to your cargo test command)
#[cfg(test)]
mod tests {
    use std::path::{Path, PathBuf};
    use std::time::Instant;
    use crate::asset::collector::{add_from_folders_impl, close_folder_threads, setup_folder_threads};
    use crate::global::EngineVersion;

    #[test]
    fn mod_test() {
        crate::util::tests::setup(EngineVersion::UE_4_27);
        unsafe { setup_folder_threads() };
        let start = Instant::now();
        let r2mods = PathBuf::from(std::env::var("RELOADEDIIMODS").unwrap())
            .join("p3rpc.isitworking/UnrealEssentials");

        if !Path::new(r2mods.as_path()).exists() {
            return;
        }
        add_from_folders_impl(r2mods.to_str().unwrap().to_owned());
        unsafe { close_folder_threads() };
        println!("Time completed: {} ms", Instant::now().duration_since(start).as_micros() as f64 / 1000.);
    }
}