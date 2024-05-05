#include <iostream>
#include <Windows.h>

VOID Filesize(DWORD min, DWORD max)
{
    TCHAR szFileName[MAX_PATH];
    DWORD dwBufferSize = GetModuleFileName(NULL, szFileName, MAX_PATH);
    HANDLE hFile = CreateFile(szFileName, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

    DWORD expectedMinSize = min;
    DWORD expectedMaxSize = max;

    DWORD fileSize = GetFileSize(hFile, NULL);
    CloseHandle(hFile);

    if (fileSize >= expectedMinSize && fileSize <= expectedMaxSize) {
        exit(0);
    }
}