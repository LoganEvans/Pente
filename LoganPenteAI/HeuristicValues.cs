﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public struct Pattern {
    public const int PATTERN_MASK = 0x1FF;
    public const int PATTERN_RADIUS = 4;
    public const int PATTERN_DIAMETER = 9;
    public const int ROW_PATTERN = 0;
    public const int COL_PATTERN = 1;
    public const int UP_DIAG_PATTERN = 2;
    public const int DOWN_DIAG_PATTERN = 3;

    private int mPattern;

    public void SetPattern(int patternWhite, int patternBlack) {
      mPattern = patternWhite + (patternBlack << Pattern.PATTERN_DIAMETER);
    }

    public void SetPattern(string fromString) {
      mPattern = Int32.Parse(fromString);
    }

    public override int GetHashCode() {
      return mPattern;
    }

    public int GetPatternCurrent() {
      return mPattern & PATTERN_MASK;
    }

    public int GetPatternOther() {
      return (mPattern >> PATTERN_DIAMETER) & PATTERN_MASK;
    }

    public static List<Pattern> GetAllMatchingPatterns(Tuple<int, int, int> patternTrio) {
      List<Pattern> retval = new List<Pattern>();
      Pattern toAdd;
      int filter = patternTrio.Item3 | (patternTrio.Item3 << PATTERN_DIAMETER);

      for (int i = 0; i < Math.Pow(2, 2 * PATTERN_DIAMETER); i++) {
        if ((i | filter) == filter) {
          toAdd = new Pattern();
          toAdd.SetPattern(patternTrio.Item1, patternTrio.Item2);
          toAdd.mPattern |= i;  // Augments the two patterns.
          retval.Add(toAdd);
        }
      }

      return retval;
    }

    public override string ToString() {
      return mPattern.ToString();
    }
  }

  public static class HeuristicValues {
    private static List<Tuple<Tuple<int, int, int>, Tuple<double, int>>> _heuristics = null;
    private static Dictionary<Pattern, Tuple<double, int>> _hDict = null;
    private static Tuple<int, int, int> _captureCheck;
    private static double[] mWin;
    private static int mProximityPriority;
    public static readonly Tuple<int, int, int> BACKWARD_CAPTURE_PATTERN = Tuple.Create(0x12, 0xc, 0x1e1);
    public static readonly Tuple<int, int, int> FORWARD_CAPTURE_PATTERN = Tuple.Create(0x90, 0x60, 0x10f);

    // The Heuristics are primarily to identify spots to look at so that the branching
    // factor will be kept low.
    static HeuristicValues() {
      InitializeHeuristics();
      // InitializeDictionary();
    }

    private static void InitializeHeuristics() {
      _heuristics = new List<Tuple<Tuple<int, int, int>, Tuple<double, int>>>();
      double[] mWin = { 1.0, 0.8, 0.6, 0.4, 0.2 };
      double bigDelta = mWin[0] / 100.0;
      double delta = bigDelta / 100.0;

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
      _heuristics.Add(Tuple.Create(Tuple.Create(0x6c, 0x0, 0x183), Tuple.Create(mWin[0], 0)));  // 0bXX11.11XX
      _heuristics.Add(Tuple.Create(Tuple.Create(0x2e, 0x0, 0x1c1), Tuple.Create(mWin[0], 0)));  // 0bXXX1.111X
      _heuristics.Add(Tuple.Create(Tuple.Create(0xf, 0x0, 0x1e0), Tuple.Create(mWin[0], 0)));  // 0bXXXX.1111

      // Block wins... generally a bad scenario
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0xf, 0x1e0), Tuple.Create(-(mWin[1] + delta), 1)));  // 0bXXXX.2222
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x2e, 0x1c1), Tuple.Create(-(mWin[1] + delta), 1)));  // 0bXXX2.222X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x6c, 0x183), Tuple.Create(-(mWin[1] + delta), 1)));  // 0bXX22.22XX

      // Create a four (could win in one)... generally a good scenario
      // Note: An unblocked 3 is also listen on this level because it generally needs to be addressed
      // immediately.
      _heuristics.Add(Tuple.Create(Tuple.Create(0xe, 0x0, 0x1c1), Tuple.Create(mWin[1] + delta, 2)));  // 0bXXX0.111X
      _heuristics.Add(Tuple.Create(Tuple.Create(0xe, 0x0, 0x1e0), Tuple.Create(mWin[1] + delta, 2)));  // 0bXXXX.1110
      _heuristics.Add(Tuple.Create(Tuple.Create(0x2c, 0x0, 0x183), Tuple.Create(mWin[1], 2)));  // 0bXX01.11XX
      _heuristics.Add(Tuple.Create(Tuple.Create(0x2c, 0x0, 0x1c1), Tuple.Create(mWin[1], 2)));  // 0bXXX1.110X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x7, 0x0, 0x1e0), Tuple.Create(mWin[1], 2)));  // 0bXXXX.0111
      _heuristics.Add(Tuple.Create(Tuple.Create(0x4c, 0x0, 0x183), Tuple.Create(mWin[1], 2)));  // 0bXX10.11XX
      _heuristics.Add(Tuple.Create(Tuple.Create(0xa8, 0x0, 0x107), Tuple.Create(mWin[1], 2)));  // 0bX101.1XXX
      _heuristics.Add(Tuple.Create(Tuple.Create(0x160, 0x0, 0xf), Tuple.Create(mWin[1], 2)));  // 0b1011.XXXX

      // Block an unblocked 3 (Block any win in two) or capture... generally a bad (but not terrible) scenario
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0xb, 0x1e0), Tuple.Create(-(mWin[2] - bigDelta), 3)));  // 0bXXXX.2022
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0xd, 0x1e0), Tuple.Create(-(mWin[2] - bigDelta), 3)));  // 0bXXXX.2202
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x2c, 0x181), Tuple.Create(-(mWin[2] - bigDelta), 3)));  // 0bXX02.220X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0xe, 0x1e0), Tuple.Create(-(mWin[2] - bigDelta), 3)));  // 0bXXXX.2220

      // Set up a capture or set up an unblocked 3, or block a capture
      _heuristics.Add(Tuple.Create(Tuple.Create(0xc, 0x2, 0x1e1), Tuple.Create(mWin[2] + bigDelta / 2, 4)));  // 0bXXXX.112X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0xc, 0x1e1), Tuple.Create(mWin[2] + delta, 4)));  // 0bXXXX.220X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x28, 0x0, 0x183), Tuple.Create(mWin[2] + bigDelta / 2 + delta, 4)));  // 0bXX01.10XX
      _heuristics.Add(Tuple.Create(Tuple.Create(0xc, 0x0, 0x1c1), Tuple.Create(mWin[2] + bigDelta / 2 + delta, 4)));  // 0bXXX0.110X

      mProximityPriority = 5;
      // Proximity checks
      _heuristics.Add(Tuple.Create(Tuple.Create(0x1, 0x0, 0x1ee), Tuple.Create(delta / 6, mProximityPriority)));  // 0bXXXX.XXX1
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x1, 0x1ee), Tuple.Create(delta / 10, mProximityPriority)));  // 0bXXXX.XXX2
      _heuristics.Add(Tuple.Create(Tuple.Create(0x2, 0x0, 0x1ed), Tuple.Create(delta / 7, mProximityPriority)));  // 0bXXXX.XX1X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x2, 0x1ed), Tuple.Create(delta / 10, mProximityPriority)));  // 0bXXXX.XX2X
      _heuristics.Add(Tuple.Create(Tuple.Create(0x4, 0x0, 0x1eb), Tuple.Create(delta / 5, mProximityPriority)));  // 0bXXXX.X1XX
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x4, 0x1eb), Tuple.Create(delta / 10, mProximityPriority)));  // 0bXXXX.X2XX
      _heuristics.Add(Tuple.Create(Tuple.Create(0x8, 0x0, 0x1e7), Tuple.Create(delta / 9, mProximityPriority)));  // 0bXXXX.1XXX
      _heuristics.Add(Tuple.Create(Tuple.Create(0x0, 0x8, 0x1e7), Tuple.Create(delta / 9, mProximityPriority)));  // 0bXXXX.2XXX

      AddReversed(_heuristics);

      _captureCheck = new Tuple<int, int, int>(0x2, 0xc, 0x1e1);  // 0bXXXX.221X
    }

    private static void InitializeDictionary() {
      String filename = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"HeuristicValueStore.txt");
      _hDict = new Dictionary<Pattern, Tuple<double, int>>();

      if (!File.Exists(filename) || File.ReadAllLines(filename).Length < 10) {
        Tuple<double, int> conflict;
        foreach (Tuple<Tuple<int, int, int>, Tuple<double, int>> heur in _heuristics) {
          foreach (Pattern pattern in Pattern.GetAllMatchingPatterns(heur.Item1)) {
            if (_hDict.ContainsKey(pattern)) {
              conflict = _hDict[pattern];
              if (conflict.Item2 > heur.Item2.Item2) {
                _hDict[pattern] = heur.Item2;
              }
            } else {
              _hDict.Add(pattern, heur.Item2);
            }
          }
        }

        StringBuilder sb = new StringBuilder();
        foreach (Pattern pattern in _hDict.Keys) {
          sb.Append(pattern.ToString() + "," + _hDict[pattern].Item1 + "," + _hDict[pattern].Item2);
          sb.Append(Environment.NewLine);
        }

        File.WriteAllText(filename, sb.ToString());
      } else {
        Regex rx = new Regex(@"(?<pattern>[^,]*),(?<heuristic>[^,]*),(?<priority>.*)$", RegexOptions.Compiled);

        foreach (String line in File.ReadAllLines(filename)) {
          GroupCollection groups = rx.Match(line).Groups;
          if (groups.Count == 4) {
            Pattern key = new Pattern();
            key.SetPattern(groups["pattern"].Value);
            _hDict[key] = Tuple.Create(Double.Parse(groups["heuristic"].Value), Int32.Parse(groups["priority"].Value));
          }
        }
      }
    }

    // Order is:
    // patternLength, patternCurrentPlayer, patternOpponentPlayer, patternIgnore, score
    private static List<Tuple<Tuple<int, int, int>, Tuple<double, int>>> GetHeuristics() {
      return _heuristics;
    }

    public static Dictionary<Pattern, Tuple<double, int>> GetHeuristicDict() {
      if (_hDict == null) {
        InitializeDictionary();
      }
      return _hDict;
    }

    public static Tuple<int, int, int> GetCaptureCheck() {
      return _captureCheck;
    }

    public static int GetProximityPriority() {
      return mProximityPriority;
    }

    public static Tuple<double, int> EstimateQualityOfCapture(Player currentPlayer, int capturesWhite, int capturesBlack) {
      Player otherPlayer = (currentPlayer == Player.White) ? Player.Black : Player.White;
      int capturesCurrent, capturesOther;
      if (currentPlayer == Player.White) {
        capturesCurrent = capturesWhite;
        capturesOther = capturesBlack;
      } else {
        capturesCurrent = capturesBlack;
        capturesOther = capturesWhite;
      }
      int priority = 5 - Math.Max(capturesCurrent + 1, capturesOther);
      double heuristic = 1.0 - 0.2 * priority;
      if (currentPlayer == Player.Black) {
        heuristic *= -1;
      }
      return Tuple.Create(heuristic, priority);
    }

    private static void AddReversed(List<Tuple<Tuple<int, int, int>, Tuple<double, int>>> inputList) {
      List<Tuple<Tuple<int, int, int>, Tuple<double, int>>> reversed = new List<Tuple<Tuple<int, int, int>, Tuple<double, int>>>();
      int reversedPatternWhite, reversedPatternBlack, reversedPatternIgnore;

      for (int i = 0; i < inputList.Count; i++) {
        foreach (Tuple<Tuple<int, int, int>, Tuple<double, int>> regular in inputList) {
          reversedPatternWhite = 0;
          reversedPatternBlack = 0;
          reversedPatternIgnore = 0;

          int index = 0;
          for (int bit_dex = 8; bit_dex >= 0; bit_dex--) {
            if ((regular.Item1.Item1 & Board.SPOT_MASKS[index]) != 0) {
              reversedPatternWhite |= Board.SPOT_MASKS[bit_dex];
            }
            if ((regular.Item1.Item2 & Board.SPOT_MASKS[index]) != 0) {
              reversedPatternBlack |= Board.SPOT_MASKS[bit_dex];
            }
            if ((regular.Item1.Item3 & Board.SPOT_MASKS[index]) != 0) {
              reversedPatternIgnore |= Board.SPOT_MASKS[bit_dex];
            }
            index++;
          }

          reversed.Add(Tuple.Create(Tuple.Create(reversedPatternWhite, reversedPatternBlack, reversedPatternIgnore), regular.Item2));
        }
      }

      foreach (Tuple<Tuple<int, int, int>, Tuple<double, int>> val in reversed) {
        inputList.Add(val);
      }
    }
  }
}
