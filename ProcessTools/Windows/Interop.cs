using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct ENUM_SERVICE_STATUS_PROCESS
    {
        internal static readonly int SizePack4 = Marshal.SizeOf(typeof(ENUM_SERVICE_STATUS_PROCESS));

        /// <summary>
        /// sizeof(ENUM_SERVICE_STATUS_PROCESS) allow Packing of 8 on 64 bit machines
        /// </summary>
        internal static readonly int SizePack8 = Marshal.SizeOf(typeof(ENUM_SERVICE_STATUS_PROCESS)) + 4;

        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        internal string pServiceName;

        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        internal string pDisplayName;

        internal SERVICE_STATUS_PROCESS ServiceStatus;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_STATUS_PROCESS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
        public uint processId;
        public int serviceFlags;
    }

    [Flags]
    internal enum ServiceType : uint
    {
        KernelDriver = 0x1,
        FileSystemDriver = 0x2,
        Adapter = 0x4,
        RecognizerDriver = 0x8,
        Win32OwnProcess = 0x10,
        Win32ShareProcess = 0x20,
        InteractiveProcess = 0x100,
        Driver = (KernelDriver | FileSystemDriver | RecognizerDriver),
        Win32 = (Win32OwnProcess | Win32ShareProcess)
    }

    [Flags]
    internal enum ServiceStateRequest : uint
    {
        Active = 0x1,
        Inactive = 0x2,
        All = (Active | Inactive)
    }

    [Flags]
    internal enum ServiceControlManagerType : uint
    {
        Connect = 0x1,
        CreateService = 0x2,
        EnumerateService = 0x4,
        Lock = 0x8,
        QueryLockStatus = 0x10,
        ModifyBootConfig = 0x20,
        AllAccess = /*STANDARD_RIGHTS_REQUIRED |*/ Connect | CreateService | EnumerateService | Lock | QueryLockStatus | ModifyBootConfig
    }

    [Flags]
    internal enum ServiceEnumType : uint
    {
        Info = 0
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

        public int X
        {
            get { return Left; }
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            get { return Top; }
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public int Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public System.Drawing.Point Location
        {
            get { return new System.Drawing.Point(Left, Top); }
            set { X = value.X; Y = value.Y; }
        }

        public System.Drawing.Size Size
        {
            get { return new System.Drawing.Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static implicit operator System.Drawing.Rectangle(RECT r)
        {
            return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator RECT(System.Drawing.Rectangle r)
        {
            return new RECT(r);
        }

        public static bool operator ==(RECT r1, RECT r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RECT r1, RECT r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(RECT r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj is RECT)
                return Equals((RECT)obj);
            else if (obj is System.Drawing.Rectangle)
                return Equals(new RECT((System.Drawing.Rectangle)obj));
            return false;
        }

        public override int GetHashCode()
        {
            return ((System.Drawing.Rectangle)this).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWINFO
    {
        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public WindowStyle dwStyle;
        public WindowExtendedStyle dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public WINDOWINFO(Boolean? filler) : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
        }

    }

    [Flags]
    internal enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }

    [Flags]
    internal enum WindowExtendedStyle : uint
    {
        ToolWindow = 0x00000080
    }

    [Flags]
    internal enum WindowStyle : uint
    {
        Overlapped = 0x00000000,
        Popup = 0x80000000,
        Child = 0x40000000,
        Minimize = 0x20000000,
        Visible = 0x10000000,
        Disabled = 0x08000000,
        ClipSiblings = 0x04000000,
        ClipChildren = 0x02000000,
        Maximize = 0x01000000,
        Caption = 0x00C00000,     /* WS_BORDER | WS_DLGFRAME  */
        Border = 0x00800000,
        DlgFrame = 0x00400000,
        VScroll = 0x00200000,
        HScroll = 0x00100000,
        SysMenu = 0x00080000,
        ThickFrame = 0x00040000,
        Group = 0x00020000,
        Tabstop = 0x00010000,
    }


    internal class Interop
    {
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

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
            IntPtr hSnap = INVALID_HANDLE_VALUE;
            hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Process, 0);
            if (hSnap == IntPtr.Zero || hSnap == INVALID_HANDLE_VALUE)  // Couldn't snap, use a different method
            {
                yield break;
            }

            var autoDisposer = new AutoDispose<IntPtr>(hSnap, (handle) => CloseHandle(handle));

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
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

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

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder szBuf = new StringBuilder(1024);
            if (0 == GetWindowText(hWnd, szBuf, szBuf.Capacity))
            {
                return null;
            }
            return szBuf.ToString();
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        public static Nullable<WINDOWINFO> GetWindowInfo(IntPtr hwnd)
        {
            WINDOWINFO wi = new WINDOWINFO(true);
            if (GetWindowInfo(hwnd, ref wi))
            {
                return wi;
            }
            return null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetThreadDesktop(uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam);

        public static bool EnumDesktopWindows(EnumDesktopWindowsDelegate callback)
        {
            return EnumDesktopWindows(GetThreadDesktop(GetCurrentThreadId()), callback, IntPtr.Zero);
        }

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string machineName, string databaseName, AccessFlags dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumServicesStatusEx(IntPtr hSCManager,
            ServiceEnumType infoLevel, ServiceType dwServiceType,
            ServiceStateRequest dwServiceState, IntPtr lpServices, UInt32 cbBufSize,
            out uint pcbBytesNeeded, out uint lpServicesReturned,
            ref uint lpResumeHandle, string pszGroupName);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseServiceHandle(IntPtr hSCObject);

        public static IEnumerable<ENUM_SERVICE_STATUS_PROCESS> GetServices(
            string machineName, string databaseName, AccessFlags dwAccess,   // From OpenSCManager
            ServiceEnumType infoLevel, ServiceType dwServiceType, ServiceStateRequest dwServiceState // From EnumServicesStatusEx
            )
        {
            List<ENUM_SERVICE_STATUS_PROCESS> results = new List<ENUM_SERVICE_STATUS_PROCESS>();

            IntPtr handle = OpenSCManager(machineName, databaseName, dwAccess);
            if (handle == IntPtr.Zero)
            {
                return null;
            }

            IntPtr buf = IntPtr.Zero;

            try
            {
                uint iBytesNeeded = 0;
                uint iServicesReturned = 0;
                uint iResumeHandle = 0;

                if (!EnumServicesStatusEx(handle, infoLevel, dwServiceType, dwServiceState, IntPtr.Zero, 0, out iBytesNeeded, out iServicesReturned, ref iResumeHandle, null))
                {
                    // allocate our memory to receive the data for all the services (including the names)
                    buf = Marshal.AllocHGlobal((int)iBytesNeeded);

                    if (!EnumServicesStatusEx(handle, infoLevel, dwServiceType, dwServiceState, buf, iBytesNeeded, out iBytesNeeded, out iServicesReturned, ref iResumeHandle, null))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    ENUM_SERVICE_STATUS_PROCESS serviceStatus;

                    // check if 64 bit system which has different pack sizes
                    if (IntPtr.Size == 8)
                    {
                        long pointer = buf.ToInt64();
                        for (int i = 0; i < (int)iServicesReturned; i++)
                        {
                            serviceStatus = (ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(new IntPtr(pointer), typeof(ENUM_SERVICE_STATUS_PROCESS));
                            results.Add(serviceStatus);

                            // incremement by sizeof(ENUM_SERVICE_STATUS_PROCESS) allow Packing of 8
                            pointer += ENUM_SERVICE_STATUS_PROCESS.SizePack8;
                        }

                    }
                    else
                    {
                        int pointer = buf.ToInt32();
                        for (int i = 0; i < (int)iServicesReturned; i++)
                        {
                            serviceStatus = (ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(new IntPtr(pointer), typeof(ENUM_SERVICE_STATUS_PROCESS));
                            results.Add(serviceStatus);

                            // incremement by sizeof(ENUM_SERVICE_STATUS_PROCESS) allow Packing of 4
                            pointer += ENUM_SERVICE_STATUS_PROCESS.SizePack4;
                        }
                    }
                }
            }
            catch (Exception /*e*/)
            {
                ;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    CloseServiceHandle(handle);
                }

                if (buf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buf);
                }
            }

            return results;
        }

        [DllImport("Psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumProcesses([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds,
               UInt32 arraySizeBytes,
               [MarshalAs(UnmanagedType.U4)] out UInt32 pBytesReturned);

        public static UInt32[] EnumProcesses()
        {
            uint[] processList = new uint[1024];  // Supporting a maximum of 1024 processes at once
            uint dwBytesReturned = 0;
            for (int retries = 3; retries > 0; --retries)
            {
                if (EnumProcesses(processList, (uint)(processList.Length * sizeof(uint)), out dwBytesReturned))
                {
                    int processesFound = (int)(dwBytesReturned / sizeof(uint));
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

        public static IntPtr[] EnumProcessModules(IntPtr hProcess, uint countOfModulesToGet)
        {
            IntPtr[] hMod = new IntPtr[countOfModulesToGet];
            uint cbNeeded = 0;
            uint cbProvided = (uint)(countOfModulesToGet * IntPtr.Size);
            if (EnumProcessModules(hProcess, hMod, cbProvided, out cbNeeded))
            {
                if (cbNeeded < cbProvided)
                {
                    int toCopy = (int) (cbNeeded / IntPtr.Size);
                    IntPtr[] newResult = new IntPtr[toCopy];
                    Array.Copy(hMod, newResult, toCopy);
                    return newResult;
                }
                return hMod;
            }

            return null;
        }

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        public static bool GetModuleBaseName(IntPtr hProcess, IntPtr hModule, out string baseName)
        {
            baseName = null;
            StringBuilder szName = new StringBuilder(1024);
            if (0 != GetModuleBaseName(hProcess, hModule, szName, (uint)szName.Capacity))
            {
                baseName = szName.ToString();
                return true;
            }
            return false;
        }

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        public static bool GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, out string moduleFileName)
        {
            moduleFileName = null;
            StringBuilder szName = new StringBuilder(1024);
            if (0 != GetModuleFileNameEx(hProcess, hModule, szName, (uint)szName.Capacity))
            {
                moduleFileName = szName.ToString();
                return true;
            }
            return false;
        }
    }
}
