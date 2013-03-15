using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  class GameState : Board {
    private List<int[]> mInfluenceMapsWhite;
    private List<int[]> mInfluenceMapsBlack;
    private int[] mCaptureMapWhite;
    private int[] mCaptureMapBlack;
    private int[] mProximityMapWhite;
    private int[] mProximityMapBlack;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    private double? mHeuristicValue;
    private Tuple<int, int> mBestMove;
    private int mDepthPermitted;

    // If depthPermitted == 0, then the heuristic value of the best move will be returned... no further
    // recursion will occure.
    public GameState(Board board, int depthPermitted) : base(board) {
      mDepthPermitted = depthPermitted;
      mHeuristicValue = null;
      initializeInfluenceMaps();
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      mDepthPermitted = copyFrom.mDepthPermitted;
      mHeuristicValue = null;
    }

    public GameState(GameState copyFrom, int row, int col, int depthPermitted) : base(copyFrom) {
      if (!move(row, col)) {
        throw new Exception("Illegal move: (" + row + ", " + col + ")");
      }
      mDepthPermitted = depthPermitted;
      mHeuristicValue = null;
    }

    // TODO:
    // This method isn't quite doing the correct thing... what's happening here should only occure
    // at a depth limit. Otherwise, the lookahead method should initialize a few new maps that are
    // generated from the minimax recursive approach. Perhaps only one map should be kept... but this
    // would require that the heuristics list the desirability of each level. That way, only results
    // at the top desirability level will be recorded.
    public Tuple<int, int> getBestMove() {
      List<int[]> influenceMaps;
      if (getCurrentPlayer() == player_t.white) {
        influenceMaps = mInfluenceMapsWhite;
      } else {
        influenceMaps = mInfluenceMapsBlack;
      }

      for (int level = 0; level < influenceMaps.Count; level++) {
        List<Tuple<int, int>> movesThisLevel = new List<Tuple<int, int>>();

        for (int row_dex = 0; row_dex < ROWS; row_dex++) {
          for (int col_dex = 0; col_dex < COLS; col_dex++) {
            if (isOnMap(row_dex, col_dex, influenceMaps[level])) {
              movesThisLevel.Add(Tuple.Create(row_dex, col_dex));
              continue;
            }

            if ((getCurrentPlayer() == player_t.white && isOnMap(row_dex, col_dex, mCaptureMapWhite)) ||
                (getCurrentPlayer() == player_t.black && isOnMap(row_dex, col_dex, mCaptureMapBlack))) {
              if (level == 0 && getCaptures(getCurrentPlayer()) == 4) {
                // Will win with one more capture
                movesThisLevel.Add(Tuple.Create(row_dex, col_dex));
              } else if (level == 2 && getCaptures(getCurrentPlayer()) == 3) {
                // Will possibly threaten a win
                movesThisLevel.Add(Tuple.Create(row_dex, col_dex));
              } else if (level >= 3) {
                // This is the same level as preventing or threatening a capture.
                movesThisLevel.Add(Tuple.Create(row_dex, col_dex));
              }
            }
          }
        }

        if (movesThisLevel.Count > 0) {
          // Select random move from the level
          Random rand = new Random();
          return movesThisLevel[rand.Next(movesThisLevel.Count];
        }
      }

      // TODO
      // Proximity check if nothing else has triggered.

      getHeuristicValue();
      return mBestMove;
    }

    private bool isOnMap(int row, int col, int[] map) {
      if ((map[row] & COL_MASKS[col]) != 0) {
        return true;
      } else {
        return false;
      }
    }

    // This function should (almost) never need to be called. Use updateInfluenceMap instead if possible.
    private void initializeInfluenceMaps() {
      List<List<Tuple<int, int, int, double> > > heuristics = HeuristicValues.getHeuristics();
      mInfluenceMapsWhite = new List<int[]>();
      mInfluenceMapsBlack = new List<int[]>();
      mCaptureMapWhite = new int[ROWS];
      mCaptureMapBlack = new int[ROWS];
      mProximityMapWhite = new int[ROWS];
      mProximityMapBlack = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          updateMaps(row_dex, col_dex);
        }
      }
    }

    private void updateMaps(int row, int col) {
      foreach (Tuple<int, int> window in getWindows(row, col)) {
        // Influence maps
        int level = 0;
        foreach (List<Tuple<int, int, int, double>> level in heuristics) {
          mInfluenceMapsWhite.Add(new int[ROWS]);
          mInfluenceMapsBlack.Add(new int[ROWS]);

          foreach (Tuple<int, int, int, double> pattern in level) {
            if (matchesPattern(windowWhite: window.Item1, windowBlack: window.Item2,
                               patternWhite: pattern.Item1, patternBlack: pattern.Item2, patternIgnore: pattern.Item3)) {
              mInfluenceMapsWhite[level][row] |= COL_MASKS[col];
            }
            if (matchesPattern(windowWhite: window.Item2, windowBlack: window.Item1,
                               patternWhite: pattern.Item2, patternBlack: pattern.Item1, patternIgnore: pattern.Item3)) {
              // Swapped black and white players
              mInfluenceMapsBlack[level][row] |= COL_MASKS[col];
            }
          }
          level++;
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

        // Capture maps and proximity maps
        List<Tuple<int, int, int, double> > proximityMaps = HeuristicValues.getProximityChecks();
        foreach (Tuple<int, int, int, double> pattern in proximityMaps) {
          if (matchesPattern(windowWhite: window.Item1, windowBlack: window.Item2,
                             patternWhite: pattern.Item1, patternBlack: pattern.Item2, patternIgnore: pattern.Item3)) {
            mProximityMapWhite[row] |= COL_MASKS[col];
          }
          // Swapped black and white
          if (matchesPattern(windowWhite: window.Item2, windowBlack: window.Item1,
                             patternWhite: pattern.Item2, patternBlack: pattern.Item1, patternIgnore: pattern.Item3)) {
            mProximityMapBlack[row] |= COL_MASKS[col];
          }
        }
      }
    }

    public double? getHeuristicValue() {
      initializeInfluenceMaps();
      if (mDepthPermitted > 0) {
        lookahead();
        if (getCurrentPlayer() == player_t.white) {
          return mHeuristicValue;
        } else {
          return -1.0 * mHeuristicValue;
        }
      } else {
        if (getWinner() != player_t.neither) {
          if (getWinner() == player_t.white) {
            return HeuristicValues.getWin();
          } else {
            return -1 * HeuristicValues.getWin();
          }
        } else {
          foreach (KeyValuePair<Tuple<int, int>, double> scoredSpot in mInfluenceMap) {
            mBestMove = mInfluenceMap[0].Key;
            if (isLegal(mBestMove.Item1, mBestMove.Item2)) {
              break;
            }
          }

          if (getCurrentPlayer() == player_t.white) {
            return mInfluenceMap[0].Value;
          } else {
            return (-1.0) * mInfluenceMap[0].Value;
          }
        }
      }
    }

    private void lookahead() {
      int index = 0;
      double? roundValue;
      foreach (KeyValuePair<Tuple<int, int>, double> scoredSpot in mInfluenceMap) {
        if (isLegal(scoredSpot.Key.Item1, scoredSpot.Key.Item2)) {
          //Console.WriteLine("branching check... " + index + ", " + scoredSpot.Key);
          if (index <= branchingCategories[0]) {
            if (mDepthPermitted <= 0) {

              //Console.WriteLine("branching check... broken" + index);
              break;
            }

            roundValue = new GameState(copyFrom: this, row: scoredSpot.Key.Item1, col: scoredSpot.Key.Item2, depthPermitted: this.mDepthPermitted - 1).getHeuristicValue();
          } else if (index <= branchingCategories[1]) {
            if (mDepthPermitted <= 1) {
              break;
            }
            roundValue = new GameState(copyFrom: this, row: scoredSpot.Key.Item1, col: scoredSpot.Key.Item2, depthPermitted: this.mDepthPermitted - 2).getHeuristicValue();
          } else if (index <= branchingCategories[2]) {
            if (mDepthPermitted <= 2) {
              break;
            }
            roundValue = new GameState(copyFrom: this, row: scoredSpot.Key.Item1, col: scoredSpot.Key.Item2, depthPermitted: this.mDepthPermitted - 3).getHeuristicValue();
          } else {
            if (mDepthPermitted <= 3) {
              break;
            }
            roundValue = new GameState(copyFrom: this, row: scoredSpot.Key.Item1, col: scoredSpot.Key.Item2, depthPermitted: this.mDepthPermitted - 4).getHeuristicValue();
          }

          if ((mHeuristicValue == null) || (roundValue > mHeuristicValue)) {
            mHeuristicValue = roundValue;
            mBestMove = scoredSpot.Key;
          }
        }
      }
    }

    private double getInfluenceValue(Tuple<int, int> key) {
      double retval = 0.0;
      int correctPatternWhite, correctPatternBlack;
      foreach (Tuple<int, int> windows in getWindows(key.Item1, key.Item2)) {
        foreach (Tuple<int, int, int, double> heuristic in HeuristicValues.getHeuristics()) {
          if (getCurrentPlayer() == player_t.white) {
            correctPatternWhite = heuristic.Item1;
            correctPatternBlack = heuristic.Item2;
          } else {
            correctPatternWhite = heuristic.Item2;
            correctPatternBlack = heuristic.Item1;
          }

          if (matchesPattern(
              windowWhite: windows.Item1, windowBlack: windows.Item2,
              patternWhite: heuristic.Item1, patternBlack: heuristic.Item2, patternIgnore: heuristic.Item3)) {
            retval += heuristic.Item4;
          }
        }

        // Special check for captures
        Tuple<int, int, int, int> captureParams = HeuristicValues.getCaptureParams();

        if (getCurrentPlayer() == player_t.white) {
          correctPatternWhite = captureParams.Item1;
          correctPatternBlack = captureParams.Item2;
        } else {
          correctPatternWhite = captureParams.Item2;
          correctPatternBlack = captureParams.Item1;
        }

        if (matchesPattern(windowWhite: windows.Item1, windowBlack: windows.Item2,
                           patternWhite: correctPatternWhite, patternBlack: correctPatternBlack, patternIgnore: captureParams.Item3)) {
          if (getCaptures(getCurrentPlayer()) == 4) {
            retval += HeuristicValues.getWin();
          } else {
            retval += (getCaptures(getCurrentPlayer()) + 1) * HeuristicValues.getBigDelta();
          }
        }
      }

      return retval;
    }

    public override bool move(int row, int col) {
      bool retval = move(row, col);
      moveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void moveTriggeredMapsUpdate(int row, int col) {
      List<Tuple<int, int>> directions = new List<Tuple<int, int>>();
      directions.Add(new Tuple<int, int>(0, 1));  // horizontal
      directions.Add(new Tuple<int, int>(1, 0));  // vertical
      directions.Add(new Tuple<int, int>(1, 1));  // forward slash
      directions.Add(new Tuple<int, int>(1, -1));  // back slash

      int index = 0;
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
  }
}
