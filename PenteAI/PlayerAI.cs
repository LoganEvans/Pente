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
    public GameState _gameState;
    protected Player _color;
    protected int _lookahead = 1;
    protected AutoResetEvent _waitOnOpponent;

    public PlayerAI() {}

    public PlayerAI(Player color, BoardInterface board) {
    }

    public void SetLookahead(int lookahead) {
      _lookahead = lookahead;
    }

    public override void SetBoard(BoardInterface board) {
      _gameState = new GameState(board);
    }

    public override void SetColor(Player color) {
      _color = color;
    }

    public override void SetOpponent(PlayerBase opponent) {
      opponent.MoveSelected += MoveSelectedEventHandler_GetOpponentMove;
    }

    public override void MoveSelectedEventHandler_GetOpponentMove(object sender, MoveSelectedEventArgs args) {
      _gameState.Move(args.row, args.col);
      _waitOnOpponent.Set();
    }

    // The order of the Tuple is <row, col>
    public Tuple<int, int> GetMove() {
      Tuple<int, int> move = _gameState.GetBestMove(_lookahead);
#if Trace
      Console.WriteLine("move number: " + mGameState.GetPlyNumber() + " color: " + mColor + ", best move: " + move);
#endif
      return move;
    }

    public override void PlayerThread() {
      //Console.WriteLine(" > PlayerThread() " + mColor);
      _waitOnOpponent = new AutoResetEvent(false);
      MoveSelectedEventArgs args;
      Tuple<int, int> move;

      while (_gameState.GetWinner() == Player.Neither) {
        if (_gameState.GetCurrentPlayer() == _color) {
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Thinking...");
#endif
          move = GetMove();
          args = new MoveSelectedEventArgs();
          args.row = move.Item1;
          args.col = move.Item2;
          args.player = _gameState.GetCurrentPlayer();
          _gameState.Move(move.Item1, move.Item2);
          OnMoveSelected(args);
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Done thinking...");
#endif
        } else {
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Waiting on opponent...");
#endif
          _waitOnOpponent.WaitOne();
#if Trace
          Console.WriteLine("(playerThread) " + mColor + " Done waiting on opponent...");
#endif
        }
      }
    }
  }
}
