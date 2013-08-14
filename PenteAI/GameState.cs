using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class GameState : Board {
    private int[] _influenceMap;
    private static int _baseDepth;

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
      double heur = Negamax(depthLimit, -9001, 9001, 1, out move);
      return move;
    }

    protected virtual double Negamax(int depthLimit, double alpha, double beta, int color, out Tuple<int, int> bestMove) {
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

    protected virtual Heuristic Minimax(int depthLimit, Heuristic heuristicAlpha, Heuristic heuristicBeta, out Tuple<int, int> bestMove) {
      Heuristic champHeur = null;
      Heuristic chumpHeur;
      bestMove = null;

      if (GetWinner() != Player.Neither) {
        return Heuristic.GetWinHeuristic(IsMaxLevel());
      }
      bool maxLevel = IsMaxLevel();

      if (IsMaxLevel()) {
        foreach (Tuple<int, int> candidateMove in GetCandidateMoves()) {
          _pliesEvaluated++;
          if (depthLimit > 0) {
            chumpHeur = GetHeuristicForMove(move:candidateMove, depthLimit:depthLimit,
                                            alpha:champHeur, beta:heuristicBeta);
          } else {
            chumpHeur = GetHeuristicValue(candidateMove.Item1, candidateMove.Item2);
          }

          // Max level, so pick max.
          if (champHeur == null || champHeur < chumpHeur) {
            champHeur = chumpHeur;
            bestMove = candidateMove;
            if (heuristicBeta != null && champHeur > heuristicBeta) {
              // Beta prune.
              break;
            }
          }
        }
      } else {
        foreach (Tuple<int, int> candidateMove in GetCandidateMoves()) {
          _pliesEvaluated++;
          if (depthLimit > 0) {
            chumpHeur = GetHeuristicForMove(move: candidateMove, depthLimit: depthLimit,
                                            alpha: heuristicAlpha, beta: champHeur);
          } else {
            chumpHeur = GetHeuristicValue(candidateMove.Item1, candidateMove.Item2);
          }

          // Min level, so pick min.
          if (champHeur == null || champHeur > chumpHeur) {
            champHeur = chumpHeur;
            bestMove = candidateMove;
            if (heuristicAlpha != null && champHeur < heuristicAlpha) {
              // Beta prune.
              break;
            }
          }
        }
      }
      champHeur.AddValue(GetHeuristicValue(bestMove.Item1, bestMove.Item2).GetValue());
      return champHeur;
    }

    // This is split out from the rest of Minimax so that GameStateBenchmark can override it
    // and make sure that the child is still a GameStateHeuristic object.
    protected virtual Heuristic GetHeuristicForMove(Tuple<int, int> move, int depthLimit, Heuristic alpha, Heuristic beta) {
      GameState child = new GameState(this);
      child.Move(move);
      Heuristic retval = child.Minimax(depthLimit - 1, alpha, beta, out move);
      _pliesEvaluated += child._pliesEvaluated;
      return retval;
    }

    protected virtual List<Tuple<int, int>> GetCandidateMoves() {
      Tuple<int, int> spot;
      List<Tuple<int, int>> candidates = new List<Tuple<int, int>>();
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (!IsLegal(row_dex, col_dex)) {
            continue;
          }

          if (IsOnMap(row_dex, col_dex, _influenceMap)) {
            spot = Tuple.Create(row_dex, col_dex);
            candidates.Add(spot);
          }
        }
      }

      if (candidates.Count == 0) {
        if (GetPlyNumber() == 0) {
          candidates.Add(Tuple.Create(ROWS / 2, COLS / 2));
        }  else if (GetPlyNumber() == 1) {
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

      Console.WriteLine("This: {0}", candidates.Count);
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
          UpdateMaps(row_dex, col_dex, disableSnap: true);
        }
      }
    }

    private void UpdateMaps(int row, int col, bool disableSnap = false) {
      if (IsOnMap(row, col, _influenceMap)) {
        return;
      }

      Heuristic heurVal;
      heurVal = GetHeuristicValue(row, col);

      if (heurVal.GetPriority() <= Heuristic.PROXIMITY_PRIORITY) {
//      _snapInfluenceLog[_snapInfluenceIdx].row = row;
//      _snapInfluenceLog[_snapInfluenceIdx].col = col;
//      _influenceMap[row] |= SPOT_MASKS[col];
//      if (!disableSnap) {
//        _snapInfluenceIdx++;
//      }
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
      int majiq;
      int score = 0;

      for (int inARow = 2; inARow < 5; inARow++) {
        foreach (int board_dex in WhiteBoardRep) {
          foreach (int line in _boardRepresentations[board_dex]) {
            acc = line;
            for (int shift_dex = 1; shift_dex < inARow; shift_dex++) {
              acc <<= shift_dex;
            }

            // This counts the number of set bits.
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            majiq = acc - ((acc >> 1) & 0x55555555);
            majiq = (majiq & 0x33333333) + ((majiq >> 2) & 0x33333333);
            score += inARow * (((majiq + (majiq >> 4) & 0xF0F0F0F) * 0x1010101) >> 24);
          }
        }

        foreach (int board_dex in BlackBoardRep) {
          foreach (int line in _boardRepresentations[board_dex]) {
            acc = line;
            for (int shift_dex = 1; shift_dex < inARow; shift_dex++) {
              acc <<= shift_dex;
            }

            // This counts the number of set bits.
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            majiq = acc - ((acc >> 1) & 0x55555555);
            majiq = (majiq & 0x33333333) + ((majiq >> 2) & 0x33333333);
            score -= inARow * (((majiq + (majiq >> 4) & 0xF0F0F0F) * 0x1010101) >> 24);
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
  }
}
