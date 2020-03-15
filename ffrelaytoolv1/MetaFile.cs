using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffrelaytoolv1
{
    public class MetaFile
    {
        public string[] games;
        public int splitsToShow;
        public int splitFocusOffset;
        public int teamsPerRow;
        public Team[] teams;
        public Layout layout = new Layout();

        public class Team
        {
            public string name;
            public string color;
            public string image;
        }

        public class Layout
        {
            public int boxHeight = 228;
            public int boxWidth = 400;
            public int timerWidth = 254;
            public int timerHeight = 64;
            public bool showSplits = true;
            public bool showCategory = true;
            public bool showGameTimes = true;
        }
    }
}
