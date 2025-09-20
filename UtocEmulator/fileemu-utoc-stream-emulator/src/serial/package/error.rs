use std::error::Error;
use std::fmt::{Display, Formatter};

#[derive(Debug)]
pub enum PackageError {
    OSPathNotLongEnough,
    OSPathMissingFileExtension
}

impl Error for PackageError {}
impl Display for PackageError {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:?}", self)
    }
}