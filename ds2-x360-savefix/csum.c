
//			BASED ON:

/*
*	
*	Dead Space 2 & 3 (PS3) Checksum Fixer - (c) 2021 by Bucanero - www.bucanero.com.ar
*
* This tool is based (reversed) on the original DS2_Checksum_Fixer, DS3_Checksum_Fixer by Red-EyeX32
*
* Information about the SDBM hash method:
*	- http://www.cse.yorku.ca/~oz/hash.html#sdbm
*/

#include "iofile.c"

/*
#define DS2_CSUM_OFFSET    0x04
#define DS2_SIZE_OFFSET    0x6C
#define DS3_CSUM_OFFSET    0x08
#define DS3_SIZE_OFFSET    0x4C
#define DS3_HED_SIZE       0x80
*/

u32 sdbm_hash(const u8* data, u32 len, u32 init)
{
    u32 crc = init;
    
    while (len--)
        crc = (crc * 0x1003f) + *data++;

    return (crc);
}
/*
void print_usage(const char* argv0)
{
	printf("USAGE: %s [option] filename\n\n", argv0);
	printf("OPTIONS        Explanation:\n");
	printf(" -d            Decrypt File\n");
	printf(" -e            Encrypt File\n\n");
	return;
}
*/

u32 BE32(u32 dw)
{
	//if (!BE) return dw;
	u32 toReturn = 0;

	toReturn |= (dw >> 24);
	toReturn |= (dw >> 8) & 0xFF00;
	toReturn |= (dw << 8) & 0xFF0000;
	toReturn |= (dw << 24);

	return toReturn;
}


u32 LE32(u32 dw) 
{
	return dw;
}

//#define E32(_val,_BE) if (_BE) ES32(_val) else _val;

/*


void* E32(_val, int BE)
{
	if (BE) return ES32(;
}


void ES33(_val)
{
	((u32)(((((u32)_val) & 0xff000000) >> 24) | \
		((((u32)_val) & 0x00ff0000) >> 8) | \
		((((u32)_val) & 0x0000ff00) << 8) | \
		((((u32)_val) & 0x000000ff) << 24)))
}
*/

//#define E32(_val)

int csum_main(char* name, int fst, int BE)
{
	size_t len;
	u8 *save;
	u32 csum_off, size_off;
	u32 csum, usr_size;
	char *opt, *bak;
	void* ES32;
	
	printf("deadspace-checksum-fixer by Bucanero (based on)\n");

	if (read_buffer(name, &save, &len) != 0)
	{
		printf("[*] Could Not Access The File (%s)\n", name);
		return -1;
	}

	// Save a file backup
	//asprintf(&bak, "%s.bak", "HED-DATA");
	//write_buffer(bak, data_hed, len);

	ES32 = BE ? BE32 : LE32;

	csum_off = 0x20; //(len == DS3_HED_SIZE) ? DS3_CSUM_OFFSET : DS2_CSUM_OFFSET;
	size_off = 0x88;//(len == DS3_HED_SIZE) ? DS3_SIZE_OFFSET : DS2_SIZE_OFFSET;
	save += fst;
	usr_size = ES32(*(u32*)(save + size_off));

	printf("[*] Calculation Size: %d bytes\n", usr_size);
	printf("[*] Stored Checksum : %08X\n", ES32(*(u32*)(save + csum_off)));

	memset(save + csum_off, 0, sizeof(u32));
	csum = sdbm_hash(save+0x1C, 0x170, 0);
	csum = sdbm_hash(save+0x18C, usr_size, csum);
	printf("[*] New Checksum    : %08X\n", csum);

	csum = ES32(csum);
	memcpy(save + csum_off, &csum, sizeof(u32));
	
	save -= fst;

	if (write_buffer(name, save, len) == 0)
		printf("[*] Successfully Wrote New Checksum!\n\n");

	//free(bak);
	free(save);

	return 0;
}
