using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Helpers
{
    internal enum NativeDialogShowState
    {
        PreShow,
        Showing,
        Closing,
        Closed
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    // ReSharper disable CSharpWarnings::CS0108
    internal enum HRESULT : long
    {
        S_FALSE = 0x0001,
        S_OK = 0x0000,
        E_INVALIDARG = 0x80070057,
        E_OUTOFMEMORY = 0x8007000E
    }

    internal class IIDGuid
    {
        // IID GUID strings for relevant COM interfaces
        internal const string IModalWindow = "b4db1657-70d7-485e-8e3e-6fcb5a5c1802";
        internal const string IFileDialog = "42f85136-db7e-439c-85f1-e4075d135fc8";
        internal const string IFileOpenDialog = "d57c7288-d4ad-4768-be02-9d969532d960";
        internal const string IFileDialogEvents = "973510DB-7D7F-452B-8975-74A85828D354";
        internal const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
        internal const string IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";
    }

    internal class CLSIDGuid
    {
        // CLSID GUID strings for relevant co-classes
        internal const string FileOpenDialog = "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7";
    }

    internal static class NativeMethods
    {
        #region General Definitions

        // Various helper constants
        internal static IntPtr NO_PARENT = IntPtr.Zero;

        #endregion

        #region File Operations Definitions

        #region Nested type: COMDLG_FILTERSPEC

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }

        #endregion

        #region Nested type: FDAP

        internal enum FDAP
        {
            BOTTOM = 0x00000000,
            TOP = 0x00000001,
        }

        #endregion

        #region Nested type: FOS

        [Flags]
        internal enum FOS : uint
        {
            OVERWRITEPROMPT = 0x00000002,
            STRICTFILETYPES = 0x00000004,
            NOCHANGEDIR = 0x00000008,
            PICKFOLDERS = 0x00000020,
            FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are file system items.
            ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            NOVALIDATE = 0x00000100,
            ALLOWMULTISELECT = 0x00000200,
            PATHMUSTEXIST = 0x00000800,
            FILEMUSTEXIST = 0x00001000,
            CREATEPROMPT = 0x00002000,
            SHAREAWARE = 0x00004000,
            NOREADONLYRETURN = 0x00008000,
            NOTESTFILECREATE = 0x00010000,
            HIDEMRUPLACES = 0x00020000,
            HIDEPINNEDPLACES = 0x00040000,
            NODEREFERENCELINKS = 0x00100000,
            DONTADDTORECENT = 0x02000000,
            FORCESHOWHIDDEN = 0x10000000,
            DEFAULTNOMINIMODE = 0x20000000
        }

        #endregion

        #region Nested type: RESPONSE

        internal enum RESPONSE
        {
            DEFAULT = 0x00000000,
            ACCEPT = 0x00000001,
            REFUSE = 0x00000002
        }

        #endregion

        #region Nested type: SIATTRIBFLAGS

        internal enum SIATTRIBFLAGS
        {
            AND = 0x00000001, // if multiple items and the attributes together.
            OR = 0x00000002, // if multiple items or the attributes together.
            APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes
        }

        #endregion

        #region Nested type: SIGDN

        internal enum SIGDN : uint
        {
            NORMALDISPLAY = 0x00000000, // SHGDN_NORMAL
            PARENTRELATIVEPARSING = 0x80018001, // SHGDN_INFOLDER | SHGDN_FORPARSING
            DESKTOPABSOLUTEPARSING = 0x80028000, // SHGDN_FORPARSING
            PARENTRELATIVEEDITING = 0x80031001, // SHGDN_INFOLDER | SHGDN_FOREDITING
            DESKTOPABSOLUTEEDITING = 0x8004c000, // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            FILESYSPATH = 0x80058000, // SHGDN_FORPARSING
            URL = 0x80068000, // SHGDN_FORPARSING
            PARENTRELATIVEFORADDRESSBAR = 0x8007c001, // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            PARENTRELATIVE = 0x80080001 // SHGDN_INFOLDER
        }

        #endregion

        #endregion

        #region KnownFolder Definitions

        // Property System structs and consts
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            internal Guid fmtid;
            internal uint pid;
        }

        #endregion
    }

#pragma warning disable 0108
    //wpffb used
    [ComImport, Guid(IIDGuid.IModalWindow), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IModalWindow
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
         PreserveSig]
        int Show([In] IntPtr parent);
    }

    // wpffb used
    [ComImport, Guid(IIDGuid.IFileDialog), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileDialog : IModalWindow
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer
        // --------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
        int Show([In] IntPtr parent);

        // IFileDialog-Specific interface members
        // --------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In] ref NativeMethods.COMDLG_FILTERSPEC rgFilterSpec);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] NativeMethods.FOS fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out NativeMethods.FOS pfos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
    }

    [ComImport, Guid(IIDGuid.IFileOpenDialog), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileOpenDialog : IFileDialog
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer
        // --------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
        int Show([In] IntPtr parent);

        // Defined on IFileDialog - repeated here due to requirements of COM interop layer
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In] ref NativeMethods.COMDLG_FILTERSPEC rgFilterSpec);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] NativeMethods.FOS fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out NativeMethods.FOS pfos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

        // Defined by IFileOpenDialog
        // ---------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
    }
#pragma warning restore 0108

    // ReSharper restore CSharpWarnings::CS0108
    // ReSharper restore FieldCanBeMadeReadOnly.Global
    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore InconsistentNaming

    // wpffb used
    [ComImport,
     Guid(IIDGuid.IFileDialogEvents),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileDialogEvents
    {
        // NOTE: some of these callbacks are cancellable - returning S_FALSE means that 
        // the dialog should not proceed (e.g. with closing, changing folder); to 
        // support this, we need to use the PreserveSig attribute to enable us to return
        // the proper HRESULT
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
        HRESULT OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
        HRESULT OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
            out NativeMethods.RESPONSE pResponse);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
            out NativeMethods.RESPONSE pResponse);
    }

    // wpffb used
    [ComImport,
     Guid(IIDGuid.IShellItem),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        // Not supported: IBindCtx
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayName([In] NativeMethods.SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
    }

    // wpffb used
    [ComImport, Guid(IIDGuid.IShellItemArray), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItemArray
    {
        // Not supported: IBindCtx
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStore([In] int flags, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyDescriptionList([In] ref NativeMethods.PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] NativeMethods.SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount(out uint pdwNumItems);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
    }

    // ---------------------------------------------------------
    // Co-class interfaces - designed to "look like" the object 
    // in the API, so that the 'new' operator can be used in a 
    // straightforward way. Behind the scenes, the C# compiler
    // morphs all 'new CoClass()' calls to 'new CoClassWrapper()'
    [ComImport, Guid(IIDGuid.IFileOpenDialog), CoClass(typeof(FileOpenDialogRCW))]
    internal interface NativeFileOpenDialog : IFileOpenDialog
    {
    }

    // ---------------------------------------------------
    // .NET classes representing runtime callable wrappers
    [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid(CLSIDGuid.FileOpenDialog)]
    internal class FileOpenDialogRCW
    {
    }

    internal class DialogHolder : IDisposable
    {
        public DialogHolder()
        {
            State = NativeDialogShowState.PreShow;
            Dialog = new NativeFileOpenDialog();
            State = NativeDialogShowState.Showing;
        }

        public NativeFileOpenDialog Dialog { get; set; }

        public NativeDialogShowState State { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            Marshal.ReleaseComObject(Dialog);
            State = NativeDialogShowState.Closed;
        }

        #endregion
    }

    public class WPFFolderBrowserDialog
    {
        // Win32 error codes
        private const int ERROR_CANCELLED = -2147023673;

        private readonly Collection<string> _fileNames;

        private bool _cancelled;
        private DialogHolder _holder;
        private Window _parentWindow;

        #region Constructors

        public WPFFolderBrowserDialog()
        {
            _fileNames = new Collection<string>();
        }

        public WPFFolderBrowserDialog(string title)
            : this()
        {
            _title = title;
        }

        #endregion

        internal void PopulateFileNames()
        {
            if (_fileNames != null)
            {
                IShellItemArray resultsArray;
                _holder.Dialog.GetResults(out resultsArray);
                uint count;
                resultsArray.GetCount(out count);

                _fileNames.Clear();
                for (int i = 0; i < count; i++)
                    _fileNames.Add(GetFileNameFromShellItem(GetShellItemAt(resultsArray, i)));

                if (count > 0)
                    FileName = _fileNames[0];
            }
        }

        private void ApplyNativeSettings()
        {
            Debug.Assert(_holder.Dialog != null, "No dialog instance to configure");

            if (_parentWindow == null && Application.Current != null)
                _parentWindow = Application.Current.MainWindow;

            _holder.Dialog.SetOptions(_flags);
            _holder.Dialog.SetTitle(_title);

            string directory = (String.IsNullOrEmpty(_fileName)) ? InitialDirectory : Path.GetDirectoryName(_fileName);

            if (directory != null)
            {
                IShellItem folder;
                SHCreateItemFromParsingName(directory, IntPtr.Zero, new Guid(IIDGuid.IShellItem), out folder);
                if (folder != null)
                    _holder.Dialog.SetFolder(folder);
            }

            if (!String.IsNullOrEmpty(_fileName))
            {
                string name = Path.GetFileName(_fileName);
                _holder.Dialog.SetFileName(name);
            }
        }

        #region Helpers

        private bool NativeDialogShowing
        {
            get
            {
                return (_holder != null) &&
                       (_holder.State == NativeDialogShowState.Showing ||
                        _holder.State == NativeDialogShowState.Closing);
            }
        }

        protected void CheckFileNamesAvailable()
        {
            if (_holder.State != NativeDialogShowState.Closed)
                throw new InvalidOperationException("Filename not available - dialog has not closed yet");
            if (_cancelled)
                throw new InvalidOperationException("Filename not available - dialog was cancelled");
            Debug.Assert(_fileNames.Count != 0,
                "FileNames empty - shouldn't happen unless dialog cancelled or not yet shown");
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void SHCreateItemFromParsingName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iIdIShellItem,
            [Out, MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem iShellItem);

        internal string GetFileNameFromShellItem(IShellItem item)
        {
            string filename;
            item.GetDisplayName(NativeMethods.SIGDN.DESKTOPABSOLUTEPARSING, out filename);
            return filename;
        }

        internal IShellItem GetShellItemAt(IShellItemArray array, int i)
        {
            IShellItem result;
            uint index = (uint)i;
            array.GetItemAt(index, out result);
            return result;
        }

        protected void ThrowIfDialogShowing(string message)
        {
            if (NativeDialogShowing)
                throw new NotSupportedException(message);
        }

        #endregion

        #region NativeDialogEventSink Nested Class

        // ReSharper disable UnusedMember.Local
        private class NativeDialogEventSink : IFileDialogEvents
        {
            private bool _firstFolderChanged = true;

            #region IFileDialogEvents Members

            public HRESULT OnFileOk(IFileDialog pfd)
            {
                CancelEventArgs args = new CancelEventArgs();
                return (args.Cancel ? HRESULT.S_FALSE : HRESULT.S_OK);
            }

            public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
            {
                return HRESULT.S_OK;
            }

            public void OnFolderChange(IFileDialog pfd)
            {
                if (_firstFolderChanged)
                    _firstFolderChanged = false;
            }

            public void OnSelectionChange(IFileDialog pfd)
            {
            }

            public void OnShareViolation(IFileDialog pfd, IShellItem psi, out NativeMethods.RESPONSE pResponse)
            {
                // Do nothing: we will ignore share violations, and don't register
                // for them, so this method should never be called
                pResponse = NativeMethods.RESPONSE.ACCEPT;
            }

            public void OnTypeChange(IFileDialog pfd)
            {
            }

            public void OnOverwrite(IFileDialog pfd, IShellItem psi, out NativeMethods.RESPONSE pResponse)
            {
                pResponse = NativeMethods.RESPONSE.ACCEPT;
            }

            #endregion
        }

        // ReSharper restore UnusedMember.Local

        #endregion

        #region Public API

        private string _fileName;

        private NativeMethods.FOS _flags = NativeMethods.FOS.NOTESTFILECREATE | NativeMethods.FOS.FORCEFILESYSTEM |
                                           NativeMethods.FOS.HIDEPINNEDPLACES |
                                           NativeMethods.FOS.DONTADDTORECENT | NativeMethods.FOS.PICKFOLDERS;

        private string _title;

        internal bool AddExtension { get; set; }

        public bool AddToMruList
        {
            get { return IsBitSet(NativeMethods.FOS.DONTADDTORECENT); }
            set { SetBit("AddToMruList", NativeMethods.FOS.DONTADDTORECENT, value); }
        }

        internal bool CheckFileExists
        {
            get { return IsBitSet(NativeMethods.FOS.FILEMUSTEXIST); }
            set { SetBit("CheckFileExists", NativeMethods.FOS.FILEMUSTEXIST, value); }
        }

        internal bool CheckPathExists
        {
            get { return IsBitSet(NativeMethods.FOS.PATHMUSTEXIST); }
            set { SetBit("CheckPathExists", NativeMethods.FOS.PATHMUSTEXIST, value); }
        }

        internal bool CheckReadOnly
        {
            get { return IsBitSet(NativeMethods.FOS.NOREADONLYRETURN); }
            set { SetBit("CheckReadOnly", NativeMethods.FOS.NOREADONLYRETURN, value); }
        }

        internal bool CheckValidNames
        {
            get { return IsBitSet(NativeMethods.FOS.NOVALIDATE); }
            set { SetBit("CheckValidNames", NativeMethods.FOS.NOVALIDATE, value); }
        }

        public bool DereferenceLinks
        {
            get { return IsBitSet(NativeMethods.FOS.NODEREFERENCELINKS); }
            set { SetBit("DereferenceLinks", NativeMethods.FOS.NODEREFERENCELINKS, value); }
        }

        public string FileName
        {
            get
            {
                CheckFileNamesAvailable();
                if (_fileNames.Count > 1)
                    throw new InvalidOperationException("Multiple files selected - the FileNames property should be used instead");
                _fileName = _fileNames[0];
                return _fileNames[0];
            }
            set { _fileName = value; }
        }

        public IEnumerable<string> FileNames
        {
            get
            {
                CheckFileNamesAvailable();
                return _fileNames;
            }
        }

        public string InitialDirectory { get; set; }

        public bool MultiSelect
        {
            get { return IsBitSet(NativeMethods.FOS.ALLOWMULTISELECT); }
            set { SetBit("MultiSelect", NativeMethods.FOS.ALLOWMULTISELECT, value); }
        }

        internal bool RestoreDirectory
        {
            get { return IsBitSet(NativeMethods.FOS.NOCHANGEDIR); }
            set { SetBit("RestoreDirectory", NativeMethods.FOS.NOCHANGEDIR, value); }
        }

        public bool ShowHiddenItems
        {
            get { return IsBitSet(NativeMethods.FOS.FORCESHOWHIDDEN); }
            set { SetBit("ShowHiddenItems", NativeMethods.FOS.FORCESHOWHIDDEN, value); }
        }

        public bool ShowPlacesList
        {
            get { return IsBitSet(NativeMethods.FOS.HIDEPINNEDPLACES); }
            set { SetBit("ShowPlacesList", NativeMethods.FOS.HIDEPINNEDPLACES, value); }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (NativeDialogShowing)
                    _holder.Dialog.SetTitle(value);
            }
        }

        private void SetBit(string name, NativeMethods.FOS bit, bool set)
        {
            ThrowIfDialogShowing(string.Format("{0} cannot be changed while dialog is showing.", name));
            if (set)
                _flags |= bit;
            else
                _flags &= ~bit;
        }

        private bool IsBitSet(NativeMethods.FOS bit)
        {
            return (_flags & bit) == bit;
        }

        public bool ShowDialog(Window owner = null)
        {
            bool result;
            if (owner != null)
                _parentWindow = owner;

            using (_holder = new DialogHolder())
            {
                // Apply outer properties to native dialog instance
                ApplyNativeSettings();

                // Show dialog
                IntPtr handle = _parentWindow == null
                    ? NativeMethods.NO_PARENT
                    : (new WindowInteropHelper(_parentWindow)).Handle;
                int hresult = _holder.Dialog.Show(handle);
                _holder.State = NativeDialogShowState.Closed;

                // Create return information
                _cancelled = hresult == ERROR_CANCELLED;
                if (_cancelled)
                    _fileNames.Clear();
                else
                    PopulateFileNames();
                result = !_cancelled;
            }
            return result;
        }

        #endregion
    }
}
