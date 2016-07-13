using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTools
{
    /// <summary>
    /// Extensions to facilitate process modifications.
    /// </summary>
    public static class ProcessExtensions
    {
        private static readonly uint[] MulDeBruijnBitPos = new uint[32]
        {
          0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
          31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };

        /// <summary>
        /// Returns the bits that are set in the <paramref name="bitfield"/>.
        /// </summary>
        public static uint[] GetSetBits(uint bitfield)
        {
            uint[] data = new uint[32];
            int enabledBitCounter = 0;

            while (bitfield != 0)
            {
                uint m = (bitfield & (0 - bitfield));
                bitfield ^= m;
                data[enabledBitCounter++] = MulDeBruijnBitPos[(m * (uint)0x077CB531U) >> 27];
            }

            Array.Resize<uint>(ref data, enabledBitCounter);
            return data;
        }

        /// <summary>
        /// Sets a bit on a <paramref name="bitfield"/> at <paramref name="index"/>.
        /// </summary>
        public static uint SetBit(uint bitfield, int index)
        {
            if (index < 0 || index > 31)
            {
                throw new IndexOutOfRangeException(string.Format("The index {0} to set the bit is out of range.", index));
            }
            return bitfield | (1u << index);
        }
    }
}
