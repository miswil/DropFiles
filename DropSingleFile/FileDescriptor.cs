using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DropSingleFile
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct FILEDESCRIPTOR
    {
        [MarshalAs(UnmanagedType.U4)]
        public FileDescriptorFlags dwFlags;
        public Guid clsid;
        public Win32Size sizel;
        public Win32Point pointl;
        public uint dwFileAttributes;
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
    public enum FileDescriptorFlags : UInt32
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

    public class FILEDESCRIPTORFactory
    {
        public FILEDESCRIPTOR Create(
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
                fileDescriptor.nFileSizeHigh = (uint)(((long)fileSize) >> 32);
                fileDescriptor.nFileSizeLow = (uint)(((long)fileSize) & 0xFFFFFFFF);
                fileDescriptor.dwFlags |= FileDescriptorFlags.FD_FILESIZE;
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
