//! # FIoContainerId

use std::fmt::{Formatter, LowerHex};
use std::ops::Deref;
use std::sync::OnceLock;
use crate::string::Hasher16;

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

impl From<u64> for ContainerId {
    fn from(value: u64) -> Self {
        Self(value)
    }
}

impl ContainerId {
    pub fn from_string(s: &str) -> Self {
        Self(Hasher16::get_cityhash64(s))
    }
}

pub(crate) static CONTAINER_HASH: OnceLock<ContainerId> = OnceLock::new();