#include "stdafx.h"

namespace PSView2
{
	LoWordList::LoWordList(DWORD dwSize)
	{
		assert(dwSize);
		m_sh = new USHORT[dwSize];
		if (!m_sh)
			throw gcnew System::OutOfMemoryException();
		m_size = dwSize;
		memset(m_sh, 0, dwSize * sizeof(USHORT));
	}

	LoWordList::LoWordList()
	{
	}

	LoWordList::~LoWordList()
	{
		if (m_sh)
			delete[] m_sh;
	}


	// Happens to be symmetric from a real to a fake and back so only one function
	DWORD LoWordList::Translate(DWORD idx)
	{
		return m_size - idx - 1;
	}

	DWORD LoWordList::GetValue(USHORT idx)
	{
		if (idx >= m_size)
			return BIT_NOT_FOUND;
		return m_sh[Translate(idx)];
	}

	DWORD LoWordList::GetIndex(USHORT val)
	{
		if (m_size == 0)
			return BIT_NOT_FOUND;
		// Have to use int--h could conceivably be assigned a -1 value
		int l = 0, m = 0, h = (int)(m_size - 1);
		// Binary search for the value
		while (l <= h)
		{
			m = ((h + l) >> 1);
			if (m_sh[m] == val)
				break;
			if (m_sh[m] < val)
				h = m - 1;
			else
				l = m + 1;
		}

		if (m_sh[m] == val)
			return Translate(m);	// Return the fake to the caller
		return BIT_NOT_FOUND;
	}

	// Returns the next value given a previous value
	DWORD LoWordList::GetNext(USHORT val)
	{
		if (m_size == 0)
			return BIT_NOT_FOUND;
		// Have to use int--h could conceivably be assigned a -1 value
		int l = 0, m = -1, h = (int)(m_size - 1);
		// Binary search for the value
		while (l <= h)
		{
			m = ((h + l) >> 1);
			if (m_sh[m] == val)
				break;
			if (m_sh[m] < val)
				h = m - 1;
			else
				l = m + 1;
		}

		// If we actually found the old value, look for the next one
		if (m_sh[m] == val)
		{
			if (m != 0)
				return m_sh[m - 1];
			return BIT_NOT_FOUND;
		}
		// No old value in which case h has the index of the next largest value.
		// Quick proof: the loop exited because h < l, in fact h == l - 1
		//	If m_sh[m] was less than val then it moved h to m - 1 so m = l from the last pass.  m_sh[h] is > val and m_sh[l] < val so h has the next value
		//	If m_sh[m] was greater than val then it moved l to m + 1 so h = m.  So again m_sh[h] > val and m_sh[l] < val so h has the next value
		// Unless of course h < 0
		if (h < 0)
			return BIT_NOT_FOUND;
		return m_sh[h];

	}

	DWORD LoWordList::GetCount()
	{
		return m_size;
	}

	bool LoWordList::SetValue(USHORT idx, USHORT val)
	{
		if (idx >= m_size)
			return false;
		m_sh[Translate(idx)] = val;
		return true;
	}

	// Delete goes through and finds the value to delete and removes it, moving 
	// all other records up after doing so.
	// It has crossed my mind that this might be optimized by actually reversing
	// the order of values in memory instead since MemHack will iterate through the
	// addresses in order and a FindNext will typically spend most of it's time 
	// deleting entries if the order is reversed, then it should dramatically
	// reduce the amount of copying to shorten the list to an expected average of
	// O(1).  If FindNext proves to be sufficiently slow, that change should
	// probably be made.
	// After doing some performance comparisons with std::hash_set, it turns out
	// my FindFirst operation is incredibly fast but FindNext is slower.  I 
	// also use dramatically less memory than std::hash_set does which can be a 
	// perf win on its own.  So I've decided to do the above and reverse the 
	// indexing transparently.
	bool LoWordList::Delete(USHORT val)
	{
		DWORD idx = GetIndex(val);

		if (idx == BIT_NOT_FOUND)
			return false;

		return DeleteAt((USHORT)idx);
	}

	bool LoWordList::DeleteAt(USHORT idx)
	{
		if (idx >= m_size)
			return false;

		DWORD id = Translate(idx);
		memmove(&m_sh[id], &m_sh[id + 1], (m_size - id - 1) * sizeof(USHORT));
		m_sh[m_size - 1] = 0;
		m_size--;
		return true;
	}
}
