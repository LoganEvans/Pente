#undef Trace
//#define Trace

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class PlayerAI : PlayerBase {
    public GameState mGameState;
    protected Player mColor;
    protected int mLookahead = 1;
    protected AutoResetEvent mWaitOnOpponent;

    public PlayerAI() {}

    public PlayerAI(Player color, BoardInterface board) {
    }

    public void SetLookahead(int lookahead) {
      mLookahead = lookahead;
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
      Tuple<int, int> move = mGameState.GetBestMove(mLookahead);
#if Trace
      Console.WriteLine("move number: " + mGameState.GetMoveNumber() + " color: " + mColor + ", best move: " + move);
#endif
      return move;
    }

    public override void PlayerThread() {
      //Console.WriteLine(" > PlayerThread() " + mColor);
      mWaitOnOpponent = new AutoResetEvent(false);
      MoveSelectedEventArgs args;
      Tuple<int, int> move;

      while (mGameState.GetWinner() == Player.Neither) {
        if (mGameState.GetCurrentPlayer() == mColor) {
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Thinking...");
#endif
          move = GetMove();
          args = new MoveSelectedEventArgs();
          args.row = move.Item1;
          args.col = move.Item2;
          args.player = mGameState.GetCurrentPlayer();
          mGameState.Move(move.Item1, move.Item2);
          OnMoveSelected(args);
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Done thinking...");
#endif
        } else {
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Waiting on opponent...");
#endif
          mWaitOnOpponent.WaitOne();
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Done waiting on opponent...");
#endif
        }
      }
    }
  }
}
