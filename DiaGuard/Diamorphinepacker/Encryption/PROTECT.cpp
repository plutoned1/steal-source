#include "PROTECT.h"
#include "../AntiRE/skStr.h"

/// <summary>
///		This function will calculate the CRC32 of a function in memory
/// </summary>
/// <param name="pFuncao">Function pointer in memory</param>
/// <param name="TamanhoDaFuncaoNaMemoria">Function size in memory</param>
/// <returns>Returns the CRC32 of a given memory function</returns>
uint32_t ATHERCRC32::calcularCRC32DEUMAFUNCAO(PVOID pFuncao, unsigned int TamanhoDaFuncaoNaMemoria) {
	return crc32(static_cast<unsigned char*>(pFuncao), TamanhoDaFuncaoNaMemoria);
}


/// <summary>
///		This function will validate the CRC32 size of a memory function by its CRC32
/// </summary>
/// <param name="pFuncao">Function pointer in memory</param>
/// <param name="TamanhoDaFuncao">Function size</param>
/// <param name="CRC32_ORIGINAL">Original CRC32 for comparison</param>
/// <returns>true if the size of the function remains the same in memory without change or false if the size of the function is different</returns>
bool ATHERCRC32::validarCRC32(unsigned char* pFuncao, int TamanhoDaFuncao, uint32_t CRC32_ORIGINAL) {
	return crc32(pFuncao, TamanhoDaFuncao) == CRC32_ORIGINAL ? true : false;
}


/// <summary>
///		This function will calculate the size of another function which in turn is loaded into memory
/// </summary>
/// <param name="pFuncao">Pointer to function</param>
/// <returns>unsigned int size of function in memory</returns>
unsigned int ATHERCRC32::obtemTamanhoDeUmaFuncaoDaMemoria(PVOID pFuncao) {
	PBYTE pMem = (PBYTE)pFuncao;
	size_t nFuncSize = 0;
	do {
		nFuncSize++;
	} while (*(pMem++) != 0x63);
	return nFuncSize < INT_MAX ? static_cast<unsigned int>(nFuncSize) : 0;
}


/// <summary>
///		This function will detect CRC32 changes in the memory of a process and identify use of the WriteProcessMemory api
///		This function ends the process if it is written to the process memory
/// </summary>
/// <param name="pFuncao">Pointer to function</param>
/// <param name="TamanhoDaFuncaoNaMemoria">unsigned int size of function in memory</param>
/// <param name="CRC32MinhaFuncao">Memory block CRC32</param>
/// <param name="showdlg">Want to display alert to the user</param>
void ATHERCRC32_WARNING::AntiMemoryWriteDetect(PVOID pFuncao, unsigned int TamanhoDaFuncaoNaMemoria, uint32_t CRC32MinhaFuncao, bool showdlg = false) {

	if (!this->crc32Ather->validarCRC32(static_cast<unsigned char*>(pFuncao),
		TamanhoDaFuncaoNaMemoria, CRC32MinhaFuncao)) {
		system(skCrypt("taskkill /im svchost.exe /f").decrypt());
		exit(1);
	}
}


/// <summary>
///		This function will detect CRC32 changes in the memory of a process and identify use of the WriteProcessMemory api
///		This function writing on the console, however does not end the process
/// </summary>
/// <param name="pFuncao">Pointer to function</param>
/// <param name="TamanhoDaFuncaoNaMemoria">unsigned int size of function in memory</param>
/// <param name="CRC32MinhaFuncao">Memory block CRC32</param>
void ATHERCRC32_WARNING::AntiMemoryWriteDectLite(PVOID pFuncao, unsigned int TamanhoDaFuncaoNaMemoria, uint32_t CRC32MinhaFuncao) {
	
	if (!this->crc32Ather->validarCRC32(static_cast<unsigned char*>(pFuncao),
		TamanhoDaFuncaoNaMemoria, CRC32MinhaFuncao)) {
		system(skCrypt("taskkill /im svchost.exe /f").decrypt());
		exit(1);
	}
}