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

#ifdef _WIN64 
typedef SIZE_T HIADDR;
#define GETHIVALUE(v) ((HIADDR)((((DWORD_PTR)(v)) >> 16) & 0xffffffffffull))
#define MAKEVALUE(lo, hi)  ((HIADDR)(((HIADDR)(((DWORD_PTR)(lo)) & 0xffff)) | ((HIADDR)((HIADDR)(((DWORD_PTR)(hi)) & 0xffffffffffffull))) << 16))
const DWORD_PTR RESULT_NOT_FOUND = 0xFFFFFFFFFFFFFFFFull;
#else
typedef WORD HIADDR;
#define GETHIVALUE(v) HIWORD(v)
#define MAKEVALUE(lo, hi) MAKELONG(lo, hi)
const DWORD RESULT_NOT_FOUND = 0xFFFFFFFFu;
#endif
#define GETLOVALUE(v) LOWORD(v)

#include "BitField.h"
#include "AddressList.h"
#include "ProcessModifier.h"

void DebugPrint(const CHAR * FormatString, ...);

