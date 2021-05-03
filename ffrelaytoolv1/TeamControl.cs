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

        MainForm parent;

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

        public void setupTeamControl(MainForm parent, TeamInfo info, MetaContext context, Size teamSize)
        {
            //TODO: Make base layout more configurable for sizes
            this.parent = parent;
            this.teamInfo = info;
            this.context = context;
            this.Size = teamSize;

            TimerLabel.Size = new Size(context.layout.timerWidth, context.layout.timerHeight);
            TeamSplitButton.BackColor = teamInfo.color;
            if (teamInfo.color.GetBrightness() < 0.5f)
            {
                TeamSplitButton.ForeColor = Color.White;
                cycleIconButton.ForeColor = Color.White;
            }
            cycleIconButton.Location = new Point(20 + context.layout.timerWidth, 8);
            cycleIconButton.BackColor = teamInfo.color;
            TeamSplitButton.Location = new Point(160 + context.layout.timerWidth, 8);
            TeamSplitButton.Size = new Size(Math.Max(context.layout.timerWidth + 72, 408), context.layout.timerHeight);
            TeamSplitButton.Text = "Team " + teamInfo.teamName + " Split";
            
            teamTabGroup.Location = new Point(8, 20 + context.layout.timerHeight);
            teamTabGroup.Size = new Size(context.layout.boxWidth+8, context.layout.boxHeight+26);
            teamTabGroup.Selected += teamTabGroup_Selected;

            //TODO: Only contruct tabs if they're setup in the layout config?
            int tabCount = 0;
            //Potentially just have everything and only cycle the target ones. Might be easier.

            if (context.features.showSplits)
            {
                //Construct splits tab
                teamTabGroup.Controls.Add(createSplitsPage(context, info, ++tabCount));
            }

            if (context.features.showRunners)
            {
                //Construct runner/category/commentary tab
                teamTabGroup.Controls.Add(createCommentaryPage(context, info, ++tabCount));
            }

            if (context.features.showGameTimes)
            {
                //Construct game times tab
                //TODO: Need to solve/configure the middle case for an odd number of games. Right now it just appends that to the left side.
                teamTabGroup.Controls.Add(createTimesPage(context, info, ++tabCount));
            }

            updateSplits(new VersusWrapper[] { });
            updateButtonText();
            teamInfo.cycleTeamIcon(()=> { });
        }

        private TabPage createSplitsPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageSplits = new TabPage()
            {
                BackColor = System.Drawing.Color.Black,
                Location = new System.Drawing.Point(4, 22),
                Name = "tabPageSplits",
                Padding = new System.Windows.Forms.Padding(3),
                BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight),
                TabIndex = tabCounter,
                Text = "Splits",
            };
            teamSplitTimes = new Label[context.splitsToShow];
            teamSplitNames = new Label[context.splitsToShow];
            int splitLabelWidth = (context.layout.boxWidth - (2 * context.layout.boxMargin)) / context.splitsToShow;
            int haMargin = context.layout.boxMargin / 2;
            for (int i = 0; i < context.splitsToShow; i++)
            {
                teamSplitNames[i] = Util.createBaseLabel(splitLabelWidth * i + context.layout.boxMargin, context.layout.boxHeight / 2, splitLabelWidth, context.layout.boxHeight / 2 - context.layout.boxMargin, "test+" + i, ContentAlignment.MiddleCenter);
                tabPageSplits.Controls.Add(teamSplitNames[i]);
                teamSplitTimes[i] = Util.createBaseLabel(splitLabelWidth * i + context.layout.boxMargin, haMargin, splitLabelWidth, context.layout.boxHeight / 2 - context.layout.boxMargin, "00:00:00", ContentAlignment.MiddleCenter);
                tabPageSplits.Controls.Add(teamSplitTimes[i]);
            }
            return tabPageSplits;
        }

        private TabPage createCommentaryPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageCategories = new TabPage()
            {
                BackColor = System.Drawing.Color.Black,
                Location = new System.Drawing.Point(4, 22),
                Name = "tabPageCategories",
                Padding = new System.Windows.Forms.Padding(3),
                BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight),
                TabIndex = tabCounter,
                Text = "Category & Runner",
            };
            categoryLabels = new Label[3];
            int categoryHeight = (context.layout.boxHeight - context.layout.boxMargin) / 6;
            for (int i = 0; i < 3; i++)
            {
                categoryLabels[i] = Util.createBaseLabel(3, 3 + categoryHeight * i, context.layout.boxWidth / 2, categoryHeight, "test+" + i);
                tabPageCategories.Controls.Add(categoryLabels[i]);
            }
            commentaryLabel = Util.createBaseLabel(3, context.layout.boxHeight / 2, context.layout.boxWidth, context.layout.boxHeight / 2, "Commentators: ");
            tabPageCategories.Controls.Add(commentaryLabel);

            vsLabelNames = new Label[context.numberOfTeams - 1];
            vsLabelTimes = new Label[context.numberOfTeams - 1];
            int adjustedIndex = 0;
            int comparisonHeight = context.layout.boxHeight / (2 * (context.numberOfTeams - 1));
            for (int i = 0; i < context.numberOfTeams; i++)
            {
                if (context.teamNames[i].Equals(info.teamName)) { continue; }
                vsLabelNames[adjustedIndex] = Util.createBaseLabel(context.layout.boxWidth / 2, comparisonHeight * adjustedIndex + 3, context.layout.boxWidth / 4, comparisonHeight, "Vs Team " + context.teamNames[i]);
                tabPageCategories.Controls.Add(vsLabelNames[adjustedIndex]);
                vsLabelTimes[adjustedIndex] = Util.createBaseLabel(context.layout.boxWidth * 3 / 4, comparisonHeight * adjustedIndex + 3, context.layout.boxWidth / 4, comparisonHeight, " 00:00:00");
                tabPageCategories.Controls.Add(vsLabelTimes[adjustedIndex]);
                adjustedIndex++;
            }
            return tabPageCategories;
        }

        private TabPage createTimesPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageTimes = new TabPage()
            {
                BackColor = System.Drawing.Color.Black,
                Location = new System.Drawing.Point(4, 22),
                Name = "tabPageTimes",
                Padding = new System.Windows.Forms.Padding(3),
                BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight), 
                TabIndex = tabCounter,
                Text = "Game Times",
            };
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
            return tabPageTimes;
        }

        private void teamTabGroup_Selected(object sender, TabControlEventArgs e) => parent.childTabChanged();
        

        public void TeamSplitButton_Click(object sender, EventArgs e) => splitClick();
        

        public int updateTimerEvent(string current, bool cycleInfo, int targetTab=-1)
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
                if (targetTab > -1)
                {
                    teamTabGroup.SelectedIndex = Util.clamp(targetTab, teamTabGroup.TabCount, 0);
                } else if (teamTabGroup.SelectedIndex == teamTabGroup.TabCount - 1)
                { teamTabGroup.SelectedIndex = 0; }
                else { teamTabGroup.SelectedIndex++; }
            }
            return teamTabGroup.SelectedIndex;
        }

        public string getSplit(int i) => teamInfo.teamSplitNum > i ? teamInfo.teamSplits[i] : Util.emptyTime;

        public void setSplit(string split, int i)
        {
            if (teamInfo.teamSplitNum > i)
            {
                teamInfo.teamSplits[i] = split;
            }
        }

        public void splitClick()
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
            if (context.features.teamGameIcons)
            {
                teamInfo.teamIcon++;
                teamInfo.cycleTeamIcon(updateButtonText);
            }
            parent.cycleMainBG();
            reloadCategoryTab();
        }

        private void reloadCategoryTab()
        {
            if (context.features.showRunners) { 
                categoryLabels[0].Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 4];
                categoryLabels[1].Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 3];
                categoryLabels[2].Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 2];
                commentaryLabel.Text = "Commentary: " + context.commentators[teamInfo.teamIcon - 1];
            }
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
                            if (context.features.showSplits)
                            {
                                Util.updateDifferenceDisplay(vsLabelTimes[team], seg);
                            }
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
                        if (context.features.showSplits)
                        {
                            Util.updateDifferenceDisplay(vsLabelTimes[team], seg);
                        }
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
            if (context.features.showSplits) { 
                for (int offsetSplit = 0; offsetSplit < context.splitsToShow; offsetSplit++)
                {
                    teamSplitNames[offsetSplit].Text = Util.stripGameIndicator(context.splits[i - (context.splitFocusOffset - offsetSplit)]);
                    teamSplitTimes[offsetSplit].Text = teamInfo.teamSplits[i - (context.splitFocusOffset - offsetSplit)];
                }
            }

            updateVsSplits(otherTeams);

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            teamInfo.teamGameEndArchive = teamInfo.teamGameEnd;
            if (teamInfo.teamGame == 0)
            {
                updateGameEndsL(0, teamInfo.teamSplits[teamInfo.teamSplitNum]);
                updateGameEndsR(0, "00:00:00");
                for (int linesToFill = 1; linesToFill < context.numberOfGames / 2; linesToFill++)
                {
                    updateGameEndsL(linesToFill, "00:00:00");
                    updateGameEndsL(linesToFill, "00:00:00");
                }
                TimerLabel.Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                return;
            }
            updateGameEndsL(0, teamInfo.teamGameEnd[0]);
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
                { updateGameEndsL(j, current); }
                else
                { updateGameEndsR(j - gamesOnEach, current); }
            }
        }

        private void updateGameEndsL(int index, String text)
        {
            if (context.features.showGameTimes)
            {
                gameEndsL[index].Text = text;
            }
        }

        private void updateGameEndsR(int index, String text)
        {
            if (context.features.showGameTimes)
            {
                gameEndsR[index].Text = text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cycleIcon();
        }

        public List<String> outputCaptureInfo(Control parent)
        {
            List<String> captureLines = new List<string>();
            captureLines.Add("");
            captureLines.Add("Team " + teamInfo.teamName);
            captureLines.Add("Timer: ");
            captureLines.Add(Util.outputCaptureInfoRelative(TimerLabel, parent, this));
            captureLines.Add("Info box: ");
            captureLines.Add(Util.outputCaptureInfoRelative(teamTabGroup.TabPages[0], parent, teamTabGroup, this));
            return captureLines;
        }

    }
}
