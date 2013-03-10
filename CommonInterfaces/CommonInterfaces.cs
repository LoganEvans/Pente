using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces {
  // This can represent the player or the identity of the player that placed a stone in a spot.
  public enum player_t {black, white, neither};

  public interface BoardInterface {
    // The assumption will be that the current player is making the move. If the move was
    // successfully made, return true. Else, return false.
    bool move(int row, int col);

    player_t getSpot(int row, int col);
    int getCaptures(player_t player);
    int getMoveNumber();
    player_t getCurrentPlayer();

    // if the return value is neither, then the game is not finished.
    player_t getWinner();
    bool isLegal(int row, int col);
  }

  public interface PlayerInterface {
    // The order of the Tuple is <row, col>
    Tuple<int, int> getMove();
    void setOpponentMove(player_t move);
  }
}
