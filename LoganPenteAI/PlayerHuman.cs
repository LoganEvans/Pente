﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonInterfaces;

namespace LoganPenteAI {
  public class PlayerHuman : PlayerInterface {
    private Board mBoard;
    private player_t mColor;

    public PlayerHuman(player_t color) {
      mBoard = new Board();
      mColor = color;
    }

    // The order of the Tuple is <row, col>
    public Tuple<int, int> getMove() {
      return null;
    }

    public void setOpponentMove(Tuple<int, int> move) {
      mBoard.move(move);
    }
  }
}
