using System;
using System.Collections.Generic;

namespace ProcessTools
{
    /// <summary>
    /// This class describes a set of properties for an associated friendly name for a process.
    /// A process can have more than one friendly name whether it be an open window or a service.
    /// The Date variable is used in the window case to determine the first window created by
    /// an application to help sort out the primary window from derivative windows.
    /// </summary>
    public class ProcessFriendlyName : IComparable<ProcessFriendlyName>, IComparer<ProcessFriendlyName>
    {
        /// <summary>
        /// The name of the window or service in this process.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The date the window was created (not relevant for services).
        /// </summary>
        public DateTime Date { get; private set; }
        /// <summary>
        /// Whether the window is visible (not relevant for services).
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// Service name
        /// </summary>
        public ProcessFriendlyName(string name)
        {
            Name = name.Trim();
            Visible = true;
            Date = DateTime.MinValue;
        }

        /// <summary>
        /// Window name
        /// </summary>
        public ProcessFriendlyName(string name, DateTime date, bool visible)
        {
            Name = name.Trim();
            Date = date;
            Visible = visible;
        }

        /// <summary>
        /// The ProcessFriendlyName can be sorted and these are the criteria:
        ///   Visibility first
        ///   Date of window creation 
        ///   Then the string name
        /// </summary>
        public int CompareTo(ProcessFriendlyName? other)
        {
            if (other == null)
            {
                return -1;
            }
            int compare = -Visible.CompareTo(other.Visible);
            if (compare != 0)
            {
                return compare;
            }
            compare = Date.CompareTo(other.Date);
            if (compare != 0)
            {
                return compare;
            }
            return string.Compare(Name, other.Name);
        }

        /// <summary>
        /// The ProcessFriendlyName can be sorted and these are the criteria:
        ///   Visibility first
        ///   Date of window creation 
        ///   Then the string name
        /// </summary>
        public int Compare(ProcessFriendlyName? x, ProcessFriendlyName? y)
        {
            if(x == null && y == null)
            {
                return 0;
            }
            if(x == null)
            {
                return -1;
            }
            if(y == null)
            {
                return 1;
            }
            return x.CompareTo(y);
        }
    }
}