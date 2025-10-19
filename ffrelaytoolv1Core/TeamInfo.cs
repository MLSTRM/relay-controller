using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace ffrelaytoolv1
{
    public class TeamInfo
    {
        public string teamName { get; set; }

        public int teamIcon { get; set; }

        public Timer teamCooldown { get; set; }

        public bool teamWaiting { get; set; }

        public string[] teamRunners { get; set; }

        public int teamGame { get; set; }

        public int teamSplitNum { get; set; }

        public string[] teamSplits { get; set; }

        public string[] teamGameEnd { get; set; }

        public string teamFinish { get; set; }

        public bool teamFinished { get; set; }

        public int teamTab { get; set; }

        public Color color { get; set; }

        public Image tabBackground { get; set; }

        public bool leftAlign { get; set; }

        private readonly string runnerFileName;

        public int teamSplitKey { get; }

        private bool useIcons;

        public TeamInfo(int numberOfGames, int numberOfSplits, string teamName, string runnerFileName, Color color, Image bg, int splitKey, bool useIcons, bool leftAlign)
        {
            tabBackground = bg;
            this.color = color;
            teamIcon = 1;
            teamCooldown = new Timer();
            teamWaiting = false;
            teamGame = 0;
            teamSplitNum = 0;
            teamFinished = false;
            teamTab = 0;
            this.leftAlign = leftAlign;
            this.teamName = teamName;
            this.runnerFileName = runnerFileName;
            this.teamSplitKey = splitKey;
            if(File.Exists(runnerFileName)){
                teamRunners = File.ReadAllLines(runnerFileName);
            } else
            {
                throw new Exception("Unable to find file with name "+runnerFileName);
            }
            teamGameEnd = new string[numberOfGames];
            for (int i = 0; i < numberOfGames; i++)
            {
                teamGameEnd[i] = Util.emptyTime;
            }
            teamSplits = new string[numberOfSplits];
            for (int i = 0; i < numberOfSplits; i++)
            {
                teamSplits[i] = Util.emptyTime;
            }
            this.useIcons = useIcons;
            cycleTeamIcon(() => { });
        }

        public void reloadRunnerInfo() {
            if(File.Exists(runnerFileName)){
                teamRunners = File.ReadAllLines(runnerFileName);
            }
        }

        public void cycleTeamIcon(Action buttonCallback)
        {
            if (useIcons)
            {
                if (File.Exists("icon_" + teamIcon + ".png"))
                {
                    File.Copy("icon_" + teamIcon + ".png", teamName + "-Icon.png", true);
                }
                else
                {
                    File.Copy("icon_1.png", teamName + "-Icon.png", true);
                    teamIcon = 1;
                }
            }
            buttonCallback();
        }
    }
}
