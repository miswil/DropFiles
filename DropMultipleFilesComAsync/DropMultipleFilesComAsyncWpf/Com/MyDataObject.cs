﻿using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;


namespace DropMultipleFilesComAsyncWpf.Com
{
    class MyDataObject : IComDataObject, IDataObjectAsyncCapability, IDisposable
    {
        private static List<FORMATETC> StaticFileFormats;
        private static readonly short FileGroupDescriptorId;
        private static readonly short FileContentsId;

        private Func<Task<IEnumerable<IDroppedObjectInfo>>>? fgdFetch;
        private IList<Func<Task<Stream>>?>? fcFetches;
        private bool isAsync;
        private bool isInAsyncOperation;
        private List<ReadStream> fetchedStreams = new();

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
                NativeMethods.CreateFormatEnumerator(
                    2u,
                    StaticFileFormats.ToArray(),
                    out var enumFmtEtc);
                return enumFmtEtc;
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
                            this.AllocFileGroupDescriptorToHGlobalAsync(IntPtr.Zero).Result;
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
                        this.AllocFileGroupDescriptorToHGlobalAsync(IntPtr.Zero).Result;
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
                Marshal.ThrowExceptionForHR(NativeMethods.DV_E_FORMATETC);
            }
        }

        public int QueryGetData(ref FORMATETC format)
        {
            if (!((format.cfFormat == FileGroupDescriptorId && this.fgdFetch != null) ||
                  (format.cfFormat == FileContentsId && this.fcFetches != null)))
            {
                return NativeMethods.DV_E_FORMATETC;
            }
            if (format.dwAspect != DVASPECT.DVASPECT_CONTENT)
            {
                return NativeMethods.DV_E_DVASPECT;
            }
            if (format.cfFormat == FileGroupDescriptorId)
            {
                if (!format.tymed.HasFlag(TYMED.TYMED_HGLOBAL))
                {
                    return NativeMethods.DV_E_TYMED;
                }
                if (format.lindex != -1)
                {
                    return NativeMethods.DV_E_LINDEX;
                }
            }
            else if (format.cfFormat == FileContentsId)
            {
                if (!format.tymed.HasFlag(TYMED.TYMED_ISTREAM))
                {
                    return NativeMethods.DV_E_TYMED;
                }
                if (format.lindex < 0 || this.fcFetches!.Count <= format.lindex)
                {
                    return NativeMethods.DV_E_LINDEX;
                }
            }
            return NativeMethods.S_OK;
        }

        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
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

        public void Dispose()
        {
            foreach (var fetchedStream in this.fetchedStreams)
            {
                fetchedStream.Dispose();
            }
        }
    }
}
