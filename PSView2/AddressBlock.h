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

#define RESULT_NOT_FOUND	0xFFFFFFFF

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
		void Set(WORD wHigh, MemoryBlock block);

		// Returns the whole address at a given index
		DWORD GetValue(USHORT idx);
		// Returns the index for a LOWORD address
		DWORD GetIndex(USHORT val);
		// Returns the index for a full address
		DWORD GetIndex(DWORD val);
		// Returns the next value given a previous value
		DWORD GetNext(USHORT val);

		// Returns the count of items
		DWORD GetCount();
		// Returns the HIWORD
		WORD HighWord();

		// Delete a given address
		bool Delete(USHORT val);
		// Delete a given address
		bool Delete(DWORD val);
		// Delete a value at a given index
		bool DeleteAt(USHORT idx);

	protected:
		USHORT mHiWord;
		LoWordList *mLoWords;
	};

}