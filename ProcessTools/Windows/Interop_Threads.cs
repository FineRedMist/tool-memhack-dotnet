using System;
using System.Runtime.InteropServices;

namespace ProcessTools.Windows
{
    [Flags]
    internal enum ThreadAccess : int
    {
        Terminate = (0x0001),
        SuspendResume = (0x0002),
        GetContext = (0x0008),
        SetContext = (0x0010),
        SetInformation = (0x0020),
        QueryInformation = (0x0040),
        SetThreadToken = (0x0080),
        Impersonate = (0x0100),
        DirectImpersonation = (0x0200)
    }

    internal partial class Interop
    {
        public static bool GetThreadTimes(IntPtr hThread, out DateTime creationTime, out DateTime exitTime, out DateTime kernelTime, out DateTime userTime)
        {
            long lpCreationTime, lpExitTime, lpKernelTime, lpUserTime;
            creationTime = exitTime = kernelTime = userTime = DateTime.MinValue;
            if (!GetThreadTimes(hThread, out lpCreationTime, out lpExitTime, out lpKernelTime, out lpUserTime))
            {
                return false;
            }

            creationTime = DateTime.FromFileTimeUtc(lpCreationTime);
            exitTime = DateTime.FromFileTimeUtc(lpExitTime);
            kernelTime = DateTime.FromFileTimeUtc(lpKernelTime);
            userTime = DateTime.FromFileTimeUtc(lpUserTime);

            return true;
        }

        public static AutoDispose<IntPtr> OpenThreadHandle(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId)
        {
            IntPtr handle = OpenThread(dwDesiredAccess, bInheritHandle, dwThreadId);
            if (handle == IntPtr.Zero || handle == INVALID_HANDLE_VALUE)
            {
                return null;
            }
            return new AutoDispose<IntPtr>(handle, (target) => CloseHandle(target));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime,
           out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

    }
}
