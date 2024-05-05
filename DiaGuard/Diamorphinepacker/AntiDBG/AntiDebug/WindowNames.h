#include <iostream>
#include <windows.h>
#include <string>
#include <vector>

using namespace std;

bool IsWindowTitle(const wchar_t* windowTitle)
{
    HWND hwnd = FindWindow(NULL, windowTitle);
    return hwnd != NULL;
}

VOID WindowClassNames()
{
    vector<wstring> windowTitlesToClose = { L"Cheat Engine 7.5", L"UD", L"IDA", L"x64dbg", L"FileGrab", L"Nigger", L"Beammer", L"Process Hacker", L"dexzunpacker" };
    for (const auto& title : windowTitlesToClose)
    {
        if (IsWindowTitle(title.c_str()))
        {
            HWND hwnd = FindWindow(NULL, title.c_str());
            exit(0);
        }
    }
}