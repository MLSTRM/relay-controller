using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public partial class TriangleControl : UserControl
    {
        public TriangleControl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brush = new SolidBrush(ForeColor);
            // not sure why this needs to be 2,0 but it looks the best sooooo
            g.FillPolygon(brush, new Point[] {new Point(0,0), new Point(Size.Width,0), new Point(0, Size.Height) });
        }
    }
}
