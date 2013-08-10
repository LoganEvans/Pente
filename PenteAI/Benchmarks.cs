using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  class Benchmarks {
    public void RandomGameToCSV(int n) {
      String filename = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"RandomGames.csv");

      StringBuilder sb = new StringBuilder();
      sb.Append("Winner,Moves,WhiteCaptures,BlackCaptures");
      sb.Append(Environment.NewLine);
      var rand = new Random();

      for (int i = 0; i < n; i++) {
        Board board = new Board();
        board.Move(9, 9);

        while (board.GetWinner() == Player.Neither) {
          board.Move(rand.Next(Board.ROWS), rand.Next(Board.COLS));
        }

        sb.Append(board.GetWinner().ToString() + "," +
                  board.GetMoveNumber().ToString() + "," +
                  board.GetCaptures(Player.White) + "," +
                  board.GetCaptures(Player.Black));
        sb.Append(Environment.NewLine);
      }

      File.WriteAllText(filename, sb.ToString());
    }

    public void TimeRandomGames(int n) {
      // Create new stopwatch
      Stopwatch stopwatch = new Stopwatch();
      Random rand = new Random();
      int totalMoves = 0;

      // Begin timing
      stopwatch.Start();

      // Do something
      for (int i = 0; i < n; i++) {
        Board board = new Board();
        board.Move(9, 9);

        while (board.GetWinner() == Player.Neither) {
          board.Move(rand.Next(Board.ROWS), rand.Next(Board.COLS));
        }

        totalMoves += board.GetMoveNumber();
      }

      // Stop timing
      stopwatch.Stop();

      // Write result
      Console.WriteLine("Time elapsed: {0}",
          stopwatch.Elapsed);
      Console.WriteLine("Total moves: {0}", totalMoves);
      Console.WriteLine("Average moves per second: {0}",
          1000.0 * (float)totalMoves / stopwatch.ElapsedMilliseconds);
    }

    public void CountFailsInN(int n) {
      // Create new stopwatch
      int COUNT = 10000;
      Random rand = new Random();
      int fails = 0;
      for (int trial = 0; trial < n; trial++) {
        int count = 0;
        for (int i = 0; i < COUNT; i++) {
          Board board = new Board();
          board.Move(9, 9);

          while (board.GetWinner() == Player.Neither) {
            board.Move(rand.Next(Board.ROWS), rand.Next(Board.COLS));
          }

          count += board.GetMoveNumber();
        }
        double avg = count / (float)COUNT;
        Console.Write("avg: {0}", avg);
        if (!(153 < avg && avg < 155)) {
          fails += 1;
          Console.Write("  !!!");
        }
        Console.WriteLine();
      }
      Console.WriteLine("{0}", fails);
    }
  }
}
