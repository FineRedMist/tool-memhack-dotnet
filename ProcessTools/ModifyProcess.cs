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

        /// <summary>
        /// Finds all instances of <paramref name="value"/> in the process's writable memory.
        /// </summary>
        public unsafe void FindFirst(T value, IProgressIndicator progress)
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
