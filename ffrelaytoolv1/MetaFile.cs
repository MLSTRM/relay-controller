using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffrelaytoolv1
{
    public class MetaFile
    {
        public string[] games;
        public int splitsToShow;
        public int splitFocusOffset;
        public int teamsPerRow;
        public Team[] teams;

        public class Team
        {
            public string name;
            public string color;
            public string image;
        }
    }
}
