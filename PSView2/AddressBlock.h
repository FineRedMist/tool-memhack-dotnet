/*****************************************************
	File: pslist.h
	Written By: Brent Scriver

	Abstract:  Interface for the classes (LoWordList,
		AddressBlock, and AddressList) that manage
		the list of addresses that have been found.
******************************************************/

#pragma once

using namespace System;
using namespace System::Collections::Generic;

#include "LoWordList.h"

// Used to track currently marked addresses that will
// later be converted into a LoWordList class
typedef BitField<65536>	MemoryBlock;

namespace PSView2
{
	// AddressBlock associates the HIWORD with the set
	// of LOWORDs and allows constructing full addresses
	class AddressBlock
	{
	public:
		AddressBlock();
		~AddressBlock();

		// Initializes the class and converts the memory block
		// to an internal LoWordList
		void Set(HIADDR wHigh, MemoryBlock block);

		// Returns the whole address at a given index
		DWORD_PTR GetValue(USHORT idx);
		// Returns the index for a LOWORD address
		SIZE_T GetIndex(USHORT val);
		// Returns the index for a full address
		SIZE_T GetIndex(DWORD_PTR val);
		// Returns the next value given a previous value
		DWORD_PTR GetNext(USHORT val);

		// Returns the count of items
		HIADDR GetCount();
		// Returns the HIWORD
		HIADDR HighWord();

		// Delete a given address
		bool Delete(USHORT val);
		// Delete a given address
		bool Delete(DWORD_PTR val);
		// Delete a value at a given index
		bool DeleteAt(USHORT idx);

	protected:
		HIADDR mHiWord;
		LoWordList *mLoWords;
	};

}