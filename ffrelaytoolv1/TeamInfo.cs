using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ffrelaytoolv1
{
    class TeamInfo
    {
        int teamIcon { get; set; }

        Timer teamCooldown { get; set; }

        bool teamWaiting { get; set; }

        string[] teamRunners { get; set; }

        int teamGame { get; set; }

        int teamSplitNum { get; set; }

        string[] teamSplits { get; set; }

        string[] teamGameEnd { get; set; }

        string teamFinish { get; set; }

        bool teamFinished { get; set; }

        int teamTab { get; set; }

        string[] teamGameEndArchive { get; set; }

        public TeamInfo(int numberOfGames, int numberOfSplits, string runnerFileName)
        {
            teamIcon = 1;
            teamCooldown = new Timer();
            teamWaiting = false;
            teamGame = 0;
            teamSplitNum = 0;
            teamFinished = false;
            teamTab = 0;
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
