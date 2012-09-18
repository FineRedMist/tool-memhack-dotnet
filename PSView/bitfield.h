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

#define BIT_NOT_FOUND   0xFFFFFFFF
#ifndef ARRAYSIZE
#define ARRAYSIZE(x)		(sizeof(x)/sizeof(x[0]))
#endif

template <DWORD size> class CBitField
{
protected:
    DWORD m_inuse;
    BYTE m_bits[(size >> 3) + (size & 0x7 ? 1 : 0)];
    
public:
    //
    // Constructor, clears all bits
    //
    inline CBitField()
    {
		Clear();
    }

	//
	// Returns the number of bits set
	//
	inline DWORD InUse()
	{
		return m_inuse;
	}

    // 
    // Sets the bit at position pos to 1 and returns the old value
    //
    inline bool Set(DWORD pos)
    {
        assert(pos < size);
        bool old = (m_bits[pos >> 3] & (1 << (pos & 0x7))) != 0;
        m_bits[pos >> 3] |= (1 << (pos & 0x7));
        m_inuse += (old ? 0 : 1);
        return old;
    }

    //
    // Toggles the bit at position pos from 1 to 0 or 0 to 1 and returns the old value
    //
    inline bool Flip(DWORD pos)
    {
        assert(pos < size);
        bool old = (m_bits[pos >> 3] & (1 << (pos & 0x7))) != 0;
        if(old)
            m_bits[pos >> 3] &= ~(1 << (pos & 0x7));
        else
            m_bits[pos >> 3] |= (1 << (pos & 0x7));
        m_inuse += (old ? -1 : 1);
        return old;
    }

    //
    // Clears the bit at position pos to 0 and returns the old value
    //
    inline bool Clear(DWORD pos)
    {
        assert(pos < size);
        bool old = (m_bits[pos >> 3] & (1 << (pos & 0x7))) != 0;
        m_bits[pos >> 3] &= ~(1 << (pos & 0x7));
        m_inuse -= (old ? 1 : 0);
        return old;
    }

    //
    // Clears all entries from the bitarray
    //
	inline void Clear()
	{
        m_inuse = 0;
        for(DWORD i = 0; i < ARRAYSIZE(m_bits); ++i)
            m_bits[i] = 0;
	}

	//
    // Retrieves the bit value at position pos
    //
    inline bool Get(DWORD pos)
    {
        assert(pos < size);
        return (m_bits[pos >> 3] & (1 << (pos & 0x7))) != 0;
    }

    //
    // Searches the array to find the index of the first free bit in the array
    // returns 0xFFFFFFFFF if one is not found.
    //
    inline DWORD FindFirstFree()
    {
        DWORD offset = 0, res = BIT_NOT_FOUND;
        if(m_inuse == size)
            return res;
        for(DWORD i = 0; i < ARRAYSIZE(m_bits); ++i)
            if(m_bits[i] < 0xFF) break;     // Relies on BYTE being unsigned char
        if(i == ARRAYSIZE(m_bits))
            return res;
        // This is a simple binary search of the bit space for the bit.  There might be an
        // intrinsic for this operation.  Unknown.
        if((m_bits[i] & 0xF) == 0xF)        // Is the first nibble full?  If so, update offset.
            offset += 4;
        if(((m_bits[i] >> offset) & 0x3) == 0x3)
            offset += 2;
        if(((m_bits[i] >> offset) & 0x1) == 0x1)
            offset += 1;
        res =  i * 8 + offset;              // Calculate the real position in the array
        return res >= size ? BIT_NOT_FOUND : res;
    }

    //
    // Searches the array to find the index of the first used bit in the array
    // returns 0xFFFFFFFFF if one is not found.
    //
    inline DWORD FindFirstUsed()
    {
        return FindNextUsed(BIT_NOT_FOUND);
    }

    //
    // Searches the array at the index + 1 bit for the next index that is set
    // returns 0xFFFFFFFF if one is not found.
    //
    inline DWORD FindNextUsed(DWORD dwLast)
    {
        DWORD offset = 0, res = BIT_NOT_FOUND;
        if(m_inuse == 0)
            return res;
        dwLast++;
        BYTE mask = (BYTE) ~((BYTE) (1 << (dwLast % 8)) - 1);
        for(DWORD i = (dwLast >> 3); i < ARRAYSIZE(m_bits); ++i)
        {
            if((m_bits[i] & mask) > 0)
                break;        // Relies on BYTE being unsigned char
            mask = 0xFF;
        }
        if(i == ARRAYSIZE(m_bits))
            return res;
        // This is a simple binary search of the bit space for the bit.  There might be an
        // intrinsic for this operation.  Unknown.
        mask = m_bits[i] & mask;
        if((mask & 0xF) == 0)          // Is the first nibble empty?  If so, update offset.
            offset += 4;
        if(((mask >> offset) & 0x3) == 0)
            offset += 2;
        if(((mask >>offset) & 0x1) == 0)
            offset += 1;
        res =  i * 8 + offset;              // Calculate the real position in the array
        return res >= size ? BIT_NOT_FOUND : res;
    }
};

