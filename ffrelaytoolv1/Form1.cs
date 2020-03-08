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
            if (File.Exists("commentators.txt"))
            {
                Commentators = File.ReadAllLines("commentators.txt");
            }
            ChocoCooldown = new Timer();
            MogCooldown = new Timer();
            TonbCooldown = new Timer();
            UpdateMogSplits();
            UpdateChocoSplits();
            UpdateTonbSplits();
            chocoIcon = mogIcon = tonbIcon = 17;
            CyclePurpleIcon();
            CycleBlueIcon();
            CycleGreenIcon();

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(0, Keys.F1);
            hook.RegisterHotKey(0, Keys.F2);
            hook.RegisterHotKey(0, Keys.F3);
            //File.Copy("bg_1.png", "background.png", true);
            //File.Copy("Rinfo_r1.png", "RInfo.png", true);
            //File.Copy("Linfo_l1.png", "LInfo.png", true);
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
                //UpdateMogIcons();
                //UpdateChocoIcons();
                //UpdateTonbIcons();
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
            //This section auto cycles
            MinuteCount++;
            if (MinuteCount >= 60) //1 Minute = 60 seconds = 240 timer ticks
            {
                if (!ChangedThisMin)
                {
                    //if (infomog.SelectedIndex == 3 || (infomog.SelectedIndex == 2 && MogGame == 0))
                    if (infomog.SelectedIndex == 2)
                    { infomog.SelectedIndex = 0; }
                    else { infomog.SelectedIndex++; }

                    //if (infochoco.SelectedIndex == 3 || (infochoco.SelectedIndex == 2 && ChocoGame == 0))
                    if (infochoco.SelectedIndex == 2)
                    { infochoco.SelectedIndex = 0; }
                    else { infochoco.SelectedIndex++; }

                   // if (infotonb.SelectedIndex == 3 || (infotonb.SelectedIndex == 2 && TonbGame == 0))
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

        private void cycleIcon(string teamName, ref int teamIcon, Label teamIconLabel, Label teamInfoCat1, 
            Label teamInfoCat2, Label teamInfoCat3, Label teamCommentary, string[] runners, string[] commentators)
        {
            teamIcon++;
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
            cycleIcon("mog", ref mogIcon, MogIconlabel, MogInfoCat1, MogInfoCat2, MogInfoCat3, MogCommentary, MogRunners, Commentators);
        }

        private void hpbutton_Click(object sender, EventArgs e)
        {
            CyclePurpleIcon();
        }
        void CyclePurpleIcon()
        {
            cycleIcon("choco", ref chocoIcon, ChocoIconlabel, ChocoInfoCat1, ChocoInfoCat2, ChocoInfoCat3, ChocoCommentary, ChocoRunners, Commentators);
        }

        private void tonbIconButton_Click(object sender, EventArgs e)
        {
            CycleGreenIcon();
        }
        void CycleGreenIcon()
        {            
            cycleIcon("tonb", ref tonbIcon, TonbIconlabel, TonbInfoCat1, TonbInfoCat2, TonbInfoCat3, TonbCommentary, TonbRunners, Commentators);
        }

        private void splitClick(ref bool waiting, ref int splitNum, ref bool finished, ref string teamFinish, 
            ref string[] teamSplits, ref string[] gameEnds, ref int teamGame, Label teamSplit4, ref string[] splits, 
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

        private void MogSplit_Click(object sender, EventArgs e)
        {
            splitClick(ref MogWaiting, ref MogSplitNum, ref MogFinished, ref MogFinish,ref MogSplits,ref MogGameEnd, ref MogGame, 
                MogSplitTime4, ref Splits, CycleBlueIcon, MogCooldown, UpdateMogSplits, MogCooldownDone);          
        }
        void MogCooldownDone(Object myObject, EventArgs myEventArgs)
        { MogWaiting = false; MogCooldown.Stop(); }

        String stripGameIndicator(String s)
        {
            return s.Replace(gameSep,"");
        }

        class splitsAndNum
        {
            public int splitNum;
            public string[] splits;
            public Label vsLabel;
            public splitsAndNum(int num, string[] splits, Label label){
                this.splitNum = num;
                this.splits = splits;
                this.vsLabel = label;
            }
        }

        void updateSplits(int splitNum, string[] Splits, int teamGame, int numberOfGames, 
            Label teamSplitName1, Label teamSplitName2, Label teamSplitName3, Label teamSplitName4,
            Label teamSplitTime1, Label teamSplitTime2, Label teamSplitTime3, Label teamSplitTime4, string[] teamSplits, splitsAndNum[] otherTeams,
            ref string[] teamGameEndArchive, string[] teamGameEnds, Label teamGameTimerL, Label teamGameEndL, Label teamTimer, Label teamGameTimerR)
        {
            int i = splitNum;
            if (i == 0) { i += 2; }
            else if (i == 1) { i++; }
            else if (i == Splits.Length - 1) { i--; }
            teamSplitName1.Text = stripGameIndicator(Splits[i - 2]);
            teamSplitName2.Text = stripGameIndicator(Splits[i - 1]);
            teamSplitName3.Text = stripGameIndicator(Splits[i]);
            teamSplitName4.Text = stripGameIndicator(Splits[i + 1]);
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
                    if (otherTeams[team].splitNum == splitNum)
                    {
                        vs[team] = true;
                    }
                    if (!vs[team] && otherTeams[team].splits[splitNum - offset] != "00:00:00" )
                    {
                        string a = teamSplits[splitNum - offset];
                        string b = otherTeams[team].splits[splitNum - offset];
                        TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                        TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                        TimeSpan seg = s1 - s2;
                        string current = "";
                        if (seg.TotalHours > -1)
                        { if (seg.TotalSeconds < 0) { current += "-"; } else { current += "+"; } }
                        current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                        otherTeams[team].vsLabel.Text = current;
                        vs[team] = true;
                    }
                }
                if(vs.All(b=>b)){
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
            string lefttimes = MogGameEnd[0] + "\n";
            string righttimes = "";
            for (int j = i; j< numberOfGames; j++)
            {
                string current = "00:00:00";
                //If we're past the selected game, then subtract the previous one to get the segment time over split time
                if (teamGame > j)
                {
                    //TimeSpan seg = TimeSpan.Parse(MogGameEnd[j]) - TimeSpan.Parse(MogGameEnd[j - 1]);
                    string a = teamGameEnds[j];
                    string b = teamGameEnds[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (teamGame == j)
                {
                    //TimeSpan seg = TimeSpan.Parse(MogSplits[MogSplitNum]) - TimeSpan.Parse(MogGameEnd[j - 1]);
                    string a = teamSplits[splitNum];
                    string b = teamGameEnds[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
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

        void UpdateMogSplits()
        {
            updateSplits(MogSplitNum, Splits, MogGame, 14, MogSplitName1, MogSplitName2, MogSplitName3, MogSplitName4,
                MogSplitTime1, MogSplitTime2, MogSplitTime3, MogSplitTime4, MogSplits, 
                new splitsAndNum[] { new splitsAndNum(ChocoSplitNum, ChocoSplits, MogSplitVs1), new splitsAndNum(TonbSplitNum, TonbSplits, MogSplitVs2) }, 
                ref MogGameEndArchive, MogGameEnd, MogGameTimersL, MogGameEndL, MogTimer, MogGameTimersR);
        }

        private void ChocoSplit_Click(object sender, EventArgs e)
        {
            splitClick(ref ChocoWaiting, ref ChocoSplitNum, ref ChocoFinished, ref ChocoFinish, ref ChocoSplits, ref ChocoGameEnd, ref ChocoGame,
                ChocoSplitTime4, ref Splits, CyclePurpleIcon, ChocoCooldown, UpdateChocoSplits, ChocoCooldownDone);            
        }
        void ChocoCooldownDone(Object myObject, EventArgs myEventArgs)
        { ChocoWaiting = false; ChocoCooldown.Stop(); }

        void UpdateChocoSplits()
        {
            updateSplits(ChocoSplitNum, Splits, ChocoGame, 14, ChocoSplitName1, ChocoSplitName2, ChocoSplitName3, ChocoSplitName4,
                ChocoSplitTime1, ChocoSplitTime2, ChocoSplitTime3, ChocoSplitTime4, ChocoSplits,
                new splitsAndNum[] { new splitsAndNum(MogSplitNum, MogSplits, ChocoSplitVs1), new splitsAndNum(TonbSplitNum, TonbSplits, ChocoSplitVs2) },
                ref ChocoGameEndArchive, ChocoGameEnd, ChocoGameTimersL, ChocoGameEndL, ChocoTimer, ChocoGameTimersR);
        }

        private void TonbSplit_Click(object sender, EventArgs e)
        {
            splitClick(ref TonbWaiting, ref TonbSplitNum, ref TonbFinished, ref TonbFinish, ref TonbSplits, ref TonbGameEnd, ref TonbGame,
                TonbSplitTime4, ref Splits, CycleGreenIcon, TonbCooldown, UpdateTonbSplits, TonbCooldownDone);
        }
        void TonbCooldownDone(Object myObject, EventArgs myEventArgs)
        { TonbWaiting = false; TonbCooldown.Stop(); }

        void UpdateTonbSplits()
        {
            updateSplits(TonbSplitNum, Splits, TonbGame, 14, TonbSplitName1, TonbSplitName2, TonbSplitName3, TonbSplitName4,
                TonbSplitTime1, TonbSplitTime2, TonbSplitTime3, TonbSplitTime4, TonbSplits,
                new splitsAndNum[] { new splitsAndNum(MogSplitNum, MogSplits, TonbSplitVs1), new splitsAndNum(ChocoSplitNum, ChocoSplits, TonbSplitVs2) },
                ref TonbGameEndArchive, TonbGameEnd, TonbGameTimersL, TonbGameEndL, TonbTimer, TonbGameTimersR);
        }

        private void MogTab_Clicked(object sender, TabControlEventArgs e)
        {
            MogTab = e.TabPageIndex;
            //UpdateMogIcons();
            ChangedThisMin = true;
        }

        /*void UpdateMogIcons()
        {
            //Mainly used to move hands around
            switch (MogTab)
            {
                case 0:
                    //Splits, 1,3,5,7
                    if (MogSplitNum == 0)
                    { File.Copy("Linfo_l1.png", "Linfo.png", true); break; }
                    if (MogSplitNum == 1)
                    { File.Copy("Linfo_l3.png", "Linfo.png", true); break; }
                    else if (MogSplitNum == Splits.Length - 1)
                    { File.Copy("Linfo_l7.png", "Linfo.png", true); break; }
                    else
                    { File.Copy("Linfo_l5.png", "Linfo.png", true); }
                    break;
                case 2:
                case 5:
                case 3:
                case 6:
                    if (MogGame == 16)
                    { File.Copy("Linfo_l8.png", "Linfo.png", true); break; }
                    if ((MogGame >= 8 && MogGame != 16 && (MogTab == 2 || MogTab == 5)) || (MogGame < 8 && (MogTab == 3 || MogTab == 6)))
                    {
                        File.Copy("Linfo_l0.png", "Linfo.png", true); break; //Top if on "wrong" page
                    }
                    switch (MogGame % 8) //2,4,5,6 each side, 8 at bottom, same side
                    {
                        case 0:
                            File.Copy("Linfo_l2.png", "Linfo.png", true);
                            break;
                        case 1:
                            File.Copy("Linfo_l4.png", "Linfo.png", true);
                            break;
                        case 2:
                            File.Copy("Linfo_l5.png", "Linfo.png", true);
                            break;
                        case 3:
                            File.Copy("Linfo_l6.png", "Linfo.png", true);
                            break;
                        case 4:
                            File.Copy("Linfo_r2.png", "Linfo.png", true);
                            break;
                        case 5:
                            File.Copy("Linfo_r4.png", "Linfo.png", true);
                            break;
                        case 6:
                            File.Copy("Linfo_r5.png", "Linfo.png", true);
                            break;
                        case 7:
                            File.Copy("Linfo_r6.png", "Linfo.png", true);
                            break;
                    }
                    break;
                default:
                    //Game Information/Commentators, use 1
                    File.Copy("Linfo_l1.png", "Linfo.png", true);
                    break;
            }
        }
        */

        private void ChocoTab_Clicked(object sender, TabControlEventArgs e)
        {
            ChocoTab = e.TabPageIndex;
            //UpdateChocoIcons();
            ChangedThisMin = true;
        }

        /*void UpdateChocoIcons()
        {
            //Mainly used to move hands around
            switch (ChocoTab)
            {
                case 0:
                    //Splits, 1,3,5,7
                    if (ChocoSplitNum == 0)
                    { File.Copy("Rinfo_r1.png", "Rinfo.png", true); break; }
                    if (ChocoSplitNum == 1)
                    { File.Copy("Rinfo_r3.png", "Rinfo.png", true); break; }
                    else if (ChocoSplitNum == Splits.Length - 1)
                    { File.Copy("RInfo_r7.png", "Rinfo.png", true); break; }
                    else
                    { File.Copy("RInfo_r5.png", "Rinfo.png", true); }
                    break;
                case 2:
                case 5:
                case 3:
                case 6:
                    if (ChocoGame == 16)
                    { File.Copy("Rinfo_r8.png", "RInfo.png", true); break; }
                    if ((ChocoGame >= 8 && ChocoGame != 16 && (ChocoTab == 2 || ChocoTab == 5)) || (ChocoGame < 8 && (ChocoTab == 3 || ChocoTab == 6)))
                    {
                        File.Copy("Rinfo_r0.png", "RInfo.png", true); break; //Top if on "wrong" page
                    }
                    switch (ChocoGame % 8) //2,4,5,6 each side, 8 at bottom, same side
                    {
                        case 0:
                            File.Copy("Rinfo_l2.png", "RInfo.png", true);
                            break;
                        case 1:
                            File.Copy("Rinfo_l4.png", "RInfo.png", true);
                            break;
                        case 2:
                            File.Copy("Rinfo_l5.png", "RInfo.png", true);
                            break;
                        case 3:
                            File.Copy("Rinfo_l6.png", "RInfo.png", true);
                            break;
                        case 4:
                            File.Copy("Rinfo_r2.png", "RInfo.png", true);
                            break;
                        case 5:
                            File.Copy("Rinfo_r4.png", "RInfo.png", true);
                            break;
                        case 6:
                            File.Copy("Rinfo_r5.png", "RInfo.png", true);
                            break;
                        case 7:
                            File.Copy("Rinfo_r6.png", "RInfo.png", true);
                            break;
                    }
                    break;
                default:
                    //Game Information/Commentators, use 1
                    File.Copy("Rinfo_r1.png", "Rinfo.png", true);
                    break;
            }
        }
        */

        private void TonbTab_Clicked(object sender, TabControlEventArgs e)
        {
            TonbTab = e.TabPageIndex;
            //UpdateTonbIcons();
            ChangedThisMin = true;
        }

        /* void UpdateTonbIcons()
         {
             //Mainly used to move hands around
             switch (TonbTab)
             {
                 case 0:
                     //Splits, 1,3,5,7
                     if (TonbSplitNum == 0)
                     { File.Copy("Rinfo_r1.png", "Rinfo.png", true); break; }
                     if (TonbSplitNum == 1)
                     { File.Copy("Rinfo_r3.png", "Rinfo.png", true); break; }
                     else if (TonbSplitNum == Splits.Length - 1)
                     { File.Copy("RInfo_r7.png", "Rinfo.png", true); break; }
                     else
                     { File.Copy("RInfo_r5.png", "Rinfo.png", true); }
                     break;
                 case 2:
                 case 5:
                 case 3:
                 case 6:
                     if (TonbGame == 16)
                     { File.Copy("Rinfo_r8.png", "RInfo.png", true); break; }
                     if ((TonbGame >= 8 && TonbGame != 16 && (TonbTab == 2 || TonbTab == 5)) || (TonbGame < 8 && (TonbTab == 3 || TonbTab == 6)))
                     {
                         File.Copy("Rinfo_r0.png", "RInfo.png", true); break; //Top if on "wrong" page
                     }
                     switch (TonbGame % 8) //2,4,5,6 each side, 8 at bottom, same side
                     {
                         case 0:
                             File.Copy("Rinfo_l2.png", "RInfo.png", true);
                             break;
                         case 1:
                             File.Copy("Rinfo_l4.png", "RInfo.png", true);
                             break;
                         case 2:
                             File.Copy("Rinfo_l5.png", "RInfo.png", true);
                             break;
                         case 3:
                             File.Copy("Rinfo_l6.png", "RInfo.png", true);
                             break;
                         case 4:
                             File.Copy("Rinfo_r2.png", "RInfo.png", true);
                             break;
                         case 5:
                             File.Copy("Rinfo_r4.png", "RInfo.png", true);
                             break;
                         case 6:
                             File.Copy("Rinfo_r5.png", "RInfo.png", true);
                             break;
                         case 7:
                             File.Copy("Rinfo_r6.png", "RInfo.png", true);
                             break;
                     }
                     break;
                 default:
                     //Game Information/Commentators, use 1
                     File.Copy("Rinfo_r1.png", "Rinfo.png", true);
                     break;
             }
         }
         * */

        private void CommUpdate_Click(object sender, EventArgs e)
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
            MogCommentary.Text = "Commentary: " + Commentators[mogIcon - 1];//.Replace(",", "\n");
            ChocoCommentary.Text = "Commentary: " + Commentators[chocoIcon - 1];//.Replace(",", "\n");
            TonbCommentary.Text = Commentators[tonbIcon - 1].Replace(",", "\n");
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
            string[] line = new string[Splits.Length+1];
            //line[0] = "Time   | Mog   | Choco | Tonb  ";
            line[0] = "Time   | Mog   | Choco | Tonb  ";
            for (int i = 0; i < Splits.Length; i++)
            {
                line[i + 1] = Splits[i] + sep + (MogSplitNum > i ? MogSplits[i] : no) + sep + (ChocoSplitNum > i ? ChocoSplits[i] : no) + sep + (TonbSplitNum > i ? TonbSplits[i] : no);
            }
            File.WriteAllLines("splits_output.txt", line);
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
                ResolveOffsets();
            }
        }

        private void ResolveOffsets()
        {
            string s1 = MogSplits[MogSplitNum>0?MogSplitNum - 1:0];
            string s2 = ChocoSplits[ChocoSplitNum>0?ChocoSplitNum - 1:0];
            string s3 = TonbSplits[TonbSplitNum - 1];
            TimeSpan t1 = new TimeSpan(int.Parse(s1.Split(':')[0]), int.Parse(s1.Split(':')[1]), int.Parse(s1.Split(':')[2]));
            TimeSpan t2 = new TimeSpan(int.Parse(s2.Split(':')[0]), int.Parse(s2.Split(':')[1]), int.Parse(s2.Split(':')[2]));
            TimeSpan t3 = new TimeSpan(int.Parse(s3.Split(':')[0]), int.Parse(s3.Split(':')[1]), int.Parse(s3.Split(':')[2]));
            if (MogSplitNum == ChocoSplitNum)
            {
                //Mog 1, Choco 1
                TimeSpan seg1 = t1 - t2;
                string current = "";
                if (seg1.TotalHours > -1)
                { if (seg1.Seconds < 0) { current += "-"; } else { current += "+"; } }
                current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg1.TotalHours, seg1);
                MogSplitVs1.Text = current;
                string swapcurrent = (current.Substring(0,1)=="-"?"+":"-")+current.Substring(1);
                ChocoSplitVs1.Text = swapcurrent;
            }
            if (ChocoSplitNum == TonbSplitNum)
            {
                //Choco 2, Tonb 2
                TimeSpan seg2 = t2 - t3;
                string current = "";
                if (seg2.TotalHours > -1)
                { if (seg2.Seconds < 0) { current += "-"; } else { current += "+"; } }
                current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg2.TotalHours, seg2);
                ChocoSplitVs2.Text = current;
                string swapcurrent = (current.Substring(0, 1) == "-" ? "+" : "-") + current.Substring(1);
                TonbSplitVs2.Text = swapcurrent;
            }
            if (MogSplitNum == TonbSplitNum)
            {
                //Mog 2, Tonb 1
                TimeSpan seg3 = t1 - t3;
                string current = "";
                if (seg3.TotalHours > -1)
                { if (seg3.Seconds < 0) { current += "-"; } else { current += "+"; } }
                current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg3.TotalHours, seg3);
                MogSplitVs2.Text = current;
                string swapcurrent = (current.Substring(0, 1) == "-" ? "+" : "-") + current.Substring(1);
                TonbSplitVs1.Text = swapcurrent;
            }
        }
    }
}
