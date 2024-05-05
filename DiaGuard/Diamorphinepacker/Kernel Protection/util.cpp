#include "util.h"

BOOL Util::EscalatePrivelages()
{
	HANDLE tokenHandle;
	TOKEN_PRIVILEGES tp;
	LUID luid;

	if (!LookupPrivilegeValue(NULL, SE_LOAD_DRIVER_NAME, &luid))
		return FALSE;

	if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &tokenHandle))
		return FALSE;

	tp.PrivilegeCount = 1;
	tp.Privileges[0].Luid = luid;
	tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

	if (!AdjustTokenPrivileges(tokenHandle, FALSE, &tp, sizeof(TOKEN_PRIVILEGES), NULL, NULL))
		return FALSE;

	CloseHandle(tokenHandle);
	return TRUE;
}

//Obtain a process ID for a given process
DWORD Util::getRunningProcessId(LPCWSTR processName)
{
	HANDLE hSnap;
	PROCESSENTRY32 procEntry;

	hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	procEntry.dwSize = sizeof(PROCESSENTRY32);

	if (Process32First(hSnap, &procEntry))
	{
		if (lstrcmp(procEntry.szExeFile, processName) == 0)
		{
			return procEntry.th32ProcessID;
		}
	}

	while (Process32Next(hSnap, &procEntry))
	{
		if (lstrcmp(procEntry.szExeFile, processName) == 0)
		{
			return procEntry.th32ProcessID;
		}
	}

	return 0;
}
