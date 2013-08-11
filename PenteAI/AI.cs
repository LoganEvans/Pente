using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PenteInterfaces;

namespace PenteAI {
  class AI {
    static void StartGame() {
      BoardInterface board = new Board();
      /*
      BoardInterface board = new GameState(Player.White, 2, 4,
//123456789012345678
"..................." +  // 0
"..................." +  // 1
"..................." +  // 2
"..................." +  // 3
"..................." +  // 4
"..................." +  // 5
"..................." +  // 6
"..................." +  // 7
"..................." +  // 8
".........WBBBB....." +  // 9 (center)
".........W........." +  // 10
".........W........." +  // 11
"..................." +  // 12
".........W........." +  // 13
"..................." +  // 14
"..................." +  // 15
"..................." +  // 16
"..................." +  // 17
"...................");  // 18
      */

      //PlayerHuman pi1 = new PlayerHuman();
      PlayerBase pi1 = new PlayerAI();
      pi1.SetBoard(board);
      pi1.SetColor(Player.White);

      PlayerHuman pi2 = new PlayerHuman();
      //PlayerBase pi2 = new PlayerAI();
      pi2.SetBoard(board);
      pi2.SetColor(Player.Black);

      Display display = new Display(board, pi1, pi2);

      // Comment out if a player is not a human.
      //pi1.SetMoveSelectedByClickListener(display);
      pi2.SetMoveSelectedByClickListener(display);

      pi1.SetOpponent(pi2);
      pi2.SetOpponent(pi1);

      Thread pi1Thread = new Thread(pi1.PlayerThread);
      pi1Thread.Start();
      Thread pi2Thread = new Thread(pi2.PlayerThread);
      pi2Thread.Start();

      Application.EnableVisualStyles();
      Application.Run(display);
    }

    static void Benchmark() {
      Benchmarks bench = new Benchmarks();
      // bench.RandomGameToCSV(1000000);
      // bench.TimeRandomGames(1000000);
      // bench.TimeRandomGames(100000);

      // bench.TimeRandomGamesWithLookahead(n:10000, depthLimit:0, branchingFactor:1);
      bench.TimeRandomGamesWithLookaheadNoPlayer(n: 1000, depthLimit: 3, branchingFactor: 3);
      // bench.CountFailsInN(1000);
    }

    [STAThread]
    static void Main(string[] args) {
      Board.InitBoard();
      GameState.InitGameState();

      // AI.StartGame();
      AI.Benchmark();
    }
  }
}
