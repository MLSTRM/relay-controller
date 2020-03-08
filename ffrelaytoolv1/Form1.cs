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

        void UpdateMogSplits()
        {
            //Main splits
            //Want to have 4, active one is third
            int i = MogSplitNum;
            if (i == 0) { i += 2; }
            else if (i == 1) { i++; }
            else if (i == Splits.Length - 1) { i--; }
            MogSplitName1.Text = stripGameIndicator(Splits[i - 2]);
            MogSplitName2.Text = stripGameIndicator(Splits[i - 1]);
            MogSplitName3.Text = stripGameIndicator(Splits[i]);
            MogSplitName4.Text = stripGameIndicator(Splits[i + 1]);
            MogSplitTime1.Text = MogSplits[i - 2];
            MogSplitTime2.Text = MogSplits[i - 1];
            MogSplitTime3.Text = MogSplits[i];
            MogSplitTime4.Text = MogSplits[i + 1];

            //Split comparisons, since this works even on FF1 it needs to be here
            int offset = 0;
            bool vs1 = false;
            bool vs2 = false;
            while (offset <= MogSplitNum)
            {
                if (ChocoSplitNum == MogSplitNum)
                { vs1 = true; }
                if (TonbSplitNum == MogSplitNum)
                { vs2 = true; }
                if (ChocoSplits[MogSplitNum - offset] != "00:00:00" && !vs1)
                {
                    string a = MogSplits[MogSplitNum - offset];
                    string b = ChocoSplits[MogSplitNum - offset];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    string current = "";
                    if (seg.TotalHours > -1)
                    { if (seg.TotalSeconds < 0) { current += "-"; } else { current += "+"; } }
                    current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    MogSplitVs1.Text = current;
                    vs1 = true;
                }
                if (TonbSplits[MogSplitNum - offset] != "00:00:00" && !vs2)
                {
                    string a = MogSplits[MogSplitNum - offset];
                    string b = TonbSplits[MogSplitNum - offset];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    string current = "";
                    if (seg.TotalHours > -1)
                    { if (seg.Seconds < 0) { current += "-"; } else { current += "+"; } }
                    current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    MogSplitVs2.Text = current;
                    vs2 = true;
                }
                if (vs1 && vs2)
                { offset = MogSplitNum; }
                offset++;
            }


            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            MogGameEndArchive = MogGameEnd;
            if (MogGame == 0)
            {
                MogGameTimersL.Text = MogSplits[MogSplitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                MogGameEndL.Text = MogSplits[MogSplitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                MogTimer.Text = MogSplits[MogSplitNum];
                return;
            }
            string lefttimes = MogGameEnd[0] + "\n";
            string righttimes = "";
            string midtimes = "";


            for (int j = 1; j < 17; j++)
            {
                string current = "00:00:00";
                //If we're past the selected game, then subtract the previous one to get the segment time over split time
                if (MogGame > j)
                {
                    //TimeSpan seg = TimeSpan.Parse(MogGameEnd[j]) - TimeSpan.Parse(MogGameEnd[j - 1]);
                    string a = MogGameEnd[j];
                    string b = MogGameEnd[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (MogGame == j)
                {
                    //TimeSpan seg = TimeSpan.Parse(MogSplits[MogSplitNum]) - TimeSpan.Parse(MogGameEnd[j - 1]);
                    string a = MogSplits[MogSplitNum];
                    string b = MogGameEnd[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    MogTimer.Text = current;
                    MogGameEndArchive[MogGame] = MogSplits[MogSplitNum];
                }
                /*if (j < 8)
                { lefttimes += current + "\n"; }
                else if (j < 16)
                { righttimes += current + "\n"; }
                else
                { midtimes += current + "\n"; }*/
                if (j < 7)
                { lefttimes += current + "\n"; }
                else
                { righttimes += current + "\n"; }
            }


            MogGameTimersL.Text = lefttimes;
            MogGameTimersR.Text = righttimes;
            //MogGameTimers2M.Text = midtimes;
            /*string E1L = "";
            string E1R = "";

            for (int k = 0; k < 7; k++)
            {
                E1L += MogGameEnd[k] + "\n";
                E1R += MogGameEnd[k + 7] + "\n";
            }
            MogGameEndL.Text = E1L;
            MogGameEndR.Text = E1R;
            MogGameEnd2M.Text = MogGameEndArchive[16];*/
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
            //Main splits
            int i = ChocoSplitNum;
            if (i == 0) { i += 2; }
            else if (i == 1) { i++; }
            else if (i == Splits.Length - 1) { i--; }
            ChocoSplitName1.Text = stripGameIndicator(Splits[i - 2]);
            ChocoSplitName2.Text = stripGameIndicator(Splits[i - 1]);
            ChocoSplitName3.Text = stripGameIndicator(Splits[i]);
            ChocoSplitName4.Text = stripGameIndicator(Splits[i + 1]);
            ChocoSplitTime1.Text = ChocoSplits[i - 2];
            ChocoSplitTime2.Text = ChocoSplits[i - 1];
            ChocoSplitTime3.Text = ChocoSplits[i];
            ChocoSplitTime4.Text = ChocoSplits[i + 1];

            //Split comparisons, since this works even on FF1 it needs to be here
            int offset = 0;
            bool vs1 = false;
            bool vs2 = false;
            while (offset <= ChocoSplitNum)
            {
                if (ChocoSplitNum == MogSplitNum)
                { vs1 = true; }
                if (TonbSplitNum == ChocoSplitNum)
                { vs2 = true; }
                if (MogSplits[ChocoSplitNum - offset] != "00:00:00" && !vs1)
                {
                    string a = MogSplits[ChocoSplitNum - offset];
                    string b = ChocoSplits[ChocoSplitNum - offset];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s2 - s1;
                    string current = "";
                    if (seg.TotalHours > -1)
                    { if (seg.TotalSeconds < 0) { current += "-"; } else { current += "+"; } }
                    current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    ChocoSplitVs1.Text = current;
                    vs1 = true;
                }
                if (TonbSplits[ChocoSplitNum - offset] != "00:00:00" && !vs2)
                {
                    string a = ChocoSplits[ChocoSplitNum - offset];
                    string b = TonbSplits[ChocoSplitNum - offset];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    string current = "";
                    if (seg.TotalHours > -1)
                    { if (seg.Seconds < 0) { current += "-"; } else { current += "+"; } }
                    current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    ChocoSplitVs2.Text = current;
                    vs2 = true;
                }
                if (vs1 && vs2)
                { offset = ChocoSplitNum; }
                offset++;
            }

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            ChocoGameEndArchive = ChocoGameEnd;
            if (ChocoGame == 0)
            {
                ChocoGameTimersL.Text = ChocoSplits[ChocoSplitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                ChocoGameEndL.Text = ChocoSplits[ChocoSplitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                ChocoTimer.Text = ChocoSplits[ChocoSplitNum];
                return;
            }
            string lefttimes = ChocoGameEnd[0] + "\n";
            string righttimes = "";
            string midtimes = "";
            for (int j = 1; j < 17; j++)
            {
                string current = "00:00:00";
                //If we're past the current game, then subtract the previous one to get the segment time over split time
                if (ChocoGame > j)
                {
                    //TimeSpan seg = TimeSpan.Parse(ChocoGameEnd[j]) - TimeSpan.Parse(ChocoGameEnd[j - 1]);
                    string a = ChocoGameEnd[j];
                    string b = ChocoGameEnd[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (ChocoGame == j)
                {
                    //TimeSpan seg = TimeSpan.Parse(ChocoSplits[ChocoSplitNum]) - TimeSpan.Parse(ChocoGameEnd[j - 1]);
                    string a = ChocoSplits[ChocoSplitNum];
                    string b = ChocoGameEnd[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    ChocoTimer.Text = current;
                    ChocoGameEndArchive[ChocoGame] = ChocoSplits[ChocoSplitNum];
                }
                /*if (j < 8)
                { lefttimes += current + "\n"; }
                else if (j < 16)
                { righttimes += current + "\n"; }
                else
                { midtimes += current + "\n"; }*/
                if (j < 7)
                { lefttimes += current + "\n"; }
                else
                { righttimes += current + "\n"; }
            }


            ChocoGameTimersL.Text = lefttimes;
            ChocoGameTimersR.Text = righttimes;
            //ChocoGameTimers2M.Text = midtimes;
            /*string E1L = "";
            string E1R = "";

            for (int k = 0; k < 8; k++)
            {
                E1L += ChocoGameEnd[k] + "\n";
                E1R += ChocoGameEnd[k + 8] + "\n";
            }
            ChocoGameEndL.Text = E1L;
            ChocoGameEndR.Text = E1R;
            ChocoGameEnd2M.Text = ChocoGameEndArchive[16];*/
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
            //Main splits
            int i = TonbSplitNum;
            if (i == 0) { i += 2; }
            else if (i == 1) { i++; }
            else if (i == Splits.Length - 1) { i--; }
            TonbSplitName1.Text = stripGameIndicator(Splits[i - 2]);
            TonbSplitName2.Text = stripGameIndicator(Splits[i - 1]);
            TonbSplitName3.Text = stripGameIndicator(Splits[i]);
            TonbSplitName4.Text = stripGameIndicator(Splits[i + 1]);
            TonbSplitTime1.Text = TonbSplits[i - 2];
            TonbSplitTime2.Text = TonbSplits[i - 1];
            TonbSplitTime3.Text = TonbSplits[i];
            TonbSplitTime4.Text = TonbSplits[i + 1];

            //Split comparisons, since this works even on FF1 it needs to be here
            int offset = 0;
            bool vs1 = false;
            bool vs2 = false;
            while (offset <= TonbSplitNum)
            {
                if (TonbSplitNum == MogSplitNum)
                { vs1 = true; }
                if (TonbSplitNum == ChocoSplitNum)
                { vs2 = true; }
                if (MogSplits[TonbSplitNum - offset] != "00:00:00" && !vs1)
                {
                    string a = MogSplits[TonbSplitNum - offset];
                    string b = TonbSplits[TonbSplitNum - offset];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s2 - s1;
                    string current = "";
                    if (seg.TotalHours > -1)
                    { if (seg.Seconds < 0) { current += "-"; } else { current += "+"; } }
                    current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    TonbSplitVs1.Text = current;
                    vs1 = true;
                }
                if (ChocoSplits[TonbSplitNum - offset] != "00:00:00" && !vs2)
                {
                    string a = ChocoSplits[TonbSplitNum - offset];
                    string b = TonbSplits[TonbSplitNum - offset];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s2 - s1;
                    string current = "";
                    if (seg.TotalHours > -1)
                    { if (seg.Seconds < 0) { current += "-"; } else { current += "+"; } }
                    current += string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    TonbSplitVs2.Text = current;
                    vs2 = true;
                }
                if (vs1 && vs2)
                { offset = TonbSplitNum; }
                offset++;
            }

            //Per Game Splits
            //Since we're always at least on FF1, just include the time for it in here, removes the special case later
            TonbGameEndArchive = TonbGameEnd;
            if (TonbGame == 0)
            {
                TonbGameTimersL.Text = TonbSplits[TonbSplitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                TonbGameEndL.Text = TonbSplits[TonbSplitNum] + "\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00\n00:00:00";
                TonbTimer.Text = TonbSplits[TonbSplitNum];
                return;
            }
            string lefttimes = TonbGameEnd[0] + "\n";
            string righttimes = "";
            string midtimes = "";
            for (int j = 1; j < 17; j++)
            {
                string current = "00:00:00";
                //If we're past the current game, then subtract the previous one to get the segment time over split time
                if (TonbGame > j)
                {
                    //TimeSpan seg = TimeSpan.Parse(TonbGameEnd[j]) - TimeSpan.Parse(TonbGameEnd[j - 1]);
                    string a = TonbGameEnd[j];
                    string b = TonbGameEnd[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (TonbGame == j)
                {
                    //TimeSpan seg = TimeSpan.Parse(TonbSplits[TonbSplitNum]) - TimeSpan.Parse(TonbGameEnd[j - 1]);
                    string a = TonbSplits[TonbSplitNum];
                    string b = TonbGameEnd[j - 1];
                    TimeSpan s1 = new TimeSpan(int.Parse(a.Split(':')[0]), int.Parse(a.Split(':')[1]), int.Parse(a.Split(':')[2]));
                    TimeSpan s2 = new TimeSpan(int.Parse(b.Split(':')[0]), int.Parse(b.Split(':')[1]), int.Parse(b.Split(':')[2]));
                    TimeSpan seg = s1 - s2;
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                    TonbTimer.Text = current;
                    TonbGameEndArchive[TonbGame] = TonbSplits[TonbSplitNum];
                }
                /*if (j < 8)
                { lefttimes += current + "\n"; }
                else if (j < 16)
                { righttimes += current + "\n"; }
                else
                { midtimes += current + "\n"; }*/
                if (j < 7)
                { lefttimes += current + "\n"; }
                else
                { righttimes += current + "\n"; }
            }


            TonbGameTimersL.Text = lefttimes;
            TonbGameTimersR.Text = righttimes;
            //TonbGameTimers2M.Text = midtimes;
            /*string E1L = "";
            string E1R = "";

            for (int k = 0; k < 8; k++)
            {
                E1L += TonbGameEnd[k] + "\n";
                E1R += TonbGameEnd[k + 8] + "\n";
            }
            TonbGameEndL.Text = E1L;
            TonbGameEndR.Text = E1R;
            TonbGameEnd2M.Text = TonbGameEndArchive[16];*/
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
