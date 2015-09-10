/*****************************************************
	File: psmod.h
	Written By: Brent Scriver

	Abstract:  Interface for CProcessModifier, the 
		primary class for searching the memory of 
		a process and changing the values.
******************************************************/

#pragma once
using namespace System;
using namespace System::Collections;
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
		CAddressList *m_addrs;
		HANDLE m_hProcess;
		DWORD m_dwErr;
		Type^ m_srchType;

	public:
		CProcessModifier() 
		{
			m_hProcess = 0;
			m_dwErr = 0;
			m_srchType = nullptr;
			m_addrs = 0;
		}

		bool Open(UInt32 procID);
		void Close();

		property UInt32 LastError
		{
			UInt32 get() { return m_dwErr; }
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
		DWORD AddRegions(WORD &wLastHigh, DWORD dwAddr, const MEMORY_BASIC_INFORMATION & mbi, ArrayList^ regions);

		DWORD GetByteCount();
		DWORD GetByteCount(Type^ type);
	};
}