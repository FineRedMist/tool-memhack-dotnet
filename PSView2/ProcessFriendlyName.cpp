#include "stdafx.h"

namespace PSView2
{
	/*****************************************************
	* class ProcessFriendlyName implementation
	*****************************************************/
	ProcessFriendlyName::ProcessFriendlyName() 	// Protected
	{
	}

	ProcessFriendlyName::ProcessFriendlyName(String^ name) 	// Service
	{
		szName = name->Trim();
		fVisible = true;
		dtDate = DateTime::FromFileTimeUtc(0);
	}

	ProcessFriendlyName::ProcessFriendlyName(String^ name, DateTime date, bool visible)	// Window
	{
		szName = name->Trim();
		dtDate = date;
		fVisible = visible;
	}

	/* The ProcessFriendlyName can be sorted and these are the criteria:
	Visibility first
	Date of window creation
	Then the string name
	*/
	int ProcessFriendlyName::CompareTo(Object^ obj)
	{
		ProcessFriendlyName^ rhs = dynamic_cast<ProcessFriendlyName^>(obj);
		if (op_equality(this, rhs))
			return 0;
		if (op_lessthan(this, rhs))
			return -1;
		return 1;
	}

	bool ProcessFriendlyName::op_equality(ProcessFriendlyName^ _j, ProcessFriendlyName^ _k)
	{
		if (_j->Visible != _k->Visible)
			return false;
		if (_j->Date != _k->Date)
			return false;
		if (String::Compare(_j->Name, _k->Name) != 0)
			return false;
		return true;
	}

	bool ProcessFriendlyName::op_lessthan(ProcessFriendlyName^ _j, ProcessFriendlyName^ _k)
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
}
