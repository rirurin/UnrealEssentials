use std::{
    fs,
    io::Cursor
};

use crate::io_toc::{ IoStoreTocHeaderCommon, IoStoreTocHeaderType2, IoStoreTocHeaderType3, IoStoreTocHeaderType4, IoStoreTocVersion };

pub fn get_toc_filenames(toc_path: &str, version: u32) {
    let open_file = fs::read(toc_path).unwrap();
    println!("Successfully opened file of {}, size {}", toc_path, open_file.len());
    let mut open_file_cursor = Cursor::new(open_file);
    let header = match IoStoreTocVersion::from(version as u8) {
        IoStoreTocVersion::Initial => panic!("4.25 UTOC is not supported"),
        IoStoreTocVersion::DirectoryIndex => IoStoreTocHeaderType2::from_buffer(&mut open_file_cursor),
        IoStoreTocVersion::PartitionSize => IoStoreTocHeaderType3::from_buffer(&mut open_file_cursor),
        IoStoreTocVersion::PerfectHash | 
        IoStoreTocVersion::PerfectHashWithOverflow => panic!("UE 5 is not supported"),
        IoStoreTocVersion::Invalid => panic!("Got invalid version number")
    };
}