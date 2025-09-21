//! # FPackageId

use std::fmt::{Formatter, LowerHex};
use std::ops::Deref;
use crate::string::Hasher16;

#[repr(transparent)]
#[derive(Debug)]
pub struct PackageId(u64);

impl Deref for PackageId {
    type Target = u64;
    fn deref(&self) -> &Self::Target {
        &self.0
    }
}

impl LowerHex for PackageId {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        self.0.fmt(f)
    }
}

impl From<u64> for PackageId {
    fn from(value: u64) -> Self {
        Self(value)
    }
}

impl PackageId {
    pub fn from_string(s: &str) -> Self {
        Self(Hasher16::get_cityhash64(s))
    }
}