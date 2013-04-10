using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonInterfaces {
  public partial class Display : Form {
    public const int ROWS = 19;
    public const int COLS = 19;
    public const int PAD_H = 40;
    public const int PAD_W = 10;
    private BoardInterface mBoard;

    public event EventHandler<MoveSelectedEventArgs> MoveSelectedByClick;

    public Display(BoardInterface board, PlayerBase playerWhite, PlayerBase playerBlack) {
      InitializeComponent();

      SetBoard(board);
      playerWhite.MoveSelected += MoveSelectedEventHandler;
      playerBlack.MoveSelected += MoveSelectedEventHandler;
    }

    private void SetText() {
      this.Text = "Pente -- Turn: " + mBoard.GetMoveNumber() +
                  " White captures: " + mBoard.GetCaptures(Player.White) +
                  " Black captures: " + mBoard.GetCaptures(Player.Black);
    }

    public void MoveSelectedEventHandler(object sender, MoveSelectedEventArgs args) {
      mBoard.Move(args.row, args.col);
      Invalidate();
      if (mBoard.GetWinner() != Player.Neither) {
        MessageBox.Show("Winner: " + mBoard.GetWinner());
      }
    }

    public void SetBoard(BoardInterface board) {
      mBoard = board;
    }

    public BoardInterface GetBoard() {
      return mBoard;
    }

    private void Display_Paint(object sender, PaintEventArgs e) {
      Graphics g = e.Graphics;
      SetText();
      DrawBoardLines(g);
      DrawStones(g);
    }

    private void DrawBoardLines(Graphics g) {
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

    private void DrawStones(Graphics g) {
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
          if (GetBoard().GetSpot(row_dex, col_dex) == Player.White) {
            g.DrawEllipse(p_outline, new Rectangle(center_col - 1, center_row - 1, pen_width + 2, pen_width + 2));
            g.DrawEllipse(p_white, new Rectangle(center_col, center_row, pen_width, pen_width));
          } else if (GetBoard().GetSpot(row_dex, col_dex) == Player.Black) {
            g.DrawEllipse(p_outline, new Rectangle(center_col - 1, center_row - 1, pen_width + 2, pen_width + 2));
          }
        }
      }
    }

    // Handles the board click.
    private void OnClick(object sender, EventArgs e) {
      int width = this.Size.Width;
      int base_w = this.PointToScreen(Point.Empty).X;
      int delta_w = (width - 2 * PAD_W) / (COLS - 1);
      int clicked_w = MousePosition.X - base_w - PAD_W;
      int region_w = delta_w / 4;
      int clicked_col;
      if (Math.Abs((clicked_w % delta_w) - delta_w) < region_w) {
        // Clicked just to the left of the line.
        clicked_col = clicked_w / delta_w + 1;
      } else if (clicked_w % delta_w < region_w) {
        // Clicked just to the right of the line.
        clicked_col = clicked_w / delta_w;
      } else {
        // Clicked too far away from the line.
        return;
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
        return;
      }

      MoveSelectedEventArgs args = new MoveSelectedEventArgs();
      args.row = clicked_row;
      args.col = clicked_col;
      args.player = mBoard.GetCurrentPlayer();
      OnMoveSelectedByClick(args);
    }

    protected virtual void OnMoveSelectedByClick(MoveSelectedEventArgs args) {
      EventHandler<MoveSelectedEventArgs> handler = MoveSelectedByClick;
      if (handler != null) {
        handler(this, args);
      }
    }
  }
}
