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
      Board board;
      PlayerAI ai1 = new PlayerAI();
      PlayerAI ai2 = new PlayerAI();
      Display display = new Display(board, ai1, ai2);

      Application.EnableVisualStyles();
      Application.Run(display);
    }
  }
}
