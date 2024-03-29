﻿using System;
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
            public bool leftAlign = true;
        }

        public class Layout
        {
            public bool horizontalDisplay = true;
            public int rowHeight = 96;
            public int rowWidth = 320;
            public int boxHeight = 192;
            public int boxWidth = 893;
            public int timerWidth = 254;
            public int timerHeight = 64;
            public int vsLabelMin = 150;
            public bool showSplits = true;
            public bool showCategory = true;
            public bool showGameTimes = true;
            public bool splitLabelsOnSplitsPage = true;
            public int boxMargin = 6;
            public int timerTickInterval = 250;
            public int infoCycleTicks = 40;
            public int splitButtonCooldown = 5000; // 5 seconds
            public int remoteSplitCooldown = 300000; // 5 minutes
            public int gameTimeLayout = 0; // 0 = opposing columns
            public double vsLabelRow = 1f;
            public string fontFamily = "Lucida Sans Typewriter";
            public float mainTimerFontSize = 60;
            public float teamTimerFontSize = 40;
            public float splitTimerFontSize = 20;
            public float defaultTimerFontSize = 16;
            public string textColour = "#000000";
            public string timerTextColor = "#FFFFFF";
            public string timerForeColor = "#000000";
            public string timerBackColor = "#FFFFFF";
            public string timerFadeColor = "#464646";
            public string timerFontFamily = "Microsoft Sans Serif";
        }

        public class Features
        {
            public bool showSplits = true;
            public bool showRunners = true;
            public bool showGameTimes = true;
            public bool syncInfoCycling = false;
            public int[] infoCyclingPattern = new int[] { };
            public bool syncSplits = true;
            public bool showAllVs = false;
            public bool vsLabelsOnSplitsPage = false;
            public bool commentatorsOnRunnerPage = true;
            public bool teamGameIcons = true;
            public bool mainLayoutBackground = true;
            public bool enableRemoteSplitting = false;
            public bool enableDiscordIntegration = false;
            public bool showMetaControl = false;
            public bool showGraph = false;
            public int graphThreshold = 0;
            public MetaControlFeatures metaControl = new MetaControlFeatures();
        }

        public class MetaControlFeatures
        {
            public int width = 813;
            public int height = 192;
            public int margin = 6;
            public string color = "#FFFFFF";
            public bool splits = false;
            public bool commentators = false;
            public int commentaryHeight = -1;
            public MetaControlGraph graph = new MetaControlGraph();
        }

        public class MetaControlGraph
        {
            public int splitsToShow = 6;
            public int splitFocusOffset = 5;
        }
    }
}
