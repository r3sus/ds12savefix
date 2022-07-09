#include "csum.c"
#include "mc02.c"

#define TEST_1

void check(char* path)
{
	FILE* save = fopen(path, "rb+");
	char* PC_ID = "RGMH", xb_ID = "CON ";

}

int main(int argc, char** argv)
{

	char* path;

	printf("Dead Space 2 (x360) save file checksum fixer\n\n");

	if (argc != 2)
	{
#ifdef TEST_1
		path = "D:/d/2/saves/ds_slot_05"; 
#else		
		printf("Usage: drop the file on exe \n");
		getchar();
		return -1;
#endif // TEST_1
	}
	else path = argv[1];

	check(path);

	csum(path);
	
	mc02(path);

	printf("\nThanks to VakhtinAndrey for Dead-Space-2-PC-Save-Editor.\n");

	printf("\nDone.\n");

	getchar();
}