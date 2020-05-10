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
            statusHeader.Text = "Status: Fetching information";
            state = reader.readParameter();
            refreshComponents();
            refreshTimer = new Timer();
            refreshTimer.Start();
            refreshTimer.Interval = 60 * 1000; //1 minute
            refreshTimer.Tick += new EventHandler((o, e) => {
                state = reader.readParameter();
                refreshComponents();
            });
            statusHeader.Text = "Status: Ready";
        }

        public void publishMessage(string teamName)
        {
            publisher.publish(teamName, new DateTime(state.timestamp));
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
                    control.Location = new Point(5 + 160 * i, 100);
                    teams[i] = control;
                    this.Controls.Add(control);
                }
            }
        }

        private SSMStructure state;
    }
}
