#include "../../AntiDump/pch.h"
#include "RemoteDebugging.h"

BOOL CheckRemoteDebuggerPresentAPI(VOID)
{
	BOOL bIsDbgPresent = FALSE;
	CheckRemoteDebuggerPresent(GetCurrentProcess(), &bIsDbgPresent);
	if (bIsDbgPresent = false) {
		exit(0);
	}
}
