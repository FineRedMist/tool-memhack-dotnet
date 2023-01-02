using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessTools.Windows
{
    [Flags]
    internal enum AccessFlags : uint
    {
        GenericRead = 0x80000000,
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

    [Flags]
    internal enum SnapshotFlags : uint
    {
        HeapList = 0x00000001,
        Process = 0x00000002,
        Thread = 0x00000004,
        Module = 0x00000008,
        Module32 = 0x00000010,
        All = (HeapList | Process | Thread | Module),
        Inherit = 0x80000000,
        NoHeaps = 0x40000000

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESSENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    };



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
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

        public static AutoDispose<IntPtr>? OpenProcessHandle(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId)
        {
            var handle = OpenProcess(processAccess, bInheritHandle, processId);
            if (handle == IntPtr.Zero || handle == INVALID_HANDLE_VALUE)
            {
                return null;
            }
            return new AutoDispose<IntPtr>(handle, (target) => CloseHandle(target));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

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

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        public static AutoDispose<IntPtr>? OpenThreadHandle(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId)
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

        [DllImport("Psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumProcesses([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds,
               UInt32 arraySizeBytes,
               [MarshalAs(UnmanagedType.U4)] out UInt32 pBytesReturned);

        public static UInt32[]? EnumProcesses()
        {
            uint[] processList = new uint[1024];  // Supporting a maximum of 1024 processes at once
            uint bytesReturned = 0;
            for (int retries = 3; retries > 0; --retries)
            {
                if (EnumProcesses(processList, (uint)(processList.Length * sizeof(uint)), out bytesReturned))
                {
                    int processesFound = (int)(bytesReturned / sizeof(uint));
                    if (processesFound < processList.Length)
                    {
                        uint[] result = new uint[processesFound];
                        Array.Copy(processList, result, processesFound);
                        return result;
                    }

                    processList = new uint[processList.Length * 2];
                }
            }
            return null;
        }

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumProcessModules(IntPtr hProcess, [In][Out] IntPtr[] lphModule, uint cb, out uint lpcbNeeded);

        public static IntPtr[]? EnumProcessModules(IntPtr hProcess, uint countOfModulesToGet)
        {
            IntPtr[] moduleHandles = new IntPtr[countOfModulesToGet];
            uint bytesNeeded = 0;
            uint bytesProvided = (uint)(countOfModulesToGet * IntPtr.Size);
            if (EnumProcessModules(hProcess, moduleHandles, bytesProvided, out bytesNeeded))
            {
                if (bytesNeeded < bytesProvided)
                {
                    int toCopy = (int)(bytesNeeded / IntPtr.Size);
                    IntPtr[] newResult = new IntPtr[toCopy];
                    Array.Copy(moduleHandles, newResult, toCopy);
                    return newResult;
                }
                return moduleHandles;
            }

            return null;
        }

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        public static bool GetModuleBaseName(IntPtr hProcess, IntPtr hModule, out string? baseName)
        {
            baseName = null;
            StringBuilder nameBuffer = new StringBuilder(1024);
            if (0 != GetModuleBaseName(hProcess, hModule, nameBuffer, (uint)nameBuffer.Capacity))
            {
                baseName = nameBuffer.ToString();
                return true;
            }
            return false;
        }

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        public static bool GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, out string? moduleFileName)
        {
            moduleFileName = null;
            StringBuilder nameBuffer = new StringBuilder(1024);
            if (0 != GetModuleFileNameEx(hProcess, hModule, nameBuffer, (uint)nameBuffer.Capacity))
            {
                moduleFileName = nameBuffer.ToString();
                return true;
            }
            return false;
        }
    }
}
