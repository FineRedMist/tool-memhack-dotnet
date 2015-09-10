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
void DebugPrint(const CHAR * str, ...)
{
	static bool fDisplay = false;

	if (fDisplay)
	{
		CHAR buf[DEBUG_BUFSIZE];

		va_list val;
		va_start(val, str);
		_vsnprintf_s(buf, DEBUG_BUFSIZE - 1, str, val); // Does not guarantee null termination
		va_end(val);
		buf[DEBUG_BUFSIZE - 1] = 0;
		OutputDebugStringA(buf);
	}
}
#else
void DebugPrint(const CHAR *, ...)
{
}
#endif

#pragma managed(pop)
