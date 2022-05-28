using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Label commentaryHeader;

        Chart graph;

        public MetaControl()
        {
            InitializeComponent();
        }

        public void setupMetaControl(MainForm parent, MetaContext context)
        {
            this.parent = parent;
            this.context = context;
            var features = context.features.metaControl;
            this.Size = new Size(features.width + features.margin * 2, features.height + features.margin * 2);

            metaTabGroup.Location = new Point(features.margin, features.margin);
            metaTabGroup.Size = new Size(features.width, features.height);
            metaTabGroup.Selected += metaTabGroup_Selected;

            int tabCount = 0;

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

            updateSplits();
        }

        private void metaTabGroup_Selected(object sender, TabControlEventArgs e) => parent.childTabChanged();

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
            int splitLabelWidth = (features.width - (2 * features.margin)) / context.splitsToShow;
            int haMargin = features.margin / 2;
            for (int i = 0; i < context.splitsToShow; i++)
            {
                splitNames[i] = Util.createBaseLabel(splitLabelWidth * i + features.margin, features.margin, splitLabelWidth, context.layout.rowHeight, "test+" + i, ContentAlignment.MiddleCenter);
                tabPageSplits.Controls.Add(splitNames[i]);
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
            graph = new Chart();
            graph.Location = new Point(features.margin, features.margin);
            graph.Size = new Size(features.width - features.margin*2, features.height - features.margin*2 - 50);
            graph.ChartAreas.Add(new ChartArea()
            {
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
                    },
                    MajorTickMark = new TickMark()
                    {
                        Enabled = false
                    }
                },
                AxisX = new Axis()
                {
                    Maximum = context.features.metaControl.graph.splitsToShow,
                    Minimum = 0,
                    LabelStyle = new LabelStyle()
                    {
                        Enabled = true
                    },
                    MajorGrid = new Grid()
                    {
                        Interval = 1,
                        LineDashStyle = ChartDashStyle.Dot
                    }
                },
            });
            for (var i = 0; i < context.splits.Length; i++) {
                graph.ChartAreas[0].AxisX.CustomLabels.Add(0.5 + i,1.5 + i,context.splits[i]);
            }
            tabPageGraph.Controls.Add(graph);
            return tabPageGraph;
        }

        private void refreshGraphData()
        {
            var teams = parent.fetchOtherTeamInfo(null);
            int currentSplit = resolveSplit(teams, context.features.metaControl.graph.splitsToShow, context.features.metaControl.graph.splitFocusOffset);
            graph.Series.Clear();
            var teamData = parent.fetchTeamInfo();
            var splitData = new (string, double[])[context.features.metaControl.graph.splitsToShow];
            for (int offsetSplit = 0; offsetSplit < context.features.metaControl.graph.splitsToShow; offsetSplit++)
            {
                int adjustedIndex = currentSplit - (context.features.metaControl.graph.splitFocusOffset - offsetSplit) - 1;
                if(adjustedIndex == -1)
                {
                    splitData[offsetSplit] = ("Start", teamData.Select(d => 0d).ToArray());
                    continue;
                }
                splitData[offsetSplit] = (Util.stripGameIndicator(context.splits[adjustedIndex]), teamData.Select(t => t.teamSplitNum > adjustedIndex ? Util.parseTimeSpan(t.teamSplits[adjustedIndex]).TotalSeconds: 0).ToArray());
            }
            //Only calculate averages where all the data is present
            var averages = splitData.Where(row => row.Item1=="Start" || !row.Item2.Contains(0)).Select(row => row.Item2.Average()).ToArray();
            // =if(COUNTA($C25:$E25)=3,FLOOR((C25-AVERAGE($C25:$E25))*86400)/60,"")
            var diffsToAverage = averages.Select((average, idx) => (splitData[idx].Item1, splitData[idx].Item2.Select(d => d-average).ToArray())).ToArray();
            if (diffsToAverage.Length > 1)
            {
                for (var teamIdx = 0; teamIdx < teamData.Length; teamIdx++)
                {
                    var team = teamData[teamIdx];
                    var series = new Series();
                    series.Color = team.color;
                    series.ChartType = SeriesChartType.Line;
                    series.MarkerStyle = MarkerStyle.Square;
                    series.BorderWidth = 4;
                    series.MarkerSize = 8;
                    for (var j = 0; j < diffsToAverage.Length; j++)
                    {
                        var row = diffsToAverage[j];
                        series.Points.AddXY(j, row.Item2[teamIdx]);
                    }
                    graph.Series.Add(series);
                }
                var height = Math.Max(diffsToAverage.Select(row => row.Item2.Select(a => Math.Abs(a)).Max()).Max(), 1);
                // Potentially round up height to the nearest 10s
                height = ((int)(height / 10) + 1) * 10;
                var startingIndex = Math.Max(0, currentSplit - context.features.metaControl.graph.splitsToShow + 1);
                graph.ChartAreas[0].AxisY.Maximum = height;
                graph.ChartAreas[0].AxisY.Minimum = -height;
                graph.ChartAreas[0].AxisY.MajorGrid.Interval = height / 2;
                graph.ChartAreas[0].AxisY.LabelStyle.Interval = height / 2;
                graph.ChartAreas[0].AxisX.Maximum = startingIndex + context.features.metaControl.graph.splitsToShow;
                graph.ChartAreas[0].AxisX.Minimum = startingIndex;
                graph.ChartAreas[0].RecalculateAxesScale();
            }
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
            commentaryHeader = Util.createBaseLabel(3, features.margin, 160, context.layout.rowHeight, "Commentary:", ContentAlignment.MiddleLeft, Color.Black, 16);
            tabPageCategories.Controls.Add(commentaryHeader);
            commentaryLabel = Util.createBaseLabel(3 + commentaryHeader.Width + 3, features.margin, features.width - commentaryHeader.Width - 3, context.layout.rowHeight, "", ContentAlignment.MiddleLeft);
            tabPageCategories.Controls.Add(commentaryLabel);
            return tabPageCategories;
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
            //if (metaTabGroup.SelectedIndex == 2)
            //{
                refreshGraphData();
            //}
        }

        public void RefreshGame()
        {
            reloadCategoryTab();
        }

        private void reloadCategoryTab()
        {
            if (context.features.metaControl.commentators)
            {
                commentaryLabel.Text = context.commentators[parent.getMaxIcon() - 1];
            }
        }
    }
}
