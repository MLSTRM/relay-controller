namespace FFRelayTool_Model
{
    public class SSMStructure
    {
        public SSMTeam[] teams;
        public long timestamp;

        public class SSMTeam
        {
            public string name;
            public string activeSplit;
            public string color;
        }
    }
}
