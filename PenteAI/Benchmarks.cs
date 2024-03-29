﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class GameStateBenchmark : GameState {
    protected int _depthLimit, _branchingFactor;
    public GameStateBenchmark(BoardInterface board, int depthLimit, int branchingFactor)
      : base(board) {
      _depthLimit = depthLimit;
      _branchingFactor = branchingFactor;
    }

    public GameStateBenchmark(GameStateBenchmark copyFrom)
      : base(copyFrom) {
      CopyMaps(copyFrom);
      _pliesEvaluated = 0;
      _depthLimit = copyFrom._depthLimit;
      _branchingFactor = copyFrom._branchingFactor;
    }

    public override Tuple<int, int> GetBestMove(int depthLimit) {
      while (true) {
        Tuple<int, int> move = base.GetBestMove(depthLimit);
        if (IsLegal(move)) {
          return move;
        }
      }
    }

    protected override List<Tuple<int, int>> GetCandidateMoves() {
      List<Tuple<int, int>> retval = new List<Tuple<int, int>>();
      // We need mBranchingFactor legal candidate moves...
      for (int i = 0; i < _branchingFactor; i++) {
        // We might need to try a few times before we stumble on something legal...
        while (true) {
          Tuple<int, int> move = Benchmarks.GetRandomMove();
          if (IsLegal(move)) {
            retval.Add(move);
            break;
          }
        }
      }
      return retval;
    }
  }

  public class PlayerBenchmark : PlayerAI {
    protected int _depthLimit, _branchingFactor;

    public PlayerBenchmark(int depthLimit, int branchingFactor) {
      _lookahead = depthLimit;
      _depthLimit = depthLimit;
      _branchingFactor = branchingFactor;
    }

    public override void SetBoard(BoardInterface board) {
      _gameState = new GameStateBenchmark(board, _depthLimit, _branchingFactor);
    }

    public int GetPlies() {
      return _gameState.GetPlyNumber();
    }
  }

  class Benchmarks {
    public static Tuple<int, int> GetRandomMove() {
      return Tuple.Create(GameState.staticRand.Next(Board.ROWS), GameState.staticRand.Next(Board.COLS));
    }

    public void RandomGameToCSV(int n) {
      String filename = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"RandomGames.csv");

      StringBuilder sb = new StringBuilder();
      sb.Append("Winner,Plies,WhiteCaptures,BlackCaptures");
      sb.Append(Environment.NewLine);

      for (int i = 0; i < n; i++) {
        Board board = new Board();
        board.Move(9, 9);

        while (board.GetWinner() == Player.Neither) {
          board.Move(GetRandomMove());
        }

        sb.Append(board.GetWinner().ToString() + "," +
                  board.GetPlyNumber().ToString() + "," +
                  board.GetCaptures(Player.White) + "," +
                  board.GetCaptures(Player.Black));
        sb.Append(Environment.NewLine);
      }

      File.WriteAllText(filename, sb.ToString());
    }

    public void TimeRandomGames(int n) {
      // Create new stopwatch
      Stopwatch stopwatch = new Stopwatch();
      int totalPlies = 0;

      // Begin timing
      stopwatch.Start();

      // Do something
      for (int i = 0; i < n; i++) {
        Board board = new Board();
        board.Move(9, 9);

        while (board.GetWinner() == Player.Neither) {
          board.Move(GetRandomMove());
        }

        totalPlies += board.GetPlyNumber();
      }

      // Stop timing
      stopwatch.Stop();

      // Write result
      Console.WriteLine("Time elapsed: {0}",
          stopwatch.Elapsed);
      Console.WriteLine("Total moves: {0}", totalPlies);
      Console.WriteLine("Average moves per second: {0}",
          1000.0 * (float)totalPlies / stopwatch.ElapsedMilliseconds);
    }

    public void CountFailsInN(int n) {
      int COUNT = 10000;
      int fails = 0;
      for (int trial = 0; trial < n; trial++) {
        int count = 0;
        for (int i = 0; i < COUNT; i++) {
          Board board = new Board();
          board.Move(9, 9);

          while (board.GetWinner() == Player.Neither) {
            board.Move(GetRandomMove());
          }

          count += board.GetPlyNumber();
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

    public void TimeRandomGamesWithLookahead(int n, int depthLimit, int branchingFactor) {
      Stopwatch stopwatch = new Stopwatch();
      int totalPlies = 0;

      // Begin timing
      stopwatch.Start();
      for (int i = 0; i < n; i++) {
        BoardInterface board = new Board();
        PlayerBenchmark pi1 = new PlayerBenchmark(depthLimit, branchingFactor);
        pi1.SetBoard(board);
        pi1.SetColor(Player.White);

        PlayerBenchmark pi2 = new PlayerBenchmark(depthLimit, branchingFactor);
        pi2.SetBoard(board);
        pi2.SetColor(Player.Black);

        pi1.SetOpponent(pi2);
        pi2.SetOpponent(pi1);

        Thread pi1Thread = new Thread(pi1.PlayerThread);
        pi1Thread.Start();
        Thread pi2Thread = new Thread(pi2.PlayerThread);
        pi2Thread.Start();

        pi1Thread.Join();
        pi2Thread.Join();

        Debug.Assert(pi1._gameState == pi2._gameState);

        totalPlies += pi1.GetPlies();
      }

      // Stop timing
      stopwatch.Stop();
      Console.WriteLine("Average: {0}", (float)totalPlies / n);

      // Write result
      Console.WriteLine("Time elapsed: {0}",
          stopwatch.Elapsed);
      Console.WriteLine("Total moves: {0}", totalPlies);
      Console.WriteLine("Average moves per second: {0}",
          1000.0 * (float)totalPlies / stopwatch.ElapsedMilliseconds);
    }

    public void TimeRandomGamesWithLookaheadNoPlayer(int n, int depthLimit, int branchingFactor) {
      Stopwatch stopwatch = new Stopwatch();
      int totalPlies = 0;
      int totalPliesEvaluated = 0;

      // Begin timing
      stopwatch.Start();

      for (int i = 0; i < n; i++) {
        BoardInterface board = new Board();
        GameStateBenchmark gs_bench = new GameStateBenchmark(board, depthLimit, branchingFactor);

        while (gs_bench.GetWinner() == Player.Neither) {
          gs_bench.Move(gs_bench.GetBestMove(depthLimit));
        }
        totalPlies += gs_bench.GetPlyNumber();
        totalPliesEvaluated += gs_bench.GetPliesEvaluated();
      }

      // Stop timing
      stopwatch.Stop();

      // Write result
      Console.WriteLine("Time elapsed: {0}",
          stopwatch.Elapsed);
      Console.WriteLine("Total plies: {0}", totalPlies);
      Console.WriteLine("Average plies per game: {0}", (float)totalPlies / n);
      Console.WriteLine("Average plies per second: {0}",
          1000.0 * (float)totalPlies / stopwatch.ElapsedMilliseconds);
      Console.WriteLine("Total plies evaluated: {0}", totalPliesEvaluated);
      Console.WriteLine("Average plies evaluated per game: {0}", (float)totalPliesEvaluated / n);
      Console.WriteLine("Average plies evaluated per second: {0}",
          1000.0 * (float)totalPliesEvaluated / stopwatch.ElapsedMilliseconds);
    }
  }
}
