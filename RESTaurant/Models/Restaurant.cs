using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant.Models
{
    internal class Restaurant
    {
        public List<Table>? AvailableTables { get; set; }
        public List<Table>? TakenTables { get; set; }


    }
}
