using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class PlayerAI : PlayerInterface {
    private GameState mGameState;
    private player_t mColor;

    public PlayerAI(player_t color) {
      mGameState = new GameState(new Board(), 1);
      mColor = color;
    }

    // The order of the Tuple is <row, col>
    public Tuple<int, int> getMove() {
      return mGameState.getBestMove();
    }

    public void setOpponentMove(Tuple<int, int> move) {
      mGameState.move(move);
    }
  }
}
