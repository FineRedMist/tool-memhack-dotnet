using System.Collections;
using System.Collections.Generic;

namespace ProcessTools
{
    /// <summary>
    /// Information about a process running on the system.
    /// 
    /// Note: Not all fields may have values if there are insufficient permissions to query them.
    /// </summary>
    public class ProcessInformation : IReadOnlyList<ProcessFriendlyName>
    {
        /// <summary>
        /// The name of the process.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The ID of the process.
        /// </summary>
        public uint ID { get; private set; }
        /// <summary>
        /// Whether the process is modifiable--if not it will not be selectable to modify.
        /// </summary>
        public bool Modifiable { get; set; }
        /// <summary>
        /// Whether this process is the idle process.
        /// </summary>
        public bool IdleProcess { get; private set; }
        /// <summary>
        /// Whether this process is the system process.
        /// </summary>
        public bool System { get; private set; }
        /// <summary>
        /// Whether this process is a service.
        /// </summary>
        public bool IsService { get; set; }
        /// <summary>
        /// The full path to the executable of this process (may not be available).
        /// </summary>
		public string FullPath { get; set; }

        /// <summary>
        /// The default friendly name of this process, typically determined by the first visible window.
        /// </summary>
        public string DefaultFriendlyName
        {
            get
            {
                return FriendlyNames.Count == 0 ? string.Empty : FriendlyNames[0].Name;
            }
        }

        List<ProcessFriendlyName> FriendlyNames;

        /// <summary>
        /// Creates the process information for a given <paramref name="processID"/> and <paramref name="processName"/>.
        /// </summary>
        public ProcessInformation(uint processID, string processName)
        {
            ID = processID;
            Modifiable = false;
            IdleProcess = (ID == 0) ? true : false;
            System = (ID == 0 || ID == 4) ? true : false;
            IsService = false;
            Name = processName;
            FullPath = null;
            FriendlyNames = new List<ProcessFriendlyName>();
        }

        /// <summary>
        /// Adds a <seealso cref="ProcessFriendlyName"/> for the current process.
        /// </summary>
        public void Add(ProcessFriendlyName name)
        {
            if (name == null)
                return;
            int foundIndex = FriendlyNames.BinarySearch(name, name);
            if (foundIndex < 0)
            {
                FriendlyNames.Insert(~foundIndex, name);
            }
            else
            {
                FriendlyNames.Insert(foundIndex, name);
            }
        }

        /// <summary>
        /// Retrieves an enumerator for all <seealso cref="ProcessFriendlyName"/> entries for this process.
        /// </summary>
        public IEnumerator<ProcessFriendlyName> GetEnumerator()
        {
            return FriendlyNames.GetEnumerator();
        }

        /// <summary>
        /// Retrieves an enumerator for all <seealso cref="ProcessFriendlyName"/> entries for this process.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return FriendlyNames.GetEnumerator();
        }

        /// <summary>
        /// The number of <seealso cref="ProcessFriendlyName"/> entries for this process.
        /// </summary>
        public int Count
        {
            get
            {
                return FriendlyNames.Count;
            }
        }

        /// <summary>
        /// Indexer to retrieve the <seealso cref="ProcessFriendlyName"/> at <paramref name="index"/>.
        /// </summary>
        public ProcessFriendlyName this[int index]
        {
            get
            {
                return FriendlyNames[index];
            }
        }
    }
}
