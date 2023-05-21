using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenWorldRedux
{
    [Serializable]
    public class FactionFile
    {
        public string factionName;

        public enum MemberRank { Member, Moderator, Admin }

        public List<string> memberString;

        public List<Tuple<string, MemberRank>> factionMembers;
    }
}
