#pragma once

#include "AddressBlock.h"

namespace PSView2
{

	// AddressList manages the entire set of addresses found for a value
	class AddressList
	{
	public:
		AddressList(DWORD dwBlockCount);
		~AddressList();

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
		AddressBlock *FindBlock(WORD wHigh);
		AddressList();

		// State variables for building the LoWordList/AddressBlock classes
		WORD m_high;
		MemoryBlock m_block;

		AddressBlock *m_blocks;		// The list of blocks
		DWORD m_count;				// Total number of addresses stored
		DWORD m_curIndex;			// Next block to be used
		DWORD m_blockCount;			// Total # of blocks

									// Enumeration information
		DWORD m_hiIdx;
		DWORD m_loIdx;
	};
}
