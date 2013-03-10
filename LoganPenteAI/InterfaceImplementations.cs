using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PenteCommonInterfaces;

namespace LoganPenteAI {
  struct Board : BoardInterface {
    public bool move(int row, int col) {
      return false;
    }

    public player_t getSpot(int row, int col) { return player_t.neither; }

    public int getCaptures(player_t player) { return -1; }
    public int getMoveNumber() { return -1; }
    public player_t getCurrentPlayer() { return player_t.neither; }

    // if the return value is player_t.neither, then the game is not finished.
    public player_t getWinner() { return player_t.neither; }
    public bool isLegal(int row, int col) { return false; }
  }
}
