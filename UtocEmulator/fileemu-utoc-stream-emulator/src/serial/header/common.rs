use bitflags::bitflags;

pub(crate) const IO_STORE_TOC_MAGIC: [u8; 0x10] = *b"-==--==--==--==-";

bitflags! {
    struct IoContainerFlags : u8 {
        const NoFlags = 0;
        const Compressed = 1 << 0;
        const Encrypted = 1 << 1;
        const Signed = 1 << 2;
        const Indexed = 1 << 3;
        const OnDemand = 1 << 4;
    }
}