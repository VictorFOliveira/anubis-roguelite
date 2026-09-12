using System;
using System.Collections.Generic;

namespace Anubis.Save
{
    [Serializable]
    public class SaveData
    {
        public int Version = 1;
        public MetaProgressionData Meta = new();
    }

    [Serializable]
    public class MetaProgressionData
    {
        public int RunsStarted;
        public int ArenasCleared;
        public int Deaths;
        public int PermanentHealthBonus;
        public List<string> BlessingsSeen = new();
        public List<string> BlessingsChosen = new();
    }
}
