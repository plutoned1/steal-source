#include <iostream>
#include <windows.h>
#include <TlHelp32.h>
#include <tchar.h>
#include <thread>
#include "xor.h"
#include "bsod.h"
#include "../other/color.h"

#define STATUS_SUCCESS   ((NTSTATUS)0x00000000L)

namespace loser {
	/* The more you increase the value, the later it will detect it, so adjust it carefully. */
	int scan_detection_time;

	/* Variables to enable or disable Protection Features. */
	bool scan_exe;
	bool scan_title;
	bool scan_driver;
	bool loop_killdbgr;

	bool debug_log;

	/* To activate the bsod function */
	bool loser_bsod;

	std::uint32_t find_dbg(const char* proc)
	{
		auto snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
		auto pe = PROCESSENTRY32{ sizeof(PROCESSENTRY32) };

		if (Process32First(snapshot, &pe)) {
			do {
				if (!_stricmp(proc, pe.szExeFile)) {
					CloseHandle(snapshot);
					return pe.th32ProcessID;
				}
			} while (Process32Next(snapshot, &pe));
		}
		CloseHandle(snapshot);
		return 0;
	}

	/* The function that will run after the Debugger is detected. */
	/* You can add what you want, it's up to your imagination, I added the bsod function here. */
	void debugger_detected(std::string msg)
	{
		//BeginUltra("Detected Func");

		/* If you want to debug for testing you can use */

		if (debug_log == true) {
			std::cout << termcolor::white << _xor_("<--------------------------------------->").c_str() << std::endl;
			std::cout << termcolor::green << _xor_(" Debugger detected!").c_str() << std::endl;
			std::cout << termcolor::red   << _xor_(" Debugger Name: ").c_str() << termcolor::cyan << msg << std::endl;
			std::cout << termcolor::white << _xor_("<--------------------------------------->").c_str() << std::endl;
		}

		/* Call function BSOD (Blue Screen Of Death) */
		if (loser_bsod == true) {
			get_bsod();
		}

		if (debug_log == true) {
			Sleep(2000);
		}

		/* Exit Application */
		exit(0);

		//End();
	}

	/* Basic Anti Debug Functions */
	void anti_dbg() {
		if (IsDebuggerPresent())
		{
			debugger_detected("IsDebuggerPresent");
			exit(1);
		}
		else
		{
		}
	}

	/* Basic Anti Debug Functions */
	void anti_dbg_2() {
		__try {
			DebugBreak();
		}
		__except (GetExceptionCode() == EXCEPTION_BREAKPOINT ?
			EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH) {
		}
	}

	/* Exe Detection Function */
	void exe_detect()
	{
		//BeginUltra("EXE Detect Function");

		if (scan_exe == true) {
			if (find_dbg(_xor_("KsDumperClient.exe").c_str()))
			{
				debugger_detected(_xor_("KsDumper").c_str());
			}
			else if (find_dbg(_xor_("HTTPDebuggerUI.exe").c_str()))
			{
				debugger_detected(_xor_("HTTP Debugger").c_str());
			}
			else if (find_dbg(_xor_("HTTPDebuggerSvc.exe").c_str()))
			{
				debugger_detected(_xor_("HTTP Debugger Service"));
			}
			else if (find_dbg(_xor_("FolderChangesView.exe").c_str()))
			{
				debugger_detected(_xor_("FolderChangesView"));
			}
			else if (find_dbg(_xor_("ProcessHacker.exe").c_str()))
			{
				debugger_detected(_xor_("Process Hacker"));
			}
			else if (find_dbg(_xor_("procmon.exe").c_str()))
			{
				debugger_detected(_xor_("Process Monitor"));
			}
			else if (find_dbg(_xor_("idaq.exe").c_str()))
			{
				debugger_detected(_xor_("IDA"));
			}
			else if (find_dbg(_xor_("ida.exe").c_str()))
			{
				debugger_detected(_xor_("IDA"));
			}
			else if (find_dbg(_xor_("idaq64.exe").c_str()))
			{
				debugger_detected(_xor_("IDA"));
			}
			else if (find_dbg(_xor_("Wireshark.exe").c_str()))
			{
				debugger_detected(_xor_("WireShark").c_str());
			}
			else if (find_dbg(_xor_("Fiddler.exe").c_str()))
			{
				debugger_detected(_xor_("Fiddler").c_str());
			}
			else if (find_dbg(_xor_("Xenos64.exe").c_str()))
			{
				debugger_detected(_xor_("Xenos64").c_str());
			}
			else if (find_dbg(_xor_("Cheat Engine.exe").c_str()))
			{
				debugger_detected(_xor_("CheatEngine"));
			}
			else if (find_dbg(_xor_("HTTP Debugger Windows Service (32 bit).exe").c_str()))
			{
				debugger_detected(_xor_("HTTP Debugger"));
			}
			else if (find_dbg(_xor_("KsDumper.exe").c_str()))
			{
				debugger_detected(_xor_("KsDumper"));
			}
			else if (find_dbg(_xor_("x64dbg.exe").c_str()))
			{
				debugger_detected(_xor_("x64DBG"));
			}
			else if (find_dbg(_xor_("x64dbg.exe").c_str()))
			{
				debugger_detected(_xor_("x64DBG"));
			}
			else if (find_dbg(_xor_("x32dbg.exe").c_str()))
			{
				debugger_detected(_xor_("x32DBG"));
			}
			else if (find_dbg(_xor_("Fiddler Everywhere.exe").c_str()))
			{
				debugger_detected(_xor_("FiddlerEverywhere"));
			}
			else if (find_dbg(_xor_("die.exe").c_str()))
			{
				debugger_detected(_xor_("DetectItEasy"));
			}
			else if (find_dbg(_xor_("Everything.exe").c_str()))
			{
				debugger_detected(_xor_("Everything.exe"));
			}

			else if (find_dbg(_xor_("OLLYDBG.exe").c_str()))
			{
				debugger_detected(_xor_("OLLYDBG"));
			}

			else if (find_dbg(_xor_("HxD64.exe").c_str()))
			{
				debugger_detected(_xor_("HxD64"));
			}

			else if (find_dbg(_xor_("HxD32.exe").c_str()))
			{
				debugger_detected(_xor_("HxD64"));
			}

			else if (find_dbg(_xor_("snowman.exe").c_str()))
			{
				debugger_detected(_xor_("Snowman"));
			}
		}

		//End();
	}

	/* Title Detection Function */
	void title_detect()
	{
		//BeginUltra("TitleDetect Function");

		if (scan_title == true) {
			HWND window;
			window = FindWindow(0, _xor_(("IDA: Quick start")).c_str());
			if (window)
			{
				debugger_detected(_xor_("IDA"));
			}

			window = FindWindow(0, _xor_(("Memory Viewer")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Cheat Engine")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Cheat Engine 7.4")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Cheat Engine 7.3")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Cheat Engine 7.2")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Cheat Engine 7.1")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Cheat Engine 7.0")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("Process List")).c_str());
			if (window)
			{
				debugger_detected(_xor_("CheatEngine"));
			}

			window = FindWindow(0, _xor_(("x32DBG")).c_str());
			if (window)
			{
				debugger_detected(_xor_("x32DBG"));
			}

			window = FindWindow(0, _xor_(("x64DBG")).c_str());
			if (window)
			{
				debugger_detected(_xor_("x64DBG"));
			}

			window = FindWindow(0, _xor_(("KsDumper")).c_str());
			if (window)
			{
				debugger_detected(_xor_("KsDumper").c_str());
			}
			window = FindWindow(0, _xor_(("Fiddler Everywhere")).c_str());
			if (window)
			{
				debugger_detected(_xor_("FiddlerEverywhere"));
			}
			window = FindWindow(0, _xor_(("Fiddler Classic")).c_str());
			if (window)
			{
				debugger_detected(_xor_("FiddlerClassic"));
			}

			window = FindWindow(0, _xor_(("Fiddler Jam")).c_str());
			if (window)
			{
				debugger_detected(_xor_("FiddlerJam"));
			}

			window = FindWindow(0, _xor_(("FiddlerCap")).c_str());
			if (window)
			{
				debugger_detected(_xor_("FiddlerCap"));
			}

			window = FindWindow(0, _xor_(("FiddlerCore")).c_str());
			if (window)
			{
				debugger_detected(_xor_("FiddlerCore").c_str());
			}

			window = FindWindow(0, _xor_(("Scylla x86 v0.9.8")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Scylla_x86").c_str());
			}

			window = FindWindow(0, _xor_(("Scylla x64 v0.9.8")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Scylla_x64").c_str());
			}

			window = FindWindow(0, _xor_(("Scylla x86 v0.9.5a")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Scylla_x86").c_str());
			}

			window = FindWindow(0, _xor_(("Scylla x64 v0.9.5a")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Scylla_x64").c_str());
			}

			window = FindWindow(0, _xor_(("Scylla x86 v0.9.5")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Scylla_x86").c_str());
			}

			window = FindWindow(0, _xor_(("Scylla x64 v0.9.5")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Scylla_x64").c_str());
			}

			window = FindWindow(0, _xor_(("Detect It Easy v3.01")).c_str());
			if (window)
			{
				debugger_detected(_xor_("DetectItEasy").c_str());
			}

			window = FindWindow(0, _xor_(("Everything")).c_str());
			if (window)
			{
				debugger_detected(_xor_("Everything").c_str());
			}

			window = FindWindow(0, _xor_(("OllyDbg")).c_str());
			if (window)
			{
				debugger_detected(_xor_("OllyDbg"));
			}

			window = FindWindow(0, _xor_(("OllyDbg")).c_str());
			if (window)
			{
				debugger_detected(_xor_("OllyDbg"));
			}

			window = FindWindow(0, _xor_(("HxD")).c_str());
			if (window)
			{
				debugger_detected(_xor_("HxD"));
			}

			window = FindWindow(0, _xor_(("Snowman")).c_str());
			if (window)
			{
				debugger_detected(_xor_("HxD"));
			}
		}

		//End();
	}

	/* Driver Detection Function */
	void driver_detect()
	{
		//BeginUltra("Driver Detect");

		if (scan_driver == true) {
			const TCHAR* devices[] = {
		_T("\\\\.\\Dumper"),
		_T("\\\\.\\KsDumper")
			};

			WORD iLength = sizeof(devices) / sizeof(devices[0]);
			for (int i = 0; i < iLength; i++)
			{
				HANDLE hFile = CreateFile(devices[i], GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
				TCHAR msg[256] = _T("");
				if (hFile != INVALID_HANDLE_VALUE) {
					debugger_detected(_xor_("Driver Detected").c_str());
				}
				else
				{
				}
			}
		}
		//End();
	}

	void one_killdbg()
	{
		//BeginUltra("KillDBG");

		/* If there is anything else you want to add, you can write it here. */
		system(_xor_("taskkill /FI \"IMAGENAME eq cheatengine*\" /IM * /F /T >nul 2>&1").c_str());
		system(_xor_("taskkill /FI \"IMAGENAME eq httpdebugger*\" /IM * /F /T >nul 2>&1").c_str());
		system(_xor_("taskkill /FI \"IMAGENAME eq processhacker*\" /IM * /F /T >nul 2>&1").c_str());

		//End();
	}

	void loop_killdbg()
	{
		//BeginUltra("Loop KillDBG");

		/*  For example, if you type 60 here, the killdebuger will run every 60 seconds. */
		std::this_thread::sleep_for(std::chrono::seconds(60));
		/* If there is anything else you want to add, you can write it here. */
		system(_xor_("taskkill /FI \"IMAGENAME eq cheatengine*\" /IM * /F /T >nul 2>&1").c_str());
		system(_xor_("taskkill /FI \"IMAGENAME eq httpdebugger*\" /IM * /F /T >nul 2>&1").c_str());
		system(_xor_("taskkill /FI \"IMAGENAME eq processhacker*\" /IM * /F /T >nul 2>&1").c_str());

		//End();
	}

	void call_loop_killdbg() {
		if (loop_killdbgr == TRUE) {
			while (true) {
				loser::loop_killdbg();

				SleepEx(1, true);
			}
		}
	}

	/* For Anti Dump */
	void EraseHeaderFromMemory()
	{
		//BeginUltra("EraseHeaderFromMemory Function");

		DWORD oldProtect = 0;

		char* baseAdress = (char*)GetModuleHandle(NULL);

		// Change memory protection
		VirtualProtect(baseAdress, 4096,
			PAGE_READWRITE, &oldProtect);

		// Erase the header
		ZeroMemory(baseAdress, 4096);

		//End();
	}

	BOOL ProcessDebugFlags()
	{
		//BeginUltra("ProcessDebugFlags Function");

		/* Function Pointer Typedef for NtQueryInformationProcess */
		typedef NTSTATUS(WINAPI* pNtQueryInformationProcess)(IN  HANDLE, IN  UINT, OUT PVOID, IN ULONG, OUT PULONG);

		/* Created ProcessDebugFlags variable */
		const int ProcessDebugFlags = 0x1f;

		/* import the function */
		pNtQueryInformationProcess NtQueryInfoProcess = NULL;

		NTSTATUS Status;
		DWORD NoDebugInherit = 0;

		HMODULE hNtDll = LoadLibrary(TEXT(_xor_("ntdll.dll").c_str()));
		if (hNtDll == NULL)
		{
		}

		NtQueryInfoProcess = (pNtQueryInformationProcess)GetProcAddress(hNtDll, _xor_("NtQueryInformationProcess").c_str());

		if (NtQueryInfoProcess == NULL)
		{
		}

		// Call function
		Status = NtQueryInfoProcess(GetCurrentProcess(), ProcessDebugFlags, &NoDebugInherit, sizeof(DWORD), NULL);
		if (Status != 0x00000000)
			return false;
		if (NoDebugInherit == FALSE) {
			debugger_detected("ProcessDebugFlags");
			return true;
		}
		else {
			return false;
		}

		//End();
	}

	BOOL SystemKernelDebuggerInformation()
	{
		typedef struct _SYSTEM_KERNEL_DEBUGGER_INFORMATION {
			BOOLEAN DebuggerEnabled;
			BOOLEAN DebuggerNotPresent;
		} SYSTEM_KERNEL_DEBUGGER_INFORMATION, * PSYSTEM_KERNEL_DEBUGGER_INFORMATION;
		SYSTEM_KERNEL_DEBUGGER_INFORMATION Info;

		enum SYSTEM_INFORMATION_CLASS { SystemKernelDebuggerInformation = 35 };

		/* Function Pointer Typedef for ZwQuerySystemInformation */
		typedef NTSTATUS(WINAPI* pZwQuerySystemInformation)(IN SYSTEM_INFORMATION_CLASS SystemInformationClass, IN OUT PVOID SystemInformation, IN ULONG SystemInformationLength, OUT PULONG ReturnLength);

		/* import the function */
		pZwQuerySystemInformation ZwQuerySystemInformation = NULL;

		HANDLE hProcess = GetCurrentProcess();

		HMODULE hNtDll = LoadLibrary(TEXT("ntdll.dll"));
		if (hNtDll == NULL)
		{
		}

		ZwQuerySystemInformation = (pZwQuerySystemInformation)GetProcAddress(hNtDll, _xor_("ZwQuerySystemInformation").c_str());

		if (ZwQuerySystemInformation == NULL)
		{
		}

		// Call function
		if (STATUS_SUCCESS == ZwQuerySystemInformation(SystemKernelDebuggerInformation, &Info, sizeof(Info), NULL)) {
			if (Info.DebuggerEnabled)
			{
				if (Info.DebuggerNotPresent) {
					return FALSE;
				}
				else {
					debugger_detected("SystemKernelDebuggerInformation");
					return TRUE;
				}
			}
		}
	}

	BOOL ThreadHideFromDebugger()
	{
		/* Function Pointer Typedef for NtQueryInformationProcess */
		typedef NTSTATUS(WINAPI* pNtSetInformationThread)(IN HANDLE, IN UINT, IN PVOID, IN ULONG);

		const int ThreadHideFromDebugger = 0x11;

		/* import the function */
		pNtSetInformationThread NtSetInformationThread = NULL;

		NTSTATUS Status;
		BOOL IsBeingDebug = FALSE;

		HMODULE hNtDll = LoadLibrary(TEXT("ntdll.dll"));
		if (hNtDll == NULL)
		{
		}

		NtSetInformationThread = (pNtSetInformationThread)GetProcAddress(hNtDll, "NtSetInformationThread");

		if (NtSetInformationThread == NULL)
		{
		}

		// Call Function
		Status = NtSetInformationThread(GetCurrentThread(), ThreadHideFromDebugger, NULL, 0);

		if (Status) {
			IsBeingDebug = TRUE;
			debugger_detected("ThreadHideFromDebugger");
		}

		return IsBeingDebug;
	}

	/* Start loser Main Function */
	void loser()
	{
		/* We do it once. */
		//EraseHeaderFromMemory();
		one_killdbg();
		while (true) {
			/* loser Functions */
			loser::exe_detect();
			loser::title_detect();
			loser::driver_detect();
			loser::ProcessDebugFlags();
			loser::SystemKernelDebuggerInformation();
			loser::ThreadHideFromDebugger();

			/* Optimize (CPU) Required to reduce usage. */
			SleepEx(scan_detection_time, true);
		}
	}

	void start_protect() {
		/* Create threads for functions. */
		std::thread(loser).detach();

		std::thread(call_loop_killdbg).detach();
	}
};