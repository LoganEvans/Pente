using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  class GameState : Board {
    private Tuple<double, int>[][] mHeuristicMapCurrent;
    private Tuple<double, int>[][] mHeuristicMapOther;
    private double mRunningHeuristicCurrent;
    private double mRunningHeuristicOther;
    private int[] mInfluenceMap;
    private static int mPossitionsEvaluated;
    private static int mBaseDepth;

    private readonly int PENALTY_PROXIMITY = 1;
    private readonly int PENALTY_NO_PROXIMITY = 3;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    private Tuple<int, int> mBestMove;
    private int mPermittedExploreSteps;

    // If depthPermitted == 0, then the heuristic value of the best move will be returned... no further
    // recursion will occure.
    public GameState(Board board, int permittedExploreSteps) : base(board) {
      mRunningHeuristicCurrent = 0.0;
      mRunningHeuristicOther = 0.0;
      mPermittedExploreSteps = permittedExploreSteps;
      InitializeMaps();
    }

    public GameState(BoardInterface board, int permittedExploreSteps) : base(board) {
      mRunningHeuristicCurrent = 0.0;
      mRunningHeuristicOther = 0.0;
      mPermittedExploreSteps = permittedExploreSteps;
      InitializeMaps();
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      mRunningHeuristicCurrent = copyFrom.mRunningHeuristicCurrent;
      mRunningHeuristicOther =copyFrom.mRunningHeuristicOther;
      CopyMaps(copyFrom);
      mPermittedExploreSteps = copyFrom.mPermittedExploreSteps;
    }

    public GameState(GameState copyFrom, int row, int col, int permittedExploreSteps) : base(copyFrom) {
      mRunningHeuristicCurrent = copyFrom.mRunningHeuristicCurrent;
      mRunningHeuristicOther =copyFrom.mRunningHeuristicOther;
      CopyMaps(copyFrom);
      if (!Move(row, col)) {
        throw new Exception("Illegal move: (" + row + ", " + col + ")");
      }
      mPermittedExploreSteps = permittedExploreSteps;
    }

    private void CopyMaps(GameState copyFrom) {
      mInfluenceMap = new int[ROWS];
      mHeuristicMapCurrent = new Tuple<double, int>[ROWS][];
      mHeuristicMapOther = new Tuple<double, int>[ROWS][];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mInfluenceMap[row_dex] = copyFrom.mInfluenceMap[row_dex];
        mHeuristicMapCurrent[row_dex] = new Tuple<double, int>[COLS];
        mHeuristicMapOther[row_dex] = new Tuple<double, int>[COLS];
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          mHeuristicMapCurrent[row_dex][col_dex] = copyFrom.mHeuristicMapCurrent[row_dex][col_dex];
          mHeuristicMapOther[row_dex][col_dex] = copyFrom.mHeuristicMapOther[row_dex][col_dex];
        }
      }
    }

    // This method triggers the Negamax search. It should only be called externally.
    public Tuple<int, int> GetBestMove() {
      Console.WriteLine(" > GetBestMove()");
      mBaseDepth = GetMoveNumber();
      mPossitionsEvaluated = 0;
      Tuple<Tuple<int, int>, Tuple<double, int>> move = Negamax();
      Console.WriteLine(" < GetBestMove()... Possitions evaluated: " + mPossitionsEvaluated + ", heuristic: " + move.Item2.Item1);
      return move.Item1;
    }

    private double TotalHeuristic() {
      double retval = 0.0;
      if (GetWinner() != Player.Neither) {
        if (GetCurrentPlayer() == Player.White) {
          return 1.0;
        } else {
          return -1.0;  // Opponent just moved, so opponent just won.
        }
      }

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (mHeuristicMapCurrent[row_dex][col_dex].Item2 == 0) {
            retval += 1.0;
          } else {
            retval += mHeuristicMapCurrent[row_dex][col_dex].Item1;
            retval -= mHeuristicMapOther[row_dex][col_dex].Item1;
          }
        }
      }
      return retval;
    }

    private Tuple<Tuple<int, int>, Tuple<double, int>> Negamax() {
      GameState nextGamestate;
      mPossitionsEvaluated++;
      int influenceCount = 0;
      if (mPossitionsEvaluated % 512 == 0) {
        Console.WriteLine(" > Negamax(), evals: " + mPossitionsEvaluated);
      }

      if (GetWinner() != Player.Neither) {
        Console.WriteLine("terminal evaluated on move " + GetMoveNumber());
        Tuple<Tuple<int, int>, Tuple<double, int>> ret = new Tuple<Tuple<int, int>, Tuple<double, int>>(null, Tuple.Create(-400.0, 0));
        return ret;
      }

      Tuple<Tuple<int, int>, Tuple<double, int>> champ = null;
      Tuple<Tuple<int, int>, Tuple<double, int>> chump;
      Tuple<int, int> spot;
      int topOffensive = Int32.MaxValue;
      int topDeffensive = Int32.MaxValue;

      foreach (Tuple<Tuple<int, int>, Tuple<double, int>> candidate in GetCandidateMoves()) {
        spot = candidate.Item1;
        if (candidate.Item2.Item2 % 2 == 0) {  // Offensive move
          topOffensive = Math.Min(topOffensive, candidate.Item2.Item2);
        }
        if (candidate.Item2.Item2 % 2 == 1) {  // Deffensive move
          topOffensive = Math.Min(topDeffensive, candidate.Item2.Item2);
        }

        if (candidate.Item2.Item2 <= topOffensive && candidate.Item2.Item2 <= topDeffensive) {
          if (mPermittedExploreSteps > 0) {
            nextGamestate = new GameState(this, candidate.Item1.Item1, candidate.Item1.Item2, mPermittedExploreSteps - 1);
            Tuple<Tuple<int, int>, Tuple<double, int>> recursedReturn = nextGamestate.Negamax();
            chump = Tuple.Create(spot, Tuple.Create(-1 * recursedReturn.Item2.Item1, recursedReturn.Item2.Item2));
          } else {
            chump = Tuple.Create(spot,
                                 Tuple.Create(mRunningHeuristicCurrent - mRunningHeuristicOther + mHeuristicMapCurrent[spot.Item1][spot.Item2].Item1,
                                              HeuristicValues.GetProximityPriority() + 1));
          }
        }
      }

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          int nextPermittedExploreSteps = mPermittedExploreSteps - 1;
          if (!IsLegal(row_dex, col_dex)) {
            continue;
          }

          if (champ != null && champ.Item2.Item1 == 1.0) {
            return champ;
          }

          spot = Tuple.Create(row_dex, col_dex);
          if (nextPermittedExploreSteps >= 0) {
            nextGamestate = new GameState(this, row_dex, col_dex, nextPermittedExploreSteps);
            Tuple<Tuple<int, int>, Tuple<double, int>> recursedReturn = nextGamestate.Negamax();
            chump = Tuple.Create(spot, Tuple.Create(-1 * recursedReturn.Item2.Item1, recursedReturn.Item2.Item2));
          } else {
            chump = Tuple.Create(spot, Tuple.Create(mRunningHeuristicCurrent - mRunningHeuristicOther + mHeuristicMapCurrent[row_dex][col_dex].Item1,
                                                    HeuristicValues.GetProximityPriority() + 1));
          }

          champ = ChooseBest(champ, chump);
        }
      }
      //Console.WriteLine("influenceCount: " + influenceCount + ", champ: " + champ);
      return champ;
    }

    private List<Tuple<Tuple<int, int>, Tuple<double, int>>> GetCandidateMoves() {
      Tuple<int, int> spot;
      List<Tuple<Tuple<int, int>, Tuple<double, int>>> candidates = new List<Tuple<Tuple<int, int>, Tuple<double, int>>>();
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (!IsLegal(row_dex, col_dex)) {
            continue;
          }

          if (IsOnMap(row_dex, col_dex, mInfluenceMap)) {
            spot = Tuple.Create(row_dex, col_dex);
            candidates.Add(Tuple.Create(spot, Tuple.Create(mHeuristicMapCurrent[row_dex][col_dex].Item1, mHeuristicMapCurrent[row_dex][col_dex].Item2)));
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
      mHeuristicMapCurrent = new Tuple<double, int>[ROWS][];
      mHeuristicMapOther = new Tuple<double, int>[ROWS][];
      mInfluenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mHeuristicMapCurrent[row_dex] = new Tuple<double, int>[COLS];
        mHeuristicMapOther[row_dex] = new Tuple<double, int>[COLS];
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          UpdateMaps(row_dex, col_dex);
        }
      }
    }

    private void UpdateMaps(int row, int col) {
      Tuple<double, int> champCurrent = null;
      Tuple<double, int> chumpCurrent = null;
      Tuple<double, int> champOther = null;
      Tuple<double, int> chumpOther = null;
      Dictionary<Pattern, Tuple<double, int>> hDict = HeuristicValues.GetHeuristicDict();
      Pattern pattern;

      foreach (Tuple<int, int> window in GetWindows(row, col)) {
        if (hDict.TryGetValue(WindowToPattern(GetCurrentPlayer(), window), out chumpCurrent)) {
          mInfluenceMap[row] |= SPOT_MASKS[col];
          if (CmpHeuristics(chumpCurrent, champCurrent) >= 0) {
            champCurrent = chumpCurrent;
          }
        }

        if (hDict.TryGetValue(WindowToPattern(GetOtherPlayer(), window), out chumpOther)) {
          mInfluenceMap[row] |= SPOT_MASKS[col];
          if (CmpHeuristics(chumpOther, champOther) >= 0) {
            champOther = chumpOther;
          }
        }

        // Check captures
        if (MatchesPattern(GetCurrentPlayer(), window, HeuristicValues.GetCaptureCheck())) {
          mInfluenceMap[row] |= SPOT_MASKS[col];

          chumpCurrent = HeuristicValues.EstimateQualityOfCapture(GetCurrentPlayer(), GetCaptures(Player.White), GetCaptures(Player.Black));
          if (CmpHeuristics(chumpCurrent, champCurrent) >= 0) {
            champCurrent = chumpCurrent;
          }
        }

        if (MatchesPattern(GetOtherPlayer(), window, HeuristicValues.GetCaptureCheck())) {
          mInfluenceMap[row] |= SPOT_MASKS[col];

          chumpOther = HeuristicValues.EstimateQualityOfCapture(GetOtherPlayer(), GetCaptures(Player.White), GetCaptures(Player.Black));
          if (CmpHeuristics(chumpOther, champOther) >= 0) {
            champOther = chumpOther;
          }
        }
      }

      if (champCurrent == null) {
        mHeuristicMapCurrent[row][col] = Tuple.Create(0.0, HeuristicValues.GetProximityPriority() + 1);
      } else {
        mHeuristicMapCurrent[row][col] = champCurrent;
      }

      if (champOther == null) {
        mHeuristicMapOther[row][col] = Tuple.Create(0.0, HeuristicValues.GetProximityPriority() + 1);
      } else {
        mHeuristicMapOther[row][col] = champOther;
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
        return -1;
      } else if (second == null) {
        return 1;
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
      mRunningHeuristicCurrent += mHeuristicMapCurrent[row][col].Item1;
      bool retval = base.Move(row, col);
      MoveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void MoveTriggeredMapsUpdate(int row, int col) {
      Tuple<double, int>[][] swap = mHeuristicMapCurrent;
      mHeuristicMapCurrent = mHeuristicMapOther;
      mHeuristicMapOther = swap;

      double swapHeuristics = mRunningHeuristicCurrent;
      mRunningHeuristicCurrent = mRunningHeuristicOther;
      mRunningHeuristicOther = swapHeuristics;

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
        pattern.SetPattern(window.Item1, window.Item2);
        if (hDict.TryGetValue(pattern, out val)) {
          return val;
        }
      }

      return Tuple.Create(0.0, HeuristicValues.GetProximityPriority() + 1);
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
