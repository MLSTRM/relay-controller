using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static ffrelaytoolv1.MetaFile;

namespace ffrelaytoolv1
{
    public partial class MetaControl : UserControl
    {
        MainForm parent;

        MetaContext context;

        Label[] splitNames;

        Label commentaryLabel;
        FlowLayoutPanel commentaryLayoutPanel;
        Label commentaryHeader;
        Label gameHeader;

        Chart graph;

        LabelUtil labelUtil;

        int bottomMarginOffset = 0;

        public MetaControl()
        {
            InitializeComponent();
        }

        public void setupMetaControl(MainForm parent, MetaContext context, LabelUtil labelUtil)
        {
            this.labelUtil = labelUtil;
            this.parent = parent;
            this.context = context;
            var features = context.features.metaControl;
            this.Size = new Size(features.width + features.margin * 2, features.height + features.margin * 2);

            // TODO: adjust for features.alwaysShowCommentary to always show commentary and current game info

            var height = features.commentaryHeight < 0 ? context.layout.rowHeight : features.commentaryHeight;
            bottomMarginOffset = - height - (2 * features.margin) - 5 - (int)(features.staticSubheaderFontSize ?? context.layout.defaultTimerFontSize);

            metaTabGroup.Location = new Point(features.margin, features.margin);
            metaTabGroup.Size = new Size(features.width, features.height + 26);
            metaTabGroup.Selected += metaTabGroup_Selected;

            int tabCount = 0;

            constructCommentaryContents(features);

            if (features.splits)
            {
                metaTabGroup.Controls.Add(createSplitsPage(context, features, ++tabCount));
            }
            if (features.commentators)
            {
                //Construct runner/category/commentary tab
                metaTabGroup.Controls.Add(createCommentaryPage(context, features, ++tabCount));
            }
            if (context.features.showGraph)
            {
                metaTabGroup.Controls.Add(createGraphPage(context, features, ++tabCount));
            }
            metaTabGroup_Selected(null, null);
            updateSplits();
        }

        private void metaTabGroup_Selected(object sender, TabControlEventArgs e) {
            var features = context.features.metaControl;
            if (features.alwaysShowCommentary)
            {
                commentaryHeader.Parent = metaTabGroup.SelectedTab;
                if(gameHeader != null)
                {
                    gameHeader.Parent = metaTabGroup.SelectedTab;
                }
                if (context.layout.useBasicNameLayout)
                {
                    commentaryLabel.Parent = metaTabGroup.SelectedTab;
                }
                else
                {
                    commentaryLayoutPanel.Parent = metaTabGroup.SelectedTab;
                }
            }
            parent.childTabChanged();
        }

        private TabPage createSplitsPage(MetaContext context, MetaControlFeatures features, int tabCounter)
        {
            TabPage tabPageSplits = new TabPage()
            {
                BackColor = ColorTranslator.FromHtml(features.color),
                Location = new Point(4, 22),
                Name = "tabPageSplits",
                Padding = new Padding(3),
                // BackgroundImage = info.tabBackground,
                Size = new Size(features.width, features.height),
                TabIndex = tabCounter,
                Text = "Splits",
            };
            splitNames = new Label[context.splitsToShow];
            if (context.layout.horizontalDisplay)
            {
                int splitLabelWidth = (features.width - (2 * features.margin)) / context.splitsToShow;
                for (int i = 0; i < context.splitsToShow; i++)
                {
                    splitNames[i] = labelUtil.createBaseLabel(
                        splitLabelWidth * i + features.margin,
                        features.height - context.layout.rowHeight + bottomMarginOffset - features.margin,
                        splitLabelWidth,
                        context.layout.rowHeight,
                        "test+" + i,
                        ContentAlignment.MiddleCenter);
                    tabPageSplits.Controls.Add(splitNames[i]);
                }
            }
            else
            {
                int splitLabelHeight = (context.layout.boxHeight + bottomMarginOffset - (2 * context.layout.boxMargin)) / context.splitsToShow;
                for (int i = 0; i < context.splitsToShow; i++)
                {
                    splitNames[i] = labelUtil.createBaseLabel((features.width - context.layout.rowWidth) / 2, context.layout.boxMargin + splitLabelHeight * i, context.layout.rowWidth, splitLabelHeight, "test+" + i, ContentAlignment.MiddleCenter);
                    tabPageSplits.Controls.Add(splitNames[i]);
                }
            }
            return tabPageSplits;
        }

        private TabPage createGraphPage(MetaContext context, MetaControlFeatures features, int tabCounter)
        {
            TabPage tabPageGraph = new TabPage()
            {
                BackColor = ColorTranslator.FromHtml(features.color),
                Location = new Point(4, 22),
                Name = "tabPageGraph",
                Padding = new Padding(3),
                // BackgroundImage = info.tabBackground,
                Size = new Size(features.width, features.height),
                TabIndex = tabCounter,
                Text = "Graph",
            };
            if (features.alwaysShowCommentary)
            {
                tabPageGraph.Controls.Add(commentaryHeader);
                if (context.layout.useBasicNameLayout)
                {
                    tabPageGraph.Controls.Add(commentaryLabel);
                }
                else
                {
                    tabPageGraph.Controls.Add(commentaryLayoutPanel);
                }
            }
            graph = new Chart();
            graph.Location = new Point(features.margin, features.margin);
            graph.Size = new Size(features.width - features.margin * 2, features.height - features.margin * 2 + bottomMarginOffset);
            graph.BackColor = ColorTranslator.FromHtml(features.color);
            var chartArea = new ChartArea()
            {
                Position =
                {
                    X = 0f,
                    Y = 0f,
                    Height = 100,
                    Width = 100
                },
                AxisY = new Axis()
                {
                    Maximum = 1,
                    Minimum = -1,
                    MajorGrid = new Grid()
                    {
                        Enabled = true,
                        Interval = 1,
                    },
                    LabelAutoFitStyle = LabelAutoFitStyles.None,
                    LabelStyle = new LabelStyle()
                    {
                        TruncatedLabels = true,
                        Format = "F",
                        Interval = 1,
                        Font = labelUtil.activeFontSized(context.features.metaControl.graph.yAxisFontSize),
                    },
                    MajorTickMark = new TickMark()
                    {
                        Enabled = false
                    }
                },
                
                BackColor = ColorTranslator.FromHtml(features.color)
            };
            if (context.features.metaControl.graph.negativeOnly) {
                chartArea.AxisX2 = new Axis()
                {
                    Maximum = context.features.metaControl.graph.splitsToShow,
                    Minimum = 0,
                    LabelStyle = new LabelStyle()
                    {
                        Enabled = true,
                        Font = labelUtil.activeFontSized(context.features.metaControl.graph.xAxisFontSize)
                    },
                    MajorGrid = new Grid()
                    {
                        Interval = 1,
                        LineDashStyle = ChartDashStyle.Dot
                    }
                };
                chartArea.AxisX2.CustomLabels.Add(-0.5, 0.5, "Start");
                for (var i = 0; i < context.splits.Length; i++)
                {
                    chartArea.AxisX2.CustomLabels.Add(0.5 + i, 1.5 + i, Util.stripGameIndicator(context.splits[i]));
                }
            } else {
                chartArea.AxisX = new Axis()
                {
                    Maximum = context.features.metaControl.graph.splitsToShow,
                    Minimum = 0,
                    LabelStyle = new LabelStyle()
                    {
                        Enabled = true,
                        Font = labelUtil.activeFontSized(context.features.metaControl.graph.xAxisFontSize)
                    },
                    MajorGrid = new Grid()
                    {
                        Interval = 1,
                        LineDashStyle = ChartDashStyle.Dot
                    }
                };
                chartArea.AxisX.CustomLabels.Add(-0.5, 0.5, "Start");
                for (var i = 0; i < context.splits.Length; i++)
                {
                    chartArea.AxisX.CustomLabels.Add(0.5 + i, 1.5 + i, Util.stripGameIndicator(context.splits[i]));
                }
            }
            graph.ChartAreas.Add(chartArea);
            tabPageGraph.Controls.Add(graph);
            return tabPageGraph;
        }

        private void refreshGraphData()
        {
            var teams = parent.fetchOtherTeamInfo(null);
            int currentSplit = context.features.metaControl.graph.predictPartialGraphLines ? resolveSplit(teams, context.features.metaControl.graph.splitsToShow, context.features.metaControl.graph.splitFocusOffset) : resolveLastCommonSplit(teams, context.features.metaControl.graph.splitsToShow, context.features.metaControl.graph.splitFocusOffset);
            graph.Series.Clear();
            if (currentSplit <= 0)
            {
                return;
            }
            var teamData = parent.fetchTeamInfo();
            var splitData = new (string, double[])[context.features.metaControl.graph.splitsToShow + 1];
            for (int offsetSplit = 0; offsetSplit < context.features.metaControl.graph.splitsToShow + 1; offsetSplit++)
            {
                int adjustedIndex = currentSplit - context.features.metaControl.graph.splitFocusOffset + offsetSplit - 1;
                if (adjustedIndex == -1)
                {
                    splitData[offsetSplit] = ("Start", teamData.Select(d => 0d).ToArray());
                    continue;
                }
                splitData[offsetSplit] = (Util.stripGameIndicator(context.splits[adjustedIndex]), teamData.Select(t => (t.teamSplitNum > adjustedIndex) || (t.teamFinished && t.teamSplitNum == adjustedIndex) ? Util.parseTimeSpan(t.teamSplits[adjustedIndex]).TotalSeconds : 0).ToArray());
            }
            //Only calculate averages where all the data is present
            var averages = splitData.Where(row => row.Item1 == "Start" || !row.Item2.Contains(0)).Select(row => row.Item2.Average()).ToArray();
            // =if(COUNTA($C25:$E25)=3,FLOOR((C25-AVERAGE($C25:$E25))*86400)/60,"")
            var diffsToAverage = averages.Select((average, idx) => (splitData[idx].Item1, splitData[idx].Item2.Select(d => d - average).ToArray())).ToArray();
            if (diffsToAverage.Length > 1)
            {
                var predictions = new (string, double?[])[splitData.Length - diffsToAverage.Length + 1];
                predictions[0] = (diffsToAverage[diffsToAverage.Length - 1].Item1, diffsToAverage[diffsToAverage.Length - 1].Item2.Cast<double?>().ToArray());
                for (var predictionIdx = 1; predictionIdx < predictions.Length; predictionIdx++)
                {
                    var offset = currentSplit - context.features.metaControl.graph.splitFocusOffset - 1 + diffsToAverage.Length + predictionIdx;
                    predictions[predictionIdx] = (splitData[predictionIdx + diffsToAverage.Length - 1].Item1, teamData.Select((t, i) =>
                    {
                        if (t.teamSplitNum >= offset || (t.teamFinished && t.teamSplitNum == offset - 1 ))
                        {
                            // Replace with proper prediction logic
                            // Need to recalculate partial averages across 2 teams when that exists.
                            // If it's just the one team, then just assume previous value and straight-line it.
                            return predictions[0].Item2[i];
                        }
                        return null;
                    }).ToArray());
                }
                // TODO - extrapolate / predict partial values here to still indicate team lead?
                for (var teamIdx = 0; teamIdx < teamData.Length; teamIdx++)
                {
                    var team = teamData[teamIdx];
                    var series = new Series();
                    series.Color = team.color;
                    series.ChartType = SeriesChartType.Line;
                    series.MarkerStyle = MarkerStyle.Square;
                    series.BorderWidth = 4;
                    series.MarkerSize = 8;
                    if (context.features.metaControl.graph.negativeOnly)
                    {
                        series.XAxisType = AxisType.Secondary;
                    }
                    for (var j = 0; j < diffsToAverage.Length; j++)
                    {
                        int adjustedIndex = currentSplit - context.features.metaControl.graph.splitFocusOffset + j;
                        var row = diffsToAverage[j];
                        series.Points.AddXY(adjustedIndex, row.Item2[teamIdx]);
                    }
                    graph.Series.Add(series);
                    if (context.features.metaControl.graph.predictPartialGraphLines)
                    {
                        // add prediction series in here
                        var predictionSeries = new Series();
                        predictionSeries.Color = team.color;
                        predictionSeries.ChartType = SeriesChartType.Line;
                        predictionSeries.MarkerStyle = MarkerStyle.Square;
                        predictionSeries.BorderDashStyle = ChartDashStyle.Dash;
                        predictionSeries.BorderWidth = 3;
                        predictionSeries.MarkerSize = 6;
                        if (context.features.metaControl.graph.negativeOnly)
                        {
                            predictionSeries.XAxisType = AxisType.Secondary;
                        }
                        // difference between splitsToShow and missing data points, adding one for overlap
                        int predictionLength = predictions.Length;
                        for (var k = 0; k < predictionLength; k++)
                        {
                            int adjustedIndex = currentSplit - context.features.metaControl.graph.splitFocusOffset + diffsToAverage.Length - 1 + k;
                            var row = predictions[k];
                            if (row.Item2[teamIdx] is double val)
                            {
                                predictionSeries.Points.AddXY(adjustedIndex, val);
                            }
                        }
                        graph.Series.Add(predictionSeries);
                    }
                }
                var (height, interval) = ResolveChartScale(diffsToAverage);
                var startingIndex = Math.Max(0, currentSplit - context.features.metaControl.graph.splitsToShow + 1);
                var oldMax = graph.ChartAreas[0].AxisY.Minimum;
                if (context.features.metaControl.graph.negativeOnly)
                {
                    graph.ChartAreas[0].AxisY.Maximum = 0;
                    graph.ChartAreas[0].AxisY.Minimum = -height;
                    graph.ChartAreas[0].AxisY.IsReversed = true;
                }
                else if (context.features.metaControl.graph.positiveOnly)
                {
                    graph.ChartAreas[0].AxisY.Maximum = height;
                    graph.ChartAreas[0].AxisY.Minimum = 0;
                } else
                {
                    graph.ChartAreas[0].AxisY.Maximum = height;
                    graph.ChartAreas[0].AxisY.Minimum = -height;
                }
                graph.ChartAreas[0].AxisY.MajorGrid.Interval = interval;
                graph.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
                graph.ChartAreas[0].AxisY.LabelStyle.Interval = interval;
                graph.ChartAreas[0].AxisY.Interval = interval;
                if (height != oldMax)
                {
                    //Only recalc labels if its changed
                    graph.ChartAreas[0].AxisY.CustomLabels.Clear();
                    var hainterval = interval / 2;
                    graph.ChartAreas[0].AxisY.CustomLabels.Add(-hainterval, hainterval, TimeSpan.FromSeconds(0).ToString(@"mm\:ss"));
                    for (double i = interval; i <= height; i += interval)
                    {
                        var spanString = TimeSpan.FromSeconds(i).ToString(@"mm\:ss");
                        if (height > 3600)
                        {
                            spanString = TimeSpan.FromSeconds(i).ToString(@"h\:mm\:ss");
                        }
                        graph.ChartAreas[0].AxisY.CustomLabels.Add(i - hainterval, i + hainterval, spanString);
                        graph.ChartAreas[0].AxisY.CustomLabels.Add(-(i - hainterval), -(i + hainterval), "-" + spanString);
                    }
                }
                if (context.features.metaControl.graph.negativeOnly)
                {
                    graph.ChartAreas[0].AxisX2.Maximum = startingIndex + context.features.metaControl.graph.splitsToShow;
                    graph.ChartAreas[0].AxisX2.Minimum = startingIndex;
                    graph.ChartAreas[0].AxisX2.LabelAutoFitStyle = LabelAutoFitStyles.WordWrap;
                } else
                {
                graph.ChartAreas[0].AxisX.Maximum = startingIndex + context.features.metaControl.graph.splitsToShow;
                graph.ChartAreas[0].AxisX.Minimum = startingIndex;
                graph.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.WordWrap;

                }
                graph.ChartAreas[0].RecalculateAxesScale();
            }
        }

        private (double, double) ResolveChartScale((string, double[])[] diffsToAverage)
        {
            //Values all in seconds.
            var height = Math.Max(diffsToAverage.Select(row => row.Item2.Select(a => Math.Abs(a)).Max()).Max(), 1);
            if (height < 60)
            {
                return (60, 15);
            }
            if (height < 120)
            {
                //Special case for narrower view if we somehow have a <2 minute spread (wild)
                return (120, 30);
            }
            // Potentially round up height to the nearest 4 minutes
            height = ((int)(height / 240) + 1) * 240;
            // Have 4 lines either side
            var interval = height / 4;
            return (height, interval);
        }

        private void constructCommentaryContents(MetaControlFeatures features)
        {
            var height = features.commentaryHeight < 0 ? context.layout.rowHeight : features.commentaryHeight;
            if (context.features.metaControl.commentaryInlineHeader)
            {
                commentaryHeader = labelUtil.createBaseLabel(3, features.height - height - features.margin, 160, height - features.margin - 5, "Commentary:", ContentAlignment.MiddleLeft, labelUtil.defaultColour, context.layout.defaultTimerFontSize);
            }
            else
            {
                var fontSize = features.staticSubheaderFontSize ?? context.layout.defaultTimerFontSize;
                commentaryHeader = labelUtil.createBaseLabel(
                    features.margin, features.height +bottomMarginOffset,
                    features.staticSubheaderSplitPoint - features.margin, (int)fontSize + 5 + features.margin,
                    "Commentary:", ContentAlignment.MiddleLeft, labelUtil.defaultColour, fontSize);
                gameHeader = labelUtil.createBaseLabel(
                    features.staticSubheaderSplitPoint, features.height + bottomMarginOffset,
                    features.width - features.staticSubheaderSplitPoint - features.margin, (int)fontSize + 5 + features.margin,
                    "Game: Final Fantasy XIII (PC - Any%)", ContentAlignment.MiddleRight,
                    labelUtil.defaultColour, fontSize);
            }
            commentaryLabel = labelUtil.createBaseLabel(3 + commentaryHeader.Width + 3, features.height - height - features.margin, features.width - commentaryHeader.Width - 3 - features.margin, height, "", ContentAlignment.MiddleLeft);
            var widthOffset = context.features.metaControl.commentaryInlineHeader ? commentaryHeader.Width : 0;
            commentaryLayoutPanel = new FlowLayoutPanel
            {
                Location = new Point(3 + widthOffset + 3, features.height - height - features.margin),
                Size = new Size(features.width - widthOffset - 3 - features.margin, height - features.margin),
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(0, 0, 0, 0),
                Margin = new Padding(0, 0, 0, 0),
            };
        }

        private TabPage createCommentaryPage(MetaContext context, MetaControlFeatures features, int tabCounter)
        {
            TabPage tabPageCategories = new TabPage()
            {
                BackColor = ColorTranslator.FromHtml(features.color),
                Location = new Point(4, 22),
                Name = "tabPageCategories",
                Padding = new Padding(3),
                // BackgroundImage = info.tabBackground,
                Size = new Size(context.layout.boxWidth, context.layout.boxHeight),
                TabIndex = tabCounter,
                Text = "Category & Runner",
            };
            tabPageCategories.Controls.Add(commentaryHeader);
            if (context.layout.useBasicNameLayout)
            {
                tabPageCategories.Controls.Add(commentaryLabel);
            }
            else
            {
                tabPageCategories.Controls.Add(commentaryLayoutPanel);
            }
            return tabPageCategories;
        }

        private int resolveLastCommonSplit(VersusWrapper[] teams, int toShow, int offset)
        {
            int i = context.splits.Length;
            if (context.features.syncSplits)
            {
                for (int team = 0; team < teams.Length; team++)
                { i = Math.Min(i, Util.clamp(teams[team].splitNum, context.splits.Length - (toShow - offset), offset)); }
            }
            return i;
        }

        private int resolveSplit(VersusWrapper[] teams, int toShow, int offset)
        {
            int i = 0;
            if (context.features.syncSplits)
            {
                for (int team = 0; team < teams.Length; team++)
                { i = Math.Max(i, Util.clamp(teams[team].splitNum, context.splits.Length - (toShow - offset), offset)); }
            }
            return i;
        }

        public int updateTimerEvent(string current, bool cycleInfo, int targetTab = -1)
        {
            updateSplits();
            if (cycleInfo)
            {
                if (targetTab > -1)
                {
                    metaTabGroup.SelectedIndex = Util.clamp(targetTab, metaTabGroup.TabCount, 0);
                }
                else if (metaTabGroup.SelectedIndex == metaTabGroup.TabCount - 1)
                {
                    metaTabGroup.SelectedIndex = 0;
                }
                else
                {
                    metaTabGroup.SelectedIndex++;
                }
            }
            return metaTabGroup.SelectedIndex;
        }

        private void updateSplits()
        {
            var teams = parent.fetchOtherTeamInfo(null);
            int i = resolveSplit(teams, context.splitsToShow, context.splitFocusOffset);
            if (context.features.showSplits)
            {
                for (int offsetSplit = 0; offsetSplit < context.splitsToShow; offsetSplit++)
                {
                    int adjustedIndex = i - (context.splitFocusOffset - offsetSplit);
                    splitNames[offsetSplit].Text = Util.stripGameIndicator(context.splits[adjustedIndex]);
                }
            }
            if (context.features.showGraph)
            {
                // This runs on every timer tick right now???
                // Should probably only happen if splits have changed?
                refreshGraphData();
            }
        }

        public void RefreshGame()
        {
            reloadCategoryTab();
        }

        private void reloadCategoryTab()
        {
            // TODO: what about swapovers - union of commentary sets? Could overflow the available space?
            if (context.features.metaControl.commentators || context.features.metaControl.alwaysShowCommentary)
            {
                var gamesUnion = parent.fetchTeamInfo().Select(team => team.teamIcon).Distinct().OrderBy(i => i);
                var totalCommentators = gamesUnion.SelectMany(game => context.commentators[game - 1]);
                var useBasic = false;
                if(totalCommentators.Count() > 6)
                {
                    useBasic = true;
                }
                commentaryLabel.Text = String.Join(", ", totalCommentators.Select(ud => ud.Name));
                commentaryLayoutPanel.Controls.Clear();
                commentaryLayoutPanel.Controls.AddRange(totalCommentators.Select(ud =>
                {
                    var control = new RichNameControl();
                    control.setupNameControl(labelUtil, ud, context, true, RichNameControl.ColorMode.DARK, useBasic);
                    if (context.features.enableDiscordIntegration)
                    {
                        control.setSpeaking(false);
                    }
                    return control;
                }).ToArray());
                var currentGame = parent.getMaxIcon();
                var runners = parent.fetchTeamInfo()[0].teamRunners;
                gameHeader.Text = $"Game: {runners[(currentGame * 4) - 4]} ({runners[(currentGame * 4) - 3]})";
            }
        }

        public void updateCommentatorSpeaking(IEnumerable<string> speaking, bool reset)
        {
            foreach (var control in commentaryLayoutPanel.Controls)
            {
                if (control is RichNameControl richName)
                {
                    if (reset)
                    {
                        richName.setSpeaking(true);
                    }
                    else if (speaking.Contains(richName.getUserName()) || speaking.Contains(richName.getDiscordName()))
                    {
                        richName.setSpeaking(true);
                    }
                    else
                    {
                        richName.setSpeaking(false);
                    }
                }
            }
        }

        public List<String> outputCaptureInfo(Control parent)
        {
            List<String> captureLines = new List<string>();
            captureLines.Add("");
            captureLines.Add("Meta info: ");
            captureLines.Add(Util.outputCaptureInfoRelative(metaTabGroup.TabPages[0], parent, metaTabGroup, this));
            return captureLines;
        }
    }
}
