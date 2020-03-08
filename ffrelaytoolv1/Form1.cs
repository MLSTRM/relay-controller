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
            
            ChocoCooldown = new Timer();
            MogCooldown = new Timer();
            TonbCooldown = new Timer();
            UpdateMogSplits();
            UpdateChocoSplits();
            UpdateTonbSplits();
            loadCommentators();
            //chocoIcon = mogIcon = tonbIcon = 17;
            CyclePurpleIcon();
            CycleBlueIcon();
            CycleGreenIcon();

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(0, Keys.F1);
            hook.RegisterHotKey(0, Keys.F2);
            hook.RegisterHotKey(0, Keys.F3);

            //Programmatic stuff
            MetaContext meta = new MetaContext(4,2,Splits,new string[]{"mog","choco","tonb"},new string[]{"FF1","FF2","FF3","FF4","FF5","FF6","FF7","FFT","FF8","9","X","12","13","15"});
            teams = new TeamControl[3];
            teams[0] = teamControl1;
            teamControl1.setupTeamControl(this, new TeamInfo(14, Splits.Length, "mog", "mog-runners.txt", Color.Blue, Properties.Resources.mog_box),meta);
            teams[1] = teamControl2;
            teamControl2.setupTeamControl(this, new TeamInfo(14, Splits.Length, "choco", "choco-runners.txt", Color.HotPink, Properties.Resources.choco_box), meta);
            teams[2] = teamControl3;
            teamControl3.setupTeamControl(this, new TeamInfo(14, Splits.Length, "tonb", "tonb-runners.txt", Color.Green, Properties.Resources.tonb_box), meta);
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F1:
                    MogSplit_Click(this, EventArgs.Empty);
                    break;
                case Keys.F2:
                    ChocoSplit_Click(this, EventArgs.Empty);
                    break;
                case Keys.F3:
                    TonbSplit_Click(this, EventArgs.Empty);
                    break;
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
            MogSplits[MogSplitNum] = current;
            ChocoSplits[ChocoSplitNum] = current;
            TonbSplits[TonbSplitNum] = current;
            if (!MogFinished) { UpdateMogSplits(); }
            if (!ChocoFinished) { UpdateChocoSplits(); }
            if (!TonbFinished) { UpdateTonbSplits(); }

            //Programmatic
            foreach (TeamControl team in teams)
            {
                team.updateTimerEvent(current, MinuteCount >= 60 && !ChangedThisMin);
            }
            //Programmatic end

            //This section auto cycles
            MinuteCount++;
            if (MinuteCount >= 60) //1 Minute = 60 seconds = 240 timer ticks
            {
                if (!ChangedThisMin)
                {
                    if (infomog.SelectedIndex == 2)
                    { infomog.SelectedIndex = 0; }
                    else { infomog.SelectedIndex++; }

                    if (infochoco.SelectedIndex == 2)
                    { infochoco.SelectedIndex = 0; }
                    else { infochoco.SelectedIndex++; }

                    if (infotonb.SelectedIndex == 2)
                    { infotonb.SelectedIndex = 0; }
                    else { infotonb.SelectedIndex++; }
                }
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

        private void cycleIcon(string teamName, int teamIcon, Label teamIconLabel, Label teamInfoCat1,
            Label teamInfoCat2, Label teamInfoCat3, Label teamCommentary, string[] runners, string[] commentators)
        {
            //teamIcon++;
            if (File.Exists("icon_" + teamIcon + ".png"))
            {
                File.Copy("icon_" + teamIcon + ".png", teamName + "-Icon.png", true);
                teamIconLabel.Text = "Cur: " + teamIcon;
            }
            else
            {
                File.Copy("icon_1.png", teamName + "-Icon.png", true);
                teamIcon = 1;
                teamIconLabel.Text = "Cur: 1";
            }
            teamInfoCat1.Text = runners[(teamIcon * 4) - 4];
            teamInfoCat2.Text = runners[(teamIcon * 4) - 3];
            teamInfoCat3.Text = runners[(teamIcon * 4) - 2];
            teamCommentary.Text = "Commentary: " + commentators[teamIcon - 1];
        }

        private void hbbutton_Click(object sender, EventArgs e)
        {
            CycleBlueIcon();
        }
        void CycleBlueIcon()
        {
            cycleIcon("mog", MogGame+1, MogIconlabel, MogInfoCat1, MogInfoCat2, MogInfoCat3, MogCommentary, MogRunners, Commentators);
        }

        private void hpbutton_Click(object sender, EventArgs e)
        {
            CyclePurpleIcon();
        }
        void CyclePurpleIcon()
        {
            cycleIcon("choco", ChocoGame+1, ChocoIconlabel, ChocoInfoCat1, ChocoInfoCat2, ChocoInfoCat3, ChocoCommentary, ChocoRunners, Commentators);
        }

        private void tonbIconButton_Click(object sender, EventArgs e)
        {
            CycleGreenIcon();
        }
        void CycleGreenIcon()
        {
            cycleIcon("tonb", TonbGame+1, TonbIconlabel, TonbInfoCat1, TonbInfoCat2, TonbInfoCat3, TonbCommentary, TonbRunners, Commentators);
        }

        private void splitClick(ref bool waiting, ref int splitNum, ref bool finished, ref string teamFinish,
            ref string[] teamSplits, ref string[] gameEnds, ref int teamGame, Label teamSplit4, string[] splits,
            Action cycleIcons, Timer cooldown, Action updateTeamSplits, EventHandler cooldownDone)
        {
            //Activate Cooldown
            if (!waiting)
            {
                if (splitNum >= splits.Length - 1)
                {
                    if (!finished)
                    {
                        teamFinish = teamSplits[splitNum];
                        gameEnds[teamGame] = teamFinish;
                        teamSplit4.Text = teamFinish;
                        finished = true;
                    }
                    return;
                }
                //Handle the splits. Showing 3 at a time, need to cycle games on end splits (Contains "Final Fantasy")
                //This year we need to catch LR and MQ. If we do "Lightning Returns: Final Fantasy XIII" it's too damn long, so we'll cut at LR
                //Catch that we're ending a game before we move onto the next one
                if (splits[splitNum].Contains(gameSep))
                {
                    //Assign the per-game timer to be our current split time, which is stored in teamSplits[splitNum]
                    gameEnds[teamGame] = teamSplits[splitNum];
                    //Move the current game along for tracking
                    teamGame++;
                    //Move onto the next game using the hand / icons
                    cycleIcons();
                }
                splitNum++;
                updateTeamSplits();
                cooldown.Enabled = true;
                cooldown.Interval = 5000;
                cooldown.Start();
                cooldown.Tick += new EventHandler(cooldownDone);
                waiting = true;
                WriteSplitFiles();
            }
        }

        String stripGameIndicator(String s)
        {
            return s.Replace(gameSep, "");
        }

        TimeSpan resolveTimeSpan(string a, string b)
        {
            TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
            TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
            TimeSpan seg = s1 - s2;
            return seg;
        }

        void updateDifferenceDisplay(Label label, TimeSpan seg)
        {
            string current = "";
            if (seg.TotalHours > -1)
            { if (seg.TotalSeconds < 0) { current += "-"; } else { current += "+"; } }
            current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
            label.Text = current;
        }

        private void MogSplit_Click(object sender, EventArgs e)
        {
            splitClick(ref MogWaiting, ref MogSplitNum, ref MogFinished, ref MogFinish, ref MogSplits, ref MogGameEnd, ref MogGame,
                MogSplitTime4, Splits, CycleBlueIcon, MogCooldown, UpdateMogSplits, MogCooldownDone);
        }
        void MogCooldownDone(Object myObject, EventArgs myEventArgs)
        { MogWaiting = false; MogCooldown.Stop(); }

        void UpdateMogSplits()
        {

        }

        private void ChocoSplit_Click(object sender, EventArgs e)
        {
            splitClick(ref ChocoWaiting, ref ChocoSplitNum, ref ChocoFinished, ref ChocoFinish, ref ChocoSplits, ref ChocoGameEnd, ref ChocoGame,
                ChocoSplitTime4, Splits, CyclePurpleIcon, ChocoCooldown, UpdateChocoSplits, ChocoCooldownDone);
        }
        void ChocoCooldownDone(Object myObject, EventArgs myEventArgs)
        { ChocoWaiting = false; ChocoCooldown.Stop(); }

        void UpdateChocoSplits()
        {
            
        }

        private void TonbSplit_Click(object sender, EventArgs e)
        {
            splitClick(ref TonbWaiting, ref TonbSplitNum, ref TonbFinished, ref TonbFinish, ref TonbSplits, ref TonbGameEnd, ref TonbGame,
                TonbSplitTime4, Splits, CycleGreenIcon, TonbCooldown, UpdateTonbSplits, TonbCooldownDone);
        }
        void TonbCooldownDone(Object myObject, EventArgs myEventArgs)
        { TonbWaiting = false; TonbCooldown.Stop(); }

        void UpdateTonbSplits()
        {
            
        }

        private void MogTab_Clicked(object sender, TabControlEventArgs e)
        {
            MogTab = e.TabPageIndex;
            ChangedThisMin = true;
        }

        private void ChocoTab_Clicked(object sender, TabControlEventArgs e)
        {
            ChocoTab = e.TabPageIndex;
            ChangedThisMin = true;
        }

        private void TonbTab_Clicked(object sender, TabControlEventArgs e)
        {
            TonbTab = e.TabPageIndex;
            ChangedThisMin = true;
        }

        private void loadCommentators()
        {
            if (File.Exists("commentators.txt"))
            {
                Commentators = File.ReadAllLines("commentators.txt");
            }
            if (File.Exists("mog-runners.txt"))
            {
                MogRunners = File.ReadAllLines("mog-runners.txt");
            }
            if (File.Exists("choco-runners.txt"))
            {
                ChocoRunners = File.ReadAllLines("choco-runners.txt");
            }
            if (File.Exists("tonb-runners.txt"))
            {
                TonbRunners = File.ReadAllLines("tonb-runners.txt");
            }
            MogCommentary.Text = "Commentary: " + Commentators[MogGame];
            ChocoCommentary.Text = "Commentary: " + Commentators[ChocoGame];
            TonbCommentary.Text = "Commentary: " + Commentators[TonbGame];
        }

        private void CommUpdate_Click(object sender, EventArgs e)
        {
            loadCommentators();
            ReadSplitFiles();
        }

        private void WriteSplitFiles()
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
            for (int i = 0; i < teams.Length - 1; i++)
            {
                if (self == teams[i]) { adjustedIndex++; }
                wrapperArray[i] = new VersusWrapper(teams[adjustedIndex].teamInfo.teamSplitNum, teams[adjustedIndex].teamInfo.teamSplits);
            }
            return wrapperArray;
        }
    }
}
