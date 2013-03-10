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
    public Display() {
      InitializeComponent();
    }

    private void Display_Paint(object sender, PaintEventArgs e) {
      Graphics g = e.Graphics;
      Rectangle rect = new Rectangle(50, 30, 100, 100);
      LinearGradientBrush lBrush = new LinearGradientBrush(rect, Color.Red, Color.Yellow, LinearGradientMode.BackwardDiagonal);
      g.FillRectangle(lBrush, rect); 
    }

    private void gT(object sender, EventArgs e) {

    }
  }
}
