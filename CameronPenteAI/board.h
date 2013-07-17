#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>

#ifndef BOARD_H
#define BOARD_H
//const int view_distance = 4;
//const int board_size = 19;
#define VIEW_DISTANCE 4
#define BOARD_SIZE = 19

enum node_state_t
{
   empty_state,
   white_state,
   black_state,
   off_board_state,
   dnc_state
};

typedef struct node
{
   uint8_t              x_pos;   /*0 on left column, 18 on right */
   uint8_t              y_pos;   /*0 on top row, 18 on bottom */
   uint8_t              *local_view[VIEW_DISTANCE][VIEW_DISTANCE];
   enum node_state_t    node_state;
} node;

#endif
