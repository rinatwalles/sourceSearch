using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;


//להוסיף שם,שם
namespace ConsoleApp1
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

        static void Main(string[] args)
        {
            int index;
            string newStr;
            //Console.WriteLine("enter file's path");
            //string path = System.Console.ReadLine();
            //Program p= new Program();//singelton
            //string path = @"C:\Users\eliwa\OneDrive\רינת\אבוש\sourceSearch\ConsoleApp1\try.txt";
            using (WordDocument document = new WordDocument(@"C:\Users\eliwa\Downloads\english.docx", FormatType.Docx))
            {
                //Gets the Word document text
                string file = document.GetText();
                //Display Word document's text content.
                Console.WriteLine(file);
                Console.ReadLine();

                //string file = File.ReadAllText(@path);
                List<string> results = find(file);
                foreach (string result in results)
                {
                    Console.WriteLine(result);
                    string postResult = source(result).GetAwaiter().GetResult();
                    string hebrowResult = hebrowSource(postResult);
                    if (hebrowResult != "0")
                    {
                        index = file.LastIndexOf(result);
                        newStr = file.Insert(index, " (" + hebrowResult + ")");
                        Console.WriteLine("(" + hebrowResult + ")");
                        file = newStr;
                    }


                }
                string pathRes = @"C:\Users\eliwa\OneDrive\רינת\אבוש\sourceSearch\ConsoleApp1\res.txt";

                //var pathRes = "data.txt";

                File.WriteAllText(pathRes, file);
            }

        }




        
        static List<string> find(string all)
        {
            List<string> results = new List<string>();
            string pettren = "(שנאמר|וכתיב|שנא'|אמר הכתוב|וכאן הוא אומר|על פסוק|על מה שנאמר|מלשון)(?<x>.+?):";
            //string all = File.ReadAllText(@path);
            MatchCollection mateches = Regex.Matches(all, pettren);
            foreach (Match match in mateches)
            {
                Console.WriteLine(match.Groups[2].Value);
                results.Add(match.Groups[2].Value);
            }
            return results;
        }

        static async Task<string> source(string str)
        {
            using (var client = new HttpClient())
            {
                string endpoint = "https://www.sefaria.org/api/search-wrapper";



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
        }
      
        static string hebrowSource(string postResult)
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
                        //chapter = Convert.ToString(Convert.ToChar('\u05D0' + Convert.ToChar(numVal - 1)));
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
    }







/*
  var values = new
                {
                    query = str,
                    sort_method = "score",
                    source_proj = "heRef",
                    //type = "text",
                    filters = new[] { "Tanach with Nikkud" },
                    //aggs = "heRef",
                    field = "naive_lemmatizer",
                    //filters = new[] { "Tanakh" },
                    filter_fields = new[] { "version" }
                };
*/



/*
static async Task Main(string[] args)
{
    string quote = "כָּל-הָעָם רָאֲוּ אֶת-הַקּוֹלֹת";
    string url = $"https://www.sefaria.org/api/search?q={quote}&category=Text";

    HttpClient client = new HttpClient();
    HttpResponseMessage response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        dynamic data = JObject.Parse(responseBody);

        if (data?.error == null && data?.he == quote)
        {
            string book = data?.section_ref?.book;
            int chapter = data?.section_ref?.chapter ?? 0;
            int verse = data?.section_ref?.verse ?? 0;

            Console.WriteLine($"{book} {chapter}:{verse}");
        }
        else
        {
            Console.WriteLine("Error: Unable to retrieve Bible reference");
        }
    }
    else
    {
        Console.WriteLine("Error: Unable to retrieve data from API");
    }
}
*/


// static List<string> source(List<string> quotes)
// {

//}


/*
   
*/

//public class NewWeather
//{
//    public string Name { get; set; }
//}





//Console.OutputEncoding = System.Text.Encoding.UTF8;
//string word1 = "\u05E9\u05E0\u05D0\u05DE\u05E8";
//string word2 = "\u05E9\u05E0\u05D0\u0027";
//string word1 = "שנאמר";
////Console.WriteLine(word2);
//string pettren = "(שנאמר|וכתיב|שנא'|אמר הכתוב|וכאן הוא אומר|על פסוק|על מה שנאמר|מלשון)(?<x>.+?):";
//string pettren1 = word1 + @"(?< x >.+?:)";
//// Regex g = new Regex(word1+@"(?< x >.+?:)");

//string path = @"C:\Users\eliwa\OneDrive\רינת\אבוש\sourceSearch\ConsoleApp1\try.txt";
//string all = File.ReadAllText(path);
////using (StreamReader r = File.OpenText(path));
//MatchCollection mateches = Regex.Matches(all, pettren);
//foreach (Match match in mateches)
//{
//    //Console.WriteLine(match.Value);
//    Console.WriteLine(match.Groups[2].Value);
//}
////string line;
////while ((line = r.ReadLine()) != null)
////{
//// X.
//// Try to match each line against the Regex.
////Match m = g.Match(line);
////if (m.Success)
////{
// Y.
// Write original line and the value.
//string v = m.Groups[1].Value;
//Console.WriteLine(line);
//Console.WriteLine("\t" + v);
//Console.WriteLine(m.Value+"\n");
//}
//}




//using (var client = new HttpClient())
//{
//    var endpoint = new Uri("https://localhost:44306/students");

//    var res2Task = client.GetAsync(endpoint).ContinueWith(x => x.Result.Content.ReadAsStringAsync());
//    var values = new NewWeather()
//    {
//        Name = "Rinat3"
//    };
//    var newPostJson = JsonConvert.SerializeObject(values);
//    //var endpoint = new Uri("https://jsonplaceholder.typicode.com/posts");

//    var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");
//    //var x = client.()
//    var postResult = client.PostAsync(endpoint, payload).ContinueWith(x => x.Result.Content.ReadAsStringAsync());

//    ////jvhsfdvksdlvfk
//    ///

//    await Task.WhenAll(new[] { res2Task, postResult });
//}


/*
 using (var client = new HttpClient())
            {
                var endpoint = new Uri("https://www.sefaria.org/api/search-wrapper");
               
            }
//var values = new
//{
//    query = str,
//    //type = "text",
//    //filters= new[] { "heRef" },
//    //aggs= "heRef"
//    //field= "heRef"
//    //filters= new[] { "Tanakh" },
//    //filter_fields=new[] {"path"}
//};
List<string> b = new List<string>();
b.Add("<b>");
List<string> bb = new List<string>();
bb.Add("</b>");

//var values = new
//{
//    from = 0,  // document offset
//    size = 100,  //number of documents to return
//    highlight = new
//    {
//        pre_tags = b,
//        post_tags=bb,
//        fields = new
//        {
//            exact= new
//            {
//                fragment_size=200
//            }
//        }
//    },
//    sort= new List<Sort>(),
//    query = new
//    {
//        match_phrase = new
//        {
//            exact = new
//            {
//                query = str
//            }
//        }
//    }

//};
var values = new
{

    query = str,
    type = "sheet",
    field = "content"


};
var newPostJson = JsonConvert.SerializeObject(values);

var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");

var postResult = await client.PostAsync(endpoint, payload).Result.Content.ReadAsStringAsync();

//var content = new FormUrlEncodedContent(values);

//var response = await client.PostAsync("https://www.sefaria.org/api/search/:index/_search", content);

//var responseString = await response.Content.ReadAsStringAsync();
*/



////var searchResponse = client.LowLevel.Search<Nest.SearchResponse<Document>>("documents", PostData.Serializable(new
////{
////    from = 0,
////    size = 10,
////    query = new
////    {
////        match = new
////        {
////            field = "naive_lemmatizer",
////            query = str
////        }
////    }
////}));
////
///
//
/*
public class Sort
{
            public CompDate comp_date { get; set; }
public Order order { get; set; }
        }
*/


//            using (var client = new HttpClient())
//{
//    var endpoint = new Uri("https://www.sefaria.org/api/search-wrapper");


//    var values = new
//    {
//        query = str,

//        type = "text",

//        field = "naive_lemmatizer",

//    };
//    var newPostJson = JsonConvert.SerializeObject(values);

//    var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");

//    var postResult = await client.PostAsync(endpoint, payload).Result.Content.ReadAsStringAsync();


//chat gpt
/* string newstr = "what is the hebrow bilbe source for: \"" + str +"\" in this format (book name, chapter, verse), answer in hebrow according to wikisource" ;

           HttpClient client = new HttpClient();

           client.DefaultRequestHeaders.Add("authorization", "Bearer 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");

           //var content = new StringContent("{\"model\": \"text-babbage-001\", \"prompt\": \"" + newstr + "\",\"temperature\": 1,\"max_tokens\": 100}",
              // Encoding.UTF8, "application/json");         //string to json without an object
           //Console.WriteLine(content.);
           var values = new 
           {
               model = "gpt-3.5-turbo",
               prompt = newstr,
               temperature = 1,
               max_tokens=100
           };
           var newPostJson = JsonConvert.SerializeObject(values);
           var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");

           HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", payload);

           string responseString = await response.Content.ReadAsStringAsync();

           Console.WriteLine(responseString);*/
/*
var searchRequest = new Nest.SearchRequest<Document>(Nest.Indices.All)
//var client = new RestClient(apiUrl);
{
    From = 0,
    Size = 10,
    Query = new MatchQuery
    {
        Field = Nest.Infer.Field<Document>(f => f.naive_lemmatizer),
        Query = str
    }
};

  var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.sefaria.org/api/search/:index/_search", content);

            var responseString = await response.Content.ReadAsStringAsync();
 */
//  var searchResponse = await client.SearchAsync<Document>(searchRequest);

/*
 * public class Document
    {
        public string path { get; set; }
        public List<string> titleVariants { get; set; }
        public string exact { get; set; }
        public int comp_date { get; set; }
        public List<string> categories { get; set; }
        public string lang { get; set; }
        public double pagesheetrank { get; set; }
        public string title { get; set; }
        public string heRef { get; set; }
        public string version { get; set; }
        public string naive_lemmatizer { get; set; }
        public string reff { get; set; }
        public int version_priority { get; set; }
        public string order { get; set; }
    }
*/