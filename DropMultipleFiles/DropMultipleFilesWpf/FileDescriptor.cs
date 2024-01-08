using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DropMultipleFilesWpf
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FILEDESCRIPTOR
    {
        [MarshalAs(UnmanagedType.U4)]
        public FileDescriptorFlags dwFlags;
        public Guid clsid;
        public Win32Size sizel;
        public Win32Point pointl;
        [MarshalAs(UnmanagedType.U4)]
        public FileAttributes dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Size
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public int x;
        public int y;
    }

    [Flags]
    public enum FileDescriptorFlags : uint
    {
        FD_CLSID = 0x00000001,
        FD_SIZEPOINT = 0x00000002,
        FD_ATTRIBUTES = 0x00000004,
        FD_CREATETIME = 0x00000008,
        FD_ACCESSTIME = 0x00000010,
        FD_WRITESTIME = 0x00000020,
        FD_FILESIZE = 0x00000040,
        FD_PROGRESSUI = 0x00004000,
        FD_LINKUI = 0x00008000,
        FD_UNICODE = 0x80000000, //Windows Vista and later
    }

    [Flags]
    public enum FileAttributes : uint
    {
        /// <summary>
        /// A file that is read-only. Applications can read the file, but cannot write to it or delete it. This attribute is not honored on directories.
        /// </summary>
        FILE_ATTRIBUTE_READONLY = 0x00000001,
        /// <summary>
        /// The file or directory is hidden. It is not included in an ordinary directory listing.
        /// </summary>
        FILE_ATTRIBUTE_HIDDEN = 0x00000002,
        /// <summary>
        /// A file or directory that the operating system uses a part of, or uses exclusively.
        /// </summary>
        FILE_ATTRIBUTE_SYSTEM = 0x00000004,
        /// <summary>
        /// The handle that identifies a directory.
        /// </summary>
        FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
        /// <summary>
        /// A file or directory that is an archive file or directory. Applications typically use this attribute to mark files for backup or removal.
        /// </summary>
        FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
        /// <summary>
        /// This value is reserved for system use.
        /// </summary>
        FILE_ATTRIBUTE_DEVICE = 0x00000040,
        /// <summary>
        /// A file that does not have other attributes set. This attribute is valid only when used alone.
        /// </summary>
        FILE_ATTRIBUTE_NORMAL = 0x00000080,
        /// <summary>
        /// A file that is being used for temporary storage. File systems avoid writing data back to mass storage if sufficient cache memory is available, because typically, an application deletes a temporary file after the handle is closed. In that scenario, the system can entirely avoid writing the data. Otherwise, the data is written after the handle is closed.
        /// </summary>
        FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
        /// <summary>
        /// A file that is a sparse file.
        /// </summary>
        FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
        /// <summary>
        /// A file or directory that has an associated reparse point, or a file that is a symbolic link.
        /// </summary>
        FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
        /// <summary>
        /// A file or directory that is compressed. For a file, all of the data in the file is compressed. For a directory, compression is the default for newly created files and subdirectories.
        /// </summary>
        FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
        /// <summary>
        /// The data of a file is not available immediately. This attribute indicates that the file data is physically moved to offline storage. This attribute is used by Remote Storage, which is the hierarchical storage management software. Applications should not arbitrarily change this attribute.
        /// </summary>
        FILE_ATTRIBUTE_OFFLINE = 0x00001000,
        /// <summary>
        /// The file or directory is not to be indexed by the content indexing service.
        /// </summary>
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
        /// <summary>
        /// 	A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for newly created files and subdirectories.
        /// </summary>
        FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,
        /// <summary>
        /// The directory or user data stream is configured with integrity (only supported on ReFS volumes). It is not included in an ordinary directory listing. The integrity setting persists with the file if it's renamed. If a file is copied the destination file will have integrity set if either the source file or destination directory have integrity set. 
        /// Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This flag is not supported until Windows Server 2012.
        /// </summary>
        FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x00008000,
        /// <summary>
        /// This value is reserved for system use.
        /// </summary>
        FILE_ATTRIBUTE_VIRTUAL = 0x00010000,
        /// <summary>
        /// The user data stream not to be read by the background data integrity scanner (AKA scrubber). When set on a directory it only provides inheritance. This flag is only supported on Storage Spaces and ReFS volumes. It is not included in an ordinary directory listing.
        /// Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This flag is not supported until Windows 8 and Windows Server 2012.
        /// </summary>
        FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x00020000,
        /// <summary>
        /// A file or directory with extended attributes.
        /// </summary>
        FILE_ATTRIBUTE_EA = 0x00040000,
        /// <summary>
        /// This attribute indicates user intent that the file or directory should be kept fully present locally even when not being actively accessed. This attribute is for use with hierarchical storage management software.
        /// </summary>
        FILE_ATTRIBUTE_PINNED = 0x00080000,
        /// <summary>
        /// This attribute indicates that the file or directory should not be kept fully present locally except when being actively accessed. This attribute is for use with hierarchical storage management software.
        /// </summary>
        FILE_ATTRIBUTE_UNPINNED = 0x00100000,
        /// <summary>
        /// This attribute only appears in directory enumeration classes (FILE_DIRECTORY_INFORMATION, FILE_BOTH_DIR_INFORMATION, etc.). When this attribute is set, it means that the file or directory has no physical representation on the local system; the item is virtual. Opening the item will be more expensive than normal, e.g. it will cause at least some of it to be fetched from a remote store.
        /// </summary>
        FILE_ATTRIBUTE_RECALL_ON_OPEN = 0x00040000,
        /// <summary>
        /// When this attribute is set, it means that the file or directory is not fully present locally. For a file that means that not all of its data is on local storage (e.g. it may be sparse with some data still in remote storage). For a directory it means that some of the directory contents are being virtualized from another location. Reading the file / enumerating the directory will be more expensive than normal, e.g. it will cause at least some of the file/directory content to be fetched from a remote store. Only kernel-mode callers can set this bit.
        /// </summary>
        FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 0x00400000,
    }

    internal class FILEDESCRIPTORFactory
    {
        public FILEDESCRIPTOR CreateFile(
            string fileName,
            DateTime? creationTime,
            DateTime? lastAccessTime,
            DateTime? lastWriteTime,
            long? fileSize)
        {
            if (fileName.Length > 259)
            {
                throw new ArgumentException("The file name must be shorter than 259 characters.");
            }
            var fileDescriptor = new FILEDESCRIPTOR();
            fileDescriptor.cFileName = fileName;
            fileDescriptor.dwFlags |= FileDescriptorFlags.FD_ATTRIBUTES;
            fileDescriptor.dwFlags |= FileDescriptorFlags.FD_PROGRESSUI;
            fileDescriptor.dwFlags |= FileDescriptorFlags.FD_UNICODE;
            if (creationTime is not null)
            {
                fileDescriptor.ftCreationTime = this.ToFILETIME(creationTime!.Value);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_CREATETIME;
            }
            if (lastAccessTime is not null)
            {
                fileDescriptor.ftLastAccessTime = this.ToFILETIME(lastAccessTime!.Value);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_ACCESSTIME;
            }
            if (lastWriteTime is not null)
            {
                fileDescriptor.ftLastWriteTime = this.ToFILETIME(lastWriteTime!.Value);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_WRITESTIME;
            }
            if (fileSize is not null)
            {
                fileDescriptor.nFileSizeHigh = (uint)((long)fileSize >> 32);
                fileDescriptor.nFileSizeLow = (uint)((long)fileSize & 0xFFFFFFFF);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_FILESIZE;
            }
            return fileDescriptor;
        }

        public FILEDESCRIPTOR CreateDirectory(
            string fileName,
            DateTime? creationTime,
            DateTime? lastAccessTime,
            DateTime? lastWriteTime)
        {
            if (fileName.Length > 259)
            {
                throw new ArgumentException("The file name must be shorter than 259 characters.");
            }
            var fileDescriptor = new FILEDESCRIPTOR();
            fileDescriptor.cFileName = fileName;
            fileDescriptor.dwFlags |= FileDescriptorFlags.FD_ATTRIBUTES;
            fileDescriptor.dwFlags |= FileDescriptorFlags.FD_PROGRESSUI;
            fileDescriptor.dwFlags |= FileDescriptorFlags.FD_UNICODE;
            fileDescriptor.dwFileAttributes |= FileAttributes.FILE_ATTRIBUTE_DIRECTORY;
            if (creationTime is not null)
            {
                fileDescriptor.ftCreationTime = this.ToFILETIME(creationTime!.Value);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_CREATETIME;
            }
            if (lastAccessTime is not null)
            {
                fileDescriptor.ftLastAccessTime = this.ToFILETIME(lastAccessTime!.Value);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_ACCESSTIME;
            }
            if (lastWriteTime is not null)
            {
                fileDescriptor.ftLastWriteTime = this.ToFILETIME(lastWriteTime!.Value);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_WRITESTIME;
            }
            return fileDescriptor;
        }

        private FILETIME ToFILETIME(DateTime dateTime)
        {
            long dateTimeUtc = dateTime.ToFileTimeUtc();
            var fileTime = new FILETIME();
            fileTime.dwHighDateTime = (int)(dateTimeUtc >> 32);
            fileTime.dwLowDateTime = (int)(dateTimeUtc & 0xFFFFFFFF);
            return fileTime;
        }
    }
}
