/*****************************************************
	File: pslist.cpp
	Written By: Brent Scriver

	Abstract:  Implementation for the classes (CShortList,
		CAddressBlock, and CAddressList) that manage
		the list of addresses that have been found.
******************************************************/

#include "stdafx.h"

namespace PSView
{
	CShortList::CShortList(DWORD dwSize)
	{
		assert(dwSize);
		m_sh = new USHORT[dwSize];
		if(!m_sh)
			throw new System::OutOfMemoryException();
		m_size = dwSize;
		memset(m_sh, 0, dwSize * sizeof(USHORT));
	}

	CShortList::CShortList()
	{
	}

	CShortList::~CShortList()
	{
		if(m_sh)
			 delete [] m_sh;
	}


	// Happens to be symmetric from a real to a fake and back so only one function
	DWORD CShortList::Translate(DWORD idx)
	{
		return m_size - idx - 1;
	}

	DWORD CShortList::GetValue(USHORT idx)
	{
		if(idx >= m_size)
			return RESULT_NOT_FOUND;
		return m_sh[Translate(idx)];
	}

	DWORD CShortList::GetIndex(USHORT val)
	{
		if(m_size == 0)
			return RESULT_NOT_FOUND;
		// Have to use int--h could conceivably be assigned a -1 value
		int l = 0, m = 0, h = (int) (m_size - 1);
		// Binary search for the value
		while(l <= h)
		{
			m = ((h + l) >> 1);
			if(m_sh[m] == val)
				break;
			if(m_sh[m] < val)
				h = m - 1;
			else
				l = m + 1;
		}

		if(m_sh[m] == val)
			return Translate(m);	// Return the fake to the caller
		return RESULT_NOT_FOUND;
	}

	// Returns the next value given a previous value
	DWORD CShortList::GetNext(USHORT val)
	{
		if(m_size == 0)
			return RESULT_NOT_FOUND;
		// Have to use int--h could conceivably be assigned a -1 value
		int l = 0, m = -1, h = (int) (m_size - 1);
		// Binary search for the value
		while(l <= h)
		{
			m = ((h + l) >> 1);
			if(m_sh[m] == val)
				break;
			if(m_sh[m] < val)
				h = m - 1;
			else
				l = m + 1;
		}

		// If we actually found the old value, look for the next one
		if(m_sh[m] == val)
		{
			if(m != 0) 
				return m_sh[m - 1];
			return RESULT_NOT_FOUND;
		}
		// No old value in which case h has the index of the next largest value.
		// Quick proof: the loop exited because h < l, in fact h == l - 1
		//	If m_sh[m] was less than val then it moved h to m - 1 so m = l from the last pass.  m_sh[h] is > val and m_sh[l] < val so h has the next value
		//	If m_sh[m] was greater than val then it moved l to m + 1 so h = m.  So again m_sh[h] > val and m_sh[l] < val so h has the next value
		// Unless of course h < 0
		if(h < 0)
			return RESULT_NOT_FOUND;
		return m_sh[h];

	}

	DWORD CShortList::GetCount()
	{
		return m_size;
	}

	bool CShortList::SetValue(USHORT idx, USHORT val)
	{
		if(idx >= m_size)
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
	bool CShortList::Delete(USHORT val)
	{
		DWORD idx = GetIndex(val);

		if(idx == RESULT_NOT_FOUND)
			return false;

		return DeleteAt((USHORT) idx);
	}

	bool CShortList::DeleteAt(USHORT idx)
	{
		if(idx >= m_size)
			return false;

		DWORD id = Translate(idx);
		memmove(&m_sh[id], &m_sh[id + 1], (m_size - id - 1) * sizeof(USHORT)); 
		m_sh[m_size - 1] = 0;
		m_size--;
		return true;
	}

	CAddressBlock::CAddressBlock()
	{
		m_high = 0;
		m_lows = 0;
	}

	void CAddressBlock::Set(WORD wHigh, CMemBlock block)
	{
		m_high = wHigh;
		DWORD dwCur = BIT_NOT_FOUND;
		DWORD i = 0;
		m_lows = new CShortList(block.InUse());
		while((dwCur = block.FindNextUsed(dwCur)) != BIT_NOT_FOUND)	// Use the built in bitfield search functions
		{															// to find the next used bit
			m_lows->SetValue((WORD) i, (USHORT) dwCur);
			++i;
		}
		assert(i == block.InUse());		// Make sure a match, should work fine however
	}

	CAddressBlock::~CAddressBlock()
	{
		if(m_lows)
			delete m_lows;
	}

	DWORD CAddressBlock::GetValue(USHORT idx)
	{
		DWORD dwRes = m_lows->GetValue(idx);
		if(dwRes == RESULT_NOT_FOUND)
			return dwRes;
		return MAKELONG(dwRes, m_high);
	}

	DWORD CAddressBlock::GetIndex(USHORT val)
	{
		return m_lows->GetIndex(val);
	}

	DWORD CAddressBlock::GetIndex(DWORD val)
	{
		if(HIWORD(val) != m_high)
			return RESULT_NOT_FOUND;
		return m_lows->GetValue(LOWORD(val));
	}

	DWORD CAddressBlock::GetNext(USHORT val)
	{
		DWORD dwAddr = m_lows->GetNext(LOWORD(val));
		if(dwAddr == RESULT_NOT_FOUND)
			return RESULT_NOT_FOUND;
		return MAKELONG(dwAddr, m_high);
	}

	DWORD CAddressBlock::GetCount()
	{
		return m_lows->GetCount();
	}

	WORD CAddressBlock::HighWord()
	{
		return m_high;
	}

	bool CAddressBlock::Delete(USHORT val)
	{
		return m_lows->Delete(val);
	}

	bool CAddressBlock::Delete(DWORD val)
	{
		if(HIWORD(val) != m_high)
			return false;
		return m_lows->Delete(LOWORD(val));
	}

	bool CAddressBlock::DeleteAt(USHORT idx)
	{
		return m_lows->DeleteAt(idx);
	}


	CAddressList::CAddressList()
	{
	}

	CAddressList::CAddressList(DWORD dwBlockCount)
	{
		m_blockCount = dwBlockCount;
		m_blocks = new CAddressBlock[dwBlockCount];
		m_count = 0;
		m_high = 0;
		m_curIndex = 0;
		m_hiIdx = RESULT_NOT_FOUND;
		m_loIdx = RESULT_NOT_FOUND;
	}

	CAddressList::~CAddressList()
	{
		if(m_blocks)
			delete [] m_blocks;
	}


	void CAddressList::Add(DWORD dwAddr)
	{
		WORD wHigh = HIWORD(dwAddr), wLow = LOWORD(dwAddr);
		// If we are adding an address that is at a new HIWORD block, then we are done 
		// with the previous one and we can "commit" it.
		if(wHigh != m_high)
			Complete();
		m_high = wHigh;
		m_block.Set(wLow);	// Flag the address in the memblock
	}

	void CAddressList::Complete()
	{
		// If there are no bits set, then no work
		if(!m_block.InUse())
		{
			m_high = 0;
			return;
		}

		// Make sure I haven't magically exceeded the amount of space preallocated
		assert(m_curIndex < m_blockCount);
		CAddressBlock *b = &m_blocks[m_curIndex];
		b->Set(m_high, m_block);	// Commit the addresses to the block
		m_count += m_block.InUse();	// Keep track of the total
		m_block.Clear();
		m_high = 0;
		m_curIndex++;
	}

	DWORD CAddressList::FindBlockIndex(WORD wHigh)
	{
		if(m_curIndex == 0)
			return RESULT_NOT_FOUND;

		WORD l = 0, m = 0, h = (WORD) (m_curIndex - 1);
		// Binary search for the block
		while(l <= h)
		{
			m = ((h + l) >> 1);
			if(m_blocks[m].HighWord() == wHigh)
				break;
			if(m_blocks[m].HighWord() > wHigh)
				h = m - 1;
			else
				l = m + 1;
		}

		if(m_blocks[m].HighWord() == wHigh)
			return m;
		return RESULT_NOT_FOUND;
	}

	CAddressBlock *CAddressList::FindBlock(WORD wHigh)
	{
		DWORD idx = FindBlockIndex(wHigh);
		if(idx == RESULT_NOT_FOUND)
			return 0;
		return &m_blocks[idx];
	}

	bool CAddressList::Exists(DWORD dwAddr)
	{
		assert(m_blocks);
		WORD wHigh = HIWORD(dwAddr), wLow = LOWORD(dwAddr);
		CAddressBlock *t = FindBlock(wHigh);

		if(!t)
			return false;

		if(t->GetIndex(wLow) != RESULT_NOT_FOUND)
			return true;

		return false;
	}

	DWORD CAddressList::Count()
	{
		return m_count;
	}

	bool CAddressList::Delete()
	{
		assert(m_blocks);

		if(GetValue() == RESULT_NOT_FOUND)
			return false;

		if(m_blocks[m_hiIdx].DeleteAt((USHORT) m_loIdx))
		{
			// Deleting at the current position will not be a problem.  Delete is 
			// essentially equivalent to a Next as well so Next should not be called
			// to get the next value after a delete.
			m_count--;
			return true;
		}

		return false;
	}

	void CAddressList::First()
	{
		m_hiIdx = 0;
		m_loIdx = 0;
	}

	void CAddressList::Next()
	{
		++m_loIdx;
	}

	DWORD CAddressList::GetValue()
	{
		if(m_hiIdx >= m_curIndex)
			return RESULT_NOT_FOUND;

		if(m_loIdx >= m_blocks[m_hiIdx].GetCount())
		{
			++m_hiIdx;
			m_loIdx = 0;
		}

		while(m_hiIdx < m_curIndex && m_blocks[m_hiIdx].GetCount() == 0)
			++m_hiIdx;

		if(m_hiIdx >= m_curIndex)
			return RESULT_NOT_FOUND;

		return m_blocks[m_hiIdx].GetValue((USHORT) m_loIdx);
	}
}