#include "CRC32.h"

int initialized = 0;
/* crc_tab[] -- this crcTable is being build by chksum_crc32GenTab().
 *		so make sure, you call it before using the other
 *		functions!
 */
u_int32_t crc_tab[256];

/// <summary>
///		chksum_crc32gentab() --      to a global crc_tab[256], this one will
///		calculate the crcTable for crc32-checksums.
///		it is generated to the polynom [..]
/// </summary>
void chksum_crc32gentab()
{
	unsigned long crc, poly;
	int i, j;

	poly = 0xEDB88320L;
	for (i = 0; i < 256; i++)
	{
		crc = i;
		for (j = 8; j > 0; j--)
		{
			if (crc & 1)
			{
				crc = (crc >> 1) ^ poly;
			}
			else
			{
				crc >>= 1;
			}
		}
		crc_tab[i] = crc;
	}
}

/* chksum_crc() -- to a given block, this one calculates the
 *				crc32-checksum until the length is
 *				reached. the crc32-checksum will be
 *				the result.
 */

/// <summary>
///		chksum_crc() -- to a given block, this one calculates the
///		rc32-checksum until the length is
///		reached. the crc32-checksum will be
///		the result.
/// </summary>
/// <param name="block">Memory Block</param>
/// <param name="length">Size of memory block</param>
/// <returns>CRC32 of memory block</returns>
u_int32_t chksum_crc32(unsigned char* block, unsigned int length) {
	register unsigned long crc;
	unsigned long i;

	crc = 0xFFFFFFFF;
	for (i = 0; i < length; i++)
	{
		crc = ((crc >> 8) & 0x00FFFFFF) ^ crc_tab[(crc ^ *block++) & 0xFF];
	}
	return (crc ^ 0xFFFFFFFF);
}

/// <summary>
///		Creates the instance for the CRC32 table and starts the CRC32 calculation process of the memory block
/// </summary>
/// <param name="block">Memory Block</param>
/// <param name="length">Size of memory block</param>
/// <returns>CRC32 of memory block</returns>
u_int32_t crc32(unsigned char* block, unsigned int length) {
	if (!initialized) {
		chksum_crc32gentab();
		initialized = 1;
	}

	return chksum_crc32(block, length);
}