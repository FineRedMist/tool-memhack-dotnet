/*****************************************************
	File: bitfield.h
	Written By: Brent Scriver

	Abstract:  Template class implementation for
		bitfields of potentially large sizes.
******************************************************/

#pragma once

/*--------------------------------------------------
  Template class declaration
--------------------------------------------------*/

const DWORD BIT_NOT_FOUND = 0xFFFFFFFF;

#ifndef ARRAYSIZE
#define ARRAYSIZE(x)		(sizeof(x)/sizeof(x[0]))
#endif

template <DWORD Size> class BitField
{
protected:
	DWORD mBitsInUse;
	BYTE mBits[(Size >> 3) + (Size & 0x7 ? 1 : 0)];

public:
	/** Initializes the BitField to all zero bits. */
	inline BitField()
	{
		Clear();
	}

	/** Returns the number of bits in use (non-zero). */
	inline DWORD InUse()
	{
		return mBitsInUse;
	}

	/** Sets the bit at BitPosition to 1 and returns the old value. */
	inline bool Set(DWORD BitPosition)
	{
		assert(BitPosition < Size);
		const bool old = (mBits[BitPosition >> 3] & (1 << (BitPosition & 0x7))) != 0;
		mBits[BitPosition >> 3] |= (1 << (BitPosition & 0x7));
		mBitsInUse += (old ? 0 : 1);
		return old;
	}

	/** Toggles the bit at BitPosition from 1 to 0 or 0 to 1 and returns the old value. */
	inline bool Flip(DWORD BitPosition)
	{
		assert(BitPosition < Size);
		const bool old = (mBits[BitPosition >> 3] & (1 << (BitPosition & 0x7))) != 0;
		if (old)
			mBits[BitPosition >> 3] &= ~(1 << (BitPosition & 0x7));
		else
			mBits[BitPosition >> 3] |= (1 << (BitPosition & 0x7));
		mBitsInUse += (old ? -1 : 1);
		return old;
	}

	/** Clears the bit at BitPosition to 0 and returns the old value. */
	inline bool Clear(DWORD BitPosition)
	{
		assert(BitPosition < Size);
		const bool old = (mBits[BitPosition >> 3] & (1 << (BitPosition & 0x7))) != 0;
		mBits[BitPosition >> 3] &= ~(1 << (BitPosition & 0x7));
		mBitsInUse -= (old ? 1 : 0);
		return old;
	}

	//
	// Clears all entries from the bitarray
	//
	inline void Clear()
	{
		mBitsInUse = 0;
		ZeroMemory(mBits, ARRAYSIZE(mBits));
	}

	//
	// Retrieves the bit value at position pos
	//
	inline bool Get(DWORD BitPosition) const
	{
		assert(BitPosition < Size);
		return (mBits[BitPosition >> 3] & (1 << (BitPosition & 0x7))) != 0;
	}

	//
	// Searches the array to find the index of the first free bit in the array
	// returns 0xFFFFFFFFF if one is not found.
	//
	inline DWORD FindFirstFree() const
	{
		DWORD offset = 0, res = BIT_NOT_FOUND;
		if (mBitsInUse == Size)
			return res;
		for (DWORD i = 0; i < ARRAYSIZE(mBits); ++i)
			if (mBits[i] < 0xFF) break;     // Relies on BYTE being unsigned char
		if (i == ARRAYSIZE(mBits))
			return res;
		// This is a simple binary search of the bit space for the bit.  There might be an
		// intrinsic for this operation.  Unknown.
		if ((mBits[i] & 0xF) == 0xF)        // Is the first nibble full?  If so, update offset.
			offset += 4;
		if (((mBits[i] >> offset) & 0x3) == 0x3)
			offset += 2;
		if (((mBits[i] >> offset) & 0x1) == 0x1)
			offset += 1;
		res = i * 8 + offset;              // Calculate the real position in the array
		return res >= Size ? BIT_NOT_FOUND : res;
	}

	class UsedBits
	{
		BitField<Size>& mBitField;

	public:
		UsedBits(BitField<Size>& BitField)
			: mBitField(BitField)
		{
		}

		class Iterator
		{
		public:
			BitField<Size>& mBitField;
			/** The number of bits that have been returned so far, not including the current */
			DWORD mBitsFound;
			/** The current bit position we are at for testing for set bits. */
			DWORD mCurrentPosition;

			Iterator(BitField<Size>& BitField, bool begin)
				: mBitField(BitField)
				, mBitsFound(begin ? 0 : BitField.InUse())
				, mCurrentPosition(BIT_NOT_FOUND)
			{
				(*this)++;
			}

			bool operator!=(const Iterator& other) const
			{
				return mBitsFound != other.mBitsFound;
			}

			DWORD operator*() const
			{
				return mCurrentPosition;
			}

			const Iterator& operator++()
			{
				if (mBitsFound >= mBitsInUse)
				{
					return *this;
				}

				++mBitsFound;
				++mCurrentPosition;

				// Compute the remaining bits in this byte to check for set bits.
				BYTE mask = (BYTE)~((BYTE)(1 << (mCurrentPosition % 8)) - 1);

				DWORD i = (mCurrentPosition >> 3);
				for (; i < ARRAYSIZE(mBits); ++i)
				{
					if ((mBits[i] & mask) > 0)
						break;        // Relies on BYTE being unsigned char
					mask = 0xFF;
				}

				if (i == ARRAYSIZE(mBits))
					return res;
				// This is a simple binary search of the bit space for the bit.  There might be an
				// intrinsic for this operation.  Unknown.
				mask = mBits[i] & mask;
				if ((mask & 0xF) == 0)          // Is the first nibble empty?  If so, update offset.
					offset += 4;
				if (((mask >> offset) & 0x3) == 0)
					offset += 2;
				if (((mask >> offset) & 0x1) == 0)
					offset += 1;
				res = i * 8 + offset;              // Calculate the real position in the array
				return res >= Size ? BIT_NOT_FOUND : res;
				return *this;
			}
		};

		/** Creates an iterator to enumerate the positions of set bits in order. */
		Iterator begin()
		{
			return Iterator(mBitField, true);
		}

		/** End of the iterator. */
		Iterator end()
		{
			return Iterator(mBitField, false);
		}
	};

	inline UsedBits UsedBits()
	{
		return USedBits(*this);
	}

	//
	// Searches the array to find the index of the first used bit in the array
	// returns 0xFFFFFFFFF if one is not found.
	//
	inline DWORD FindFirstUsed() const
	{
		return FindNextUsed(BIT_NOT_FOUND);
	}

	//
	// Searches the array at the index + 1 bit for the next index that is set
	// returns 0xFFFFFFFF if one is not found.
	//
	inline DWORD FindNextUsed(DWORD LastFoundIndex) const
	{
		DWORD offset = 0, res = BIT_NOT_FOUND;
		if (mBitsInUse == 0)
			return res;
		LastFoundIndex++;
		BYTE mask = (BYTE)~((BYTE)(1 << (LastFoundIndex % 8)) - 1);
		DWORD i = (LastFoundIndex >> 3);
		for (; i < ARRAYSIZE(mBits); ++i)
		{
			if ((mBits[i] & mask) > 0)
				break;        // Relies on BYTE being unsigned char
			mask = 0xFF;
		}
		if (i == ARRAYSIZE(mBits))
			return res;
		// This is a simple binary search of the bit space for the bit.  There might be an
		// intrinsic for this operation.  Unknown.
		mask = mBits[i] & mask;
		if ((mask & 0xF) == 0)          // Is the first nibble empty?  If so, update offset.
			offset += 4;
		if (((mask >> offset) & 0x3) == 0)
			offset += 2;
		if (((mask >> offset) & 0x1) == 0)
			offset += 1;
		res = i * 8 + offset;              // Calculate the real position in the array
		return res >= Size ? BIT_NOT_FOUND : res;
	}
};
