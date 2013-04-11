using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  class GameState : Board {
    private int[] mInfluenceMap;
    private static int mPossitionsEvaluated;
    private static int mBaseDepth;
    private double mRunningHeuristic;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    public GameState(BoardInterface board) : base(board) {
      InitializeMaps();
      mRunningHeuristic = 0.0;
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      CopyMaps(copyFrom);
      mRunningHeuristic = copyFrom.mRunningHeuristic;
    }

    private void CopyMaps(GameState copyFrom) {
      mInfluenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mInfluenceMap[row_dex] = copyFrom.mInfluenceMap[row_dex];
      }
    }

    // This method triggers the Negamax search. It should only be called externally.
    public Tuple<int, int> GetBestMove(int depthLimit) {
      Console.WriteLine(" > GetBestMove()");
      mBaseDepth = GetMoveNumber();
      mPossitionsEvaluated = 0;
      Tuple<int, int> move;
      double heuristic = Minimax(depthLimit, null, null, out move);
      //Tuple<Tuple<int, int>, Tuple<double, int>> move = Negamax();
      Console.WriteLine(" < GetBestMove()... Possitions evaluated: " + mPossitionsEvaluated + ", heuristic: " + heuristic);
      return move;
    }

    double Minimax(int depth, double? heuristicAlpha, double? heuristicBeta, out Tuple<int, int> bestMove) {
      double? champHeur = null;
      double chumpHeur;
      GameState child = null;
      Tuple<int, int> move;
      bestMove = null;

      if (depth == 0 || GetWinner() != Player.Neither) {
        return mRunningHeuristic;
      }

      if (IsMaxLevel()) {
        foreach (Tuple<int, int> candidateMove in GetCandidateMoves()) {
          child = new GameState(this);
          child.Move(candidateMove.Item1, candidateMove.Item2);
          chumpHeur = child.Minimax(depth - 1, champHeur, heuristicBeta, out move);

          // Max level, so pick max.
          if (champHeur == null || champHeur < chumpHeur) {
            champHeur = chumpHeur;
            if (heuristicBeta != null && champHeur > heuristicBeta) {
              bestMove = candidateMove;
              // Beta prune.
              break;
            }
          }
        }
      } else {
        foreach (Tuple<int, int> candidateMove in GetCandidateMoves()) {
          child = new GameState(this);
          child.Move(candidateMove.Item1, candidateMove.Item2);
          chumpHeur = child.Minimax(depth - 1, heuristicAlpha, champHeur, out move);

          // Min level, so pick min.
          if (champHeur == null || champHeur > chumpHeur) {
            champHeur = chumpHeur;
            if (heuristicAlpha != null && champHeur < heuristicAlpha) {
              bestMove = candidateMove;
              // Beta prune.
              break;
            }
          }
        }
      }

      return (double)champHeur;
    }

    private List<Tuple<int, int>> GetCandidateMoves() {
      Tuple<int, int> spot;
      List<Tuple<int, int>> candidates = new List<Tuple<int, int>>();
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (!IsLegal(row_dex, col_dex)) {
            continue;
          }

          if (IsOnMap(row_dex, col_dex, mInfluenceMap)) {
            spot = Tuple.Create(row_dex, col_dex);
            candidates.Add(spot);
          }
        }
      }

      return candidates;
    }

    private bool IsOnMap(int row, int col, int[] map) {
      if ((map[row] & SPOT_MASKS[col]) != 0) {
        return true;
      } else {
        return false;
      }
    }

    // This function should (almost) never need to be called. Use UpdateMaps instead if possible.
    private void InitializeMaps() {
      mInfluenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          UpdateMaps(row_dex, col_dex);
        }
      }
    }

    private void UpdateMaps(int row, int col) {
      if (IsOnMap(row, col, mInfluenceMap)) {
        return;
      }

      Tuple<double, int> heurVal;
      heurVal = GetHeuristicValue(row, col);
      if (heurVal.Item2 < HeuristicValues.GetProximityPriority()) {
        mInfluenceMap[row] |= SPOT_MASKS[col];
      }
    }

    // After finding the expected winner and uncertainy (the second tuple) associated with a move (the first tuple), determine which one is better.
    private Tuple<Tuple<int, int>, Tuple<double, int>> ChooseBest(Tuple<Tuple<int, int>, Tuple<double, int>> a,
                                                                  Tuple<Tuple<int, int>, Tuple<double, int>> b) {
      if (a == null && b == null) {
        return null;
      } else if (a == null) {
        return b;
      } else if (b == null) {
        return a;
      } else if (CmpHeuristics(a.Item2, b.Item2) >= 0) {
        return a;
      } else {
        return b;
      }
    }

    private int CmpHeuristics(Tuple<double, int> first, Tuple<double, int> second) {
      if (first == null && second == null) {
        return 0;
      } else if (first == null) {
        return 1;
      } else if (second == null) {
        return -1;
      } else if (first.Item2 < second.Item2) {
        return 1;
      } else if (first.Item2 > first.Item2) {
        return -1;
      } else if (first.Item1 > second.Item1) {
        return 1;
      } else if (first.Item1 < second.Item1) {
        return -1;
      }
      return 0;
    }

    public override bool Move(int row, int col) {
      mRunningHeuristic += GetHeuristicValue(row, col).Item1;
      bool retval = base.Move(row, col);
      MoveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void MoveTriggeredMapsUpdate(int row, int col) {
      List<Tuple<int, int>> directions = new List<Tuple<int, int>>();
      directions.Add(new Tuple<int, int>(0, 1));  // horizontal
      directions.Add(new Tuple<int, int>(1, 0));  // vertical
      directions.Add(new Tuple<int, int>(1, 1));  // forward slash
      directions.Add(new Tuple<int, int>(1, -1));  // back slash

      foreach (Tuple<int, int> direction in directions) {
        int inspectedCol;
        int inspectedRow;

        for (int i = -4; i <= 4; i++) {
          inspectedCol = col + (direction.Item1 * i);
          inspectedRow = row + (direction.Item2 * i);

          if (0 <= inspectedRow && inspectedRow < ROWS && 0 <= inspectedCol && inspectedCol < COLS) {
            UpdateMaps(inspectedRow, inspectedCol);
          }
        }
      }
    }

    // TODO, this doesn't check for captures.
    public Tuple<double, int> GetHeuristicValue(int row, int col) {
      if (!IsLegal(row, col)) {
        return Tuple.Create(0.0, HeuristicValues.GetProximityPriority() + 1);
      }

      Dictionary<Pattern, Tuple<double, int>> hDict = HeuristicValues.GetHeuristicDict();
      Tuple<double, int> val = null;
      Pattern pattern;

      foreach (Tuple<int, int> window in GetWindows(row, col)) {
        pattern = new Pattern();

        if (IsMaxLevel()) {
          pattern.SetPattern(window.Item1, window.Item2);
          if (hDict.TryGetValue(pattern, out val)) {
            return val;
          }
        } else {
          pattern.SetPattern(window.Item2, window.Item1);
          if (hDict.TryGetValue(pattern, out val)) {
            return Tuple.Create(0 - val.Item1, val.Item2);
          }
        }
      }

      return Tuple.Create(0.0, HeuristicValues.GetProximityPriority() + 1);
    }

    private bool IsMaxLevel() {
      if (GetCurrentPlayer() == Player.White) {
        return true;
      } else {
        return false;
      }
    }

    public Pattern WindowToPattern(Player color, Tuple<int, int> window) {
      Pattern retval = new Pattern();
      if (color == Player.White) {
        retval.SetPattern(window.Item1, window.Item2);
      } else {
        retval.SetPattern(window.Item2, window.Item1);
      }
      return retval;
    }
  }
}
