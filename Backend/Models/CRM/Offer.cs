using System;
using System.Collections.Generic;

namespace Backend.Models
{
    //! Antaglingen helt onödig
    public class Offer //OrgId
    {
        public int Id { get; set; }
        public string SalesName { get; set; } //? User från CRM
        public string TypeOfProject { get; set; } // Typ av tjänst
        public DateTime Deliverydate { get; set; }
        public float Price { get; set; }
        public float PricePerUnit { get; set; }
        public float Discounts { get; set; }
        public float EstimatedTime { get; set; } // Estimerade timmar
        public string FreeText { get; set; } // Typ leverans och betalningsvillkor
        public bool Successful { get; set; } // Resulterade offerten i en beställning
    }
}