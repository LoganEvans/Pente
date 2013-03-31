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

  public class MoveSelectedEventArgs : EventArgs {
    public player_t player { get; set; }
    public int row { get; set; }
    public int col { get; set; }
  }

  // The PlayerInterface shouldn't assume that when getMove is called that the move
  // will be made. It isn't made until setMove is called. This will allow the game controller
  // to synchronize the boards in a uniform fashion.
  public abstract class PlayerBase {
    public abstract void setBoard(BoardInterface board);
    public abstract void setColor(player_t color);
    public abstract void setOpponent(PlayerBase opponent);

    // This should be the start of any processing done by this player.
    public abstract void playerThread();

    public event EventHandler<MoveSelectedEventArgs> MoveSelected;

    protected virtual void OnMoveSelected(MoveSelectedEventArgs args) {
      EventHandler<MoveSelectedEventArgs> handler = MoveSelected;
      if (handler != null) {
        handler(this, args);
      }
    }

    public abstract void MoveSelectedEventHandler_getOpponentMove(object sender, MoveSelectedEventArgs args);
  }
}
