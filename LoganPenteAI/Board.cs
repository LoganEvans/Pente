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
    public const int WINDOW_MASK = 0x1FF;
    private const int WINDOW_RADIUS = 4;
    private static readonly Tuple<int, int, int> BACKWARD_CAPTURE_PATTERN = Tuple.Create(0x12, 0xc, 0x1e1);
    private static readonly Tuple<int, int, int> FORWARD_CAPTURE_PATTERN = Tuple.Create(0x90, 0x60, 0x10f);
    private const int ROW_WINDOW = 0;
    private const int COL_WINDOW = 1;
    private const int UP_DIAG_WINDOW = 2;
    private const int DOWN_DIAG_WINDOW = 3;

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

    private player_t mWinner;
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

      mWinner = player_t.neither;
      mMoveNumber = 0;
      mCapturesWhite = 0;
      mCapturesBlack = 0;
      move(ROWS / 2, COLS / 2);
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
          setSpot(row_dex, col_dex, copyFrom.getSpot(row_dex, col_dex));
        }
      }
      mWinner = copyFrom.getWinner();
      mMoveNumber = copyFrom.getMoveNumber();
      mCapturesWhite = copyFrom.getCaptures(player_t.white);
      mCapturesBlack = copyFrom.getCaptures(player_t.black);
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

    public Board(player_t nextPlayer, int capturesWhite, int capturesBlack, String boardStr) {
      Debug.Assert(boardStr.Length == ROWS * COLS);
      mCapturesWhite = capturesWhite;
      mCapturesBlack = capturesBlack;
      mWinner = player_t.neither;
      mMoveNumber = (nextPlayer == player_t.white) ? 4 : 5;  // To avoid the rule on moves 0 and 2

      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      mColsWhite = new int[ROWS];
      mColsBlack = new int[ROWS];
      mUpDiagWhite = new int[DIAGONALS];
      mUpDiagBlack = new int[DIAGONALS];
      mDownDiagWhite = new int[DIAGONALS];
      mDownDiagBlack = new int[DIAGONALS];

      player_t color;
      for (int row_dex = 0; row_dex < ROWS; row_dex++) {
        for (int col_dex = 0; col_dex < COLS; col_dex++) {
          if (boardStr[row_dex * ROWS + col_dex] == 'W') {
            color = player_t.white;
          } else if (boardStr[row_dex * ROWS + col_dex] == 'B') {
            color = player_t.black;
          } else {
            color = player_t.neither;
          }

          setSpot(row_dex, col_dex, color);
        }
      }
    }

    public virtual bool move(int row, int col) {
      if (!isLegal(row, col)) {
        return false;
      }
      player_t current_player = getCurrentPlayer();

      setSpot(row, col, getCurrentPlayer());

      performCaptures(row, col);
      if (isGameOver(row, col)) {
        mWinner = current_player;
      }
      mMoveNumber++;

      return true;
    }

    private void incrementPlayerCaptures(player_t color) {
      if (color == player_t.white) {
        mCapturesWhite++;
      } else {
        mCapturesBlack++;
      }
    }

    private bool performCaptures(int row, int col) {
      Debug.Assert(ROW_WINDOW == 0);
      Debug.Assert(COL_WINDOW == 1);
      Debug.Assert(UP_DIAG_WINDOW == 2);
      Debug.Assert(DOWN_DIAG_WINDOW == 3);

      player_t current = getCurrentPlayer();

      bool retval = false;
      List<Tuple<int, int>> windows = getWindows(row, col);

      var directions = new List<Tuple<int, int>>();
      directions.Add(Tuple.Create(0, 1));  // by row
      directions.Add(Tuple.Create(1, 0));  // by col
      directions.Add(Tuple.Create(-1, 1));  // by up diag
      directions.Add(Tuple.Create(1, 1));  // by down diag

      for (int i = 0; i < directions.Count; i++) {
        if (matchesPattern(current, windows[i], FORWARD_CAPTURE_PATTERN)) {
          setSpot(row + directions[i].Item1, col + directions[i].Item2, player_t.neither);
          setSpot(row + 2 * directions[i].Item1, col + 2 * directions[i].Item2, player_t.neither);
          incrementPlayerCaptures(current);
          retval = true;
        }

        if (matchesPattern(current, windows[i], BACKWARD_CAPTURE_PATTERN)) {
          setSpot(row - directions[i].Item1, col - directions[i].Item2, player_t.neither);
          setSpot(row - 2 * directions[i].Item1, col - 2 * directions[i].Item2, player_t.neither);
          incrementPlayerCaptures(current);
          retval = true;
        }
      }

      return retval;
    }

    public List<Tuple<int, int>> getWindows(int row, int col) {
      Debug.Assert(ROW_WINDOW == 0);
      Debug.Assert(COL_WINDOW == 1);
      Debug.Assert(UP_DIAG_WINDOW == 2);
      Debug.Assert(DOWN_DIAG_WINDOW == 3);

      var retval = new List<Tuple<int, int>>();
      int whiteRow, blackRow, whiteCol, blackCol, whiteUpDiag, blackUpDiag, whiteDownDiag, blackDownDiag;

      whiteRow = ((mRowsWhite[row] << WINDOW_RADIUS) >> col) & WINDOW_MASK;
      blackRow = ((mRowsBlack[row] << WINDOW_RADIUS) >> col) & WINDOW_MASK;
      retval.Add(Tuple.Create(whiteRow, blackRow));  // ROW_WINDOW == 0

      whiteCol = ((mColsWhite[col] << WINDOW_RADIUS) >> row) & WINDOW_MASK;
      blackCol = ((mColsBlack[col] << WINDOW_RADIUS) >> row) & WINDOW_MASK;
      retval.Add(Tuple.Create(whiteCol, blackCol));  // COL_WINDOW == 1

      whiteUpDiag = ((mUpDiagWhite[upDiagIndex(row, col)] << WINDOW_RADIUS) >> col) & WINDOW_MASK;
      blackUpDiag = ((mUpDiagBlack[upDiagIndex(row, col)] << WINDOW_RADIUS) >> col) & WINDOW_MASK;
      retval.Add(Tuple.Create(whiteUpDiag, blackUpDiag));  // UP_DIAG_WINDOW == 2

      whiteDownDiag = ((mDownDiagWhite[downDiagIndex(row, col)] << WINDOW_RADIUS) >> col) & WINDOW_MASK;
      blackDownDiag = ((mDownDiagBlack[downDiagIndex(row, col)] << WINDOW_RADIUS) >> col) & WINDOW_MASK;
      retval.Add(Tuple.Create(whiteDownDiag, blackDownDiag));  // DOWN_DIAG_WINDOW == 3

      return retval;
    }

    public bool matchesPattern(player_t currentPlayer, Tuple<int, int> window, Tuple<int, int, int> patternTrio) {
      if (currentPlayer == player_t.white) {
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
    private bool isGameOver(int row, int col) {
      if (getWinner() != player_t.neither) {
        return true;
      }

      if (mCapturesWhite >= 5 || mCapturesBlack >= 5) {
        return true;
      }

      if (mMoveNumber > ROWS * COLS) {
        return true;
      }

      foreach (Tuple<int, int> window in getWindows(row, col)) {
        int currentWindow = (getCurrentPlayer() == player_t.white) ? window.Item1 : window.Item2;
        if ((currentWindow & (currentWindow << 1) & (currentWindow << 2) & (currentWindow << 3) & (currentWindow << 4)) != 0) {
          return true;
        }
      }

      return false;
    }

    public player_t getSpot(int row, int col) {
      if (0 <= row && row < ROWS && 0 <= col && col < COLS) {
        if ((mRowsWhite[row] & SPOT_MASKS[col]) != 0) {
          return player_t.white;
        } else if ((mRowsBlack[row] & SPOT_MASKS[col]) != 0) {
          return player_t.black;
        } else {
          return player_t.neither;
        }
      } else {
        return player_t.neither;
      }
    }

    public int getCaptures(player_t player) {
      if (player == player_t.white) {
        return mCapturesWhite;
      } else {
        return mCapturesBlack;
      }
    }

    public int getMoveNumber() {
      return mMoveNumber;
    }

    public player_t getCurrentPlayer() {
      if (mMoveNumber % 2 == 0) {
        return player_t.white;
      } else {
        return player_t.black;
      }
    }

    public player_t getOtherPlayer() {
      if (mMoveNumber % 2 == 0) {
        return player_t.black;
      } else {
        return player_t.white;
      }
    }

    // if the return value is player_t.neither, then the game is not finished.
    public player_t getWinner() {
      return mWinner;
    }

    public bool isLegal(int row, int col) {
      if (mMoveNumber == 2) {
        int center = ROWS / 2;
        if (center - 2 <= row && row <= center + 2 &&
            center - 2 <= col && col <= center + 2) {
          return false;
        }
      }

      if (getWinner() != player_t.neither) {
        return false;
      }

      if ((mRowsWhite[row] & SPOT_MASKS[col]) != 0 ||
          (mRowsBlack[row] & SPOT_MASKS[col]) != 0) {
        return false;
      }

      return true;
    }

    private void setSpot(int row, int col, player_t color) {
      if (color == player_t.white) {
        mRowsWhite[row] |= SPOT_MASKS[col];
        mColsWhite[col] |= SPOT_MASKS[row];
        mUpDiagWhite[upDiagIndex(row, col)] |= SPOT_MASKS[col];
        mDownDiagWhite[downDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else if (color == player_t.black) {
        mRowsBlack[row] |= SPOT_MASKS[col];
        mColsBlack[col] |= SPOT_MASKS[row];
        mUpDiagBlack[upDiagIndex(row, col)] |= SPOT_MASKS[col];
        mDownDiagBlack[downDiagIndex(row, col)] |= SPOT_MASKS[col];
      } else {
        mRowsWhite[row] &= (~SPOT_MASKS[col]);
        mRowsBlack[row] &= (~SPOT_MASKS[col]);

        mColsWhite[col] &= (~SPOT_MASKS[row]);
        mColsBlack[col] &= (~SPOT_MASKS[row]);

        mUpDiagWhite[upDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        mUpDiagBlack[upDiagIndex(row, col)] &= (~SPOT_MASKS[col]);

        mDownDiagWhite[downDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
        mDownDiagBlack[downDiagIndex(row, col)] &= (~SPOT_MASKS[col]);
      }
    }

    private static int upDiagIndex(int row, int col) {
      return row + col;
    }

    private static int downDiagIndex(int row, int col) {
      return row - col + COLS - 1;
    }
  }
}
