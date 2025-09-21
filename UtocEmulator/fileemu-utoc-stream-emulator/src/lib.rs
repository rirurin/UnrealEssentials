//! # UTOC File Emulator
//! *Rust portion*

pub mod asset {
    pub mod collector;
    pub mod node;
}
pub mod io_toc; // Types for IO Store Table of Contents
pub mod global;
pub mod logger;
/// Platform agnostic abstractions
pub mod platform;
/// Modules for serialization
pub mod serial {
    /// Modules for creating chunk IDs (in UTOC)
    pub mod chunk {
        pub mod ctype;
        pub mod error;
        pub mod id;
    }
    /// Modules for creating UCAS header
    pub mod container {
        pub mod error;
        pub mod header;
        pub mod id;
        pub mod locale;
        pub mod package;
        pub mod redirect;
    }
    /// Modules for creating UTOC header
    pub mod header {
        pub mod common;
        pub mod error;
        pub mod version;
    }
    /// Modules for reading loose IO Store assets
    pub mod package {
        pub mod bundle;
        pub mod error;
        pub mod export;
        pub mod id;
        pub mod index;
        pub mod summary;
    }
}
/// Unreal serialized string types
pub mod string;
pub mod metadata;
pub mod util {
    pub mod tests;
}