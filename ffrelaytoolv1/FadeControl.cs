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

namespace ffrelaytoolv1
{
    public partial class FadeControl : UserControl
    {
        public FadeControl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(Size.Width, 0), ForeColor, Color.Transparent);
            g.FillRectangle(brush, new Rectangle(new Point(0), Size));
        }
    }
}
