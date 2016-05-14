using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessTools.Windows
{
    [Flags]
    enum QueryFullProcessImageNameFlags : uint
    {
        None = 0x00000000,
        ProcessNameNative = 0x00000001,
    }

    [Flags]
    internal enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESSENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public int th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    };

    internal partial class Interop
    {
        public static AutoDispose<IntPtr> OpenProcessHandle(ProcessAccessFlags processAccess, bool bInheritHandle, int processId)
        {
            var handle = OpenProcess(processAccess, bInheritHandle, processId);
            if (handle == IntPtr.Zero || handle == INVALID_HANDLE_VALUE)
            {
                return null;
            }
            return new AutoDispose<IntPtr>(handle, (target) => CloseHandle(target));
        }

        public static IEnumerable<PROCESSENTRY32> SnapProcesses()
        {
            IntPtr snapHandle = INVALID_HANDLE_VALUE;
            snapHandle = CreateToolhelp32Snapshot(SnapshotFlags.Process, 0);
            if (snapHandle == IntPtr.Zero || snapHandle == INVALID_HANDLE_VALUE)  // Couldn't snap, use a different method
            {
                yield break;
            }

            var autoDisposer = new AutoDispose<IntPtr>(snapHandle, (handle) => CloseHandle(handle));

            PROCESSENTRY32 procEntry = new PROCESSENTRY32();
            procEntry.dwSize = (uint)Marshal.SizeOf(procEntry);
            if (Process32First(autoDisposer.Value, ref procEntry))  // Go through each of the processes
            {
                do
                {
                    yield return procEntry;
                } while (Process32Next(autoDisposer.Value, ref procEntry)); // Next process
            }
        }

        public static int[] EnumProcesses()
        {
            int[] processList = new int[1024];  // Supporting a maximum of 1024 processes at once
            uint bytesReturned = 0;
            for (int retries = 3; retries > 0; --retries)
            {
                if (EnumProcesses(processList, (uint)(processList.Length * sizeof(uint)), out bytesReturned))
                {
                    int processesFound = (int)(bytesReturned / sizeof(uint));
                    if (processesFound < processList.Length)
                    {
                        int[] result = new int[processesFound];
                        Array.Copy(processList, result, processesFound);
                        return result;
                    }

                    processList = new int[processList.Length * 2];
                }
            }
            return null;
        }

        public static bool QueryFullProcessImageName(IntPtr hProcess, out string moduleFileName)
        {
            uint nameBufferLength = 4096;
            StringBuilder nameBuffer = new StringBuilder((int)nameBufferLength);
            if (!QueryFullProcessImageName(hProcess, QueryFullProcessImageNameFlags.None, nameBuffer, ref nameBufferLength))
            {
                moduleFileName = null;
                return false;
            }
            moduleFileName = nameBuffer.ToString();
            return true;
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryFullProcessImageName(IntPtr hProcess, QueryFullProcessImageNameFlags flags, [Out] StringBuilder fullPath, ref uint fullPathCharLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("Psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumProcesses([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] int[] processIds,
               UInt32 arraySizeBytes,
               [MarshalAs(UnmanagedType.U4)] out UInt32 pBytesReturned);

    }
}
