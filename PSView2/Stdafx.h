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
#include "BitField.h"
#include "AddressList.h"
#include "ProcessModifier.h"

void DebugPrint(const CHAR * FormatString, ...);
