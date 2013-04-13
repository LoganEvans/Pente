using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class PlayerAI : PlayerBase {
    private GameState mGameState;
    private Player mColor;
    private const int LOOKAHEAD = 3;
    private AutoResetEvent mWaitOnOpponent;

    public PlayerAI() {}

    public PlayerAI(Player color, BoardInterface board) {
    }

    public override void SetBoard(BoardInterface board) {
      mGameState = new GameState(board);
    }

    public override void SetColor(Player color) {
      mColor = color;
    }

    public override void SetOpponent(PlayerBase opponent) {
      opponent.MoveSelected += MoveSelectedEventHandler_GetOpponentMove;
    }

    public override void MoveSelectedEventHandler_GetOpponentMove(object sender, MoveSelectedEventArgs args) {
      mGameState.Move(args.row, args.col);
      mWaitOnOpponent.Set();
    }

    // The order of the Tuple is <row, col>
    public Tuple<int, int> GetMove() {
      Tuple<int, int> move = mGameState.GetBestMove(LOOKAHEAD);
      Console.WriteLine("move number: " + mGameState.GetMoveNumber() + " color: " + mColor + ", best move: " + move);
      return move;
    }

    public void SetMove(Tuple<int, int> move) {
      //Console.WriteLine(" > setOpponentMove(" + move + ") for AI " + mColor);
      mGameState.Move(move.Item1, move.Item2);
    }

    public override void PlayerThread() {
      //Console.WriteLine(" > PlayerThread() " + mColor);
      mWaitOnOpponent = new AutoResetEvent(false);
      MoveSelectedEventArgs args;
      Tuple<int, int> move;

      while (mGameState.GetWinner() == Player.Neither) {
        if (mGameState.GetCurrentPlayer() == mColor) {
          Console.WriteLine("(playerThread) " + mColor + " Thinking...");
          move = GetMove();
          args = new MoveSelectedEventArgs();
          args.row = move.Item1;
          args.col = move.Item2;
          args.player = mGameState.GetCurrentPlayer();
          OnMoveSelected(args);
          mGameState.Move(move.Item1, move.Item2);
          Console.WriteLine("(playerThread) " + mColor + " Done thinking...");
        } else {
          Console.WriteLine("(playerThread) " + mColor + " Waiting on opponent...");
          mWaitOnOpponent.WaitOne();
          Console.WriteLine("(playerThread) " + mColor + " Done waiting on opponent...");
        }
      }
    }
  }
}
