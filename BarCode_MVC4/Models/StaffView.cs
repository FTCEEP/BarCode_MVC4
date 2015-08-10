using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarCode_MVC4.Models
{
    public class StaffView
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string DecryptionPassword { get; set; }
        public string Role { get; set; }
    }
}