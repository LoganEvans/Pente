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
    private AutoResetEvent waitOnClick;
    private AutoResetEvent waitOnOpponent;

    public PlayerHuman() {
    }

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
      opponent.MoveTriggered += MoveTriggeredEventHandler_getOpponentMove;
    }

    public override void MoveTriggeredEventHandler_getOpponentMove(object sender, MoveTriggeredEventArgs args) {
      mBoard.move(args.row, args.col);
      //Console.WriteLine(mColor + " Setting waitOnOpponent");
      waitOnOpponent.Set();
    }

    public void setClickReceivedListener(Display display) {
      display.ClickReceived += ClickReceivedEventHandler;
    }

    public void ClickReceivedEventHandler(object sender, MoveTriggeredEventArgs args) {
      if (args.player == mColor) {
        mBoard.move(args.row, args.col);
        OnMoveTriggered(args);
        //Console.WriteLine(mColor + " Setting waitOnClick");
        waitOnClick.Set();
      }
    }

    public override void playerThread() {
      //Console.WriteLine(" > playerThread() " + mColor);
      waitOnClick = new AutoResetEvent(false);
      waitOnOpponent = new AutoResetEvent(false);

      while (mBoard.getWinner() == player_t.neither) {
        if (mBoard.getCurrentPlayer() == mColor) {
          //Console.WriteLine("(playerThread) " + mColor + " Waiting on click...");
          waitOnClick.WaitOne();
          //Console.WriteLine("(playerThread) " + mColor + " Done waiting on click...");
        } else {
          //Console.WriteLine("(playerThread) " + mColor + " Waiting on opponent...");
          waitOnOpponent.WaitOne();
          //Console.WriteLine("(playerThread) " + mColor + " Done waiting on opponent...");
        }
      }
    }
  }
}
