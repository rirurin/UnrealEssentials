use crate::global::EngineVersion;
use crate::logger::{invoke_println, RELOADED_LOGGER};
use crate::serial::container::id::CONTAINER_HASH;

pub(crate) fn setup(v: EngineVersion) {
    if RELOADED_LOGGER.get().is_none() {
        let _ = RELOADED_LOGGER.set(invoke_println);
    }
    match CONTAINER_HASH.get() {
        Some(_) => unsafe { v.force_change_toc_version() },
        None => v.set_toc_version()
    }
}