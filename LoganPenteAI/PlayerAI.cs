using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class PlayerAI : PlayerBase {
    private GameState mGameState;
    private Player mColor;
    private const int LOOKAHEAD = 2;

    public PlayerAI() {
    }

    public PlayerAI(Player color, Board board) {
      SetBoard(board);
      SetColor(color);
    }

    public override void SetBoard(BoardInterface board) {
      mGameState = new GameState(board, LOOKAHEAD);
    }

    public override void SetColor(Player color) {
      mColor = color;
    }

    public override void SetOpponent(PlayerBase opponent) {
      MoveSelected += opponent.MoveSelectedEventHandler_GetOpponentMove;
    }

    public override void MoveSelectedEventHandler_GetOpponentMove(object sender, MoveSelectedEventArgs args) {
      mGameState.Move(args.row, args.col);
    }

    // The order of the Tuple is <row, col>
    public Tuple<int, int> GetMove() {
      Tuple<int, int> move = mGameState.GetBestMove();
      Console.WriteLine("move number: " + mGameState.GetMoveNumber() + " color: " + mColor + ", best move: " + move);
      return move;
    }

    public void SetMove(Tuple<int, int> move) {
      //Console.WriteLine(" > setOpponentMove(" + move + ") for AI " + mColor);
      mGameState.Move(move.Item1, move.Item2);
    }

    public override void PlayerThread() {
    }
  }
}
