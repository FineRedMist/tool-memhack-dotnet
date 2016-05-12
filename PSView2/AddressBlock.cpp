/*****************************************************
	File: pslist.cpp
	Written By: Brent Scriver

	Abstract:  Implementation for the classes (LoWordList,
		AddressBlock, and AddressList) that manage
		the list of addresses that have been found.
******************************************************/

#include "stdafx.h"

namespace PSView2
{
	AddressBlock::AddressBlock()
	{
		mHiWord = 0;
		mLoWords = 0;
	}

	void AddressBlock::Set(WORD wHigh, MemoryBlock block)
	{
		mHiWord = wHigh;
		DWORD dwCur = BIT_NOT_FOUND;
		DWORD i = 0;
		mLoWords = new LoWordList(block.InUse());
		while ((dwCur = block.FindNextUsed(dwCur)) != BIT_NOT_FOUND)	// Use the built in bitfield search functions
		{															// to find the next used bit
			mLoWords->SetValue((WORD)i, (USHORT)dwCur);
			++i;
		}
		assert(i == block.InUse());		// Make sure a match, should work fine however
	}

	AddressBlock::~AddressBlock()
	{
		if (mLoWords)
			delete mLoWords;
	}

	DWORD AddressBlock::GetValue(USHORT idx)
	{
		DWORD dwRes = mLoWords->GetValue(idx);
		if (dwRes == RESULT_NOT_FOUND)
			return dwRes;
		return MAKELONG(dwRes, mHiWord);
	}

	DWORD AddressBlock::GetIndex(USHORT val)
	{
		return mLoWords->GetIndex(val);
	}

	DWORD AddressBlock::GetIndex(DWORD val)
	{
		if (HIWORD(val) != mHiWord)
			return RESULT_NOT_FOUND;
		return mLoWords->GetValue(LOWORD(val));
	}

	DWORD AddressBlock::GetNext(USHORT val)
	{
		DWORD dwAddr = mLoWords->GetNext(LOWORD(val));
		if (dwAddr == RESULT_NOT_FOUND)
			return RESULT_NOT_FOUND;
		return MAKELONG(dwAddr, mHiWord);
	}

	DWORD AddressBlock::GetCount()
	{
		return mLoWords->GetCount();
	}

	WORD AddressBlock::HighWord()
	{
		return mHiWord;
	}

	bool AddressBlock::Delete(USHORT val)
	{
		return mLoWords->Delete(val);
	}

	bool AddressBlock::Delete(DWORD val)
	{
		if (HIWORD(val) != mHiWord)
			return false;
		return mLoWords->Delete(LOWORD(val));
	}

	bool AddressBlock::DeleteAt(USHORT idx)
	{
		return mLoWords->DeleteAt(idx);
	}
}
