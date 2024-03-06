using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sourceSearch
{
    internal class Post
    {
        public static async Task<string> VerseSource(string str)
        {
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

            var postResult = await (await client.PostAsync(endpoint, payload)).Content.ReadAsStringAsync();

            return postResult;

        }
        private static void ConfigurationService(ServiceCollection services)
        {
            services.AddHttpClient();
        }
    }
}
