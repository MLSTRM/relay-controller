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
            splitClick(ref MogWaiting, ref MogSplitNum, ref MogFinished, ref MogFinish, ref MogSplits, ref MogGameEnd, ref MogGame,
                MogSplitTime4, ref Splits, CycleBlueIcon, MogCooldown, UpdateMogSplits, MogCooldownDone);
        }
        void MogCooldownDone(Object myObject, EventArgs myEventArgs)
        { MogWaiting = false; MogCooldown.Stop(); }

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

        void updateSplits(int splitNum, string[] Splits, int teamGame, int numberOfGames,
            Label teamSplitName1, Label teamSplitName2, Label teamSplitName3, Label teamSplitName4,
            Label teamSplitTime1, Label teamSplitTime2, Label teamSplitTime3, Label teamSplitTime4, string[] teamSplits, VersusWrapper[] otherTeams,
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
                    //If we're on the same split, then don't update the versus differences.
                    if (otherTeams[team].splitNum == splitNum)
                    {
                        vs[team] = true;
                        if (splitNum > 0)
                        {
                            TimeSpan seg = resolveTimeSpan(teamSplits[splitNum - 1], otherTeams[team].splits[splitNum - 1]);
                            updateDifferenceDisplay(otherTeams[team].vsLabel, seg);
                        }
                    }
                    //Otherwise update the value based on the live difference (Only if greater than static difference?)
                    if (!vs[team] && otherTeams[team].splits[splitNum - offset] != "00:00:00")
                    {
                        TimeSpan seg = resolveTimeSpan(teamSplits[splitNum - offset], otherTeams[team].splits[splitNum - offset]);
                        if (splitNum - offset > 0)
                        {
                            //If the offset on the previous split is larger than the live one then use that.
                            TimeSpan olderseg = resolveTimeSpan(teamSplits[splitNum - offset - 1], otherTeams[team].splits[splitNum - offset - 1]);
                            if (Math.Abs(olderseg.Ticks) > Math.Abs(seg.Ticks))
                            {
                                seg = olderseg;
                            }
                        }
                        updateDifferenceDisplay(otherTeams[team].vsLabel, seg);
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
            string lefttimes = MogGameEnd[0] + "\n";
            string righttimes = "";
            for (int j = i; j < numberOfGames; j++)
            {
                string current = "00:00:00";
                //If we're past the selected game, then subtract the previous one to get the segment time over split time
                if (teamGame > j)
                {
                    TimeSpan seg = resolveTimeSpan(teamGameEnds[j], teamGameEnds[j - 1]);
                    current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)seg.TotalHours, seg);
                }
                else if (teamGame == j)
                {
                    TimeSpan seg = resolveTimeSpan(teamSplits[splitNum], teamGameEnds[j - 1]);
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
                new VersusWrapper[] { new VersusWrapper(ChocoSplitNum, ChocoSplits, MogSplitVs1), new VersusWrapper(TonbSplitNum, TonbSplits, MogSplitVs2) },
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
                new VersusWrapper[] { new VersusWrapper(MogSplitNum, MogSplits, ChocoSplitVs1), new VersusWrapper(TonbSplitNum, TonbSplits, ChocoSplitVs2) },
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
                new VersusWrapper[] { new VersusWrapper(MogSplitNum, MogSplits, TonbSplitVs1), new VersusWrapper(ChocoSplitNum, ChocoSplits, TonbSplitVs2) },
                ref TonbGameEndArchive, TonbGameEnd, TonbGameTimersL, TonbGameEndL, TonbTimer, TonbGameTimersR);
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
            MogCommentary.Text = "Commentary: " + Commentators[mogIcon - 1];
            ChocoCommentary.Text = "Commentary: " + Commentators[chocoIcon - 1];
            TonbCommentary.Text = "Commentary: " + Commentators[tonbIcon - 1];
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
            string[] line = new string[Splits.Length + 1];
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
            string s1 = MogSplits[MogSplitNum > 0 ? MogSplitNum - 1 : 0];
            string s2 = ChocoSplits[ChocoSplitNum > 0 ? ChocoSplitNum - 1 : 0];
            string s3 = TonbSplits[TonbSplitNum > 0 ? TonbSplitNum - 1 : 0];
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
                string swapcurrent = (current.Substring(0, 1) == "-" ? "+" : "-") + current.Substring(1);
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
