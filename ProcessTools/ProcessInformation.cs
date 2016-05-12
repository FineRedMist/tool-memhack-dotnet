using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return rgFriendlyNames.Count == 0 ? string.Empty : rgFriendlyNames[0].Name;
            }
        }

        List<ProcessFriendlyName> rgFriendlyNames;

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
            rgFriendlyNames = new List<ProcessFriendlyName>();
        }

        public void Add(ProcessFriendlyName name)
        {
            if (name == null)
                return;
            rgFriendlyNames.Add(name);
            rgFriendlyNames.Sort();
        }

        public IEnumerator<ProcessFriendlyName> GetEnumerator()
        {
            return rgFriendlyNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rgFriendlyNames.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return rgFriendlyNames.Count;
            }
        }

        public ProcessFriendlyName this[int index]
        {
            get
            {
                return rgFriendlyNames[index];
            }
        }
    }
}
