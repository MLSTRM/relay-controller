using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ffrelaytoolv1.MetaFile;

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
        Label vsLabelSingle;

        Label[] gameEndsL;
        Label[] gameEndsR;
        Label[] gameShortL;
        Label[] gameShortR;

        //TODO: refactor into <Game, Category, Runner> - Runner then embeds a RichNameControl also
        Label gameLabel;
        Label categoryLabel;
        Label runnerLabel;
        RichNameControl runnerRichNameLabel;
        Label commentaryLabel;
        FlowLayoutPanel commentaryLayoutPanel;
        Label commentaryHeader;

        LabelUtil labelUtil;

        public TeamControl()
        {
            InitializeComponent();
        }

        public void setupTeamControl(MainForm parent, TeamInfo info, MetaContext context, Size teamSize, LabelUtil labelUtil)
        {
            this.labelUtil = labelUtil;
            //TODO: Make base layout more configurable for sizes
            this.parent = parent;
            this.teamInfo = info;
            this.context = context;
            this.Size = teamSize;

            TimerLabel.Size = new Size(context.layout.timerWidth, context.layout.timerHeight);
            TimerLabel.BackColor = info.color;
            TimerLabel.ForeColor = ColorTranslator.FromHtml(context.layout.timerTextColor);
            TimerLabel.Font = labelUtil.activeTimerFontSized(context.layout.teamTimerFontSize);

            TeamSplitButton.BackColor = teamInfo.color;
            if (teamInfo.color.GetBrightness() < 0.5f)
            {
                TeamSplitButton.ForeColor = Color.White;
                cycleIconButton.ForeColor = Color.White;
            }
            cycleIconButton.Location = new Point(20 + context.layout.timerWidth, 8);
            cycleIconButton.BackColor = teamInfo.color;
            var timerBlockWidth = cycleIconButton.Location.X + 64;
            int splitButtonsY = 8;
            int splitButtonsX = cycleIconButton.Location.X + 80;
            int tabGroupY = 20 + context.layout.timerHeight;
            if (Size.Width < 2 * timerBlockWidth)
            {
                splitButtonsY += context.layout.timerHeight + 4;
                splitButtonsX = 12;
                tabGroupY += context.layout.timerHeight + 4;
            }
            TeamSplitButton.Location = new Point(splitButtonsX, splitButtonsY);
            TeamSplitButton.Size = new Size(context.layout.timerWidth, context.layout.timerHeight);
            TeamSplitButton.Text = "Team " + teamInfo.teamName + " Split";
            undoButton.Location = new Point(TeamSplitButton.Size.Width + 8 + TeamSplitButton.Location.X, TeamSplitButton.Location.Y);
            undoButton.Size = new Size(64, context.layout.timerHeight);
            undoButton.Text = "Undo";
            
            teamTabGroup.Location = new Point(8, tabGroupY);
            teamTabGroup.Size = new Size(context.layout.boxWidth, context.layout.boxHeight + 26);
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

            if (context.features.showGraph)
            {
                teamTabGroup.Controls.Add(createGraphPage(context, info, ++tabCount));
            }

            updateSplits(new VersusWrapper[] { });
            updateButtonText();
            teamInfo.cycleTeamIcon(() => { });
        }

        private TabPage createSplitsPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageSplits = new TabPage()
            {
                BackColor = info.color,
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
            
            int haMargin = context.layout.boxMargin / 2;
            for (int i = 0; i < context.splitsToShow; i++)
            {
                //horizontal mode: labels are written across the page using the rowHeight as separation
                if (context.layout.horizontalDisplay)
                {
                    int splitLabelWidth = (context.layout.boxWidth - (2 * context.layout.boxMargin)) / context.splitsToShow;
                    if (context.layout.splitLabelsOnSplitsPage)
                    {
                        teamSplitNames[i] = labelUtil.createBaseLabel(splitLabelWidth * i + context.layout.boxMargin, context.layout.rowHeight, splitLabelWidth, context.layout.rowHeight - context.layout.boxMargin, "test+" + i, ContentAlignment.MiddleCenter);
                        tabPageSplits.Controls.Add(teamSplitNames[i]);
                    }
                    teamSplitTimes[i] = labelUtil.createBaseLabel(splitLabelWidth * i + context.layout.boxMargin, haMargin, splitLabelWidth, context.layout.rowHeight - context.layout.boxMargin, "00:00:00", ContentAlignment.MiddleCenter, 20);
                    tabPageSplits.Controls.Add(teamSplitTimes[i]);
                } else
                {
                    //vertical mode: labels are written down the page using the rowWidth as separation
                    //can be left or right aligned.
                    int splitLabelHeight = (context.layout.boxHeight - (2 * context.layout.boxMargin)) / context.splitsToShow;
                    if (teamInfo.leftAlign)
                    {
                        if (context.layout.splitLabelsOnSplitsPage)
                        {
                            teamSplitNames[i] = labelUtil.createBaseLabel(context.layout.rowWidth, splitLabelHeight * i + context.layout.boxMargin, context.layout.rowWidth - context.layout.boxMargin, splitLabelHeight, "test+" + i, ContentAlignment.MiddleCenter);
                            tabPageSplits.Controls.Add(teamSplitNames[i]);
                        }
                        teamSplitTimes[i] = labelUtil.createBaseLabel(haMargin, splitLabelHeight * i + context.layout.boxMargin, context.layout.rowWidth - context.layout.boxMargin, splitLabelHeight, "00:00:00", ContentAlignment.MiddleCenter, 20);
                        tabPageSplits.Controls.Add(teamSplitTimes[i]);
                    } else
                    {
                        int w = context.layout.rowWidth - context.layout.boxMargin;
                        if (context.layout.splitLabelsOnSplitsPage)
                        {
                            teamSplitNames[i] = labelUtil.createBaseLabel(context.layout.boxWidth - context.layout.rowWidth - w, splitLabelHeight * i + context.layout.boxMargin, w, splitLabelHeight, "test+" + i, ContentAlignment.MiddleCenter);
                            tabPageSplits.Controls.Add(teamSplitNames[i]);
                        }
                        teamSplitTimes[i] = labelUtil.createBaseLabel(context.layout.boxWidth - w - haMargin, splitLabelHeight * i + context.layout.boxMargin, w, splitLabelHeight, "00:00:00", ContentAlignment.MiddleCenter, 20);
                        tabPageSplits.Controls.Add(teamSplitTimes[i]);
                    }
                }
            }
            if (context.features.vsLabelsOnSplitsPage)
            {
                if (context.features.showAllVs)
                {
                    vsLabelNames = new Label[context.numberOfTeams - 1];
                    vsLabelTimes = new Label[context.numberOfTeams - 1];
                    int adjustedIndex = 0;
                    int comparisonHeight = context.layout.boxHeight / (2 * (context.numberOfTeams - 1));
                    for (int i = 0; i < context.numberOfTeams; i++)
                    {
                        if (context.teamNames[i].Equals(info.teamName)) { continue; }
                        vsLabelNames[adjustedIndex] = labelUtil.createBaseLabel(context.layout.boxWidth / 2, comparisonHeight * adjustedIndex + 3, context.layout.boxWidth / 4, comparisonHeight, "Vs Team " + context.teamNames[i]);
                        tabPageSplits.Controls.Add(vsLabelNames[adjustedIndex]);
                        vsLabelTimes[adjustedIndex] = labelUtil.createBaseLabel(context.layout.boxWidth * 3 / 4, comparisonHeight * adjustedIndex + 3, context.layout.boxWidth / 4, comparisonHeight, " 00:00:00");
                        tabPageSplits.Controls.Add(vsLabelTimes[adjustedIndex]);
                        adjustedIndex++;
                    }
                }
                else
                {
                    vsLabelSingle = labelUtil.createBaseLabel(context.layout.boxWidth / 2, context.layout.rowHeight / 6, context.layout.boxWidth / 2, context.layout.rowHeight, "");
                    tabPageSplits.Controls.Add(vsLabelSingle);
                }
            }
            return tabPageSplits;
        }

        private TabPage createCommentaryPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageCategories = new TabPage()
            {
                BackColor = info.color,
                Location = new Point(4, 22),
                Name = "tabPageCategories",
                Padding = new Padding(3),
                BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight),
                TabIndex = tabCounter,
                Text = "Category & Runner",
            };
            int categoryHeight = (context.layout.rowHeight - context.layout.boxMargin) / 3;
            var runnerTextWidth = TextRenderer.MeasureText("Runner:", labelUtil.activeFontSized(context.layout.defaultTimerFontSize));
            if (teamInfo.leftAlign)
            {
                gameLabel = labelUtil.createBaseLabel(3, 3, (context.layout.boxWidth * 2) / 3, categoryHeight, "Final Fantasy");
                categoryLabel = labelUtil.createBaseLabel(3, 3 + categoryHeight, (context.layout.boxWidth * 2) / 3, categoryHeight, "test+1");
                runnerLabel = labelUtil.createBaseLabel(3, 3 + categoryHeight * 2, (context.layout.boxWidth * 2) / 3, categoryHeight, "Runner: ");
                tabPageCategories.Controls.Add(gameLabel);
                tabPageCategories.Controls.Add(categoryLabel);
                tabPageCategories.Controls.Add(runnerLabel);
                runnerRichNameLabel = new RichNameControl
                {
                    Location = new Point(3 + runnerTextWidth.Width + 3, 6 + categoryHeight * 2),
                    BackColor = Color.Transparent
                };
                if (!context.layout.useBasicNameLayout)
                {
                    runnerLabel.Width = runnerTextWidth.Width;
                    tabPageCategories.Controls.Add(runnerRichNameLabel);
                }
                if (context.features.commentatorsOnRunnerPage)
                {
                    commentaryHeader = labelUtil.createBaseLabel(3, context.layout.rowHeight, 160, context.layout.rowHeight, "Commentary:", ContentAlignment.MiddleLeft, labelUtil.defaultColour, context.layout.defaultTimerFontSize);
                    tabPageCategories.Controls.Add(commentaryHeader);
                    commentaryLabel = labelUtil.createBaseLabel(3 + commentaryHeader.Width + 3, context.layout.rowHeight, context.layout.boxWidth - commentaryHeader.Width - 3, context.layout.rowHeight, "", ContentAlignment.MiddleLeft);
                    commentaryLayoutPanel = new FlowLayoutPanel
                    {
                        Location = new Point(3 + commentaryHeader.Width, context.layout.rowHeight),
                        Size = new Size(context.layout.boxWidth - commentaryHeader.Width - 3, context.layout.rowHeight),
                        FlowDirection = FlowDirection.LeftToRight,
                        Padding = new Padding(3),
                        Margin = new Padding(0),
                    };
                    if (context.layout.useBasicNameLayout)
                    {
                        tabPageCategories.Controls.Add(commentaryLabel);
                    }
                    else
                    {
                        tabPageCategories.Controls.Add(commentaryLayoutPanel);
                    }
                }
            } else
            {
                int categoryW = (context.layout.boxWidth * 2) / 3;
                gameLabel = labelUtil.createBaseLabel(context.layout.boxWidth - categoryW - 3, 3, (context.layout.boxWidth * 2) / 3, categoryHeight, "test+0", ContentAlignment.MiddleRight);
                categoryLabel = labelUtil.createBaseLabel(context.layout.boxWidth - categoryW - 3, 3 + categoryHeight, (context.layout.boxWidth * 2) / 3, categoryHeight, "test+1", ContentAlignment.MiddleRight);
                runnerLabel = labelUtil.createBaseLabel(context.layout.boxWidth - categoryW - 3, 3 + categoryHeight * 2, (context.layout.boxWidth * 2) / 3, categoryHeight, "test+" + 2, ContentAlignment.MiddleRight);
                tabPageCategories.Controls.Add(gameLabel);
                tabPageCategories.Controls.Add(categoryLabel);
                tabPageCategories.Controls.Add(runnerLabel);
                runnerRichNameLabel = new RichNameControl
                {
                    Location = new Point(context.layout.boxWidth - categoryW - 3 + runnerTextWidth.Width, 6 + categoryHeight * 2),
                    BackColor = Color.Transparent
                };
                if (!context.layout.useBasicNameLayout)
                {
                    runnerLabel.Width = runnerTextWidth.Width;
                    tabPageCategories.Controls.Add(runnerRichNameLabel);
                }
                if (context.features.commentatorsOnRunnerPage)
                {
                    commentaryHeader = labelUtil.createBaseLabel(context.layout.boxWidth - 160 - 3, context.layout.rowHeight, 160, context.layout.rowHeight, ":Commentary", ContentAlignment.MiddleRight, labelUtil.defaultColour, context.layout.defaultTimerFontSize);
                    tabPageCategories.Controls.Add(commentaryHeader);
                    int commentaryW = context.layout.boxWidth - commentaryHeader.Width - 3;
                    commentaryLabel = labelUtil.createBaseLabel(context.layout.boxWidth - commentaryW - (3 + commentaryHeader.Width + 3), context.layout.rowHeight, commentaryW, context.layout.rowHeight, "", ContentAlignment.MiddleRight);
                    commentaryLayoutPanel = new FlowLayoutPanel
                    {
                        Location = new Point(context.layout.boxWidth - commentaryW - (3 + commentaryHeader.Width + 3), context.layout.rowHeight),
                        Size = new Size(commentaryW, context.layout.rowHeight),
                        FlowDirection = FlowDirection.LeftToRight,
                        Padding = new Padding(3),
                        Margin = new Padding(0),
                    };
                    if (context.layout.useBasicNameLayout)
                    {
                        tabPageCategories.Controls.Add(commentaryLabel);
                    } else
                    {
                        tabPageCategories.Controls.Add(commentaryLayoutPanel);
                    }

                }
            }
            if (!context.features.vsLabelsOnSplitsPage)
            {
                var labelSize = labelUtil.calcLabelSize(20, "-00:00:00");
                var width = Math.Max(context.layout.vsLabelMin, labelSize.Width);
                var vsLabelX = teamInfo.leftAlign ? context.layout.boxWidth / 2 : context.layout.boxWidth / 4;
                var vsLabelTimeX = teamInfo.leftAlign ? context.layout.boxWidth - width - context.layout.boxMargin : context.layout.boxMargin;
                var vsLabelTimeAlign = teamInfo.leftAlign ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                var vsLabelAlign = teamInfo.leftAlign ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
                if (context.features.showAllVs)
                {
                    vsLabelNames = new Label[context.numberOfTeams - 1];
                    vsLabelTimes = new Label[context.numberOfTeams - 1];
                    int adjustedIndex = 0;
                    int comparisonHeight = context.layout.boxHeight / (2 * (context.numberOfTeams - 1));
                    for (int i = 0; i < context.numberOfTeams; i++)
                    {
                        if (context.teamNames[i].Equals(info.teamName)) { continue; }
                        vsLabelNames[adjustedIndex] = labelUtil.createBaseLabel(vsLabelX, comparisonHeight * adjustedIndex + 3, context.layout.boxWidth / 4, comparisonHeight, "Vs Team " + context.teamNames[i], vsLabelAlign);
                        tabPageCategories.Controls.Add(vsLabelNames[adjustedIndex]);

                        vsLabelTimes[adjustedIndex] = labelUtil.createBaseLabel(vsLabelTimeX, comparisonHeight * adjustedIndex + 3, width, comparisonHeight, " 00:00:00", vsLabelTimeAlign);
                        tabPageCategories.Controls.Add(vsLabelTimes[adjustedIndex]);
                        adjustedIndex++;
                    }
                }
                else
                {
                    vsLabelSingle = labelUtil.createBaseLabel(vsLabelTimeX, (int)(categoryHeight*context.layout.vsLabelRow), width, categoryHeight, "", vsLabelTimeAlign, context.layout.splitTimerFontSize);
                    tabPageCategories.Controls.Add(vsLabelSingle);
                    vsLabelSingle.BringToFront();
                }
            }
            return tabPageCategories;
        }

        private TabPage createTimesPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageTimes = new TabPage()
            {
                BackColor = info.color,
                Location = new System.Drawing.Point(4, 22),
                Name = "tabPageTimes",
                Padding = new System.Windows.Forms.Padding(3),
                BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight), 
                TabIndex = tabCounter,
                Text = "Game Times",
            };

            //TODO: make this more width-reactive and give more possible layout options
            if (context.layout.gameTimeLayout == 0)
            {
                int gamesOnEach = (context.numberOfGames + 1) / 2;
                gameEndsL = new Label[gamesOnEach];
                gameEndsR = new Label[context.numberOfGames - gamesOnEach];
                gameShortL = new Label[gamesOnEach];
                gameShortR = new Label[context.numberOfGames - gamesOnEach];
                int offset = tabPageTimes.Height / gamesOnEach;

                for (int i = 0; i < gamesOnEach; i++)
                {
                    // box center line - ~60
                    gameEndsL[i] = labelUtil.createBaseLabel(76, offset * i, 117, offset, Util.emptyTime, ContentAlignment.MiddleCenter);
                    tabPageTimes.Controls.Add(gameEndsL[i]);
                    // box center line + ~60
                    gameEndsR[i] = labelUtil.createBaseLabel(200, offset * i, 117, offset, Util.emptyTime, ContentAlignment.MiddleCenter);
                    tabPageTimes.Controls.Add(gameEndsR[i]);
                    // box centre line - ~95?
                    gameShortL[i] = labelUtil.createBaseLabel(3, offset * i, 90, offset, context.games[i].PadLeft(4, ' ') + ": ", ContentAlignment.MiddleRight);
                    tabPageTimes.Controls.Add(gameShortL[i]);
                    // box centre line + ~95?
                    gameShortR[i] = labelUtil.createBaseLabel(295, offset * i, 90, offset, " :" + context.games[i + gamesOnEach].PadRight(4, ' '), ContentAlignment.MiddleLeft);
                    tabPageTimes.Controls.Add(gameShortR[i]);
                }
            } else
            {
                throw new Exception("Unrecognised game time panel layout option");
            }
            return tabPageTimes;
        }

        private TabPage createGraphPage(MetaContext context, TeamInfo info, int tabCounter)
        {
            TabPage tabPageGraph = new TabPage()
            {
                BackColor = info.color,
                Location = new Point(4, 22),
                Name = "tabPageGraph",
                Padding = new Padding(3),
                BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight),
                TabIndex = tabCounter,
                Text = "Graph",
            };
            return tabPageGraph;
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
                {
                    teamTabGroup.SelectedIndex = 0;
                } else {
                    teamTabGroup.SelectedIndex++;
                }
            }
            return teamTabGroup.SelectedIndex;
        }

        public string getSplit(int i) => teamInfo.teamSplitNum > i ? teamInfo.teamSplits[i] : Util.emptyTime;

        public void setSplit(string split, int i, bool force = false)
        {
            if ((teamInfo.teamSplitNum > i || force) && split != teamInfo.teamSplits[i])
            {
                teamInfo.teamSplits[i] = split;
                if (context.splits[i].Contains(Util.gameSep))
                {
                    //find game index
                    // minus 2 because 1 due to 1-indexing on take, 1 so that we get the game BEFORE including the current separator
                    var gameidx = Util.getGameIndexForSplit(context.splits, i-2);
                    teamInfo.teamGameEnd[gameidx] = teamInfo.teamSplits[i];
                }
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
                        parent.checkAllTeamsFinished();
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
                    cycleIcon(1);
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

        public void undoSplitClick()
        {
            if(teamInfo.teamSplitNum == 0)
            {
                //Can't go into negative splits
                return;
            }
            if (teamInfo.teamFinished)
            {
                teamInfo.teamFinish = Util.emptyTime;
                teamInfo.teamGameEnd[teamInfo.teamGame] = Util.emptyTime;
                teamSplitTimes[context.splitsToShow - 1].Text = Util.emptyTime;
                teamInfo.teamFinished = false;
            }
            teamInfo.teamSplits[teamInfo.teamSplitNum] = Util.emptyTime;
            teamInfo.teamSplitNum--;
            if (context.splits[teamInfo.teamSplitNum].Contains(Util.gameSep))
            {
                teamInfo.teamGame--;
                teamInfo.teamGameEnd[teamInfo.teamGame] = Util.emptyTime;
                cycleIcon(-1);
            }
            if (teamInfo.teamWaiting)
            {
                //Shortcircuit the wait if we undid a split
                teamInfo.teamWaiting = false;
                teamInfo.teamCooldown.Stop();
                TeamSplitButton.BackColor = teamInfo.color;
            }
            parent.WriteSplitFiles();
        }

        private void updateButtonText() => cycleIconButton.Text = "Update " + teamInfo.teamName + " Icon\n Cur: " + teamInfo.teamIcon;

        private void cycleIcon(int amount)
        {
            var newIcon = teamInfo.teamIcon + amount;
            if (newIcon <= context.numberOfGames && newIcon > 0)
            {
                teamInfo.teamIcon = newIcon;
            }
            teamInfo.cycleTeamIcon(updateButtonText);
            parent.cycleMainBG();
            reloadCategoryTab();
        }

        private void reloadCategoryTab()
        {
            if (context.features.showRunners) { 
                gameLabel.Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 4];
                categoryLabel.Text = teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 3];
                if (!context.layout.useBasicNameLayout)
                {
                    runnerRichNameLabel.setupNameControl(labelUtil, UserDetailsUtils.parseUserFromDetailsString(teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 2]), context, false);
                } else
                {
                    runnerLabel.Text = "Runner: "+teamInfo.teamRunners[(teamInfo.teamIcon * 4) - 2];
                }
                if (context.features.commentatorsOnRunnerPage)
                {
                    commentaryLabel.Text = String.Join(", ", context.commentators[teamInfo.teamIcon - 1].Select(ud => ud.Name));
                    commentaryLayoutPanel.Controls.Clear();
                    commentaryLayoutPanel.Controls.AddRange(context.commentators[teamInfo.teamIcon - 1].Select(ud =>
                    {
                        var control = new RichNameControl();
                        control.setupNameControl(labelUtil, ud, context, true);
                        if (context.features.enableDiscordIntegration)
                        {
                            control.setSpeaking(false);
                        }
                        return control;
                    }).ToArray());
                }
            }
        }

        public void updateCommentatorSpeaking(IEnumerable<string> speaking, bool reset)
        {
            foreach(var control in commentaryLayoutPanel.Controls)
            {
                if(control is RichNameControl richName)
                {
                    if (reset)
                    {
                        richName.setSpeaking(true);
                    } else if(speaking.Contains(richName.getUserName()))
                    {
                        richName.setSpeaking(true);
                    } else
                    {
                        richName.setSpeaking(false);
                    }
                }
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
            TimeSpan maxSeg = new TimeSpan(0, 0, 0);
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
                                if (context.features.showAllVs)
                                { Util.updateDifferenceDisplay(vsLabelTimes[team], seg); }
                                else if (seg > maxSeg)
                                { maxSeg = seg; }
                            }
                        }
                    }
                    //Otherwise update the value based on the live difference (Only if greater than static difference?)
                    if (!vs[team] && otherTeams[team].splits[teamInfo.teamSplitNum - offset] != Util.emptyTime)
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
                            if (context.features.showAllVs)
                            { Util.updateDifferenceDisplay(vsLabelTimes[team], seg); }
                            else if (seg > maxSeg)
                            { maxSeg = seg; }
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
            if (!context.features.showAllVs)
            { Util.updatePositiveDifferenceDisplay(vsLabelSingle, maxSeg); }
        }

        private void updateSplits(VersusWrapper[] otherTeams)
        {
            int i = Util.clamp(teamInfo.teamSplitNum, context.splits.Length - (context.splitsToShow - context.splitFocusOffset), context.splitFocusOffset);
            if (context.features.syncSplits)
            {
                for (int team = 0; team < otherTeams.Length; team++)
                { i = Math.Max(i, Util.clamp(otherTeams[team].splitNum, context.splits.Length - (context.splitsToShow - context.splitFocusOffset), context.splitFocusOffset)); }
            }
            if (context.features.showSplits) { 
                for (int offsetSplit = 0; offsetSplit < context.splitsToShow; offsetSplit++)
                {
                    int adjustedIndex = i - (context.splitFocusOffset - offsetSplit);
                    if (context.layout.splitLabelsOnSplitsPage)
                    {
                        teamSplitNames[offsetSplit].Text = Util.stripGameIndicator(context.splits[adjustedIndex]);
                    }
                    teamSplitTimes[offsetSplit].Text = adjustedIndex == 0 ? teamInfo.teamSplits[adjustedIndex] : Util.hideUnsetSplit(teamInfo.teamSplits[adjustedIndex]);
                    teamSplitTimes[offsetSplit].ForeColor = labelUtil.defaultColour;
                    if(adjustedIndex < teamInfo.teamSplitNum)
                    {
                        teamSplitTimes[offsetSplit].ForeColor = ColorTranslator.FromHtml(context.layout.timerFadeColor);
                    }
                }
            }

            updateVsSplits(otherTeams);

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            if (teamInfo.teamGame == 0)
            {
                updateGameEnds(0, teamInfo.teamSplits[teamInfo.teamSplitNum]);
                for(i = 1; i < context.numberOfGames; i++)
                {
                    updateGameEnds(i, Util.emptyTime);
                }
                TimerLabel.Text = teamInfo.teamSplits[teamInfo.teamSplitNum];
                return;
            }
            updateGameEnds(0, teamInfo.teamGameEnd[0]);
            for (int j = 1; j < context.numberOfGames; j++)
            {
                string current = Util.emptyTime;
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
                }
                updateGameEnds(j, current);
            }
        }

        public void reconstructSplitMetadata()
        {
            teamInfo.teamSplitNum = Array.IndexOf(teamInfo.teamSplits, "00:00:00");
            teamInfo.teamGame = Util.getGameIndexForSplit(context.splits, teamInfo.teamSplitNum);
            var splitMapping = Util.extractGameEndsFromSplits(context.splits);
            for (var i = 0; i<teamInfo.teamGame; i++)
            {
                (var _, int gameEndIndex) = splitMapping[i];
                var endSplit = teamInfo.teamSplits[gameEndIndex];
                teamInfo.teamGameEnd[i] = endSplit;
                updateGameEnds(i, endSplit);
            }
            cycleIcon(teamInfo.teamGame);
        }

        private void updateGameEnds(int index, String text)
        {
            if (!context.features.showGameTimes)
            {
                return;
            }
            // Resolve label based on display type
            var label = resolveGameEndLabelForUpdate(index);
            label.Text = text;
        }

        private Label resolveGameEndLabelForUpdate(int index)
        {
            if (context.layout.gameTimeLayout == 0)
            {
                //TODO: this has to depend on display time for game page
                int gamesOnEach = (context.numberOfGames + 1) / 2;
                if (index < gamesOnEach)
                {
                    return gameEndsL[index];
                }
                else
                {
                    return gameEndsR[index - gamesOnEach];
                }
            }
            throw new Exception("Unrecognised game time panel layout option");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cycleIcon(1);
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

        private void undoButton_Click(object sender, EventArgs e)
        {
            undoSplitClick();
        }
    }
}
