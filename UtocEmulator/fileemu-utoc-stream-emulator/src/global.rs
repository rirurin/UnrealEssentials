use crate::log;
use crate::serial::chunk::ctype::{Types as ChunkIdType, CHUNK_ID_VERSION};
use crate::serial::container::id::CONTAINER_HASH;
use crate::serial::header::version::{IoStoreTocVersion, TOC_VERSION};
use crate::string::Hasher16;

#[no_mangle]
pub unsafe extern "C" fn set_toc_version(ver: IoStoreTocVersion, chunk: ChunkIdType) {
    log!(Debug, "Using TOC version {:?}, Chunk ID type {:?}", ver, chunk);
    let _ = TOC_VERSION.set(ver);
    let _ = CHUNK_ID_VERSION.set(chunk);
    let _ = CONTAINER_HASH.set(Hasher16::get_cityhash64("Game"));
}