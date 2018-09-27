using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSView2;

namespace MemHack
{
    static class ProcessExtensions
    {
        public static string GetAddress(this CAddressValue value)
        {
            return "0x" + value.Address.ToString("X16");
        }
    }
}
