﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace sourceSearch
{
    public static class ResultFunc
    {
        public static Dictionary<string, string> books;
        static ResultFunc()
        {
            books = new(){
                {"Genesis", "בראשית"},
                {"Exodus", "שמות"},
                {"Leviticus", "ויקרא"},
                {"Numbers","במדבר" },
                {"Deuteronomy","דברים" },
                { "Joshua", "יהושע" },
                { "Judges", "שופטים" },
                {"Ruth","רות"},
                { "I Samuel", "שמואל א" },
                {"II Samuel","שמואל ב" },
                { "I Kings", "מלכים א" },
                { "II Kings", "מלכים ב" },
                {"I Chronicles","דברי הימים א"},
                {"II Chronicles","דברי הימים ב" },
                { "Ezra", "עזרא" },
                { "Nehemiah", "נחמיה" },
                { "Esther","אסתר"},
                {"Psalms","תהילים"},
                {"Job","איוב" },
                { "Proverbs", "משלי" },
                { "Ecclesiastes", "קהלת" },
                { "Song of Songs", "שיר השירים" },
                {"Hosea","הושע"},
                {"Amos","עמוס" },
                { "Micah", "מיכה" },
                { "Joel", "יואל" },
                { "Obadiah","עובדיה"},
                {"Jonah","יונה"},
                {"Nahum","נחום" },
                { "Habakkuk", "חבקוק" },
                { "Zephaniah", "צפניה" },
                {"Haggai","חגי"},
                { "Zachariah","זכריה"},
                {"Malachi","מלאכי"},
                {"Isaiah","ישעיהו" },
                { "Jeremiah", "ירמיהו" },
                { "Lamentations", "איכה" },
                {"Ezekiel","יחזקאל"}
            };

        }
        public static string HebrowSource(string postResult)
        {
            int index, numVal;
            string result, chapter, verse;

            foreach (var book in books)
            {
                if (postResult.Contains(book.Key))
                {
                    result = book.Value + " ";
                    index = postResult.IndexOf(book.Key) + book.Key.Length + 1;
                    if (postResult[index + 1].Equals(':'))
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        index = index + 2;//number :

                        chapter = FormatHebrew(numVal);
                        result = result + chapter + " ";
                    }
                    else if (postResult[index + 2].Equals(':'))
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        numVal = numVal * 10 + Int32.Parse(new string(postResult[index + 1], 1));
                        chapter = FormatHebrew(numVal);
                        index = index + 3;//number number :
                        result = result + chapter + " ";
                    }
                    else
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        numVal = numVal * 10 + Int32.Parse(new string(postResult[index + 1], 1));
                        numVal = numVal * 10 + Int32.Parse(new string(postResult[index + 2], 1));

                        chapter = FormatHebrew(numVal);
                        index = index + 4;//number number :
                        result = result + chapter + " ";
                    }
                    if (postResult[index + 1].Equals(' '))
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        verse = FormatHebrew(numVal);

                        result = result + verse;
                    }
                    else if (postResult[index + 2].Equals(' '))
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        numVal = numVal * 10 + Int32.Parse(new string(postResult[index + 1], 1));
                        verse = FormatHebrew(numVal);
                        result = result + verse;
                    }
                    else
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        numVal = numVal * 10 + Int32.Parse(new string(postResult[index + 1], 1));
                        numVal = numVal * 10 + Int32.Parse(new string(postResult[index + 2], 1));

                        verse = FormatHebrew(numVal);
                        index = index + 4;//number number :
                        result = result + chapter + " ";
                    }
                    return result;
                }
            }
            return "0";
        }




        static String[] let1000 = { " א'", " ב'", " ג'", " ד'", " ה'" };
        static String[] let100 = { "ק", "ר", "ש", "ת" };
        static String[] let10 = { "י", "כ", "ל", "מ", "נ", "ס", "ע", "פ", "צ" };
        static String[] let1 = { "א", "ב", "ג", "ד", "ה", "ו", "ז", "ח", "ט" };
        static String FormatHebrew(int num)
        {
            if (num <= 0 || num >= 6000)
                throw new Exception();
            StringBuilder ret = new StringBuilder();

            if (num >= 100)
            {
                if (num >= 1000 & num < 6000)
                {

                    ret.Append(let1000[num / 1000 - 1]);
                    num %= 1000;
                }

                if (num < 500)
                {

                    ret.Append(let100[(num / 100) - 1]);

                }
                else if (num >= 500 && num < 900)
                {
                    ret.Append("ת");
                    ret.Append(let100[((num - 400) / 100) - 1]);
                }
                else if (num >= 900 && num < 1000)
                {
                    ret.Append("תת");
                    ret.Append(let100[((num - 800) / 100) - 1]);

                }

                num %= 100;
            }
            switch (num)
            {
                // Avoid letter combinations from the Tetragrammaton
                case 16:
                    ret.Append("טז");
                    break;
                case 15:
                    ret.Append("טו");
                    break;
                default:
                    if (num >= 10)
                    {

                        ret.Append(let10[(num / 10) - 1]);
                        num %= 10;
                    }
                    if (num > 0)
                    {

                        ret.Append(let1[num - 1]);
                    }
                    break;
            }
            return ret.ToString();
        }

        public static string SameVerse(string res, string oldRes)
        {
            var subsRes = res.Split(' ');
            var subsOld = oldRes.Split(' ');
            string concat = string.Empty;

            if (subsRes.Length == subsOld.Length)
            {
                if (subsRes.Length == 4 && subsRes[0] == subsOld[0] && subsRes[1] == subsOld[1])
                {
                    if (subsRes[2] == subsOld[2])
                        concat = $"שם שם {subsRes[3]}";
                    else
                        concat = $"שם {subsRes[3]} {subsRes[2]}";
                }
                else if (subsRes.Length == 3 && subsRes[0] == subsOld[0])
                {
                    if (subsRes[1] == subsOld[1])
                        concat = $"שם שם {subsRes[2]}";
                    else
                        concat = $"שם {subsRes[1]} {subsRes[1]}";
                }

                return concat;
            }

            return res;


        }
        public static string PartVerse(string verse)
        {
            string postResult, hebrowResult;
            var newVerse = verse;
            var subs = verse.Split(' ');

            for (int i = subs.Length - 1; i >= 3; i--)
            {
                newVerse = newVerse.Remove(newVerse.Length - subs[i].Length - 1, subs[i].Length + 1);
                postResult = Post.VerseSource(newVerse).GetAwaiter().GetResult();
                hebrowResult = HebrowSource(postResult);
                if (hebrowResult != "0")
                {
                    return hebrowResult;
                }

            }
            return "0";

        }

    }
    public static class StringExtensions//no refernce
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }


}

