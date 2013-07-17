#include <stdio.h>
#include <stdlib.h>
#include "board.h"


int
main(int argc, char **argv)
{
   printf("char size is %i\n", (int) sizeof(char));
   printf("int size is %i\n", (int) sizeof(int));
   printf("uint8_t size is %i\n", (int) sizeof(uint8_t));
   return 0;
}
