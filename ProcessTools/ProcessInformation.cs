using System.Collections;
using System.Collections.Generic;

namespace ProcessTools
{
    public class ProcessInformation : IReadOnlyList<ProcessFriendlyName>
    {
        public string Name { get; private set; }
        public uint ID { get; private set; }

        public bool Modifiable { get; set; }
        public bool IdleProcess { get; private set; }
        public bool System { get; private set; }
        public bool IsService { get; set; }
		public string FullPath { get; set; }

		private string User { get; set; }

        public string DefaultFriendlyName
        {
            get
            {
                return FriendlyNames.Count == 0 ? string.Empty : FriendlyNames[0].Name;
            }
        }

        List<ProcessFriendlyName> FriendlyNames;

        public ProcessInformation(uint processID, string processName)
        {
            ID = processID;
            Modifiable = false;
            IdleProcess = (ID == 0) ? true : false;
            System = (ID == 0 || ID == 4) ? true : false;
            IsService = false;
            Name = processName;
            FullPath = null;
            User = null;
            FriendlyNames = new List<ProcessFriendlyName>();
        }

        public void Add(ProcessFriendlyName name)
        {
            if (name == null)
                return;
            FriendlyNames.Add(name);
            FriendlyNames.Sort();
        }

        public IEnumerator<ProcessFriendlyName> GetEnumerator()
        {
            return FriendlyNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return FriendlyNames.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return FriendlyNames.Count;
            }
        }

        public ProcessFriendlyName this[int index]
        {
            get
            {
                return FriendlyNames[index];
            }
        }
    }
}
