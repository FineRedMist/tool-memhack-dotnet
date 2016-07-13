using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ProcessTools.Windows;

namespace ProcessTools
{

    /// <summary>
    /// Information about the byte sequence stored at a given address.
    /// </summary>
    public class AddressValueInformation
    {
        /// <summary>
        /// The address at which the value was queried.
        /// </summary>
        public UIntPtr Address { get; private set; }
        /// <summary>
        /// The byte sequence at that location (size determined by request).
        /// </summary>
        public byte[] CurrentBuffer { get; private set; }
    }

    /// <summary>
    /// Information about an address found during searching (including which byte values 
    /// that were matched in the list of values to search for).
    /// </summary>
    public class FoundAddressInformation : AddressValueInformation
    {
        /// <summary>
        /// A bitfield representing the indicies of matched values from the list of values provided.
        /// </summary>
        public uint MatchedValues { get; private set; }
    }

    /// <summary>
    /// Class to facilitate modifying the process memory for serializable struct <typeparamref name="T"/>.
    /// </summary>
    public class ModifyProcess<T>
        where T : struct
    {
        private readonly ProcessQueryMemory Query;

        static ModifyProcess()
        {
            T temp = default(T);
            int size = Marshal.SizeOf(typeof(T));
            IntPtr alloc = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(temp, alloc, false);
            Marshal.DestroyStructure<T>(alloc);
            Marshal.FreeHGlobal(alloc);
        }

        /// <summary>
        /// Creates a new process modifying instance based on the target process referenced by <paramref name="processQuery"/>.
        /// </summary>
        public ModifyProcess(ProcessQueryMemory processQuery)
        {
            Query = processQuery;
        }

        /* Thoughts:
         *   I want to have a List of byte[] sequences to search.
         *   A given address may match multiple byte[] sequences
         *   I think I can just return the address and the largest number of bytes matched
         *      The calling app can then use that information to query that address and corresponding byte[] range and evaluate the possible values
         *      The calling app (memhack) can have a column for each type as relevant.
         *      The active search in the calling app can then remove columns as appropriate.
         *      Note that means FindNext will return previously found addresses where the new sequences matches any of the byte[] sequences provided in the list.
         *   This moves up the translation of T -> byte[] to the caller, I only care about byte sequences.
         *   I need to return in FindFirst and FindNext the list of addresses, block sizes, and current values.
         *   I'll want an api to query the values for the current list as well.
         */
        
        /// <summary>
        /// Finds all instances of the <paramref name="values"/> in the process's writable memory.
        /// </summary>
        public unsafe void FindFirst(IReadOnlyList<byte[]> values, IProgressIndicator progress)
        {
            byte[] dataToFind = ValueToBytes(value);
            fixed(byte* data = dataToFind)
            {
                foreach (var writableBlock in Query.GetContiguousWritableBlocks()
                    .OrderByDescending(block => block.RegionSize.ToUInt64()))
                {
                }
            }
        }

        /// <summary>
        /// Filters the list of previously found addresses to those that contain <paramref name="value"/>.
        /// </summary>
        public unsafe void FindNext(T value, IProgressIndicator progress)
        {
        }

        /// <summary>
        /// Sets the value at <paramref name="address"/> in the process to <paramref name="value"/>.
        /// </summary>
        public unsafe void SetValue(UIntPtr address, T value)
        {
        }

        private unsafe List<UIntPtr> SearchBlock(WritableMemoryBlock block, byte* data, int size, List<UIntPtr> previousAddresses, IProgressIndicator progress)
        {
            return null;
        }

        private static byte[] ValueToBytes(T value)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr alloc = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, alloc, false);
            byte[] result = new byte[size];
            Marshal.Copy(alloc, result, 0, size);
            Marshal.DestroyStructure<T>(alloc);
            Marshal.FreeHGlobal(alloc);

            return result;
        }

        private static T BytesToValue(byte[] value)
        {
            int size = Marshal.SizeOf(typeof(T));
            if (size != value.Length)
            {
                throw new ArgumentOutOfRangeException("value");
            }
            IntPtr alloc = Marshal.AllocHGlobal(size);
            Marshal.Copy(value, 0, alloc, size);
            T result = Marshal.PtrToStructure<T>(alloc);
            Marshal.FreeHGlobal(alloc);

            return result;
        }
    }
}
