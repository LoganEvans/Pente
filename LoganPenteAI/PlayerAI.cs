using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class PlayerAI : PlayerBase {
    private GameState mGameState;
    private player_t mColor;
    private const int LOOKAHEAD = 2;

    public PlayerAI() {
    }

    public PlayerAI(player_t color, Board board) {
      setBoard(board);
      setColor(color);
    }

    public override void setBoard(BoardInterface board) {
      mGameState = new GameState(board, LOOKAHEAD);
    }

    public override void setColor(player_t color) {
      mColor = color;
    }

    public override void setOpponent(PlayerBase opponent) {
      MoveTriggered += opponent.MoveTriggeredEventHandler_getOpponentMove;
    }

    public override void MoveTriggeredEventHandler_getOpponentMove(object sender, MoveTriggeredEventArgs args) {
      mGameState.move(args.row, args.col);
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

    public override void playerThread() {
    }
  }
}
