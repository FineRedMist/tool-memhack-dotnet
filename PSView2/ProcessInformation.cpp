#include "stdafx.h"

namespace PSView2
{
	/*****************************************************
	* class ProcessInformation implementation
	*****************************************************/
	ProcessInformation::ProcessInformation()
	{
	}

	ProcessInformation::ProcessInformation(DWORD dwProcessID, String^ szProcessName)
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
		rgFriendlyNames = gcnew List<ProcessFriendlyName^>();
	}

	String^ ProcessInformation::DefaultFriendlyName()
	{
		if (rgFriendlyNames->Count == 0)
			return "";
		return (dynamic_cast<ProcessFriendlyName^>(rgFriendlyNames[0]))->Name;
	}

	void ProcessInformation::Sort()
	{
		rgFriendlyNames->Sort();
	}

	void ProcessInformation::Add(ProcessFriendlyName^ name)
	{
		if (!name)
			return;
		rgFriendlyNames->Add(name);
	}

	DWORD ProcessInformation::ID::get()
	{
		return dwID;
	}

	String^ ProcessInformation::Name::get()
	{
		return szName;
	}

	bool ProcessInformation::Modifiable::get()
	{
		return fModifiable;
	}

	void ProcessInformation::Modifiable::set(bool value)
	{
		fModifiable = value;
	}

	bool ProcessInformation::IsService::get()
	{
		return fIsService;
	}

	void ProcessInformation::IsService::set(bool value)
	{
		fIsService = value;
	}

	String^ ProcessInformation::FullPath::get()
	{
		return szFullPath;
	}

	void ProcessInformation::FullPath::set(String^ value)
	{
		szFullPath = value;
	}

	int ProcessInformation::Count::get()
	{
		return rgFriendlyNames->Count;
	}

	ProcessFriendlyName^ ProcessInformation::FriendlyName(int idx)
	{
		return dynamic_cast<ProcessFriendlyName^>(rgFriendlyNames[idx]);
	}

}
