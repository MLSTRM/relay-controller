using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRelayTool_RemoteClient
{
    public partial class TeamControl : UserControl
    {
        public TeamControl()
        {
            InitializeComponent();
            cooldown = new Timer();
        }

        private Timer cooldown;

        private Form1 parent;

        public void setup(Form1 parent, string teamName, Color buttonColour, string activeSplit)
        {
            this.parent = parent;
            this.teamName = teamName;
            this.buttonColour = buttonColour;
            this.activeSplit = activeSplit;
            update();
        }

        public void update()
        {
            TeamLabel.Text = "Team " + teamName;
            splitLabel.Text = activeSplit;
            button1.BackColor = buttonColour;
            if (buttonColour.GetBrightness() < 0.5f)
            {
                button1.ForeColor = Color.White;
            }
        }

        public void refreshSplit(string activeSplit)
        {
            this.activeSplit = activeSplit;
            update();
        }

        private string teamName;

        private Color buttonColour;

        private string activeSplit;

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text = "Team " + teamName + " split - DISABLED";
            button1.Enabled = false;
            button1.BackColor = Color.Gray;
            parent.publishMessage(this.teamName);
            cooldown.Start();
            cooldown.Enabled = true;
            cooldown.Interval = 10 * 1000; //10 seconds
            cooldown.Tick += new EventHandler((o, ev) => { 
                cooldown.Stop(); 
                button1.Text = "Team "+teamName+" Split"; 
                button1.Enabled = true;
                button1.BackColor = buttonColour;
            });
        }
    }
}
