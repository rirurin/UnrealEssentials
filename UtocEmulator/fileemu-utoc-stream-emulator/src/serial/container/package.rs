//! # FPackageStoreEntry/FFilePackageStoreEntry

use crate::serial::package::id::PackageId;

#[derive(Debug)]
pub struct Type1 { // 4.25+, 4.26-4.27
    id: PackageId,

    export_bundle_size: u64,
    export_count: u32,
    export_bundle_count: u32,
    imported_packages: Vec<PackageId>
}

#[derive(Debug)]
pub struct Type2 { // 5.0 - 5.2
    id: PackageId,

    export_count: u32,
    export_bundle_count: u32,
    imported_packages: Vec<PackageId>, // serialized as TFilePackageStoreEntryCArrayView
    // shader_map_hashes: Vec<u8>, // Vec<FSHA1>
}

#[derive(Debug)]
pub struct Type3 { // 5.3+
    id: PackageId,

    imported_packages: Vec<PackageId>, // serialized as TFilePackageStoreEntryCArrayView
    // shader_map_hashes: Vec<u8>, // Vec<FSHA1>
}