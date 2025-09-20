//! # UTOC File Emulator
//! *Rust portion*

pub mod asset {
    pub mod collector;
    pub mod node;
}
pub mod io_toc; // Types for IO Store Table of Contents
pub mod global;
pub mod logger;
pub mod platform; // Platform agnostic abstractions
pub mod serial {
    pub mod chunk {
        pub mod ctype;
        pub mod error;
        pub mod id;
    }
    pub mod container {
        pub mod id;
    }
    pub mod header {
        pub mod common;
        pub mod error;
        pub mod version;
    }
    pub mod package {
        pub mod error;
        pub mod id;
    }
}
pub mod string; // Unreal serialized string types
pub mod metadata;
pub mod util {
    pub mod tests;
}