using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class Board : BoardInterface {
    public const int ROWS = 19;
    public const int COLS = 19;
    public const int ROW_MASK = 0x7FFFF;
    public readonly int[] COL_MASKS = {0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
                                        0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000,
                                        0x4000, 0x8000, 0x10000, 0x20000, 0x40000};

    // Note: It is possible to fix the size of this by using a struct and an unsafe code block. It is possible
    // that doing so would greatly speed up code that deals with the board. Evaluate this.
    private int[] mRowsWhite;
    private int[] mRowsBlack;
    private player_t mWinner;
    private int mMoveNumber;
    private int mCapturesWhite;
    private int mCapturesBlack;

    public Board() {
      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      mWinner = player_t.neither;
      mMoveNumber = 0;
      mCapturesWhite = 0;
      mCapturesBlack = 0;
      move(ROWS / 2, COLS / 2);
    }

    public Board(Board copyFrom) {
      mRowsWhite = new int[ROWS];
      mRowsBlack = new int[ROWS];
      for (int i = 0; i < ROWS; i++) {
        mRowsWhite[i] = copyFrom.mRowsWhite[i];
        mRowsBlack[i] = copyFrom.mRowsBlack[i];
      }
      mWinner = copyFrom.mWinner;
      mMoveNumber = copyFrom.mMoveNumber;
      mCapturesWhite = copyFrom.mCapturesWhite;
      mCapturesBlack = copyFrom.mCapturesBlack;
    }

    public bool move(int row, int col) {
      if (!isLegal(row, col)) {
        return false;
      }
      player_t current_player = getCurrentPlayer();

      if (current_player == player_t.white) {
        mRowsWhite[row] |= COL_MASKS[col];
      } else {
        mRowsBlack[row] |= COL_MASKS[col];
      }

      performCaptures(row, col);
      if (isGameOver(row, col)) {
        mWinner = current_player;
      }
      mMoveNumber++;

      return true;
    }

    public bool move(Tuple<int, int> spot) {
      if (spot == null) {
        return false;
      }
      return move(spot.Item1, spot.Item2);
    }

    private bool performCaptures(int row, int col) {
      List<Tuple<int, int>> directions = new List<Tuple<int, int>>();
      directions.Add(new Tuple<int, int>(0, 1));  // horizontal
      directions.Add(new Tuple<int, int>(1, 0));  // vertical
      directions.Add(new Tuple<int, int>(1, 1));  // forward slash
      directions.Add(new Tuple<int, int>(1, -1));  // back slash

      player_t current = getCurrentPlayer();
      player_t other = getNonCurrentPlayer();

      bool retval = false;

      foreach (Tuple<int, int> direction in directions) {
        for (int sign = 0; sign <= 1; sign++) {
          int a = direction.Item1, b = direction.Item2;
          if (sign == 1) {
            a *= -1;
            b *= -1;
          }

          if (getSpot(row + a, col + b) == other &&
              getSpot(row + a + a, col + b + b) == other &&
              getSpot(row + a + a + a, col + b + b + b) == current) {
            retval = true;
            if (current == player_t.white) {
              mRowsBlack[row + a] &= (~COL_MASKS[col + b]);
              mRowsBlack[row + a + a] &= (~COL_MASKS[col + b + b]);
              mCapturesWhite++;
            } else {
              mRowsWhite[row + a] &= (~COL_MASKS[col + b]);
              mRowsWhite[row + a + a] &= (~COL_MASKS[col + b + b]);
              mCapturesBlack++;
            }
          }
        }
      }

      return retval;
    }

    public List<Tuple<int, int> > getWindows(int row, int col) {
      List<Tuple<int, int>> windows = new List<Tuple<int, int>>();
      List<Tuple<int, int>> directions = new List<Tuple<int, int>>();
      directions.Add(new Tuple<int, int>(0, 1));  // horizontal
      directions.Add(new Tuple<int, int>(1, 0));  // vertical
      directions.Add(new Tuple<int, int>(1, 1));  // forward slash
      directions.Add(new Tuple<int, int>(1, -1));  // back slash

      int index = 0;
      foreach (Tuple<int, int> direction in directions) {
        int windowWhite = 0;
        int windowBlack = 0;
        int inspectedCol;
        int inspectedRow;
        index = 0;

        for (int i = -4; i <= 4; i++) {
          inspectedCol = col + (direction.Item1 * i);
          inspectedRow = row + (direction.Item2 * i);

          try {
            if ((mRowsWhite[inspectedRow] & COL_MASKS[inspectedCol]) != 0) {
              windowWhite |= COL_MASKS[index];
            }
          } catch (IndexOutOfRangeException) {
          }

          try {
            if ((mRowsBlack[inspectedRow] & COL_MASKS[inspectedCol]) != 0) {
              windowBlack |= COL_MASKS[index];
            }
          } catch (IndexOutOfRangeException) {
          }

          index++;
        }
        windows.Add(new Tuple<int, int>(windowWhite, windowBlack));
      }
      return windows;
    }

    // patternLength should be an odd number
    public bool matchesPattern(int windowWhite, int windowBlack,
                               int patternWhite, int patternBlack, int patternIgnore) {
      if (((windowWhite | patternIgnore) == (patternWhite | patternIgnore)) &&
          ((windowBlack | patternIgnore) == (patternBlack | patternIgnore))) {
        return true;
      } else {
        return false;
      }
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

      // Strategy: Construct 4 numbers that each represent 9 spots on the board. Then, perform bit shifts on
      // the numbers to see if 5 adjacent ones are set.
      int[] boardRows;
      if (getCurrentPlayer() == player_t.white) {
        boardRows = mRowsWhite;
      } else {
        boardRows = mRowsBlack;
      }

      List<Tuple<int, int>> directions = new List<Tuple<int, int>>();
      directions.Add(new Tuple<int, int>(0, 1));  // horizontal
      directions.Add(new Tuple<int, int>(1, 0));  // vertical
      directions.Add(new Tuple<int, int>(1, 1));  // forward slash
      directions.Add(new Tuple<int, int>(1, -1));  // back slash

      foreach (Tuple<int, int> direction in directions) {
        int window = 0;
        int inspectedCol;
        int inspectedRow;
        int index = 0;
        for (int i = -4; i <= 4; i++) {
          inspectedCol = col + (direction.Item1 * i);
          inspectedRow = row + (direction.Item2 * i);

          try {
            if ((boardRows[inspectedRow] & COL_MASKS[inspectedCol]) != 0) {
              window |= COL_MASKS[index];
            }
          } catch (IndexOutOfRangeException) {
            break;
          }

          index++;
        }

        if ((window & (window << 1) & (window << 2) & (window << 3) & (window << 4)) != 0) {
          return true;
        }
      }

      return false;
    }

    public player_t getSpot(int row, int col) {
      try {
        if ((mRowsWhite[row] & COL_MASKS[col]) != 0) {
          return player_t.white;
        } else if ((mRowsBlack[row] & COL_MASKS[col]) != 0) {
          return player_t.black;
        } else {
          return player_t.neither;
        }
      } catch (IndexOutOfRangeException) {
        return player_t.neither;
      }
    }

    public int getCaptures(player_t player) { return -1; }
    public int getMoveNumber() { return mMoveNumber; }
    public player_t getCurrentPlayer() {
      if (mMoveNumber % 2 == 0) {
        return player_t.white;
      } else {
        return player_t.black;
      }
    }

    public player_t getNonCurrentPlayer() {
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

      if ((mRowsWhite[row] & COL_MASKS[col]) != 0 ||
          (mRowsBlack[row] & COL_MASKS[col]) != 0) {
        return false;
      }

      return true;
    }
  }
}
