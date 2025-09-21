//! # FContainerHeader/FIoContainerHeader

use crate::serial::container::error::ContainerError;
use crate::serial::container::id::ContainerId;
use crate::serial::container::locale::{ Type1 as LocaleType1, Type2 as LocaleType2 };
use crate::serial::container::redirect::{ Type1 as RedirectType1, Type2 as RedirectType2 };
use crate::serial::container::package::{Type1 as PackageType1, Type2 as PackageType2, Type3 as PackageType3 };

#[derive(Debug)]
pub struct Type1 { // 4.25+, 4.26-4.27
    container_id: ContainerId,
    packages: Vec<PackageType1>,
    localization: LocaleType1,
    redirect: RedirectType1,
}

pub(crate) static IO_CONTAINER_HEADER_SIG: u32 = 0x496f436e;

#[repr(u32)]
#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum IoContainerHeaderVersion {
    Initial,
    LocalizedPackages = 1, // 5.0
    OptionalSegmentPackages = 2, // 5.1-5.2
    NoExportInfo = 3, // 5.3-5.4
    SoftPackageReferences = 4, // 5.5
    SoftPackageReferencesOffset = 5, // 5.6+
}

impl TryFrom<u32> for IoContainerHeaderVersion {
    type Error = ContainerError;
    fn try_from(value: u32) -> Result<Self, Self::Error> {
        if value <= Self::SoftPackageReferences as u32 {
            Ok(unsafe { std::mem::transmute(value)})
        } else {
            Err(ContainerError::InvalidHeaderVersion(value))
        }
    }
}

#[derive(Debug)]
pub struct Type2 { // 5.0
    container_id: ContainerId,
    packages: Vec<PackageType2>,
    // redirects name map
    localization: LocaleType2,
    redirect: RedirectType2,
}

#[derive(Debug)]
pub struct Type3 { // 5.1 - 5.2
    container_id: ContainerId,
    packages: Vec<PackageType2>,
    optional_packages: Vec<PackageType2>,
    // redirects name map
    localization: LocaleType2,
    redirect: RedirectType2,
}

#[derive(Debug)]
pub struct Type4 { // 5.3 - 5.4
    container_id: ContainerId,
    packages: Vec<PackageType3>,
    optional_packages: Vec<PackageType3>,
    // redirects name map
    localization: LocaleType2,
    redirect: RedirectType2,
}

/// # Important!
/// Since the crate uses global state shared between threads, tests must only be run single-threaded
/// (append -- --test-threads=1 to your cargo test command)
#[cfg(test)]
mod tests {
}