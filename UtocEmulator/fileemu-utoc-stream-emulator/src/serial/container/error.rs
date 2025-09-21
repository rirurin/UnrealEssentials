use std::error::Error;
use std::fmt::{Display, Formatter};

#[derive(Debug)]
pub enum ContainerError {
    InvalidHeaderVersion(u32),
}

impl Error for ContainerError {}
impl Display for ContainerError {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:?}", self)
    }
}