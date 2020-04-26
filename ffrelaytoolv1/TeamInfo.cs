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

        public string[] teamGameEndArchive { get; set; }

        public Color color { get; set; }

        public Image tabBackground { get; set; }

        private readonly string runnerFileName;

        public TeamInfo(int numberOfGames, int numberOfSplits, string teamName, string runnerFileName, Color color, Image bg)
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
            this.teamName = teamName;
            this.runnerFileName = runnerFileName;
            if(File.Exists(runnerFileName)){
                teamRunners = File.ReadAllLines(runnerFileName);
            } else
            {
                throw new Exception("Unable to find file with name"+runnerFileName);
            }
            teamGameEnd = new string[numberOfGames];
            teamGameEndArchive = new string[numberOfGames];
            for (int i = 0; i < numberOfGames; i++)
            {
                teamGameEnd[i] = "00:00:00";
                teamGameEndArchive[i] = "00:00:00";
            }
            teamSplits = new string[numberOfSplits];
            for (int i = 0; i < numberOfSplits; i++)
            {
                teamSplits[i] = "00:00:00";
            }
        }

        public void reloadRunnerInfo() {
            if(File.Exists(runnerFileName)){
                teamRunners = File.ReadAllLines(runnerFileName);
            }
        }

        public void cycleTeamIcon(Action buttonCallback)
        {
            if (File.Exists("icon_" + teamIcon + ".png"))
            {
                File.Copy("icon_" + teamIcon + ".png", teamName + "-Icon.png", true);
                buttonCallback();
            }
            else
            {
                File.Copy("icon_1.png", teamName + "-Icon.png", true);
                teamIcon = 1;
                buttonCallback();
            }
        }
    }
}
