using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace DropMultipleFiles
{
    internal static class DropFileExtensions
    {
        public static void SetFileGroupDescriptor(
            this IDataObject dataObject,
            IEnumerable<DropFileInfo> files)
        {
            var fileGroupDescriptorBinary = new MemoryStream();
            fileGroupDescriptorBinary.Write(BitConverter.GetBytes(files.Count()));

            var fdSize = Marshal.SizeOf<FILEDESCRIPTOR>();
            var fdArray = new byte[fdSize];
            var fdPtr = Marshal.AllocHGlobal(fdSize);
            try
            {
                foreach (var file in files)
                {
                    var fileDescriptor = new FILEDESCRIPTORFactory().Create(
                        file.Name,
                        file.CreationTime, file.LastAccessTime, file.LastWriteTime,
                        file.Size);
                    Marshal.StructureToPtr(fileDescriptor, fdPtr, false);
                    Marshal.Copy(fdPtr, fdArray, 0, fdSize);
                    fileGroupDescriptorBinary.Write(fdArray);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(fdPtr);
            }
            fileGroupDescriptorBinary.Position = 0;
            dataObject.SetData("FileGroupDescriptorW", fileGroupDescriptorBinary);
        }

        public static void SetFileContents(
            this MyDataObject dataObject,
            IEnumerable<Stream> fileContents)
        {
            dataObject.SetFileContents(fileContents.GetEnumerator());
        }
    }
}
