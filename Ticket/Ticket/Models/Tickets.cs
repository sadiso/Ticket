using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket.Models
{
    public class Tickets
    {
        public int TicketId { get; set; }
        public string TicketCode { get; set; }
        public string Datetime { get; set; }  
        public int UserId { get; set; }
    }
}
