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
    private AutoResetEvent autoEvent;
    private event EventHandler<MoveTriggeredEventArgs> clickListener;

    public PlayerHuman(player_t color, Board board) {
      mBoard = board;
      mColor = color;
    }

    public override void setBoard(BoardInterface board) {
      mBoard = new Board(board);
    }

    public override void setColor(player_t color) {
      mColor = color;
    }

    public override void setOpponent(PlayerBase opponent) {
      MoveTriggered += opponent.MoveTriggeredEventHandler_getOpponentMove;
    }

    public override void MoveTriggeredEventHandler_getOpponentMove(object sender, MoveTriggeredEventArgs args) {
      mBoard.move(args.row, args.col);
      //autoEvent.Set();
    }

    public void setClickReceivedListener(Display display) {
      display.ClickReceived += ClickReceivedEventHandler;
    }

    public void ClickReceivedEventHandler(object sender, MoveTriggeredEventArgs args) {
      if (args.player == mColor) {
        mBoard.move(args.row, args.col);
        OnMoveTriggered(args);
      }
    }

    public override void playerThread() {
      return;
    //while (mBoard.getWinner() == player_t.neither) {
    //  if (mBoard.getCurrentPlayer() == mColor) {
    //    Console.WriteLine("my turn... " + mColor);
    //  } else {
    //    autoEvent.WaitOne();
    //  }
    //}
    }
  }
}
