using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace DropSingleFileWpf
{
    internal static class DropFileExtensions
    {
        public static void SetFileGroupDescriptor(
            this IDataObject dataObject,
            string fileName,
            DateTime? creationTime = null,
            DateTime? lastAccessTime = null,
            DateTime? lastWriteTime = null,
            long? fileSize = null)
        {
            var fileGroupDescriptorBinary = new MemoryStream();
            fileGroupDescriptorBinary.Write(BitConverter.GetBytes(1));

            var fileDescriptor = new FILEDESCRIPTORFactory().Create(
                fileName,
                creationTime, lastAccessTime, lastWriteTime,
                fileSize);
            var fdSize = Marshal.SizeOf<FILEDESCRIPTOR>();
            var fdPtr = Marshal.AllocHGlobal(fdSize);
            try
            {
                Marshal.StructureToPtr(fileDescriptor, fdPtr, false);
                var fdArray = new byte[fdSize];
                Marshal.Copy(fdPtr, fdArray, 0, fdSize);
                fileGroupDescriptorBinary.Write(fdArray);
            }
            finally
            {
                Marshal.FreeHGlobal(fdPtr);
            }
            fileGroupDescriptorBinary.Position = 0;
            dataObject.SetData("FileGroupDescriptorW", fileGroupDescriptorBinary);
        }

        public static void SetFileContents(
            this IDataObject dataObject,
            Stream stream)
        {
            dataObject.SetData("FileContents", stream);
        }
    }
}
