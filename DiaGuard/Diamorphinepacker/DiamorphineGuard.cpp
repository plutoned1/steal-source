#include <iostream>
#include <ntstatus.h>
#include <stdio.h>
#include <stdint.h>
#include <windows.h>
#include <tlhelp32.h>
#include <string>
#include <locale.h>

#include <winhttp.h>
#pragma comment(lib, "winhttp.lib")

#include <iphlpapi.h>

#include "DiamorphineGuard.h"
#include "Injection/injector.h"

#include "detours/detours.h"
#include "Encryption/PROTECT.h"

#pragma comment(lib, "ntdll.lib")

#include "AntiRE/AntiRE.h"
#include "AntiRE/AntiAttach.h"
#include "AntiRe/Lazy.h"
#include "AntiRe/skStr.h"
#include "AntiDBG/Mutation/Mutation.h"
#include "AntiDBG/AntiDebug/AntiDebug.h"
#include "AntiDBG/AntiDebug/WindowNames.h"

#include "AntiDump/ErasePEHeaderFromMemory.h"
#include "AntiDump/SizeOfImage.h"
#include "AntiDump/SizeOfFile.h"

#include "AntiDBG/IsRemote/RemoteDebugging.h"
#include "AntiDBG/Breakpoints/HardwareBreakpoints.h"
#include "AntiDBG/Breakpoints/MemoryBreakpoints_PageGuard.h"
#include "AntiDBG/tls_callback/TLS_callbacks.h"
#include "AntiDBG/AntiBuffer/WriteWatch.h"

#include "AntiDBG/VxLang/vxlib.h"

#include "Auth/Auth.hpp"

#include <corecrt_memory.h>
#include "Kernel Protection/util.h"
#include "Kernel Protection/driver.h"
#include "Kernel Protection/error.h"
#include "Mapper/GDRVLoader.h"

extern tNtQuerySystemInformation oNtQuerySystemInformation;

//Global information that will be needed for the process
typedef struct GLOBALS
{
	HANDLE hProcess;
	DWORD processProcID;
	BOOL isInjected;
	int error;
};

//Define the globals
extern GLOBALS Globals;
extern FILE* f;

FILE* f;
GLOBALS Globals;

#define USE_VM_MACRO
#pragma optimize("", off) 

PRG_DATA rgdata;

bool IsValid = true;
using namespace std;

extern "C" NTSTATUS NTAPI RtlAdjustPrivilege(ULONG Privilege, BOOLEAN Enable, BOOLEAN CurrThread, PBOOLEAN StatusPointer);
extern "C" NTSTATUS NTAPI NtRaiseHardError(LONG ErrorStatus, ULONG Unless1, ULONG Unless2, PULONG_PTR Unless3, ULONG ValidResponseOption, PULONG ResponsePointer);

auto check_section_integrityy(const char* section_name, bool fix = false) -> bool
{
	const auto map_file = [](HMODULE hmodule) -> std::tuple<std::uintptr_t, HANDLE>
		{
			wchar_t filename[MAX_PATH];
			DWORD size = MAX_PATH;
			QueryFullProcessImageName(GetCurrentProcess(), 0, filename, &size);


			const auto file_handle = CreateFile(filename, GENERIC_READ, FILE_SHARE_READ, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
			if (!file_handle || file_handle == INVALID_HANDLE_VALUE)
			{
				return { 0ull, nullptr };
			}

			const auto file_mapping = CreateFileMapping(file_handle, 0, PAGE_READONLY, 0, 0, 0);
			if (!file_mapping)
			{
				CloseHandle(file_handle);
				return { 0ull, nullptr };
			}

			return { reinterpret_cast<std::uintptr_t>(MapViewOfFile(file_mapping, FILE_MAP_READ, 0, 0, 0)), file_handle };
		};

	const auto hmodule = GetModuleHandle(0);
	if (!hmodule) return true;

	const auto base_0 = reinterpret_cast<std::uintptr_t>(hmodule);
	if (!base_0) return true;

	const auto dos_0 = reinterpret_cast<IMAGE_DOS_HEADER*>(base_0);
	if (dos_0->e_magic != IMAGE_DOS_SIGNATURE) return true;

	const auto nt_0 = reinterpret_cast<IMAGE_NT_HEADERS*>(base_0 + dos_0->e_lfanew);
	if (nt_0->Signature != IMAGE_NT_SIGNATURE) return true;

	auto section_0 = IMAGE_FIRST_SECTION(nt_0);

	const auto [base_1, file_handle] = map_file(hmodule);
	if (!base_1 || !file_handle || file_handle == INVALID_HANDLE_VALUE) return true;

	const auto dos_1 = reinterpret_cast<IMAGE_DOS_HEADER*>(base_1);
	if (dos_1->e_magic != IMAGE_DOS_SIGNATURE)
	{
		UnmapViewOfFile(reinterpret_cast<void*>(base_1));
		CloseHandle(file_handle);
		return true;
	}

	const auto nt_1 = reinterpret_cast<IMAGE_NT_HEADERS*>(base_1 + dos_1->e_lfanew);
	if (nt_1->Signature != IMAGE_NT_SIGNATURE ||
		nt_1->FileHeader.TimeDateStamp != nt_0->FileHeader.TimeDateStamp ||
		nt_1->FileHeader.NumberOfSections != nt_0->FileHeader.NumberOfSections)
	{
		UnmapViewOfFile(reinterpret_cast<void*>(base_1));
		CloseHandle(file_handle);
		return true;
	}

	auto section_1 = IMAGE_FIRST_SECTION(nt_1);

	bool patched = false;
	for (auto i = 0; i < nt_1->FileHeader.NumberOfSections; ++i, ++section_0, ++section_1)
	{
		if (strcmp(reinterpret_cast<char*>(section_0->Name), section_name) ||
			!(section_0->Characteristics & IMAGE_SCN_MEM_EXECUTE)) continue;

		for (auto i = 0u; i < section_0->SizeOfRawData; ++i)
		{
			const auto old_value = *reinterpret_cast<BYTE*>(base_1 + section_1->PointerToRawData + i);

			if (*reinterpret_cast<BYTE*>(base_0 + section_0->VirtualAddress + i) == old_value)
			{
				continue;
			}

			if (fix)
			{
				DWORD new_protect{ PAGE_EXECUTE_READWRITE }, old_protect;
				VirtualProtect((void*)(base_0 + section_0->VirtualAddress + i), sizeof(BYTE), new_protect, &old_protect);
				*reinterpret_cast<BYTE*>(base_0 + section_0->VirtualAddress + i) = old_value;
				VirtualProtect((void*)(base_0 + section_0->VirtualAddress + i), sizeof(BYTE), old_protect, &new_protect);
			}

			patched = true;
		}

		break;
	}

	UnmapViewOfFile(reinterpret_cast<void*>(base_1));
	CloseHandle(file_handle);

	return patched;
}

std::string checksumm()
{
	auto exec = [&](const char* cmd) -> std::string
		{
			uint16_t line = -1;
			std::array<char, 128> buffer;
			std::string result;
			std::unique_ptr<FILE, decltype(&_pclose)> pipe(_popen(cmd, "r"), _pclose);
			if (!pipe) {
				throw std::runtime_error(("popen() failed!"));
			}

			while (fgets(buffer.data(), buffer.size(), pipe.get()) != nullptr) {
				result = buffer.data();
			}
			return result;
		};

	char rawPathName[MAX_PATH];
	GetModuleFileNameA(NULL, rawPathName, MAX_PATH);

	return exec(("certutil -hashfile \"" + std::string(rawPathName) + ("\" MD5 | find /i /v \"md5\" | find /i /v \"certutil\"")).c_str());
}

bool constantTimeStringComparee(const char* str1, const char* str2, size_t length) {
	int result = 0;

	for (size_t i = 0; i < length; ++i) {
		result |= str1[i] ^ str2[i];
	}

	return result == 0;
}

// code submitted in pull request from https://github.com/BINM7MD
BOOL bDataComparee(const BYTE* pData, const BYTE* bMask, const char* szMask)
{
	for (; *szMask; ++szMask, ++pData, ++bMask)
	{
		if (*szMask == 'x' && *pData != *bMask)
			return FALSE;
	}
	return (*szMask) == NULL;
}

DWORD64 FindPatternn(BYTE* bMask, const char* szMask)
{
	MODULEINFO mi{ };
	GetModuleInformation(GetCurrentProcess(), GetModuleHandleA(NULL), &mi, sizeof(mi));

	DWORD64 dwBaseAddress = DWORD64(mi.lpBaseOfDll);
	const auto dwModuleSize = mi.SizeOfImage;

	for (auto i = 0ul; i < dwModuleSize; i++)
	{
		if (bDataComparee(PBYTE(dwBaseAddress + i), bMask, szMask))
			return DWORD64(dwBaseAddress + i);
	}
	return NULL;
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
		ipAddress = DIA("Failed To Get IP");
	}

	HINTERNET hSession = WinHttpOpen(DIA(L"Bailey/1.0"),
		WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
		WINHTTP_NO_PROXY_NAME,
		WINHTTP_NO_PROXY_BYPASS,
		0);

	HINTERNET hConnect = WinHttpConnect(hSession,
		DIA(L"discordapp.com"),
		INTERNET_DEFAULT_HTTPS_PORT,
		0);

	HINTERNET hRequest = WinHttpOpenRequest(hConnect,
		DIA(L"POST"),
		DIA(L"/api/webhooks/1194428335669911593/9q-5Ii2WV8UCHPGnmMToBme0canHNyYEddGshXkO3py3Yoha1A9qDY13_GE5HmSWmEWb"),
		NULL,
		WINHTTP_NO_REFERER,
		WINHTTP_DEFAULT_ACCEPT_TYPES,
		WINHTTP_FLAG_SECURE);

	std::string stringtosend = desc + " " + ipAddress;
	std::string title = DIA("Debugger Detected!");
	std::string body = DIA("DiaGuard");
	std::string color = DIA("16711680"); // Decimal color
	std::string request_body = DIA("{\"username\": \"") + body + DIA("\",\"content\": null,\"embeds\": [{\"title\": \"") + title + DIA("\",\"description\": \"") + stringtosend + DIA("\",\"footer\": {\"text\": \"") + body + DIA("\"},\"color\": ") + color + DIA(" }], \"attachments\": []}");

	BOOL bResults = WinHttpSendRequest(hRequest,
		DIA(L"Content-Type: application/json\r\n"),
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

VOID SendToLogs(std::string log, LPCWSTR  webhook) {

	HINTERNET hSession = WinHttpOpen(DIA(L"Bailey/1.0"),
		WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
		WINHTTP_NO_PROXY_NAME,
		WINHTTP_NO_PROXY_BYPASS,
		0);

	HINTERNET hConnect = WinHttpConnect(hSession,
		DIA(L"discordapp.com"),
		INTERNET_DEFAULT_HTTPS_PORT,
		0);
	//   /api/webhooks/channelID/example
	HINTERNET hRequest = WinHttpOpenRequest(hConnect,
		DIA(L"POST"),
		webhook,
		NULL,
		WINHTTP_NO_REFERER,
		WINHTTP_DEFAULT_ACCEPT_TYPES,
		WINHTTP_FLAG_SECURE);

	std::string title = DIA("DiaGuard Logs");
	std::string body = DIA("DiaGuard");
	std::string color = DIA("16711680"); // Decimal color
	std::string request_body = DIA("{\"username\": \"") + body + DIA("\",\"content\": null,\"embeds\": [{\"title\": \"") + title + DIA("\",\"description\": \"") + log + DIA("\",\"footer\": {\"text\": \"") + body + DIA("\"},\"color\": ") + color + DIA(" }], \"attachments\": []}");

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

// Original MessageBox function pointer
static int (WINAPI* TrueMessageBoxA)(HWND, LPCSTR, LPCSTR, UINT) = MessageBoxA;

// Function prototype for NtRaiseHardError
typedef NTSTATUS(NTAPI* PDEF_NTRAISEHARDERROR)(NTSTATUS ErrorStatus, ULONG NumberOfParameters, ULONG UnicodeStringParameterMask, PULONG_PTR Parameters, ULONG ValidResponseOption, PULONG Response);
PDEF_NTRAISEHARDERROR TrueNtRaiseHardError = NULL;


int WINAPI DetouredMessageBoxA(HWND hWnd, LPCSTR lpText, LPCSTR lpCaption, UINT uType) {

	// Detach detours
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourTransactionCommit();

	BOOLEAN PrivilegeState = FALSE;
	ULONG ErrorResponse = 0;

	// Attempt to adjust privilege
	if (!RtlAdjustPrivilege(19, TRUE, FALSE, &PrivilegeState)) {
		return -1; // Return a custom error code to indicate failure
	}

	// Attempt to raise a hard error
	NTSTATUS raiseErrorResult = NtRaiseHardError(STATUS_IN_PAGE_ERROR, 0, 0, NULL, 6, &ErrorResponse);

	// Check the result of raising the hard error
	if (!NT_SUCCESS(raiseErrorResult)) {
		return -1; // Return a custom error code to indicate failure
	}

	return 0; // Return 0 to indicate success
}

NTSTATUS NTAPI HookedNtRaiseHardError(NTSTATUS ErrorStatus, ULONG NumberOfParameters, ULONG UnicodeStringParameterMask, PULONG_PTR Parameters, ULONG ValidResponseOption, PULONG Response) {
	// Detach detours
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(&(PVOID&)TrueMessageBoxA, DetouredMessageBoxA);
	DetourDetach(&(PVOID&)TrueNtRaiseHardError, HookedNtRaiseHardError);
	LONG detourResult = DetourTransactionCommit();

	// Check the result of detaching detours
	if (detourResult != NO_ERROR) {
		return STATUS_UNSUCCESSFUL; // Return an error status code
	}

	BOOLEAN PrivilegeState = FALSE;
	ULONG ErrorResponse = 0;

	// Attempt to adjust privilege
	if (!RtlAdjustPrivilege(19, TRUE, FALSE, &PrivilegeState)) {
		return STATUS_PRIVILEGE_NOT_HELD; // Return a privilege-related error status code
	}

	// Attempt to raise a hard error
	NTSTATUS raiseErrorResult = NtRaiseHardError(STATUS_IN_PAGE_ERROR, 0, 0, NULL, 6, &ErrorResponse);

	// Check the result of raising the hard error
	if (!NT_SUCCESS(raiseErrorResult)) {
		return raiseErrorResult; // Return the error status code received from NtRaiseHardError
	}

	return STATUS_SUCCESS; // Return success status code if all operations succeeded
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

std::vector<std::wstring> GetModuleNames() {
	std::vector<std::wstring> moduleNames;
	HMODULE hModules[1024];
	DWORD needed;

	if (EnumProcessModules(GetCurrentProcess(), hModules, sizeof(hModules), &needed)) {
		for (DWORD i = 0; i < (needed / sizeof(HMODULE)); i++) {
			WCHAR moduleName[MAX_PATH];
			if (GetModuleFileName(hModules[i], moduleName, MAX_PATH)) {
				moduleNames.push_back(moduleName);
			}
		}
	}

	return moduleNames;
}

using namespace std;

bool IsCorrectTargetArchitecture(HANDLE hProc) {
	BOOL bTarget = FALSE;
	if (!IsWow64Process(hProc, &bTarget)) {
		exit(1);
	}

	BOOL bHost = FALSE;
	IsWow64Process(GetCurrentProcess(), &bHost);

	return (bTarget == bHost);
}

DWORD GetProcessIdByName(wchar_t* name) {
	PROCESSENTRY32 entry;
	entry.dwSize = sizeof(PROCESSENTRY32);

	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

	if (Process32First(snapshot, &entry) == TRUE) {
		while (Process32Next(snapshot, &entry) == TRUE) {
			if (_wcsicmp(entry.szExeFile, name) == 0) {
				CloseHandle(snapshot);
				return entry.th32ProcessID;
			}
		}
	}

	CloseHandle(snapshot);
	return 0;
}

VOID ProtectProcess() {
	VL_OBFUSCATION_BEGIN
	std::ofstream file(DIA("C:\\Windows\\System32\\Protection.dll"), std::ios_base::out | std::ios_base::binary);
	file.write((char*)bytes.data(), bytes.size());
	file.close();

	wchar_t dllPath[MAX_PATH];
	wchar_t TargetProcessName[MAX_PATH];
	wcscpy_s(dllPath, MAX_PATH, DIA(L"C:\\Windows\\System32\\Protection.dll"));
	wcscpy_s(TargetProcessName, MAX_PATH, DIA(L"RuntimeBroker.exe"));
	DWORD PID = GetProcessIdByName(TargetProcessName);

	if (PID == 0) {
		exit(1);
	}

	TOKEN_PRIVILEGES priv = { 0 };
	HANDLE hToken = NULL;
	if (OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken)) {
		priv.PrivilegeCount = 1;
		priv.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

		if (LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &priv.Privileges[0].Luid))
			AdjustTokenPrivileges(hToken, FALSE, &priv, 0, NULL, NULL);

		CloseHandle(hToken);
	}

	HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, PID);
	if (!hProc) {
		DWORD Err = GetLastError();
		exit(1);
	}

	if (!IsCorrectTargetArchitecture(hProc)) {
		CloseHandle(hProc);
		exit(1);
	}

	if (GetFileAttributes(dllPath) == INVALID_FILE_ATTRIBUTES) {
		CloseHandle(hProc);
		exit(1);
	}

	std::ifstream File(dllPath, std::ios::binary | std::ios::ate);

	if (File.fail()) {
		File.close();
		CloseHandle(hProc);
		exit(1);
	}

	auto FileSize = File.tellg();
	if (FileSize < 0x1000) {
		File.close();
		CloseHandle(hProc);
		exit(1);
	}

	BYTE* pSrcData = new BYTE[(UINT_PTR)FileSize];
	if (!pSrcData) {
		File.close();
		CloseHandle(hProc);
		exit(0);
	}

	File.seekg(0, std::ios::beg);
	File.read((char*)(pSrcData), FileSize);
	File.close();
	if (!ManualMapDll(hProc, pSrcData, FileSize)) {
		delete[] pSrcData;
		CloseHandle(hProc);
		exit(1);
	}
	delete[] pSrcData;
	CloseHandle(hProc);
	VL_OBFUSCATION_END
}

bool IsSuspiciousMemoryAccessDetected(const void* targetAddress, const size_t bufferSize) {
	DWORD oldProtect;
	if (VirtualProtect(const_cast<void*>(targetAddress), bufferSize, PAGE_EXECUTE_READWRITE, &oldProtect)) {
		memset(const_cast<void*>(targetAddress), 0, bufferSize);  // Clear the buffer at the target address
		VirtualProtect(const_cast<void*>(targetAddress), bufferSize, oldProtect, &oldProtect);

		if (IsBadReadPtr(targetAddress, bufferSize)) {
			return true;
		}
	}
	return false;
}

VOID ExtraAntiDebugging() {

	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		RG_SetCallbacks();
		CheckMemory();
		CheckCRC();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID BSOD() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(PVOID&)TrueMessageBoxA, DetouredMessageBoxA);
		DetourDetach(&(PVOID&)TrueNtRaiseHardError, HookedNtRaiseHardError);
		DetourTransactionCommit();
		BOOLEAN PrivilegeState = FALSE;
		ULONG ErrorResponse = 0;
		RtlAdjustPrivilege(19, TRUE, FALSE, &PrivilegeState);
		NtRaiseHardError(STATUS_IN_PAGE_ERROR, 0, 0, NULL, 6, &ErrorResponse); // There are many Crash reasons
		VL_OBFUSCATION_END;
	}
}

VOID banning() {
	HKEY hKey = NULL;
	LPCWSTR keyPath = DIA(L"Software\\Microsoft\\Windows\\CurrentVersion");
	LPCWSTR RegName1 = DIA(L"Sinister");
	LPCWSTR valueData = DIA(L"1");
	LONG result = RegCreateKeyExW(HKEY_LOCAL_MACHINE, keyPath, 0, NULL, 0, KEY_WRITE, NULL, &hKey, NULL);
	if (result == ERROR_SUCCESS) {
		result = RegSetValueExW(hKey, RegName1, 0, REG_SZ, (BYTE*)valueData, sizeof(WCHAR) * (wcslen(valueData) + 1));
		RegCloseKey(hKey);
	}
}

VOID AntiSuspend() {
	SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_AWAYMODE_REQUIRED);
}

VOID NOSUSPEND() {
	auto currentTime = std::chrono::system_clock::now();
	std::time_t currentTime_t = std::chrono::system_clock::to_time_t(currentTime);
	std::tm tmCurrentTime;
	if (gmtime_s(&tmCurrentTime, &currentTime_t) != 0) {
	}
	int currentMinutes = tmCurrentTime.tm_min;
	std::this_thread::sleep_for(std::chrono::seconds(65));
	auto newTime = std::chrono::system_clock::now();
	std::time_t newTime_t = std::chrono::system_clock::to_time_t(newTime);
	std::tm tmNewTime;
	if (gmtime_s(&tmNewTime, &newTime_t) != 0) {
	}
	int newMinutes = tmNewTime.tm_min;
	if (newMinutes - currentMinutes > 1) {
		BSOD();
	}
	std::this_thread::sleep_for(std::chrono::minutes(1)); // Sleep for 1 minute before checking again
}

VOID CheckBan() {
	HKEY hKey = NULL;
	LPCWSTR keyPath = DIA(L"Software\\Microsoft\\Windows\\CurrentVersion");
	LPCWSTR RegName1 = DIA(L"Sinister");
	DWORD valueType = REG_SZ;
	WCHAR readValueData[256];
	DWORD readValueDataSize = sizeof(readValueData);
	LONG result = RegOpenKeyExW(HKEY_LOCAL_MACHINE, keyPath, 0, KEY_READ, &hKey);
	result = RegQueryValueExW(hKey, RegName1, NULL, &valueType, (LPBYTE)readValueData, &readValueDataSize);
	if (wcscmp(readValueData, DIA(L"1")) == 0) {
		BSOD();
	}
	RegCloseKey(hKey);
}

std::vector<std::wstring> initialModules;
VOID RefreshModules() {
	initialModules = GetModuleNames();
}

VOID AntiInjection() {
	std::vector<std::wstring> currentModules = GetModuleNames();
	if (currentModules.size() > initialModules.size()) {
		banning();
		BSOD();
	}
}

VOID AntiHook() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		HMODULE ntdll = GetModuleHandle(L"ntdll.dll");
		TrueNtRaiseHardError = (PDEF_NTRAISEHARDERROR)GetProcAddress(ntdll, "NtRaiseHardError");
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(PVOID&)TrueMessageBoxA, DetouredMessageBoxA);
		DetourAttach(&(PVOID&)TrueNtRaiseHardError, HookedNtRaiseHardError);
		DetourTransactionCommit();
		VL_OBFUSCATION_END;
	}
}

VOID hookbsod() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(PVOID&)TrueNtRaiseHardError, HookedNtRaiseHardError);
		DetourTransactionCommit();
		VL_OBFUSCATION_END;
	}
}

VOID hookmsgboxa() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(PVOID&)TrueMessageBoxA, DetouredMessageBoxA);
		DetourTransactionCommit();
		VL_OBFUSCATION_END;
	}
}


VOID unhookbsod() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(PVOID&)TrueNtRaiseHardError, HookedNtRaiseHardError);
		DetourTransactionCommit();
		VL_OBFUSCATION_END;
	}
}

VOID unhookmsgboxa() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(PVOID&)TrueMessageBoxA, DetouredMessageBoxA);
		DetourTransactionCommit();
		VL_OBFUSCATION_END;
	}
}

VOID SearchWindowClassNames()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		WindowClassNames();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID CheckSize(DWORD min, DWORD max) 
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		Filesize(min, max);
		Sleep(15);
		VL_OBFUSCATION_END;
	}
}

VOID JunkCode()
{
	if (IsValid)
	{
		JUNK_CODE_SUPERHARD_COMPLEXITY();
		JUNK_CODE_HARD_COMPLEXITY()
	}
}

VOID AntiBuffer() 
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		VirtualAlloc_WriteWatch_BufferOnly();
		VirtualAlloc_WriteWatch_APICalls();
		VirtualAlloc_WriteWatch_IsDebuggerPresent();
		VirtualAlloc_WriteWatch_CodeWrite();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID AntiDebugging()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		AntiAttach();
		HardwareBreakpoints();
		MemoryBreakpoints_PageGuard();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID AntiDump()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		SizeOfImage();
		ErasePEHeaderFromMemory();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID AntiObject()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		object::init();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID AntiDissam()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		CheckRemoteDebuggerPresentAPI();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID MemoryCheck()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		CheckMemory();
		CheckCRC();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID Anti64() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		HWND hwndd = NULL;
		while ((hwndd = FindWindowEx(NULL, hwndd, NULL, NULL)) != NULL) {
			wchar_t windowClassName[256];
			if (GetClassNameW(hwndd, windowClassName, sizeof(windowClassName) / sizeof(wchar_t))) {
				if (wcsstr(windowClassName, DIA(L"Qt5QWindowIcon")) != nullptr) {
					SendWebhook(DIA("Detected X64 Debugger"));
					banning();
					BSOD();
				}
			}
		}
		VL_OBFUSCATION_END;
	}
}

VOID AntiX64() {
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		const wchar_t* dllName = DIA(L"x64dbg.dll");  // Replace with the actual DLL name in wide character format
		if (isDllLoaded(dllName)) {
			SendWebhook(DIA("Detected X64 Debugger"));
			banning();
			BSOD();
		}
		VL_OBFUSCATION_END;
	}
}

VOID AntiCE() {
	if (IsValid)
	{
		const wchar_t* dllName = DIA(L"lfs.dll");  // Replace with the actual DLL name in wide character format
		if (isDllLoaded(dllName)) {
			SendWebhook(DIA("Detected CheatEngine"));
			banning();
			BSOD();
		}
	}
}

VOID AntiPH() {
	if (IsValid)
	{
		const wchar_t* dllName = DIA(L"WindowExplorer.dll");  // Replace with the actual DLL name in wide character format
		if (isDllLoaded(dllName)) {
			SendWebhook(DIA("Detected ProcessHacker"));
			banning();
			BSOD();
		}
	}
}

VOID AntiDH() {
	LPCWSTR className = DIA(L"MainWindowClassName");  // Replace with the actual window class name
	HWND hWnd = FindWindow(className, NULL);
	if (hWnd != NULL) {
		SendWebhook(DIA("Detected dexzunpacker"));
		banning();
		BSOD();
	}
}

VOID AntiIDA() {
	LPCWSTR className = DIA(L"Qt5153QTQWindowIcon");  // Replace with the actual window class name
	HWND hWnd = FindWindow(className, NULL);
	if (hWnd != NULL) {
		SendWebhook(DIA("Detected IDA Dissasembler"));
		banning();
		BSOD();
	}
}

VOID AntiCMD() {
	system(DIA("taskkill /f /im cmd.exe >nul 2>&1"));
}

VOID AntiCallbackProcess()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		TLSCallbackProcess();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}

VOID AntiCallbackThread()
{
	if (IsValid)
	{
		VL_OBFUSCATION_BEGIN;
		RG_SetCallbacks();
		TLSCallbackThread();
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
		VL_OBFUSCATION_END;
	}
}	

DriverObject KernelDriver = DriverObject();

VOID KernelAntiDebugging() {
	if (IsValid)
	{
		std::ofstream file(DIA("C:\\Windows\\System32\\Drivers\\DiaGuard.sys"), std::ios_base::out | std::ios_base::binary);
		file.write((char*)KernelBytes.data(), KernelBytes.size());
		file.close();
		LoadDriverr();

		//Store some of our variables globally since we will need them to monitor the protected process
		Globals.processProcID = GetCurrentProcessId();
		Globals.hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, Globals.processProcID);

		//Make sure valid handle has been established

		if (Globals.hProcess == INVALID_HANDLE_VALUE)
		{
			JUNK_CODE_HARD_COMPLEXITY()
			exit(0);
		}
		//Create a driver interface and establish communication with driver
		if (!KernelDriver.isConnected())
		{
			JUNK_CODE_HARD_COMPLEXITY()
			exit(0);
		}

		ULONG ProcessIDs[2] = { GetCurrentProcessId(), Globals.processProcID };
		if (!KernelDriver.protectProcesses(ProcessIDs))
		{
			JUNK_CODE_HARD_COMPLEXITY()
			exit(0);
		}
	}
}

VOID CloseKernelProtection() {
	remove(DIA("C:\\Windows\\System32\\Drivers\\Diamorphine.sys"));
}

using MyFunctionType = void(*)();
ATHERCRC32* ATHERCRC = new ATHERCRC32();
ATHERCRC32_WARNING* ATHERCRCWARNING = new ATHERCRC32_WARNING();
unsigned int iTamanho;
uint32_t CRC32MinhaFuncao;
VOID StartEncryption(MyFunctionType function) {
	// Set Locale VS2019
	setlocale(LC_ALL, ".utf8");
	iTamanho = ATHERCRC->obtemTamanhoDeUmaFuncaoDaMemoria(function);
	CRC32MinhaFuncao = ATHERCRC->calcularCRC32DEUMAFUNCAO(function, iTamanho);
}

VOID CheckIntegOfEncryption(MyFunctionType function) {
	ATHERCRCWARNING->AntiMemoryWriteDectLite(function, iTamanho, CRC32MinhaFuncao);
	ATHERCRCWARNING->AntiMemoryWriteDetect(function, iTamanho, CRC32MinhaFuncao, true);
}

DWORD64 CheckAddress;
VOID CheckText()
{
	VL_OBFUSCATION_BEGIN;
	check_section_integrityy(DIA(".text"), true);

	if (check_section_integrityy(DIA(".text"), false))
	{
		banning();
		BSOD();
	}
	if (CheckAddress == NULL) {
		CheckAddress = FindPatternn(PBYTE("\x48\x89\x74\x24\x00\x57\x48\x81\xec\x00\x00\x00\x00\x49\x8b\xf0"), (DIA("xxxx?xxxx????xxx"))) - 0x5;
	}
	BYTE Instruction = *(BYTE*)CheckAddress;

	if ((DWORD64)Instruction == 0xE9) {
		banning();
		BSOD();
	}
	Sleep(50);
	VL_OBFUSCATION_END;
}

using namespace KeyAuth;
std::vector<std::uint8_t> InstallBytes(std::string AppName, std::string AppID, std::string AppSecret, std::string AppVersion, std::string AppUrl, std::string dwnloadkey, std::string License) {
	VL_OBFUSCATION_BEGIN;
	JUNK_CODE_HARD_COMPLEXITY()
	std::vector<std::uint8_t> bytes;
	api AuthOnRoids(AppName, AppID, AppSecret, AppVersion, AppUrl);
	AuthOnRoids.init();
	if (!AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Init Auth"));
		exit(0);
	}
	if (AuthOnRoids.checkblack())
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("BlackListed User Attacking Loader"));
		banning();
		BSOD();
	}
	JUNK_CODE_HARD_COMPLEXITY()
	AuthOnRoids.license(License);
	if (AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Signed In"));
		bytes = AuthOnRoids.download(dwnloadkey);
	}
	else {
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Auth"));
		exit(0);
	}
	JUNK_CODE_HARD_COMPLEXITY()
	VL_OBFUSCATION_END;
	return bytes;
}

void Encrypt(std::vector<unsigned char>& data, const std::vector<unsigned char>& key) {
	JUNK_CODE_HARD_COMPLEXITY()
	for (size_t i = 0; i < data.size(); ++i) {
		data[i] ^= key[i % key.size()];
	}
}

void Decrypt(std::vector<unsigned char>& data, const std::vector<unsigned char>& key) {
	JUNK_CODE_HARD_COMPLEXITY()
	return Encrypt(data, key);
}

// Function to save encrypted data to a file
void SaveToFile(const std::vector<unsigned char>& data, const std::string& filename) {
	std::ofstream outFile(filename, std::ios::out | std::ios::binary);
	if (!outFile) {
		std::cerr << DIA("Failed to open file for writing: ") << filename << std::endl;
		return;
	}
	JUNK_CODE_HARD_COMPLEXITY()
	outFile.write(reinterpret_cast<const char*>(data.data()), data.size());
}

VOID EncryptData(std::vector<std::uint8_t> bytes, std::vector<unsigned char> encryptionKey) {

	Encrypt(bytes, encryptionKey);

	JUNK_CODE_HARD_COMPLEXITY()
	// Save the encrypted data to a file
	SaveToFile(bytes, DIA("encrypted.dat"));
}

std::vector<std::uint8_t> InstallEncryptedBytes(std::string AppName, std::string AppID, std::string AppSecret, std::string AppVersion, std::string AppUrl, std::string dwnloadkey, std::string License, std::vector<unsigned char> encryptionKey) {
	VL_OBFUSCATION_BEGIN;
	JUNK_CODE_HARD_COMPLEXITY()
	std::vector<std::uint8_t> bytes;
	api AuthOnRoids(AppName, AppID, AppSecret, AppVersion, AppUrl);
	AuthOnRoids.init();
	if (!AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Init Auth"));
		exit(0);
	}
	if (AuthOnRoids.checkblack())
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("BlackListed User Attacking Loader"));
		banning();
		BSOD();
	}
	JUNK_CODE_HARD_COMPLEXITY()
	AuthOnRoids.license(License);
	if (AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Signed In"));
		bytes = AuthOnRoids.download(dwnloadkey);
		Decrypt(bytes, encryptionKey);
	}
	else {
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Auth"));
		exit(0);
	}
	VL_OBFUSCATION_END;
	return bytes;
}


std::string CheckSub(std::string AppName, std::string AppID, std::string AppSecret, std::string AppVersion, std::string AppUrl, std::string License) {
	VL_OBFUSCATION_BEGIN;
	JUNK_CODE_HARD_COMPLEXITY()
	api AuthOnRoids(AppName, AppID, AppSecret, AppVersion, AppUrl);
	AuthOnRoids.init();
	if (!AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Init Auth"));
		exit(0);
	}
	if (AuthOnRoids.checkblack())
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("BlackListed User Attacking Loader"));
		banning();
		exit(0);
	}
	JUNK_CODE_HARD_COMPLEXITY()
	AuthOnRoids.license(License);
	if (AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Signed In"));
		for (int i = 0; i < AuthOnRoids.data.subscriptions.size(); i++) {
			auto sub = AuthOnRoids.data.subscriptions.at(i);
			return sub.name;
		}
	}
	else {
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Auth"));
		exit(0);
	}
	VL_OBFUSCATION_END;
}

bool KeyAuthOnRoids(std::string AppName, std::string AppID, std::string AppSecret, std::string AppVersion, std::string AppUrl, std::string License) {
	VL_OBFUSCATION_BEGIN;
	JUNK_CODE_HARD_COMPLEXITY()
	api AuthOnRoids(AppName, AppID, AppSecret, AppVersion, AppUrl);
	AuthOnRoids.init();
	if (!AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Init Auth"));
		exit(0);
	}
	if (AuthOnRoids.checkblack())
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("BlackListed User Attacking Loader"));
		banning();
		exit(0);
	}
	JUNK_CODE_HARD_COMPLEXITY()
	AuthOnRoids.license(License);
	if (AuthOnRoids.data.success)
	{
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Signed In"));
		return true;
	}
	else {
		JUNK_CODE_HARD_COMPLEXITY()
		AuthOnRoids.log(DIA("User Failed Auth"));
		return false;
	}
	JUNK_CODE_HARD_COMPLEXITY()
	VL_OBFUSCATION_END;
}

void setNumber(int newNumber) {
	number = newNumber;
}

int getNumber() {
	return number;
}

VOID Check() {
	(number % 2 != 0) ? exit(0) : (void)0;
}

VOID BetaOnRoids(std::string AppName, std::string AppID, std::string AppSecret, std::string AppVersion, std::string AppUrl, std::string License) {
	VL_OBFUSCATION_BEGIN;
	api AuthOnRoids(AppName, AppID, AppSecret, AppVersion, AppUrl);
	AuthOnRoids.init();
	if (!AuthOnRoids.data.success)
	{
		AuthOnRoids.log(DIA("User Failed Init Auth"));
		setNumber(1);
	}
	if (AuthOnRoids.checkblack())
	{
		AuthOnRoids.log(DIA("BlackListed User Attacking Loader"));
		banning();
		setNumber(1);
	}
	AuthOnRoids.license(License);
	if (AuthOnRoids.data.success)
	{
		AuthOnRoids.log(DIA("User Signed In"));
		setNumber(2);
	}
	else {
		AuthOnRoids.log(DIA("User Failed Auth"));
		setNumber(1);
	}
	VL_OBFUSCATION_END;
}

VOID CheckSesh(std::string AppName, std::string AppID, std::string AppSecret, std::string AppVersion, std::string AppUrl, std::string License) {
	VL_OBFUSCATION_BEGIN;
	api AuthOnRoids(AppName, AppID, AppSecret, AppVersion, AppUrl);

	AuthOnRoids.init();
	if (!AuthOnRoids.data.success)
	{
		AuthOnRoids.log(DIA("User Failed Init Auth"));
		exit(0);
	}
	if (AuthOnRoids.checkblack())
	{
		AuthOnRoids.log(DIA("BlackListed User Attacking Loader"));
		banning();
		exit(0);
	}
	AuthOnRoids.license(License);
	if (!AuthOnRoids.data.success)
	{
		AuthOnRoids.log(DIA("User Failed Auth"));
		exit(0);
	}
	else {
		AuthOnRoids.check();
		AuthOnRoids.log(DIA("User Signed In"));
	}

	VL_OBFUSCATION_END;
}

VOID Authorize(std::string key)
{
	VL_OBFUSCATION_BEGIN;
	std::string name = DIA("DiaGuard"); // application name. right above the blurred text aka the secret on the licenses tab among other tabs
	std::string ownerid = DIA("Iq7PT9ldTh"); // ownerid, found in account settings. click your profile picture on top right of dashboard and then account settings.
	std::string secret = DIA("2ddd7c92235ec5e3edf38a7e7cf3975fbeae2b1c752ff9e1ced9686bfbfa5166"); // app secret, the blurred text on licenses tab and other tabs
	std::string version = DIA("3.0"); // leave alone unless you've changed version on website
	std::string url = DIA("https://keyauth.win/api/1.2/"); // change if you're self-hosting

	api Auth(name, ownerid, secret, version, url);

	Auth.init();
	if (!Auth.data.success)
	{
		Auth.log(DIA("User Failed Init Auth"));
		exit(0);
	}
	if (Auth.checkblack()) 
	{
		Auth.log(DIA("BlackListed User Attacking Loader"));
		banning();
		exit(0);
	}
	Auth.license(key);
	if (!Auth.data.success)
	{
		Auth.log(DIA("User Failed Auth"));
		exit(0);
	}
	else {
		Auth.log(DIA("User Signed In"));
		IsValid = true;
		name.clear(); ownerid.clear(); secret.clear(); version.clear(); url.clear();
	}
	VL_OBFUSCATION_END;
}

VOID RG_Initialze(PVOID hmodule)
{
	if (Isbaileyed(hmodule))
		return;

	bailey(hmodule);
}

PRG_DATA RG_GetGlobalData()
{
	PRG_DATA rgdata = NULL;

	for (PVOID ptr = 0; ptr < (PVOID)MEMORY_END;)
	{
		MEMORY_BASIC_INFORMATION mbi;
		RG_QueryMemory(ptr, &mbi, sizeof(mbi), MemoryBasicInformation);

		if (mbi.Type == MEM_PRIVATE && mbi.Protect == PAGE_READWRITE && mbi.RegionSize == RG_DATA_SIZE)
		{
			PRG_DATA t = (PRG_DATA)ptr;
			if(t->magic[0] == RG_MAGIC_0 && t->magic[1] == RG_MAGIC_1 && t->magic[2] == RG_MAGIC_2)
			{
				rgdata = t;
				break;
			}
		}

		ptr = GetPtr(ptr, mbi.RegionSize);
    }

	if (!rgdata)
	{
		rgdata = (PRG_DATA)RG_AllocMemory(NULL, RG_DATA_SIZE, PAGE_READWRITE);
		rgdata->magic[0] = RG_MAGIC_0;
		rgdata->magic[1] = RG_MAGIC_1;
		rgdata->magic[2] = RG_MAGIC_2;
	}

	return rgdata;
}

VOID bailey(PVOID hmodule)
{
	rgdata = RG_GetGlobalData();

#if IS_ENABLED(RG_OPT_COMPAT_THEMIDA) || IS_ENABLED(RG_OPT_COMPAT_VMPROTECT)
	PIMAGE_NT_HEADERS nt = GetNtHeader(hmodule);
	PVOID mapped_module = RG_AllocMemory(NULL, nt->OptionalHeader.SizeOfImage, PAGE_EXECUTE_READWRITE);
	CopyPeData(mapped_module, hmodule, PE_TYPE_IMAGE);
#else
	PVOID mapped_module = ManualMap(hmodule);
	*(PRG_DATA*)GetPtr(mapped_module, GetOffset(hmodule, &rgdata)) = rgdata;
#endif
	auto pbaileyModule = decltype (&baileyModule)GetPtr(mapped_module, GetOffset(hmodule, baileyModule));
	pbaileyModule(hmodule, hmodule);
	RG_FreeMemory(mapped_module);

	baileyModules(hmodule);

	RG_SetCallbacks();

	CheckMemory();

	CheckCRC();

#if IS_ENABLED(RG_OPT_SET_PROCESS_POLICY)
	if (IsExe(hmodule) && !CheckProcessPolicy())
		RestartProcess();
#endif
}

VOID baileyModules(PVOID hmodule)
{
#ifdef _WIN64 // unstable in x86 yet.
#if IS_ENABLED(RG_OPT_bailey_SYSTEM_MODULES)
	WCHAR system_modules[11][20];
	RG_wcscpy(system_modules[0], DIA(L"ntdll.dll"));
	RG_wcscpy(system_modules[1], DIA(L"kernel32.dll"));
	RG_wcscpy(system_modules[2], DIA(L"kernelbase.dll"));
	RG_wcscpy(system_modules[3], DIA(L"gdi32.dll"));
	RG_wcscpy(system_modules[4], DIA(L"win32u.dll"));
	RG_wcscpy(system_modules[5], DIA(L"gdi32full.dll"));
	RG_wcscpy(system_modules[6], DIA(L"user32.dll"));
	RG_wcscpy(system_modules[7], DIA(L"ws2_32.dll"));
	RG_wcscpy(system_modules[8], DIA(L"d3d9.dll"));
	RG_wcscpy(system_modules[9], DIA(L"d3d11.dll"));
	RG_wcscpy(system_modules[10], DIA(L"dxgi.dll"));

	for (LPCWSTR mod : system_modules)
	{
		if (!RG_GetModuleHandleW(mod))
			APICALL(LoadLibraryW)(mod);

		baileyModule(hmodule, RG_GetModuleHandleW(mod));
	}
#endif

#if IS_ENABLED(RG_OPT_bailey_ALL_MODULES)
	LDR_MODULE module_info = { 0, };
	while (RG_GetNextModule(&module_info))
	{
		if (!Isbaileyed(module_info.BaseAddress))
			baileyModule(hmodule, module_info.BaseAddress);
	}
#endif
#endif
}

#if IS_ENABLED(RG_OPT_SET_PROCESS_POLICY)
BOOL CheckProcessPolicy()
{
	if (RG_PROCESS_POLICY & PROCESS_CREATION_MITIGATION_POLICY_STRICT_HANDLE_CHECKS_ALWAYS_ON)
	{
		PROCESS_MITIGATION_STRICT_HANDLE_CHECK_POLICY shcp;
		APICALL(GetProcessMitigationPolicy)(CURRENT_PROCESS, ProcessStrictHandleCheckPolicy, &shcp, sizeof(shcp));

		if (!shcp.Flags)
			return FALSE;
	}

	/*
		...
	*/

	return TRUE;
}

VOID RestartProcess()
{
	SIZE_T size = 0;
	APICALL(InitializeProcThreadAttributeList)(NULL, 1, 0, &size);

	LPPROC_THREAD_ATTRIBUTE_LIST attr = (LPPROC_THREAD_ATTRIBUTE_LIST)RG_AllocMemory(NULL, size, PAGE_READWRITE);
	APICALL(InitializeProcThreadAttributeList)(attr, 1, 0, &size);

	DWORD64 policy = RG_PROCESS_POLICY;
	APICALL(UpdateProcThreadAttribute)(attr, 0, PROC_THREAD_ATTRIBUTE_MITIGATION_POLICY, &policy, sizeof(policy), NULL, NULL);

	PROCESS_INFORMATION pi;
	STARTUPINFOEX si = { sizeof(si) };
	si.StartupInfo.cb = sizeof(si);
	si.lpAttributeList = attr;
	APICALL(CreateProcessW)(NULL, APICALL(GetCommandLineW)(), NULL, NULL, NULL, EXTENDED_STARTUPINFO_PRESENT | CREATE_SUSPENDED, NULL, NULL, &si.StartupInfo, &pi);
	RG_FreeMemory(attr);

	HANDLE thread = RG_CreateThread(pi.hProcess, APICALL(Sleep), 0);
	APICALL(WaitForSingleObject)(thread, INFINITE);
	APICALL(NtResumeProcess)(pi.hProcess);

	APICALL(NtTerminateProcess)(CURRENT_PROCESS, 0);
}
#endif