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

	void AddressBlock::Set(HIADDR wHigh, MemoryBlock block)
	{
		mHiWord = wHigh;
		DWORD dwCur = BIT_NOT_FOUND;
		USHORT i = 0;
		mLoWords = new LoWordList(block.InUse());
		while ((dwCur = block.FindNextUsed(dwCur)) != BIT_NOT_FOUND)	// Use the built in bitfield search functions
		{															// to find the next used bit
			mLoWords->SetValue(i, (USHORT)dwCur);
			++i;
		}
		assert(i == block.InUse());		// Make sure a match, should work fine however
	}

	AddressBlock::~AddressBlock()
	{
		if (mLoWords)
			delete mLoWords;
	}

	DWORD_PTR AddressBlock::GetValue(USHORT idx)
	{
		DWORD dwRes = mLoWords->GetValue(idx);
		if (dwRes == BIT_NOT_FOUND)
			return dwRes;
		return MAKEVALUE(dwRes, mHiWord);
	}

	SIZE_T AddressBlock::GetIndex(USHORT val)
	{
		return mLoWords->GetIndex(val);
	}

	SIZE_T AddressBlock::GetIndex(SIZE_T val)
	{
		if (GETHIVALUE(val) != mHiWord)
			return BIT_NOT_FOUND;
		auto Result = mLoWords->GetValue(GETLOVALUE(val));
		return Result == BIT_NOT_FOUND
			? RESULT_NOT_FOUND
			: Result;
	}

	SIZE_T AddressBlock::GetNext(USHORT val)
	{
		DWORD dwAddr = mLoWords->GetNext(GETLOVALUE(val));
		if (dwAddr == BIT_NOT_FOUND)
			return RESULT_NOT_FOUND;
		return MAKEVALUE(dwAddr, mHiWord);
	}

	SIZE_T AddressBlock::GetCount()
	{
		return mLoWords->GetCount();
	}

	SIZE_T AddressBlock::HighWord()
	{
		return mHiWord;
	}

	bool AddressBlock::Delete(USHORT val)
	{
		return mLoWords->Delete(val);
	}

	bool AddressBlock::Delete(SIZE_T val)
	{
		if (GETHIVALUE(val) != mHiWord)
			return false;
		return mLoWords->Delete(GETLOVALUE(val));
	}

	bool AddressBlock::DeleteAt(USHORT idx)
	{
		return mLoWords->DeleteAt(idx);
	}
}
