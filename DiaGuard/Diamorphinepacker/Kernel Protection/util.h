#include <Windows.h>
#include <TlHelp32.h>


//Utility functions that will be used to initialize
namespace Util
{
	BOOL EscalatePrivelages();
	DWORD getRunningProcessId(LPCWSTR processName);
}