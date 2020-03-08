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

        public TeamControl()
        {
            InitializeComponent();
        }

        public void setupTeamControl(Form1 parent, TeamInfo info)
        {
            this.parent = parent;
            teamInfo = info;
            TeamSplitButton.BackColor = teamInfo.color;
            foreach (TabPage page in teamTabGroup.TabPages)
            {
                page.BackgroundImage = teamInfo.tabBackground;
            }
        }

        private void TeamSplitButton_Click(object sender, EventArgs e)
        {

        }

        public void updateTimerEvent(string current, bool cycleInfo)
        {
            teamInfo.teamSplits[teamInfo.teamSplitNum] = current;
            if (!teamInfo.teamFinished)
            {
                updateSplits();
            }
            if (cycleInfo)
            {
                if (teamTabGroup.SelectedIndex == 2)
                { teamTabGroup.SelectedIndex = 0; }
                else { teamTabGroup.SelectedIndex++; }
            }
        }

        private void updateSplits()
        {
            //Use method below.
            //Some callback to parent to resolve versus wrappers
            //Make split number configurable/resolvable
            //Make split offset resolvable.

            //Text + Time control for amount shifting.
        }

        void updateSplits(int splitNum, string[] Splits, int teamGame, int numberOfGames,
            Label teamSplitName1, Label teamSplitName2, Label teamSplitName3, Label teamSplitName4,
            Label teamSplitTime1, Label teamSplitTime2, Label teamSplitTime3, Label teamSplitTime4, string[] teamSplits, VersusWrapper[] otherTeams,
            ref string[] teamGameEndArchive, string[] teamGameEnds, Label teamGameTimerL, Label teamGameEndL, Label teamTimer, Label teamGameTimerR)
        {
            int i = splitNum;
            if (i == 0) { i += 2; }
            else if (i == 1) { i++; }
            else if (i == Splits.Length - 1) { i--; }
            teamSplitName1.Text = Util.stripGameIndicator(Splits[i - 2]);
            teamSplitName2.Text = Util.stripGameIndicator(Splits[i - 1]);
            teamSplitName3.Text = Util.stripGameIndicator(Splits[i]);
            teamSplitName4.Text = Util.stripGameIndicator(Splits[i + 1]);
            teamSplitTime1.Text = teamSplits[i - 2];
            teamSplitTime2.Text = teamSplits[i - 1];
            teamSplitTime3.Text = teamSplits[i];
            teamSplitTime4.Text = teamSplits[i + 1];

            //Split comparisons, since this works even on FF1 it needs to be here
            int offset = 0;
            bool[] vs = new bool[otherTeams.Length];
            while (offset <= splitNum)
            {
                for (int team = 0; team < otherTeams.Length; team++)
                {
                    //If we're on the same split, then don't update the versus differences.
                    if (otherTeams[team].splitNum == splitNum)
                    {
                        vs[team] = true;
                        if (splitNum > 0)
                        {
                            TimeSpan seg = Util.resolveTimeSpan(teamSplits[splitNum - 1], otherTeams[team].splits[splitNum - 1]);
                            Util.updateDifferenceDisplay(otherTeams[team].vsLabel, seg);
                        }
                    }
                    //Otherwise update the value based on the live difference (Only if greater than static difference?)
                    if (!vs[team] && otherTeams[team].splits[splitNum - offset] != "00:00:00")
                    {
                        TimeSpan seg = Util.resolveTimeSpan(teamSplits[splitNum - offset], otherTeams[team].splits[splitNum - offset]);
                        if (splitNum - offset > 0)
                        {
                            //If the offset on the previous split is larger than the live one then use that.
                            TimeSpan olderseg = Util.resolveTimeSpan(teamSplits[splitNum - offset - 1], otherTeams[team].splits[splitNum - offset - 1]);
                            if (Math.Abs(olderseg.Ticks) > Math.Abs(seg.Ticks))
                            {
                                seg = olderseg;
                            }
                        }
                        Util.updateDifferenceDisplay(otherTeams[team].vsLabel, seg);
                        vs[team] = true;
                    }
                }
                if (vs.All(b => b))
                {
                    offset = splitNum;
                }
                offset++;
            }

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            teamGameEndArchive = teamGameEnds;
            if (teamGame == 0)
            {
                teamGameTimerL.Text = teamSplits[splitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                teamGameEndL.Text = teamSplits[splitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                teamTimer.Text = teamSplits[splitNum];
                return;
            }
            string lefttimes = teamGameEnds[0] + "\n";
            string righttimes = "";
            for (int j = 1; j < numberOfGames; j++)
            {
                string current = "00:00:00";
                //If we're past the selected game, then subtract the previous one to get the segment time over split time
                if (teamGame > j)
                {
                    TimeSpan seg = Util.resolveTimeSpan(teamGameEnds[j], teamGameEnds[j - 1]);
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (teamGame == j)
                {
                    TimeSpan seg = Util.resolveTimeSpan(teamSplits[splitNum], teamGameEnds[j - 1]);
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    teamTimer.Text = current;
                    teamGameEndArchive[teamGame] = teamSplits[splitNum];
                }
                if (j < 7)
                { lefttimes += current + "\n"; }
                else
                { righttimes += current + "\n"; }
            }
            teamGameTimerL.Text = lefttimes;
            teamGameTimerR.Text = righttimes;
        }

    }
}
