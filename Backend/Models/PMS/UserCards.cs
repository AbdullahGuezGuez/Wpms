namespace Backend.Models.PMS
{
    public class UserCards
    {
    public string TrelloCardId {get; set;}
    public TrelloCard TrelloCard {get; set;}

    public string TrelloMemberId{get; set;}
    public int UserId {get; set;}
    public User User {get; set;}

    }
}