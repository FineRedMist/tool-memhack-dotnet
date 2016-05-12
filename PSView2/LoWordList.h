#pragma once

namespace PSView2
{
	// LoWordList implements the list of LOWORD portions
	// of found addresses for a given HIWORD
	class LoWordList
	{
	public:
		// dwSize indicates the number of LOWORDs to preallocate
		LoWordList(DWORD dwSize);
		~LoWordList();

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
		LoWordList();
		USHORT *m_sh;	// Allocated short list array
		DWORD m_size;	// Number of elements currently in use in the array
	};
}
