namespace Backend.Dtos
{
	public class ChangePrioDto
    {
        public int ProjectId { get; set; }
        public int NewPrio { get; set; }
        public int OldPrio { get; set; }
    }
}