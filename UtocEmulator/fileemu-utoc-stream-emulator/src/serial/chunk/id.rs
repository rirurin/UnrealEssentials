//! # FIoChunkId

use crate::{
    serial::chunk::{
        ctype::{ CHUNK_ID_VERSION, ChunkTypeImpl, Types as ChunkIdType },
        error::ChunkError
    },
    string::Hasher16
};
use std::{
    fmt::{ Debug, Formatter, LowerHex },
    hash::Hash,
    ops::{ Deref, DerefMut }
};
// static FIoChunkId CreateIoChunkId ...

#[repr(C)]
pub struct ChunkId {
    storage: [u8; 0xc],
}

impl Deref for ChunkId {
    type Target = [u8];
    fn deref(&self) -> &Self::Target {
        self.storage.as_slice()
    }
}

impl DerefMut for ChunkId {
    fn deref_mut(&mut self) -> &mut Self::Target {
        self.storage.as_mut_slice()
    }
}

impl Debug for ChunkId {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        <Self as LowerHex>::fmt(self, f)
    }
}

impl LowerHex for ChunkId {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        let mut id = String::with_capacity(26);
        for i in self.storage.as_slice() {
            id.push_str(&format!("{:02x}", i));
        }
        write!(f, "{}", id)
    }
}

impl ChunkId {
    pub fn from_os_path(str: &str) -> Result<Self, ChunkError> {
        let ia = str.find('/').ok_or(ChunkError::OSPathNotLongEnough)?;
        let ic = str[ia + 1..].find('/').ok_or(ChunkError::OSPathNotLongEnough)?;
        let xt = str.rfind('.').unwrap();
        let hash = Hasher16::get_cityhash64(&format!("/game{}", &str[ia + ic + 1..xt]));
        match *CHUNK_ID_VERSION.get().unwrap() {
            ChunkIdType::Type1 => {
                let ctype = Type1::extension_to_chunk_type(
                    &str[xt + 1..]).ok_or(ChunkError::InvalidFileExtension)?;
                Ok(Type1::create_chunk_id_generic(hash, 0, ctype))
            },
            ChunkIdType::Type2 => {
                let ctype = Type2::extension_to_chunk_type(
                    &str[xt + 1..]).ok_or(ChunkError::InvalidFileExtension)?;
                Ok(Type2::create_chunk_id_generic(hash, 0, ctype))
            },
            ChunkIdType::Type3 => {
                let ctype = Type3::extension_to_chunk_type(
                    &str[xt + 1..]).ok_or(ChunkError::InvalidFileExtension)?;
                Ok(Type3::create_chunk_id_generic(hash, 0, ctype))
            },
            ChunkIdType::Type4 => {
                let ctype = Type4::extension_to_chunk_type(
                    &str[xt + 1..]).ok_or(ChunkError::InvalidFileExtension)?;
                Ok(Type4::create_chunk_id_generic(hash, 0, ctype))
            },
        }
    }
}

pub trait ChunkIdImpl where Self: Sized {
    type ChunkType : ChunkTypeImpl;

    fn extension_to_chunk_type(extension: &str) -> Option<Self::ChunkType> {
        match extension {
            "uasset" | "umap" => Some(Self::ChunkType::get_export_bundle_data_id()), //.uasset, .umap
            "ubulk" => Some(Self::ChunkType::get_bulk_data()), // .ubulk
            "uptnl" => Some(Self::ChunkType::get_optional_bulk_data()), // .uptnl
            _ => return None
        }
    }

    fn create_chunk_id_generic(chunk_id: u64, chunk_index: u16, io_chunk_type: Self::ChunkType) -> ChunkId;
    fn create_package_data_chunk_id(package_id: u64) -> ChunkId {
        Self::create_chunk_id_generic(package_id, 0, Self::ChunkType::get_export_bundle_data_id())
    }

    // create_external_file_chunk_id: Package Id is FBlake3 hash of the file's contents
    // UE 5.5+ : create_bulk_data_io_chunk_id: chunk_group in data[10]
}

pub struct Type1; // 4.25
impl ChunkIdImpl for Type1 {
    type ChunkType = super::ctype::Type1;

    fn create_chunk_id_generic(chunk_id: u64, chunk_index: u16, io_chunk_type: Self::ChunkType) -> ChunkId {
        let mut out = ChunkId { storage: [0; 0xc] };
        *(<&mut [u8; 4]>::try_from(&mut out[0..4]).unwrap()) = (chunk_id as u32).to_ne_bytes();
        *(<&mut [u8; 2]>::try_from(&mut out[4..6]).unwrap()) = chunk_index.to_ne_bytes();
        out[11] = io_chunk_type.into();
        out
    }
}

pub struct Type2; // 4.25+, 4.26 - 4.27
impl ChunkIdImpl for Type2 {
    type ChunkType = super::ctype::Type2;

    fn create_chunk_id_generic(chunk_id: u64, chunk_index: u16, io_chunk_type: Self::ChunkType) -> ChunkId {
        let mut out = ChunkId { storage: [0; 0xc] };
        *(<&mut [u8; 8]>::try_from(&mut out[0..8]).unwrap()) = chunk_id.to_ne_bytes();
        *(<&mut [u8; 2]>::try_from(&mut out[8..10]).unwrap()) = chunk_index.to_ne_bytes();
        out[11] = io_chunk_type.into();
        out
    }
}

pub struct Type3; // 5.0 - 5.1
impl ChunkIdImpl for Type3 {
    type ChunkType = super::ctype::Type3;

    fn create_chunk_id_generic(chunk_id: u64, chunk_index: u16, io_chunk_type: Self::ChunkType) -> ChunkId {
        let mut out = ChunkId { storage: [0; 0xc] };
        *(<&mut [u8; 8]>::try_from(&mut out[0..8]).unwrap()) = chunk_id.to_ne_bytes();
        *(<&mut [u8; 2]>::try_from(&mut out[8..10]).unwrap()) = chunk_index.to_be_bytes(); // NETWORK_ORDER
        out[11] = io_chunk_type.into();
        out
    }
}

pub struct Type4; // 5.2+
impl ChunkIdImpl for Type4 {
    type ChunkType = super::ctype::Type4;

    fn create_chunk_id_generic(chunk_id: u64, chunk_index: u16, io_chunk_type: Self::ChunkType) -> ChunkId {
        let mut out = ChunkId { storage: [0; 0xc] };
        *(<&mut [u8; 8]>::try_from(&mut out[0..8]).unwrap()) = chunk_id.to_ne_bytes();
        *(<&mut [u8; 2]>::try_from(&mut out[8..10]).unwrap()) = chunk_index.to_be_bytes(); // NETWORK_ORDER
        out[11] = io_chunk_type.into();
        out
    }
}

/// # Important!
/// Since the crate uses global state shared between threads, tests must only be run single-threaded
/// (append -- --test-threads=1 to your cargo test command)
#[cfg(test)]
mod tests {
    use crate::global::EngineVersion;
    use crate::serial::header::version::IoStoreTocVersion;
    use crate::serial::chunk::ctype::Types;
    use super::*;

    const TEST_PATH: &'static str = "P3R/Content/Xrd777/Schedule/Data/DataAsset/Bf/BF_P3HDscheduler_04.uasset";

    /*
    // TODO
    #[test]
    fn hash_path_type1() {
        crate::util::tests::setup(EngineVersion::UE_4_25);
        let hash = ChunkId::from_os_path(TEST_PATH).unwrap();
        println!("T1: {:?}", hash);
    }
    */

    #[test]
    fn hash_path_type2() {
        crate::util::tests::setup(EngineVersion::UE_4_27);
        let hash = ChunkId::from_os_path(TEST_PATH).unwrap();
        assert_eq!(hash.storage, [152, 137, 181, 98, 75, 46, 120, 62, 0, 0, 0, 2]);
    }

    #[test]
    fn hash_path_type3() {
        crate::util::tests::setup(EngineVersion::UE_5_0);
        let hash = ChunkId::from_os_path(TEST_PATH).unwrap();
        assert_eq!(hash.storage, [152, 137, 181, 98, 75, 46, 120, 62, 0, 0, 0, 1]);
    }

    #[test]
    fn hash_path_type4() {
        crate::util::tests::setup(EngineVersion::UE_5_3);
        let hash = ChunkId::from_os_path(TEST_PATH).unwrap();
        assert_eq!(hash.storage, [152, 137, 181, 98, 75, 46, 120, 62, 0, 0, 0, 1]);
    }
}