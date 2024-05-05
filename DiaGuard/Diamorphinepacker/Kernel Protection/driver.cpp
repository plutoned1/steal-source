#include "includes.h"

#pragma comment(lib, "ntdll.lib")


tNtQuerySystemInformation oNtQuerySystemInformation;
tNtQueryObject oNtQueryObject;

tNtLoadDriver oNtLoadDriver;
tNtUnloadDriver oNtUnloadDriver;

//Default Constructor for the driver interface takes the service RegistryPath in which to establish communication 
DriverObject::DriverObject()
{
	hDriver = CreateFileA("\\\\.\\acDriverDevice", GENERIC_ALL, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
}

//Check communication with the driver by sending a request and recieving the correct key back
BOOL DriverObject::isConnected()
{
	KERNEL_REQUEST Request;
	ZeroMemory(&Request, sizeof(KERNEL_REQUEST));
	Request.Buffer = 0;

	if (DeviceIoControl(this->hDriver, IO_STARTUPREQUEST, &Request, sizeof(Request), &Request, sizeof(Request), NULL, NULL))
	{
		if (Request.Buffer == 4)
		{
			return TRUE;
		}
		else return FALSE;
	}
	else return FALSE;
}

//Send a message to the driver telling it to use ObRegisterCallbacks on any handle created for that process
BOOL DriverObject::protectProcesses(ULONG ProcessIDS[2])
{
	KERNEL_REQUEST Request;
	ZeroMemory(&Request, sizeof(KERNEL_REQUEST));
	Request.ProcessIDs[0] = ProcessIDS[0];
	Request.ProcessIDs[1] = ProcessIDS[1];

	if (DeviceIoControl(this->hDriver, IO_PROTECTEDPROCESSINFO, &Request, sizeof(Request), &Request, sizeof(Request), NULL, NULL))
	{
		if (Request.Buffer == 1)
		{
			this->isProcessProtected = TRUE;
			return TRUE;
		}
		else return FALSE;
	}
	else return FALSE;

}

//Checks to see if there are any privelaged open handles with the game process and anticheat process that should not be there
BOOL DriverObject::areProcessesProtected()
{
	return this->isProcessProtected;

	if (!oNtQuerySystemInformation)
		return FALSE;

	PSYSTEM_HANDLE_INFORMATION HandleInfo;
	ULONG infoSize = 0x40000;

	HandleInfo = (PSYSTEM_HANDLE_INFORMATION)malloc(infoSize);

	if (NT_SUCCESS(oNtQuerySystemInformation((SYSTEM_INFORMATION_CLASS1)0x10, HandleInfo, infoSize, NULL)))
	{
		for (int i = 0; i < HandleInfo->HandleCount; i++)
		{
			SYSTEM_HANDLE_TABLE_ENTRY_INFO currHandle = HandleInfo->Handles[i];

			if (currHandle.ProcessId != Globals.processProcID)
				continue;

			if ((currHandle.GrantedAccess & PROCESS_ALL_ACCESS) == 0 || (currHandle.GrantedAccess & PROCESS_VM_OPERATION) == 0)
				continue;

			POBJECT_TYPE_INFORMATION TypeInfo;
			
		}
	}

	free(HandleInfo);
	return TRUE;
}

//Grabs any report structures that have been filled out by the driver. Will exit if the number of reports reaches 3
BOOL DriverObject::QueryReports()
{
	KERNEL_QUERY Reports;
	ZeroMemory(&Reports, sizeof(KERNEL_QUERY));
	static int violationCount = 0;

	if (DeviceIoControl(this->hDriver, IO_QUERYREPORTS, NULL, NULL, &Reports, sizeof(Reports), NULL, NULL))
	{
		for (int i = 0; i < Reports.NumOfReports; i++)
		{
			if (Reports.Reports[i] == NULL)
				continue;

			violationCount++;
			if (violationCount >= 3)
				exit(0);

			switch (Reports.Reports[i]->ReportID)
			{
				case KERNEL_HOOK:
				{
					exit(0);
				}
			}
		}

		return TRUE;
	}

	return FALSE;
}