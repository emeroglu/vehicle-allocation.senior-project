using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehicle.Allocation.Abstract
{
    public class Account
    {
        public int pk { get; set; }
        public string type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string field { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
    }
}