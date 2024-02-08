use crate::{
    asset_collector,
    file_log,
    toc_factory, toc_factory::{CONTAINER_DATA, CONTAINER_ENTRIES_OSPATH_POOL, TARGET_TOC, TARGET_CAS, PartitionBlock}
};
use std::{
    ffi::CStr,
    os::raw::c_char
};

#[no_mangle]
#[allow(non_snake_case)]
// modId is used by the asset collector profiler
pub unsafe extern "C" fn AddFromFolders(modId: *const c_char, modPath: *const c_char) {
    asset_collector::add_from_folders(CStr::from_ptr(modId).to_str().unwrap(), CStr::from_ptr(modPath).to_str().unwrap());
}

#[no_mangle]
#[allow(non_snake_case)]
// haiiii Reloaded!!!! :3
pub unsafe extern "C" fn BuildTableOfContentsEx(
    // UTOC
    basePath: *const c_char,
    version: u32,
    tocData: *mut *const u8,
    tocLength: *mut u64,
    // UCAS
    blocks: *mut *const PartitionBlock,
    blockCount: *mut usize,
    header: *mut *const u8,
    headerSize: *mut usize
    ) -> bool {
    let base_path_owned = CStr::from_ptr(basePath).to_str().unwrap();
    let toc_path = base_path_owned.to_owned() + "\\" + TARGET_TOC;
    let cas_path = base_path_owned.to_owned() + "\\" + TARGET_CAS;
    match toc_factory::build_table_of_contents(&toc_path, version) {
        Some(n) => {
            println!("Built table of contents, version {}", version);
            // UTOC
            *tocLength = n.len() as u64; // set length parameter
            *tocData = n.leak().as_ptr(); // leak memory lol (toc data needs to live for rest of program)
            // UCAS
            let container_lock = CONTAINER_DATA.lock().unwrap();
            match (*container_lock).as_ref() {
                Some(n) => {
                    println!("Built container file");
                    *blocks = n.virtual_blocks.as_ptr();
                    *blockCount = n.virtual_blocks.len();
                    *header = n.header.as_ptr();
                    *headerSize = n.header.len();
                    true
                },
                None => false
            }
        },
        None => false
    }
}

#[no_mangle]
#[allow(non_snake_case)]
pub unsafe extern "C" fn PrintAssetCollectorResults() {
    asset_collector::print_asset_collector_results();
}

#[no_mangle]
#[allow(non_snake_case)]
pub unsafe extern "C" fn GetTocFilenames(tocPath: *const c_char, chunkIds: *mut *const u8, names: *mut *const u8) -> bool {
    let filename = CStr::from_ptr(tocPath).to_str().unwrap();
    // this gets called when the target TOC already has an open file handle made by Win32
    // all we need to get the info needed for filenames is to read header and directory index
    println!("UTOC Emulator File Access TODO for {}", filename);
    false
}

#[no_mangle]
#[allow(non_snake_case)]
pub unsafe extern "C" fn GetTocFilenamesEx(tocPath: *const c_char, version: u32, chunkIds: *mut *const u8, names: *mut *const u8) -> bool {
    file_log::get_toc_filenames(CStr::from_ptr(tocPath).to_str().unwrap(), version);
    false
}