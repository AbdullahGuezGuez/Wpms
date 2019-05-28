using System.Collections.Generic;
using System;

namespace Backend.Models
{
    public partial class TrelloBoard
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Closed { get; set; }
        public string Url { get; set; }
        public DateTime DateLastView { get; set; }
        public ICollection<Membership> Memberships { get; set; }
        public int OrganizationId { get; set; }
    }
    public partial class Membership
    {
        public string Id { get; set; } //Id för denna board
        public string IdMember { get; set; } //MedlemsId för Trello
        public MemberType MemberType { get; set; }
        public string TrelloBoardId { get; set; }
        public TrelloBoard TrelloBoard { get; set; }
    }

    public enum MemberType { Admin, Normal }    //TODO: MAPAS I ENUM
}
