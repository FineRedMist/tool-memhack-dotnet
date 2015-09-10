// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once


#include <windows.h>
#include <assert.h>
#include <psapi.h>
#include <tlhelp32.h>
#include <accctrl.h>
#include <aclapi.h>
#include <vcclr.h>
#include "bitfield.h"
#include "PSView2.h"
#include "pslist.h"
#include "psint.h"
#include "psmod.h"

void DebugPrint(const CHAR * str, ...);
