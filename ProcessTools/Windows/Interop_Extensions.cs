using System;

namespace ProcessTools.Windows
{
    internal static class Interop_Extensions
    {
        public static bool IsWritableMemory(this MEMORY_BASIC_INFORMATION mbi)
	    {
		    return ((mbi.Protect == MemoryPageProtection.ReadWrite || mbi.Protect == MemoryPageProtection.ExecuteReadWrite) 
                && mbi.State == MemoryState.Commit
                && mbi.Type == MemoryType.Private
                && mbi.RegionSize.ToUInt64() > 0);
	    }

        public static WritableMemoryBlock ToWritableMemoryBlock(this MEMORY_BASIC_INFORMATION mbi)
        {
            WritableMemoryBlock block = new WritableMemoryBlock();
            block.BaseAddress = mbi.BaseAddress;
            block.RegionSize = mbi.RegionSize;
            return block;
        }

        public static UIntPtr GetEndAddress(this WritableMemoryBlock block)
        {
            return (UIntPtr) (block.BaseAddress.ToUInt64() + block.RegionSize.ToUInt64());
        }
    }
}
