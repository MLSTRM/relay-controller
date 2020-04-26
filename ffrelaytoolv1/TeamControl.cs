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

        Label[] gameEndsL;
        Label[] gameEndsR;
        Label[] gameShortL;
        Label[] gameShortR;

        Label[] categoryLabels;
        Label commentaryLabel;

        public TeamControl()
        {
            InitializeComponent();
        }

        public void setupTeamControl(Form1 parent, TeamInfo info, MetaContext context)
        {
            //TODO: Make base layout more configurable for sizes
            this.parent = parent;
            this.teamInfo = info;
            this.context = context;
            TimerLabel.Size = new System.Drawing.Size(context.layout.timerWidth, context.layout.timerHeight);
            TeamSplitButton.BackColor = teamInfo.color;
            if (teamInfo.color.GetBrightness() < 0.5f)
            {
                TeamSplitButton.ForeColor = Color.White;
                cycleIconButton.ForeColor = Color.White;
            }
            cycleIconButton.Location = new Point(20 + context.layout.timerWidth, 8);
            cycleIconButton.BackColor = teamInfo.color;
            TeamSplitButton.Location = new Point(8, 16 + context.layout.timerHeight);
            TeamSplitButton.Size = new Size(Math.Max(context.layout.timerWidth + 72, 408), 54);
            teamTabGroup.Location = new Point(8, 80 + context.layout.timerHeight);

            //TODO: Only contruct tabs if they're setup in the layout config?
            //Potentially just have everything and only cycle the target ones. Might be easier.
            foreach (TabPage page in teamTabGroup.TabPages)
            {
                page.BackgroundImage = teamInfo.tabBackground;
                page.Size = new Size(context.layout.boxWidth, context.layout.boxHeight);
            }

            teamTabGroup.Selected += teamTabGroup_Selected;

            //Construct splits tab
            teamSplitTimes = new Label[context.splitsToShow];
            teamSplitNames = new Label[context.splitsToShow];
            int splitLabelHeight = (context.layout.boxHeight - (2 * context.layout.boxMargin)) / (context.splitsToShow + context.numberOfTeams);
            for (int i = 0; i < context.splitsToShow; i++)
            {
                teamSplitNames[i] = Util.createBaseLabel(3, splitLabelHeight * i + context.layout.boxMargin, 256, splitLabelHeight, "test+" + i);
                tabPageSplits.Controls.Add(teamSplitNames[i]);
                teamSplitTimes[i] = Util.createBaseLabel(265, splitLabelHeight * i + context.layout.boxMargin, 117, splitLabelHeight, "00:00:00");
                tabPageSplits.Controls.Add(teamSplitTimes[i]);
            }
            vsLabelNames = new Label[context.numberOfTeams - 1];
            vsLabelTimes = new Label[context.numberOfTeams - 1];
            int adjustedIndex = 0;
            for (int i = 0; i < context.numberOfTeams; i++)
            {
                if (context.teamNames[i].Equals(info.teamName)) { continue; }
                int height = adjustedIndex + context.splitsToShow + 1;
                vsLabelNames[adjustedIndex] = Util.createBaseLabel(3, splitLabelHeight * height + context.layout.boxMargin, 230, splitLabelHeight, "Vs Team " + context.teamNames[i]);
                tabPageSplits.Controls.Add(vsLabelNames[adjustedIndex]);
                vsLabelTimes[adjustedIndex] = Util.createBaseLabel(265, splitLabelHeight * height + context.layout.boxMargin, 140, splitLabelHeight, "00:00:00");
                tabPageSplits.Controls.Add(vsLabelTimes[adjustedIndex]);
                adjustedIndex++;
            }

            //Construct runner/category/commentary tab
            categoryLabels = new Label[3];
            int categoryHeight = (context.layout.boxHeight-context.layout.boxMargin) / 6;
            for (int i = 0; i < 3; i++)
            {
                categoryLabels[i] = Util.createBaseLabel(3, 3 + categoryHeight * i, 391, categoryHeight, "test+" + i, ContentAlignment.MiddleCenter);
                tabPageCategories.Controls.Add(categoryLabels[i]);
            }
            commentaryLabel = Util.createBaseLabel(3, context.layout.boxHeight / 2, 391, context.layout.boxHeight / 2, "Commentators: ", ContentAlignment.MiddleCenter);
            tabPageCategories.Controls.Add(commentaryLabel);

            //Construct game times tab
            //TODO: Need to solve/configure the middle case for an odd number of games. Right now it just appends that to the left side.
            int gamesOnEach = (context.numberOfGames + 1) / 2;
            gameEndsL = new Label[gamesOnEach];
            gameEndsR = new Label[context.numberOfGames - gamesOnEach];
            gameShortL = new Label[gamesOnEach];
            gameShortR = new Label[context.numberOfGames - gamesOnEach];
            int offset = tabPageTimes.Height / gamesOnEach;

            for (int i = 0; i < gamesOnEach; i++)
            {
                gameEndsL[i] = Util.createBaseLabel(76, offset * i, 117, offset, "00:00:00", ContentAlignment.MiddleCenter);
                tabPageTimes.Controls.Add(gameEndsL[i]);
                gameEndsR[i] = Util.createBaseLabel(200, offset * i, 117, offset, "00:00:00", ContentAlignment.MiddleCenter);
                tabPageTimes.Controls.Add(gameEndsR[i]);
                gameShortL[i] = Util.createBaseLabel(3, offset * i, 90, offset, context.games[i].PadLeft(4, ' ') + ": ", ContentAlignment.MiddleRight);
                tabPageTimes.Controls.Add(gameShortL[i]);
                gameShortR[i] = Util.createBaseLabel(295, offset * i, 90, offset, " :" + context.games[i + gamesOnEach].PadRight(4, ' '), ContentAlignment.MiddleLeft);
                tabPageTimes.Controls.Add(gameShortR[i]);
            }

            updateSplits(new VersusWrapper[] { });
            updateButtonText();
        }

        private void teamTabGroup_Selected(object sender, TabControlEventArgs e) => parent.childTabChanged();
        

        public void TeamSplitButton_Click(object sender, EventArgs e) => splitClick();
        

        public void updateTimerEvent(string current, bool cycleInfo)
        {
            if (!teamInfo.teamFinished)
            {
                teamInfo.teamSplits[teamInfo.teamSplitNum] = current;
                updateSplits(parent.fetchOtherTeamInfo(this));
            }
            else
            {
                updateVsSplits(parent.fetchOtherTeamInfo(this));
            }
            if (cycleInfo)
            {
                if (teamTabGroup.SelectedIndex == 2)
                { teamTabGroup.SelectedIndex = 0; }
                else { teamTabGroup.SelectedIndex++; }
            }
        }

        public string getSplit(int i) => teamInfo.teamSplitNum > i ? teamInfo.teamSplits[i] : Util.emptyTime;

        public void setSplit(string split, int i)
        {
            if (teamInfo.teamSplitNum > i)
            {
                teamInfo.teamSplits[i] = split;
            }
        }

        private void splitClick()
        {
            //Activate Cooldown
            if (!teamInfo.teamWaiting)
            {
                if (teamInfo.teamSplitNum >= context.splits.Length - 1)
                {
                    if (!teamInfo.teamFinished)
                    {
                        teamInfo.teamFinish = teamInfo.teamSplits[teamInfo.teamSplitNum];
                        teamInfo.teamGameEnd[teamInfo.teamGame] = teamInfo.teamFinish;
                        teamSplitTimes[context.splitsToShow - 1].Text = teamInfo.teamFinish;
                        teamInfo.teamFinished = true;
                    }
                    return;
                }
                //Handle the splits. Showing 3 at a time, need to cycle games on end splits (Contains "Final Fantasy")
                //This year we need to catch LR and MQ. If we do "Lightning Returns: Final Fantasy XIII" it's too damn long, so we'll cut at LR
                //Catch that we're ending a game before we move onto the next one
                if (context.splits[teamInfo.teamSplitNum].Contains(Util.gameSep))
                {
                    //Assign the per-game timer to be our current split time, which is stored in teamSplits[splitNum]
                    teamInfo.teamGameEnd[teamInfo.teamGame] = teamInfo.teamSplits[teamInfo.teamSplitNum];
                    //Move the current game along for tracking
                    teamInfo.teamGame++;
                    //Move onto the next game using the hand / icons
                    cycleIcon();
                }
                teamInfo.teamSplitNum++;
                //updateSplits(parent.fetchOtherTeamInfo(this));
                teamInfo.teamCooldown.Enabled = true;
                teamInfo.teamCooldown.Interval = context.layout.splitButtonCooldown;
                teamInfo.teamCooldown.Start();
                teamInfo.teamCooldown.Tick += new EventHandler((o, e) => { teamInfo.teamWaiting = false; teamInfo.teamCooldown.Stop(); TeamSplitButton.BackColor = teamInfo.color; });
                teamInfo.teamWaiting = true;
                TeamSplitButton.BackColor = Color.Gray;
                parent.WriteSplitFiles();
            }
        }

        private void updateButtonText() => cycleIconButton.Text = "Update " + teamInfo.teamName + " Icon\n Cur: " + teamInfo.teamIcon;

        private void cycleIcon()
        {
            teamInfo.teamIcon++;
            teamInfo.cycleTeamIcon(updateButtonText);
            reloadCategoryTab();
        }

        private void reloadCategoryTab()
        {
            categoryLabels[0].Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 4];
            categoryLabels[1].Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 3];
            categoryLabels[2].Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 2];
            commentaryLabel.Text = "Commentary: " + context.commentators[teamInfo.teamIcon - 1];
        }

        public void reloadRunnerInfo()
        {
            teamInfo.reloadRunnerInfo();
            reloadCategoryTab();
        }

        private void updateVsSplits(VersusWrapper[] otherTeams)
        {
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
                            int notFinished = 1;
                            if (teamInfo.teamSplitNum == context.splits.Length - 1 && (teamInfo.teamFinished || otherTeams[team].finished))
                            {
                                notFinished = 0;
                            }
                            TimeSpan seg = Util.resolveTimeSpan(teamInfo.teamSplits[teamInfo.teamSplitNum - notFinished],
                                otherTeams[team].splits[teamInfo.teamSplitNum - notFinished]);
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
                            if ((Math.Abs(olderseg.Ticks) > Math.Abs(seg.Ticks)) && (Math.Sign(olderseg.Ticks) == Math.Sign(seg.Ticks)))
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
        }

        private void updateSplits(VersusWrapper[] otherTeams)
        {

            int i = Util.clamp(teamInfo.teamSplitNum, context.splits.Length - (context.splitsToShow - context.splitFocusOffset), context.splitFocusOffset);
            for (int offsetSplit = 0; offsetSplit < context.splitsToShow; offsetSplit++)
            {
                teamSplitNames[offsetSplit].Text = Util.stripGameIndicator(context.splits[i - (context.splitFocusOffset - offsetSplit)]);
                teamSplitTimes[offsetSplit].Text = teamInfo.teamSplits[i - (context.splitFocusOffset - offsetSplit)];
            }

            updateVsSplits(otherTeams);

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            teamInfo.teamGameEndArchive = teamInfo.teamGameEnd;
            if (teamInfo.teamGame == 0)
            {
                gameEndsL[0].Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                gameEndsR[0].Text = "00:00:00";
                for (int linesToFill = 1; linesToFill < context.numberOfGames / 2; linesToFill++)
                {
                    gameEndsL[linesToFill].Text = "00:00:00";
                    gameEndsR[linesToFill].Text = "00:00:00";
                }
                TimerLabel.Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                return;
            }
            gameEndsL[0].Text = teamInfo.teamGameEnd[0];
            int gamesOnEach = (context.numberOfGames + 1) / 2;
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
                if (j < gamesOnEach)
                { gameEndsL[j].Text = current; }
                else
                { gameEndsR[j - gamesOnEach].Text = current; }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cycleIcon();
        }

    }
}
