using DropMutipleFilesComAsyncWinForms.Com;

namespace DropMutipleFilesComAsyncWinForms
{
    internal interface IDroppedObjectInfo
    {
        FILEDESCRIPTOR ToFILEDESCRIPTOR();
    }

    internal record DroppedFileInfo(
        string Name,
        DateTime? CreationTime = null,
        DateTime? LastAccessTime = null,
        DateTime? LastWriteTime = null,
        long? Size = null) : IDroppedObjectInfo
    {
        public FILEDESCRIPTOR ToFILEDESCRIPTOR()
        {
            var fileDescriptor = new FILEDESCRIPTORFactory().CreateFile(
                this.Name,
                this.CreationTime, this.LastAccessTime, this.LastWriteTime,
                this.Size);
            return fileDescriptor;
        }
    }

    internal record DroppedDirectoryInfo(
        string Name,
        DateTime? CreationTime = null,
        DateTime? LastAccessTime = null,
        DateTime? LastWriteTime = null) : IDroppedObjectInfo
    {
        public FILEDESCRIPTOR ToFILEDESCRIPTOR()
        {
            var fileDescriptor = new FILEDESCRIPTORFactory().CreateDirectory(
                this.Name,
                this.CreationTime, this.LastAccessTime, this.LastWriteTime);
            return fileDescriptor;
        }
    }
}
