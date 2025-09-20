use std::error::Error;
use std::fmt::{Display, Formatter};

#[derive(Debug)]
pub enum ChunkError {
    InvalidChunkType(u8),
    OSPathNotLongEnough,
    InvalidFileExtension
}

impl Error for ChunkError {}
impl Display for ChunkError {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:?}", self)
    }
}