namespace DropMultipleFiles
{
    internal record DropFileInfo(
        string Name,
        DateTime? CreationTime = null,
        DateTime? LastAccessTime = null,
        DateTime? LastWriteTime = null,
        long? Size = null);
}
