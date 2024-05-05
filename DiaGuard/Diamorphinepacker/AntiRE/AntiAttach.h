#include <stdio.h>
#include <Windows.h>

VOID AntiAttach() {
	HANDLE hProcess = GetCurrentProcess();
	HMODULE hMod = GetModuleHandleW(L"ntdll.dll");
	FARPROC func_DbgUiRemoteBreakin = GetProcAddress(hMod, "DbgUiRemoteBreakin");
	WriteProcessMemory(hProcess, func_DbgUiRemoteBreakin, AntiAttach, 6, NULL);

}