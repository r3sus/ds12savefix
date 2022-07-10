#include "csum.c"
#include "mc02.c"

#define TEST_1
/*
enum aid
{
	PC = "RGMH";
};
*/

//int fst = 0, BE = 0, ds2 = 0;
/*
DWORD fst;
_Bool BE;
_Bool ds2;
*/

/*
int check1(struct PLiD p1)
{
}
*/

void exit1(char * str)
{

	printf(str);
	getchar();
	exit(0);
}

/* 
Validate file
Detect platform => BE, fst
Return ver
*/

struct PLiD {
	char* pl;
	char* sig;
	DWORD fst;
	int BE;
	int ds2;
};

struct PLiD check(char* path)
{

	struct PLiD PC = { "PC", "RGMH", 0x2834, 0, 0 }, XB = { "XB", "CON ", 0xD000, 1, 0 }, that;
	

	FILE* save = fopen(path, "rb");
	
	// Detect platform, BE, fst
	
	DWORD tmp;
	fread(&tmp, 4, 1, save);
	printf("%x", tmp);
	fclose(save);


	exit1("");
	
	if (memcmp(save, XB.sig, 4)) that = XB;
	else if (memcmp(save, PC.sig, 4)) that = PC;
	else exit1("wrong file?");

	char* DS2 = "ds_2";//, * tmp = DS2;

	fseek(save, that.fst, SEEK_SET);
	//fread(&tmp, 4, 1, save);

	//printf(tmp);

	that.ds2 = memcmp(save + 0x24, DS2, 4);
	
	fclose(save);
	return that;
}

int main(int argc, char** argv)
{

	char* path;

	printf("Dead Space 1/2 (PC/XB) save file fixer\n\n");

	if (argc != 2)
	{
#ifdef TEST_1
		path = "../saves/ds_slot_05";
		path = "../saves/ds_slot_02.deadspace2saved";
		path = "../saves/ds_slot_01.deadspacesaved";
#else		
		printf("Usage: drop the file on exe \n");
		getchar();
		return -1;
#endif // TEST_1
	}
	else path = argv[1];
	printf("%s\n", path);
	struct PLiD that = check(path);

#ifdef TEST_1
	printf("%s\n", that.pl);

	if (that.ds2) printf("DS2\n");
	else printf("not DS2\n");

	exit1("dbg");
#endif

	if (that.ds2) csum_main(path, that.fst, that.BE);
	
	mc02_main(path, that.fst, that.BE);

	printf("\nThanks to VakhtinAndrey for Dead-Space-2-PC-Save-Editor.\n");

	printf("\nDone.\n");

	getchar();
}