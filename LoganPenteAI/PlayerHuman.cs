using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class PlayerHuman : PlayerBase {
    private Board mBoard;
    private Player mColor;
    private AutoResetEvent mWaitOnClick;
    private AutoResetEvent mWaitOnOpponent;

    public PlayerHuman() { }

    public override void SetBoard(BoardInterface board) {
      mBoard = new Board(board);
    }

    public override void SetColor(Player color) {
      mColor = color;
    }

    public override void SetOpponent(PlayerBase opponent) {
      opponent.MoveSelected += MoveSelectedEventHandler_GetOpponentMove;
    }

    public override void MoveSelectedEventHandler_GetOpponentMove(object sender, MoveSelectedEventArgs args) {
      mBoard.Move(args.row, args.col);
      Console.WriteLine(mColor + " Setting mWaitOnOpponent");
      mWaitOnOpponent.Set();
    }

    public void SetMoveSelectedByClickListener(Display display) {
      display.MoveSelectedByClick += MoveSelectedByClickEventHandler;
    }

    public void MoveSelectedByClickEventHandler(object sender, MoveSelectedEventArgs args) {
      if (args.player == mColor) {
        mBoard.Move(args.row, args.col);
        OnMoveSelected(args);
        Console.WriteLine(mColor + " Setting mWaitOnClick");
        mWaitOnClick.Set();
      }
    }

    public override void PlayerThread() {
      //Console.WriteLine(" > PlayerThread() " + mColor);
      mWaitOnClick = new AutoResetEvent(false);
      mWaitOnOpponent = new AutoResetEvent(false);

      while (mBoard.GetWinner() == Player.Neither) {
        if (mBoard.GetCurrentPlayer() == mColor) {
          Console.WriteLine("(playerThread) " + mColor + " Waiting on click...");
          mWaitOnClick.WaitOne();
          Console.WriteLine("(playerThread) " + mColor + " Done waiting on click...");
        } else {
          Console.WriteLine("(playerThread) " + mColor + " Waiting on opponent...");
          mWaitOnOpponent.WaitOne();
          Console.WriteLine("(playerThread) " + mColor + " Done waiting on opponent...");
        }
      }
    }
  }
}
