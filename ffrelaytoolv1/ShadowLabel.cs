using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public class ShadowLabel : Label
    {
        #region Properties
        protected int _xOffset = 2;
        protected int _yOffset = 2;
        #endregion

        #region Constructor
        public ShadowLabel() : base() => InitializeComponent();
        #endregion

        #region Accessors
        /// <summary>Specifies the solid-colour value of the shadow. No alpha information from this setting is used.</summary>
        /// <remarks>Alpha blending is handled programmatically via the <i>Alpha</i> accessor value.</remarks>
        /// <seealso cref="Alpha"/>
        public Color ShadowColor { get; set; } = Color.Black;

        /// <summary>Specifies the vertical translation of the shadow (up/down). Range: -25 to +25.</summary>
        /// <remarks>Using a negative value shifts the shadow up, while a positive value shifts downwards.</remarks>
        public int xOffset
        {
            get => this._xOffset;
            set => this._xOffset = (value < 0) ? Math.Max(value, -25) : Math.Min(25, value);
        }

        /// <summary>Specifies the horizontal translation of the shadow (left/right). Range: -25 to +25.</summary>
        /// <remarks>Using a negative value shifts the shadow left, while a positive value goes right.</remarks>
        public int yOffset
        {
            get => this._yOffset;
            set => this._yOffset = (value < 0) ? Math.Max(value, -25) : Math.Min(25, value);
        }

        /// <summary>Specifies the starting Alpha value of the shadow (how solid is it).</summary>
        /// <remarks>The shadow is made more transparent as it deepens, from this value to zero.</remarks>
        public byte Alpha { get; set; } = 255;
        #endregion

        #region Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var textSize = g.MeasureString(this.Text, this.Font);
            var heightOffset = (this.Size.Height - textSize.Height) / 2;


            int xStart = Math.Min(this.Location.X, this.Location.X + xOffset),
                xEnd = Math.Max(this.Location.X, this.Location.X + xOffset),
                yStart = Math.Min(this.Location.Y, this.Location.Y + yOffset),
                yEnd = Math.Max(this.Location.Y, this.Location.Y + yOffset),
                steps, xIncrement, yIncrement, alphaIncrement;

            steps = Math.Max(xEnd - xStart, yEnd - yStart);
            xIncrement = (xOffset < 0 ? -1 : 1) * (int)Math.Floor((xEnd - xStart) / (float)steps);
            yIncrement = (yOffset < 0 ? -1 : 1) * (int)Math.Floor((yEnd - yStart) / (float)steps);
            alphaIncrement = (int)Math.Floor(Alpha / (float)steps);

            if (steps > 0)
            {
                for (int i = steps; i > 0; i--)
                    g.DrawString(
                        this.Text,
                        this.Font,
                        new SolidBrush(
                                Color.FromArgb(
                                    this.Alpha - (alphaIncrement * i),
                                    ShadowColor.R,
                                    ShadowColor.G,
                                    ShadowColor.B
                                )
                            ),
                            new PointF()
                            {
                                X = (xIncrement * i), // this.Location.X + (xIncrement * i), 
                                Y = (yIncrement * i) + heightOffset  // this.Location.Y + (yIncrement * i) 
                            }
                        );

                g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(0f, heightOffset));
            }
            else base.OnPaint(e);
        }
        #endregion

        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() =>
            components = new System.ComponentModel.Container();
        #endregion
    }
}
