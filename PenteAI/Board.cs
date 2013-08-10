using System;
using System.Collections.Generic;
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
    private int[] mRowsWhite;
    private int[] mRowsBlack;
    private int[] mColsWhite;
    private int[] mColsBlack;
    private int[] mUpDiagWhite;
    private int[] mUpDiagBlack;
    private int[] mDownDiagWhite;
    private int[] mDownDiagBlack;

    private Player mWinner;
    private int mMoveNumber;
    private int mCapturesWhite;
    private int mCapturesBlack;

    public const int NUM_DIRECTIONS = 4;
    public enum Direction {ByRow = 0, ByCol = 1, ByUpDiag = 2, ByDownDiag = 3};
    private static Tuple<int, int>[] sDirectionIncrements = null;
    private static Direction[] sAllDirections = null;

    // This should be called early on.
    public static void InitBoard() {
      if (sDirectionIncrements == null) {
        sDirectionIncrements = new Tuple<int, int>[NUM_DIRECTIONS];
        sDirectionIncrements[(int)Direction.ByRow] = Tuple.Create(0, 1);
        sDirectionIncrements[(int)Direction.ByCol] = Tuple.Create(1, 0);
        sDirectionIncrements[(int)Direction.ByUpDiag] = Tuple.Create(-1, 1);
        sDirectionIncrements[(int)Direction.ByDownDiag] = Tuple.Create(1, 1);

        sAllDirections = new Direction[4];
        sAllDirections[(int)Direction.ByRow] = Direction.ByRow;
        sAllDirections[(int)Direction.ByCol] = Direction.ByCol;
        sAllDirections[(int)Direction.ByUpDiag] = Direction.ByUpDiag;
        sAllDirections[(int)Direction.ByDownDiag] = Direction.ByDownDiag;
      }
    }

    public Board() {
      if (sDirectionIncrements == null) {
        InitBoard();
      }
      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      mColsWhite = new int[ROWS];
      mColsBlack = new int[ROWS];
      mUpDiagWhite = new int[DIAGONALS];
      mUpDiagBlack = new int[DIAGONALS];
      mDownDiagWhite = new int[DIAGONALS];
      mDownDiagBlack = new int[DIAGONALS];

      mWinner = Player.Neither;
      mMoveNumber = 0;
      mCapturesWhite = 0;
      mCapturesBlack = 0;
      Move(ROWS / 2, COLS / 2);
    }

    public Board(BoardInterface copyFrom) {
      if (sDirectionIncrements == null) {
        InitBoard();
      }
      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      mColsWhite = new int[ROWS];
      mColsBlack = new int[ROWS];
      mUpDiagWhite = new int[DIAGONALS];
      mUpDiagBlack = new int[DIAGONALS];
      mDownDiagWhite = new int[DIAGONALS];
      mDownDiagBlack = new int[DIAGONALS];
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          SetSpot(row_dex, col_dex, copyFrom.GetSpot(row_dex, col_dex));
        }
      }
      mWinner = copyFrom.GetWinner();
      mMoveNumber = copyFrom.GetMoveNumber();
      mCapturesWhite = copyFrom.GetCaptures(Player.White);
      mCapturesBlack = copyFrom.GetCaptures(Player.Black);
    }

    public Board(Board copyFrom) {
      if (sDirectionIncrements == null) {
        InitBoard();
      }
      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      mColsWhite = new int[ROWS];
      mColsBlack = new int[ROWS];
      mUpDiagWhite = new int[DIAGONALS];
      mUpDiagBlack = new int[DIAGONALS];
      mDownDiagWhite = new int[DIAGONALS];
      mDownDiagBlack = new int[DIAGONALS];

      for (int i = 0; i < ROWS; i++) {
        mRowsWhite[i] = copyFrom.mRowsWhite[i];
        mRowsBlack[i] = copyFrom.mRowsBlack[i];
        mColsWhite[i] = copyFrom.mColsWhite[i];
        mColsBlack[i] = copyFrom.mColsBlack[i];
      }

      for (int i = 0; i < DIAGONALS; i++) {
        mUpDiagWhite[i] = copyFrom.mUpDiagWhite[i];
        mUpDiagBlack[i] = copyFrom.mUpDiagBlack[i];
        mDownDiagWhite[i] = copyFrom.mDownDiagWhite[i];
        mDownDiagBlack[i] = copyFrom.mDownDiagBlack[i];
      }

      mWinner = copyFrom.mWinner;
      mMoveNumber = copyFrom.mMoveNumber;
      mCapturesWhite = copyFrom.mCapturesWhite;
      mCapturesBlack = copyFrom.mCapturesBlack;
    }

    public Board(Player nextPlayer, int capturesWhite, int capturesBlack, String boardStr) {
      if (sDirectionIncrements == null) {
        InitBoard();
      }
      Debug.Assert(boardStr.Length == ROWS * COLS);
      mCapturesWhite = capturesWhite;
      mCapturesBlack = capturesBlack;
      mWinner = Player.Neither;
      mMoveNumber = 2 * capturesWhite + 2 * capturesBlack;

      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      mColsWhite = new int[ROWS];
      mColsBlack = new int[ROWS];
      mUpDiagWhite = new int[DIAGONALS];
      mUpDiagBlack = new int[DIAGONALS];
      mDownDiagWhite = new int[DIAGONALS];
      mDownDiagBlack = new int[DIAGONALS];

      Player color;
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (boardStr[row_dex * ROWS + col_dex] == 'W') {
            color = Player.White;
            mMoveNumber++;
          } else if (boardStr[row_dex * ROWS + col_dex] == 'B') {
            color = Player.Black;
            mMoveNumber++;
          } else {
            color = Player.Neither;
          }

          SetSpot(row_dex, col_dex, color);
        }
      }

      if (GetCurrentPlayer() != nextPlayer) {
        mMoveNumber++;
      }
    }

    public virtual bool Move(int row, int col) {
      if (!IsLegal(row, col)) {
        return false;
      }
      Player current_player = GetCurrentPlayer();

      SetSpot(row, col, GetCurrentPlayer());

      if (PerformCapturesAndCheckWin(row, col)) {
        mWinner = current_player;
      }
      mMoveNumber++;

      return true;
    }

    private void IncrementPlayerCaptures(Player color) {
      if (color == Player.White) {
        mCapturesWhite++;
      } else {
        mCapturesBlack++;
      }
    }

    // Returns false if there is no win, true if there is.
    private bool PerformCapturesAndCheckWin(int row, int col) {
      Player currentPlayer = GetCurrentPlayer();
      Tuple<int, int>[] windows = GetWindows(row, col);

      foreach (Direction direction in sAllDirections) {
        // Check for a capture forward
        if (MatchesPattern(currentPlayer, windows[(int)direction], HeuristicValues.FORWARD_CAPTURE_PATTERN)) {
          SetSpot(row + sDirectionIncrements[(int)direction].Item1,
                  col + sDirectionIncrements[(int)direction].Item2, Player.Neither);
          SetSpot(row + 2 * sDirectionIncrements[(int)direction].Item1,
                  col + 2 * sDirectionIncrements[(int)direction].Item2, Player.Neither);
          IncrementPlayerCaptures(currentPlayer);
        }

        // Check for a capture backward
        if (MatchesPattern(currentPlayer, windows[(int)direction], HeuristicValues.BACKWARD_CAPTURE_PATTERN)) {
          SetSpot(row - sDirectionIncrements[(int)direction].Item1,
                  col - sDirectionIncrements[(int)direction].Item2, Player.Neither);
          SetSpot(row - 2 * sDirectionIncrements[(int)direction].Item1,
                  col - 2 * sDirectionIncrements[(int)direction].Item2, Player.Neither);
          IncrementPlayerCaptures(currentPlayer);
        }

        // Check for a pente
        int currentWindow = (currentPlayer == Player.White) ? windows[(int)direction].Item1 : windows[(int)direction].Item2;
        if ((currentWindow & (currentWindow << 1) & (currentWindow << 2) & (currentWindow << 3) & (currentWindow << 4)) != 0) {
          return true;
        }
      }

      if (mCapturesWhite >= 5 || mCapturesBlack >= 5) {
        return true;
      }

      /*
      if (mMoveNumber > ROWS * COLS) {
        return true;
      }
      */

      return false;
    }

    public Tuple<int, int>[] GetWindows(int row, int col) {
      var retval = new Tuple<int, int>[NUM_DIRECTIONS];
      int whiteRow, blackRow, whiteCol, blackCol, whiteUpDiag, blackUpDiag, whiteDownDiag, blackDownDiag;

      whiteRow = ((mRowsWhite[row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackRow = ((mRowsBlack[row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByRow] = Tuple.Create(whiteRow, blackRow);

      whiteCol = ((mColsWhite[col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      blackCol = ((mColsBlack[col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByCol] = Tuple.Create(whiteCol, blackCol);

      whiteUpDiag = ((mUpDiagWhite[UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackUpDiag = ((mUpDiagBlack[UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval[(int)Direction.ByUpDiag] = Tuple.Create(whiteUpDiag, blackUpDiag);

      whiteDownDiag = ((mDownDiagWhite[DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackDownDiag = ((mDownDiagBlack[DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
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
        if ((mRowsWhite[row] & SPOT_MASKS[col]) != 0) {
          return Player.White;
        } else if ((mRowsBlack[row] & SPOT_MASKS[col]) != 0) {
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
        return mCapturesWhite;
      } else {
        return mCapturesBlack;
      }
    }

    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    public int GetMoveNumber() {
      return mMoveNumber;
    }

    public Player GetCurrentPlayer() {
      if (mMoveNumber % 2 == 0) {
        return Player.White;
      } else {
        return Player.Black;
      }
    }

    public Player GetOtherPlayer() {
      if (mMoveNumber % 2 == 0) {
        return Player.Black;
      } else {
        return Player.White;
      }
    }

    // if the return value is Player.Neither, then the game is not finished.
    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    public Player GetWinner() {
      return mWinner;
    }

    public bool IsLegal(int row, int col) {
      if (mMoveNumber == 2) {
        int center = ROWS / 2;
        if (center - 2 <= row && row <= center + 2 &&
            center - 2 <= col && col <= center + 2) {
          return false;
        }
      }

      if (GetWinner() != Player.Neither) {
        return false;
      }

      if ((mRowsWhite[row] & SPOT_MASKS[col]) != 0 ||
          (mRowsBlack[row] & SPOT_MASKS[col]) != 0) {
        return false;
      }

      return true;
    }

    private void SetSpot(int row, int col, Player color) {
      if (color == Player.White) {
        mRowsWhite[row] |= SPOT_MASKS[col];
        mColsWhite[col] |= SPOT_MASKS[row];
        mUpDiagWhite[UpDiagIndex(row, col)] |= SPOT_MASKS[col];
        mDownDiagWhite[DownDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else if (color == Player.Black) {
        mRowsBlack[row] |= SPOT_MASKS[col];
        mColsBlack[col] |= SPOT_MASKS[row];
        mUpDiagBlack[UpDiagIndex(row, col)] |= SPOT_MASKS[col];
        mDownDiagBlack[DownDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else {
        mRowsWhite[row] &= (~SPOT_MASKS[col]);
        mRowsBlack[row] &= (~SPOT_MASKS[col]);

        mColsWhite[col] &= (~SPOT_MASKS[row]);
        mColsBlack[col] &= (~SPOT_MASKS[row]);

        mUpDiagWhite[UpDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        mUpDiagBlack[UpDiagIndex(row, col)] &= (~SPOT_MASKS[col]);

        mDownDiagWhite[DownDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        mDownDiagBlack[DownDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
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
  }
}
