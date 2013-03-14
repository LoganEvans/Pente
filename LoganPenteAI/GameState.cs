using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  class GameState : Board {
    private List<KeyValuePair<Tuple<int, int>, double> > mInfluenceMap;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    private double? mHeuristicValue;
    private Tuple<int, int> mBestMove;
    private int mDepthPermitted;

    // If depthPermitted == 0, then the heuristic value of the best move will be returned... no further
    // recursion will occure.
    public GameState(Board board, int depthPermitted) : base(board) {
      mDepthPermitted = depthPermitted;
      mHeuristicValue = null;
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

    public Tuple<int, int> getBestMove() {
      // Triggers the update of mBestMove.
      getHeuristicValue();
      return mBestMove;
    }

    private void initializeInfluenceMap() {
      mInfluenceMap = new List<KeyValuePair<Tuple<int, int>, double> >();

      for (int row_dex = 0; row_dex < Board.ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < Board.COLS; col_dex++) {
          //Console.WriteLine(" influence map: " + row_dex + ", " + col_dex);
          Tuple<int, int> key = new Tuple<int, int>(row_dex, col_dex);
          mInfluenceMap.Add(new KeyValuePair<Tuple<int, int>, double>(key, getInfluenceValue(key)));
        }
      }

      mInfluenceMap.Sort(
          delegate(KeyValuePair<Tuple<int, int>, double> first, KeyValuePair<Tuple<int, int>, double> second) {
            return first.Value.CompareTo(second.Value);
          });
    }

    private double? getHeuristicValue() {
      initializeInfluenceMap();
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
          mBestMove = mInfluenceMap[0].Key;
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
        try {
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
        } catch (Exception) {
          continue;
        }

        if ((mHeuristicValue == null) || (roundValue > mHeuristicValue)) {
          mHeuristicValue = roundValue;
          mBestMove = scoredSpot.Key;
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
  }

  static class HeuristicValues {
    private static List<Tuple<int, int, int, double> > _heuristics;
    private static Tuple<int, int, int, int> _captureParams;
 
    // The Heuristics are primarily to identify spots to look at so that the branching
    // factor will be kept low.
    static HeuristicValues() {
      _heuristics = new List<Tuple<int, int, int, double> >();
      double win = getWin();
      double delta = getDelta();
      double bigDelta = getBigDelta();

      // Shorthand: 0b is a collection of 3 binary masks.
      // . represents the middle spot (where the next piece will go)
      // 1 represents where current player must have a stone
      // 2 represents where opponent must have a stone
      // X represents where it doesn't matter if there is a stone
      _heuristics.Add(new Tuple<int, int, int, double>(0x6c, 0x0, 0x183, win));  // 0bXX11.11XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x2e, 0x0, 0x1c1, win));  // 0bXXX1.111X
      _heuristics.Add(new Tuple<int, int, int, double>(0xf, 0x0, 0x1e0, win));  // 0bXXXX.1111
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0xf, 0x1e0, win / 5.0));  // 0bXXXX.2222
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0x2e, 0x1c1, win / 5.0));  // 0bXXX2.222X
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0x6c, 0x183, win / 5.0));  // 0bXX22.22XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0xe, 0x1e1, bigDelta + delta));  // 0bXXXX.222X
      _heuristics.Add(new Tuple<int, int, int, double>(0x28, 0x0, 0x183, bigDelta));  // 0bXX01.10XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x2c, 0x0, 0x181, bigDelta));  // 0bXX01.110X
      _heuristics.Add(new Tuple<int, int, int, double>(0xe, 0x0, 0x1e1, bigDelta));  // 0bXXXX.111X
      _heuristics.Add(new Tuple<int, int, int, double>(0xc, 0x2, 0x1e1, bigDelta / 2 - delta));  // 0bXXXX.112X
      _heuristics.Add(new Tuple<int, int, int, double>(0x4, 0x0, 0x1e3, 10 * delta));  // 0bXXXX.01XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x44, 0x0, 0x183, 10 * delta));  // 0bXX10.01XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x5, 0x0, 0x1e0, 10 * delta));  // 0bXXXX.0101
      _heuristics.Add(new Tuple<int, int, int, double>(0x1, 0x0, 0x1e0, 6 * delta));  // 0bXXXX.0001
      _heuristics.Add(new Tuple<int, int, int, double>(0x2, 0x0, 0x1e1, 3 * delta));  // 0bXXXX.001X
      _heuristics.Add(new Tuple<int, int, int, double>(0x8, 0x0, 0x1e7, delta));  // 0bXXXX.1XXX
      _heuristics.Add(new Tuple<int, int, int, double>(0x8, 0x4, 0x1e3, -delta));  // 0bXXXX.12XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x1, 0x0, 0x1ee, delta / 10));  // 0bXXXX.XXX1
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0x1, 0x1ee, delta / 10));  // 0bXXXX.XXX2
      _heuristics.Add(new Tuple<int, int, int, double>(0x2, 0x0, 0x1ed, delta / 10));  // 0bXXXX.XX1X
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0x2, 0x1ed, delta / 10));  // 0bXXXX.XX2X
      _heuristics.Add(new Tuple<int, int, int, double>(0x4, 0x0, 0x1eb, delta / 10));  // 0bXXXX.X1XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0x4, 0x1eb, delta / 10));  // 0bXXXX.X2XX
      _heuristics.Add(new Tuple<int, int, int, double>(0x8, 0x0, 0x1e7, delta / 10));  // 0bXXXX.1XXX
      _heuristics.Add(new Tuple<int, int, int, double>(0x0, 0x8, 0x1e7, delta / 10));  // 0bXXXX.2XXX

      List<Tuple<int, int, int, double> > reversed = new List<Tuple<int, int, int, double> >();

      int reversedPatternWhite, reversedPatternBlack, reversedPatternIgnore;

      Board board = new Board();  // Used to reverence COL_MASKS
      foreach (Tuple<int, int, int, double> regular in _heuristics) {
        reversedPatternWhite = 0;
        reversedPatternBlack = 0;
        reversedPatternIgnore = 0;

        int index = 0;
        for (int i = 8; i >= 0; i--) {
          if ((regular.Item1 & board.COL_MASKS[index]) != 0) {
            reversedPatternWhite |= board.COL_MASKS[index];
          }
          if ((regular.Item2 & board.COL_MASKS[index]) != 0) {
            reversedPatternBlack |= board.COL_MASKS[index];
          }
          if ((regular.Item3 & board.COL_MASKS[index]) != 0) {
            reversedPatternIgnore |= board.COL_MASKS[index];
          }
          index++;
        }

        reversed.Add(new Tuple<int, int, int, double>(reversedPatternWhite, reversedPatternBlack,
                                                      reversedPatternIgnore, regular.Item4));
      }

      _heuristics.AddRange(reversed);

      _captureParams = new Tuple<int, int, int, int>(0x7, 0x1, 0x6, 0x70);  // 0bXXX.221 => 0x1, 0x6, 0x70
    }

    // Order is:
    // patternLength, patternCurrentPlayer, patternOpponentPlayer, patternIgnore, score
    public static List<Tuple<int, int, int, double> > getHeuristics() {
      return _heuristics;
    }

    public static Tuple<int, int, int, int> getCaptureParams() {
      return _captureParams;
    }

    public static double getDelta() {
      return getWin() / 1000.0;
    }

    public static double getWin() {
      return 1.0;
    }

    public static double getBigDelta() {
      return getWin() / 10.0;
    }
  }
}
