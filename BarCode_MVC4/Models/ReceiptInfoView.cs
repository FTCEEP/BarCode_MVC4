using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarCode_MVC4.Models
{
    public class ReceiptInfoView
    {
        public string Type { get; set; }
        public string Prod { get; set; }
        public string Thickness { get; set; }
        public string Widt { get; set; }
        public Nullable<int> Leng { get; set; }
        public string Splice { get; set; }
        public Nullable<int> NewWeight { get; set; }
        public Nullable<int> GrossWeight { get; set; }
        public string Ptno { get; set; }
        public Nullable<System.DateTime> ProductDate { get; set; }
        public string CustomerNO { get; set; }
        public string PackNo { get; set; }
        public string StaffID { get; set; }
        public string ZoneID { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
    }
}