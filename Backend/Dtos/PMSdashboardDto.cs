using System.Collections.Generic;
using Backend.Models;

namespace Backend.Dtos
{
    public class PMSdashboardDto
    {
        public List<PMSdashboardProjectDto> Projects { get; set; }
        public int NumberOfProjects { get; set; }
    }

    public class PMSdashboardProjectDto
    {
        
        public string Name { get; set; }
        public int Priority { get; set; }
        public int Id { get; set; }
        public ProjectValuesDto Values { get; set; }
        public List<PMSdashboardCardDto> ProductionCards { get; set; }
        public bool HasProductionList { get; set; }
        public bool HasCards { get; set; }
    }

    public class PMSdashboardCardDto
    {
        public string Content { get; set; }
        public int EstimatedTime { get; set; }
        public bool HasEstimatedTime { get; set; }
        public bool HasAssignedUser { get; set; }
        public string User { get; set; }
        public string Url { get; set; }
    }
}