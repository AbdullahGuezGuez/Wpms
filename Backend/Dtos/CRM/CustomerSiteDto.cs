using System.Collections.Generic;
using Backend.Models;

namespace Backend.Dtos
{
    public class CustomerSiteDto
    {
        public Customer Customer { get; set; }
        //public List<Contactperson> Contactpersons { get; set; }
        public string Status { get; set; }
        //*Finns mer Stuff att lägga till  */
    }
}