using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public partial class TeamControl : UserControl
    {
        public TeamInfo teamInfo;

        Form1 parent;

        MetaContext context;

        Label[] teamSplitNames;
        Label[] teamSplitTimes;
        Label[] vsLabelNames;
        Label[] vsLabelTimes;

        Label gameEndsL;
        Label gameEndsR;
        Label gameShortL;
        Label gameShortR;

        Label[] categoryLabels;
        Label commentaryLabel;

        public TeamControl()
        {
            InitializeComponent();
        }

        public void setupTeamControl(Form1 parent, TeamInfo info, MetaContext context)
        {
            this.parent = parent;
            this.teamInfo = info;
            this.context = context;
            TeamSplitButton.BackColor = teamInfo.color;
            foreach (TabPage page in teamTabGroup.TabPages)
            {
                page.BackgroundImage = teamInfo.tabBackground;
            }

            //Construct splits tab
            teamSplitTimes = new Label[context.splitsToShow];
            teamSplitNames = new Label[context.splitsToShow];
            for (int i = 0; i < context.splitsToShow; i++)
            {
                teamSplitNames[i] = Util.createBaseLabel(3, 30 * i + 6, 256, 29, "");
                tabPageSplits.Container.Add(teamSplitNames[i]);
                teamSplitTimes[i] = Util.createBaseLabel(265, 30 * i + 6, 117, 29, "00:00:00");
                tabPageSplits.Container.Add(teamSplitTimes[i]);
            }
            vsLabelNames = new Label[context.numberOfTeams - 1];
            vsLabelTimes = new Label[context.numberOfTeams - 1];
            int adjustedIndex = 0;
            for (int i = 0; i < context.numberOfTeams - 1; i++)
            {
                if (context.teamNames[i].Equals(info.teamName)) { adjustedIndex++; }
                vsLabelNames[i] = Util.createBaseLabel(3, 156 + 30 * i, 230, 29, "Vs Team " + context.teamNames[adjustedIndex]);
                tabPageSplits.Container.Add(vsLabelNames[i]);
                vsLabelTimes[i] = Util.createBaseLabel(242, 156 + 30 * i, 140, 29, "00:00:00");
                tabPageSplits.Container.Add(vsLabelTimes[i]);
            }

            //Construct runner/category/commentary tab
            categoryLabels = new Label[3];
            for (int i = 0; i < 3; i++)
            {
                categoryLabels[i] = Util.createBaseLabel(3, 3 + 30 * i, 391, 35, "", ContentAlignment.MiddleCenter);
                tabPageCategories.Container.Add(categoryLabels[i]);
            }
            commentaryLabel = Util.createBaseLabel(3, 96, 391, 68, "Commentators: ", ContentAlignment.MiddleCenter);
            tabPageCategories.Container.Add(commentaryLabel);

            //Construct game times tab
            int gamesOnEach = context.numberOfGames / 2;
            gameEndsL = Util.createBaseLabel(76, 11, 109, 147, "00:00:00", ContentAlignment.MiddleCenter);
            tabPageTimes.Container.Add(gameEndsL);
            gameEndsR = Util.createBaseLabel(204, 11, 109, 147, "00:00:00", ContentAlignment.MiddleCenter);
            tabPageTimes.Container.Add(gameEndsR);
            string titlesL = "";
            string titlesR = "";
            for (int i = 0; i < gamesOnEach; i++)
            {
                titlesL += context.games[i] + " : ";
                titlesR += " : " + context.games[i + gamesOnEach];
            }
            gameShortL = Util.createBaseLabel(3, 9, 90, 142, titlesL, ContentAlignment.MiddleRight);
            tabPageTimes.Container.Add(gameEndsL);
            gameShortR = Util.createBaseLabel(295, 9, 90, 142, titlesR, ContentAlignment.MiddleLeft);
            tabPageTimes.Container.Add(gameEndsR);
        }

        private void TeamSplitButton_Click(object sender, EventArgs e)
        {

        }

        public void updateTimerEvent(string current, bool cycleInfo)
        {
            teamInfo.teamSplits[teamInfo.teamSplitNum] = current;
            if (!teamInfo.teamFinished)
            {
                updateSplits(parent.fetchOtherTeamInfo(this));
            }
            if (cycleInfo)
            {
                if (teamTabGroup.SelectedIndex == 2)
                { teamTabGroup.SelectedIndex = 0; }
                else { teamTabGroup.SelectedIndex++; }
            }
        }

        void updateSplits(VersusWrapper[] otherTeams)
        {

            int i = Util.Clamp(teamInfo.teamSplitNum, context.splits.Length - (context.splitsToShow - context.splitFocusOffset - 1), context.splitFocusOffset);
            for (int offsetSplit = 0; i < context.splitsToShow; i++)
            {
                teamSplitNames[offsetSplit].Text = Util.stripGameIndicator(context.splits[i - (context.splitFocusOffset - offsetSplit)]);
                teamSplitTimes[offsetSplit].Text = teamInfo.teamSplits[i - (context.splitFocusOffset - offsetSplit)];
            }

            //Split comparisons, since this works even on FF1 it needs to be here
            int offset = 0;
            bool[] vs = new bool[otherTeams.Length];
            while (offset <= teamInfo.teamSplitNum)
            {
                for (int team = 0; team < otherTeams.Length; team++)
                {
                    //If we're on the same split, then don't update the versus differences.
                    if (otherTeams[team].splitNum == teamInfo.teamSplitNum)
                    {
                        vs[team] = true;
                        if (teamInfo.teamSplitNum > 0)
                        {
                            TimeSpan seg = Util.resolveTimeSpan(teamInfo.teamSplits[teamInfo.teamSplitNum - 1],
                                otherTeams[team].splits[teamInfo.teamSplitNum - 1]);
                            Util.updateDifferenceDisplay(vsLabelTimes[team], seg);
                        }
                    }
                    //Otherwise update the value based on the live difference (Only if greater than static difference?)
                    if (!vs[team] && otherTeams[team].splits[teamInfo.teamSplitNum - offset] != "00:00:00")
                    {
                        TimeSpan seg = Util.resolveTimeSpan(teamInfo.teamSplits[teamInfo.teamSplitNum - offset],
                            otherTeams[team].splits[teamInfo.teamSplitNum - offset]);
                        if (teamInfo.teamSplitNum - offset > 0)
                        {
                            //If the offset on the previous split is larger than the live one then use that.
                            TimeSpan olderseg = Util.resolveTimeSpan(teamInfo.teamSplits[teamInfo.teamSplitNum - offset - 1],
                                otherTeams[team].splits[teamInfo.teamSplitNum - offset - 1]);
                            if (Math.Abs(olderseg.Ticks) > Math.Abs(seg.Ticks))
                            {
                                seg = olderseg;
                            }
                        }
                        Util.updateDifferenceDisplay(vsLabelTimes[team], seg);
                        vs[team] = true;
                    }
                }
                if (vs.All(b => b))
                {
                    offset = teamInfo.teamSplitNum;
                }
                offset++;
            }

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            teamInfo.teamGameEndArchive = teamInfo.teamGameEnd;
            if (teamInfo.teamGame == 0)
            {
                gameEndsL.Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                //teamGameEndL.Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                for (int linesToFill = 0; i < context.numberOfGames / 2; i++)
                {
                    gameEndsL.Text += "\n00:00:00";
                    //teamGameEndL.Text += "\n00:00:00";
                }
                TimerLabel.Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                return;
            }
            string lefttimes = teamInfo.teamGameEnd[0] + "\n";
            string righttimes = "";
            for (int j = 1; j < context.numberOfGames; j++)
            {
                string current = "00:00:00";
                //If we're past the selected game, then subtract the previous one to get the segment time over split time
                if (teamInfo.teamGame > j)
                {
                    TimeSpan seg = Util.resolveTimeSpan(teamInfo.teamGameEnd[j], teamInfo.teamGameEnd[j - 1]);
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (teamInfo.teamGame == j)
                {
                    TimeSpan seg = Util.resolveTimeSpan(teamInfo.teamSplits[teamInfo.teamSplitNum], teamInfo.teamGameEnd[j - 1]);
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    TimerLabel.Text = current;
                    teamInfo.teamGameEndArchive[teamInfo.teamGame] = teamInfo.teamSplits[teamInfo.teamSplitNum];
                }
                if (j < context.numberOfGames / 2)
                { lefttimes += current + "\n"; }
                else
                { righttimes += current + "\n"; }
            }
            gameEndsL.Text = lefttimes;
            gameEndsR.Text = righttimes;
        }

    }
}
