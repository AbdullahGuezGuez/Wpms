namespace Backend.Dtos
{
    public class ProjectValuesDto
    {
        public string TrelloUrl { get; set; }
        public string TrelloBoardName { get; set; }
        public bool HasTrello { get; set; }
        public int ProjectId { get; set; }
        public int EstimatedTime { get; set; }
        public int CompletedTime { get; set; }
        public int InProductionTime { get; set; }
        public int InTestTime { get; set; }
        public int EstimatedCount { get; set; }
        public int CompletedCount { get; set; }
        public int InProductionCount { get; set; }
        public int InTestCount { get; set; }
    }
}