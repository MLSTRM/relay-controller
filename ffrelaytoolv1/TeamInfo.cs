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
    class TeamInfo
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

        public Bitmap tabBackground { get; set; }

        public TeamInfo(int numberOfGames, int numberOfSplits, string teamName, string runnerFileName, Color color, Bitmap bg)
        {
            this.tabBackground = bg;
            this.color = color;
            teamIcon = 1;
            teamCooldown = new Timer();
            teamWaiting = false;
            teamGame = 0;
            teamSplitNum = 0;
            teamFinished = false;
            teamTab = 0;
            this.teamName = teamName;
            if(File.Exists(runnerFileName)){
                teamRunners = File.ReadAllLines(runnerFileName);
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
    }
}
