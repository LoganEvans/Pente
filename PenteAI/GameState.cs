using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PenteInterfaces;

namespace PenteAI {
  public class GameState : Board {
    private int[] mInfluenceMap;
    private static int mBaseDepth;
    private static int mPositionsEvaluated = 0;

    private readonly int[] branchingCategories = {3, 10, 50, ROWS * COLS};

    public GameState(BoardInterface board) : base(board) {
      mInfluenceMap = new int[ROWS];
      InitializeMaps();
    }

    public GameState(GameState copyFrom) : base(copyFrom) {
      CopyMaps(copyFrom);
    }

    public GameState(Player nextPlayer, int capturesWhite, int capturesBlack, String boardStr) : base(nextPlayer, capturesWhite, capturesBlack, boardStr) {
      mInfluenceMap = new int[ROWS];
      InitializeMaps();
    }

    private void CopyMaps(GameState copyFrom) {
      mInfluenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        mInfluenceMap[row_dex] = copyFrom.mInfluenceMap[row_dex];
      }
    }

    // This method triggers the Negamax search. It should only be called externally.
    public Tuple<int, int> GetBestMove(int depthLimit) {
      mBaseDepth = GetMoveNumber();
      Tuple<int, int> move;
      Heuristic heuristic = Minimax(depthLimit, null, null, out move);
      //Tuple<Tuple<int, int>, Heuristic> move = Negamax();
      return move;
    }

    Heuristic Minimax(int depthLimit, Heuristic heuristicAlpha, Heuristic heuristicBeta, out Tuple<int, int> bestMove) {
      mPositionsEvaluated++;
      Heuristic champHeur = null;
      Heuristic chumpHeur;
      GameState child = null;
      Tuple<int, int> move;
      bestMove = null;

      if (GetWinner() != Player.Neither) {
        return Heuristic.GetWinHeuristic(IsMaxLevel());
      }

      if (IsMaxLevel()) {
        foreach (Tuple<int, int> candidateMove in GetCandidateMoves()) {
          child = new GameState(this);
          child.Move(candidateMove.Item1, candidateMove.Item2);
          if (depthLimit > 0) {
            chumpHeur = child.Minimax(depthLimit - 1, champHeur, heuristicBeta, out move);
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
          child = new GameState(this);
          child.Move(candidateMove.Item1, candidateMove.Item2);
          if (depthLimit > 0) {
            chumpHeur = child.Minimax(depthLimit - 1, heuristicAlpha, champHeur, out move);
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

    private List<Tuple<int, int>> GetCandidateMoves() {
      Tuple<int, int> spot;
      List<Tuple<int, int>> candidates = new List<Tuple<int, int>>();
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (!IsLegal(row_dex, col_dex)) {
            continue;
          }

          if (IsOnMap(row_dex, col_dex, mInfluenceMap)) {
            spot = Tuple.Create(row_dex, col_dex);
            candidates.Add(spot);
          }
        }
      }

      if (candidates.Count == 0) {
        if (GetMoveNumber() == 1) {
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
      mInfluenceMap = new int[ROWS];

      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          UpdateMaps(row_dex, col_dex);
        }
      }
    }

    private void UpdateMaps(int row, int col) {
      if (IsOnMap(row, col, mInfluenceMap)) {
        return;
      }

      Heuristic heurVal;
      heurVal = GetHeuristicValue(row, col);
      if (heurVal.GetPriority() < Heuristic.PROXIMITY_PRIORITY) {
        mInfluenceMap[row] |= SPOT_MASKS[col];
      }
    }

    public override bool Move(int row, int col) {
      bool retval = base.Move(row, col);
      MoveTriggeredMapsUpdate(row, col);
      return retval;
    }

    public void MoveTriggeredMapsUpdate(int row, int col) {
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
            UpdateMaps(inspectedRow, inspectedCol);
          }
        }
      }
    }

    // TODO, this doesn't check for captures.
    public Heuristic GetHeuristicValue(int row, int col) {
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
  }
}
