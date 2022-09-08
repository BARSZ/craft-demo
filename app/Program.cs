// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http.Headers;
using Freshdesk;
using System.Linq;

namespace CraftDemo
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            System.Console.Write("Enter GitHub username: ");
            string username = Console.ReadLine();

            if(username is not null)
            {
                await AddContactToFreshdesk(username);
            }
        }
        private static async Task AddContactToFreshdesk(string username)
        {
            //Get GitHub profile by username
             client.DefaultRequestHeaders.Accept.Clear();
             client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

             client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("GITHUB_TOKEN");

             var streamTask = client.GetStreamAsync("https://api.github.com/users/" + username);

             var contact = await JsonSerializer.DeserializeAsync<craft_demo.Contact>(await streamTask);

             client.DefaultRequestHeaders.Accept.Clear();
            
             FreshdeskService freshdeskService = new FreshdeskService("WczThvvBuAaZfNDx0dg3", new Uri("https://vasilsimeonov.freshdesk.com"));
             bool contactExists = false;

             //Get a list of Freshdesk contacts
             var users = freshdeskService.GetUsers().ToList();

             foreach(var user in users)
             {
                //Check if a contact exists
                if(user.User.Name == contact.name)
                {
                    contactExists = true;
                    
                    //Update contact
                    freshdeskService.UpdateUser(new UpdateUserRequest()
                    {
                        User = new User()
                        {
                            Name = contact.name,
                            Email = contact.email,
                            Description = contact.html_url + "\n Updated!",
                            Address = contact.location
                        }
                    }
                    ,(long)user.User.Id);
                    System.Console.WriteLine("Contact updated successfully!");
                }
             }
             if(!contactExists)
             {
                //Create new contact
                 GetUserResponse userResponse = freshdeskService.CreateUser(new CreateUserRequest()
                 {
                    User = new User()
                    {
                        Name = contact.name,
                        Email = contact.email,
                        Description = contact.html_url,
                        Address = contact.location
                    }
                 });
                 System.Console.WriteLine("Contact added successfully!");
             }
        }
    }
}
