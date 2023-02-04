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
        bool EnableAutoCycle = true;
        private int infoCyclingIndex = 0;

        string[] Splits;

        MetaContext meta;

        TeamControl[] teams;
        MetaControl metaControl;

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

            var enableString = EnableAutoCycle ? "enabled" : "disabled";
            autoCycleToggle.Text = $"Auto cycle ({enableString})";

            //Programmatic stuff
            meta = new MetaContext(metaFile.splitsToShow, metaFile.splitFocusOffset, Splits,
                teamNames, metaFile.games, metaFile.layout, metaFile.features);
            teams = new TeamControl[metaFile.teams.Length];
            //Create team controls based on the meta file.
            int wide = Math.Min(metaFile.teamsPerRow, metaFile.teams.Length);
            double height = Math.Ceiling((double)metaFile.teams.Length / (double)metaFile.teamsPerRow);
            var teamWidth = Math.Max((meta.layout.boxWidth + 30), (meta.layout.timerWidth + 80));
            var teamHeight = (meta.layout.boxWidth + 30) > (2 * meta.layout.timerWidth + 80) ? meta.layout.boxHeight + meta.layout.timerHeight + 56 : meta.layout.boxHeight + meta.layout.timerHeight * 2 + 60;
            Size teamSize = new Size(teamWidth, teamHeight);
            for (int i = 0; i < metaFile.teams.Length; i++)
            {
                teams[i] = new TeamControl();
                int row = i / metaFile.teamsPerRow;
                teams[i].Location = new Point(15 + (i % metaFile.teamsPerRow) * (teamSize.Width + 10), 120 + (teamSize.Height + 10) * row);
                MetaFile.Team team = metaFile.teams[i];
                var backImage = team.image != null ? Image.FromFile(team.image) : null;
                teams[i].setupTeamControl(this, new TeamInfo(metaFile.games.Length, Splits.Length, team.name, team.name.ToLower() + "-runners.txt",
                    ColorTranslator.FromHtml(team.color), backImage, team.splitKey, metaFile.features.teamGameIcons, team.leftAlign), meta, teamSize);
                this.Controls.Add(teams[i]);
            }
            if (metaFile.features.showMetaControl)
            {
                metaControl = new MetaControl();
                int row = metaFile.teams.Length / metaFile.teamsPerRow;
                metaControl.Location = new Point(15 + (metaFile.teams.Length % metaFile.teamsPerRow) * (teamSize.Width + 10), 120 + (teamSize.Height + 10) * row);
                metaControl.setupMetaControl(this, meta);
                Controls.Add(metaControl);
            }
            loadCommentators();
            var windowWidth = 30 + wide * (teamSize.Width + 10);
            var windowHeight = 150 + (teamSize.Height + 10 + 26) * (int)height;
            if (meta.features.showMetaControl)
            {
                var offset = 0;
                if (metaFile.teams.Length < metaFile.teamsPerRow)
                {
                    windowWidth += meta.features.metaControl.width + meta.features.metaControl.margin + 10;
                    if(meta.features.metaControl.height > teamSize.Height)
                    {
                        offset = meta.features.metaControl.height - teamSize.Height;
                        windowHeight += offset;
                    }
                }
                else
                {
                    if (metaFile.teams.Length > metaFile.teamsPerRow && metaFile.teams.Length % metaFile.teamsPerRow != 0)
                    {
                        offset = -teamSize.Height - 10;
                    }
                    windowHeight += offset + meta.features.metaControl.height + meta.features.metaControl.margin;
                }
            }
            this.Size = new Size(windowWidth, windowHeight);
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
            //TODO add some kind of debug pane to the main form to see this in action
            if(messages.Count == 0)
            {
                return;
            }
            string consumptionLog = $"{getCurrentTimeString()}: Consuming {messages.Count} messages";
            foreach(OutboundMessage m in messages)
            {
                TimeSpan d = new TimeSpan(m.time);
                string messageTimeString = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)d.TotalHours, d);
                string messageInfo = $"message for {m.team} with time {messageTimeString} - ";
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
                    double diffMillis = (m.time - lastSplit.Ticks)/10_000;
                    if (diffMillis > meta.layout.remoteSplitCooldown)
                    {
                        //Only split if it's 5 minutes after the most recent one (to prevent duplication)
                        team.splitClick();
                        
                        team.setSplit(messageTimeString, team.teamInfo.teamSplitNum - 1);
                        messageInfo += $"Split Accepted";
                    } else
                    {
                        messageInfo += $"Ignored due to cooldown {diffMillis} / {meta.layout.remoteSplitCooldown}";
                    }
                }
                consumptionLog += "\r\n" + messageInfo;
            }
            LastPollLabel.Text = consumptionLog;
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

        int getTeamSplitOffset()
        {
            var teams = fetchOtherTeamInfo(null);
            int i = 0;
            var maxOffset = meta.splits.Length - (meta.splitsToShow - meta.splitFocusOffset);
            for (int team = 0; team < teams.Length; team++)
            { i = Math.Max(i, Util.clamp(teams[team].splitNum, maxOffset, meta.splitFocusOffset)); }
            return i;
        }

        int getTeamSplitMinOffset()
        {
            var teams = fetchOtherTeamInfo(null);
            return teams.Min(t => t.splitNum);
        }

        string getCurrentTimeString()
        {
            TimeSpan d = DateTime.Now.ToUniversalTime() - TimerStart;
            string current = string.Format("{0:D2}:{1:mm}:{1:ss}", (int)d.TotalHours, d);
            return current;
        }

        void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            string current = getCurrentTimeString();
            //string current = d.ToString(@"hh\:mm\:ss");
            MainTimer.Text = current;
            bool toCycle = !ChangedThisMin && EnableAutoCycle;

            if (!meta.features.syncInfoCycling)
            {
                foreach (TeamControl team in teams)
                {
                    team.updateTimerEvent(current, toCycle);
                }
                if (meta.features.showMetaControl)
                {
                    metaControl.updateTimerEvent(current, toCycle);
                }
            }
            else
            {
                if (meta.features.infoCyclingPattern.Length > 0)
                {
                    int targetTab = -1;
                    if (toCycle)
                    {
                        infoCyclingIndex++;
                        if (infoCyclingIndex == meta.features.infoCyclingPattern.Length)
                        {
                            infoCyclingIndex = 0;
                        }
                        targetTab = meta.features.infoCyclingPattern[infoCyclingIndex];
                        if(targetTab == 2 && getTeamSplitMinOffset() < meta.features.graphThreshold)
                        {
                            targetTab = 0; // Show splits if the graph is not yet full of enough data to show meaningful info
                        }
                    }
                    teams.ToList().ForEach(t => t.updateTimerEvent(current, toCycle, targetTab));
                    if (meta.features.showMetaControl)
                    {
                        metaControl.updateTimerEvent(current, toCycle, targetTab);
                    }
                }
                else
                {
                    int targetTab = teams[0].updateTimerEvent(current, toCycle);
                    if (!toCycle) { targetTab = -1; }
                    teams.Except(new TeamControl[] { teams[0] }).ToList()
                        .ForEach(t => t.updateTimerEvent(current, toCycle, targetTab));
                    if (meta.features.showMetaControl)
                    {
                        metaControl.updateTimerEvent(current, toCycle, targetTab);
                    }
                }
            }

            //This section auto cycles
            MinuteCount++;
            if (MinuteCount >= infoCycleTicks) //1 Minute = 60 seconds = 240 timer ticks
            {
                ChangedThisMin = false;
                MinuteCount = 0;
            }
        }

        public void checkAllTeamsFinished()
        {
            var allFinished = teams.Select(team => team.teamInfo.teamFinished).All(b => b);
            if (allFinished)
            {
                MainTimerRunning = false;
                tick.Stop();
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
            if (meta.features.showMetaControl)
            {
                metaControl.RefreshGame();
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
                int bgnum = getMaxIcon();
                if (File.Exists("background_" + bgnum + ".png"))
                {
                    File.Copy("background_" + bgnum + ".png", "background.png", true);
                }
            }
            if (meta.features.showMetaControl)
            {
                metaControl.RefreshGame();
            }
        }

        public int getMaxIcon()
        {
            return teams.Max(t => t.teamInfo.teamIcon);
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
            var len = teams.Length;
            if(self != null)
            {
                len--;
            }
            VersusWrapper[] wrapperArray = new VersusWrapper[len];
            int adjustedIndex = 0;
            for (int i = 0; i < teams.Length; i++)
            {
                if (self == teams[i]) { continue; }
                wrapperArray[adjustedIndex] = new VersusWrapper(teams[i].teamInfo.teamSplitNum, teams[i].teamInfo.teamSplits, teams[i].teamInfo.teamFinished);
                adjustedIndex++;
            }
            return wrapperArray;
        }

        public TeamInfo[] fetchTeamInfo()
        {
            return teams.Select(t => t.teamInfo).ToArray();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            //need to post process timer indicies for teams here to do it properly?
            ReadSplitFiles();
            Array.ForEach(teams,team => team.reconstructSplitMetadata());
            StartTimer();
        }

        private void autoCycleToggle_Click(object sender, EventArgs e)
        {
            EnableAutoCycle = !EnableAutoCycle;
            var enableString = EnableAutoCycle ? "enabled" : "disabled";
            autoCycleToggle.Text = $"Auto cycle ({enableString})";
        }
    }
}
