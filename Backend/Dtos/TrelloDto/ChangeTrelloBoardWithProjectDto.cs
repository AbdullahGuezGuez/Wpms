using System;
using System.Collections.Generic;
using Backend.Models;


namespace Backend.Dtos.TrelloDto
{
    public class ChangeTrelloBoardWithProjectDto
    {
        public string trelloBoardId { get; set; }
        public int projectId { get; set; }
        
    }
}