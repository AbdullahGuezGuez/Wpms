using System.Collections.Generic;

namespace Backend.Dtos.TrelloDto
{

        public class TasksDto 
        {
            public string UserName { get; set; }
            public List<string> CardNames { get; set; }
        }
}