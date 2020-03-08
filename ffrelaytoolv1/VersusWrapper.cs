using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    class VersusWrapper
    {
        public int splitNum;

        public string[] splits;

        public Label vsLabel;

        public VersusWrapper(int num, string[] splits, Label label)
        {
            this.splitNum = num;
            this.splits = splits;
            this.vsLabel = label;
        }
    }
}
