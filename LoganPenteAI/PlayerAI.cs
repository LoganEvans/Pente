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

    public PlayerAI(player_t color, Board board) {
      mGameState = new GameState(board, 2);
      mColor = color;
    }

    // The order of the Tuple is <row, col>
    public Tuple<int, int> getMove() {
      Tuple<int, int> move = mGameState.getBestMove();
      Console.WriteLine("move number: " + mGameState.getMoveNumber() + " color: " + mColor + ", best move: " + move);
      return move;
    }

    public void setMove(Tuple<int, int> move) {
      //Console.WriteLine(" > setOpponentMove(" + move + ") for AI " + mColor);
      mGameState.move(move.Item1, move.Item2);
    }
  }
}
