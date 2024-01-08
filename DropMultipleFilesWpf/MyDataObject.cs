using System.IO;
using System.Windows;

namespace DropMultipleFilesWpf
{
    class MyDataObject : IDataObject
    {
        private DataObject dataObject;
        private IEnumerator<Stream>? fileContents;

        public MyDataObject()
        {
            this.dataObject = new DataObject();
        }

        public void SetFileContents(IEnumerator<Stream> fileContents)
        {
            this.fileContents = fileContents;
        }

        public object GetData(Type format)
        {
            return this.GetData(format.FullName!);
        }

        public object GetData(string format)
        {
            return this.GetData(format, true);
        }

        public object GetData(string format, bool autoConvert)
        {
            if (format == "FileContents" &&
                this.fileContents is not null &&
                this.fileContents.MoveNext())
            {
                return fileContents.Current;
            }
            return this.dataObject.GetData(format, autoConvert);
        }

        public bool GetDataPresent(Type format)
        {
            return this.GetDataPresent(format.FullName!);
        }

        public bool GetDataPresent(string format)
        {
            return this.GetDataPresent(format, true);
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            if (format == "FileContents" &&
                this.fileContents is not null)
            {
                return true;
            }
            return this.dataObject.GetDataPresent(format, autoConvert);
        }

        public string[] GetFormats()
        {
            return this.GetFormats(true);
        }

        public string[] GetFormats(bool autoConvert)
        {
            var formats = this.dataObject.GetFormats(autoConvert);
            if (this.fileContents is not null)
            {
                return formats.Concat(new[] { "FileContents" }).ToArray();
            }
            else
            {
                return formats;
            }
        }

        public void SetData(object data)
        {
            this.dataObject.SetData(data);
        }

        public void SetData(string format, object data)
        {
            this.dataObject.SetData(format, data);
        }

        public void SetData(string format, object data, bool autoConvert)
        {
            this.dataObject.SetData(format, data, autoConvert);
        }

        public void SetData(Type format, object data)
        {
            this.dataObject.SetData(format, data);
        }
    }
}
