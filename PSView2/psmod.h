/*****************************************************
	File: psmod.h
	Written By: Brent Scriver

	Abstract:  Interface for CProcessModifier, the 
		primary class for searching the memory of 
		a process and changing the values.
******************************************************/

#pragma once
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;

namespace PSView2
{

	public ref struct CAddressValue
	{
		UInt32 Address;
		UInt64 Value;
	};

	union UValueTest
	{
		unsigned char szBuf[8];
		ULONGLONG ull;
		DWORD dw;
		WORD w;
		unsigned char c;
	};

	struct SSearchParams
	{
		union
		{
			ULONGLONG ull;
			DWORD dw;
			WORD w;
			unsigned char c;
		};
		DWORD dwBytes;
	};


	public ref class CProcessModifier
	{
	private:
		CAddressList *mFoundAddresses;
		HANDLE mProcessHandle;
		DWORD mLastProcessError;
		Type^ mSearchType;

		ref class CRegion
		{
		public:
			UInt32 StartAddress;
			UInt32 AllocationSize;
		};

	public:
		CProcessModifier()
		{
			mProcessHandle = 0;
			mLastProcessError = 0;
			mSearchType = nullptr;
			mFoundAddresses = 0;
		}

		bool Open(UInt32 procID);
		void Close();

		property UInt32 LastError
		{
			UInt32 get() { return mLastProcessError; }
		}

		UInt64 FindFirst(Object^ obj);
		UInt64 FindFirst(Object^ obj, IProgressIndicator^ pb);
		UInt64 FindNext(Object^ obj);
		UInt64 FindNext(Object^ obj, IProgressIndicator^ pb);
		bool SetValue(UInt32 addr, Object^ obj);

		property UInt32 Count
		{
			UInt32 get();
		}

		property array<CAddressValue^>^ AddressValues
		{
			array<CAddressValue^>^ get();
		}

		~CProcessModifier()
		{
			Close();
		}
	protected:
		String^ MemoryBlockInfo(const MEMORY_BASIC_INFORMATION & mbi);
		bool GetSearchParams(Object^ obj, SSearchParams& search);
		bool TestAndUpdate(const SSearchParams& srch, UValueTest& tst, BYTE c);
		bool Compare(const SSearchParams& srch, const UValueTest& tst);
		bool SetValueHelper(const SSearchParams& srch, UValueTest& tst);

		DWORD GetByteCount();
		DWORD GetByteCount(Type^ type);

	private:
		DWORD AddRegions(WORD &wLastHigh, DWORD dwAddr, const MEMORY_BASIC_INFORMATION & mbi, List<CRegion^>^ regions);
	};
}
