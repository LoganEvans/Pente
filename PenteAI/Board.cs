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
    public const int WIN_CAPS = 5;
    public const int ROW_MASK = 0x7FFFF;
    private const int DIAGONALS = ROWS + COLS - 1;
    public static readonly int[] SPOT_MASKS = {0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
                                               0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000,
                                               0x4000, 0x8000, 0x10000, 0x20000, 0x40000};

    // Note: It is possible to fix the size of this by using a struct and an unsafe code block. It is possible
    // that doing so would greatly speed up code that deals with the board. Evaluate this.
    protected enum BoardRep {
      RowsWhite = 0,
      RowsBlack = 1,
      ColsWhite = 2,
      ColsBlack = 3,
      UpDiagWhite = 4,
      UpDiagBlack = 5,
      DownDiagWhite = 6,
      DownDiagBlack = 7
    };

    protected static readonly int[] WhiteBoardRep = {
      (int)BoardRep.RowsWhite,
      (int)BoardRep.ColsWhite,
      (int)BoardRep.UpDiagWhite,
      (int)BoardRep.DownDiagWhite
    };

    protected static readonly int[] BlackBoardRep = {
      (int)BoardRep.RowsBlack,
      (int)BoardRep.ColsBlack,
      (int)BoardRep.UpDiagBlack,
      (int)BoardRep.DownDiagBlack
    };

    protected int[][] _boardRepresentations;
    protected static readonly int _NUM_BOARD_REPS = 8;

    public struct _snapEntry {
      public int row;
      public int col;
      public Player newValue;
      public Player oldValue;
    }

    protected _snapEntry[] _snapLog;
    protected int _snapIdx;
    protected const int _SNAP_LOG_LENGTH = ROWS * COLS + 2 * 2 * WIN_CAPS;

    private Player _winner;
    private int _plyNumber;
    private int _capturesWhite;
    private int _capturesBlack;

    public const int NUM_DIRECTIONS = 4;
    public enum Direction {ByRow = 0, ByCol = 1, ByUpDiag = 2, ByDownDiag = 3};
    protected static Tuple<int, int>[] _directionIncrements = null;
    protected static Tuple<int, int>[] DirectionIncrements { get { return _directionIncrements; } }
    protected static Direction[] _allDirections = null;
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

    private void InitializeBoardRepresentations() {
      _boardRepresentations = new int[_NUM_BOARD_REPS][];
      _boardRepresentations[(int)BoardRep.RowsWhite] = new int[ROWS];
      _boardRepresentations[(int)BoardRep.RowsBlack] = new int[ROWS];
      _boardRepresentations[(int)BoardRep.ColsWhite] = new int[COLS];
      _boardRepresentations[(int)BoardRep.ColsBlack] = new int[COLS];
      _boardRepresentations[(int)BoardRep.UpDiagWhite] = new int[DIAGONALS];
      _boardRepresentations[(int)BoardRep.UpDiagBlack] = new int[DIAGONALS];
      _boardRepresentations[(int)BoardRep.DownDiagWhite] = new int[DIAGONALS];
      _boardRepresentations[(int)BoardRep.DownDiagBlack] = new int[DIAGONALS];
    }

    private void InitializeBoardRepresentations(int[][] copyFrom) {
      InitializeBoardRepresentations();
      for (int i = 0; i < _NUM_BOARD_REPS; i++) {
        for (int count = 0; count < _boardRepresentations[i].Length; count++) {
          _boardRepresentations[i][count] = copyFrom[i][count];
        }
      }
    }

    public Board() {
      if (DirectionIncrements == null) {
        InitBoard();
      }

      InitializeBoardRepresentations();

      _winner = Player.Neither;
      _plyNumber = 0;
      _capturesWhite = 0;
      _capturesBlack = 0;

      // 2 players, 2 board modifications per capture, WIN_CAPS captures per player.

//    _snapIdx = 0;
//    _snapLog = new _snapEntry[_SNAP_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_LOG_LENGTH; i++) {
//      _snapLog[i] = new _snapEntry();
//    }
    }

    public Board(BoardInterface copyFrom) {
      if (DirectionIncrements == null) {
        InitBoard();
      }
      InitializeBoardRepresentations();
      _winner = copyFrom.GetWinner();
      _plyNumber = copyFrom.GetPlyNumber();
      _capturesWhite = copyFrom.GetCaptures(Player.White);
      _capturesBlack = copyFrom.GetCaptures(Player.Black);

//    _snapLog = new _snapEntry[_SNAP_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_LOG_LENGTH; i++) {
//      _snapLog[i] = new _snapEntry();
//    }
//    _snapIdx = 0;
    }

    public Board(Board copyFrom) {
      if (DirectionIncrements == null) {
        InitBoard();
      }
      InitializeBoardRepresentations(copyFrom._boardRepresentations);
      _winner = copyFrom._winner;
      _plyNumber = copyFrom._plyNumber;
      _capturesWhite = copyFrom._capturesWhite;
      _capturesBlack = copyFrom._capturesBlack;

//    _snapLog = new _snapEntry[_SNAP_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_LOG_LENGTH; i++) {
//      _snapLog[i] = new _snapEntry();
//    }
//    for (int i = 0; i < copyFrom._snapIdx; i++) {
//      // Structs are copy on assign.
//      _snapLog[i] = copyFrom._snapLog[i];
//    }
//    _snapIdx = copyFrom._snapIdx;
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
      InitializeBoardRepresentations();

      // We can't really roll back any farther than this if we're initializing
      // from a raw board.
//    _snapLog = new _snapEntry[_SNAP_LOG_LENGTH];
//    for (int i = 0; i < _SNAP_LOG_LENGTH; i++) {
//      _snapLog[i] = new _snapEntry();
//    }
//    _snapIdx = 0;

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

          SetSpot(row_dex, col_dex, color, disableSnap: true);
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

      if (_capturesWhite >= WIN_CAPS || _capturesBlack >= WIN_CAPS) {
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

      whiteRow = ((_boardRepresentations[(int)BoardRep.RowsWhite][row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackRow = ((_boardRepresentations[(int)BoardRep.RowsBlack][row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByRow] = Tuple.Create(whiteRow, blackRow);

      whiteCol = ((_boardRepresentations[(int)BoardRep.ColsWhite][col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      blackCol = ((_boardRepresentations[(int)BoardRep.ColsBlack][col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByCol] = Tuple.Create(whiteCol, blackCol);

      whiteUpDiag = ((_boardRepresentations[(int)BoardRep.UpDiagWhite][UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col)
        & Pattern.PATTERN_MASK;
      blackUpDiag = ((_boardRepresentations[(int)BoardRep.UpDiagBlack][UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col)
        & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByUpDiag] = Tuple.Create(whiteUpDiag, blackUpDiag);

      whiteDownDiag = ((_boardRepresentations[(int)BoardRep.DownDiagWhite][DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col)
        & Pattern.PATTERN_MASK;
      blackDownDiag = ((_boardRepresentations[(int)BoardRep.DownDiagBlack][DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col)
        & Pattern.PATTERN_MASK;
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
        if ((_boardRepresentations[(int)BoardRep.RowsWhite][row] & SPOT_MASKS[col]) != 0) {
          return Player.White;
        } else if ((_boardRepresentations[(int)BoardRep.RowsBlack][row] & SPOT_MASKS[col]) != 0) {
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

      if ((_boardRepresentations[(int)BoardRep.RowsWhite][row] & SPOT_MASKS[col]) != 0 ||
          (_boardRepresentations[(int)BoardRep.RowsBlack][row] & SPOT_MASKS[col]) != 0) {
        return false;
      }

      return true;
    }

    public bool IsLegal(Tuple<int, int> move) {
      if (move != null) {
        return IsLegal(move.Item1, move.Item2);
      } else {
        return false;
      }
    }

    public virtual Tuple<int, int, int, int> GetSnapshotData() {
      return Tuple.Create(_plyNumber, _capturesWhite, _capturesBlack, _snapIdx);
    }

    // Caution: A Rollback WILL clear the _winner field, so don't use it if the board is already
    // in that state.
    public virtual void Rollback(Tuple<int, int, int, int> snapshotData) {
      _winner = Player.Neither;
      _plyNumber = snapshotData.Item1;
      _capturesWhite = snapshotData.Item2;
      _capturesBlack = snapshotData.Item3;
      // _snapIdx is pointing at the next log location, but nothing has been written there.
      _snapIdx--;
      for (; _snapIdx > snapshotData.Item4; _snapIdx--) {
        SetSpot(_snapLog[_snapIdx].row, _snapLog[_snapIdx].col, _snapLog[_snapIdx].oldValue, disableSnap: true);
      }
      SetSpot(_snapLog[_snapIdx].row, _snapLog[_snapIdx].col, _snapLog[_snapIdx].oldValue, disableSnap: true);
    }

    private void SetSpot(int row, int col, Player color, bool disableSnap = false) {
//    if (!disableSnap) {
//      _snapLog[_snapIdx].row = row;
//      _snapLog[_snapIdx].col = col;
//      _snapLog[_snapIdx].newValue = color;
//      // If color == Player.Neither, we just made a capture.
//      if (color == Player.Neither) {
//        _snapLog[_snapIdx].oldValue = GetSpot(row, col);
//      } else {
//        _snapLog[_snapIdx].oldValue = Player.Neither;
//      }
//      _snapIdx++;
//    }

      if (color == Player.White) {
        _boardRepresentations[(int)BoardRep.RowsWhite][row] |= SPOT_MASKS[col];
        _boardRepresentations[(int)BoardRep.ColsWhite][col] |= SPOT_MASKS[row];
        _boardRepresentations[(int)BoardRep.UpDiagWhite][UpDiagIndex(row, col)] |= SPOT_MASKS[col];
        _boardRepresentations[(int)BoardRep.DownDiagWhite][DownDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else if (color == Player.Black) {
        _boardRepresentations[(int)BoardRep.RowsBlack][row] |= SPOT_MASKS[col];
        _boardRepresentations[(int)BoardRep.ColsBlack][col] |= SPOT_MASKS[row];
        _boardRepresentations[(int)BoardRep.UpDiagBlack][UpDiagIndex(row, col)] |= SPOT_MASKS[col];
        _boardRepresentations[(int)BoardRep.DownDiagBlack][DownDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else {
        _boardRepresentations[(int)BoardRep.RowsWhite][row] &= (~SPOT_MASKS[col]);
        _boardRepresentations[(int)BoardRep.RowsBlack][row] &= (~SPOT_MASKS[col]);

        _boardRepresentations[(int)BoardRep.ColsWhite][col] &= (~SPOT_MASKS[row]);
        _boardRepresentations[(int)BoardRep.ColsBlack][col] &= (~SPOT_MASKS[row]);

        _boardRepresentations[(int)BoardRep.UpDiagWhite][UpDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        _boardRepresentations[(int)BoardRep.UpDiagBlack][UpDiagIndex(row, col)] &= (~SPOT_MASKS[col]);

        _boardRepresentations[(int)BoardRep.DownDiagWhite][DownDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        _boardRepresentations[(int)BoardRep.DownDiagBlack][DownDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
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
      if (!(first.GetWinner() == second.GetWinner()) &&
           (first.GetCaptures(Player.White) == second.GetCaptures(Player.White)) &&
           (first.GetCaptures(Player.Black) == second.GetCaptures(Player.Black)) &&
           (first.GetPlyNumber() == second.GetPlyNumber())) {
        return false;
      }

      for (int i = 0; i < _NUM_BOARD_REPS; i++) {
        if (!first._boardRepresentations[i].SequenceEqual(second._boardRepresentations[i])) {
          return false;
        }
      }
      return true;
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
