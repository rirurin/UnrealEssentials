use crate::serial::header::error::TocHeaderError;
use std::sync::OnceLock;

#[repr(u8)]
#[derive(Debug, Copy, Clone, PartialEq, PartialOrd)]
// One byte sized enum
// https://doc.rust-lang.org/nomicon/other-reprs.html#repru-repri
pub enum IoStoreTocVersion {
    Invalid = 0,
    Initial, // UE 4.25
    DirectoryIndex, // UE 4.25+/4.26
    PartitionSize, // UE 4.27
    PerfectHash,
    PerfectHashWithOverflow, // UE 5.0 - 5.3
    OnDemandMetaData, // UE 5.4
    RemovedOnDemandMetaData,
    ReplaceIoChunkHashWithIoHash // UE 5.5+
    //LatestPlusOne
}

impl From<IoStoreTocVersion> for u8 {
    fn from(val: IoStoreTocVersion) -> u8 {
        val as u8
    }
}

impl TryFrom<u8> for IoStoreTocVersion {
    type Error = TocHeaderError;
    fn try_from(val: u8) -> Result<Self, Self::Error> {
        if val <= Self::ReplaceIoChunkHashWithIoHash as u8 {
            Ok(unsafe { std::mem::transmute(val)})
        } else {
            Err(TocHeaderError::InvalidTocType(val))
        }
    }
}

pub(crate) static TOC_VERSION: OnceLock<IoStoreTocVersion> = OnceLock::new();