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
    private const int ROW_MASK = 0x7FFFF;
    // Note: It is possible to fix the size of this by using a struct and an unsafe code block. It is possible
    // that doing so would greatly speed up code that deals with the board. Evaluate this.
    private int[] mRows;
    private player_t mWinner;
    private int mTurnNumber;
    private Tuple<int, int> mCaptures;

    public Board() {
      mRows = new int[ROWS];
      mWinner = player_t.neither;
      mTurnNumber = 0;
      mCaptures = new Tuple<int, int>(0, 0);
    }

    public Board(Board copyFrom) {
      mRows = new int[ROWS];
      for (int i = 0; i < ROWS; i++) {
        mRows[i] = copyFrom.mRows[i];
      }
      mWinner = copyFrom.mWinner;
      mTurnNumber = copyFrom.mTurnNumber;
      mCaptures = copyFrom.mCaptures;
    }

    public bool move(int row, int col) {
      if (this.isCapture(row, col)) {
      }
      if (this.isGameOver(row)) {
        
      }
      return false;
    }

    private bool isCapture(int row, int col) {
      return false;
    }

    // Specifies the row of the last move. This allows the method to shorten its
    // check for a game over.
    private bool isGameOver(int row) {
      return false;
    }

    public player_t getSpot(int row, int col) {
      return player_t.neither;
    }

    public int getCaptures(player_t player) { return -1; }
    public int getMoveNumber() { return -1; }
    public player_t getCurrentPlayer() { return player_t.neither; }

    // if the return value is player_t.neither, then the game is not finished.
    public player_t getWinner() { return player_t.neither; }
    public bool isLegal(int row, int col) { return false; }
  }

  public class PlayerAI : PlayerInterface {
    // The order of the Tuple is <row, col>
    public Tuple<int, int> getMove() {
      return null;
    }

    public void setOpponentMove(player_t move) {
    }
  }
}
