using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using ProcessTools.Windows;
using System.Threading;

namespace ProcessTools
{
    class FoundAddressList
        : IReadOnlyList<UIntPtr>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ReaderWriterLockSlim mSync = new ReaderWriterLockSlim();
        private int mCount = 0;

        class ActiveAddresses
        {
            public WritableMemoryBlock Block;
            public List<ushort> ShortOffsets = new List<ushort>();
            public List<uint> Offsets = new List<uint>();

            public int Count { get { return ShortOffsets.Count + Offsets.Count; } }

            public UIntPtr this[int index]
            {
                get
                {
                    if (index < ShortOffsets.Count)
                    {
                        return (UIntPtr)(Block.BaseAddress.ToUInt64() + ShortOffsets[index]);
                    }
                    return (UIntPtr)(Block.BaseAddress.ToUInt64() + Offsets[index - ShortOffsets.Count]);
                }
            }

            #region public bool Within(UIntPtr address)
            public bool Within(UIntPtr address)
            {
                ulong addr = address.ToUInt64();
                ulong end = Block.GetEndAddress().ToUInt64();
                ulong start = Block.BaseAddress.ToUInt64();
                return addr >= start && addr < end;
            }
            #endregion

            #region public int AddAddress(UIntPtr address)
            public int AddAddress(UIntPtr address)
            {
                ulong offset = address.ToUInt64() - Block.BaseAddress.ToUInt64();
                if (offset <= ushort.MaxValue)
                {
                    ushort off = (ushort)offset;
                    int idx = ShortOffsets.BinarySearch(off);
                    if (idx >= 0)
                    {
                        return -1;
                    }
                    ShortOffsets.Insert(~idx, off);
                    return ~idx;
                }
                else if (offset < uint.MaxValue)
                {
                    uint off = (uint)offset;
                    int idx = Offsets.BinarySearch(off);
                    if (idx >= 0)
                    {
                        return -1;
                    }
                    Offsets.Insert(~idx, off);
                    return ShortOffsets.Count + ~idx;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("address");
                }
            }
            #endregion

            #region public int RemoveAddress(UIntPtr address)
            public int RemoveAddress(UIntPtr address)
            {
                ulong offset = address.ToUInt64() - Block.BaseAddress.ToUInt64();
                if (offset <= ushort.MaxValue)
                {
                    ushort off = (ushort)offset;
                    int idx = ShortOffsets.BinarySearch(off);
                    if (idx >= 0)
                    {
                        ShortOffsets.RemoveAt(idx);
                        return idx;
                    }
                    return -1;
                }
                else if (offset < uint.MaxValue)
                {
                    uint off = (uint)offset;
                    int idx = Offsets.BinarySearch(off);
                    if (idx >= 0)
                    {
                        Offsets.RemoveAt(idx);
                        return ShortOffsets.Count + idx;
                    }
                    return -1;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("address");
                }
            }
            #endregion

            #region public IEnumerable<UIntPtr> GetAddresses(ulong startAddress, ulong endAddress)
            public IEnumerable<UIntPtr> GetAddresses(ulong startAddress, ulong endAddress)
            {
                ulong baseAddress = Block.BaseAddress.ToUInt64();
                ulong startOffset = startAddress - baseAddress;
                ulong endOffset = endAddress - baseAddress;

                if (startOffset <= ushort.MaxValue)
                {
                    int idx = ShortOffsets.BinarySearch((ushort) startOffset);
                    idx = idx >= 0 ? idx : ~idx;
                    for(; idx < ShortOffsets.Count; ++idx)
                    {
                        var offset = ShortOffsets[idx];
                        if (offset >= endAddress)
                        {
                            yield break;
                        }
                        yield return (UIntPtr)(baseAddress + offset);
                    }
                    startOffset = ushort.MaxValue;
                    ++startOffset;
                }

                {
                    int idx = Offsets.BinarySearch((uint)startOffset);
                    idx = idx >= 0 ? idx : ~idx;
                    for (; idx < Offsets.Count; ++idx)
                    {
                        var offset = Offsets[idx];
                        if (offset >= endAddress)
                        {
                            yield break;
                        }
                        yield return (UIntPtr)(baseAddress + offset);
                    }
                }
            }
            #endregion

            #region private void AddAddresses(IEnumerable<UIntPtr> addresses)
            private void AddAddresses(IEnumerable<UIntPtr> addresses)
            {
                ulong baseAddress = Block.BaseAddress.ToUInt64();

                foreach (var address in addresses)
                {
                    ulong offset = address.ToUInt64() - baseAddress;
                    if (offset <= ushort.MaxValue)
                    {
                        ShortOffsets.Add((ushort)offset);
                    }
                    else
                    {
                        Offsets.Add((uint)offset);
                    }
                }
            }
            #endregion

            /// <summary>
            /// If this writable memory block gets split (a block is no longer available splitting a contiguous block)
            /// then we need to split the found addresses and drop those from the block that is no longer present.
            /// We also need to rebase the offsets for any in the other writable blocks.
            /// 
            /// We use the <paramref name="valueSize"/> to determine if an address crosses into a now removed block.
            /// </summary>
            public IEnumerable<ActiveAddresses> Split(IEnumerable<WritableMemoryBlock> newBlocks, uint valueSize)
            {
                ulong current = Block.BaseAddress.ToUInt64();

                foreach (var block in newBlocks)
                {
                    ulong blockStart = block.BaseAddress.ToUInt64();
                    ulong blockSize = block.RegionSize.ToUInt64();
                    ulong blockEnd = blockStart + blockSize;

                    if(current < blockStart)
                    {
                        ActiveAddresses result = new ActiveAddresses();
                        result.Block = block;
                        result.AddAddresses(GetAddresses(blockStart, blockEnd - valueSize));
                        if (result.Count > 0)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        private List<ActiveAddresses> mActiveAddresses = new List<ActiveAddresses>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public FoundAddressList()
        {
            var changed = CollectionChanged;
            if (changed != null)
            {
                changed(null, null);
            }
            var changed2 = PropertyChanged;
            if (changed2 != null)
            {
                changed2(null, null);
            }
        }

        #region public int Count
        public int Count
        {
            get
            {
                mSync.EnterReadLock();
                int result = mCount;
                mSync.ExitReadLock();
                return result;
            }
        }
        #endregion

        #region public UIntPtr this[int index]
        public UIntPtr this[int index]
        {
            get
            {
                UIntPtr result = UIntPtr.Zero;

                int target = index;
                mSync.EnterReadLock();
                foreach (var addresses in mActiveAddresses)
                {
                    if (target < addresses.Count)
                    {
                        result = addresses[target];
                        break;
                    }
                    target -= addresses.Count;
                }
                mSync.ExitReadLock();
                if (result == UIntPtr.Zero)
                {
                    throw new IndexOutOfRangeException();
                }
                return result;
            }
        }
        #endregion

        public IEnumerator<UIntPtr> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
