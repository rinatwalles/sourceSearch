using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SourceSearch
{
    
    static class Program
    {
        public class CompDate { }
        public class Order { }

        public static Dictionary<string, string> books;
        
        //public Dictionary<string, string> letters;


            static Program()
        {
            books = new Dictionary<string, string>(){
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

        static async void Main()
        {
            int index;
            string newStr;
            string postResult, hebrowResult="", oldRes;
            string shomRes;
            
            var path = @"C:\Users\eliwa\OneDrive\רינת\אבוש\sourceSearch\ConsoleApp1\try.txt";
            

                var file = File.ReadAllText(@path);
                var results = FindVerse(file);
                foreach (string result in results)
                {
                    oldRes = hebrowResult;
                    Console.WriteLine(result);
                    postResult = await VerseSource(result);
                    hebrowResult = HebrowSource(postResult);
                    if (hebrowResult != "0")
                    {
                        shomRes = SameVerse(hebrowResult,oldRes);
                        index = file.LastIndexOf(result);
                        newStr = file.Insert(index, " (" + shomRes + ")");
                        Console.WriteLine("(" + shomRes + ")");
                        file = newStr;
                    }
                    else 
                    {
                        hebrowResult = PartVerse(result);
                        if (hebrowResult != "0")
                        {
                            shomRes = SameVerse(hebrowResult, oldRes);
                            index = file.LastIndexOf(result);
                            newStr = file.Insert(index, " (" + shomRes + ")");
                            Console.WriteLine("(" + shomRes + ")");
                            file = newStr;
                        }


                    }

                
                }
                var pathRes = @"C:\Users\eliwa\OneDrive\רינת\אבוש\sourceSearch\ConsoleApp1\res.txt";


                File.WriteAllText(pathRes, file);
            

        }




        
        static List<string> FindVerse(string all)
        {
            var results = new List<string>();
            var pettren = "(שנאמר|וכתיב|שנא'|אמר הכתוב|וכאן הוא אומר|על פסוק|על מה שנאמר|מלשון)(?<x>.+?):";
            //string all = File.ReadAllText(@path);
            MatchCollection mateches = Regex.Matches(all, pettren);
            foreach (Match match in mateches)
            {
                Console.WriteLine(match.Groups[2].Value);
                results.Add(match.Groups[2].Value);
            }
            return results;
        }
        static string PartVerse(string verse)
        {
            string postResult,hebrowResult;
            var newVerse = verse;
            var subs = verse.Split(' ');

            for (int i = subs.Length - 1; i >= 3; i--)
            {
                newVerse = newVerse.Remove(newVerse.Length - subs[i].Length-1, subs[i].Length+1);
                postResult = VerseSource(newVerse).GetAwaiter().GetResult();
                hebrowResult = HebrowSource(postResult);
                if (hebrowResult != "0")
                {
                    return hebrowResult;
                }

            }
            return "0";

        }
        static string SameVerse(string res, string oldRes)
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

        static async Task<string> VerseSource(string str)
        {
            // using var client = new HttpClient();
            var serviceCollection = new ServiceCollection();
            ConfigurationService(serviceCollection);
            var services = serviceCollection.BuildServiceProvider();
            var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient();
            
            var endpoint = "https://www.sefaria.org/api/search-wrapper";

            var values = new
            {
                query = str,

                type = "text",

                field = "naive_lemmatizer",

                filters = new[] { "Tanakh" }

            };
            var newPostJson = JsonConvert.SerializeObject(values);

            var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");

            var postResult = await client.PostAsync(endpoint, payload).Result.Content.ReadAsStringAsync();

            return postResult;
            
        }
        private static void ConfigurationService(ServiceCollection services)
        {
            services.AddHttpClient();
        }
      
        static string HebrowSource(string postResult)
        {
            int index, numVal;
            string result,chapter,verse;
          
            foreach (var book in books)
            {
                if (postResult.Contains(book.Key))
                {
                    result = book.Value + " ";
                    index=postResult.IndexOf(book.Key)+book.Key.Length+1;
                    if (postResult[index + 1].Equals(':'))
                    {
                        numVal = Int32.Parse(new string(postResult[index], 1));
                        index= index + 2;//number :
                        
                        chapter = FormatHebrew(numVal);
                        result = result + chapter + " ";
                    }
                    else if (postResult[index + 2].Equals(':'))
                    {
                        numVal = Int32.Parse(new string(postResult[index],1));
                        numVal = numVal*10+Int32.Parse(new string(postResult[index + 1], 1));
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
                        numVal = numVal*10+Int32.Parse(new string(postResult[index + 1], 1));
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
        public static String FormatHebrew(int num) 
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

    }
    public static class StringExtensions//no refernce
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}




