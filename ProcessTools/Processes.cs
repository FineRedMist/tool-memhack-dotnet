using ProcessTools.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessTools
{
    /// <summary>
    /// Provides the methods to query the processes on the system.
    /// </summary>
    public class Processes
    {
        /// <summary>
        /// Returns the current process list.
        /// </summary>
        public static SortedList<uint, ProcessInformation> GetProcessList(string idleProcessName, string systemName)
        {
            var processes = QueryToolHelp(idleProcessName, systemName);
            if (processes.Count == 0)
            {
                processes = QueryEnumProcesses(idleProcessName, systemName);
            }
            QueryWindows(processes);
            QueryServices(processes);

            return processes;
        }

        private static SortedList<uint, ProcessInformation> QueryToolHelp(string idleProcessName, string systemName)
        {
            SortedList<uint, ProcessInformation> result = new SortedList<uint, ProcessInformation>();

            IntPtr hSnap = Interop.INVALID_HANDLE_VALUE;
            hSnap = Interop.CreateToolhelp32Snapshot(SnapshotFlags.Process, 0);
            if (hSnap == IntPtr.Zero || hSnap == Interop.INVALID_HANDLE_VALUE)  // Couldn't snap, use a different method
            {
                return result;
            }

            PROCESSENTRY32 procEntry = new PROCESSENTRY32();
            procEntry.dwSize = (uint) Marshal.SizeOf(procEntry);
            if (Interop.Process32First(hSnap, ref procEntry))  // Go through each of the processes
            {
                do
                {
                    // Set up the structures
                    ProcessInformation p = new ProcessInformation(procEntry.th32ProcessID, procEntry.szExeFile);
                    if (p.ID == 0) // The idle process has the zero id, so save myself some work by special casing it
                    {
                        p.Add(new ProcessFriendlyName(idleProcessName));
                    }
                    else if (p.ID == 4)
                    {
                        p.Add(new ProcessFriendlyName(systemName));
                    }
                    GetProcessFullPath(p);
                    result[p.ID] = p;
                } while (Interop.Process32Next(hSnap, ref procEntry)); // Next process
            }

            Interop.CloseHandle(hSnap);
            return result;
        }

        private static SortedList<uint, ProcessInformation> QueryEnumProcesses(string idleProcessName, string systemName)
        {
            SortedList<uint, ProcessInformation> result = new SortedList<uint, ProcessInformation>();

            uint[] pdwList = new uint[1024];  // Supporting a maximum of 1024 processes at once
            uint dwNeeded = 0;

            if (!Interop.EnumProcesses(pdwList, (uint)(pdwList.Length * sizeof(uint)), out dwNeeded))
            {
                // dwNeeded = GetLastError();  // Mostly for debugging if I ever see it happen myself
                return result;
            }
            uint dwCount = dwNeeded / sizeof(uint);

            // Get the information for the list of processes
            for (uint i = 0; i < dwCount; ++i)
            {
                result[pdwList[i]] = GetProcessInfo(pdwList[i], idleProcessName, systemName);
            }

            return result;
        }

        private class EnumProcessWindows
        {
            SortedList<uint, ProcessInformation> mProcesses;

            public EnumProcessWindows(SortedList<uint, ProcessInformation> processes)
            {
                mProcesses = processes;
            }

            public bool EnumDesktopWindowsDelegate(IntPtr hwnd, IntPtr lParam)
            {
                uint dwProcess = 0;
                uint dwThread = Interop.GetWindowThreadProcessId(hwnd, out dwProcess);
                bool visible;
                DateTime date = DateTime.MinValue;
                string name;

                if (dwThread == 0 || dwProcess == 0)    // We somehow mystically failed to get one of the thread or process ids
                {
                    return true;
                }

                WINDOWINFO wi = new WINDOWINFO(true);

                // Get window information
                if (!Interop.GetWindowInfo(hwnd, ref wi))
                {
                    return true;
                }

                // There are some classes of windows that I don't want to display so skip them
                if (wi.dwExStyle.HasFlag(WindowExtendedStyle.ToolWindow) 
                    || 0 != (wi.dwStyle & (WindowStyle.Popup | WindowStyle.Child | WindowStyle.Disabled)))
                {
                    return true;
                }

                // Get the text of the window
                name = Interop.GetWindowText(hwnd);
                if (string.IsNullOrEmpty(name))
                {
                    return true;
                }

                // Flag whether the window is visible.  There are some that I still want to display unfortunately
                // which means I'll also have a bunch of others that I don't want to display in the list
                visible = (wi.dwStyle & (WindowStyle.Minimize | WindowStyle.Visible)) != 0;

                DateTime ftExited, ftKernel, ftUser;
                // I use the thread information to determine when the window is created.  Typically the first window
                // created is really the interesting one so I sort them based on date created
                IntPtr hThread = Interop.OpenThread(ThreadAccess.QUERY_INFORMATION, false, dwThread);
                if (hThread != IntPtr.Zero)
                {
                    Interop.GetThreadTimes(hThread, out date, out ftExited, out ftKernel, out ftUser);
                    Interop.CloseHandle(hThread);
                }

                ProcessFriendlyName s = new ProcessFriendlyName(name, date, visible);
                ProcessInformation p = null;
                if (!mProcesses.TryGetValue(dwProcess, out p))
                {
                    p = new ProcessInformation(dwProcess, string.Empty);
                    mProcesses[dwProcess] = p;
                }
                p.Add(s);

                return true;
            }
        }

        private static void QueryWindows(SortedList<uint, ProcessInformation> processes)
        {
            EnumProcessWindows enumerate = new EnumProcessWindows(processes);
            Interop.EnumDesktopWindows(Interop.GetThreadDesktop(Interop.GetCurrentThreadId()), enumerate.EnumDesktopWindowsDelegate, IntPtr.Zero);
        }

        private static void UpdateProcessInformation(SortedList<uint, ProcessInformation> processes, ENUM_SERVICE_STATUS_PROCESS serviceStatus)
        {
            uint processId = serviceStatus.ServiceStatus.processId;
            if (processId == 0)
            {
                return;
            }
            ProcessInformation processInfo = null;
            if (!processes.TryGetValue(processId, out processInfo))
            {
                processInfo = new ProcessInformation(processId, string.Empty);
                processes[processId] = processInfo;
            }
            processInfo.Add(new ProcessFriendlyName(serviceStatus.pDisplayName));
            processInfo.IsService = true;
        }

        private static void QueryServices(SortedList<uint, ProcessInformation> processes)
        {
            IntPtr handle = Interop.OpenSCManager(null, null, AccessFlags.GenericRead);
            if (handle == IntPtr.Zero)
            {
                return;
            }

            IntPtr buf = IntPtr.Zero;

            try
            {
                uint iBytesNeeded = 0;
                uint iServicesReturned = 0;
                uint iResumeHandle = 0;

                if (!Interop.EnumServicesStatusEx(handle, ServiceEnumType.Info, ServiceType.Win32 | ServiceType.Driver, ServiceStateRequest.All, IntPtr.Zero, 0, out iBytesNeeded, out iServicesReturned, ref iResumeHandle, null))
                {
                    // allocate our memory to receive the data for all the services (including the names)
                    buf = Marshal.AllocHGlobal((int)iBytesNeeded);

                    if (!Interop.EnumServicesStatusEx(handle, ServiceEnumType.Info, ServiceType.Win32 | ServiceType.Driver, ServiceStateRequest.All, buf, iBytesNeeded, out iBytesNeeded, out iServicesReturned, ref iResumeHandle, null))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    ENUM_SERVICE_STATUS_PROCESS serviceStatus;

                    // check if 64 bit system which has different pack sizes
                    if (IntPtr.Size == 8)
                    {
                        long pointer = buf.ToInt64();
                        for (int i = 0; i < (int)iServicesReturned; i++)
                        {
                            serviceStatus = (ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(new IntPtr(pointer), typeof(ENUM_SERVICE_STATUS_PROCESS));
                            UpdateProcessInformation(processes, serviceStatus);

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
                            UpdateProcessInformation(processes, serviceStatus);

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
                    Interop.CloseServiceHandle(handle);
                }

                if (buf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        private static readonly ProcessAccessFlags ProcessReadWriteFlags = ProcessAccessFlags.VirtualMemoryOperation
            | ProcessAccessFlags.SetInformation
            | ProcessAccessFlags.VirtualMemoryWrite
            | ProcessAccessFlags.QueryInformation
            | ProcessAccessFlags.VirtualMemoryRead;

        private static readonly ProcessAccessFlags ProcessReadOnlyFlags = ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead;

        /// <summary>
        /// Acquires the full path to the process
        /// </summary>
        private static void GetProcessFullPath(ProcessInformation processInfo)
        {
            processInfo.FullPath = null;
            processInfo.IsService = false;

            // If I succeed to open the process with the options needed to modify it, I know it is modifiable
            IntPtr hProcess = Interop.OpenProcess(ProcessReadWriteFlags, false, processInfo.ID);
            if (IntPtr.Zero == hProcess)  // I failed to open the process for modification so just open it to read the path info
                hProcess = Interop.OpenProcess(ProcessReadOnlyFlags, false, processInfo.ID);
            else // I succeeded opening the process so set the flag
                processInfo.Modifiable = true;

            if (IntPtr.Zero != hProcess)
            {
                IntPtr[] moduleHandles = new IntPtr[1];
                uint cbNeeded = 0;
                // Enumerate the modules in the process--the first one is the application which we can get the path from
                if (Interop.EnumProcessModules(hProcess, moduleHandles, (uint)IntPtr.Size, out cbNeeded))
                {   // Uses size in bytes
                    StringBuilder baseName = new StringBuilder(1024);
                    if (0 != Interop.GetModuleFileNameEx(hProcess, moduleHandles[0], baseName, (uint)baseName.Capacity))
                    {
                        processInfo.FullPath = baseName.ToString();
                    }
                }
                Interop.CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Acquires the name, path, and other details for the process
        /// </summary>
        private static ProcessInformation GetProcessInfo(uint processID, string idleProcessName, string systemName)
        {
            // Get a handle to the process and set some values for the process
            string nm = (processID == 0) ? idleProcessName : ((processID == 4) ? systemName : null);
            string fp = null;
            bool fModifiable = false;

            // Attempt to open the process with all the permissions we would need to modify it
            // and as a backup open te process with enough information to get what we need.
            IntPtr hProcess = Interop.OpenProcess(ProcessReadWriteFlags, false, processID);
            if (IntPtr.Zero == hProcess)  // Failed to open the process so it isn't modifiable, backup goal
                hProcess = Interop.OpenProcess(ProcessReadOnlyFlags, false, processID);
            else                // Succeeded to open it so the process is modifiable
                fModifiable = true;

            // Get the process name.
            if (IntPtr.Zero != hProcess)
            {
                IntPtr[] hMod = new IntPtr[1];
                uint cbNeeded;

                // The first module is (typically) the application itself, so we can get the information we need from there
                if (Interop.EnumProcessModules(hProcess, hMod, (uint)IntPtr.Size, out cbNeeded))
                {   // Uses size in bytes
                    StringBuilder szProcessName = new StringBuilder(1024);
                    StringBuilder szModulePath = new StringBuilder(1024);
                    if (0 != Interop.GetModuleBaseName(hProcess, hMod[0], szProcessName, (uint)szProcessName.Capacity))
                    {
                        nm = szProcessName.ToString();
                    }
                    // Uses size in bytes
                    if (0 != Interop.GetModuleFileNameEx(hProcess, hMod[0], szModulePath, (uint)szModulePath.Capacity))
                    {
                        fp = szModulePath.ToString();
                    }
                }

                Interop.CloseHandle(hProcess);
            }

            ProcessInformation res = new ProcessInformation(processID, nm);
            res.Modifiable = fModifiable;
            res.FullPath = fp;

            return res;
        }
    }
}
