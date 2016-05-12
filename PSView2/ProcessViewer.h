/*****************************************************
File: psview.h
Written By: Brent Scriver

Abstract:  Interfaces for ProcessFriendlyName,
ProcessInformation, and ProcessViewer.  The latter
is the primary class that handles the
enumeration of processes and identifying
information about each process.
******************************************************/

#pragma once
using namespace System;
using namespace System::Collections::Generic;
using namespace ProcessTools;

namespace PSView2
{
	public ref class ProcessViewer
	{
	public:
		ProcessViewer() {}

		// Returns the process list
		SortedList<UInt32, ProcessInformation^>^ GetProcessList(String^ IdleProcessName, String^ SystemName);

	protected:
		// Acquires the full path to the process
		void GetProcessFullPath(ProcessInformation^ p);
		// Acquires the name, path, and other details for the process
		ProcessInformation^ GetProcessInfo(DWORD processID, String^ IdleProcessName, String^ SystemName);
	};

}
