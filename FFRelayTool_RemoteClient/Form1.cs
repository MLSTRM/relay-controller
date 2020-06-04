using FFRelayTool_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRelayTool_RemoteClient
{
    public partial class Form1 : Form
    {
        private SSMReader reader;
        private SNSPublisher publisher;
        private TeamControl[] teams;

        private Timer refreshTimer;

        public Form1()
        {
            reader = new SSMReader();
            publisher = new SNSPublisher();
            InitializeComponent();
            statusHeader.Text = "Status: Fetching information - please wait";
            refreshTimer = new Timer();
            refreshTimer.Start();
            refreshTimer.Interval = 10 * 1000; //10 seconds
            refreshTimer.Tick += new EventHandler((o, e) => {
                state = reader.readParameter();
                if (state != null && state.teams!=null && state.timestamp != 0)
                {
                    statusHeader.Text = "Status: next refresh: " + (DateTime.Now + new TimeSpan(0, 0, 10)).ToString();
                    refreshComponents();
                }
            });
        }

        public void publishMessage(string teamName)
        {
            publisher.publish(teamName, new DateTime(state.timestamp,DateTimeKind.Utc));
        }

        private void refreshComponents()
        {
            if (teams!=null && teams.Length != 0)
            {
                for (int i = 0; i < state.teams.Length; i++)
                {
                    teams[i].refreshSplit(state.teams[i].activeSplit);
                }
            }
            else
            {
                teams = new TeamControl[state.teams.Length];
                for (int i = 0; i < state.teams.Length; i++)
                {
                    TeamControl control = new TeamControl();
                    control.setup(this, state.teams[i].name, ColorTranslator.FromHtml(state.teams[i].color), state.teams[i].activeSplit);
                    control.Location = new Point(5 + 210 * i, 100);
                    teams[i] = control;
                    this.Controls.Add(control);
                }
                eventStart.Text = "Event started: " + new DateTime(state.timestamp).ToString();
            }
        }

        private SSMStructure state;
    }
}
