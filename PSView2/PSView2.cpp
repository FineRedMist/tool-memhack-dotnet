/*****************************************************
File: psview.cpp
Written By: Brent Scriver

Abstract:  Implementation for CFriendlyName,
CProcessInfo, and CProcessViewer.  The latter
is the primary class that handles the
enumeration of processes and identifying
information about each process.
******************************************************/

#include "stdafx.h"

const ULONGLONG AndMask[9] =
{
	0,
	0xFF,
	0xFFFF,
	0,
	0xFFFFFFFF,
	0,
	0,
	0,
	0xFFFFFFFFFFFFFFFF
};

namespace PSView2
{
	/*****************************************************
	* class CFriendlyName implementation
	*****************************************************/
	CFriendlyName::CFriendlyName() 	// Protected
	{
	}

	CFriendlyName::CFriendlyName(String^ name) 	// Service
	{
		szName = name->Trim();
		fVisible = true;
		dtDate = DateTime::FromFileTimeUtc(0);
	}

	CFriendlyName::CFriendlyName(String^ name, DateTime date, bool visible)	// Window
	{
		szName = name->Trim();
		dtDate = date;
		fVisible = visible;
	}

	/* The CFriendlyName can be sorted and these are the criteria:
	Visibility first
	Date of window creation
	Then the string name
	*/
	int CFriendlyName::CompareTo(Object^ obj)
	{
		CFriendlyName^ rhs = dynamic_cast<CFriendlyName^>(obj);
		if (op_equality(this, rhs))
			return 0;
		if (op_lessthan(this, rhs))
			return -1;
		return 1;
	}

	bool CFriendlyName::op_equality(CFriendlyName^ _j, CFriendlyName^ _k)
	{
		if (_j->Visible != _k->Visible)
			return false;
		if (_j->Date != _k->Date)
			return false;
		if (String::Compare(_j->Name, _k->Name) != 0)
			return false;
		return true;
	}

	bool CFriendlyName::op_lessthan(CFriendlyName^ _j, CFriendlyName^ _k)
	{
		if (_j->Visible == true && _k->Visible == false)
			return true;
		if (_j->Visible == false && _k->Visible == true)
			return false;
		if (_j->Date < _k->Date)
			return true;
		if (_j->Date > _k->Date)
			return false;
		if (String::Compare(_j->Name, _k->Name) < 0)
			return true;
		return false;
	}

	// This callback assists with the process of identifying the windows for each process
	BOOL CALLBACK EnumWindowsProcHelper(HWND hwnd, LPARAM lparam)
	{
		// Get the list of processes
		gcroot<SortedList<UInt32, CProcessInfo^>^>* pinnedData = reinterpret_cast<gcroot<SortedList<UInt32, CProcessInfo^>^>*>(lparam);
		SortedList<UInt32, CProcessInfo^>^ data = *pinnedData;
		DWORD dwProcess = 0;
		DWORD dwThread = GetWindowThreadProcessId(hwnd, &dwProcess);
		bool visible;
		DateTime date;
		String^ name;

		if (!dwThread || !dwProcess)	// We somehow mystically failed to get one of the thread or process ids
			return TRUE;
		WINDOWINFO wi;
		wi.cbSize = sizeof(wi);
		// Get window information
		if (!GetWindowInfo(hwnd, &wi))
			return TRUE;
		// There are some classes of windows that I don't want to display.  So skip them
		if ((wi.dwExStyle & WS_EX_TOOLWINDOW) || (wi.dwStyle & (WS_POPUP | WS_CHILD | WS_DISABLED)))
			return TRUE;

		// Get the text of the window
		WCHAR szBuf[1024];
		if (!GetWindowTextW(hwnd, szBuf, ARRAYSIZE(szBuf) - 1))
			return TRUE;
		szBuf[ARRAYSIZE(szBuf) - 1] = 0;
		name = gcnew String(szBuf);
		name = name->Trim();
		// If I didn't end up with a name, drop it
		if (name->Length == 0)
			return TRUE;
		// Flag whether the window is visible.  There are some that I still want to display unfortunately
		// which means I'll also have a bunch of others that I don't want to display in the list
		visible = (wi.dwStyle & (WS_MINIMIZE | WS_VISIBLE)) != 0;
		__int64 ftCreated, ftExited, ftKernel, ftUser;

		// I use the thread information to determine when the window is created.  Typically the first window
		// created is really the interesting one so I sort them based on date created
		HANDLE hThread = OpenThread(THREAD_QUERY_INFORMATION, FALSE, dwThread);
		if (hThread && GetThreadTimes(hThread, (FILETIME *)&ftCreated, (FILETIME *)&ftExited, (FILETIME *)&ftKernel, (FILETIME *)&ftUser))
		{
			date = DateTime::FromFileTimeUtc(ftCreated);
		}
		else
		{
			date = DateTime::FromFileTimeUtc(0);
		}
		if (hThread)
			CloseHandle(hThread);
		hThread = 0;
		CFriendlyName^ s = gcnew CFriendlyName(name, date, visible);
		CProcessInfo^ p = dynamic_cast<CProcessInfo^>(data[dwProcess]);
		p->Add(s);

		return TRUE;
	}

	/*****************************************************
	* class CProcessViewer implementation
	*****************************************************/
	SortedList<UInt32, CProcessInfo^>^ CProcessViewer::GetProcessList(String^ IdleProcessName, String^ SystemName)
	{
		DWORD pdwList[1024];  // Supporting a maximum of 1024 processes at once
		DWORD dwCount = 0;
		DWORD dwNeeded = 0;
		HANDLE hSnap = INVALID_HANDLE_VALUE;
		auto res = gcnew SortedList<UInt32, CProcessInfo^>();
		SC_HANDLE sc = 0;

		// The toolhelp32 library helps enumerate processes, modules, heaps and many other 
		// fine little goodies sitting inside applications that are running.
		// Here I snap the process list
		hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
		if (hSnap == 0 || hSnap == INVALID_HANDLE_VALUE)  // Couldn't snap, use a different method
		{
#ifdef DEBUG
			DWORD dwErr = GetLastError();   // Debugging if I do see a failure
#endif
			if (!::EnumProcesses(pdwList, sizeof(pdwList), &dwNeeded))
			{
				dwNeeded = GetLastError();	// Mostly for debugging if I ever see it happen myself
				goto Cleanup;
			}
			dwCount = dwNeeded / sizeof(DWORD);

			// Get the information for the list of processes
			for (unsigned int i = 0; i < dwCount; ++i)
			{
				res[pdwList[i]] = GetProcessInfo(pdwList[i], IdleProcessName, SystemName);
			}
		}
		else	// Snap successful, using that way to get information
		{
			PROCESSENTRY32 procEntry;
			procEntry.dwSize = sizeof(procEntry);
			if (Process32First(hSnap, &procEntry))	// Go through each of the processes
			{
				do
				{
					// Set up the structures
					CProcessInfo^ p = gcnew CProcessInfo(procEntry.th32ProcessID, gcnew String(procEntry.szExeFile));
					if (!p->ID)	// The idle process has the zero id, so save myself some work by special casing it
					{
						p->Add(gcnew CFriendlyName(IdleProcessName));
					}
					else if (p->ID == 4)
					{
						p->Add(gcnew CFriendlyName(SystemName));
					}
					GetProcessFullPath(p);
					res[p->ID] = p;
				} while (Process32Next(hSnap, &procEntry));	// Next process
			}
		}

		{
			// I pin the list so that I can ensure it won't move while enumerating windows
			gcroot<SortedList<UInt32, CProcessInfo^>^>* data = new gcroot<SortedList<UInt32, CProcessInfo^>^>(res);

			// This goes through the windows of the system so that each process can be updated with the list of 
			// windows it has
			EnumDesktopWindows(GetThreadDesktop(GetCurrentThreadId()), EnumWindowsProcHelper, (LPARAM)data);

			delete data;
		}

		sc = OpenSCManager(0, 0, GENERIC_READ);
		if (!sc)
			goto Cleanup;
		BYTE rgBuf[4096];
		ENUM_SERVICE_STATUS_PROCESS *svcs = (ENUM_SERVICE_STATUS_PROCESS *)rgBuf;
		DWORD dwError = 0, dwReturned = 0, dwResume = 0;
		// We now enumerate the services to get the service information and update the processes
#pragma warning( push )
#pragma warning( disable : 4127 )
		while (1)
#pragma warning( pop )
		{
			if (!EnumServicesStatusEx(sc, SC_ENUM_PROCESS_INFO, SERVICE_DRIVER | SERVICE_WIN32, SERVICE_ACTIVE, rgBuf, sizeof(rgBuf), &dwNeeded, &dwReturned, &dwResume, 0))
			{
				dwError = GetLastError();
				if (dwError != ERROR_MORE_DATA)
					break;
			}
			for (unsigned int i = 0; i < dwReturned; ++i)
			{
				// Add the service information
				CFriendlyName^ s = gcnew CFriendlyName(gcnew String(svcs[i].lpDisplayName));
				if (s->Name->Length == 0)
					continue;

				if (!svcs[i].ServiceStatusProcess.dwProcessId)
					continue;
				// Lookup the process to update from the hash table
				CProcessInfo^ pr = dynamic_cast<CProcessInfo^>(res[svcs[i].ServiceStatusProcess.dwProcessId]);
				pr->Add(s);
				pr->IsService = true;
			}
			if (!dwResume)
				break;
		}

		for (int i = 0; i < res->Count; ++i)
		{
			CProcessInfo^ pr = res[res->Keys[i]];
			pr->Sort();
		}

	Cleanup:
		if (hSnap != INVALID_HANDLE_VALUE && hSnap != 0)
			CloseHandle(hSnap);
		if (sc)
			CloseServiceHandle(sc);
		return res;
	}

	void CProcessViewer::GetProcessFullPath(CProcessInfo^ p)
	{
		p->FullPath = nullptr;
		WCHAR szProcessName[MAX_PATH];
		p->IsService = false;

		// If I succeed to open the process with the options needed to modify it, I know it is modifiable
		HANDLE hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_SET_INFORMATION | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, p->ID);
		if (0 == hProcess)  // I failed to open the process for modification so just open it to read the path info
			hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, p->ID);
		else // I succeeded opening the process so set the flag
			p->Modifiable = true;

		if (NULL != hProcess)
		{
			HMODULE hMod;
			DWORD cbNeeded;
			// Enumerate the modules in the process--the first one is the application which we can get the path from
			if (EnumProcessModules(hProcess, &hMod, sizeof(hMod), &cbNeeded))
			{	// Uses size in bytes
				if (GetModuleFileNameExW(hProcess, hMod, szProcessName, sizeof(szProcessName)))
				{
					p->FullPath = gcnew String(szProcessName);
				}
			}
			CloseHandle(hProcess);
		}
	}

	CProcessInfo^ CProcessViewer::GetProcessInfo(DWORD processID, String^ IdleProcessName, String^ SystemName)
	{
		// Get a handle to the process and set some values for the process
		String^ nm = (processID == 0) ? IdleProcessName : ((processID == 4) ? SystemName : nullptr);
		String^ fp = nullptr;
		bool fModifiable = false;

		WCHAR szProcessName[MAX_PATH];

		// Attempt to open the process with all the permissions we would need to modify it
		// and as a backup open te process with enough information to get what we need.
		HANDLE hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_SET_INFORMATION | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, processID);
		if (0 == hProcess)	// Failed to open the process so it isn't modifiable, backup goal
			hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, processID);
		else				// Succeeded to open it so the process is modifiable
			fModifiable = true;

		// Get the process name.
		if (NULL != hProcess)
		{
			HMODULE hMod;
			DWORD cbNeeded;

			// The first module is (typically) the application itself, so we can get the information we need from there
			if (EnumProcessModules(hProcess, &hMod, sizeof(hMod), &cbNeeded))
			{	// Uses size in bytes
				if (GetModuleBaseNameW(hProcess, hMod, szProcessName, sizeof(szProcessName)))
				{
					nm = gcnew String(szProcessName);
				}
				// Uses size in bytes
				if (GetModuleFileNameExW(hProcess, hMod, szProcessName, sizeof(szProcessName)))
				{
					fp = gcnew String(szProcessName);
				}
			}
		}

		CProcessInfo^ res = gcnew CProcessInfo(processID, nm);
		res->Modifiable = fModifiable;
		res->FullPath = fp;

		if (hProcess)
			CloseHandle(hProcess);
		return res;
	}

	/*****************************************************
	* class CProcessInfo implementation
	*****************************************************/
	CProcessInfo::CProcessInfo()
	{
	}

	CProcessInfo::CProcessInfo(DWORD dwProcessID, String^ szProcessName)
	{
		// Default initialization
		dwID = dwProcessID;
		fModifiable = false;
		fIdleProcess = (dwProcessID == 0) ? true : false;
		fSystem = (dwProcessID == 0 || dwProcessID == 4) ? true : false;
		fIsService = false;
		szName = szProcessName;
		szFullPath = nullptr;
		szUser = nullptr;
		rgFriendlyNames = gcnew List<CFriendlyName^>();
	}

	String^ CProcessInfo::DefaultFriendlyName()
	{
		if (rgFriendlyNames->Count == 0)
			return "";
		return (dynamic_cast<CFriendlyName^>(rgFriendlyNames[0]))->Name;
	}

	void CProcessInfo::Sort()
	{
		rgFriendlyNames->Sort();
	}

	void CProcessInfo::Add(CFriendlyName^ name)
	{
		if (!name)
			return;
		rgFriendlyNames->Add(name);
	}

	DWORD CProcessInfo::ID::get()
	{
		return dwID;
	}

	String^ CProcessInfo::Name::get()
	{
		return szName;
	}

	bool CProcessInfo::Modifiable::get()
	{
		return fModifiable;
	}

	void CProcessInfo::Modifiable::set(bool value)
	{
		fModifiable = value;
	}

	bool CProcessInfo::IsService::get()
	{
		return fIsService;
	}

	void CProcessInfo::IsService::set(bool value)
	{
		fIsService = value;
	}

	String^ CProcessInfo::FullPath::get()
	{
		return szFullPath;
	}

	void CProcessInfo::FullPath::set(String^ value)
	{
		szFullPath = value;
	}

	int CProcessInfo::Count::get()
	{
		return rgFriendlyNames->Count;
	}

	CFriendlyName^ CProcessInfo::FriendlyName(int idx)
	{
		return dynamic_cast<CFriendlyName^>(rgFriendlyNames[idx]);
	}

}
