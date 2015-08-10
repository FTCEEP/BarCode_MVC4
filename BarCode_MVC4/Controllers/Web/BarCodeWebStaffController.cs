using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Net;

using BarCode_MVC4.Models;
using BarCode_MVC4.Controllers;

using KendoGridBinder;
using Kendo.Mvc.UI;
using Kendo.DynamicLinq;

namespace BarCode_MVC4.Controllers.Web
{
    public class BarCodeWebStaffController : Controller
    {
        //
        // GET: /BarCodeWebStaff/
        private DBOperation oDB = new DBOperation();
        private BarCodeEntities db = new BarCodeEntities();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {
            var result = db.Staff_View.Select(s => s);
            var InfoList = result;
            var InfoCount = 0;
            var InfoRole = Session["Role"].ToString();
            //ViewBag.UserRole = Session["Role"].ToString();
            //ViewData["UserRole2"] = Session["Role"].ToString();
            try
            {
                //為admin
                if (!String.IsNullOrEmpty(filter) && Session["Role"].ToString() == "Admin")
                {
                    List<String> liFilter = filter.TrimEnd(',').Split(',').ToList();
                    if (liFilter[0].ToString() != " ")
                    {
                        String Account = liFilter[0].ToString();
                        result = result
                            .Where(s => s.Account == Account);
                    }
                    if (liFilter[1].ToString() != " ")
                    {
                        String Name = liFilter[1].ToString();
                        result = result
                            .Where(s => s.Name == Name);
                    }
                }
                //非admin
                else if (Session["Role"].ToString() == "User")
                {
                    String Account = Session["Account"].ToString();
                    result = result
                           .Where(s => s.Account == Account);
                }

                InfoList = result
                   .OrderBy(Info => Info.Account)
                   .Skip(skip)
                   .Take(take);

                InfoCount = result.Count();
            }
            catch(Exception ex)
            {

            }
            return Json(new { InfoList, InfoCount, InfoRole });
        }
        public ActionResult Create()
        {
            return View();
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult CreateStaff(IEnumerable<StaffView> staffs)
        {
            //產生加密誤密碼字串
            String sPwd = oDB.EncryptString(staffs.FirstOrDefault().DecryptionPassword);
            try
            {
                if (ModelState.IsValid)
                {
                    var staff = new Staff
                    {
                        Account = staffs.FirstOrDefault().Account,
                        Name = staffs.FirstOrDefault().Name,
                        Password = sPwd,
                        DecryptionPassword = staffs.FirstOrDefault().DecryptionPassword,
                        Role = staffs.FirstOrDefault().Role
                    };
                    if (TryUpdateModel(staff))
                    {
                        db.Staff.Add(staff);
                        db.SaveChanges();
                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
            };
            return Json(null);
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult UpdateStaff(IEnumerable<StaffView> staffs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //產生加密誤密碼字串
                    String sPwd = oDB.EncryptString(staffs.FirstOrDefault().DecryptionPassword);

                    var staff = new Staff
                    {
                        Account = staffs.FirstOrDefault().Account,
                        Name = staffs.FirstOrDefault().Name,
                        Password = sPwd,
                        DecryptionPassword = staffs.FirstOrDefault().DecryptionPassword,
                        Role = staffs.FirstOrDefault().Role
                    };
                    if (TryUpdateModel(staff))
                    {
                        db.Entry(staff).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            };
            return Json(null);
        }
        //AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult DeleteStaff(IEnumerable<StaffView> staffs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Staff staff = db.Staff.Find(staffs.FirstOrDefault().Account);
                    db.Staff.Remove(staff);
                    if (TryUpdateModel(staff))
                    {
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            };
            return Json(null);
        }
    }
}
