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
                ProcessInformation processInfo = new ProcessInformation(processEntry.th32ProcessID, processEntry.szExeFile);
                if (processInfo.ID == 0) // The idle process has the zero id, so save myself some work by special casing it
                {
                    processInfo.Add(new ProcessFriendlyName(idleProcessName));
                }
                else if (processInfo.ID == 4)
                {
                    processInfo.Add(new ProcessFriendlyName(systemName));
                }
                GetProcessFullPath(processInfo);
                result[processInfo.ID] = processInfo;
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
                uint processId = 0;
                uint threadId = Interop.GetWindowThreadProcessId(hwnd, out processId);
                bool visible;
                DateTime windowThreadDate = DateTime.MinValue;
                string windowTitle;

                if (threadId == 0 || processId == 0)    // We somehow mystically failed to get one of the thread or process ids
                {
                    return true;
                }

                // Get window information
                var maybeWindowInfo = Interop.GetWindowInfo(hwnd);

                if (!maybeWindowInfo.HasValue)
                {
                    return true;
                }

                var windowInfo = maybeWindowInfo.Value;

                // There are some classes of windows that I don't want to display so skip them
                if (windowInfo.dwExStyle.HasFlag(WindowExtendedStyle.ToolWindow) 
                    || 0 != (windowInfo.dwStyle & (WindowStyle.Popup | WindowStyle.Child | WindowStyle.Disabled)))
                {
                    return true;
                }

                // Get the text of the window
                windowTitle = Interop.GetWindowText(hwnd);
                if (string.IsNullOrEmpty(windowTitle))
                {
                    return true;
                }

                // Flag whether the window is visible.  There are some that I still want to display unfortunately
                // which means I'll also have a bunch of others that I don't want to display in the list
                visible = (windowInfo.dwStyle & (WindowStyle.Minimize | WindowStyle.Visible)) != 0;

                DateTime threadExitedTime, threadKernelTime, threadUserTime;
                // I use the thread information to determine when the window is created.  Typically the first window
                // created is really the interesting one so I sort them based on date created
                using (var threadHandle = Interop.OpenThreadHandle(ThreadAccess.QUERY_INFORMATION, false, threadId))
                {
                    if (threadHandle != null)
                    {
                        Interop.GetThreadTimes(threadHandle.Value, out windowThreadDate, out threadExitedTime, out threadKernelTime, out threadUserTime);
                    }
                }

                ProcessFriendlyName windowFriendlyName = new ProcessFriendlyName(windowTitle, windowThreadDate, visible);
                ProcessInformation processInfo = null;
                if (!mProcesses.TryGetValue(processId, out processInfo))
                {
                    processInfo = new ProcessInformation(processId, string.Empty);
                    mProcesses[processId] = processInfo;
                }
                processInfo.Add(windowFriendlyName);

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

        private static AutoDispose<IntPtr> OpenProcess(uint processId, out bool isReadOnly)
        {
            var processHandle = Interop.OpenProcessHandle(ProcessReadWriteFlags, false, processId);
            if (processHandle != null)
            {
                // If I succeed to open the process with the options needed to modify it, I know it is modifiable
                isReadOnly = false;
                return processHandle;
            }
            // I failed to open the process for modification so just open it to read the path info
            isReadOnly = true;
            return Interop.OpenProcessHandle(ProcessReadOnlyFlags, false, processId);
        }

        /// <summary>
        /// Acquires the full path to the process
        /// </summary>
        private static void GetProcessFullPath(ProcessInformation processInfo)
        {
            processInfo.FullPath = null;
            processInfo.IsService = false;

            bool modifiable;
            using (var processHandle = OpenProcess(processInfo.ID, out modifiable))
            {
                processInfo.Modifiable = modifiable;
                if (null != processHandle)
                {
                    IntPtr[] moduleHandles = Interop.EnumProcessModules(processHandle.Value, 1);
                    // Enumerate the modules in the process--the first one is the application which we can get the path from
                    if (moduleHandles != null && moduleHandles.Length > 0)
                    {   // Uses size in bytes
                        string moduleFilename;
                        if (Interop.GetModuleFileNameEx(processHandle.Value, moduleHandles[0], out moduleFilename))
                        {
                            processInfo.FullPath = moduleFilename;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Acquires the name, path, and other details for the process
        /// </summary>
        private static ProcessInformation GetProcessInfo(uint processID, string idleProcessName, string systemName)
        {
            // Get a handle to the process and set some values for the process
            string processName = (processID == 0) ? idleProcessName : ((processID == 4) ? systemName : null);
            string fullProcessPath = null;

            bool modifiable;
            using (var processHandle = OpenProcess(processID, out modifiable))
            {
                // Get the process name.
                if (null != processHandle)
                {
                    IntPtr[] moduleHandles = Interop.EnumProcessModules(processHandle.Value, 1);

                    // The first module is (typically) the application itself, so we can get the information we need from there
                    if (moduleHandles != null && moduleHandles.Length > 0)
                    {
                        string moduleProcessName;
                        if (Interop.GetModuleBaseName(processHandle.Value, moduleHandles[0], out moduleProcessName))
                        {
                            processName = moduleProcessName;
                        }
                        string moduleFilename;
                        if (Interop.GetModuleFileNameEx(processHandle.Value, moduleHandles[0], out moduleFilename))
                        {
                            fullProcessPath = moduleFilename;
                        }
                    }
                }
            }

            ProcessInformation result = new ProcessInformation(processID, processName);
            result.Modifiable = modifiable;
            result.FullPath = fullProcessPath;

            return result;
        }
    }
}
