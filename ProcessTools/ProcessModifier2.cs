﻿using ProcessTools.Windows;
using System;
using System.Collections.Generic;

namespace ProcessTools
{
    /// <summary>
    /// Provides the necessary functionality for modifying a process.
    /// </summary>
    public class ProcessModifier2
    {
        AutoDispose<IntPtr> mProcessHandle;
        /// <summary>
        /// Information about the process being examined.
        /// </summary>
        public ProcessInformation ProcessInfo { get; private set; }
        /// <summary>
        /// Whether the process was open in read-only mode.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        private ProcessModifier2(ProcessInformation processInfo, AutoDispose<IntPtr> processHandle, bool isReadOnly)
        {
            ProcessInfo = processInfo;
            mProcessHandle = processHandle;
            IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// Creates a new process modifier based on the provided <paramref name="processInfo"/>.
        /// </summary>
        public static ProcessModifier2 Create(ProcessInformation processInfo)
        {
            bool isReadOnly;
            var processHandle = Processes.OpenProcess(processInfo.ID, out isReadOnly);
            if (processHandle == null)
            {
                return null;
            }
            return new ProcessModifier2(processInfo, processHandle, isReadOnly);
        }

        /// <summary>
        /// Retrieves the writable blocks from the current process being modified after 
        /// merging contiguous blocks.
        /// </summary>
        internal IEnumerable<WritableMemoryBlock> GetContiguousWritableBlocks()
        {
            WritableMemoryBlock? current = null;

            foreach (var block in Interop.GetWritableAddresses(mProcessHandle.Value))
            {
                if (current.HasValue && current.Value.GetEndAddress() == block.BaseAddress)
                {
                    WritableMemoryBlock temp = current.Value;
                    temp.RegionSize = (UIntPtr)(temp.RegionSize.ToUInt64() + block.RegionSize.ToUInt64());
                    current = temp;
                }
                else
                {
                    if (current.HasValue)
                    {
                        yield return current.Value;
                    }
                    current = block;
                }
            }

            if (current.HasValue)
            {
                yield return current.Value;
            }
        }
    }
}
