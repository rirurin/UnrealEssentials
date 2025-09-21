use std::collections::HashMap;
use std::sync::LazyLock;
use crate::serial::chunk::id::ChunkId;

#[derive(Debug, PartialEq, Eq, Hash)]
pub enum KeyType {
    File,
    Dir
}

#[derive(Debug, PartialEq, Eq, Hash)]
pub struct Key {
    _t: KeyType,
    name: String
}

impl Key {
    pub fn file(name: impl Into<String>) -> Self {
        Self { _t: KeyType::File, name: name.into() }
    }

    pub fn dir(name: impl Into<String>) -> Self {
        Self { _t: KeyType::Dir, name: name.into() }
    }

    pub fn get_name(&self) -> &str {
        &self.name
    }
}

#[derive(Debug)]
pub struct File {
    hash: ChunkId,
    size: u64,
    os: String,
    prio: u64,
}

impl File {
    pub fn new(hash: ChunkId, size: u64, os: String, prio: u64) -> Self {
        Self { hash, size, os, prio }
    }
}

#[derive(Debug)]
pub enum Node {
    File(File),
    Dir(HashMap<Key, Node>),
    Return
}

impl Node {
    pub fn new_dir(name: impl Into<String>) -> (Key, Self) {
        (Key::dir(name), Self::Dir(HashMap::new()))
    }

    pub fn new_file(name: impl Into<String>, hash: ChunkId, size: u64, os: impl Into<String>, prio: u64) -> (Key, Self) {
        (Key::file(name), Self::File(File::new(hash, size, os.into(), prio)))
    }

    pub fn new_ret() -> (Key, Self) {
        (Key::dir(""), Self::Return)
    }

    pub fn insert(&mut self, value: (Key, Self)) {
        if let Self::Dir(dir) = self {
            dir.insert(value.0, value.1);
        }
    }

    pub fn is_empty(&self) -> bool {
        match self {
            Self::Dir(m) => m.is_empty(),
            _ => true
        }
    }

    pub fn get_file(&self, key: impl Into<String>) -> Option<&Node> {
        match self {
            Self::Dir(m) => m.get(&Key::file(key)),
            _ => None
        }
    }

    pub fn get_dir(&self, key: impl Into<String>) -> Option<&Node> {
        match self {
            Self::Dir(m) => m.get(&Key::dir(key)),
            _ => None
        }
    }

    pub fn get_dir_mut(&mut self, key: impl Into<String>) -> Option<&mut Node> {
        match self {
            Self::Dir(m) => m.get_mut(&Key::dir(key)),
            _ => None
        }
    }
}

static UP_DIR: LazyLock<(Key, Node)> = LazyLock::new(|| Node::new_ret());

impl<'a> IntoIterator for &'a Node {
    type Item = (&'a str, &'a Node);
    type IntoIter = NodeIterator<'a>;
    fn into_iter(self) -> Self::IntoIter {
        Self::IntoIter {
            queue: vec![("Game", self)]
        }
    }
}

pub struct NodeIterator<'a> {
     queue: Vec<(&'a str, &'a Node)>
}

impl<'a> Iterator for NodeIterator<'a> {
    type Item = (&'a str, &'a Node);
    fn next(&mut self) -> Option<Self::Item> {
        self.queue.pop().map(|(k, v)| {
            if let Node::Dir(d) = v {
                self.queue.push(((&*UP_DIR).0.get_name(), &(&*UP_DIR).1));
                self.queue.extend(d.iter().map(|(k, v) | (k.get_name(), v)));
            }
            (k, v)
        })
    }
}