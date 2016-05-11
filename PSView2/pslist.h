/*****************************************************
	File: pslist.h
	Written By: Brent Scriver

	Abstract:  Interface for the classes (CShortList,
		CAddressBlock, and CAddressList) that manage
		the list of addresses that have been found.
******************************************************/

#pragma once
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;

#define RESULT_NOT_FOUND	0xFFFFFFFF

// Used to track currently marked addresses that will
// later be converted into a CShortList class
typedef BitField<65536>	CMemBlock;

namespace PSView2
{
	// CShortList implements the list of LOWORD portions
	// of found addresses for a given HIWORD
	class CShortList
	{
	public:
		// dwSize indicates the number of LOWORDs to preallocate
		CShortList(DWORD dwSize);
		~CShortList();

		// Returns the LOWORD at a given index
		DWORD GetValue(USHORT idx);
		// Returns the index for a given LOWORD
		DWORD GetIndex(USHORT val);
		// Returns the next value given a previous value
		DWORD GetNext(USHORT val);
		// Returns the current count
		DWORD GetCount();

		// Sets the value at a particular index
		bool SetValue(USHORT idx, USHORT val);
		// Deletes the provided value--returns whether the address was successfully deleted for ensuring higher level counts are updated
		bool Delete(USHORT val);
		// Deletes the value at the given index--returns whether the address was successfully deleted
		bool DeleteAt(USHORT idx);


	protected:
		DWORD Translate(DWORD idx);
		CShortList();
		USHORT *m_sh;	// Allocated short list array
		DWORD m_size;	// Number of elements currently in use in the array
	};

	// CAddressBlock associates the HIWORD with the set
	// of LOWORDs and allows constructing full addresses
	class CAddressBlock
	{
	public:
		CAddressBlock();
		~CAddressBlock();

		// Initializes the class and converts the memory block
		// to a CShortList
		void Set(WORD wHigh, CMemBlock block);

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
		CShortList *mLoWords;
	};


	// CAddressList manages the entire set of addresses found for a value
	class CAddressList
	{
	public:
		CAddressList(DWORD dwBlockCount);
		~CAddressList();

		// Add an address
		void Add(DWORD dwAddr);
		// Indicates completion of the adding process (to commit the last addresses in the temporary storage variables)
		void Complete();

		// Whether a given address exists
		bool Exists(DWORD dwAddr);

		// # of elements in the list
		DWORD Count();

		// Enumeration functions

		// Deletes the value at the current position in the enumeration
		bool Delete();
		// Sets the enumeration pointer to the beginning
		void First();
		// Sets the enumeration pointer to the next value
		void Next();
		// Returns the value at the current position of the enumeration pointer
		DWORD GetValue();
	protected:
		DWORD FindBlockIndex(WORD wHigh);
		CAddressBlock *FindBlock(WORD wHigh);
		CAddressList();

		// State variables for building the CShortList/CAddressBlock classes
		WORD m_high;
		CMemBlock m_block;

		CAddressBlock *m_blocks;	// The list of blocks
		DWORD m_count;				// Total number of addresses stored
		DWORD m_curIndex;			// Next block to be used
		DWORD m_blockCount;			// Total # of blocks

		// Enumeration information
		DWORD m_hiIdx;
		DWORD m_loIdx;
	};
}