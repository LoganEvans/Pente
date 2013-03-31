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
      Board board = new Board();

      PlayerHuman pi1 = new PlayerHuman();
      //PlayerBase pi1 = new PlayerAI();

      PlayerHuman pi2 = new PlayerHuman();
      //PlayerBase pi2 = new PlayerAI();

      pi1.setBoard(board);
      pi2.setBoard(board);
      pi1.setColor(player_t.white);
      pi2.setColor(player_t.black);
      pi1.setOpponent(pi2);
      pi2.setOpponent(pi1);

      Thread pi1Thread = new Thread(pi1.playerThread);
      Thread pi2Thread = new Thread(pi2.playerThread);

      Display display = new Display(board, pi1, pi2);

      if (pi1 is PlayerHuman) {
        pi1.setClickReceivedListener(display);
      }

      if (pi2 is PlayerHuman) {
        pi2.setClickReceivedListener(display);
      }

      pi1Thread.Start();
      pi2Thread.Start();

      Application.EnableVisualStyles();
      Application.Run(display);
    }
  }
}
