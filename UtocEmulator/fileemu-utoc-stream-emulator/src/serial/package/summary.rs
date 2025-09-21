//! # FPackageSummary/FZenPackageSummary

use rkyv::{Archive, Deserialize, Serialize};
use crate::string::FMappedName;

pub trait SummaryImpl {
    fn get_package_flags(&self) -> u32;
    fn get_import_map_offset(&self) -> i32;
    fn get_export_map_offset(&self) -> i32;
    fn get_export_bundle_offset(&self) -> i32;
}

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Deserialize, Serialize)]
pub struct Type1 { // 4.25
    package_flags: u32,
    name_map_offset: i32,
    import_map_offset: i32,
    export_map_offset: i32,
    export_bundle_offset: i32,
    graph_data_offset: i32,
    graph_data_size: i32,
    bulk_data_start_offset: i32,
    global_import_index: i32,
    padding: i32
}

impl SummaryImpl for Type1 {
    fn get_package_flags(&self) -> u32 {
        self.package_flags
    }

    fn get_import_map_offset(&self) -> i32 {
        self.import_map_offset
    }

    fn get_export_map_offset(&self) -> i32 {
        self.export_map_offset
    }

    fn get_export_bundle_offset(&self) -> i32 {
        self.export_bundle_offset
    }
}

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Deserialize, Serialize)]
pub struct Type2 { // 4.25+, 4.26-4.27
    name: FMappedName,
    source_name: FMappedName,
    package_flags: u32,
    cooked_header_size: u32,
    name_map_names_offset: i32,
    name_map_names_size: i32,
    name_map_hashes_offset: i32,
    name_map_hashes_size: i32,
    import_map_offset: i32,
    export_map_offset: i32,
    export_bundle_offset: i32,
    graph_data_offset: i32,
    graph_data_size: i32,
    pad: i32
}

impl SummaryImpl for Type2 {
    fn get_package_flags(&self) -> u32 {
        self.package_flags
    }

    fn get_import_map_offset(&self) -> i32 {
        self.import_map_offset
    }

    fn get_export_map_offset(&self) -> i32 {
        self.export_map_offset
    }

    fn get_export_bundle_offset(&self) -> i32 {
        self.export_bundle_offset
    }
}

impl Type2 {
    // pub fn
}

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Deserialize, Serialize)]
pub struct Type3 { // 5.0-5.2
    has_versioning_info: u32,
    header_size: u32,
    name: FMappedName,
    package_flags: u32,
    cooked_header_size: u32,
    imported_public_export_hashes_offset: i32,
    import_map_offset: i32,
    export_map_offset: i32,
    export_bundle_entries_offset: i32,
    graph_data_offset: i32
}

impl SummaryImpl for Type3 {
    fn get_package_flags(&self) -> u32 {
        self.package_flags
    }

    fn get_import_map_offset(&self) -> i32 {
        self.import_map_offset
    }

    fn get_export_map_offset(&self) -> i32 {
        self.export_map_offset
    }

    fn get_export_bundle_offset(&self) -> i32 {
        self.export_bundle_entries_offset
    }
}

#[repr(C)]
#[derive(Debug)]
#[derive(Archive, Deserialize, Serialize)]
pub struct Type4 { // 5.3+
    has_versioning_info: u32,
    header_size: u32,
    name: FMappedName,
    package_flags: u32,
    cooked_header_size: u32,
    imported_public_export_hashes_offset: i32,
    import_map_offset: i32,
    export_map_offset: i32,
    export_bundle_entries_offset: i32,
    dependency_bundle_headers_offset: i32,
    dependency_bundle_entries_offset: i32,
    imported_package_names_offset: i32
}

impl SummaryImpl for Type4 {
    fn get_package_flags(&self) -> u32 {
        self.package_flags
    }

    fn get_import_map_offset(&self) -> i32 {
        self.import_map_offset
    }

    fn get_export_map_offset(&self) -> i32 {
        self.export_map_offset
    }

    fn get_export_bundle_offset(&self) -> i32 {
        self.export_bundle_entries_offset
    }
}

#[repr(u8)]
#[derive(Debug, Clone, Copy, Ord, PartialOrd, Eq, PartialEq)]
pub enum ZenVersion {
    Initial,
    DataResourceTable, // 5.2
    ImportedPackageNames, // 5.3
    ExportDependencies // 5.3
}


#[repr(C)]
#[derive(Debug)]
struct ZenVersioningInfo {
    zen_version: ZenVersion,
    package_version: u8, // FPackageFileVersion
    licensee_version: i32,
    custom_versions: u8 // FCustomVersionContainer
}

/// # Important!
/// Since the crate uses global state shared between threads, tests must only be run single-threaded
/// (append -- --test-threads=1 to your cargo test command)
#[cfg(test)]
mod tests {
    use rkyv::rancor::Error;
    use crate::global::EngineVersion;
    use crate::serial::package::summary::{ArchivedType2, Type2, ArchivedType3, Type3, ArchivedType4, Type4};
    use crate::string::FMappedName;

    #[test]
    fn read_summary_persona3reload_spr_ui_battle_result() {
        crate::util::tests::setup(EngineVersion::UE_4_27);
        let path = std::env::current_dir().unwrap().join("assets/P3R/SPR_UI_Battle_Result.uasset");
        if !path.exists() {
            return;
        }
        let file = std::fs::read(path).unwrap();
        let header = rkyv::access::<ArchivedType2, Error>(&file[..size_of::<ArchivedType2>()]).unwrap();
        let header = rkyv::deserialize::<Type2, Error>(header).unwrap();
        assert_eq!(FMappedName::new(0, 0), header.name);
        assert_eq!(FMappedName::new(0, 0), header.source_name);
        assert_eq!(0x80000000, header.package_flags);
        assert_eq!(1516, header.cooked_header_size);
        assert_eq!(64, header.name_map_names_offset);
        assert_eq!(680, header.name_map_names_size);
        assert_eq!(744, header.name_map_hashes_offset);
        assert_eq!(304, header.name_map_hashes_size);
        assert_eq!(1048, header.import_map_offset);
        assert_eq!(1120, header.export_map_offset);
        assert_eq!(1192, header.export_bundle_offset);
        assert_eq!(1224, header.graph_data_offset);
        assert_eq!(784, header.graph_data_size);
        assert_eq!(0, header.pad);
    }

    #[test]
    fn read_summary_dragonballsparkingzero_sk_0000_00_01_base() {
        crate::util::tests::setup(EngineVersion::UE_5_1);
        let path = std::env::current_dir().unwrap().join("assets/DSZ/SK_0000_00_01_BASE.uasset");
        if !path.exists() {
            return;
        }
        let file = std::fs::read(path).unwrap();
        let header = rkyv::access::<ArchivedType3, Error>(&file[..size_of::<ArchivedType3>()]).unwrap();
        let header = rkyv::deserialize::<Type3, Error>(header).unwrap();
        println!("{:?}", header);
    }

    #[test]
    fn read_summary_sonicracingcrossworlds_bp_driver_pawn() {
        crate::util::tests::setup(EngineVersion::UE_5_4);
        let path = std::env::current_dir().unwrap().join("assets/SRC/BP_DriverPawn.uasset");
        if !path.exists() {
            return;
        }
        let file = std::fs::read(path).unwrap();
        let header = rkyv::access::<ArchivedType4, Error>(&file[..size_of::<ArchivedType4>()]).unwrap();
        let header = rkyv::deserialize::<Type4, Error>(header).unwrap();
        println!("{:?}", header);
    }
}