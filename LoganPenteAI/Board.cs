using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CommonInterfaces;

namespace LoganPenteAI {
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

    public Board() {
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
      Debug.Assert(boardStr.Length == ROWS * COLS);
      mCapturesWhite = capturesWhite;
      mCapturesBlack = capturesBlack;
      mWinner = Player.Neither;
      mMoveNumber = (nextPlayer == Player.White) ? 4 : 5;  // To avoid the rule on moves 0 and 2

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
          } else if (boardStr[row_dex * ROWS + col_dex] == 'B') {
            color = Player.Black;
          } else {
            color = Player.Neither;
          }

          SetSpot(row_dex, col_dex, color);
        }
      }
    }

    public virtual bool Move(int row, int col) {
      if (!IsLegal(row, col)) {
        return false;
      }
      Player current_player = GetCurrentPlayer();

      SetSpot(row, col, GetCurrentPlayer());

      PerformCaptures(row, col);
      if (IsGameOver(row, col)) {
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

    private bool PerformCaptures(int row, int col) {
      Debug.Assert(Pattern.ROW_PATTERN == 0);
      Debug.Assert(Pattern.COL_PATTERN == 1);
      Debug.Assert(Pattern.UP_DIAG_PATTERN == 2);
      Debug.Assert(Pattern.DOWN_DIAG_PATTERN == 3);

      Player current = GetCurrentPlayer();

      bool retval = false;
      List<Tuple<int, int>> windows = GetWindows(row, col);

      var directions = new List<Tuple<int, int>>();
      directions.Add(Tuple.Create(0, 1));  // by row
      directions.Add(Tuple.Create(1, 0));  // by col
      directions.Add(Tuple.Create(-1, 1));  // by up diag
      directions.Add(Tuple.Create(1, 1));  // by down diag

      for (int i = 0; i < directions.Count; i++) {
        if (MatchesPattern(current, windows[i], HeuristicValues.FORWARD_CAPTURE_PATTERN)) {
          SetSpot(row + directions[i].Item1, col + directions[i].Item2, Player.Neither);
          SetSpot(row + 2 * directions[i].Item1, col + 2 * directions[i].Item2, Player.Neither);
          IncrementPlayerCaptures(current);
          retval = true;
        }

        if (MatchesPattern(current, windows[i], HeuristicValues.BACKWARD_CAPTURE_PATTERN)) {
          SetSpot(row - directions[i].Item1, col - directions[i].Item2, Player.Neither);
          SetSpot(row - 2 * directions[i].Item1, col - 2 * directions[i].Item2, Player.Neither);
          IncrementPlayerCaptures(current);
          retval = true;
        }
      }

      return retval;
    }

    public List<Tuple<int, int>> GetWindows(int row, int col) {
      Debug.Assert(Pattern.ROW_PATTERN == 0);
      Debug.Assert(Pattern.COL_PATTERN == 1);
      Debug.Assert(Pattern.UP_DIAG_PATTERN == 2);
      Debug.Assert(Pattern.DOWN_DIAG_PATTERN == 3);

      var retval = new List<Tuple<int, int>>();
      int whiteRow, blackRow, whiteCol, blackCol, whiteUpDiag, blackUpDiag, whiteDownDiag, blackDownDiag;

      whiteRow = ((mRowsWhite[row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackRow = ((mRowsBlack[row] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval.Add(Tuple.Create(whiteRow, blackRow));  // ROW_PATTERN == 0

      whiteCol = ((mColsWhite[col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      blackCol = ((mColsBlack[col] << Pattern.PATTERN_RADIUS) >> row) & Pattern.PATTERN_MASK;
      retval.Add(Tuple.Create(whiteCol, blackCol));  // COL_Pattern.PATTERN == 1

      whiteUpDiag = ((mUpDiagWhite[UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackUpDiag = ((mUpDiagBlack[UpDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval.Add(Tuple.Create(whiteUpDiag, blackUpDiag));  // UP_DIAG_Pattern.PATTERN == 2

      whiteDownDiag = ((mDownDiagWhite[DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      blackDownDiag = ((mDownDiagBlack[DownDiagIndex(row, col)] << Pattern.PATTERN_RADIUS) >> col) & Pattern.PATTERN_MASK;
      retval.Add(Tuple.Create(whiteDownDiag, blackDownDiag));  // DOWN_DIAG_PATTERN == 3

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

    // Specifies the row of the last move. This allows the method to shorten its
    // check for a game over.
    private bool IsGameOver(int row, int col) {
      if (GetWinner() != Player.Neither) {
        return true;
      }

      if (mCapturesWhite >= 5 || mCapturesBlack >= 5) {
        return true;
      }

      if (mMoveNumber > ROWS * COLS) {
        return true;
      }

      foreach (Tuple<int, int> window in GetWindows(row, col)) {
        int currentWindow = (GetCurrentPlayer() == Player.White) ? window.Item1 : window.Item2;
        if ((currentWindow & (currentWindow << 1) & (currentWindow << 2) & (currentWindow << 3) & (currentWindow << 4)) != 0) {
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

    private static int UpDiagIndex(int row, int col) {
      return row + col;
    }

    private static int DownDiagIndex(int row, int col) {
      return row - col + COLS - 1;
    }
  }
}
