using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  static class HeuristicValues {
    private static List<List<Tuple<int, int, int, double> > > _heuristics;
    private static Tuple<int, int, int> _captureCheck;
    public readonly static int explorationLevel = 5;
 
    // The Heuristics are primarily to identify spots to look at so that the branching
    // factor will be kept low.
    static HeuristicValues() {
      _heuristics = new List<List<Tuple<int, int, int, double> > >();
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
      _heuristics[2].Add(new Tuple<int, int, int, double>(0xe, 0x0, 0x1c1, bigDelta));  // 0bXXX0.111X
      _heuristics[2].Add(new Tuple<int, int, int, double>(0xe, 0x0, 0x1e0, bigDelta));  // 0bXXXX.1110
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x2c, 0x0, 0x183, bigDelta));  // 0bXX01.11XX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x2c, 0x0, 0x1c1, bigDelta));  // 0bXXX1.110X
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x7, 0x0, 0x1e0, bigDelta));  // 0bXXXX.0111
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x4c, 0x0, 0x183, bigDelta));  // 0bXX10.11XX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0xa8, 0x0, 0x107, bigDelta));  // 0bX101.1XXX
      _heuristics[2].Add(new Tuple<int, int, int, double>(0x160, 0x0, 0xf, bigDelta));  // 0b1011.XXXX

      // Block an unblocked 3 (Block any win in two) or capture
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0xb, 0x1e0, bigDelta));  // 0bXXXX.2022
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0xd, 0x1e0, bigDelta));  // 0bXXXX.2202
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0x2c, 0x181, bigDelta / 2 + delta));  // 0bXX02.220X
      _heuristics[3].Add(new Tuple<int, int, int, double>(0x0, 0xe, 0x1e0, bigDelta / 2));  // 0bXXXX.2220

      // Set up a capture or set up an unblocked 3, or block a capture
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[4].Add(new Tuple<int, int, int, double>(0xc, 0x2, 0x1e1, bigDelta / 2 - delta));  // 0bXXXX.112X
      _heuristics[4].Add(new Tuple<int, int, int, double>(0x0, 0xc, 0x1e1, bigDelta));  // 0bXXXX.220X
      _heuristics[4].Add(new Tuple<int, int, int, double>(0x28, 0x0, 0x183, bigDelta));  // 0bXX01.10XX
      _heuristics[4].Add(new Tuple<int, int, int, double>(0xc, 0x0, 0x1c1, bigDelta));  // 0bXXX0.110X

      // Proximity checks
      _heuristics.Add(new List<Tuple<int, int, int, double> >());
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x1, 0x0, 0x1ee, delta / 10));  // 0bXXXX.XXX1
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x0, 0x1, 0x1ee, delta / 10));  // 0bXXXX.XXX2
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x2, 0x0, 0x1ed, delta / 10));  // 0bXXXX.XX1X
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x0, 0x2, 0x1ed, delta / 10));  // 0bXXXX.XX2X
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x4, 0x0, 0x1eb, delta / 10));  // 0bXXXX.X1XX
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x0, 0x4, 0x1eb, delta / 10));  // 0bXXXX.X2XX
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x8, 0x0, 0x1e7, delta / 9));  // 0bXXXX.1XXX
      _heuristics[5].Add(new Tuple<int, int, int, double>(0x0, 0x8, 0x1e7, delta / 9));  // 0bXXXX.2XXX

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
    }

    // Order is:
    // patternLength, patternCurrentPlayer, patternOpponentPlayer, patternIgnore, score
    public static List<List<Tuple<int, int, int, double> > > getHeuristics() {
      return _heuristics;
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

    // This does not evaluate captures
    // Return is: <exptectedWinner, uncertainty>
    public static Tuple<player_t, double> estimateQuality(int ruleLevel, player_t currentPlayer) {
      player_t otherPlayer = (currentPlayer == player_t.white) ? player_t.black : player_t.white;

      if (ruleLevel == 0) {
        return Tuple.Create(currentPlayer, 0.0);
      } else if (ruleLevel == 1) {
        // The only reason this would be evaluated is if the lookahead branch took some appearantly poor choices.
        // Since the rules don't distinguish between a blockable four and an unblockable four, we can't say all that much
        return Tuple.Create(otherPlayer, 0.5);
      } else if (ruleLevel == 2) {
        return Tuple.Create(currentPlayer, 0.3);
      } else if (ruleLevel == 3) {
        return Tuple.Create(otherPlayer, 0.7);
      } else if (ruleLevel == 4) {
        return Tuple.Create(currentPlayer, 0.9);
      } else {
        // Triggers off proximity checks. If this is the quality, then we can't say much of anything, except that
        // white likely has an advantage.
        return Tuple.Create(player_t.white, 1.0);
      }
    }

    public static Tuple<player_t, double> estimateQualityOfCapture(player_t currentPlayer, int capturesWhite, int capturesBlack) {
      player_t otherPlayer = (currentPlayer == player_t.white) ? player_t.black : player_t.white;
      int capturesCurrent, capturesOther;
      if (currentPlayer == player_t.white) {
        capturesCurrent = capturesWhite;
        capturesOther = capturesBlack;
      } else {
        capturesCurrent = capturesBlack;
        capturesOther = capturesWhite;
      }

      if (capturesCurrent >= capturesOther) {
        // E.g., if 4 captures, uncertainty is 0.0. If 3 captures, uncertainty is 0.2
        return Tuple.Create(currentPlayer, 1.0 - 0.2 * (capturesCurrent + 1));
      } else {
        return Tuple.Create(otherPlayer, 1.0 - 0.2 * (capturesOther));
      }
    }
  }
}
