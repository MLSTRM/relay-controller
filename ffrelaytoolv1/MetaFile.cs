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
        public Features features = new Features();

        public class Team
        {
            public string name;
            public string color;
            public string image;
            public int splitKey;
        }

        public class Layout
        {
            public int boxHeight = 192;
            public int boxWidth = 893;
            public int timerWidth = 254;
            public int timerHeight = 64;
            public bool showSplits = true;
            public bool showCategory = true;
            public bool showGameTimes = true;
            public int boxMargin = 6;
            public int timerTickInterval = 250;
            public int infoCycleTicks = 40;
            public int splitButtonCooldown = 5000;
        }

        public class Features
        {
            public bool showSplits = true;
            public bool showRunners = true;
            public bool showGameTimes = true;
            public bool syncInfoCycling = false;
            public bool syncSplits = true;
            public bool showAllVs = false;
            public bool vsLabelsOnSplitsPage = false;
            public bool teamGameIcons = true;
            public bool mainLayoutBackground = true;
            public bool enableRemoteSplitting = false;
        }
    }
}
