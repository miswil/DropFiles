using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComWpf
{
    class MyDataObject : IComDataObject
    {
        private DataObject dataObject;
        private IList<Stream?>? fileContents;

        public MyDataObject()
        {
            this.dataObject = new DataObject();
        }

        public void SetFileContents(IList<Stream?> fileContents)
        {
            this.fileContents = fileContents;
        }

        #region System.Windows.IDataObject interfaces
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
        #endregion System.Windows.IDataObject interfaces

        #region System.Runtime.InteropServices.ComTypes.IDataObject interfaces
        int IComDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            return ((IComDataObject)this.dataObject).DAdvise(ref pFormatetc, advf, adviseSink, out connection);
        }

        void IComDataObject.DUnadvise(int connection)
        {
            ((IComDataObject)this.dataObject).DUnadvise(connection);
        }

        int IComDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise)
        {
            return ((IComDataObject)this.dataObject).EnumDAdvise(out enumAdvise);
        }

        IEnumFORMATETC IComDataObject.EnumFormatEtc(DATADIR direction)
        {
            return ((IComDataObject)this.dataObject).EnumFormatEtc(direction);
        }

        int IComDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            return ((IComDataObject)this.dataObject).GetCanonicalFormatEtc(ref formatIn, out formatOut);
        }

        void IComDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            if (DataFormats.GetDataFormat(format.cfFormat).Name == "FileContents" &&
                this.fileContents is not null)
            {
                this.dataObject.SetData("FileContents", this.fileContents[format.lindex]);
            }
            ((IComDataObject)this.dataObject).GetData(ref format, out medium);
        }

        void IComDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            if (DataFormats.GetDataFormat(format.cfFormat).Name == "FileContents" &&
                this.fileContents is not null)
            {
                this.dataObject.SetData("FileContents", this.fileContents[format.lindex]);
            }
            ((IComDataObject)this.dataObject).GetDataHere(ref format, ref medium);
        }

        int IComDataObject.QueryGetData(ref FORMATETC format)
        {
            return ((IComDataObject)this.dataObject).QueryGetData(ref format);
        }

        void IComDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            var format = DataFormats.GetDataFormat(formatIn.cfFormat).Name;
            Debug.WriteLine(format);
            ((IComDataObject)this.dataObject).SetData(ref formatIn, ref medium, release);
        }
        #endregion System.Runtime.InteropServices.ComTypes.IDataObject interfaces
    }
}
