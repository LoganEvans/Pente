using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CommonInterfaces;

namespace LoganPenteAI {
  public partial class Display : Form {
    public const int ROWS = 19;
    public const int COLS = 19;
    public const int PAD_H = 40;
    public const int PAD_W = 10;
    private Board mBoard;
    private PlayerInterface mPlayerWhite;
    private PlayerInterface mPlayerBlack;

    /*
    public Display() {
      InitializeComponent();
    }

    public Display(Board board) {
      InitializeComponent();
      setBoard(board);
    }
    */

    public Display(Board board, PlayerInterface playerWhite, PlayerInterface playerBlack) {
      InitializeComponent();
      setBoard(board);
      mPlayerWhite = playerWhite;
      mPlayerBlack = playerBlack;

      gameLoop();
    }

    private void gameLoop() {
      while (mBoard.getWinner() == player_t.neither) {
        Console.WriteLine(" > gameLoop.loop... ");
        Invalidate();
        if (getBoard().getWinner() != player_t.neither) {
          MessageBox.Show("Winner: " + getBoard().getWinner());
        }

        if (getBoard().getCurrentPlayer() == player_t.white &&
            (mPlayerWhite is PlayerHuman)) {
          // Wait for a mouse click.
          Console.WriteLine("Player white (human)");
          return;
        } else if (getBoard().getCurrentPlayer() == player_t.black &&
                   mPlayerBlack is PlayerHuman) {
          // Again, wait for mouse click...
          Console.WriteLine("Player black (human)");
          return;
        } else if (getBoard().getCurrentPlayer() == player_t.white) {
          // White is an AI...
          Tuple<int, int> move = mPlayerWhite.getMove();
          Console.WriteLine("Player white (AI). move: " + move);
          setMoveForAll(move);
          //mPlayerBlack.setOpponentMove(move);
        } else {
          // Black is an AI...
          Tuple<int, int> move = mPlayerBlack.getMove();
          Console.WriteLine("Player black (AI). move: " + move);
          setMoveForAll(move);
          //mPlayerWhite.setOpponentMove(move);
        }
      }
    }

    private void setMoveForAll(Tuple<int, int> move) {
      mBoard.move(move.Item1, move.Item2);
      mPlayerWhite.setMove(move);
      mPlayerBlack.setMove(move);
    }

    public void setBoard(Board board) {
      mBoard = board;
    }

    public Board getBoard() {
      return mBoard;
    }

    private void Display_Paint(object sender, PaintEventArgs e) {
      Graphics g = e.Graphics;
      drawBoard(g);
      drawStones(g);
    }

    private void drawBoard(Graphics g) {
      int pen_width = 2;
      int width = this.Size.Width;
      int height = this.Size.Height;
      int delta_w = (width - 2 * PAD_W) / (COLS - 1);
      int delta_h = (height - 2 * PAD_H) / (ROWS - 1);

      Pen p_lines = new Pen(Color.Black, pen_width);

      // Draw rows.
      for (int h_dex = 0; h_dex < ROWS; h_dex++) {
        g.DrawLine(p_lines, new Point(PAD_W, h_dex * delta_h + PAD_H),
                            new Point((COLS - 1) * delta_w + PAD_W, h_dex * delta_h + PAD_H));
      }

      // Draw cols
      for (int w_dex = 0; w_dex < COLS; w_dex++) {
        g.DrawLine(p_lines, new Point(w_dex * delta_w + PAD_W, PAD_H),
                            new Point(w_dex * delta_w + PAD_W, (ROWS - 1) * delta_h + PAD_H));
      }
    }

    private void drawStones(Graphics g) {
      int width = this.Size.Width;
      int height = this.Size.Height;
      int delta_w = (width - 2 * PAD_W) / (COLS - 1);
      int delta_h = (height - 2 * PAD_H) / (ROWS - 1);
      int pen_width = delta_w * 2 / 5;
      Pen p_white = new Pen(Color.White, pen_width);
      Pen p_outline = new Pen(Color.Black, pen_width + 2);

      for (int col_dex = 0; col_dex < COLS; col_dex++) {
        for (int row_dex = 0; row_dex < ROWS; row_dex++) {
          int center_col = PAD_W + col_dex * delta_w - pen_width / 2;
          int center_row = PAD_H + row_dex * delta_h - pen_width / 2;
          if (getBoard().getSpot(row_dex, col_dex) == player_t.white) {
            g.DrawEllipse(p_outline, new Rectangle(center_col - 1, center_row - 1, pen_width + 2, pen_width + 2));
            g.DrawEllipse(p_white, new Rectangle(center_col, center_row, pen_width, pen_width));
          } else if (getBoard().getSpot(row_dex, col_dex) == player_t.black) {
            g.DrawEllipse(p_outline, new Rectangle(center_col - 1, center_row - 1, pen_width + 2, pen_width + 2));
          }
        }
      }
    }

    // Handles the board click.
    private void onClick(object sender, EventArgs e) {
      Tuple<int, int> spot = getClickedSpot();
      if ((getBoard().getCurrentPlayer() == player_t.white && mPlayerWhite is PlayerHuman) ||
          (getBoard().getCurrentPlayer() == player_t.black && mPlayerBlack is PlayerHuman)) {
        if (spot == null) {
          return;
        } else {
          //mPlayerWhite.setOpponentMove(spot);
          //mPlayerBlack.setOpponentMove(spot);
          setMoveForAll(spot);
        }
      }

      gameLoop();
    }

    // Returns <row, col> or null
    private Tuple<int, int> getClickedSpot() {
      int width = this.Size.Width;
      int base_w = this.PointToScreen(Point.Empty).X;
      int delta_w = (width - 2 * PAD_W) / (COLS - 1);
      int clicked_w = MousePosition.X - base_w - PAD_W;
      int region_w = delta_w / 4;
      int clicked_col;
      //if (Math.Abs(clicked_w - region_w)
      if (Math.Abs((clicked_w % delta_w) - delta_w) < region_w) {
        // Clicked just to the left of the line.
        clicked_col = clicked_w / delta_w + 1;
      } else if (clicked_w % delta_w < region_w) {
        // Clicked just to the right of the line.
        clicked_col = clicked_w / delta_w;
      } else {
        // Clicked too far away from the line.
        return null;
      }

      int height = this.Size.Height;
      int base_h = this.PointToScreen(Point.Empty).Y;
      int delta_h = (height - 2 * PAD_H) / (ROWS - 1);
      int region_h = delta_h / 4;
      int clicked_h = MousePosition.Y - base_h - PAD_H;
      int clicked_row;
      //if (Math.Abs(clicked_h - region_h)
      if (Math.Abs((clicked_h % delta_h) - delta_h) < region_h) {
        // Clicked just to the left of the line.
        clicked_row = clicked_h / delta_h + 1;
      } else if (clicked_h % delta_h < region_h) {
        // Clicked just to the right of the line.
        clicked_row = clicked_h / delta_h;
      } else {
        // Clicked too far away from the line.
        return null;
      }

      return new Tuple<int, int>(clicked_row, clicked_col);
    }
  }
}
