using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class GameState : Board {
    private int[] _influenceMap;
    private static int _baseDepth;
    public double[][] _topLevelHeuristics;

    protected int _pliesEvaluated;

    public static Random staticRand = null;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    public struct _snapInfluenceEntry {
      public int row;
      public int col;
    }

    protected _snapInfluenceEntry[] _snapInfluenceLog;
    protected int _snapInfluenceIdx;
    protected const int _SNAP_INFLUENCE_LOG_LENGTH = ROWS * COLS;

    public static void InitGameState() {
      if (staticRand == null) {
        staticRand = new Random();
      }
    }

    public GameState(BoardInterface board) : base(board) {
      _pliesEvaluated = 0;
      _influenceMap = new int[ROWS];

//    _snapInfluenceLog = new _snapInfluenceEntry[_SNAP_INFLUENCE_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_INFLUENCE_LOG_LENGTH; i++) {
//      _snapInfluenceLog[i] = new _snapInfluenceEntry();
//    }

      InitializeMaps();
//    _snapInfluenceIdx = 0;
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      CopyMaps(copyFrom);
      _pliesEvaluated = copyFrom._pliesEvaluated;
//    _snapInfluenceLog = new _snapInfluenceEntry[_SNAP_INFLUENCE_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_INFLUENCE_LOG_LENGTH; i++) {
//      _snapInfluenceLog[i] = copyFrom._snapInfluenceLog[i];
//    }

      InitializeMaps();
//    _snapInfluenceIdx = 0;
    }

    public GameState(Player nextPlayer, int capturesWhite, int capturesBlack, String boardStr)
        : base(nextPlayer, capturesWhite, capturesBlack, boardStr) {
      _pliesEvaluated = 0;
      _influenceMap = new int[ROWS];
//    _snapInfluenceLog = new _snapInfluenceEntry[_SNAP_INFLUENCE_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_INFLUENCE_LOG_LENGTH; i++) {
//      _snapInfluenceLog[i] = new _snapInfluenceEntry();
//    }
      InitializeMaps();
//    _snapInfluenceIdx = 0;
    }

    protected void CopyMaps(GameState copyFrom) {
      _influenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        _influenceMap[row_dex] = copyFrom._influenceMap[row_dex];
      }
    }

    public virtual Tuple<int, int> GetBestMove(int depthLimit) {
      _baseDepth = GetPlyNumber();
      Tuple<int, int> move;
   // Heuristic heuristic = Minimax(depthLimit, null, null, out move);
      _topLevelHeuristics = new double[ROWS][];
      for (int row_idx = 0; row_idx < ROWS; row_idx++) {
        _topLevelHeuristics[row_idx] = new double[COLS];
      }
        
      double heur = Negamax(depthLimit, -9001, 9001, 1, out move, topOfSearchTree: true);

      Console.WriteLine("==========");
      Console.WriteLine(GetHeuristicStr(_topLevelHeuristics));
      Console.WriteLine("==========");
      Console.WriteLine("plies evaluated: {0}", GetPliesEvaluated());
      return move;
    }

    protected virtual double Negamax(int depthLimit, double alpha, double beta, int color, out Tuple<int, int> bestMove,
                                     bool topOfSearchTree=false) {
      double value = 0.0;
      Tuple<int, int> move = null;
      bestMove = move;

      if (depthLimit == 0 || GetWinner() != Player.Neither) {
        // Should this iterate over the board instead of the candidate moves?
        _pliesEvaluated++;
        value = GetAdjHeuristicValue();

        // XXX hack
        while (!this.IsLegal(bestMove)) {
          bestMove = Tuple.Create(GameState.staticRand.Next(Board.ROWS), GameState.staticRand.Next(Board.COLS));
        }

        return color * value;
      }

      foreach (Tuple<int, int> candidateMove in GetCandidateMoves()) {
        _pliesEvaluated++;
        GameState child = new GameState(this);
        child.Move(candidateMove);
        value = -child.Negamax(depthLimit - 1, -beta, -alpha, -color, out move);

        if (topOfSearchTree) {
          _topLevelHeuristics[candidateMove.Item1][candidateMove.Item2] = value;
        }

        if (!this.IsLegal(bestMove)) {
          bestMove = candidateMove;
        }

     // Tuple<Tuple<int, int, int, int>, int> snapshotData = GetSnapshotData();
     // Move(candidateMove);
     // value = -Negamax(depthLimit - 1, -beta, -alpha, -color);
     // Rollback(snapshotData);

        if (value >= beta) {
          bestMove = candidateMove;
          return value;
        }

        if (value > alpha) {
          bestMove = candidateMove;
          alpha = value;
        }
      }

      return alpha;
    }

    protected virtual List<Tuple<int, int>> GetCandidateMoves() {
    //for (int row_idx = 0; row_idx < ROWS; row_idx++) {
    //  for (int col_idx = 1; col_idx < (1 << COLS); col_idx <<= 1) {
    //    if ((_influenceMap[row_idx] & col_idx) != 0) {
    //      Console.Write("  *");
    //    } else {
    //      Console.Write("  .");
    //    }
    //  }
    //  Console.WriteLine();
    //}
    //Console.WriteLine();

      Tuple<int, int> spot;
      List<Tuple<int, int>> candidates = new List<Tuple<int, int>>();
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          // Trim illegal
          if (!IsLegal(row_dex, col_dex)) {
            continue;
          }

          // Use the influence map
          if (IsOnMap(row_dex, col_dex, _influenceMap)) {
            spot = Tuple.Create(row_dex, col_dex);
            candidates.Add(spot);
          }
        }
      }

      // If the influence map was empty...
      if (candidates.Count == 0) {
        if (GetPlyNumber() == 0) {
          // White's first move.
          candidates.Add(Tuple.Create(ROWS / 2, COLS / 2));
        } else if (GetPlyNumber() == 2) {
          // White's second move.
          for (int row_dex = 0; row_dex < ROWS; row_dex++) {
            for (int col_dex = 0; col_dex < COLS; col_dex++) {
              if (Math.Abs(row_dex - 9) <= 3 && Math.Abs(col_dex - 9) <= 3) {
                if (!IsLegal(row_dex, col_dex)) {
                  continue;
                }
                candidates.Add(Tuple.Create(row_dex, col_dex));
              }
            }
          }
        } else {
          for (int row_dex = 0; row_dex < ROWS; row_dex++) {
            for (int col_dex = 0; col_dex < COLS; col_dex++) {
              if (!IsLegal(row_dex, col_dex)) {
                continue;
              }

              if (GetHeuristicValue(row_dex, col_dex).GetPriority() <= Heuristic.PROXIMITY_PRIORITY) {
                spot = Tuple.Create(row_dex, col_dex);
                candidates.Add(spot);
              }
            }
          }
        }
      }

      //Console.WriteLine("Candidates: {0} Depth: {1}", candidates.Count, GetPlyNumber());
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
      _influenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          UpdateMaps(row_dex, col_dex);
        }
      }
    }

    private void UpdateMaps(int row, int col) {
      if (IsOnMap(row, col, _influenceMap)) {
        return;
      }

      Heuristic heurVal;
      heurVal = GetHeuristicValue(row, col);

      if (heurVal.GetPriority() <= Heuristic.PROXIMITY_PRIORITY) {
        _influenceMap[row] |= SPOT_MASKS[col];
      }
    }

    public override bool Move(int row, int col) {
      bool retval = base.Move(row, col);
      MoveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void MoveTriggeredMapsUpdate(int row, int col) {
      foreach (Tuple<int, int> direction in _directionIncrements) {
        int inspectedRow;
        int inspectedCol;

        for (int i = -4; i <= 4; i++) {
          inspectedRow = row + (direction.Item1 * i);
          inspectedCol = col + (direction.Item2 * i);

          if (0 <= inspectedRow && inspectedRow < ROWS && 0 <= inspectedCol && inspectedCol < COLS) {
            UpdateMaps(inspectedRow, inspectedCol);
          }
        }
      }
    }

    public virtual double GetAdjHeuristicValue() {
      int acc;
      int bitsSet;
      int majiq;
      int score = 0;
      int toAdd;

      for (int inARow = 2; inARow < 5; inARow++) {
        foreach (int board_dex in WhiteBoardRep) {
          foreach (int line in _boardRepresentations[board_dex]) {
            acc = line;
            for (int shift_dex = 1; shift_dex <= inARow; shift_dex++) {
              acc = acc & (acc << shift_dex);
            }

            if (acc != 0) {
              Console.WriteLine("acc: 0x{0} line: 0x{1}", acc.ToString("X"), line.ToString("X"));
            }

            bitsSet = 0; // c accumulates the total bits set in v
            while (acc != 0) {
              bitsSet++;
              acc &= acc - 1;
            }
            toAdd = inARow * bitsSet;

            // This counts the number of set bits.
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
       //   majiq = acc - ((acc >> 1) & 0x55555555);
       //   majiq = (majiq & 0x33333333) + ((majiq >> 2) & 0x33333333);
       //   toAdd = inARow * (((majiq + (majiq >> 4) & 0xF0F0F0F) * 0x1010101) >> 24);
            if (toAdd > 0) {
              Console.WriteLine(GetBoardStateStr());
              Console.WriteLine("toAdd: {0} line: 0x{1}", toAdd, line.ToString("X"));
              Console.ReadLine();
            }
            score += toAdd;
          }
        }

        foreach (int board_dex in BlackBoardRep) {
          foreach (int line in _boardRepresentations[board_dex]) {
            acc = line;
            for (int shift_dex = 1; shift_dex <= inARow; shift_dex++) {
              acc = acc & (acc << shift_dex);
            }

            if (acc != 0) {
              // This counts the number of set bits.
              // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
              majiq = acc - ((acc >> 1) & 0x55555555);
              majiq = (majiq & 0x33333333) + ((majiq >> 2) & 0x33333333);
              score -= inARow * (((majiq + (majiq >> 4) & 0xF0F0F0F) * 0x1010101) >> 24);
            }
          }
        }
      }

      return (double)score;
    }

    // TODO, this doesn't check for captures.
    public virtual Heuristic GetHeuristicValue(int row, int col) {
      if (!IsLegal(row, col)) {
        return new Heuristic(0.0, Heuristic.PROXIMITY_PRIORITY + 1);
      }

      Dictionary<Pattern, Heuristic> hDict = HeuristicValues.GetHeuristicDict();
      Heuristic val;
      Pattern pattern;

      foreach (Tuple<int, int> window in GetWindows(row, col)) {
        if (IsMaxLevel()) {
          pattern = new Pattern(window.Item1, window.Item2);
          if (hDict.TryGetValue(pattern, out val)) {
            return val;
          }
        } else {
          pattern = new Pattern(window.Item2, window.Item1);
          if (hDict.TryGetValue(pattern, out val)) {
            return val;
          }
        }
      }

      return new Heuristic(0.0, Heuristic.PROXIMITY_PRIORITY + 1);
    }

    private bool IsMaxLevel() {
      if (GetCurrentPlayer() == Player.White) {
        return true;
      } else {
        return false;
      }
    }

    public Pattern WindowToPattern(Player color, Tuple<int, int> window) {
      Pattern retval;
      if (color == Player.White) {
        retval = new Pattern(window.Item1, window.Item2);
      } else {
        retval = new Pattern(window.Item2, window.Item1);
      }
      return retval;
    }

    public int GetPliesEvaluated() {
      return _pliesEvaluated;
    }

    public virtual Tuple<Tuple<int, int, int, int>, int> GetSnapshotData() {
      return Tuple.Create(base.GetSnapshotData(), _snapInfluenceIdx);
    }

    protected virtual void Rollback(Tuple<Tuple<int, int, int, int>, int> snapshotData) {
      base.Rollback(snapshotData.Item1);

      // _snapInfluenceIdx is pointing at the next log location, but nothing has been written there.
      for (; _snapInfluenceIdx > snapshotData.Item2; _snapInfluenceIdx--) {
        _influenceMap[_snapInfluenceLog[_snapInfluenceIdx].row] &= ~SPOT_MASKS[_snapInfluenceLog[_snapInfluenceIdx].row];
      }
    }

    public string GetHeuristicStr(double[][] heuristics) {
      StringBuilder sb = new StringBuilder();
      sb.Append(String.Format("turn: {0} white: {1} black: {2} winner: {3}",
                GetPlyNumber(), GetCaptures(Player.White), GetCaptures(Player.Black), GetWinner()));
      sb.Append(Environment.NewLine);
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (GetSpot(row_dex, col_dex) == Player.White) {
            sb.Append("  W");
          } else if (GetSpot(row_dex, col_dex) == Player.Black) {
            sb.Append("  B");
          } else {
            sb.Append(String.Format("{0,3}", heuristics[row_dex][col_dex]));
          }
        }
        sb.Append(" | ");
        sb.Append(row_dex.ToString());
        sb.Append(Environment.NewLine);
      }
      for (int col_dex = 0; col_dex < COLS; col_dex++) {
        sb.Append(String.Format("{0,3}", col_dex));
      }
      sb.Append(Environment.NewLine);
      return sb.ToString();
    }

  }
}
