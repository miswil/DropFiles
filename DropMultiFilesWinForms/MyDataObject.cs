namespace DropMultiFilesWinForms
{
    class MyDataObject : DataObject
    {
        private IEnumerator<Stream>? fileContents;

        public void SetFileContents(IEnumerator<Stream> fileContents)
        {
            this.fileContents = fileContents;
        }

        public override object? GetData(string format, bool autoConvert)
        {
            if (format == "FileContents" &&
                this.fileContents is not null &&
                this.fileContents.MoveNext())
            {
                return fileContents.Current;
            }
            return base.GetData(format, autoConvert);
        }

        public override bool GetDataPresent(string format, bool autoConvert)
        {
            if (format == "FileContents" &&
                this.fileContents is not null)
            {
                return true;
            }
            return base.GetDataPresent(format, autoConvert);
        }

        public override string[] GetFormats(bool autoConvert)
        {
            var formats = base.GetFormats(autoConvert);
            if (this.fileContents is not null)
            {
                return formats.Concat(new[] { "FileContents" }).ToArray();
            }
            else
            {
                return formats;
            }
        }
    }
}
