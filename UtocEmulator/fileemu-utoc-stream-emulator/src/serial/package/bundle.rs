//! Structures for defining export bundles, including their header and entries

/// # FExportBundleHeader
#[repr(C)]
#[derive(Debug)]
pub struct HeaderType1 { // 4.25-4.27
    first_entry_index: u32,
    entry_count: u32
}

/// # FExportBundleHeader
#[repr(C)]
#[derive(Debug)]
pub struct HeaderType2 { // 5.0 - 5.2
    serial_offset: u64,
    first_entry_index: u32,
    entry_count: u32
}

#[repr(u32)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum CommandType {
    Create,
    Serialize,
    Count, // Added in 4.26
}

/// # FExportBundleEntry
#[repr(C)]
#[derive(Debug)]
pub struct Entry { // 4.25
    local_export_index: u32,
    command_type: CommandType,
}