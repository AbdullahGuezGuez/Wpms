using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;

namespace Backend.Dtos
{
    public class ActivitiesForNotificationDto
    {
        public int NumberOfTodos { get; set; }
        public List<ActivityNotificationDto> UsersActivities { get; set; }
                
    }
}