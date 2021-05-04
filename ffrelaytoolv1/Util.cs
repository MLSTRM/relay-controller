using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ffrelaytoolv1
{
    static class Util
    {
        public static string gameSep = "!";

        public static string emptyTime = "00:00:00";

        public readonly static FontFamily lucida = new FontFamily("Lucida Sans Typewriter");

        public readonly static Font lucidaFont = new Font(lucida, 16);

        public static Font lucidaFontSized(int size) => new Font(lucida, size);

        public static TimeSpan resolveTimeSpan(string a, string b)
        {
            TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
            TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
            TimeSpan seg = s1 - s2;
            return seg;
        }

        public static TimeSpan parseTimeSpan(string a)
        {
            return new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
        }

        public static String stripGameIndicator(String s) => s.Replace(gameSep, "");

        public static void updateDifferenceDisplay(Label label, TimeSpan seg)
        {
            string current = "";
            if (seg.TotalHours > -1)
            { if (seg.TotalSeconds < 0) { current += "-"; } else { current += "+"; } }
            current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
            label.Text = current;
        }

        public static void updatePositiveDifferenceDisplay(Label label, TimeSpan seg)
        {
            if (seg.TotalHours <= 0)
            { label.Text = ""; }
            else
            { label.Text = string.Format("+{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg); }
        }

        public static T clamp<T>(T target, T upper, T lower) where T : IComparable{
            if (target.CompareTo(lower) < 0)
            {
                return lower;
            }
            else if (target.CompareTo(upper)>0)
            {
                return upper;
            }
            return target;
        }

        public static Label createBaseLabel(int x, int y, int w, int h, string text, ContentAlignment textAlign)
        {
            System.Console.WriteLine("creating label at " + x + ", " + y);
            return new Label
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                ForeColor = Color.White,
                TextAlign = textAlign,
                BackColor = Color.Transparent,
                Font = Util.lucidaFont,
                Text = text
            };
        }

        public static Label createBaseLabel(int x, int y, int w, int h, string text) => 
            createBaseLabel(x, y, w, h, text, ContentAlignment.MiddleLeft);

        public static String outputCaptureInfo(Control control, Control parent)
        {
            return "l: " + control.Location.X + 
                ", t: " + control.Location.Y + 
                ", r: " + (parent.ClientSize.Width - control.Location.X - control.Size.Width) + 
                ", b: " + (parent.ClientSize.Height - control.Location.Y - control.Size.Height);
        }

        public static String outputCaptureInfoRelative(Control control, Control parent, params Control[] outer)
        {
            return "l: " + (outer.Sum(c => c.Location.X) + control.Location.X) +
                ", t: " + (outer.Sum(c => c.Location.Y) + control.Location.Y) +
                ", r: " + (parent.ClientSize.Width - outer.Sum(c => c.Location.X) - control.Location.X - control.Size.Width) +
                ", b: " + (parent.ClientSize.Height - outer.Sum(c => c.Location.Y) - control.Location.Y - control.Size.Height);
        }
    }
}
