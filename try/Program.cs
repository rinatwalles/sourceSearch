using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
namespace try1
{


class Program
{
    static async Task Main(string[] args)
    {
        string query = "שלום";
        string category = "Tanakh";

        string url = $"https://www.sefaria.org/api/v2/search?q={query}&category={category}";

        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            //dynamic data = JsonConvert.DeserializeObject(responseBody);

            //        if (data?.error == null)
            //        {
            //            List<dynamic> hits = data?.hits;

            //            if (hits?.Count > 0)
            //            {
            //                foreach (dynamic hit in hits)
            //                {
            //                    string refStr = hit?.ref;
            //                    string text = hit?.highlight[0]?.text;

            //                    Console.WriteLine($"Reference: {refStr}");
            //                    Console.WriteLine($"Text: {text}");
            //                    Console.WriteLine();
            //                }
            //            }
            //            else
            //            {
            //                Console.WriteLine("Error: No matching results found");
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine($"Error: {data.error}");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("Error: Unable to retrieve data from API");
            //    }
        }
    }
}
}
