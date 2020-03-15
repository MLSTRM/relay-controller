using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace ffrelaytoolv1
{
    public partial class Form1 : Form
    {
        /*
         * Boxes have gone 228->166
         * Loss of 62px
         * Container goes 254->192
         * 
         */


        Timer tick;
        KeyboardHook hook = new KeyboardHook();
        bool MainTimerRunning = false;
        public DateTime TimerStart;
        int numberOfGames = 14;
        //int bgnum = 1;
        int chocoIcon = 1;
        int mogIcon = 1;
        int tonbIcon = 1;
        Timer MogCooldown;
        Timer ChocoCooldown;
        Timer TonbCooldown;
        bool MogWaiting = false;
        bool ChocoWaiting = false;
        bool TonbWaiting = false;
        int MinuteCount = 0;
        bool ChangedThisMin = false;
        String gameSep = "!";

        string[] Splits;
        string[] MogRunners;
        string[] ChocoRunners;
        string[] TonbRunners;
        string[] Commentators;

        int MogGame = 0;
        int MogSplitNum = 0;
        string[] MogSplits;
        string[] MogGameEnd = {  "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", 
                                    "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00",
                                    "00:00:00", "00:00:00"};
        string MogFinish;
        bool MogFinished = false;
        int MogTab = 0;

        int ChocoGame = 0;
        int ChocoSplitNum = 0;
        string[] ChocoSplits;
        string[] ChocoGameEnd = {  "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", 
                                     "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00",
                                     "00:00:00", "00:00:00"};
        string ChocoFinish;
        bool ChocoFinished = false;
        int ChocoTab = 0;

        int TonbGame = 0;
        int TonbSplitNum = 0;
        string[] TonbSplits;
        string[] TonbGameEnd = {  "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", 
                                     "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00",
                                     "00:00:00", "00:00:00"};
        string TonbFinish;
        bool TonbFinished = false;
        int TonbTab = 0;

        string[] MogGameEndArchive = {  "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", 
                                     "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00",
                                     "00:00:00", "00:00:00"};
        string[] ChocoGameEndArchive = {  "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", 
                                     "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00",
                                     "00:00:00", "00:00:00"};
        string[] TonbGameEndArchive = {  "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", 
                                     "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00",
                                     "00:00:00", "00:00:00"};

        MetaContext meta;

        TeamControl[] teams;

        public Form1()
        {
            InitializeComponent();
            tick = new Timer();
            if (File.Exists("splits.txt"))
            {
                Splits = File.ReadAllLines("splits.txt");
                MogSplits = new string[Splits.Length];
                ChocoSplits = new string[Splits.Length];
                TonbSplits = new string[Splits.Length];
                for (int i = 0; i < Splits.Length; i++)
                { MogSplits[i] = "00:00:00"; ChocoSplits[i] = "00:00:00"; TonbSplits[i] = "00:00:00"; }
            }

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(0, Keys.F1);
            hook.RegisterHotKey(0, Keys.F2);
            hook.RegisterHotKey(0, Keys.F3);

            MetaFile metaFile = JsonConvert.DeserializeObject<MetaFile>(File.ReadAllText("meta.json"));

            string[] teamNames = metaFile.teams.Select(team => team.name).ToArray();

            //Programmatic stuff
            meta = new MetaContext(metaFile.splitsToShow,metaFile.splitFocusOffset,Splits,teamNames,metaFile.games);
            teams = new TeamControl[metaFile.teams.Length];
            //Create team controls based on the meta file.
            int wide = metaFile.teamsPerRow;
            int height = (metaFile.teams.Length / metaFile.teamsPerRow);
            for(int i = 0; i<metaFile.teams.Length; i++)
            {
                teams[i] = new TeamControl();
                int row = i / metaFile.teamsPerRow;
                teams[i].Location = new Point(15 + i*440,120+440*row);
                MetaFile.Team team = metaFile.teams[i];
                teams[i].setupTeamControl(this, new TeamInfo(metaFile.games.Length, Splits.Length, team.name, team.name + "-runners.txt",
                    ColorTranslator.FromHtml(team.color), Image.FromFile(team.image)), meta);
                this.Controls.Add(teams[i]);
            }
            loadCommentators();
            this.Size = new Size(30 + wide * 440, 150 + 440 * height);
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            int i = -1;
            switch (e.Key)
            {
                case Keys.F1:
                    i = 0;
                    break;
                case Keys.F2:
                    i = 1;
                    break;
                case Keys.F3:
                    i=2;
                    break;
            }
            if (i > -1 && i < teams.Length)
            {
                teams[i].TeamSplitButton_Click(this, EventArgs.Empty);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!MainTimerRunning)
            {
                //Start the timer, do some stuff
                MainTimerRunning = true;
                TimerStart = DateTime.Now;
                tick.Enabled = true;
                tick.Interval = 1;
                tick.Start();
                tick.Tick += new EventHandler(TimerEventProcessor);
                tick.Interval = 250;
                ChangedThisMin = true;
            }
            else
            {
                //Not sure if we want to stop the timer, give a dialog box?
            }
        }

        void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            TimeSpan d = DateTime.Now - TimerStart;
            //TimeSpan d = DateTime.Now - TimerStart + TimeSpan.FromHours(60);
            //TimeSpan f = d.Add(TimeSpan.FromSeconds(1));
            string current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)d.TotalHours, d);
            //string current = d.ToString(@"hh\:mm\:ss");
            MainTimer.Text = current;
            bool toCycle = !ChangedThisMin;

            foreach (TeamControl team in teams)
            {
                team.updateTimerEvent(current, toCycle);
            }

            //This section auto cycles
            MinuteCount++;
            if (MinuteCount >= 10) //1 Minute = 60 seconds = 240 timer ticks
            {
                ChangedThisMin = false;
                MinuteCount = 0;
            }

        }

        private void button1_Click_1(object sender, EventArgs e) //Stop click warning
        {
            popup PU = new popup();
            DialogResult dr = PU.ShowDialog();
            if (dr == DialogResult.OK)
            {
                //Reset
                if (MainTimerRunning)
                {
                    TimerStart = new DateTime(0);
                    MainTimerRunning = false;
                    tick.Stop();
                    //Reset time?
                }
            }
            else
            {

            }
            PU.Dispose();
        }

        public void childTabChanged()
        {
            ChangedThisMin = true;
        }

        private void loadCommentators()
        {
            if (File.Exists("commentators.txt"))
            {
                Commentators = File.ReadAllLines("commentators.txt");
                meta.commentators = Commentators;
            }
            foreach(TeamControl team in teams){
                team.reloadRunnerInfo();
            }
        }

        private void CommUpdate_Click(object sender, EventArgs e)
        {
            loadCommentators();
            ReadSplitFiles();
        }

        public void WriteSplitFiles()
        {
            string sep = "|";
            string no = "00:00:00";
            if (File.Exists("splits_output.txt"))
            {
                File.Delete("splits_output.txt");
            }
            string[] lines = new string[Splits.Length + 1];
            //line[0] = "Time   | Mog   | Choco | Tonb  ";
            lines[0] = "Time   | Mog   | Choco | Tonb  ";
            for (int i = 0; i < Splits.Length; i++)
            {
                lines[i + 1] = Splits[i] + sep + (MogSplitNum > i ? MogSplits[i] : no) + sep + (ChocoSplitNum > i ? ChocoSplits[i] : no) + sep + (TonbSplitNum > i ? TonbSplits[i] : no);
            }
            File.WriteAllLines("splits_output.txt", lines);
        }

        private void ReadSplitFiles()
        {
            if (File.Exists("splits_output.txt"))
            {
                string[] lines = File.ReadAllLines("splits_output.txt");
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] split = lines[i].Split('|');
                    if (MogSplitNum > i - 1)
                    {
                        MogSplits[i - 1] = split[1];
                    }
                    if (ChocoSplitNum > i - 1)
                    {
                        ChocoSplits[i - 1] = split[2];
                    }
                    if (TonbSplitNum > i - 1)
                    {
                        TonbSplits[i - 1] = split[3];
                    }
                }
            }
        }

        public VersusWrapper[] fetchOtherTeamInfo(TeamControl self)
        {
            VersusWrapper[] wrapperArray = new VersusWrapper[teams.Length - 1];
            int adjustedIndex = 0;
            for (int i = 0; i < teams.Length; i++)
            {
                if (self == teams[i]) { continue; }
                wrapperArray[adjustedIndex] = new VersusWrapper(teams[i].teamInfo.teamSplitNum, teams[i].teamInfo.teamSplits);
                adjustedIndex++;
            }
            return wrapperArray;
        }
    }
}
