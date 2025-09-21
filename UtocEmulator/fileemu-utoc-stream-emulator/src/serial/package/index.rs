use std::ops::Deref;
use rkyv::{Archive, Deserialize, Serialize};

#[repr(transparent)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[derive(Archive, Serialize, Deserialize)]
pub struct SerialPackageIndex(i32);

impl Deref for SerialPackageIndex {
    type Target = i32;
    fn deref(&self) -> &Self::Target {
        &self.0
    }
}

impl From<i32> for SerialPackageIndex {
    fn from(value: i32) -> Self {
        Self(value)
    }
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum PackageIndex {
    Import(i32),
    Export(i32),
    None
}

impl From<SerialPackageIndex> for PackageIndex {
    fn from(index: SerialPackageIndex) -> Self {
        match index {
            i if *index < 0 => Self::Import(-*i - 1),
            i if *index > 0 => Self::Export(*i - 1),
            _ => Self::None
        }
    }
}