using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using CommonInterfaces;

namespace LoganPenteAI {
  class LPE_AI {
    [STAThread]
    static void Main(string[] args) {
      // TODO: It would be nice if the program could be threaded in a way that would allow the
      // Display class to handle display issues and the main thread to handle game control issues.
      // Read up on delegates and thread-safe multithreading.
      BoardInterface board = new Board();

      PlayerHuman pi1 = new PlayerHuman();
      //PlayerBase pi1 = new PlayerAI();
      pi1.setBoard(board);
      pi1.setColor(player_t.white);

      PlayerHuman pi2 = new PlayerHuman();
      //PlayerBase pi2 = new PlayerAI();

      Display display = new Display(board, pi1, pi2);

      // Comment out if a player is not a human.
      pi1.setMoveSelectedByClickListener(display);
      pi2.setMoveSelectedByClickListener(display);

      pi2.setBoard(board);
      pi2.setColor(player_t.black);

      pi1.setOpponent(pi2);
      pi2.setOpponent(pi1);

      Thread pi1Thread = new Thread(pi1.playerThread);
      pi1Thread.Start();
      Thread pi2Thread = new Thread(pi2.playerThread);
      pi2Thread.Start();

      Application.EnableVisualStyles();
      Application.Run(display);
    }
  }
}
