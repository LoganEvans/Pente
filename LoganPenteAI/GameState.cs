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

    private readonly int PENALTY_PROXIMITY = 1;
    private readonly int PENALTY_NO_PROXIMITY = 3;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    private double? mHeuristicValue;
    private Tuple<int, int> mBestMove;
    private int mPermittedExploreSteps;

    // If depthPermitted == 0, then the heuristic value of the best move will be returned... no further
    // recursion will occure.
    public GameState(Board board, int permittedExploreSteps) : base(board) {
      mPermittedExploreSteps = permittedExploreSteps;
      mHeuristicValue = null;
      initializeInfluenceMaps();
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      copyMaps(copyFrom);
      mPermittedExploreSteps = copyFrom.mPermittedExploreSteps;
      mHeuristicValue = null;
    }

    public GameState(GameState copyFrom, int row, int col, int permittedExploreSteps) : base(copyFrom) {
      copyMaps(copyFrom);
      if (!move(row, col)) {
        throw new Exception("Illegal move: (" + row + ", " + col + ")");
      }
      mPermittedExploreSteps = permittedExploreSteps;
      mHeuristicValue = null;
    }

    private void copyMaps(GameState copyFrom) {
      mInfluenceMapsWhite = new List<int[]>();
      mInfluenceMapsBlack = new List<int[]>();
      int[] toAddWhite;
      int[] toAddBlack;
      for (int i = 0; i < copyFrom.mInfluenceMapsWhite.Count; i++) {
        toAddWhite = new int[ROWS];
        toAddBlack = new int[ROWS];
        for (int row_dex = 0; row_dex < ROWS; row_dex++) {
          toAddWhite[row_dex] = copyFrom.mInfluenceMapsWhite[i][row_dex];
          toAddBlack[row_dex] = copyFrom.mInfluenceMapsBlack[i][row_dex];
        }
        mInfluenceMapsWhite.Add(toAddWhite);        
        mInfluenceMapsBlack.Add(toAddBlack);        
      }

      mCaptureMapWhite = new int[ROWS];
      mCaptureMapBlack = new int[ROWS];
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mCaptureMapWhite[row_dex] = copyFrom.mCaptureMapWhite[row_dex];
        mCaptureMapBlack[row_dex] = copyFrom.mCaptureMapBlack[row_dex];
      }
    }

    // This method triggers the minimax search. It should only be called externally.
    public Tuple<int, int> getBestMove() {
      return minimax().Item1;
    }

    private Tuple<Tuple<int, int>, Tuple<player_t, double>> estimateWinnerAndUncertaintyAtDepth() {
      List<int[]> influenceMaps;
      if (getCurrentPlayer() == player_t.white) {
        influenceMaps = mInfluenceMapsWhite;
      } else {
        influenceMaps = mInfluenceMapsBlack;
      }
      Tuple<int, int> spot;
      Tuple<int, int> legalSpot = null;

      for (int level = 0; level < influenceMaps.Count; level++) {
        List<Tuple<int, int>> movesThisLevel = new List<Tuple<int, int>>();

        for (int row_dex = 0; row_dex < ROWS; row_dex++) {
          for (int col_dex = 0; col_dex < COLS; col_dex++) {
            if (!isLegal(row_dex, col_dex)) {
              continue;
            } else if (legalSpot == null) {
              legalSpot = Tuple.Create(row_dex, col_dex);
            }
            spot = Tuple.Create(row_dex, col_dex);

            if (isOnMap(row_dex, col_dex, influenceMaps[level])) {
              movesThisLevel.Add(spot);
              continue;
            }

            if ((getCurrentPlayer() == player_t.white && isOnMap(row_dex, col_dex, mCaptureMapWhite)) ||
                (getCurrentPlayer() == player_t.black && isOnMap(row_dex, col_dex, mCaptureMapBlack))) {
              if (level == 0 && getCaptures(getCurrentPlayer()) == 4) {
                // Will win with one more capture
                movesThisLevel.Add(spot);
              } else if (level == 2 && getCaptures(getCurrentPlayer()) == 3) {
                // Will possibly threaten a win
                movesThisLevel.Add(spot);
              } else if (level >= 3) {
                // This is the same level as preventing or threatening a capture.
                movesThisLevel.Add(spot);
              }
            }
          }
        }

        if (movesThisLevel.Count > 0) {
          return Tuple.Create(movesThisLevel[0], HeuristicValues.estimateQuality(level, getCurrentPlayer()));
        }
      }

      return Tuple.Create(legalSpot, HeuristicValues.estimateQuality(5, getCurrentPlayer()));
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
        int level_dex = 0;
        foreach (List<Tuple<int, int, int, double>> level in HeuristicValues.getHeuristics()) {
          mInfluenceMapsWhite.Add(new int[ROWS]);
          mInfluenceMapsBlack.Add(new int[ROWS]);

          foreach (Tuple<int, int, int, double> pattern in level) {
            if (matchesPattern(windowWhite: window.Item1, windowBlack: window.Item2,
                               patternWhite: pattern.Item1, patternBlack: pattern.Item2, patternIgnore: pattern.Item3)) {
              mInfluenceMapsWhite[level_dex][row] |= COL_MASKS[col];
            }
            if (matchesPattern(windowWhite: window.Item2, windowBlack: window.Item1,
                               patternWhite: pattern.Item2, patternBlack: pattern.Item1, patternIgnore: pattern.Item3)) {
              // Swapped black and white players
              mInfluenceMapsBlack[level_dex][row] |= COL_MASKS[col];
            }
          }
          level_dex++;
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
    }

    // After finding the expected winner and uncertainy (the second tuple) associated with a move (the first tuple), determine which one is better.
    private Tuple<Tuple<int, int>, Tuple<player_t, double>> chooseBest(Tuple<Tuple<int, int>, Tuple<player_t, double>> a, Tuple<Tuple<int, int>, Tuple<player_t, double>> b) {
      player_t currentPlayer;
      if (getCurrentPlayer() == player_t.white) {
        currentPlayer = player_t.white;
      } else {
        currentPlayer = player_t.black;
      }

      if (a == null && b == null) {
        return null;
      } else if (a == null) {
        return b;
      } else if (b == null) {
        return a;
      } else if (a.Item2.Item1 == currentPlayer && b.Item2.Item1 == currentPlayer) {
        // Projected winner is the current player, decide between them based on the uncertainty (lower is better here)
        if (a.Item2.Item2 < b.Item2.Item2) {
          return a;
        } else {
          return b;
        }
      } else if (a.Item2.Item1 == currentPlayer) {
        return a;
      } else if (b.Item2.Item1 == currentPlayer) {
        return b;
      } else {
        // Both moves are losing moves, so pick the one with a higher uncertainty
        if (a.Item2.Item2 > b.Item2.Item2) {
          return a;
        } else {
          return b;
        }
      }
    }

    private Tuple<Tuple<int, int>, Tuple<player_t, double>> minimax() {
      Console.WriteLine(" > minimax(), moveNumber: " + getMoveNumber() + ", permittedExploreSteps: " + mPermittedExploreSteps);
      if (mPermittedExploreSteps <= 0) {
        return estimateWinnerAndUncertaintyAtDepth();
      }

      List<int[]> influenceMaps;
      int[] explored = new int[ROWS];

      Tuple<Tuple<int, int>, Tuple<player_t, double>> bestSoFar = null;
      Tuple<Tuple<int, int>, Tuple<player_t, double>> chump = null;
      Tuple<int, int> spot;

      int[] captureMap;

      if (getCurrentPlayer() == player_t.white) {
        influenceMaps = mInfluenceMapsWhite;
        captureMap = mCaptureMapWhite;
      } else {
        influenceMaps = mInfluenceMapsBlack;
        captureMap = mCaptureMapBlack;
      }

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          spot = Tuple.Create(row_dex, col_dex);
          for (int level = 0; level < influenceMaps.Count; level++) {
            if (isOnMap(row_dex, col_dex, explored)) {
              // This move has already triggered a recursive call
              break;
            }

            if (!isLegal(row_dex, col_dex)) {
              // Spot is not legal, so try next spot
              break;;
            } else if (bestSoFar == null) {
              bestSoFar = Tuple.Create(spot, HeuristicValues.estimateQuality(level, getCurrentPlayer()));
            }

            if (isOnMap(row_dex, col_dex, influenceMaps[level])) {
              explored[row_dex] |= COL_MASKS[col_dex];
              if (level < HeuristicValues.explorationLevel) {
                // The branching factor should be really low here, so permit a full lookahead
                chump = new GameState(this, row_dex, col_dex, mPermittedExploreSteps).minimax();
              } else if (level == HeuristicValues.explorationLevel) {
                // Branching is significantly higher, so decrement permitted explore steps
                chump = new GameState(this, row_dex, col_dex, mPermittedExploreSteps - PENALTY_PROXIMITY).minimax();
              } else {
                // The entire board is in this level. Only permit a lookahead if a lot of leeway is being permitted.
                chump = new GameState(this, row_dex, col_dex, mPermittedExploreSteps - PENALTY_NO_PROXIMITY).minimax();
              }
            }
          }

          /*
          // TODO. This code will trigger a lookahead on captures.
          if (isOnMap(row_dex, col_dex, captureMap)) {
            if (chump != null) {
              chump = chooseBest(chump,
                                 Tuple.Create(spot,
                                              HeuristicValues.estimateQualityOfCapture(getCurrentPlayer(),
                                                                                       getCaptures(player_t.white),
                                                                                       getCaptures(player_t.black))));
            } else {
              chump = Tuple.Create(spot, HeuristicValues.estimateQualityOfCapture(getCurrentPlayer(), getCaptures(player_t.white), getCaptures(player_t.black)));
            }
          }
          */

          if (bestSoFar == null) {
            bestSoFar = chump;
          } else {
            bestSoFar = chooseBest(bestSoFar, chump);
          }
        }
      }
      return bestSoFar;
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
  }
}
