using System.Collections.Generic;
using System;
using Backend.Models.PMS;
using Newtonsoft.Json;

public partial class TrelloCard
{
    public string Id { get; set; }
    public bool Closed { get; set; }
    public DateTime DateLastActivity { get; set; }
    public string Desc { get; set; }
    public string IdBoard { get; set; }
    //public string[] IdLabels { get; set; }
    public string IdList { get; set; }
    public string Name { get; set; }                    //TODO: SE ÖVER ARRAYERNA
    public float Pos { get; set; }
    public string Url { get; set; }
    public ICollection<UserCards> UserCards { get; set; }
    public List<string> IdMembers { get; set; }
    public List<CustomFieldItem> CustomFieldItems { get; set; }
}
public class IdMembers{
    public string Id {get;set;}
    public string MemberId {get;set;}
}
    //public Label[] Labels { get; set; }

    // public object Address { get; set; }
    // public object CheckItemStates { get; set; }
    // public object Coordinates { get; set; }
    // public object CreationMethod { get; set; }
    // public object DescData { get; set; }
    // public object DueReminder { get; set; }
    // public object Due { get; set; }
    // public object LocationName { get; set; }
    // public object IdAttachmentCover { get; set; }
    // public object[] IdMembersVoted { get; set; }
    // public object[] IdChecklists { get; set; }
    // public long IdShort { get; set; }
    // public string ShortLink { get; set; }
    // public Limits Limits { get; set; }
    // public Badges Badges { get; set; }
    // public bool ManualCoverAttachment { get; set; }
    // public bool DueComplete { get; set; }
    // public bool Subscribed { get; set; }
    // public Uri ShortUrl { get; set; }


// public partial class Badges
// {
//     public AttachmentsByType AttachmentsByType { get; set; }
//     public bool Location { get; set; }
//     public long Votes { get; set; }
//     public bool ViewingMemberVoted { get; set; }
//     public bool Subscribed { get; set; }
//     public string Fogbugz { get; set; }
//     public long CheckItems { get; set; }
//     public long CheckItemsChecked { get; set; }
//     public long Comments { get; set; }
//     public long Attachments { get; set; }
//     public bool Description { get; set; }
//     public object Due { get; set; }
//     public bool DueComplete { get; set; }
// }

// public partial class AttachmentsByType
// {
//     public Trello Trello { get; set; }
// }

// public partial class Trello
// {
//     public long Board { get; set; }
//     public long Card { get; set; }
// }

public partial class Label
{
    public string Id { get; set; }
    public string IdBoard { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}

public partial class CustomFieldItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }

        [JsonProperty("idCustomField")]
        public string IdCustomField { get; set; }

        [JsonProperty("idModel")]
        public string IdModel { get; set; }

        [JsonProperty("modelType")]
        public string ModelType { get; set; }
        public string TrelloCardId { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("number")]
        public int Number { get; set; }
        public int Id { get; set; }
        public string CustomFieldItemId { get; set; }
    }

// public partial class Limits
// {
//     public Attachments Attachments { get; set; }
//     public Attachments Checklists { get; set; }
//     public Attachments Stickers { get; set; }
// }

// public partial class Attachments
// {
//     public PerCard PerCard { get; set; }
// }

// public partial class PerCard
// {
//     public string Status { get; set; }
//     public long DisableAt { get; set; }
//     public long WarnAt { get; set; }
// }
