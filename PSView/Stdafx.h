// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x500
#endif

#ifndef WINVER
#define WINVER 0x500
#endif

#ifdef _DEBUG
#ifndef DEBUG
#define DEBUG 1
#endif
#endif

#include <windows.h>
#include <assert.h>
#include <psapi.h>
#include <tlhelp32.h>
#include <accctrl.h>
#include <aclapi.h>
#include "bitfield.h"
#include "PSView.h"
#include "pslist.h"
#include "psint.h"
#include "psmod.h"

void DebugPrint(const CHAR * str, ...);
