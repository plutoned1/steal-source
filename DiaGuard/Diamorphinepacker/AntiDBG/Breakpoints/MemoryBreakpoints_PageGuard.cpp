#include "../../AntiDump/pch.h"
#include "MemoryBreakpoints_PageGuard.h"

BOOL MemoryBreakpoints_PageGuard()
{
	UCHAR *pMem = NULL;
	SYSTEM_INFO SystemInfo = { 0 };
	DWORD OldProtect = 0;
	PVOID pAllocation = NULL; // Get the page size for the system 

	// Retrieves information about the current system.
	GetSystemInfo(&SystemInfo);

	// Allocate memory 
	pAllocation = VirtualAlloc(NULL, SystemInfo.dwPageSize, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
	if (pAllocation == NULL)
		return FALSE;

	// Write a ret to the buffer (opcode 0xc3)
	RtlFillMemory(pAllocation, 1, 0xC3);

	// Make the page a guard page         
	if (VirtualProtect(pAllocation, SystemInfo.dwPageSize, PAGE_EXECUTE_READWRITE | PAGE_GUARD, &OldProtect) == 0)
		return FALSE;

	__try
	{
		((void(*)())pAllocation)(); // Exception or execution, which shall it be :D?
	}
	__except (GetExceptionCode() == STATUS_GUARD_PAGE_VIOLATION ? EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH)
	{
		VirtualFree(pAllocation, 0, MEM_RELEASE);
		return FALSE;
	}

	VirtualFree(pAllocation, 0, MEM_RELEASE);
	exit(0);
}
