using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BarCode_MVC4.Models;
using BarCode_MVC4.Controllers;

namespace BarCode_MVC4.Controllers.Web
{
    public class BarCodeWebLoginController : Controller
    {
        //加入oDB物件
        DBOperation oDB = new DBOperation();

        public ActionResult Login()
        {
            //判斷使用者是否已經登入
            if (User.Identity.IsAuthenticated)
            {
                //已登入則導入到Index頁面
                return RedirectToAction("Index", "BarCodeWebIndex");
            }
            else 
            {
                //否則導入回登入頁面
                return View();
            }
        }
        //傳入登入資料的Action
        [HttpPost]
        public ActionResult Login(LoginView UserInfo) 
        {
            //前端資料輸入驗證
            if (this.ModelState.IsValid)
            {
                //密碼加密
                String sPwd = oDB.EncryptString(UserInfo.Password);
                //check使用者登入的帳密
                string sValidate = oDB.LoginCheck(UserInfo.Account, sPwd);
                //判斷後如果沒有錯誤訊息則登入
                if (String.IsNullOrEmpty(sValidate))
                {
                    Session["Account"] = UserInfo.Account;
                    Session["Role"] = oDB.RoleInfo(UserInfo.Account);
                    return RedirectToAction("Index", "BarCodeWebIndex");
                }
                else
                {
                    //加入錯誤訊息
                    ViewData["ErrorMsg"] = sValidate;
                    //停留在登入畫面
                    return View();
                }
            }
            else
            {
                //停留在登入畫面
                return View();
            }
        }
    }
}
