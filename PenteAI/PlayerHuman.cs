using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class PlayerHuman : PlayerBase {
    private Board _board;
    private Player _color;
    private AutoResetEvent _waitOnClick;
    private AutoResetEvent _waitOnOpponent;

    public PlayerHuman() { }

    public override void SetBoard(BoardInterface board) {
      _board = new Board(board);
    }

    public override void SetColor(Player color) {
      _color = color;
    }

    public override void SetOpponent(PlayerBase opponent) {
      opponent.MoveSelected += MoveSelectedEventHandler_GetOpponentMove;
    }

    public override void MoveSelectedEventHandler_GetOpponentMove(object sender, MoveSelectedEventArgs args) {
      _board.Move(args.row, args.col);
      Console.WriteLine(_color + " Setting mWaitOnOpponent");
      _waitOnOpponent.Set();
    }

    public void SetMoveSelectedByClickListener(Display display) {
      display.MoveSelectedByClick += MoveSelectedByClickEventHandler;
    }

    public void MoveSelectedByClickEventHandler(object sender, MoveSelectedEventArgs args) {
      if (args.player == _color) {
        _board.Move(args.row, args.col);
        OnMoveSelected(args);
        Console.WriteLine(_color + " Setting mWaitOnClick");
        _waitOnClick.Set();
      }
    }

    public override void PlayerThread() {
      //Console.WriteLine(" > PlayerThread() " + mColor);
      _waitOnClick = new AutoResetEvent(false);
      _waitOnOpponent = new AutoResetEvent(false);

      while (_board.GetWinner() == Player.Neither) {
        if (_board.GetCurrentPlayer() == _color) {
          Console.WriteLine("(playerThread) " + _color + " Waiting on click...");
          _waitOnClick.WaitOne();
          Console.WriteLine("(playerThread) " + _color + " Done waiting on click...");
        } else {
          Console.WriteLine("(playerThread) " + _color + " Waiting on opponent...");
          _waitOnOpponent.WaitOne();
          Console.WriteLine("(playerThread) " + _color + " Done waiting on opponent...");
        }
      }
    }
  }
}
