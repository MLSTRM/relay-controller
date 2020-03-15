using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public partial class warning : Form
    {
        public warning()
        {
            InitializeComponent();
        }

        public void setWarning(String warn){
            warnLabel.Text = warn;
        }
    }
}
