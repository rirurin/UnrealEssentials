// use std::fs::{ DirEntry, File };
use walkdir::DirEntry;

#[cfg(target_family = "unix")]
use std::os::unix::fs::MetadataExt;

#[cfg(target_os = "windows")]
use std::os::windows::fs::MetadataExt;

pub struct Metadata;

impl Metadata {

    #[cfg(target_family = "unix")]
    pub fn get_object_size(fs_obj: &DirEntry) -> u64 {
        fs_obj.metadata().unwrap().file_size()
    }

    /*
    #[cfg(target_family = "unix")]
    pub fn get_file_size(fs_obj: &File) -> u64 {
        fs_obj.metadata().unwrap().file_size()
    }
    */

    #[cfg(target_os = "windows")]
    pub fn get_object_size(fs_obj: &DirEntry) -> u64 {
        fs_obj.metadata().unwrap().file_size()
    }

    /*
    #[cfg(target_os = "windows")]
    pub fn get_file_size(fs_obj: &File) -> u64 {
        fs_obj.metadata().unwrap().file_size()
    }
    */
}