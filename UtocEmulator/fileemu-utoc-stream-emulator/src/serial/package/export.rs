//! # FExportMapEntry

use rkyv::{Archive, Deserialize, Serialize};
use crate::serial::package::index::SerialPackageIndex;
use crate::string::FMappedName;

type PackageObjectIndex = u64;
type ObjectFlags = u32;
type FilterFlags = u8;

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Serialize, Deserialize)]
pub struct MapEntryType1 { // 4.25
    serial_size: u64,
    object_name: FMappedName,
    outer_index: SerialPackageIndex,
    class_index: SerialPackageIndex,
    super_index: SerialPackageIndex,
    template_index: SerialPackageIndex,
    global_import_index: i32,
    object_flags: ObjectFlags,
    filter_flags: FilterFlags
}

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Serialize, Deserialize)]
pub struct MapEntryType2 { // 4.25+, 4.26-4.27
    cooked_serial_offset: u64,
    cooked_serial_size: u64,
    object_name: FMappedName,
    outer_index: PackageObjectIndex,
    class_index: PackageObjectIndex,
    super_index: PackageObjectIndex,
    template_index: PackageObjectIndex,
    global_import_index: PackageObjectIndex,
    object_flags: ObjectFlags,
    filter_flags: FilterFlags
}

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Serialize, Deserialize)]
pub struct MapEntryType3 { // 5.0+
    /// Offset from start of exports data (HeaderSize + CookedSerialOffset gives actual offset in iobuffer)
    cooked_serial_offset: u64,
    cooked_serial_size: u64,
    object_name: FMappedName,
    outer_index: PackageObjectIndex,
    class_index: PackageObjectIndex,
    super_index: PackageObjectIndex,
    template_index: PackageObjectIndex,
    public_export_hash: u64,
    object_flags: ObjectFlags,
    filter_flags: FilterFlags
}

/// # Important!
/// Since the crate uses global state shared between threads, tests must only be run single-threaded
/// (append -- --test-threads=1 to your cargo test command)
#[cfg(test)]
mod tests {
}