// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http.Headers;
using Freshdesk;
using System.Linq;
using craft_demo;

namespace CraftDemo
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static FreshdeskService freshdeskService;
        static async Task Main(string[] args)
        {
            string username = "";
            System.Console.Write("Enter GitHub username: ");
            try
            {
                username = Console.ReadLine();
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
            Task<Contact> getGitTask = GetGitHubProfile(username);
            Contact contact = await getGitTask;

            if(ContactExists(contact) == 0)
            {
                await CreateFreshdeskContact(contact);
            }
            else
            {
                await UpdateFreshdeskContact(contact, ContactExists(contact));
            }
        }
        private static async Task<Contact> GetGitHubProfile(string username)
        {
            //Get GitHub profile by username
             client.DefaultRequestHeaders.Accept.Clear();
             client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
             client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("GITHUB_TOKEN");

             var streamContact = client.GetStreamAsync("https://api.github.com/users/" + username);
             var contact = await JsonSerializer.DeserializeAsync<Contact>(await streamContact);

             client.DefaultRequestHeaders.Accept.Clear();

             return contact;
        }
        private static Task UpdateFreshdeskContact(Contact contact, long id)
        {
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
            },id);
            System.Console.WriteLine("Contact updated successfully!");
           return Task.CompletedTask;
        }
        private static Task CreateFreshdeskContact(Contact contact)
        {
            freshdeskService = new FreshdeskService("WczThvvBuAaZfNDx0dg3", new Uri("https://vasilsimeonov.freshdesk.com"));
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
                 return Task.CompletedTask;
        }
        private static long ContactExists(Contact contactToAdd)
        {
            freshdeskService = new FreshdeskService("WczThvvBuAaZfNDx0dg3", new Uri("https://vasilsimeonov.freshdesk.com"));
            
            //Get a list of Freshdesk contacts
             var users = freshdeskService.GetUsers().ToList();

             //Check if contact already exists
             foreach(var user in users)
             {
                if(user.User.Name == contactToAdd.name)
                {
                    return (long)user.User.Id;
                }
             }
             return 0;
        }
    }
}
