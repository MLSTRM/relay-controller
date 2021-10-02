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
using ffrelaytoolv1.Cloud;
using FFRelayTool_Model;

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
        public DateTime TimerStart = new DateTime(0);

        int timerTickInterval = 250;

        int infoCycleTicks = 40;

        int MinuteCount = 0;
        bool ChangedThisMin = false;

        string[] Splits;

        MetaContext meta;

        TeamControl[] teams;

        private SSMUpdater updater;

        private SQSReader reader;

        private Timer sqsTimer;

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
            meta = new MetaContext(metaFile.splitsToShow, metaFile.splitFocusOffset, Splits,
                teamNames, metaFile.games, metaFile.layout, metaFile.features);
            teams = new TeamControl[metaFile.teams.Length];
            //Create team controls based on the meta file.
            int wide = Math.Min(metaFile.teamsPerRow, metaFile.teams.Length);
            double height = Math.Ceiling((double)metaFile.teams.Length / (double)metaFile.teamsPerRow);
            Size teamSize = new Size(Math.Max(430, meta.layout.boxWidth + 30), Math.Max(350, meta.layout.boxHeight + meta.layout.timerHeight + 56));
            for (int i = 0; i < metaFile.teams.Length; i++)
            {
                teams[i] = new TeamControl();
                int row = i / metaFile.teamsPerRow;
                teams[i].Location = new Point(15 + (i % metaFile.teamsPerRow) * (teamSize.Width + 10), 120 + (teamSize.Height + 10) * row);
                MetaFile.Team team = metaFile.teams[i];
                var backImage = team.image != null ? Image.FromFile(team.image) : null;
                teams[i].setupTeamControl(this, new TeamInfo(metaFile.games.Length, Splits.Length, team.name, team.name.ToLower() + "-runners.txt",
                    ColorTranslator.FromHtml(team.color), backImage, team.splitKey, metaFile.features.teamGameIcons), meta, teamSize);
                this.Controls.Add(teams[i]);
            }
            loadCommentators();
            this.Size = new Size(30 + wide * (teamSize.Width + 10), 150 + (teamSize.Height + 10) * (int)height);
            outputCaptureInformation();
            cycleMainBG();

            if (meta.features.enableRemoteSplitting)
            {
                updater = new SSMUpdater();
                reader = new SQSReader();
                sqsTimer = new Timer();
                sqsTimer.Enabled = true;
                sqsTimer.Interval = 10 * 1000; //10 second poll
                sqsTimer.Tick += new EventHandler((o, e) => { handleOutboundMessages(reader.consume()); });
                sqsTimer.Start();
                //broadcastState();
                FormClosing += new FormClosingEventHandler((o, e) => { teardownState(); });
            }
        }

        public void handleOutboundMessages(List<OutboundMessage> messages)
        {
            if(messages.Count == 0)
            {
                return;
            }
            foreach(OutboundMessage m in messages)
            {
                TeamControl team = teams.ToList().Find(t => t.teamInfo.teamName == m.team);
                if (!team.teamInfo.teamFinished)
                {
                    TimeSpan lastSplit;
                    if (team.teamInfo.teamSplitNum > 0)
                    {
                        lastSplit = Util.parseTimeSpan(team.teamInfo.teamSplits[team.teamInfo.teamSplitNum - 1]);
                    } else
                    {
                        lastSplit = new TimeSpan(0);
                    }
                    double diff = (m.time - lastSplit.Ticks)/10_000;
                    if (diff > 5 * 60 * 1000)
                    {
                        //Only split if it's 5 minutes after the most recent one (to prevent duplication)
                        team.splitClick();
                        TimeSpan d = new TimeSpan(m.time);
                        team.setSplit(string.Format("{0:D2}:{1:mm}:{1:ss}", (int)d.TotalHours, d), team.teamInfo.teamSplitNum - 1);
                    }
                }
            }
            broadcastState();
        }

        public void broadcastState()
        {
            if (!meta.features.enableRemoteSplitting)
            {
                return;
            }
            SSMStructure structure = new SSMStructure();
            if (MainTimerRunning)
            {
                structure.timestamp = TimerStart.Ticks;
            }
            structure.teams = teams.ToList().Select(t =>
            {
                SSMStructure.SSMTeam team = new SSMStructure.SSMTeam();
                team.name = t.teamInfo.teamName;
                team.color = ColorTranslator.ToHtml(t.teamInfo.color);
                team.activeSplit = meta.splits[t.teamInfo.teamSplitNum];
                return team;
            }).ToArray();
            updater.updateValue(structure);
        }

        public void teardownState()
        {
            if (!meta.features.enableRemoteSplitting)
            {
                return;
            }
            SSMStructure structure = new SSMStructure();
            updater.updateValue(structure);
        }

        private void outputCaptureInformation()
        {
            List<String> captureLines = new List<string>();
            captureLines.Add("Program Size: ");
            captureLines.Add(Util.outputCaptureInfo(this, this));
            captureLines.Add("Main timer:");
            captureLines.Add(Util.outputCaptureInfo(MainTimer, this));
            foreach (TeamControl team in teams)
            {
                captureLines.AddRange(team.outputCaptureInfo(this));
            }
            File.WriteAllLines("capture-info.txt", captureLines);
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e) =>
            teams.ToList().FindAll(t=>t.teamInfo.teamSplitKey == (int)e.Key)
                .ForEach(t => t.TeamSplitButton_Click(this, EventArgs.Empty));

        private void button1_Click(object sender, EventArgs e)
        {
            TimerStart = DateTime.Now.ToUniversalTime();
            StartTimer();
        }

        private void StartTimer()
        {
            if (!MainTimerRunning)
            {
                //Start the timer, do some stuff
                MainTimerRunning = true;
                tick.Enabled = true;
                tick.Interval = 1;
                tick.Start();
                tick.Tick += new EventHandler(TimerEventProcessor);
                tick.Interval = timerTickInterval;
                ChangedThisMin = true;
                broadcastState();
            }
            else
            {
                //Not sure if we want to stop the timer, give a dialog box?
            }
            ResumeButton.Enabled = false;
            ResumeButton.Visible = false;
        }

        void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            TimeSpan d = DateTime.Now.ToUniversalTime() - TimerStart;
            //TimeSpan d = DateTime.Now - TimerStart + TimeSpan.FromHours(60);
            //TimeSpan f = d.Add(TimeSpan.FromSeconds(1));
            string current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)d.TotalHours, d);
            //string current = d.ToString(@"hh\:mm\:ss");
            MainTimer.Text = current;
            bool toCycle = !ChangedThisMin;

            if (!meta.features.syncInfoCycling)
            {
                foreach (TeamControl team in teams)
                {
                    team.updateTimerEvent(current, toCycle);
                }
            }
            else
            {
                int targetTab = teams[0].updateTimerEvent(current, toCycle);
                if (!toCycle) { targetTab = -1; }
                teams.Except(new TeamControl[] { teams[0] }).ToList()
                    .ForEach(t => t.updateTimerEvent(current, toCycle, targetTab));
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
                    teardownState();
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
            foreach (TeamControl team in teams)
            {
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
            string[] lines = new string[Splits.Length + 2];
            //line[0] = "Time   | Mog   | Choco | Tonb  ";
            //Parameterised
            lines[0] = TimerStart.ToUniversalTime().ToString("u");
            int timePad = Splits.Select(str => str.Length).Max();
            lines[1] = "Time".PadRight(timePad, ' ');
            for (int i = 0; i < Splits.Length; i++)
            {
                lines[i + 2] = Splits[i].PadRight(timePad, ' ');
            }
            for (int i = 0; i < teams.Length; i++)
            {
                TeamControl team = teams[i];
                lines[1] += sep + team.teamInfo.teamName.PadRight(8, ' ');
                for (int j = 0; j < Splits.Length; j++)
                {
                    lines[j + 2] += sep + team.getSplit(j);
                }
            }
            File.WriteAllLines("splits_output.txt", lines);
            if (meta.features.enableRemoteSplitting)
            {
                broadcastState();
            }
        }

        public void cycleMainBG()
        {
            if (meta.features.mainLayoutBackground) { 
                int bgnum = teams.Max(t => t.teamInfo.teamIcon);
                if (File.Exists("background_" + bgnum + ".png"))
                {
                    File.Copy("background_" + bgnum + ".png", "background.png", true);
                }
            }
        }

        private void ReadSplitFiles()
        {
            if (File.Exists("splits_output.txt"))
            {
                string[] lines = File.ReadAllLines("splits_output.txt");
                if(TimerStart == null || TimerStart.Ticks == 0)
                {
                    TimerStart = DateTime.Parse(lines[0]).ToUniversalTime();
                }
                for (int line = 2; line < lines.Length; line++)
                {
                    string[] split = lines[line].Split('|');
                    var i = line - 1;
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
                        teams[j - 1].setSplit(split[j], i - 1, true);
                    }
                }
            }
        }

        public VersusWrapper[] fetchOtherTeamInfo(TeamControl self)
        {
            //return teams.Except(new TeamControl[] { self }).Select(tc => new VersusWrapper(tc.teamInfo.teamSplitNum, tc.teamInfo.teamSplits, tc.teamInfo.teamFinished));
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

        private void button1_Click_2(object sender, EventArgs e)
        {
            //need to post process timer indicies for teams here to do it properly?
            ReadSplitFiles();
            Array.ForEach(teams,team => team.reconstructSplitMetadata());
            StartTimer();
        }
    }
}
