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

        public static TimeSpan resolveTimeSpan(string a, string b)
        {
            TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
            TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
            TimeSpan seg = s1 - s2;
            return seg;
        }

        public static String stripGameIndicator(String s)
        {
            return s.Replace(gameSep, "");
        }

        public static void updateDifferenceDisplay(Label label, TimeSpan seg)
        {
            string current = "";
            if (seg.TotalHours > -1)
            { if (seg.TotalSeconds < 0) { current += "-"; } else { current += "+"; } }
            current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
            label.Text = current;
        }

        public static T Clamp<T>(T target, T upper, T lower) where T : IComparable{
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

        public static FontFamily lucida = new FontFamily("Lucida Sans Typewriter");

        public static Font lucidaFont = new Font(lucida, 16);

        public static Label createBaseLabel(int x, int y, int w, int h, string text, ContentAlignment textAlign)
        {
            Label label = new Label();
            label.Location = new Point(x, y);
            label.Size = new Size(w, h);
            label.ForeColor = Color.White;
            label.TextAlign = textAlign;
            label.BackColor = Color.Transparent;
            label.Font = Util.lucidaFont;
            label.Text = text;
            return label;
        }

        public static Label createBaseLabel(int x, int y, int w, int h, string text)
        {
            return createBaseLabel(x, y, w, h, text, ContentAlignment.MiddleLeft);
        }
    }
}
