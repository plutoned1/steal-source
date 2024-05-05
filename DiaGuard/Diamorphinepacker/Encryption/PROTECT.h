#include <iostream>
#include <Windows.h>
#include "CRC32.h"

/// <summary>
///		Class responsible for CRC32 integrity and encryption
/// </summary>
class ATHERCRC32 {


public:
	uint32_t calcularCRC32DEUMAFUNCAO(PVOID pFuncao, unsigned int TamanhoDaFuncaoNaMemoria);

	bool validarCRC32(unsigned char* pFuncao, int TamanhoDaFuncao, uint32_t CRC32_ORIGINAL);

	unsigned int obtemTamanhoDeUmaFuncaoDaMemoria(PVOID pFuncao);

	/// <summary>
	///		This function is used to obtain a pointer to any type of function
	///		Class, public and private functions
	///		This function is not restricted to VOID types, it accepts all types of data as return
	///     This function tracks the pointer in memory for a given class function
	/// </summary>
	/// <param name="f">myclass::myfunctionMethod</param>
	/// <returns>PVOID to be assigned to a dynamic data AUTO, original pointer retrieved for the function in memory</returns>
	template<typename T, typename R>
	void* pvoid_cast(R(T::* f)())
	{
		union
		{
			R(T::* pf)();
			void* p;
		};
		pf = f;
		return p;
	}

};


/// <summary>
///		Class responsible for evaluating the CRC32 of memory blocks and punishing attempts to write in the process memory
/// </summary>
class ATHERCRC32_WARNING {

private:
	ATHERCRC32* crc32Ather = new ATHERCRC32();

public:
	void AntiMemoryWriteDetect(PVOID pFuncao, unsigned int TamanhoDaFuncaoNaMemoria, uint32_t CRC32MinhaFuncao, bool showdlg);
	void AntiMemoryWriteDectLite(PVOID pFuncao, unsigned int TamanhoDaFuncaoNaMemoria, uint32_t CRC32MinhaFuncao);

};