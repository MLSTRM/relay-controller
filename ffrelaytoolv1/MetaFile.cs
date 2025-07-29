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
            public bool splitLabesOppositeAlign = false;
            public int? splitLabelSize = null;
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
            public float defaultTimerSubFontSize = 14;
            public string textColour = "#000000";
            public string timerTextColor = "#FFFFFF";
            public string timerForeColor = "#000000";
            public string timerBackColor = "#FFFFFF";
            public string timerFadeColor = "#464646";
            public string timerFontFamily = "Microsoft Sans Serif";
            public bool useBasicNameLayout = false;
            public bool teamControlUsesTeamColours = true;
            public string teamControlColor = "#FFFFFF";
            public int teamNameDecoration = 0;
            public bool decorateRunnerNames = false;
            public int teamLabelColorAlpha = 255;
            public int teamTimerColorAlpha = 255;
            public bool teamLabelDropShadow = false;
            public bool teamTimerDropShadow = false;
        }

        public class Features
        {
            public bool defaultAutoCycling = true;
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
            public bool permanentRunnerNames = false;
            public int graphThreshold = 0;
            public MetaControlFeatures metaControl = new MetaControlFeatures();
            public DiscordFeatures discord = new DiscordFeatures();
        }

        public class DiscordFeatures
        {
            public ulong guildId = 0;
            public ulong channelId = 0;
            public string token = "";
        }

        public class MetaControlFeatures
        {
            public int width = 813;
            public int height = 192;
            public int margin = 6;
            public string color = "#FFFFFF";
            public bool splits = false;
            public bool commentators = false;
            public int commentaryHeight = 100;
            public MetaControlGraph graph = new MetaControlGraph();
            // TODO: new option for split commentary/game display for new 2 team display concept.
            // Basically the same as alwaysShowCommentary but it goes in a different parent rather than each tab group being replicated across.
            // Then want a combined graph/split page maybe, but its basically just static view and can be comped in the layout overall (with meta control size adjustment and split page statically showing)
            public bool alwaysShowCommentary = false;
            public int commentaryFlowDirection = 1;
            public bool commentaryInlineHeader = true;
            public float commentatorNameSize = 14f;
            public float commentatorPronounSize = 12f;
            public float? staticSubheaderFontSize = 16f;
            public int staticSubheaderSplitPoint = 300;
        }

        public class MetaControlGraph
        {
            public int splitsToShow = 6;
            public int splitFocusOffset = 5;
            public bool predictPartialGraphLines = false;
            public bool positiveOnly = false;
            public bool negativeOnly = false;
            public float xAxisFontSize = 12f;
            public float yAxisFontSize = 12f;
            public bool showStart = true;
        }
    }
}
