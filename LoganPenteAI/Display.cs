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

namespace LoganPenteAI {
  public partial class Display : Form {
    public const int ROWS = 19;
    public const int COLS = 19;

    public Display() {
      InitializeComponent();
    }

    private void Display_Paint(object sender, PaintEventArgs e) {
      Graphics g = e.Graphics;
      drawBoard(g);
    }

    private void drawBoard(Graphics g) {
      int pen_width = 2;
      int width = this.Size.Width;
      int height = this.Size.Height;
      int pad_h = 40;
      int pad_w = 20;
      int delta_w = (width - 2 * pad_w) / (COLS - 1);
      int delta_h = (height - 2 * pad_h) / (ROWS - 1);

      Pen p_lines = new Pen(Color.Black, pen_width);

      // Draw rows.
      for (int h_dex = 0; h_dex < ROWS; h_dex++) {
        g.DrawLine(p_lines, new Point(pad_w, h_dex * delta_h + pad_h),
                            new Point((COLS - 1) * delta_w + pad_w, h_dex * delta_h + pad_h));
      }

      // Draw cols
      for (int w_dex = 0; w_dex < COLS; w_dex++) {
        g.DrawLine(p_lines, new Point(w_dex * delta_w + pad_w, pad_h),
                            new Point(w_dex * delta_w + pad_w, (ROWS - 1) * delta_h + pad_h));
      }
    }
  }
}
