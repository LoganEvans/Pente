using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using PenteInterfaces;

namespace PenteAI {
  public class Board : BoardInterface {
    public const int ROWS = 19;
    public const int COLS = 19;
    public const int ROW_MASK = 0x7FFFF;
    private const int DIAGONALS = ROWS + COLS - 1;
    public static readonly int[] SPOT_MASKS = {0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
                                               0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000,
                                               0x4000, 0x8000, 0x10000, 0x20000, 0x40000};

    // Note: It is possible to fix the size of this by using a struct and an unsafe code block. It is possible
    // that doing so would greatly speed up code that deals with the board. Evaluate this.
    private int[] _rowsWhite;
    private int[] _rowsBlack;
    private int[] _colsWhite;
    private int[] _colsBlack;
    private int[] _upDiagWhite;
    private int[] _upDiagBlack;
    private int[] _downDiagWhite;
    private int[] _downDiagBlack;

    private Player _winner;
    private int _plyNumber;
    private int _capturesWhite;
    private int _capturesBlack;

    public const int NUM_DIRECTIONS = 4;
    public enum Direction {ByRow = 0, ByCol = 1, ByUpDiag = 2, ByDownDiag = 3};
    private static Tuple<int, int>[] _directionIncrements = null;
    protected static Tuple<int, int>[] DirectionIncrements { get { return _directionIncrements; } }
    private static Direction[] _allDirections = null;
    protected static Direction[] AllDirections { get { return _allDirections; } }

    // This should be called early on.
    public static void InitBoard() {
      if (_directionIncrements == null) {
        _directionIncrements = new Tuple<int, int>[NUM_DIRECTIONS];
        _directionIncrements[(int)Direction.ByRow] = Tuple.Create(0, 1);
        _directionIncrements[(int)Direction.ByCol] = Tuple.Create(1, 0);
        _directionIncrements[(int)Direction.ByUpDiag] = Tuple.Create(-1, 1);
        _directionIncrements[(int)Direction.ByDownDiag] = Tuple.Create(1, 1);

        _allDirections = new Direction[4];
        _allDirections[(int)Direction.ByRow] = Direction.ByRow;
        _allDirections[(int)Direction.ByCol] = Direction.ByCol;
        _allDirections[(int)Direction.ByUpDiag] = Direction.ByUpDiag;
        _allDirections[(int)Direction.ByDownDiag] = Direction.ByDownDiag;
      }
    }

    public Board() {
      if (DirectionIncrements == null) {
        InitBoard();
      }
      _rowsWhite = new int[ROWS];
      _rowsBlack = new int[ROWS];
      _colsWhite = new int[ROWS];
      _colsBlack = new int[ROWS];
      _upDiagWhite = new int[DIAGONALS];
      _upDiagBlack = new int[DIAGONALS];
      _downDiagWhite = new int[DIAGONALS];
      _downDiagBlack = new int[DIAGONALS];

      _winner = Player.Neither;
      _plyNumber = 0;
      _capturesWhite = 0;
      _capturesBlack = 0;
      Move(ROWS / 2, COLS / 2);
    }

    public Board(BoardInterface copyFrom) {
      if (DirectionIncrements == null) {
        InitBoard();
      }
      _rowsWhite = new int[ROWS];
      _rowsBlack = new int[ROWS];
      _colsWhite = new int[ROWS];
      _colsBlack = new int[ROWS];
      _upDiagWhite = new int[DIAGONALS];
      _upDiagBlack = new int[DIAGONALS];
      _downDiagWhite = new int[DIAGONALS];
      _downDiagBlack = new int[DIAGONALS];
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          SetSpot(row_dex, col_dex, copyFrom.GetSpot(row_dex, col_dex));
        }
      }
      _winner = copyFrom.GetWinner();
      _plyNumber = copyFrom.GetPlyNumber();
      _capturesWhite = copyFrom.GetCaptures(Player.White);
      _capturesBlack = copyFrom.GetCaptures(Player.Black);
    }

    public Board(Board copyFrom) {
      if (DirectionIncrements == null) {
        InitBoard();
      }
      _rowsWhite = new int[ROWS];
      _rowsBlack = new int[ROWS];
      _colsWhite = new int[ROWS];
      _colsBlack = new int[ROWS];
      _upDiagWhite = new int[DIAGONALS];
      _upDiagBlack = new int[DIAGONALS];
      _downDiagWhite = new int[DIAGONALS];
      _downDiagBlack = new int[DIAGONALS];

      for (int i = 0; i < ROWS; i++) {
        _rowsWhite[i] = copyFrom._rowsWhite[i];
        _rowsBlack[i] = copyFrom._rowsBlack[i];
        _colsWhite[i] = copyFrom._colsWhite[i];
        _colsBlack[i] = copyFrom._colsBlack[i];
      }

      for (int i = 0; i < DIAGONALS; i++) {
        _upDiagWhite[i] = copyFrom._upDiagWhite[i];
        _upDiagBlack[i] = copyFrom._upDiagBlack[i];
        _downDiagWhite[i] = copyFrom._downDiagWhite[i];
        _downDiagBlack[i] = copyFrom._downDiagBlack[i];
      }

      _winner = copyFrom._winner;
      _plyNumber = copyFrom._plyNumber;
      _capturesWhite = copyFrom._capturesWhite;
      _capturesBlack = copyFrom._capturesBlack;
    }

    public Board(Player nextPlayer, int capturesWhite, int capturesBlack, String boardStr) {
      if (DirectionIncrements == null) {
        InitBoard();
      }
      Debug.Assert(boardStr.Length == ROWS * COLS);
      _capturesWhite = capturesWhite;
      _capturesBlack = capturesBlack;
      _winner = Player.Neither;
      _plyNumber = 2 * capturesWhite + 2 * capturesBlack;

      _rowsWhite = new int[ROWS];
      _rowsBlack = new int[ROWS];
      _colsWhite = new int[ROWS];
      _colsBlack = new int[ROWS];
      _upDiagWhite = new int[DIAGONALS];
      _upDiagBlack = new int[DIAGONALS];
      _downDiagWhite = new int[DIAGONALS];
      _downDiagBlack = new int[DIAGONALS];

      Player color;
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (boardStr[row_dex * ROWS + col_dex] == 'W') {
            color = Player.White;
            _plyNumber++;
          } else if (boardStr[row_dex * ROWS + col_dex] == 'B') {
            color = Player.Black;
            _plyNumber++;
          } else {
            color = Player.Neither;
          }

          SetSpot(row_dex, col_dex, color);
        }
      }

      if (GetCurrentPlayer() != nextPlayer) {
        _plyNumber++;
      }
    }

    public virtual bool Move(int row, int col) {
      if (!IsLegal(row, col)) {
        return false;
      }
      Player current_player = GetCurrentPlayer();

      SetSpot(row, col, GetCurrentPlayer());

      if (PerformCapturesAndCheckWin(row, col)) {
        _winner = current_player;
      }
      _plyNumber++;

      return true;
    }

    // The intention is that this method is a convenience
    // overload for the other one, but since this isn't
    // virtual, I won't have to override it later.
    public bool Move(Tuple<int, int> move) {
      return Move(move.Item1, move.Item2);
    }

    private void IncrementPlayerCaptures(Player color) {
      if (color == Player.White) {
        _capturesWhite++;
      } else {
        _capturesBlack++;
      }
    }

    // Returns false if there is no win, true if there is.
    private bool PerformCapturesAndCheckWin(int row, int col) {
      Player currentPlayer = GetCurrentPlayer();
      Tuple<int, int>[] windows = GetWindows(row, col);

      foreach (Direction direction in AllDirections) {
        // Check for a capture forward
        if (MatchesPattern(currentPlayer, windows[(int)direction], HeuristicValues.FORWARD_CAPTURE_PATTERN)) {
          SetSpot(row + DirectionIncrements[(int)direction].Item1,
                  col + DirectionIncrements[(int)direction].Item2, Player.Neither);
          SetSpot(row + 2 * DirectionIncrements[(int)direction].Item1,
                  col + 2 * DirectionIncrements[(int)direction].Item2, Player.Neither);
          IncrementPlayerCaptures(currentPlayer);
        }

        // Check for a capture backward
        if (MatchesPattern(currentPlayer, windows[(int)direction], HeuristicValues.BACKWARD_CAPTURE_PATTERN)) {
          SetSpot(row - DirectionIncrements[(int)direction].Item1,
                  col - DirectionIncrements[(int)direction].Item2, Player.Neither);
          SetSpot(row - 2 * DirectionIncrements[(int)direction].Item1,
                  col - 2 * DirectionIncrements[(int)direction].Item2, Player.Neither);
          IncrementPlayerCaptures(currentPlayer);
        }

        // Check for a pente
        int currentWindow = (currentPlayer == Player.White) ? windows[(int)direction].Item1 : windows[(int)direction].Item2;
        if ((currentWindow & (currentWindow << 1) & (currentWindow << 2) & (currentWindow << 3) & (currentWindow << 4)) != 0) {
          return true;
        }
      }

      if (_capturesWhite >= 5 || _capturesBlack >= 5) {
        return true;
      }

      /*
      if (mPlyNumber > ROWS * COLS) {
        return true;
      }
      */

      return false;
    }

    public Tuple<int, int>[] GetWindows(int row, int col) {
      var retval = new Tuple<int, int>[NUM_DIRECTIONS];
      int whiteRow, blackRow, whiteCol, blackCol, whiteUpDiag, blackUpDiag, whiteDownDiag, blackDownDiag;

      whiteRow = ((_rowsWhite[row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackRow = ((_rowsBlack[row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByRow] = Tuple.Create(whiteRow, blackRow);

      whiteCol = ((_colsWhite[col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      blackCol = ((_colsBlack[col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByCol] = Tuple.Create(whiteCol, blackCol);

      whiteUpDiag = ((_upDiagWhite[UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackUpDiag = ((_upDiagBlack[UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByUpDiag] = Tuple.Create(whiteUpDiag, blackUpDiag);

      whiteDownDiag = ((_downDiagWhite[DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackDownDiag = ((_downDiagBlack[DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByDownDiag] = Tuple.Create(whiteDownDiag, blackDownDiag);

      return retval;
    }

    public bool MatchesPattern(Player currentPlayer, Tuple<int, int> window, Tuple<int, int, int> patternTrio) {
      if (currentPlayer == Player.White) {
        if (((window.Item1 | patternTrio.Item3) == (patternTrio.Item1 | patternTrio.Item3)) &&
            ((window.Item2 | patternTrio.Item3) == (patternTrio.Item2 | patternTrio.Item3))) {
          return true;
        }
      } else {
        if (((window.Item1 | patternTrio.Item3) == (patternTrio.Item2 | patternTrio.Item3)) &&
            ((window.Item2 | patternTrio.Item3) == (patternTrio.Item1 | patternTrio.Item3))) {
          return true;
        }
      }
      return false;
    }

    public Player GetSpot(int row, int col) {
      if (0 <= row && row < ROWS && 0 <= col && col < COLS) {
        if ((_rowsWhite[row] & SPOT_MASKS[col]) != 0) {
          return Player.White;
        } else if ((_rowsBlack[row] & SPOT_MASKS[col]) != 0) {
          return Player.Black;
        } else {
          return Player.Neither;
        }
      } else {
        return Player.Neither;
      }
    }

    public int GetCaptures(Player player) {
      if (player == Player.White) {
        return _capturesWhite;
      } else {
        return _capturesBlack;
      }
    }

    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    public int GetPlyNumber() {
      return _plyNumber;
    }

    public Player GetCurrentPlayer() {
      if (_plyNumber % 2 == 0) {
        return Player.White;
      } else {
        return Player.Black;
      }
    }

    public Player GetOtherPlayer() {
      if (_plyNumber % 2 == 0) {
        return Player.Black;
      } else {
        return Player.White;
      }
    }

    // if the return value is Player.Neither, then the game is not finished.
    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    public Player GetWinner() {
      return _winner;
    }

    public bool IsLegal(int row, int col) {
      if (_plyNumber == 2) {
        int center = ROWS / 2;
        if (center - 2 <= row && row <= center + 2 &&
            center - 2 <= col && col <= center + 2) {
          return false;
        }
      }

      if (GetWinner() != Player.Neither) {
        return false;
      }

      if ((_rowsWhite[row] & SPOT_MASKS[col]) != 0 ||
          (_rowsBlack[row] & SPOT_MASKS[col]) != 0) {
        return false;
      }

      return true;
    }

    public bool IsLegal(Tuple<int, int> move) {
      return IsLegal(move.Item1, move.Item2);
    }

    private void SetSpot(int row, int col, Player color) {
      if (color == Player.White) {
        _rowsWhite[row] |= SPOT_MASKS[col];
        _colsWhite[col] |= SPOT_MASKS[row];
        _upDiagWhite[UpDiagIndex(row, col)] |= SPOT_MASKS[col];
        _downDiagWhite[DownDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else if (color == Player.Black) {
        _rowsBlack[row] |= SPOT_MASKS[col];
        _colsBlack[col] |= SPOT_MASKS[row];
        _upDiagBlack[UpDiagIndex(row, col)] |= SPOT_MASKS[col];
        _downDiagBlack[DownDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else {
        _rowsWhite[row] &= (~SPOT_MASKS[col]);
        _rowsBlack[row] &= (~SPOT_MASKS[col]);

        _colsWhite[col] &= (~SPOT_MASKS[row]);
        _colsBlack[col] &= (~SPOT_MASKS[row]);

        _upDiagWhite[UpDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        _upDiagBlack[UpDiagIndex(row, col)] &= (~SPOT_MASKS[col]);

        _downDiagWhite[DownDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        _downDiagBlack[DownDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
      }
    }

    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    private static int UpDiagIndex(int row, int col) {
      return row + col;
    }

    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    private static int DownDiagIndex(int row, int col) {
      return row - col + COLS - 1;
    }

    public string GetBoardStateStr() {
      StringBuilder sb = new StringBuilder();
      sb.Append(String.Format("turn: {0} white: {1} black: {2} winner: {3}",
                GetPlyNumber(), GetCaptures(Player.White), GetCaptures(Player.Black), GetWinner()));
      sb.Append(Environment.NewLine);
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (GetSpot(row_dex, col_dex) == Player.White) {
            sb.Append(" W");
          } else if (GetSpot(row_dex, col_dex) == Player.Black) {
            sb.Append(" B");
          } else {
            sb.Append(" .");
          }
        }
        sb.Append(" | ");
        sb.Append(row_dex.ToString());
        sb.Append(Environment.NewLine);
      }
      for (int col_dex = 0; col_dex < COLS; col_dex++) {
        sb.Append(String.Format("{0,2}", col_dex));
      }
      sb.Append(Environment.NewLine);
      return sb.ToString();
    }

    public static bool operator ==(Board first, Board second) {
      return (
          Enumerable.SequenceEqual(first._rowsWhite, second._rowsWhite) &&
          Enumerable.SequenceEqual(first._rowsBlack, second._rowsBlack) &&
          Enumerable.SequenceEqual(first._colsWhite, second._colsWhite) &&
          Enumerable.SequenceEqual(first._colsBlack, second._colsBlack) &&
          Enumerable.SequenceEqual(first._upDiagWhite, second._upDiagWhite) &&
          Enumerable.SequenceEqual(first._upDiagBlack, second._upDiagBlack) &&
          Enumerable.SequenceEqual(first._downDiagWhite, second._downDiagWhite) &&
          Enumerable.SequenceEqual(first._downDiagBlack, second._downDiagBlack) &&
          (first.GetWinner() == second.GetWinner()) &&
          (first.GetCaptures(Player.White) == second.GetCaptures(Player.White)) &&
          (first.GetCaptures(Player.Black) == second.GetCaptures(Player.Black)) &&
          (first.GetPlyNumber() == second.GetPlyNumber())
      );
    }

    public static bool operator !=(Board first, Board second) {
      return (first == second) ? false : true;
    }

    public override bool Equals(object obj) {
      return base.Equals(obj);
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }
}
