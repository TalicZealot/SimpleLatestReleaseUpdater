using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleLatestReleaseUpdater
{
    public static class UpdateGetter
    {
        private const string GithubApiCall = "https://api.github.com/repos/";

        internal class Release
        {
            public Asset[] assets { get; set; }
        }

        internal class Asset
        {
            public string name { get; set; }
            public string browser_download_url { get; set; }
        }

        public static async Task<string> GetLatestReleaseLink(string username, string repository, string updateNamePrefix)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.UseDefaultCredentials = true;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                    HttpResponseMessage response = await client.GetAsync(GithubApiCall + username + "/" + repository + "/releases");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        Release release = JsonConvert.DeserializeObject<IEnumerable<Release>>(data).FirstOrDefault();
                        if (release is null)
                        {
                            return "Error: No releases in repository";
                        }
                        Asset update = release.assets.Where(a => a.name.StartsWith(updateNamePrefix)).FirstOrDefault();
                        if (update != null)
                        {
                            return update.browser_download_url;
                        }
                        else
                        {
                            return "Error: Update not found";
                        }
                    }
                    return $"Error: {response.StatusCode}";
                }
            }
        }
    }
}
