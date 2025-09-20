use std::error::Error;
use std::fmt::{Display, Formatter};

#[derive(Debug)]
pub enum TocHeaderError {
    InvalidTocType(u8)
}

impl Error for TocHeaderError {}
impl Display for TocHeaderError {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:?}", self)
    }
}