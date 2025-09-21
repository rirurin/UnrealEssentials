use crate::serial::package::id::PackageId;
use crate::string::FMappedName;

#[derive(Debug)]
pub struct Type1(Vec<(PackageId, PackageId)>); // 4.25+, 4.26-4.27

#[derive(Debug)]
pub struct Type2Entry {
    source_package_id: PackageId,
    target_package_id: PackageId,
    source_package_name: FMappedName
}

#[derive(Debug)]
pub struct Type2(Vec<Type2Entry>); // 5.0