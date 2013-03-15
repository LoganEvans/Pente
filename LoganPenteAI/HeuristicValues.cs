using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoganPenteAI {
  static class HeuristicValues {
    private static List<List<Tuple<int, int, int, double> > > _heuristics;
    private static Tuple<int, int, int> _captureCheck;
    private static List<Tuple<int, int, int, double> > _proximityChecks;
 
    // The Heuristics are primarily to identify spots to look at so that the branching
    // factor will be kept low.
    static HeuristicValues() {
      _heuristics = new List<List<Tuple<int, int, int, double> > >();
      _proximityChecks = new List<Tuple<int, int, int, double> >();
      double win = getWin();
      double delta = getDelta();
      double bigDelta = getBigDelta();

      // Shorthand: 0b is a collection of 3 binary masks.
      // . represents the middle spot (where the next piece will go)
      // 1 represents where current player must have a stone
      // 2 represents where opponent must have a stone
      // X represents where it doesn't matter if there is a stone

      // The level (index) of _heuristics indicates how important a move is. In general,
      // if this is a winning move, it's level 0. If it blocks a winning move, it's 1.
      // If it is a proactive move, it is an even level, if it's a defensive move, it's odd level.
      // So, an even number N suggests than the active player can win on the Nth move.
      // An odd number M suggests that the active player thwarts a win on the Mth move.
      // The value of these possitions is used to distinguish between rules at the same level.
      // When searching for a win, all move up to and including level 2 should be considered.

      // Wins
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[0].Add(new Tuple<int, int, int, double>(0x6c, 0x0, 0x183, win));  // 0bXX11.11XX
      _heuristics[0].Add(new Tuple<int, int, int, double>(0x2e, 0x0, 0x1c1, win));  // 0bXXX1.111X
      _heuristics[0].Add(new Tuple<int, int, int, double>(0xf, 0x0, 0x1e0, win));  // 0bXXXX.1111

      // Block wins
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[1].Add(new Tuple<int, int, int, double>(0x0, 0xf, 0x1e0, win / 5.0));  // 0bXXXX.2222
      _heuristics[1].Add(new Tuple<int, int, int, double>(0x0, 0x2e, 0x1c1, win / 5.0));  // 0bXXX2.222X
      _heuristics[1].Add(new Tuple<int, int, int, double>(0x0, 0x6c, 0x183, win / 5.0));  // 0bXX22.22XX

      // Create a four (could win in one)
      // Note: An unblocked 3 is also listen on this level because it generally needs to be addressed
      // immediately.
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[2].Add(new Tuple<int, int, int, double>(0xe, 0x0, 0x1e1, bigDelta));  // 0bXXXX.111X
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x2c, 0x0, 0x1c3, bigDelta));  // 0bXXX1.11XX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x7, 0x0, 0x1e0, bigDelta));  // 0bXXXX.0111
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x4c, 0x0, 0x183, bigDelta));  // 0bXX10.11XX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0xa8, 0x0, 0x107, bigDelta));  // 0bX101.1XXX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x160, 0x0, 0xf, bigDelta));  // 0b1011.XXXX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x28, 0x0, 0x183, bigDelta));  // 0bXX01.10XX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0xc, 0x0, 0x1c1, bigDelta));  // 0bXXX0.110X

      // Block an unblocked 3 (Block any win in two) or capture, or set up a capture
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0x2c, 0x181, bigDelta / 2 + delta));  // 0bXX02.220X
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0xe, 0x1e0, bigDelta / 2));  // 0bXXXX.2220
      _heuristics[3].Add(new Tuple<int, int, int, double>(0xc, 0x2, 0x1e1, bigDelta / 2 - delta));  // 0bXXXX.112X
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0xc, 0x1e1, bigDelta));  // 0bXXXX.220X

      // Lower level patterns, such as a split 3, split 4, or split 5, should be investigated due to the proximity
      // checks. 

      List<Tuple<int, int, int, double> > reversed = new List<Tuple<int, int, int, double> >();
      int reversedPatternWhite, reversedPatternBlack, reversedPatternIgnore;

      Board board = new Board();  // Used to reference COL_MASKS
      for (int i = 0; i < _heuristics.Count; i++) {
        foreach (Tuple<int, int, int, double> regular in _heuristics[i]) {
          reversedPatternWhite = 0;
          reversedPatternBlack = 0;
          reversedPatternIgnore = 0;

          int index = 0;
          for (int bit_dex = 8; bit_dex >= 0; bit_dex--) {
            if ((regular.Item1 & board.COL_MASKS[index]) != 0) {
              reversedPatternWhite |= board.COL_MASKS[bit_dex];
            }
            if ((regular.Item2 & board.COL_MASKS[index]) != 0) {
              reversedPatternBlack |= board.COL_MASKS[bit_dex];
            }
            if ((regular.Item3 & board.COL_MASKS[index]) != 0) {
              reversedPatternIgnore |= board.COL_MASKS[bit_dex];
            }
            index++;
          }

          reversed.Add(new Tuple<int, int, int, double>(reversedPatternWhite, reversedPatternBlack,
                                                        reversedPatternIgnore, regular.Item4));
        }

        _heuristics[i].AddRange(reversed);
      }

      _captureCheck = new Tuple<int, int, int>(0x2, 0xc, 0x1e1);  // 0bXXXX.221X

      // Another stone (of either type) is in the vacinity
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x1, 0x0, 0x1ee, delta / 10));  // 0bXXXX.XXX1
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x0, 0x1, 0x1ee, delta / 10));  // 0bXXXX.XXX2
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x2, 0x0, 0x1ed, delta / 10));  // 0bXXXX.XX1X
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x0, 0x2, 0x1ed, delta / 10));  // 0bXXXX.XX2X
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x4, 0x0, 0x1eb, delta / 10));  // 0bXXXX.X1XX
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x0, 0x4, 0x1eb, delta / 10));  // 0bXXXX.X2XX
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x8, 0x0, 0x1e7, delta / 9));  // 0bXXXX.1XXX
      _proximityChecks.Add(new Tuple<int, int, int, double>(0x0, 0x8, 0x1e7, delta / 9));  // 0bXXXX.2XXX
    }

    // Order is:
    // patternLength, patternCurrentPlayer, patternOpponentPlayer, patternIgnore, score
    public static List<List<Tuple<int, int, int, double> > > getHeuristics() {
      return _heuristics;
    }

    // The common case is that a spot isn't anywhere near anything interesting. The proximityChecks tests
    // will identify that this stone can "see" something else within a distance of 5.
    public static List<Tuple<int, int, int, double> > getProximityChecks() {
      return _proximityChecks;
    }

    public static Tuple<int, int, int> getCaptureCheck() {
      return _captureCheck;
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
