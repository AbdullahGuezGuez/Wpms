namespace Backend.Models
{
    public class ActivityContactperson
    {
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int ContactpersonId { get; set; }
        public Contactperson Contactperson { get; set; }
    }
}