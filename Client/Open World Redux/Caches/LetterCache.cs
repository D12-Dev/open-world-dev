using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class LetterCache
    {
        private static string letterTitle = "";
        private static string letterDescription = "";
        private static LetterDef letterType = null;

        public static void GetLetterDetails(string title, string description, LetterDef type)
        {
            letterTitle = title;
            letterDescription = description;
            letterType = type;
        }

        public static void GenerateLetter()
        {
            Find.LetterStack.ReceiveLetter(letterTitle,
                letterDescription,
                letterType);
        }
    }
}
