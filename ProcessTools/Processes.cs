using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return null;
        }


        /// <summary>
        /// Acquires the full path to the process
        /// </summary>
        private void GetProcessFullPath(ProcessInformation processInfo)
        {
        }

        /// <summary>
        /// Acquires the name, path, and other details for the process
        /// </summary>
        private ProcessInformation GetProcessInfo(uint processID, string idleProcessName, string systemName)
        {
            return null;
        }
    }
}
