use crate::global::set_toc_version;
use crate::logger::{invoke_println, RELOADED_LOGGER};
use crate::serial::chunk::ctype::{Types, CHUNK_ID_VERSION};
use crate::serial::container::id::CONTAINER_HASH;
use crate::serial::header::version::{IoStoreTocVersion, TOC_VERSION};

#[allow(mutable_transmutes)]
pub(crate) fn setup(v: IoStoreTocVersion, t: Types) {
    if RELOADED_LOGGER.get().is_none() {
        let _ = RELOADED_LOGGER.set(invoke_println);
    }
    match CONTAINER_HASH.get() {
        Some(_) => {
            // force change TOC_VERSION and CHUNK_ID_VERSION
            *unsafe { std::mem::transmute::<_, &mut IoStoreTocVersion>(TOC_VERSION.get().unwrap()) } = v;
            *unsafe { std::mem::transmute::<_, &mut Types>(CHUNK_ID_VERSION.get().unwrap()) } = t;
        },
        None => {
            unsafe { set_toc_version(v, t) };
        }
    }
}