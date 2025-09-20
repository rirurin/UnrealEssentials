//! # FIoContainerId

use std::fmt::{Formatter, LowerHex};
use std::ops::Deref;
use std::sync::OnceLock;

#[repr(transparent)]
#[derive(Debug)]
pub struct ContainerId(u64);

impl Deref for ContainerId {
    type Target = u64;
    fn deref(&self) -> &Self::Target {
        &self.0
    }
}

impl LowerHex for ContainerId {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        self.0.fmt(f)
    }
}

pub(crate) static CONTAINER_HASH: OnceLock<u64> = OnceLock::new();