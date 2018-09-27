/*****************************************************
	File: psmod.h
	Written By: Brent Scriver

	Abstract:  Interface for ProcessModifier, the 
		primary class for searching the memory of 
		a process and changing the values.
******************************************************/

#pragma once
using namespace System;
using namespace System::Collections::Generic;
using namespace ProcessTools;

#ifdef _WIN64 
typedef UInt64 AddrType;
#else
typedef UInt32 AddrType;
#endif

namespace PSView2
{

	public ref struct CAddressValue
	{
		AddrType Address;
		AddrType Value;
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


	public ref class ProcessModifier
	{
	private:
		AddressList *mFoundAddresses;
		HANDLE mProcessHandle;
		DWORD mLastProcessError;
		Type^ mSearchType;

		ref class CRegion
		{
		public:
			AddrType StartAddress;
			AddrType AllocationSize;
		};

	public:
		static ProcessModifier^ Open(UInt32 procID);
		void Close();

		property UInt32 LastError
		{
			UInt32 get() { return mLastProcessError; }
		}

		AddrType FindFirst(Object^ obj);
		AddrType FindFirst(Object^ obj, IProgressIndicator^ pb);
		AddrType FindNext(Object^ obj);
		AddrType FindNext(Object^ obj, IProgressIndicator^ pb);
		bool SetValue(AddrType addr, Object^ obj);

		property AddrType Count
		{
			AddrType get();
		}

		property array<CAddressValue^>^ AddressValues
		{
			array<CAddressValue^>^ get();
		}

		~ProcessModifier()
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
		ProcessModifier();
		SIZE_T AddRegions(SIZE_T &wLastHigh, SIZE_T dwAddr, const MEMORY_BASIC_INFORMATION & mbi, List<CRegion^>^ regions);
	};
}
