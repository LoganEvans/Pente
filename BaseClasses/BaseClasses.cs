using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses {

  // This can represent the player or the identity of the player that placed a stone in a spot.
  public enum player_t {black, white, neither};

  public abstract class BoardBase {
    public const int numRows = 19;
    public const int numCols = 19;

    protected BoardBase() { }
    protected BoardBase(BoardBase board) { }

    // The assumption will be that the current player is making the move. If the move was
    // successfully made, return true. Else, return false.
    public abstract bool move(int row, int col);

    public abstract player_t getSpot(int row, int col);
    public abstract int getCaptures(player_t player);
    public abstract int getMoveNumber();
    public abstract player_t getCurrentPlayer();

    // if the return value is neither, then the game is not finished.
    public abstract player_t getWinner();
    public abstract bool isLegal(int row, int col);
  }

  public abstract class PlayerBase {
    protected PlayerBase(player_t player, BoardBase board) { }

    // The order of the Tuple is <row, col>
    public abstract Tuple<int, int> getMove();
    public abstract void setOpponentMove(player_t move);
  }

  public abstract class HeuristicBase {
    public abstract player_t predictWinner(BoardBase board);
    public abstract double getHeuristic(BoardBase board, double valueWhiteWin=1.0, double valueBlackWin=1.0);
    public abstract double getUncertainty();
    public abstract double getHeuristicWin(player_t player);
    public abstract double getHeuristicLoss(player_t player);
  }
}

