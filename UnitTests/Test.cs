using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Freshdesk;
using System;
using System.Linq;

namespace UnitTests;

public class Test
{
        private static readonly HttpClient client = new HttpClient();
        [Fact]
        public void GetCorrectGitUser()
        {
            string username = "BARSZ";
            GitHubUser vasil = new GitHubUser
            {
                name = "Vasil Simeonov"
            };
            string name = GetNameAsync(username).Result;
            Assert.Equal(vasil.name, name);
        }
        [Fact]
        public void GetListOfFreshdeskUsers()
        {
             FreshdeskService freshdeskService = new FreshdeskService("WczThvvBuAaZfNDx0dg3", new Uri("https://vasilsimeonov.freshdesk.com"));
             bool pass = true;
             try
             {
                var users = freshdeskService.GetUsers().ToList();
             }
             catch(Exception ex)
             {
                System.Console.WriteLine(ex.ToString());
                pass = false;
             }
             Assert.True(pass);
        }
        private async Task<string> GetNameAsync(string username)
        {
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

             client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("GITHUB_TOKEN");

             var streamTask = client.GetStreamAsync("https://api.github.com/users/" + username);

             var contact = await JsonSerializer.DeserializeAsync<GitHubUser>(await streamTask);

             client.DefaultRequestHeaders.Accept.Clear();

             return contact.name;
        }
}