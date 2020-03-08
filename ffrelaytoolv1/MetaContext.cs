﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffrelaytoolv1
{
    public class MetaContext
    {
        public readonly int splitsToShow;

        public readonly int splitFocusOffset;

        public readonly int numberOfGames;

        public readonly int numberOfTeams;

        public readonly string[] splits;

        public readonly string[] teamNames;

        //Abbreviations for time display
        public readonly string[] games;

        public MetaContext(int splitsToShow, int splitFocusOffset, string[] splits, string[] teamNames, string[] games)
        {
            this.splitsToShow = splitsToShow;
            this.splitFocusOffset = splitFocusOffset;
            this.splits = splits;
            this.teamNames = teamNames;
            this.numberOfTeams = teamNames.Length;
            this.games = games;
            this.numberOfGames = games.Length;
        }
    }
}
