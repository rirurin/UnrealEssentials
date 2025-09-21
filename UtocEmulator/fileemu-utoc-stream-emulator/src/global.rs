use crate::log;
use crate::serial::chunk::ctype::{Types as ChunkIdType, Types, CHUNK_ID_VERSION};
use crate::serial::container::id::{ContainerId, CONTAINER_HASH};
use crate::serial::header::version::{IoStoreTocVersion, TOC_VERSION};

#[repr(u32)]
#[derive(Debug, Clone, Copy, Ord, PartialOrd, Eq, PartialEq)]
#[allow(nonstandard_style)]
/// **This must stay in sync with EngineVersion in UTOC.Emulator.Interfaces!**
pub enum EngineVersion {
    UE_4_25 = (4 << 0x8) + 25,
    UE_4_26 = (4 << 0x8) + 26, // 4.25+ (e.g Scarlet Nexus) is treated as 4.26
    UE_4_27 = (4 << 0x8) + 27,
    UE_5_0 = (5 << 0x8) + 0,
    UE_5_1 = (5 << 0x8) + 1,
    UE_5_2 = (5 << 0x8) + 2,
    UE_5_3 = (5 << 0x8) + 3,
    UE_5_4 = (5 << 0x8) + 4,
    UE_5_5 = (5 << 0x8) + 5,
    UE_5_6 = (5 << 0x8) + 6,
    UE_5_7 = (5 << 0x8) + 7,
    // if there are any games that require a special ID, then define them with [value] << 0x10
}

impl From<&EngineVersion> for IoStoreTocVersion {
    fn from(value: &EngineVersion) -> Self {
        match value {
            EngineVersion::UE_4_25 => Self::Initial,
            EngineVersion::UE_4_26 => Self::DirectoryIndex,
            EngineVersion::UE_4_27 => Self::DirectoryIndex,
            EngineVersion::UE_5_0 => Self::PerfectHashWithOverflow,
            EngineVersion::UE_5_1 => Self::PerfectHashWithOverflow,
            EngineVersion::UE_5_2 => Self::PerfectHashWithOverflow,
            EngineVersion::UE_5_3 => Self::PerfectHashWithOverflow,
            EngineVersion::UE_5_4 => Self::OnDemandMetaData,
            EngineVersion::UE_5_5 => Self::ReplaceIoChunkHashWithIoHash,
            EngineVersion::UE_5_6 => Self::ReplaceIoChunkHashWithIoHash,
            EngineVersion::UE_5_7 => Self::ReplaceIoChunkHashWithIoHash,
        }
    }
}

impl From<&EngineVersion> for ChunkIdType {
    fn from(value: &EngineVersion) -> Self {
        match value {
            EngineVersion::UE_4_25 => Self::Type1,
            EngineVersion::UE_4_26 => Self::Type2,
            EngineVersion::UE_4_27 => Self::Type2,
            EngineVersion::UE_5_0 => Self::Type3,
            EngineVersion::UE_5_1 => Self::Type3,
            EngineVersion::UE_5_2 => Self::Type4,
            EngineVersion::UE_5_3 => Self::Type4,
            EngineVersion::UE_5_4 => Self::Type4,
            EngineVersion::UE_5_5 => Self::Type4,
            EngineVersion::UE_5_6 => Self::Type4,
            EngineVersion::UE_5_7 => Self::Type4,
        }
    }
}

impl EngineVersion {
    pub fn set_toc_version(&self) {
        let vtoc = self.into();
        let vcid = self.into();
        log!(Debug, "EngineVersion::{:?} -> TOC version {:?}, ChunkID type {:?}", self, vtoc, vcid);
        let _ = TOC_VERSION.set(vtoc);
        let _ = CHUNK_ID_VERSION.set(vcid);
        let _ = CONTAINER_HASH.set(ContainerId::from_string("Game"));
    }

    #[allow(mutable_transmutes)]
    pub(crate) unsafe fn force_change_toc_version(&self) {
        let vtoc = self.into();
        let vcid = self.into();
        log!(Debug, "EngineVersion::{:?} -> TOC version {:?}, ChunkID type {:?}", self, vtoc, vcid);
        *unsafe { std::mem::transmute::<_, &mut IoStoreTocVersion>(TOC_VERSION.get().unwrap()) } = vtoc;
        *unsafe { std::mem::transmute::<_, &mut Types>(CHUNK_ID_VERSION.get().unwrap()) } = vcid;
    }
}

#[no_mangle]
pub unsafe extern "C" fn set_toc_version(ver: EngineVersion) {
    ver.set_toc_version();
}