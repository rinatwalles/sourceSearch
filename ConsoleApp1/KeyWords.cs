using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceSearch
{
    public static class KeyWords
    {
        public static HashSet<string> words;
        static KeyWords()
        {
            words.Add("שנאמר");
            words.Add("שנא'");
            words.Add("על דרך");
            words.Add("וכתיב");
            words.Add("מלשון");
            words.Add("וכן הוא אומר");
            words.Add("אמר הכתוב");
            words.Add("וכאן הוא אומר");
            words.Add("על פסוק");
            words.Add("על מה שנאמר");
            words.Add("על פסוק");
        }
    }
}
