using ProcessTools.Windows;
using System;
using System.Collections.Generic;

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

            foreach (var processEntry in Interop.SnapProcesses())
            {
                // Set up the structures
                ProcessInformation p = new ProcessInformation(processEntry.th32ProcessID, processEntry.szExeFile);
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
            }

            return result;
        }

        private static SortedList<uint, ProcessInformation> QueryEnumProcesses(string idleProcessName, string systemName)
        {
            SortedList<uint, ProcessInformation> result = new SortedList<uint, ProcessInformation>();

            uint[] processList = Interop.EnumProcesses();
            if (processList == null)
            {
                return result;
            }

            // Get the information for the list of processes
            foreach(uint processId in processList)
            {
                result[processId] = GetProcessInfo(processId, idleProcessName, systemName);
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

                // Get window information
                var windowInfo = Interop.GetWindowInfo(hwnd);

                if (!windowInfo.HasValue)
                {
                    return true;
                }

                var wi = windowInfo.Value;

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
            Interop.EnumDesktopWindows(enumerate.EnumDesktopWindowsDelegate);
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
            var services = Interop.GetServices(null, null, AccessFlags.GenericRead, ServiceEnumType.Info, ServiceType.Driver | ServiceType.Win32, ServiceStateRequest.Active);
            if (services == null)
            {
                return;
            }

            foreach (var service in services)
            {
                UpdateProcessInformation(processes, service);
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
                IntPtr[] moduleHandles = Interop.EnumProcessModules(hProcess, 1);
                // Enumerate the modules in the process--the first one is the application which we can get the path from
                if (moduleHandles != null && moduleHandles.Length > 0)
                {   // Uses size in bytes
                    string moduleFilename;
                    if (Interop.GetModuleFileNameEx(hProcess, moduleHandles[0], out moduleFilename))
                    {
                        processInfo.FullPath = moduleFilename;
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
                IntPtr[] moduleHandles = Interop.EnumProcessModules(hProcess, 1);

                // The first module is (typically) the application itself, so we can get the information we need from there
                if (moduleHandles != null && moduleHandles.Length > 0)
                { 
                    string szProcessName;
                    if (Interop.GetModuleBaseName(hProcess, moduleHandles[0], out szProcessName))
                    {
                        nm = szProcessName;
                    }
                    string moduleFilename;
                    if (Interop.GetModuleFileNameEx(hProcess, moduleHandles[0], out moduleFilename))
                    {
                        fp = moduleFilename;
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
