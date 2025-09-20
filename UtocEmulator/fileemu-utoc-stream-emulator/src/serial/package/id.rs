//! # FPackageId

use std::fmt::{Formatter, LowerHex};
use std::ops::Deref;
use crate::serial::package::error::PackageError;

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