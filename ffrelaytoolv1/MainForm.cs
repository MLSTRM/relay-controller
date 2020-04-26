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
    public partial class MainForm : Form
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

        int timerTickInterval = 250;

        int infoCycleTicks = 40;

        int MinuteCount = 0;
        bool ChangedThisMin = false;

        string[] Splits;

        MetaContext meta;

        TeamControl[] teams;

        public MainForm()
        {
            InitializeComponent();
            tick = new Timer();
            if (File.Exists("splits.txt"))
            {
                Splits = File.ReadAllLines("splits.txt");
            }

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(0, Keys.F1);
            hook.RegisterHotKey(0, Keys.F2);
            hook.RegisterHotKey(0, Keys.F3);

            MetaFile metaFile = JsonConvert.DeserializeObject<MetaFile>(File.ReadAllText("meta.json"));

            string[] teamNames = metaFile.teams.Select(team => team.name).ToArray();

            timerTickInterval = metaFile.layout.timerTickInterval;
            infoCycleTicks = metaFile.layout.infoCycleTicks;

            //Programmatic stuff
            meta = new MetaContext(metaFile.splitsToShow, metaFile.splitFocusOffset, Splits, teamNames, metaFile.games, metaFile.layout);
            teams = new TeamControl[metaFile.teams.Length];
            //Create team controls based on the meta file.
            int wide = Math.Min(metaFile.teamsPerRow, metaFile.teams.Length);
            double height = Math.Ceiling((double)metaFile.teams.Length / (double)metaFile.teamsPerRow);
            Size teamSize = new Size(Math.Max(430, meta.layout.boxWidth + 30), Math.Max(400, meta.layout.boxHeight + meta.layout.timerHeight + 106));
            for(int i = 0; i<metaFile.teams.Length; i++)
            {
                teams[i] = new TeamControl();
                int row = i / metaFile.teamsPerRow;
                teams[i].Location = new Point(15 + (i%metaFile.teamsPerRow)*(teamSize.Width+10), 120+(teamSize.Height+10)* row);
                MetaFile.Team team = metaFile.teams[i];
                teams[i].setupTeamControl(this, new TeamInfo(metaFile.games.Length, Splits.Length, team.name, team.name.ToLower() + "-runners.txt",
                    ColorTranslator.FromHtml(team.color), Image.FromFile(team.image)), meta, teamSize);
                this.Controls.Add(teams[i]);
            }
            loadCommentators();
            this.Size = new Size(30 + wide * (teamSize.Width + 10), 150 + (teamSize.Height + 10) * (int)height);
            outputCaptureInformation();
        }

        private void outputCaptureInformation()
        {
            List<String> captureLines = new List<string>();
            captureLines.Add("Program Size: ");
            captureLines.Add(Util.outputCaptureInfo(this, this));
            captureLines.Add("Main timer:");
            captureLines.Add(Util.outputCaptureInfo(MainTimer, this));
            foreach(TeamControl team in teams)
            {
                captureLines.AddRange(team.outputCaptureInfo(this));
            }
            File.WriteAllLines("capture-info.txt", captureLines);
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
                tick.Interval = timerTickInterval;
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
            if (MinuteCount >= infoCycleTicks) //1 Minute = 60 seconds = 240 timer ticks
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

        public void childTabChanged() => ChangedThisMin = true;

        private void loadCommentators()
        {
            if (File.Exists("commentators.txt"))
            {
                meta.commentators = File.ReadAllLines("commentators.txt");
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
            if (File.Exists("splits_output.txt"))
            {
                File.Delete("splits_output.txt");
            }
            string[] lines = new string[Splits.Length + 1];
            //line[0] = "Time   | Mog   | Choco | Tonb  ";
            //Parameterised
            int timePad = Splits.Select(str => str.Length).Max();
            lines[0] = "Time".PadRight(timePad,' ');
            for (int i = 0; i < Splits.Length; i++)
            {
                lines[i + 1] = Splits[i].PadRight(timePad, ' ');
            }
            for (int i = 0; i < teams.Length; i++)
            {
                TeamControl team = teams[i];
                lines[0] += sep + team.teamInfo.teamName.PadRight(8,' ');
                for (int j = 0; j < Splits.Length; j++)
                {
                    lines[j + 1] += sep + team.getSplit(j);
                }
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
                    if (split.Length != teams.Length + 1)
                    {
                        warning PU = new warning();
                        PU.setWarning("WARN: line " + i + " did not match expected seperations.");
                        DialogResult dr = PU.ShowDialog();
                        PU.Dispose();
                        return;
                    }
                    for (var j = 1; j < split.Length; j++)
                    {
                        teams[j - 1].setSplit(split[j], i - 1);
                    }
                }
            }
        }

        public VersusWrapper[] fetchOtherTeamInfo(TeamControl self)
        {
            //teams.Except(new TeamControl[] { self }).Select(tc => new VersusWrapper(tc.teamInfo.teamSplitNum, tc.teamInfo.teamSplits, tc.teamInfo.teamFinished));
            VersusWrapper[] wrapperArray = new VersusWrapper[teams.Length - 1];
            int adjustedIndex = 0;
            for (int i = 0; i < teams.Length; i++)
            {
                if (self == teams[i]) { continue; }
                wrapperArray[adjustedIndex] = new VersusWrapper(teams[i].teamInfo.teamSplitNum, teams[i].teamInfo.teamSplits, teams[i].teamInfo.teamFinished);
                adjustedIndex++;
            }
            return wrapperArray;
        }
    }
}
