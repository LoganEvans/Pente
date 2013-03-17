using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  class GameState : Board {
    private List<double[]> mHeuristicMap;
    private int[] mInfluenceMapWhite;
    private int[] mInfluenceMapBlack;
    private static int mPossitionsEvaluated;
    private int[] mCaptureMapWhite;
    private int[] mCaptureMapBlack;
    private int[] mProximityMapWhite;
    private int[] mProximityMapBlack;

    private readonly int PENALTY_PROXIMITY = 1;
    private readonly int PENALTY_NO_PROXIMITY = 3;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    private Tuple<int, int> mBestMove;
    private int mPermittedExploreSteps;

    // If depthPermitted == 0, then the heuristic value of the best move will be returned... no further
    // recursion will occure.
    public GameState(Board board, int permittedExploreSteps) : base(board) {
      mPermittedExploreSteps = permittedExploreSteps;
      initializeInfluenceMaps();
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
      mInfluenceMapWhite = new int[ROWS];
      mInfluenceMapBlack = new int[ROWS];
      mCaptureMapWhite = new int[ROWS];
      mCaptureMapBlack = new int[ROWS];
      mProximityMapWhite = new int[ROWS];
      mProximityMapBlack = new int[ROWS];
      mHeuristicMap = new List<double[]>();

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mInfluenceMapWhite[row_dex] = copyFrom.mInfluenceMapWhite[row_dex];
        mInfluenceMapBlack[row_dex] = copyFrom.mInfluenceMapBlack[row_dex];
        mCaptureMapWhite[row_dex] = copyFrom.mCaptureMapWhite[row_dex];
        mCaptureMapBlack[row_dex] = copyFrom.mCaptureMapBlack[row_dex];
        mProximityMapWhite[row_dex] = copyFrom.mProximityMapWhite[row_dex];
        mProximityMapBlack[row_dex] = copyFrom.mProximityMapBlack[row_dex];

        mHeuristicMap.Add(new double[COLS]);
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          mHeuristicMap[row_dex][col_dex] = copyFrom.mHeuristicMap[row_dex][col_dex];
        }
      }
    }

    // This method triggers the minimax search. It should only be called externally.
    public Tuple<int, int> getBestMove() {
      Console.WriteLine(" > getBestMove()");
      mPossitionsEvaluated = 0;
      Tuple<int, int> move = minimax().Item1;
      Console.WriteLine("Possitions evaluated: " + mPossitionsEvaluated);
      return move;
    }

    private Tuple<Tuple<int, int>, double> minimax() {
      GameState nextGamestate;
      mPossitionsEvaluated++;
      if (mPossitionsEvaluated % 512 == 0) {
        Console.WriteLine(" > minimax(), evals: " + mPossitionsEvaluated);
      }

      if (getWinner() != player_t.neither) {
        Console.WriteLine("terminal evaluated");
        return Tuple.Create(Tuple.Create(-1, -1), (getWinner() == player_t.white) ? 1.0 : -1.0);
      }

      int[] influenceMap;
      int[] captureMap;
      int[] proximityMap;
      if (getCurrentPlayer() == player_t.white) {
        influenceMap = mInfluenceMapWhite;
        captureMap = mCaptureMapWhite;
        proximityMap = mProximityMapWhite;
      } else {
        influenceMap = mInfluenceMapBlack;
        captureMap = mCaptureMapBlack;
        proximityMap = mProximityMapBlack;
      }

      Tuple<Tuple<int, int>, double> champ = null;
      Tuple<Tuple<int, int>, double> chump;
      Tuple<int, int> spot;

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          int nextPermittedExploreSteps;
          if (!isLegal(row_dex, col_dex)) {
            continue;
          }

          if (isOnMap(row_dex, col_dex, influenceMap) || isOnMap(row_dex, col_dex, captureMap)) {
            if (mPermittedExploreSteps > 0) {
              nextPermittedExploreSteps = mPermittedExploreSteps - 1;
            } else {
              nextPermittedExploreSteps = mPermittedExploreSteps;
            }
          } else if (isOnMap(row_dex, col_dex, proximityMap)) {
            nextPermittedExploreSteps = mPermittedExploreSteps - PENALTY_PROXIMITY;
          } else {
            nextPermittedExploreSteps = mPermittedExploreSteps - PENALTY_NO_PROXIMITY;
          }

          spot = Tuple.Create(row_dex, col_dex);
          if (nextPermittedExploreSteps >= 0) {
            nextGamestate = new GameState(this, row_dex, col_dex, nextPermittedExploreSteps);
///////////
            chump = Tuple.Create(spot, nextGamestate.minimax().Item2);
          } else {
            chump = Tuple.Create(spot, mHeuristicMap[row_dex][col_dex]);
            if ((getCurrentPlayer() == player_t.white && isOnMap(row_dex, col_dex, mCaptureMapWhite)) ||
                (getCurrentPlayer() == player_t.black && isOnMap(row_dex, col_dex, mCaptureMapBlack))) {
              double captureValue = HeuristicValues.estimateQualityOfCapture(getCurrentPlayer(), getCaptures(player_t.white), getCaptures(player_t.black));
              if (chump != null && captureValue > chump.Item2) {
                chump = Tuple.Create(spot, captureValue);
              }
            }
          }

          champ = chooseBest(champ, chump);
        }
      }
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
    private void initializeInfluenceMaps() {
      mHeuristicMap = new List<double[]>();
      mInfluenceMapWhite = new int[ROWS];
      mInfluenceMapBlack = new int[ROWS];
      mCaptureMapWhite = new int[ROWS];
      mCaptureMapBlack = new int[ROWS];
      mProximityMapWhite = new int[ROWS];
      mProximityMapBlack = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mHeuristicMap.Add(new double[COLS]);
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          updateMaps(row_dex, col_dex);
        }
      }
    }

    private void updateMaps(int row, int col) {
      bool isProximity;
      bool whiteProximity, blackProximity;

      foreach (Tuple<int, int> window in getWindows(row, col)) {
        isProximity = false;
        whiteProximity = false;
        blackProximity = false;
        // Proximity maps
        foreach (Tuple<int, int, int, double> proximity in HeuristicValues.getProximity()) {
          if (whiteProximity || (mProximityMapWhite[row] & COL_MASKS[col]) != 0) {
            // Pass
            whiteProximity = true;
            if (blackProximity) {
              break;
            }
          } else if (matchesPattern(windowWhite: window.Item1, windowBlack: window.Item2,
                             patternWhite: proximity.Item1, patternBlack: proximity.Item2, patternIgnore: proximity.Item3)) {
            mProximityMapWhite[row] |= COL_MASKS[col];
            isProximity = true;
            whiteProximity = true;
          }

          if (blackProximity || (mProximityMapBlack[row] & COL_MASKS[col]) != 0) {
            blackProximity = true;
          } else if (matchesPattern(windowWhite: window.Item2, windowBlack: window.Item1,
                             patternWhite: proximity.Item2, patternBlack: proximity.Item1, patternIgnore: proximity.Item3)) {
            // Swapped black and white players
            mProximityMapBlack[row] |= COL_MASKS[col];
            isProximity = true;
            blackProximity = true;
          }
        }

        // Influence maps
        foreach (Tuple<int, int, int, double> heuristic in HeuristicValues.getHeuristics()) {
          if (whiteProximity && matchesPattern(windowWhite: window.Item1, windowBlack: window.Item2,
                                               patternWhite: heuristic.Item1, patternBlack: heuristic.Item2, patternIgnore: heuristic.Item3)) {
            mInfluenceMapWhite[row] |= COL_MASKS[col];
            whiteProximity = true;
          }
          if (blackProximity && matchesPattern(windowWhite: window.Item2, windowBlack: window.Item1,
                                               patternWhite: heuristic.Item2, patternBlack: heuristic.Item1, patternIgnore: heuristic.Item3)) {
            // Swapped black and white players
            mInfluenceMapBlack[row] |= COL_MASKS[col];
            blackProximity = true;
          }

          if (whiteProximity && blackProximity) {
            break;
          }
        }

        // Capture maps
        Tuple<int, int, int> capturePattern = HeuristicValues.getCaptureCheck();
        if (matchesPattern(windowWhite: window.Item1, windowBlack: window.Item2,
                           patternWhite: capturePattern.Item1, patternBlack: capturePattern.Item2, patternIgnore: capturePattern.Item3)) {
          mCaptureMapWhite[row] |= COL_MASKS[col];
        }
        // Swapped black and white
        if (matchesPattern(windowWhite: window.Item2, windowBlack: window.Item1,
                           patternWhite: capturePattern.Item2, patternBlack: capturePattern.Item1, patternIgnore: capturePattern.Item3)) {
          mCaptureMapWhite[row] |= COL_MASKS[col];
        }

      }
      mHeuristicMap[row][col] = getHeuristicValue(row, col);
    }

    // After finding the expected winner and uncertainy (the second tuple) associated with a move (the first tuple), determine which one is better.
    private Tuple<Tuple<int, int>, double> chooseBest(Tuple<Tuple<int, int>, double> a, Tuple<Tuple<int, int>, double> b) {
      if (a == null && b == null) {
        return null;
      } else if (a == null) {
        return b;
      } else if (b == null) {
        return a;
      } else if (Math.Abs(a.Item2) > Math.Abs(b.Item2)) {
        return a;
      } else {
        return b;
      }
    }

    public override bool move(int row, int col) {
      bool retval = base.move(row, col);
      moveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void moveTriggeredMapsUpdate(int row, int col) {
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
    public double getHeuristicValue(int row, int col) {
      if (!isLegal(row, col)) {
        return 0.0;
      } else {
        int patternCurrent, patternOther;
        // Finds the largest absolute value. If the value is negative, then the guess is that the opponent wil win.
        foreach (Tuple<int, int> window in getWindows(row, col)) {
          foreach (Tuple<int, int, int, double> heuristic in HeuristicValues.getHeuristics()) {
            if (getCurrentPlayer() == player_t.white) {
              patternCurrent = heuristic.Item1;
              patternOther = heuristic.Item2;
            } else {
              patternCurrent = heuristic.Item2;
              patternOther = heuristic.Item1;
            }

            if (matchesPattern(window.Item1, window.Item2, patternCurrent, patternOther, heuristic.Item3)) {
              return heuristic.Item4;
            }
          }
        }
      }

      return 0.0;
    }
  }
}
