using System;
using System.Collections.Generic;
using Backend.Models;


namespace Backend.Dtos.TrelloDto
{
    public class CreateTrelloBoardDto
    {
        public string name { get; set; }
        public int projectId { get; set; }
        
    }
}