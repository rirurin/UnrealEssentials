//! FCulturePackageMap/FIoContainerHeaderLocalizedPackage

use std::collections::HashMap;
use crate::serial::container::redirect::Type1 as RedirectionType1;
use crate::serial::package::id::PackageId;
use crate::string::FMappedName;

#[derive(Debug)]
pub struct Type1(HashMap<String, RedirectionType1>); // 4.25+, 4.26-4.27

#[derive(Debug)]
pub struct Type2(Vec<(PackageId, FMappedName)>); // 5.0

/// # Important!
/// Since the crate uses global state shared between threads, tests must only be run single-threaded
/// (append -- --test-threads=1 to your cargo test command)
#[cfg(test)]
mod tests {
}