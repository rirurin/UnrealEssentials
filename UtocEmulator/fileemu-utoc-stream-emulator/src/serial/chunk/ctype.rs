//! # EIoChunkType

use std::sync::OnceLock;
use crate::serial::chunk::error::ChunkError;

pub trait ChunkTypeImpl {
    fn get_export_bundle_data_id() -> Self;
    fn get_container_header_id() -> Self;
    fn get_bulk_data() -> Self;
    fn get_optional_bulk_data() -> Self;
}

#[repr(u8)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum Types {
    Type1 = 1,
    Type2,
    Type3,
    Type4
}

pub(crate) static CHUNK_ID_VERSION: OnceLock<Types> = OnceLock::new();

#[repr(u8)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum Type1 { // 4.25
	Invalid,
	InstallManifest,
	ExportBundleData,
	BulkData,
	OptionalBulkData,
	LoaderGlobalMeta,
	LoaderInitialLoadMeta,
	LoaderGlobalNames,
	LoaderGlobalNameHashes
}

impl From<Type1> for u8 {
    fn from(val: Type1) -> u8 {
        val as u8
    }
}

impl TryFrom<u8> for Type1 {
    type Error = ChunkError;
    fn try_from(val: u8) -> Result<Self, Self::Error> {
        if val <= Self::LoaderGlobalNameHashes as u8 {
            Ok(unsafe { std::mem::transmute(val)})
        } else {
            Err(ChunkError::InvalidChunkType(val))
        }
    }
}

impl ChunkTypeImpl for Type1 {
    fn get_export_bundle_data_id() -> Self {
        Self::ExportBundleData
    }
    fn get_container_header_id() -> Self {
        Self::Invalid
    }
    fn get_bulk_data() -> Self {
        Self::BulkData
    }
    fn get_optional_bulk_data() -> Self {
        Self::OptionalBulkData
    }
}

#[repr(u8)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum Type2 { // 4.25+, 4.26 - 4.27
    Invalid,
    InstallManifest,
    ExportBundleData,
    BulkData,
    OptionalBulkData,
    MemoryMappedBulkData,
    LoaderGlobalMeta,
    LoaderInitialLoadMeta,
    LoaderGlobalNames,
    LoaderGlobalNameHashes,
    ContainerHeader
}

impl From<Type2> for u8 {
    fn from(val: Type2) -> u8 {
        val as u8
    }
}

impl TryFrom<u8> for Type2 {
    type Error = ChunkError;
    fn try_from(val: u8) -> Result<Self, Self::Error> {
        if val <= Self::ContainerHeader as u8 {
            Ok(unsafe { std::mem::transmute(val)})
        } else {
            Err(ChunkError::InvalidChunkType(val))
        }
    }
}

impl ChunkTypeImpl for Type2 {
    fn get_export_bundle_data_id() -> Self {
        Self::ExportBundleData
    }
    fn get_container_header_id() -> Self {
        Self::ContainerHeader
    }
    fn get_bulk_data() -> Self {
        Self::BulkData
    }
    fn get_optional_bulk_data() -> Self {
        Self::OptionalBulkData
    }
}

#[repr(u8)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum Type3 { // 5.0 - 5.1
    Invalid = 0,
    ExportBundleData = 1,
    BulkData = 2,
    OptionalBulkData = 3,
    MemoryMappedBulkData = 4,
    ScriptObjects = 5,
    ContainerHeader = 6,
    ExternalFile = 7,
    ShaderCodeLibrary = 8,
    ShaderCode = 9,
    PackageStoreEntry = 10,
    DerivedData = 11,
    EditorDerivedData = 12,
}

impl From<Type3> for u8 {
    fn from(val: Type3) -> u8 {
        val as u8
    }
}

impl TryFrom<u8> for Type3 {
    type Error = ChunkError;
    fn try_from(val: u8) -> Result<Self, Self::Error> {
        if val <= Self::EditorDerivedData as u8 {
            Ok(unsafe { std::mem::transmute(val)})
        } else {
            Err(ChunkError::InvalidChunkType(val))
        }
    }
}

impl ChunkTypeImpl for Type3 {
    fn get_export_bundle_data_id() -> Self {
        Self::ExportBundleData
    }
    fn get_container_header_id() -> Self {
        Self::ContainerHeader
    }
    fn get_bulk_data() -> Self {
        Self::BulkData
    }
    fn get_optional_bulk_data() -> Self {
        Self::OptionalBulkData
    }
}

#[repr(u8)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum Type4 { // 5.2+
    Invalid = 0,
    ExportBundleData = 1,
    BulkData = 2,
    OptionalBulkData = 3,
    MemoryMappedBulkData = 4,
    ScriptObjects = 5,
    ContainerHeader = 6,
    ExternalFile = 7,
    ShaderCodeLibrary = 8,
    ShaderCode = 9,
    PackageStoreEntry = 10,
    DerivedData = 11,
    EditorDerivedData = 12,
    PackageResource = 13,
}

impl From<Type4> for u8 {
    fn from(val: Type4) -> u8 {
        val as u8
    }
}

impl TryFrom<u8> for Type4 {
    type Error = ChunkError;
    fn try_from(val: u8) -> Result<Self, Self::Error> {
        if val <= Self::PackageResource as u8 {
            Ok(unsafe { std::mem::transmute(val)})
        } else {
            Err(ChunkError::InvalidChunkType(val))
        }
    }
}

impl ChunkTypeImpl for Type4 {
    fn get_export_bundle_data_id() -> Self {
        Self::ExportBundleData
    }
    fn get_container_header_id() -> Self {
        Self::ContainerHeader
    }
    fn get_bulk_data() -> Self {
        Self::BulkData
    }
    fn get_optional_bulk_data() -> Self {
        Self::OptionalBulkData
    }
}