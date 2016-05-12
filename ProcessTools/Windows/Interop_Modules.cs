using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessTools.Windows
{
    internal partial class Interop
    {
        public static IntPtr[] EnumProcessModules(IntPtr hProcess, uint countOfModulesToGet)
        {
            IntPtr[] moduleHandles = new IntPtr[countOfModulesToGet];
            uint bytesNeeded = 0;
            uint bytesProvided = (uint)(countOfModulesToGet * IntPtr.Size);
            if (EnumProcessModules(hProcess, moduleHandles, bytesProvided, out bytesNeeded))
            {
                if (bytesNeeded < bytesProvided)
                {
                    int toCopy = (int)(bytesNeeded / IntPtr.Size);
                    IntPtr[] newResult = new IntPtr[toCopy];
                    Array.Copy(moduleHandles, newResult, toCopy);
                    return newResult;
                }
                return moduleHandles;
            }

            return null;
        }

        public static bool GetModuleBaseName(IntPtr hProcess, IntPtr hModule, out string baseName)
        {
            baseName = null;
            StringBuilder nameBuffer = new StringBuilder(1024);
            if (0 != GetModuleBaseName(hProcess, hModule, nameBuffer, (uint)nameBuffer.Capacity))
            {
                baseName = nameBuffer.ToString();
                return true;
            }
            return false;
        }

        public static bool GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, out string moduleFileName)
        {
            moduleFileName = null;
            StringBuilder nameBuffer = new StringBuilder(1024);
            if (0 != GetModuleFileNameEx(hProcess, hModule, nameBuffer, (uint)nameBuffer.Capacity))
            {
                moduleFileName = nameBuffer.ToString();
                return true;
            }
            return false;
        }

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumProcessModules(IntPtr hProcess, [In][Out] IntPtr[] lphModule, uint cb, out uint lpcbNeeded);

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);
    }
}
