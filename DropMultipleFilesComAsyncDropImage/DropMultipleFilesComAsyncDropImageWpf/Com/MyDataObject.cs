using System.Collections.Frozen;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;


namespace DropMultipleFilesComAsyncIconWpf.Com
{
    class MyDataObject : IComDataObject, IDataObjectAsyncCapability, IDisposable
    {
        private static readonly List<FORMATETC> StaticFileFormats;

        private static readonly short FileGroupDescriptorId;
        private static readonly short FileContentsId;

        private List<(FORMATETC format, STGMEDIUM medium)> data = new();
        private Func<Task<IEnumerable<IDroppedObjectInfo>>>? fgdFetch;
        private IList<Func<Task<Stream>>?>? fcFetches;
        private bool isAsync;
        private bool isInAsyncOperation;
        private List<ReadStream> fetchedContents = new();

        public event EventHandler<EventArgs>? AsyncOperationStart;
        public event EventHandler<EventArgs>? AsyncOperationEnd;

        static MyDataObject()
        {
            FileGroupDescriptorId = (short)DataFormats.GetDataFormat("FileGroupDescriptorW").Id;
            FileContentsId = (short)DataFormats.GetDataFormat("FileContents").Id;
            StaticFileFormats =
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
            connection = 0;
            return NativeMethods.E_NOTIMPL;
        }

        public void DUnadvise(int connection)
        {
            Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
        }

        public int EnumDAdvise(out IEnumSTATDATA? enumAdvise)
        {
            enumAdvise = null;
            return NativeMethods.OLE_E_ADVISENOTSUPPORTED;
        }

        public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
        {
            if (direction == DATADIR.DATADIR_GET)
            {
                return new EnumDropFileFormat(this.data.Select(d => d.format).ToList());
            }
            if (direction == DATADIR.DATADIR_SET)
            {
                Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
            }
            return default!;
        }

        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            formatOut = default;
            return NativeMethods.DATA_S_SAMEFORMATETC;
        }

        public void GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            medium = new STGMEDIUM();
            this.GetDataHere(ref format, ref medium);
        }

        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            var tryGet = this.TryGetData(format, out var foundMedium);
            if (tryGet != NativeMethods.S_OK)
            {
                Marshal.ThrowExceptionForHR(tryGet);
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
                        medium.unionmember = IntPtr.Zero;
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
                var fcFetch = this.fcFetches![format.lindex];
                var stream = fcFetch!().Result;
                var comStream = new ReadStream(stream);
                this.fetchedContents.Add(comStream);
                if (medium.unionmember == IntPtr.Zero)
                {
                    medium.unionmember = Marshal.GetIUnknownForObject(comStream);
                    medium.tymed = TYMED.TYMED_ISTREAM;
                }
                else
                {
                    var destStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                    try
                    {
                        comStream.CopyTo(destStream, long.MaxValue, IntPtr.Zero, IntPtr.Zero);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(destStream);
                    }
                }
            }
            else
            {
                var hr = foundMedium.CopyTo(format.tymed, ref medium);
                if (hr != NativeMethods.S_OK)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        public int QueryGetData(ref FORMATETC format)
        {
            var result = this.TryGetData(format, out _);
            return result;
        }

        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            if (!release)
            {
                Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
            }
            var format = formatIn;
            var exists = this.data.Where(d =>
                d.format.cfFormat == format.cfFormat &&
                d.format.tymed == format.tymed &&
                d.format.dwAspect == format.dwAspect &&
                d.format.ptd == format.ptd).ToList();
            foreach (var exist in exists)
            {
                this.data.Remove(exist);
            }
            this.data.Add((formatIn, medium));
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

        private async Task<IntPtr> AllocFileGroupDescriptorToHGlobalAsync(IntPtr specifiedHMem)
        {
            var infos = (await this.fgdFetch!().ConfigureAwait(false)).ToList();
            var fdSize = Marshal.SizeOf<FILEDESCRIPTOR>();
            var fgdSize = Marshal.SizeOf<int>() + fdSize * infos.Count;

            IntPtr handle;
            if (specifiedHMem == IntPtr.Zero)
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

        private int TryGetData(FORMATETC format, out STGMEDIUM medium)
        {
            medium = new STGMEDIUM();
            var formats = this.data
                .Concat(StaticFileFormats.Select(f => (format: f, medium: default(STGMEDIUM))));

            // Check format
            var sameFormat = formats.Where(f => f.format.cfFormat == format.cfFormat);
            if (!sameFormat.Any())
            {
                return NativeMethods.DV_E_FORMATETC;
            }

            // Check aspect
            var sameAspect = sameFormat.Where(f => f.format.dwAspect == format.dwAspect);
            if (!sameAspect.Any())
            {
                return NativeMethods.DV_E_DVASPECT;
            }

            // Check medium type
            var sameMedium = sameAspect.Where(f => format.tymed.HasFlag(f.format.tymed));
            if (!sameMedium.Any())
            {
                return NativeMethods.DV_E_TYMED;
            }

            // Check index
            if (format.cfFormat == FileContentsId)
            {
                if (format.lindex < 0 || this.fcFetches!.Count <= format.lindex)
                {
                    return NativeMethods.DV_E_LINDEX;
                }
                else
                {
                    return NativeMethods.S_OK;
                }
            }
            var sameIndex = sameMedium.Where(f => f.format.lindex == format.lindex);
            if (!sameIndex.Any())
            {
                return NativeMethods.DV_E_LINDEX;
            }

            // choose first one even if some data are found.
            medium =  sameIndex.First().medium;
            return NativeMethods.S_OK;
        }

        public void Dispose()
        {
            foreach (var d in this.data)
            {
                var medium = d.medium;
                NativeMethods.ReleaseStgMedium(ref medium);
            }
            foreach (var fetchedStream in this.fetchedContents)
            {
                fetchedStream.Dispose();
            }
        }

        private class EnumDropFileFormat : IEnumFORMATETC
        {
            private int index;
            private List<FORMATETC> formats;

            public EnumDropFileFormat(List<FORMATETC> formats)
            {
                this.formats = formats;
            }

            public void Clone(out IEnumFORMATETC newEnum)
            {
                var enumDropFileFormat = new EnumDropFileFormat(this.formats);
                enumDropFileFormat.index = this.index;
                newEnum = enumDropFileFormat;
            }

            public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
            {
                int celtFetched = 0;
                var allFormats = this.formats.Concat(StaticFileFormats).ToList();
                for (;
                    this.index < allFormats.Count && celtFetched < celt && celtFetched < rgelt.Length;
                    ++celtFetched, ++this.index)
                {
                    rgelt[celtFetched] = allFormats[this.index];
                }
                if (pceltFetched?.Length > 0)
                {
                    pceltFetched[0] = celtFetched;
                }

                return celt == celtFetched ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }

            public int Reset()
            {
                this.index = 0;
                return NativeMethods.S_OK;
            }

            public int Skip(int celt)
            {
                var nFormats = StaticFileFormats.Count + this.formats.Count;
                if (this.index + celt < nFormats)
                {
                    this.index += celt;
                    return NativeMethods.S_OK;
                }
                else
                {
                    this.index = nFormats;
                    return NativeMethods.S_FALSE;
                }
            }
        }
    }
}
