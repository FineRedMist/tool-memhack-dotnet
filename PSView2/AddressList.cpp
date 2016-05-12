
#include "stdafx.h"

namespace PSView2
{
	AddressList::AddressList()
	{
	}

	AddressList::AddressList(DWORD dwBlockCount)
	{
		m_blockCount = dwBlockCount;
		m_blocks = new AddressBlock[dwBlockCount];
		m_count = 0;
		m_high = 0;
		m_curIndex = 0;
		m_hiIdx = RESULT_NOT_FOUND;
		m_loIdx = RESULT_NOT_FOUND;
	}

	AddressList::~AddressList()
	{
		if (m_blocks)
			delete[] m_blocks;
	}


	void AddressList::Add(DWORD dwAddr)
	{
		WORD wHigh = HIWORD(dwAddr), wLow = LOWORD(dwAddr);
		// If we are adding an address that is at a new HIWORD block, then we are done 
		// with the previous one and we can "commit" it.
		if (wHigh != m_high)
			Complete();
		m_high = wHigh;
		m_block.Set(wLow);	// Flag the address in the memblock
	}

	void AddressList::Complete()
	{
		// If there are no bits set, then no work
		if (!m_block.InUse())
		{
			m_high = 0;
			return;
		}

		// Make sure I haven't magically exceeded the amount of space preallocated
		assert(m_curIndex < m_blockCount);
		AddressBlock *b = &m_blocks[m_curIndex];
		b->Set(m_high, m_block);	// Commit the addresses to the block
		m_count += m_block.InUse();	// Keep track of the total
		m_block.Clear();
		m_high = 0;
		m_curIndex++;
	}

	DWORD AddressList::FindBlockIndex(WORD wHigh)
	{
		if (m_curIndex == 0)
			return RESULT_NOT_FOUND;

		WORD l = 0, m = 0, h = (WORD)(m_curIndex - 1);
		// Binary search for the block
		while (l <= h)
		{
			m = ((h + l) >> 1);
			if (m_blocks[m].HighWord() == wHigh)
				break;
			if (m_blocks[m].HighWord() > wHigh)
				h = m - 1;
			else
				l = m + 1;
		}

		if (m_blocks[m].HighWord() == wHigh)
			return m;
		return RESULT_NOT_FOUND;
	}

	AddressBlock *AddressList::FindBlock(WORD wHigh)
	{
		DWORD idx = FindBlockIndex(wHigh);
		if (idx == RESULT_NOT_FOUND)
			return 0;
		return &m_blocks[idx];
	}

	bool AddressList::Exists(DWORD dwAddr)
	{
		assert(m_blocks);
		WORD wHigh = HIWORD(dwAddr), wLow = LOWORD(dwAddr);
		AddressBlock *t = FindBlock(wHigh);

		if (!t)
			return false;

		if (t->GetIndex(wLow) != RESULT_NOT_FOUND)
			return true;

		return false;
	}

	DWORD AddressList::Count()
	{
		return m_count;
	}

	bool AddressList::Delete()
	{
		assert(m_blocks);

		if (GetValue() == RESULT_NOT_FOUND)
			return false;

		if (m_blocks[m_hiIdx].DeleteAt((USHORT)m_loIdx))
		{
			// Deleting at the current position will not be a problem.  Delete is 
			// essentially equivalent to a Next as well so Next should not be called
			// to get the next value after a delete.
			m_count--;
			return true;
		}

		return false;
	}

	void AddressList::First()
	{
		m_hiIdx = 0;
		m_loIdx = 0;
	}

	void AddressList::Next()
	{
		++m_loIdx;
	}

	DWORD AddressList::GetValue()
	{
		if (m_hiIdx >= m_curIndex)
			return RESULT_NOT_FOUND;

		if (m_loIdx >= m_blocks[m_hiIdx].GetCount())
		{
			++m_hiIdx;
			m_loIdx = 0;
		}

		while (m_hiIdx < m_curIndex && m_blocks[m_hiIdx].GetCount() == 0)
			++m_hiIdx;

		if (m_hiIdx >= m_curIndex)
			return RESULT_NOT_FOUND;

		return m_blocks[m_hiIdx].GetValue((USHORT)m_loIdx);
	}
}
