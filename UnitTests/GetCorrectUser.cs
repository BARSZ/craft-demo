using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
namespace UnitTests;

public class GetCorrectUser
{
        private static readonly HttpClient client = new HttpClient();
        [Fact]
        public void GetGitHubUser()
        {
            string username = "BARSZ";
            GitHubUser vasil = new GitHubUser
            {
                name = "Vasil Simeonov"
            };
            string name = GetNameAsync(username).Result;
            Assert.Equal(vasil.name, name);
        }
        private async Task<string> GetNameAsync(string username)
        {
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

             client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("GITHUB_TOKEN");

             var streamTask = client.GetStreamAsync("https://api.github.com/users/" + username);

             var contact = await JsonSerializer.DeserializeAsync<GitHubUser>(await streamTask);

             return contact.name;
        }
}