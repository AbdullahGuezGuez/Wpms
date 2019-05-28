namespace Backend.Models
{
    public class NextstepContactperson
    {
        public int NextstepId { get; set; }
        public NextStep NextStep { get; set; }

        public int ContactpersonId { get; set; }
        public Contactperson Contactperson { get; set; }
    }
}