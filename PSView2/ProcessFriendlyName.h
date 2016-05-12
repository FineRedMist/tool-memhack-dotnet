#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;

namespace PSView2
{
	/* class ProcessFriendlyName
	This class describes a set of properties for an associated friendly name for a process.
	A process can have more than one friendly name whether it be an open window or a service.
	The Date variable is used in the window case to determine the first window created by
	an application to help sort out the primary window from derivative windows.
	*/
	public ref class ProcessFriendlyName : public IComparable
	{
	protected:
		String^ szName;
		DateTime dtDate;
		bool fVisible;

		ProcessFriendlyName();

	public:

		ProcessFriendlyName(String^ name);	// Service
		ProcessFriendlyName(String^ name, DateTime date, bool visible);	// Window

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

		/* The ProcessFriendlyName can be sorted and these are the criteria:
		Visibility first
		Date of window creation
		Then the string name
		*/
		virtual int CompareTo(Object^ obj);
		static bool op_equality(ProcessFriendlyName^ _j, ProcessFriendlyName^ _k);
		static bool op_lessthan(ProcessFriendlyName^ _j, ProcessFriendlyName^ _k);
	};
}