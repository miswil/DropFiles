using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using IFormDataObject = System.Windows.Forms.IDataObject;


namespace DropMultipleFilesComAsyncDragImageWinForms.Com
{
    class MyDataObject : IComDataObject, IDataObjectAsyncCapability, IDisposable
    {
        private static readonly short FileGroupDescriptorId;
        private static readonly short FileContentsId;

        private Func<Task<IEnumerable<IDroppedObjectInfo>>>? fgdFetch;
        private IList<Func<Task<Stream>>?>? fcFetches;
        private bool isAsync;
        private bool isInAsyncOperation;
        private List<ReadStream> fetchedStreams = new();
        private DataObject dataObject;

        public event EventHandler<EventArgs>? AsyncOperationStart;
        public event EventHandler<EventArgs>? AsyncOperationEnd;

        static MyDataObject()
        {
            FileGroupDescriptorId = (short)DataFormats.GetFormat("FileGroupDescriptorW").Id;
            FileContentsId = (short)DataFormats.GetFormat("FileContents").Id;
        }

        public MyDataObject()
        {
            this.dataObject = new DataObject();
        }

        public void SetFileGroupDescriptorFetch(
            Func<Task<IEnumerable<IDroppedObjectInfo>>> fgdFetch)
        {
            this.fgdFetch = fgdFetch ?? throw new ArgumentNullException(nameof(fgdFetch));
        }

        public void SetFileContentFetch(
            IEnumerable<Func<Task<Stream>>?> fcFetch)
        {
            if (fcFetch is null)
            {
                throw new ArgumentNullException(nameof(fcFetch));
            }

            this.fcFetches = fcFetch.ToList();
        }

        #region System.Runtime.InteropServices.ComTypes.IDataObject
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            return ((IComDataObject)this.dataObject).DAdvise(ref pFormatetc, advf, adviseSink, out connection);
        }

        public void DUnadvise(int connection)
        {
            ((IComDataObject)this.dataObject).DUnadvise(connection);
        }

        public int EnumDAdvise(out IEnumSTATDATA? enumAdvise)
        {
            return ((IComDataObject)this.dataObject).EnumDAdvise(out enumAdvise);
        }

        public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
        {
            if (direction == DATADIR.DATADIR_GET)
            {
                var formats = ((IComDataObject)this.dataObject).EnumFormatEtc(direction);
                return new EnumDropFileFormat(formats);
            }
            if (direction == DATADIR.DATADIR_SET)
            {
                Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
            }
            return default!;
        }

        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            return ((IComDataObject)this.dataObject).GetCanonicalFormatEtc(ref formatIn, out formatOut);
        }

        public void GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            medium = new STGMEDIUM();
            var query = this.QueryGetData(ref format);
            if (query != NativeMethods.S_OK)
            {
                Marshal.ThrowExceptionForHR(query);
                return;
            }
            if (format.cfFormat == FileGroupDescriptorId)
            {
                if (this.isAsync)
                {
                    if (this.isInAsyncOperation)
                    {
                        medium.unionmember =
                            this.AllocFileGroupDescriptorToHGlobalAsync(nint.Zero).Result;
                        medium.tymed = TYMED.TYMED_HGLOBAL;
                    }
                    else
                    {
                        medium.unionmember = nint.Zero;
                        medium.tymed = TYMED.TYMED_NULL;
                    }
                }
                else
                {
                    medium.unionmember =
                        this.AllocFileGroupDescriptorToHGlobalAsync(nint.Zero).Result;
                    medium.tymed = TYMED.TYMED_HGLOBAL;
                }
            }
            else if (format.cfFormat == FileContentsId &&
                this.fcFetches![format.lindex] is not null)
            {
                var fcFetch = this.fcFetches![format.lindex];
                var stream = fcFetch!().Result;
                var comStream = new ReadStream(stream);
                this.fetchedStreams.Add(comStream);
                medium.unionmember = Marshal.GetIUnknownForObject(comStream);
                medium.tymed = TYMED.TYMED_ISTREAM;
            }
            else
            {
                ((IComDataObject)this.dataObject).GetData(ref format, out medium);
            }
        }

        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            var query = this.QueryGetData(ref format);
            if (query != NativeMethods.S_OK)
            {
                Marshal.ThrowExceptionForHR(query);
                return;
            }
            if (format.cfFormat == FileGroupDescriptorId)
            {
                if (this.isAsync)
                {
                    if (this.isInAsyncOperation)
                    {
                        medium.unionmember =
                            this.AllocFileGroupDescriptorToHGlobalAsync(medium.unionmember).Result;
                        medium.tymed = TYMED.TYMED_HGLOBAL;
                    }
                    else
                    {
                        medium.unionmember = nint.Zero;
                        medium.tymed = TYMED.TYMED_NULL;
                    }
                }
                else
                {
                    medium.unionmember =
                        this.AllocFileGroupDescriptorToHGlobalAsync(medium.unionmember).Result;
                    medium.tymed = TYMED.TYMED_HGLOBAL;
                }
            }
            else if (format.cfFormat == FileContentsId)
            {
                Marshal.ThrowExceptionForHR(NativeMethods.DV_E_FORMATETC);
            }
            else
            {
                ((IComDataObject)this.dataObject).GetDataHere(ref format, ref medium);
            }
        }

        public int QueryGetData(ref FORMATETC format)
        {
            if ((format.cfFormat == FileGroupDescriptorId && this.fgdFetch == null) ||
                (format.cfFormat == FileContentsId && this.fcFetches == null))
            {
                return NativeMethods.DV_E_FORMATETC;
            }
            else if (format.cfFormat == FileGroupDescriptorId &&
                format.dwAspect != DVASPECT.DVASPECT_CONTENT)
            {
                if (!format.tymed.HasFlag(TYMED.TYMED_HGLOBAL))
                {
                    return NativeMethods.DV_E_TYMED;
                }
                if (format.lindex != -1)
                {
                    return NativeMethods.DV_E_LINDEX;
                }
                return NativeMethods.S_OK;
            }
            else if (format.cfFormat == FileContentsId &&
                format.dwAspect != DVASPECT.DVASPECT_CONTENT)
            {
                if (!format.tymed.HasFlag(TYMED.TYMED_ISTREAM))
                {
                    return NativeMethods.DV_E_TYMED;
                }
                if (format.lindex < 0 || this.fcFetches!.Count <= format.lindex)
                {
                    return NativeMethods.DV_E_LINDEX;
                }
                return NativeMethods.S_OK;
            }
            return ((IComDataObject)this.dataObject).QueryGetData(ref format);
        }

        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            ((IComDataObject)this.dataObject).SetData(ref formatIn, ref medium, release);
        }
        #endregion System.Runtime.InteropServices.ComTypes.IDataObject

        #region IDataObjectAsyncCapability
        void IDataObjectAsyncCapability.SetAsyncMode(bool fDoOpAsync)
        {
            this.isAsync = fDoOpAsync;
        }

        void IDataObjectAsyncCapability.GetAsyncMode(out bool pfIsOpAsync)
        {
            pfIsOpAsync = this.isAsync;
        }

        void IDataObjectAsyncCapability.StartOperation(IBindCtx pbcReserved)
        {
            this.isInAsyncOperation = true;
            this.AsyncOperationStart?.Invoke(this, EventArgs.Empty);
        }

        void IDataObjectAsyncCapability.InOperation([Out][MarshalAs(UnmanagedType.Bool)] out bool pfInAsyncOp)
        {
            pfInAsyncOp = this.isInAsyncOperation;
        }

        void IDataObjectAsyncCapability.EndOperation(int hResult, IBindCtx pbcReserved, DragDropEffects dwEffects)
        {
            this.isInAsyncOperation = false;
            this.AsyncOperationEnd?.Invoke(this, EventArgs.Empty);
        }
        #endregion IDataObjectAsyncCapability

        private async Task<nint> AllocFileGroupDescriptorToHGlobalAsync(nint specifiedHMem)
        {
            var infos = (await this.fgdFetch!().ConfigureAwait(false)).ToList();
            var fdSize = Marshal.SizeOf<FILEDESCRIPTOR>();
            var fgdSize = Marshal.SizeOf<int>() + fdSize * infos.Count;

            nint handle;
            if (specifiedHMem == nint.Zero)
            {
                handle = NativeMethods.GlobalAlloc(
                    AllocFlag.GMEM_MOVEABLE,
                    fgdSize);
            }
            else
            {
                var size = NativeMethods.GlobalSize(specifiedHMem);
                if (size < fgdSize)
                {
                    Marshal.ThrowExceptionForHR(NativeMethods.STG_E_MEDIUMFULL);
                }
                handle = specifiedHMem;
            }
            try
            {
                var ptr = NativeMethods.GlobalLock(handle);
                try
                {
                    Marshal.Copy(BitConverter.GetBytes(infos.Count), 0, ptr, Marshal.SizeOf<int>());
                    ptr += Marshal.SizeOf<int>();
                    foreach (var info in infos)
                    {
                        var fd = info.ToFILEDESCRIPTOR();
                        Marshal.StructureToPtr(fd, ptr, false);
                        ptr += fdSize;
                    }
                }
                finally
                {
                    NativeMethods.GlobalUnlock(handle);
                }
                return handle;
            }
            catch
            {
                NativeMethods.GlobalFree(handle);
                throw;
            }
        }

        public void Dispose()
        {
            foreach (var fetchedStream in this.fetchedStreams)
            {
                fetchedStream.Dispose();
            }
        }

        private class EnumDropFileFormat : IEnumFORMATETC
        {
            private static List<FORMATETC> staticFileFormats =
                [
                    new FORMATETC
                    {
                        cfFormat = FileGroupDescriptorId,
                        dwAspect = DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        ptd = nint.Zero,
                        tymed = TYMED.TYMED_HGLOBAL,
                    },
                    new FORMATETC
                    {
                        cfFormat = FileContentsId,
                        dwAspect = DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        ptd = nint.Zero,
                        tymed = TYMED.TYMED_ISTREAM,
                    },
                ];

            private readonly IEnumFORMATETC dynamicFormats;
            private int index;

            public EnumDropFileFormat(IEnumFORMATETC formats)
            {
                this.dynamicFormats = formats;
            }

            public void Clone(out IEnumFORMATETC newEnum)
            {
                this.dynamicFormats.Clone(out var clone);
                var enumDropFileFormat = new EnumDropFileFormat(clone);
                enumDropFileFormat.index = this.index;
                newEnum = enumDropFileFormat;
            }

            public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
            {
                var dynamicFetched = new int[1];
                var dynamicResult = this.dynamicFormats.Next(celt, rgelt, dynamicFetched);

                FORMATETC[] staticRgelt = new FORMATETC[celt - dynamicFetched[0]];
                int celtFetched = 0;
                for (;
                    this.index < staticFileFormats.Count && celtFetched < celt - dynamicFetched[0] && celtFetched < rgelt.Length;
                    ++celtFetched, ++this.index)
                {
                    staticRgelt[celtFetched] = staticFileFormats[this.index];
                }
                for (int i = dynamicFetched[0]; i < celt; ++i)
                {
                    rgelt[i] = staticRgelt[i - dynamicFetched[0]];
                }

                if (pceltFetched?.Length > 0)
                {
                    pceltFetched[0] = dynamicFetched[0] + celtFetched;
                }

                return celt - dynamicFetched[0] == celtFetched ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }

            public int Reset()
            {
                this.index = 0;
                return NativeMethods.S_OK;
            }

            public int Skip(int celt)
            {
                if (this.index + celt < staticFileFormats.Count)
                {
                    this.index += celt;
                    return NativeMethods.S_OK;
                }
                else
                {
                    this.index = staticFileFormats.Count;
                    return NativeMethods.S_FALSE;
                }
            }
        }
    }
}
