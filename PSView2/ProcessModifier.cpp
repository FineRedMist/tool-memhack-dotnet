/*****************************************************
	File: psmod.cpp
	Written By: Brent Scriver

	Abstract:  Implementation for ProcessModifier, the 
		primary class for searching the memory of 
		a process and changing the values.
******************************************************/

#include "stdafx.h"

namespace PSView2
{
	bool IsWritableMemory(const MEMORY_BASIC_INFORMATION& mbi)
	{
		return ((mbi.Protect == PAGE_READWRITE || mbi.Protect == PAGE_EXECUTE_READWRITE) && mbi.State == MEM_COMMIT && mbi.Type == MEM_PRIVATE && mbi.RegionSize > 0);
	}

	class CApplicationMemoryBuffer
	{
	protected:
		BYTE m_rgBuf[4096];
		DWORD m_dwBufStart;
		DWORD m_dwBufSize;
		HANDLE m_hProcess;
		MEMORY_BASIC_INFORMATION mbi;

#ifdef DEBUG
		DWORD m_dwVirtualQueryCallCount;
		DWORD m_dwReadProcessMemoryCallCount;
#endif

	public:
		CApplicationMemoryBuffer(HANDLE hProcess)
		{
			m_dwBufStart = 0;
			m_dwBufSize = 0;
			m_hProcess = hProcess;
			memset(&mbi, 0, sizeof(mbi));
#ifdef DEBUG
			m_dwReadProcessMemoryCallCount = m_dwVirtualQueryCallCount = 0;
#endif
		}

		bool Read(DWORD dwAddr, BYTE *rgBuf, DWORD dwLen)
		{
			// If I already have the value in the buffer, just return it
			if (dwAddr >= m_dwBufStart && dwAddr + dwLen <= m_dwBufStart + m_dwBufSize)
			{
				memcpy(rgBuf, &(m_rgBuf[dwAddr - m_dwBufStart]), dwLen);
				return true;
			}

			// I save the MEMORY_BASIC_INFORMATION because VirtualQueryEx is a really expensive call
			if (dwAddr >= ((DWORD)mbi.BaseAddress) && dwAddr < ((DWORD)mbi.BaseAddress) + mbi.RegionSize &&
				!IsWritableMemory(mbi))
				return false;

			// Verifying the address is in a region that is appropriate to modify
			if (dwAddr < ((DWORD)mbi.BaseAddress) || dwAddr >= ((DWORD)mbi.BaseAddress) + mbi.RegionSize)
			{
#ifdef DEBUG
				m_dwVirtualQueryCallCount++;
#endif
				if (!VirtualQueryEx(m_hProcess, (LPVOID)dwAddr, &mbi, sizeof(mbi)))
					return false;
				if (!IsWritableMemory(mbi))
					return false;
			}
			DWORD dwBase = dwAddr & 0xFFFFFC00;	// Get the lowest 1k for the region
			// How much is left from the dwBase address to read in?
			DWORD dwSize = min(mbi.RegionSize - (dwBase - ((DWORD)mbi.BaseAddress)), sizeof(m_rgBuf));
			DWORD dwRead = dwSize;
#ifdef DEBUG
			m_dwReadProcessMemoryCallCount++;
#endif
			if (!ReadProcessMemory(m_hProcess, (LPVOID)dwBase, m_rgBuf, dwSize, &dwRead) || dwRead != dwSize)
			{
				// I might have corrupted the old data in the buffer, so I need to clear out the variables
				// to ensure a future read doesn't get bogus data.
				m_dwBufStart = 0;
				m_dwBufSize = 0;
			}

			// Update fields
			this->m_dwBufStart = dwBase;
			this->m_dwBufSize = dwSize;

			// I know the buffer is now populated so this should complete for sure.  Just
			// avoiding duplicating the code.
			return Read(dwAddr, rgBuf, dwLen);
		}
	};

	bool ProcessModifier::Open(UInt32 procID)
	{
		Close();
		mProcessHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_SET_INFORMATION | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, procID);

		if (mProcessHandle != 0)
		{
			return true;
		}
		else
		{
			mLastProcessError = ::GetLastError();
			return false;
		}
	}

	void ProcessModifier::Close()
	{
		if (mProcessHandle)
		{
			mLastProcessError = 0;
			CloseHandle(mProcessHandle);
			delete mFoundAddresses;
			mFoundAddresses = 0;
			mProcessHandle = 0;
		}
	}

	bool ProcessModifier::SetValueHelper(const SSearchParams& srch, UValueTest& tst)
	{
		switch (srch.dwBytes)
		{
		case 1:
			tst.c = srch.c;
			return true;
		case 2:
			tst.w = srch.w;
			return true;
		case 4:
			tst.dw = srch.dw;
			return true;
		case 8:
			tst.ull = srch.ull;
			return true;
		};
		return false;
	}

	bool ProcessModifier::Compare(const SSearchParams& srch, const UValueTest& tst)
	{
		switch (srch.dwBytes)
		{
		case 1:
			if (srch.c == tst.c)
				return true;
			return false;
		case 2:
			if (srch.w == tst.w)
				return true;
			return false;
		case 4:
			if (srch.dw == tst.dw)
				return true;
			return false;
		case 8:
			if (srch.ull == tst.ull)
				return true;
			return false;
		};
		return false;
	}

	bool ProcessModifier::TestAndUpdate(const SSearchParams& srch, UValueTest& tst, BYTE c)
	{
		for (DWORD i = 0; i < srch.dwBytes - 1; ++i)
			tst.szBuf[i] = tst.szBuf[i + 1];
		tst.szBuf[srch.dwBytes - 1] = c;
		return Compare(srch, tst);
	}

	DWORD ProcessModifier::GetByteCount()
	{
		return GetByteCount(mSearchType);
	}
	DWORD ProcessModifier::GetByteCount(Type^ t)
	{
		if (t == Byte::typeid || t == Char::typeid)
		{
			return 1;
		}
		else if (t == Int16::typeid || t == UInt16::typeid)
		{
			return 2;
		}
		else if (t == Int32::typeid || t == UInt32::typeid)
		{
			return 4;
		}
		else if (t == Int64::typeid || t == UInt64::typeid)
		{
			return 8;
		}

		return 0;
	}

	bool ProcessModifier::GetSearchParams(Object^ obj, SSearchParams& search)
	{
		search.dwBytes = GetByteCount(obj->GetType());
		switch (search.dwBytes)
		{
		case 1:
			search.c = (unsigned char)Convert::ToByte(obj);
			return true;
		case 2:
			search.w = (unsigned short)Convert::ToUInt16(obj);
			return true;
		case 4:
			search.dw = (unsigned int)Convert::ToUInt32(obj);
			return true;
		case 8:
			search.ull = (unsigned __int64)Convert::ToUInt64(obj);
			return true;
		};
		return false;
	}

	String^ ProcessModifier::MemoryBlockInfo(const MEMORY_BASIC_INFORMATION & mbi)
	{
		String^ s = nullptr;
		UInt32 a;
		DWORD dwProtect = mbi.Protect;
		a = (DWORD)mbi.BaseAddress;

		s = "Memory: 0x";
		s = s->Concat(s, a.ToString("X8"));
		s = s->Concat(s, " for ");
		a = mbi.RegionSize;
		s = s->Concat(s, a.ToString());
		s = s->Concat(s, " bytes (");
		switch (mbi.State)
		{
		case MEM_COMMIT:
			s = s->Concat(s, "commit, ");
			break;
		case MEM_FREE:
			s = s->Concat(s, "free, ");
			break;
		case MEM_RESERVE:
			s = s->Concat(s, "reserve, ");
			break;
		default:
			s = s->Concat(s, "unknown(");
			a = mbi.State;
			s = s->Concat(s, a.ToString("X8"));
			s = s->Concat(s, "), ");
			break;
		};
		if (mbi.State != MEM_FREE)
		{
			switch (mbi.Type)
			{
			case MEM_IMAGE:
				s = s->Concat(s, "image, ");
				break;
			case MEM_MAPPED:
				s = s->Concat(s, "mapped, ");
				break;
			case MEM_PRIVATE:
				s = s->Concat(s, "private, ");
				break;
			default:
				s = s->Concat(s, "unknown(");
				a = mbi.Type;
				s = s->Concat(s, a.ToString("X8"));
				s = s->Concat(s, "), ");
				break;
			};
		}
		else
		{
			s = s->Concat(s, "free, ");
		}
		if (!dwProtect && mbi.State == MEM_RESERVE)
		{
			s = s->Concat(s, "reserve");
		}
		if (dwProtect & PAGE_EXECUTE)
		{
			s = s->Concat(s, "EXECUTE");
			dwProtect -= PAGE_EXECUTE;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_EXECUTE_READ)
		{
			s = s->Concat(s, "EXECUTE_READ");
			dwProtect -= PAGE_EXECUTE_READ;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_EXECUTE_READWRITE)
		{
			s = s->Concat(s, "EXECUTE_READWRITE");
			dwProtect -= PAGE_EXECUTE_READWRITE;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_EXECUTE_WRITECOPY)
		{
			s = s->Concat(s, "EXECUTE_WRITECOPY");
			dwProtect -= PAGE_EXECUTE_WRITECOPY;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_NOACCESS)
		{
			s = s->Concat(s, "NOACCESS");
			dwProtect -= PAGE_NOACCESS;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_READONLY)
		{
			s = s->Concat(s, "READONLY");
			dwProtect -= PAGE_READONLY;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_READWRITE)
		{
			s = s->Concat(s, "READWRITE");
			dwProtect -= PAGE_READWRITE;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_WRITECOPY)
		{
			s = s->Concat(s, "WRITECOPY");
			dwProtect -= PAGE_WRITECOPY;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_GUARD)
		{
			s = s->Concat(s, "GUARD");
			dwProtect -= PAGE_GUARD;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}
		if (dwProtect & PAGE_NOCACHE)
		{
			s = s->Concat(s, "NOCACHE");
			dwProtect -= PAGE_NOCACHE;
			if (dwProtect)
				s = s->Concat(s, " | ");
		}

		s = s->Concat(s, ")");
		return s;
	}
	UInt64 ProcessModifier::FindFirst(Object^ obj)
	{
		return FindFirst(obj, nullptr);
	}

	DWORD ProcessModifier::AddRegions(WORD &wLastHigh, DWORD dwAddr, const MEMORY_BASIC_INFORMATION &mbi, List<CRegion^>^ regions)
	{
		DWORD dwHighCount = 0;
		// So it turns out that regions can span multiple 64k blocks, yeah!
		// So loop through and count all the blocks that a region spans
		WORD wLast = HIWORD(dwAddr + mbi.RegionSize - 1);
		for (WORD wTemp = HIWORD(dwAddr); wTemp <= wLast; ++wTemp)
		{
			if (wLastHigh != wTemp || (wTemp == 0 && dwHighCount == 0))
			{
				wLastHigh = wTemp;
				dwHighCount++;
			}
		}
		CRegion^ reg = gcnew CRegion();
		reg->StartAddress = dwAddr;
		reg->AllocationSize = mbi.RegionSize;
		regions->Add(reg);
		return dwHighCount;
	}

	UInt64 ProcessModifier::FindFirst(Object^ obj, IProgressIndicator^ pb)
	{
		mSearchType = obj->GetType();
		UInt64 count = 0;
		SSearchParams search;
#ifdef DEBUG
		DWORD dwQueryCallCount = 0, dwReadCallCount = 0;
#endif
		if (!GetSearchParams(obj, search))
			return 0;

		if (mFoundAddresses)
			delete mFoundAddresses;
		mFoundAddresses = 0;

		auto regions = gcnew List<CRegion^>();
		BYTE rgBuf[4096];
		UValueTest tst;
		tst.ull = 0;
		DWORD dwCurPos = 0;
		DWORD dwToRead, dwRead;
		WORD wLastHigh = 0;
		DWORD dwHighCount = 0;
		int iMax = 0;
		{
			DWORD dwAddr = 0;
			MEMORY_BASIC_INFORMATION mbi;
			while (VirtualQueryEx(mProcessHandle, (VOID *)dwAddr, &mbi, sizeof(mbi)))
			{
#ifdef DEBUG
				dwQueryCallCount++;
#endif
				if (IsWritableMemory(mbi))
				{
					dwHighCount += AddRegions(wLastHigh, dwAddr, mbi, regions);
					iMax += mbi.RegionSize;
					// System::Diagnostics::Debug::WriteLine(MemoryBlockInfo(mbi));
				}

				dwAddr += mbi.RegionSize;
			}
		}
		if (pb)
		{
			pb->SetCurrent(0);
			pb->SetMaximum(iMax);
		}

		// Allocate the address list with the count of the high dwords
		mFoundAddresses = new AddressList(dwHighCount);

		iMax = 0;
		for (int j = 0; j < regions->Count; ++j)
		{
			CRegion^ reg = dynamic_cast<CRegion^>(regions[j]);

			for (DWORD i = 0; i < reg->AllocationSize; ++i)
			{
				if (i % 4096 == 0)
				{
					dwCurPos = 0;
					dwToRead = min(reg->AllocationSize - i, sizeof(rgBuf));
#ifdef DEBUG
					dwReadCallCount++;
#endif
					if (!ReadProcessMemory(mProcessHandle, ((BYTE *)reg->StartAddress) + i, rgBuf, dwToRead, &dwRead))
					{
						mLastProcessError = ::GetLastError();
						break; // This region no longer seems valid, give up.
					}

					if (pb)
					{
						pb->SetCurrent(iMax + i);
					}

				}

				if (TestAndUpdate(search, tst, rgBuf[dwCurPos++]) && i + 1 >= search.dwBytes)
				{
					UInt32 addr = reg->StartAddress + i - search.dwBytes + 1;
					mFoundAddresses->Add(addr);
					count++;
				}
			}
			if (pb)
			{
				iMax += reg->AllocationSize;
				pb->SetCurrent(iMax);
			}
		}
		mFoundAddresses->Complete();
		return count;
	}

	UInt64 ProcessModifier::FindNext(Object^ obj)
	{
		return FindNext(obj, nullptr);
	}

	bool ProcessModifier::SetValue(UInt32 addr, Object^ obj)
	{
		if (mSearchType == nullptr)
			return false;
		if (mSearchType != obj->GetType())
			return false;

		UValueTest tst;
		SSearchParams srch;
		if (!GetSearchParams(obj, srch))
			return false;
		if (!SetValueHelper(srch, tst))
			return false;

		// Has to be a valid address that we previously found.
		if (!mFoundAddresses->Exists(addr))
			return false;

		// Make sure it is still a valid region.
		MEMORY_BASIC_INFORMATION mbi;
		if (!VirtualQueryEx(mProcessHandle, (LPVOID)addr, &mbi, sizeof(mbi)))
			return false;
		if (((DWORD)mbi.BaseAddress) > addr || addr > ((DWORD)mbi.BaseAddress) + mbi.RegionSize)
			return false;

		if (mbi.Protect != PAGE_READWRITE || mbi.State != MEM_COMMIT && mbi.Type != MEM_PRIVATE)
			return false;

		DWORD dwWritten;
		if (!WriteProcessMemory(mProcessHandle, (LPVOID)addr, tst.szBuf, srch.dwBytes, &dwWritten) || dwWritten != srch.dwBytes)
			return false;
		return true;
	}

	UInt64 ProcessModifier::FindNext(Object^ obj, IProgressIndicator^ pb)
	{
		if (mSearchType == nullptr)
			return FindFirst(obj);
		if (mSearchType != obj->GetType())
			return 0;

		SSearchParams search;
		UValueTest tst;
		DWORD lastAddr, addr = 0;
		if (!GetSearchParams(obj, search))
			return 0;

		if (pb)
		{
			pb->SetCurrent(0);
			pb->SetMaximum(mFoundAddresses->Count());
		}

		CApplicationMemoryBuffer buf(mProcessHandle);
		lastAddr = addr = RESULT_NOT_FOUND;
		volatile int i = 0;
		for (mFoundAddresses->First(); (addr = mFoundAddresses->GetValue()) != RESULT_NOT_FOUND; ++i)
		{
			if (pb)
			{
				pb->SetCurrent(i);
			}
			if (!buf.Read(addr, tst.szBuf, search.dwBytes) || !Compare(search, tst))
			{
				mFoundAddresses->Delete();
				continue;
			}
			lastAddr = addr;
			mFoundAddresses->Next();
		}
		if (pb)
		{
			pb->SetCurrent(i);
		}

		return mFoundAddresses->Count();
	}

	ULONGLONG GetTestValue(const UValueTest& tst, DWORD dwByteCount)
	{
		switch (dwByteCount)
		{
		case 1:
			return tst.c;
		case 2:
			return tst.w;
		case 4:
			return tst.dw;
		case 8:
			return tst.ull;
		};
		return 0;
	}

	array<CAddressValue^>^ ProcessModifier::AddressValues::get()
	{
		if (mSearchType == nullptr)
			return nullptr;

		DWORD dwBytes = GetByteCount();
		UValueTest tst;
		DWORD dwRead, addr = 0;
		MEMORY_BASIC_INFORMATION mbi;

		auto al = gcnew List<CAddressValue^>();
		addr = RESULT_NOT_FOUND;
		for (mFoundAddresses->First(); (addr = mFoundAddresses->GetValue()) != RESULT_NOT_FOUND;)
		{
			if (!VirtualQueryEx(mProcessHandle, (LPVOID)addr, &mbi, sizeof(mbi)))
			{
				mFoundAddresses->Delete();
				continue;
			}
			if (!ReadProcessMemory(mProcessHandle, (LPVOID)addr, tst.szBuf, dwBytes, &dwRead) || dwRead != dwBytes)
			{
				mFoundAddresses->Delete();
				continue;
			}

			CAddressValue^ val = gcnew CAddressValue();
			val->Address = addr;
			val->Value = GetTestValue(tst, dwBytes);
			al->Add(val);
			mFoundAddresses->Next();
		}

		return al->ToArray();
	}

	UInt32 ProcessModifier::Count::get()
	{
		if (!mFoundAddresses)
			return 0;
		return (UInt32)mFoundAddresses->Count();
	}

}
