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
      mPermittedExploreSteps = permittedExploreSteps;
      initializeMaps();
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      copyMaps(copyFrom);
      mPermittedExploreSteps = copyFrom.mPermittedExploreSteps;
    }

    public GameState(GameState copyFrom, int row, int col, int permittedExploreSteps) : base(copyFrom) {
      copyMaps(copyFrom);
      if (!move(row, col)) {
        throw new Exception("Illegal move: (" + row + ", " + col + ")");
      }
      mPermittedExploreSteps = permittedExploreSteps;
    }

    private void copyMaps(GameState copyFrom) {
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

    // This method triggers the minimax search. It should only be called externally.
    public Tuple<int, int> getBestMove() {
      Console.WriteLine(" > getBestMove()");
      mBaseDepth = getMoveNumber();
      mPossitionsEvaluated = 0;
      Tuple<int, int> move = minimax().Item1;
      Console.WriteLine("Possitions evaluated: " + mPossitionsEvaluated);
      return move;
    }

    private double totalHeuristic() {
      double retval = 0.0;
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (mHeuristicMapCurrent[row_dex][col_dex].Item2 == 0) {
            return 1.0;
          } else {
            retval += mHeuristicMapCurrent[row_dex][col_dex].Item1;
            retval -= mHeuristicMapOther[row_dex][col_dex].Item1;
          }
        }
      }
      return retval;
    }

    private Tuple<Tuple<int, int>, Tuple<double, int>> minimax() {
      GameState nextGamestate;
      mPossitionsEvaluated++;
      int influenceCount = 0;
      if (mPossitionsEvaluated % 512 == 0) {
        Console.WriteLine(" > minimax(), evals: " + mPossitionsEvaluated);
      }

      if (getWinner() != player_t.neither) {
        Console.WriteLine("terminal evaluated on move " + getMoveNumber());
        double winnerVal = (getWinner() == player_t.white) ? 1.0 : -1.0;
        Tuple<Tuple<int, int>, Tuple<double, int>> ret = new Tuple<Tuple<int, int>, Tuple<double, int>>(null, Tuple.Create(winnerVal, 0));
        return ret;
      }

      Tuple<Tuple<int, int>, Tuple<double, int>> champ = null;
      Tuple<Tuple<int, int>, Tuple<double, int>> chump;
      Tuple<int, int> spot;

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          int nextPermittedExploreSteps = mPermittedExploreSteps - 1;
          if (!isLegal(row_dex, col_dex)) {
            continue;
          }

          /*
          if (isOnMap(row_dex, col_dex, mInfluenceMap)) {
            influenceCount++;
            if (mPermittedExploreSteps > 0) {
              nextPermittedExploreSteps = mPermittedExploreSteps - 1;
            } else {
              nextPermittedExploreSteps = mPermittedExploreSteps;
            }
          } else if (mHeuristicMapCurrent[row_dex][col_dex].Item2 == HeuristicValues.getProximityPriority()) {
            nextPermittedExploreSteps = mPermittedExploreSteps - PENALTY_PROXIMITY;
          } else {
            nextPermittedExploreSteps = mPermittedExploreSteps - PENALTY_NO_PROXIMITY;
          }
          */

          if (champ != null && champ.Item2.Item1 == 1.0) {
            return champ;
          }

          spot = Tuple.Create(row_dex, col_dex);
          if (nextPermittedExploreSteps >= 0) {
            nextGamestate = new GameState(this, row_dex, col_dex, nextPermittedExploreSteps);
            Tuple<Tuple<int, int>, Tuple<double, int>> recursedReturn = nextGamestate.minimax();
            chump = Tuple.Create(spot, Tuple.Create(-1 * recursedReturn.Item2.Item1, recursedReturn.Item2.Item2));
          } else {
            chump = Tuple.Create(spot, Tuple.Create(totalHeuristic(), HeuristicValues.getProximityPriority() + 1));
          }

          /*
          if (champ != null && chump != null && champ.Item2.Item2 < 5 || chump.Item2.Item2 < 5) {
            Console.WriteLine("champ: " + champ + ", chump: " + chump);
          }
          */
          champ = chooseBest(champ, chump);
          if (champ.Item2.Item2 > chump.Item2.Item2) {
            Console.WriteLine("!!!!!!!!!!!!!!!!????");
          }
        }
      }
      //Console.WriteLine("influenceCount: " + influenceCount + ", champ: " + champ);
      return champ;
    }

    private bool isOnMap(int row, int col, int[] map) {
      if ((map[row] & COL_MASKS[col]) != 0) {
        return true;
      } else {
        return false;
      }
    }

    // This function should (almost) never need to be called. Use updateMaps instead if possible.
    private void initializeMaps() {
      mHeuristicMapCurrent = new Tuple<double, int>[ROWS][];
      mHeuristicMapOther = new Tuple<double, int>[ROWS][];
      mInfluenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mHeuristicMapCurrent[row_dex] = new Tuple<double, int>[COLS];
        mHeuristicMapOther[row_dex] = new Tuple<double, int>[COLS];
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          updateMaps(row_dex, col_dex);
        }
      }
    }

    private void updateMaps(int row, int col) {
      Tuple<double, int> champCurrent = null;
      Tuple<double, int> chumpCurrent = null;
      Tuple<double, int> champOther = null;
      Tuple<double, int> chumpOther = null;

      foreach (Tuple<int, int> window in getWindows(row, col)) {
        // Check all heuristic rules.
        foreach (Tuple<Tuple<int, int, int>, Tuple<double, int>> heuristicPair in HeuristicValues.getHeuristics()) {
          if (matchesPattern(getCurrentPlayer(), window, heuristicPair.Item1)) {
            mInfluenceMap[row] |= COL_MASKS[col];

            chumpCurrent = heuristicPair.Item2;
            if (cmpHeuristics(chumpCurrent, champCurrent) >= 0) {
              champCurrent = chumpCurrent;
            }
          }

          if (matchesPattern(getOtherPlayer(), window, heuristicPair.Item1)) {
            mInfluenceMap[row] |= COL_MASKS[col];

            chumpOther = heuristicPair.Item2;
            if (cmpHeuristics(chumpOther, champOther) >= 0) {
              champOther = chumpOther;
            }
          }
        }

        // Check captures
        if (matchesPattern(getCurrentPlayer(), window, HeuristicValues.getCaptureCheck())) {
          mInfluenceMap[row] |= COL_MASKS[col];

          chumpCurrent = HeuristicValues.estimateQualityOfCapture(getCurrentPlayer(), getCaptures(player_t.white), getCaptures(player_t.black));
          if (cmpHeuristics(chumpCurrent, champCurrent) >= 0) {
            champCurrent = chumpCurrent;
          }
        }

        if (matchesPattern(getOtherPlayer(), window, HeuristicValues.getCaptureCheck())) {
          mInfluenceMap[row] |= COL_MASKS[col];

          chumpOther = HeuristicValues.estimateQualityOfCapture(getOtherPlayer(), getCaptures(player_t.white), getCaptures(player_t.black));
          if (cmpHeuristics(chumpOther, champOther) >= 0) {
            champOther = chumpOther;
          }
        }
      }
      if (champCurrent == null) {
        mHeuristicMapCurrent[row][col] = Tuple.Create(0.0, HeuristicValues.getProximityPriority() + 1);
      } else {
        mHeuristicMapCurrent[row][col] = champCurrent;
      }

      if (champOther == null) {
        mHeuristicMapOther[row][col] = Tuple.Create(0.0, HeuristicValues.getProximityPriority() + 1);
      } else {
        mHeuristicMapOther[row][col] = champOther;
      }
    }

    // After finding the expected winner and uncertainy (the second tuple) associated with a move (the first tuple), determine which one is better.
    private Tuple<Tuple<int, int>, Tuple<double, int>> chooseBest(Tuple<Tuple<int, int>, Tuple<double, int>> a,
                                                                  Tuple<Tuple<int, int>, Tuple<double, int>> b) {
      if (a == null && b == null) {
        return null;
      } else if (a == null) {
        return b;
      } else if (b == null) {
        return a;
      } else if (cmpHeuristics(a.Item2, b.Item2) >= 0) {
        return a;
      } else {
        return b;
      }
    }

    private int cmpHeuristics(Tuple<double, int> first, Tuple<double, int> second) {
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

    public override bool move(int row, int col) {
      bool retval = base.move(row, col);
      moveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void moveTriggeredMapsUpdate(int row, int col) {
      Tuple<double, int>[][] swap = mHeuristicMapCurrent;
      mHeuristicMapCurrent = mHeuristicMapOther;
      mHeuristicMapOther = swap;

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
            updateMaps(inspectedRow, inspectedCol);
          }
        }
      }
    }

    // TODO, this doesn't check for captures.
    public Tuple<double, int> getHeuristicValue(int row, int col) {
      if (!isLegal(row, col)) {
        return Tuple.Create(0.0, HeuristicValues.getProximityPriority() + 1);
      } else {
        int patternCurrent, patternOther;
        // Finds the largest absolute value. If the value is negative, then the guess is that the opponent wil win.
        foreach (Tuple<int, int> window in getWindows(row, col)) {
          foreach (Tuple<Tuple<int, int, int>, Tuple<double, int>> heuristicPair in HeuristicValues.getHeuristics()) {
            if (matchesPattern(getCurrentPlayer(), window, heuristicPair.Item1)) {
              return heuristicPair.Item2;
            }
          }
        }
      }

      return Tuple.Create(0.0, HeuristicValues.getProximityPriority() + 1);
    }
  }
}
