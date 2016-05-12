using System;
using System.Runtime.InteropServices;

namespace ProcessTools.Windows
{
    [Flags]
    internal enum AccessFlags : uint
    {
        GenericRead = 0x80000000,
    }

    internal partial class Interop
    {
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);
    }
}
