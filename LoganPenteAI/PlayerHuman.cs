using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class PlayerHuman : PlayerBase {
    private Board mBoard;
    private player_t mColor;
    private AutoResetEvent mWaitOnClick;
    private AutoResetEvent mWaitOnOpponent;

    public override void setBoard(BoardInterface board) {
      mBoard = new Board(board);
    }

    public override void setColor(player_t color) {
      mColor = color;
    }

    public override void setOpponent(PlayerBase opponent) {
      opponent.MoveSelected += MoveSelectedEventHandler_getOpponentMove;
    }

    public override void MoveSelectedEventHandler_getOpponentMove(object sender, MoveSelectedEventArgs args) {
      mBoard.move(args.row, args.col);
      //Console.WriteLine(mColor + " Setting mWaitOnOpponent");
      mWaitOnOpponent.Set();
    }

    public void setMoveSelectedByClickListener(Display display) {
      display.MoveSelectedByClick += MoveSelectedByClickEventHandler;
    }

    public void MoveSelectedByClickEventHandler(object sender, MoveSelectedEventArgs args) {
      if (args.player == mColor) {
        mBoard.move(args.row, args.col);
        OnMoveSelected(args);
        //Console.WriteLine(mColor + " Setting mWaitOnClick");
        mWaitOnClick.Set();
      }
    }

    public override void playerThread() {
      //Console.WriteLine(" > playerThread() " + mColor);
      mWaitOnClick = new AutoResetEvent(false);
      mWaitOnOpponent = new AutoResetEvent(false);

      while (mBoard.getWinner() == player_t.neither) {
        if (mBoard.getCurrentPlayer() == mColor) {
          //Console.WriteLine("(playerThread) " + mColor + " Waiting on click...");
          mWaitOnClick.WaitOne();
          //Console.WriteLine("(playerThread) " + mColor + " Done waiting on click...");
        } else {
          //Console.WriteLine("(playerThread) " + mColor + " Waiting on opponent...");
          mWaitOnOpponent.WaitOne();
          //Console.WriteLine("(playerThread) " + mColor + " Done waiting on opponent...");
        }
      }
    }
  }
}
