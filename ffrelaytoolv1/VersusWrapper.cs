using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public class VersusWrapper
    {
        public int splitNum;

        public bool finished;

        public string[] splits;

        public VersusWrapper(int num, string[] splits, bool finished)
        {
            this.splitNum = num;
            this.splits = splits;
            this.finished = finished;
        }
    }
}
