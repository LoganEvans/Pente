using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PenteInterfaces {
  // This can represent the player or the identity of the player that placed a stone in a spot.
  public enum Player {Neither, Black, White};

  public interface BoardInterface {
    // The assumption will be that the current player is making the move. If the move was
    // successfully made, return true. Else, return false.
    bool Move(int row, int col);
    Player GetSpot(int row, int col);
    int GetCaptures(Player player);
    int GetPlyNumber();
    Player GetCurrentPlayer();

    // if the return value is neither, then the game is not finished.
    Player GetWinner();
    bool IsLegal(int row, int col);
  }

  public class MoveSelectedEventArgs : EventArgs {
    public Player player { get; set; }
    public int row { get; set; }
    public int col { get; set; }
  }

  // The PlayerInterface shouldn't assume that when getMove is called that the move
  // will be made. It isn't made until setMove is called. This will allow the game controller
  // to synchronize the boards in a uniform fashion.
  public abstract class PlayerBase {
    public abstract void SetBoard(BoardInterface board);
    public abstract void SetColor(Player color);
    public abstract void SetOpponent(PlayerBase opponent);

    // This should be the start of any processing done by this player.
    public abstract void PlayerThread();

    public event EventHandler<MoveSelectedEventArgs> MoveSelected;

    protected virtual void OnMoveSelected(MoveSelectedEventArgs args) {
      EventHandler<MoveSelectedEventArgs> handler = MoveSelected;
      if (handler != null) {
        handler(this, args);
      }
    }

    public abstract void MoveSelectedEventHandler_GetOpponentMove(object sender, MoveSelectedEventArgs args);
  }
}
