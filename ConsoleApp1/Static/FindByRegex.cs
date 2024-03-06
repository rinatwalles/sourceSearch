using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace sourceSearch
{
    public static class FindByRegex
    {
        public static List<string> FindVerse(string all)
        {
            var results = new List<string>();
            var pettren = "(שנאמר|וכתיב|שנא'|אמר הכתוב|וכאן הוא אומר|על פסוק|על מה שנאמר|מלשון)(?<x>.+?):";
            MatchCollection mateches = Regex.Matches(all, pettren);
            foreach (Match match in mateches)
            {
                Console.WriteLine(match.Groups[2].Value);
                results.Add(match.Groups[2].Value);
            }
            return results;
        }
    }
}
