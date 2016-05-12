using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ProcessTools.Windows
{
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

    internal partial class Interop
    {
        public static IEnumerable<ENUM_SERVICE_STATUS_PROCESS> GetServices(
            string machineName, string databaseName, AccessFlags accessFlags,   // From OpenSCManager
            ServiceEnumType infoLevel, ServiceType serviceType, ServiceStateRequest serviceStateRequest // From EnumServicesStatusEx
            )
        {
            List<ENUM_SERVICE_STATUS_PROCESS> results = new List<ENUM_SERVICE_STATUS_PROCESS>();

            IntPtr handle = OpenSCManager(machineName, databaseName, accessFlags);
            if (handle == IntPtr.Zero)
            {
                return null;
            }

            IntPtr serviceStatusBuffer = IntPtr.Zero;

            try
            {
                uint bytesNeeded = 0;
                uint servicesReturned = 0;
                uint resumeHandle = 0;

                if (!EnumServicesStatusEx(handle, infoLevel, serviceType, serviceStateRequest, IntPtr.Zero, 0, out bytesNeeded, out servicesReturned, ref resumeHandle, null))
                {
                    // allocate our memory to receive the data for all the services (including the names)
                    serviceStatusBuffer = Marshal.AllocHGlobal((int)bytesNeeded);

                    if (!EnumServicesStatusEx(handle, infoLevel, serviceType, serviceStateRequest, serviceStatusBuffer, bytesNeeded, out bytesNeeded, out servicesReturned, ref resumeHandle, null))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    ENUM_SERVICE_STATUS_PROCESS serviceStatus;

                    // check if 64 bit system which has different pack sizes
                    if (IntPtr.Size == 8)
                    {
                        long pointer = serviceStatusBuffer.ToInt64();
                        for (int i = 0; i < (int)servicesReturned; i++)
                        {
                            serviceStatus = (ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(new IntPtr(pointer), typeof(ENUM_SERVICE_STATUS_PROCESS));
                            results.Add(serviceStatus);

                            // incremement by sizeof(ENUM_SERVICE_STATUS_PROCESS) allow Packing of 8
                            pointer += ENUM_SERVICE_STATUS_PROCESS.SizePack8;
                        }

                    }
                    else
                    {
                        int pointer = serviceStatusBuffer.ToInt32();
                        for (int i = 0; i < (int)servicesReturned; i++)
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

                if (serviceStatusBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(serviceStatusBuffer);
                }
            }

            return results;
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
    }
}
