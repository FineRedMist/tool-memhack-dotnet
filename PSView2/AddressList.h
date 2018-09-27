#pragma once

#include "AddressBlock.h"

namespace PSView2
{

	// AddressList manages the entire set of addresses found for a value
	class AddressList
	{
	public:
		AddressList(SIZE_T dwBlockCount);
		~AddressList();

		// Add an address
		void Add(DWORD_PTR dwAddr);
		// Indicates completion of the adding process (to commit the last addresses in the temporary storage variables)
		void Complete();

		// Whether a given address exists
		bool Exists(DWORD_PTR dwAddr);

		// # of elements in the list
		SIZE_T Count();

		// Enumeration functions

		// Deletes the value at the current position in the enumeration
		bool Delete();
		// Sets the enumeration pointer to the beginning
		void First();
		// Sets the enumeration pointer to the next value
		void Next();
		// Returns the value at the current position of the enumeration pointer
		DWORD_PTR GetValue();
	protected:
		SIZE_T FindBlockIndex(HIADDR wHigh);
		AddressBlock *FindBlock(HIADDR wHigh);
		AddressList();

		// State variables for building the LoWordList/AddressBlock classes
		HIADDR m_high;
		MemoryBlock m_block;

		AddressBlock *m_blocks;		// The list of blocks
		SIZE_T m_count;				// Total number of addresses stored
		HIADDR m_curIndex;			// Next block to be used
		SIZE_T m_blockCount;			// Total # of blocks

									// Enumeration information
		SIZE_T m_hiIdx;
		SIZE_T m_loIdx;
	};
}
