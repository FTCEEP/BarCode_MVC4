using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BarCode_MVC4.Models
{
    //[MetadataType(typeof(Staff))]
    public class LoginView
    {
        [Required(ErrorMessage = "請輸入使用者帳號")]
        public string Account { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }
    }
}