using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ProcessTools.Windows
{
    internal struct WritableMemoryBlock
    {
        public UIntPtr BaseAddress;
        public UIntPtr RegionSize;
    }

    internal struct MEMORY_BASIC_INFORMATION
    {
        public UIntPtr BaseAddress;
        public UIntPtr AllocationBase;
        public MemoryPageProtection AllocationProtect;
        public UIntPtr RegionSize;
        public MemoryState State;
        public MemoryPageProtection Protect;
        public MemoryType Type;
    }

    internal enum MemoryPageProtection : uint
    {
        Execute = 0x00000010,
        ExecuteRead = 0x00000020,
        ExecuteReadWrite = 0x00000040,
        ExecuteWriteCopy = 0x00000080,
        NoAccess = 0x00000001,
        ReadOnly = 0x00000002,
        ReadWrite = 0x00000004,
        WriteCopy = 0x00000008,
        Guard = 0x00000100,
        NoCache = 0x00000200,
        WriteCombine = 0x00000400
    }

    internal enum MemoryState : uint
    {
        Commit = 0x1000,
        Free = 0x10000,
        Reserve = 0x2000
    }

    internal enum MemoryType : uint
    {
        Image = 0x1000000,
        Mapped = 0x40000,
        Private = 0x20000
    }

    internal partial class Interop
    {
        public static IEnumerable<WritableMemoryBlock> GetWritableAddresses(IntPtr hProcess, UIntPtr? startAddress = null, UIntPtr? size = null)
        {
            UIntPtr currentAddress = startAddress.HasValue ? startAddress.Value : UIntPtr.Zero;
            ulong? endAddress = startAddress.HasValue && size.HasValue
                ? (ulong?)(startAddress.Value.ToUInt64() + size.Value.ToUInt64())
                : null;

            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();
            uint bufferSize = (uint) Marshal.SizeOf(mbi);
            do
            {
                if (UIntPtr.Zero == VirtualQueryEx(hProcess, currentAddress, out mbi, bufferSize))
                {
                    yield break;
                }

                if (mbi.IsWritableMemory())
                {
                    yield return mbi.ToWritableMemoryBlock();
                }

                ulong newAddress = mbi.BaseAddress.ToUInt64() + mbi.RegionSize.ToUInt64();
                if (newAddress < currentAddress.ToUInt64()
                    || (endAddress.HasValue && newAddress >= endAddress.Value))
                {
                    yield break;
                }
                currentAddress = (UIntPtr)(newAddress);
            } while (true);
        }

        public static bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] buffer, uint bytesToRead, out uint bytesRead)
        {
            UIntPtr bigBytesRead;
            bool result = ReadProcessMemory(hProcess, lpBaseAddress, buffer, (UIntPtr)bytesToRead, out bigBytesRead);
            bytesRead = bigBytesRead.ToUInt32();
            return result;
        }

        public static bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] buffer, uint bytesToWrite, out uint bytesWritten)
        {
            UIntPtr bigBytesWritten;
            bool result = WriteProcessMemory(hProcess, lpBaseAddress, buffer, (UIntPtr)bytesToWrite, out bigBytesWritten);
            bytesWritten = bigBytesWritten.ToUInt32();
            return result;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UIntPtr VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] buffer, UIntPtr size, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] buffer, UIntPtr size, out UIntPtr lpNumberOfBytesWritten);
    }
}
