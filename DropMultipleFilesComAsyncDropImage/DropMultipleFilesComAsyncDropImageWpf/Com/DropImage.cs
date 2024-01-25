using System.Windows;

namespace DropMultipleFilesComAsyncIconWpf.Com
{
    public enum DropImageType : int
    {
        DROPIMAGE_INVALID = -1,
        DROPIMAGE_NONE = 0,
        DROPIMAGE_COPY = DragDropEffects.Copy,
        DROPIMAGE_MOVE = DragDropEffects.Move,
        DROPIMAGE_LINK = DragDropEffects.Link,
        DROPIMAGE_LABEL = 6,
        DROPIMAGE_WARNING = 7,
        DROPIMAGE_NOIMAGE = 8,
    }
}
