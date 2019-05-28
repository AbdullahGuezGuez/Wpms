using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Backend.Data
{
    public class WpmsRepository : IWpmsRepository
    {


        private readonly DataContext _context;
        public WpmsRepository(DataContext context)
        {

            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = _context.Users.AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<string> DeleteUserFromTrelloBoard(int orgId, string boardId, string trelloMemberId)
        {
            using (HttpClient client = new HttpClient())
            {
                var organization = await _context.Organizations.FirstAsync(x => x.Id == orgId);
                var url = $"https://api.trello.com/1/boards/{boardId}/members/{trelloMemberId}&key={organization.Trellokey}&token={organization.Trellotoken}";
                await client.DeleteAsync(url);
            }
            return ("aa");
        }

        public async Task<string> AddUserToTrelloBoard(string boardId, string userMail, int orgId)
        {
            using (HttpClient client = new HttpClient())
            {
                var organization = await _context.Organizations.FirstAsync(x => x.Id == orgId);
                var url = $"https://api.trello.com/1/boards/{boardId}/members?email={userMail}&key={organization.Trellokey}&token={organization.Trellotoken}";
                await client.PutAsync(url, null);
            }
            return ("asdasda");
        }

        public TrelloBoard ConvertJsonToTrelloBoard(string json)
        {
            var board = JsonConvert.DeserializeObject<TrelloBoard>(json);
            return board;
        }

        public async Task<string> CreateTrelloBoard(string name, int orgId)
        {

            using (HttpClient client = new HttpClient())
            {
                var organization = await _context.Organizations.FirstAsync(x => x.Id == orgId);


                var url = $"https://api.trello.com/1/boards?name={name}&defaultLabels=true&defaultLists=true&idOrganization={organization.TrelloTeamName}&keepFromSource=none&prefs_permissionLevel=private&prefs_voting=disabled&prefs_comments=members&prefs_invitations=members&prefs_selfJoin=true&prefs_cardCovers=true&prefs_background=blue&prefs_cardAging=regular&key={organization.Trellokey}&token={organization.Trellotoken}";

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, null);

                    if (null == response || !response.IsSuccessStatusCode ||
                        response.StatusCode == HttpStatusCode.NoContent)
                        return null;

                    var responseJson = await response.Content.ReadAsStringAsync();
                    return responseJson;
                }
                catch (HttpRequestException rx)
                {
                    Console.WriteLine(rx);
                    throw;
                }
            }
        }
        public async Task<bool> TrelloBoardExist(string name)
        {
            bool exist = false;
            if (await _context.TrelloBoards.AnyAsync(x => x.Name == name))
            {
                exist = true;
            }
            return exist;
        }

        public Task<bool> SaveAll()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> CreateTrelloBoard(string name)
        {
            throw new NotImplementedException();
        }

        public string ComposeUri(string path, string extraFields = "")
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteUserFromTrelloBoard()
        {
            throw new NotImplementedException();
        }

        public Task<string> AddUserToTrelloBoard()
        {
            throw new NotImplementedException();
        }
    }
}