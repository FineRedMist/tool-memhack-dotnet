// stdafx.cpp : source file that includes just the standard includes
// PSView2.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"
#include <stdio.h>

#pragma managed(push, off)

#define DEBUG_BUFSIZE	256
// Simple little debug function for convenience
// Since the typical amount of memory scanned is large, this is expensive so it is by default off
// determined by the static.  However to turn it on I can set a breakpoint in here to enable it

#ifdef DEBUG
void DebugPrint(const CHAR * FormatString, ...)
{
	static bool display = false;

	if (display)
	{
		CHAR outputString[DEBUG_BUFSIZE];

		va_list varArgs;
		va_start(varArgs, FormatString);
		_vsnprintf_s(outputString, DEBUG_BUFSIZE - 1, FormatString, varArgs); // Does not guarantee null termination
		va_end(varArgs);
		outputString[DEBUG_BUFSIZE - 1] = 0;
		OutputDebugStringA(outputString);
	}
}
#else
void DebugPrint(const CHAR *, ...)
{
}
#endif

#pragma managed(pop)
