/*****************************************************
File: psview.h
Written By: Brent Scriver

Abstract:  Interfaces for CFriendlyName,
CProcessInfo, and CProcessViewer.  The latter
is the primary class that handles the
enumeration of processes and identifying
information about each process.
******************************************************/

#pragma once
using namespace System;
using namespace System::Collections;
using namespace System::Windows::Forms;

namespace PSView2
{
	/* class CFriendlyName
	This class describes a set of properties for an associated friendly name for a process.
	A process can have more than one friendly name whether it be an open window or a service.
	The Date variable is used in the window case to determine the first window created by
	an application to help sort out the primary window from derivative windows.
	*/
	public ref class CFriendlyName : public IComparable
	{
	protected:
		String^ szName;
		DateTime dtDate;
		bool fVisible;

		CFriendlyName();

	public:

		CFriendlyName(String^ name);	// Service
		CFriendlyName(String^ name, DateTime date, bool visible);	// Window

		property String^ Name
		{
			String^ get() { return szName; }
		}
		property DateTime Date
		{
			DateTime get() { return dtDate; }
		}
		property bool Visible
		{
			bool get() { return fVisible; }
		}

		/* The CFriendlyName can be sorted and these are the criteria:
		Visibility first
		Date of window creation
		Then the string name
		*/
		virtual int CompareTo(Object^ obj);
		static bool op_equality(CFriendlyName^ _j, CFriendlyName^ _k);
		static bool op_lessthan(CFriendlyName^ _j, CFriendlyName^ _k);
	};

	public ref class CProcessInfo
	{
	protected:
		UInt32 dwID;
		bool fModifiable;
		bool fIdleProcess;
		bool fSystem;
		bool fIsService;
		String^ szName;
		String^ szFullPath;
		String^ szUser;
		ArrayList^ rgFriendlyNames;

		CProcessInfo();

	public:
		CProcessInfo(DWORD dwProcessID, String^ szProcessName);
		void Sort();
		void Add(CFriendlyName^ name);
		String^ DefaultFriendlyName();

		property DWORD ID
		{
			DWORD get();
		}
		property String^ Name
		{
			String^ get();
		}

		property bool IsService
		{
			bool get();
			void set(bool value);
		}
		property bool Modifiable
		{
			bool get();
			void set(bool value);
		}
		property String^ FullPath
		{
			String^ get();
			void set(String^ value);
		}

		property int Count
		{
			int get();
		}
		CFriendlyName^ FriendlyName(int idx);
	};

	public ref class CProcessViewer
	{
	public:
		CProcessViewer() {}

		// Returns the process list
		SortedList^ GetProcessList(String^ IdleProcessName, String^ SystemName);

	protected:
		// Acquires the full path to the process
		void GetProcessFullPath(CProcessInfo^ p);
		// Acquires the name, path, and other details for the process
		CProcessInfo^ GetProcessInfo(DWORD processID, String^ IdleProcessName, String^ SystemName);
	};

}
