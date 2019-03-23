using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehicle.Allocation.Abstract
{
    public class Offer
    {
        public int pk { get; set; }
        public string firm { get; set; }
        public string phone { get; set; }
        public string date { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public int population { get; set; }
        public decimal fee { get; set; }        
    }
}