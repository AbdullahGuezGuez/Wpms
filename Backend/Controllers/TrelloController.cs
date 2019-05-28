using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Dtos;
using Backend.Models;
using AutoMapper;
using Backend.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Backend.Models.PMS;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous] //! Ta bort vid leverans
    public class TrelloController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IWpmsRepository _wpmsRepository;
 
        public TrelloController(DataContext context, IMapper mapper, IWpmsRepository IWpmsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _wpmsRepository = IWpmsRepository;
        }
 
        [HttpGet("updatetrellomembers")]
        public async Task<IActionResult> SetTrelloIdToAllUsers()
        {
            using(HttpClient client = new HttpClient())
            {
 
                var organizations = await _context.Organizations.ToListAsync();
               
 
                foreach(var organization in organizations)
                {
                    var organizationUsers = await _context.Users.FromSql("SELECT * FROM AspNetUsers WHERE id IN (SELECT UserId From OrganizationUser where OrganizationUser.OrganizationId = " + organization.Id + ")").ToListAsync();
                    try                                
                    {
                        HttpResponseMessage memberResponse = await client.GetAsync("https://api.trello.com/1/organizations/"+organization.TrelloTeamName+"/members?key="+organization.Trellokey+"&token="+organization.Trellotoken);
                        memberResponse.EnsureSuccessStatusCode();
                        string memberResponseBody = await memberResponse.Content.ReadAsStringAsync();
                        var trelloMemberships = JsonConvert.DeserializeObject<List<TrelloMemberDto>>(memberResponseBody);
                        foreach(var user in organizationUsers)
                        {
                            foreach(var trelloMembership in trelloMemberships)
                            {
                                if(trelloMembership.fullName.ToUpper() == user.FullName.ToUpper())
                                {
                                    user.TrelloMemberId = trelloMembership.id;
                                    break;
                                }
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch(HttpRequestException e)
                    {
                        Console.WriteLine(e);
                    }
                }
               
            }


            return Ok();
        }

       
        //TODO: PMS ADMIN CLAIM
        [HttpGet("updatetrellodata")]
        public async Task<IActionResult> UpdateTrelloData()
        {
            await SetTrelloIdToAllUsers();
            var TrelloBoardsExists = await _context.TrelloBoards.AnyAsync();
            var TrelloCardsExists = await _context.TrelloCards.AnyAsync();
            var TrelloListsExists = await _context.TrelloLists.AnyAsync();
 
            if(TrelloBoardsExists || TrelloCardsExists || TrelloListsExists)
            {
                await TrelloDbRemove();
            }
           
            using(HttpClient client = new HttpClient())
            {
                var organizations = await _context.Organizations.ToListAsync();
            
 
                foreach(var organization in organizations)
                {
                   var organizationUsers = await _context.Users.FromSql("SELECT * FROM AspNetUsers WHERE id IN (SELECT UserId From OrganizationUser where AspNetUsers.Masked = false AND OrganizationUser.OrganizationId = " + organization.Id + ")").ToListAsync();
                   
                    try                                
                    {
                        HttpResponseMessage boardResponse = await client.GetAsync("https://api.trello.com/1/organizations/"+organization.TrelloTeamName+"/boards?key="+organization.Trellokey+"&token="+organization.Trellotoken);
                        boardResponse.EnsureSuccessStatusCode();
                        string boardResponseBody = await boardResponse.Content.ReadAsStringAsync();
                        var boards = JsonConvert.DeserializeObject<List<TrelloBoard>>(boardResponseBody);
                        foreach(var board in boards)
                        {
                            board.OrganizationId = organization.Id;
                            await _context.TrelloBoards.AddAsync(board);
                            HttpResponseMessage listResponse = await client.GetAsync("https://api.trello.com/1/boards/" + board.Id + "/lists?cards=none&card_fields=all&filter=open&fields=all&key="+organization.Trellokey+"&token="+organization.Trellotoken);
                            listResponse.EnsureSuccessStatusCode();
                            string listResponseBody = await listResponse.Content.ReadAsStringAsync();

                            var lists = JsonConvert.DeserializeObject<List<TrelloList>>(listResponseBody);
                            foreach(var list in lists)
                            {
                                await _context.TrelloLists.AddAsync(list);
                                HttpResponseMessage cardResponse = await client.GetAsync("https://api.trello.com/1/lists/" + list.Id + "/cards?fields=all&customFieldItems=true&key="+organization.Trellokey+"&token="+organization.Trellotoken);
                                cardResponse.EnsureSuccessStatusCode();
                                string cardResponseBody = await cardResponse.Content.ReadAsStringAsync();

                                var cards = JsonConvert.DeserializeObject<List<TrelloCard>>(cardResponseBody);
                                foreach(var card in cards)
                                {
                                    await _context.TrelloCards.AddAsync(card);
                                    foreach(var cardMember in card.IdMembers)
                                    {
                                        var userCard = new UserCards();
                                        userCard.TrelloCardId = card.Id;
                                        foreach(var user in organizationUsers)
                                        {
                                            if(user.TrelloMemberId == cardMember)
                                            {
                                                userCard.TrelloMemberId = cardMember;
                                                userCard.User = user;
                                                await _context.UserCards.AddAsync(userCard);
                                                await _context.SaveChangesAsync();
                                                break;
                                            }
                                        }

                                    }
                                }
                            }
                        }
 
                        await _context.SaveChangesAsync();
                    }
                    catch(HttpRequestException e)
                    {
                        Console.WriteLine(e);
                    }
                }
               
            }
            return Ok();
        }
 
 
        private async System.Threading.Tasks.Task TrelloDbRemove()
        {  
            var lists = await _context.TrelloLists.ToListAsync();
            _context.RemoveRange(lists);
            var cards = await _context.TrelloCards.ToListAsync();
            _context.RemoveRange(cards);
            var boards = await _context.TrelloBoards.ToListAsync();
            _context.RemoveRange(boards);
            var userCards = await _context.UserCards.ToListAsync();
            _context.RemoveRange(userCards);
            var values = await _context.Values.ToListAsync();
            _context.RemoveRange(values);
            var customFieldItems = await _context.CustomFieldItems.ToListAsync();
            _context.RemoveRange(customFieldItems);
            await _context.SaveChangesAsync();
        }
 
    }
}