using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehicle.Allocation.Abstract
{
    public class Request
    {
        public int pk { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string date { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public int population { get; set; }
    }
}