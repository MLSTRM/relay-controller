using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    static class Util
    {
        public static string gameSep = "!";

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
    }
}
