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
    public class BarCodeWebCustomerController : Controller
    {
        //
        // GET: /BarCodeWebCustomer/
        private BarCodeEntities db = new BarCodeEntities();
        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {
            var result = db.Customer.Select(c => c);
            if (!String.IsNullOrEmpty(filter))
            {
                List<String> liFilter = filter.TrimEnd(',').Split(',').ToList();
                if (liFilter[0].ToString() != " ")
                {
                    String CustomerNo = liFilter[0].ToString();
                    result = result
                        .Where(c => c.CustomerNo == CustomerNo);
                }
                //if (liFilter.Count > 1) 
                if (liFilter[1].ToString() != " ")
                {
                    String CustomerName = liFilter[1].ToString();
                    result = result
                        .Where(c => c.CustomerName == CustomerName);
                }
            }

            var InfoList = result
               .OrderBy(Info => Info.CustomerNo)
               .Skip(skip)
               .Take(take);

            var InfoCount = result.Count();

            return Json(new { InfoList, InfoCount });
        }
       
        public ActionResult Create()
        {
            return View();
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult CreateCustomer(IEnumerable<CustomerView> customers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customer = new Customer
                    {
                        CustomerNo = customers.FirstOrDefault().CustomerNo,
                        CustomerName = customers.FirstOrDefault().CustomerName
                    };
                    if (TryUpdateModel(customer))
                        {
                            db.Customer.Add(customer);
                            db.SaveChanges();
                        }
                }
            }
            catch (Exception ex)
            {
            };
            return Json(null);
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
       public ActionResult UpdateCustomer(IEnumerable<CustomerView> customers)
        {
            try 
            {
                if (ModelState.IsValid) 
                {
                    var customer = new Customer 
                        {
                            CustomerNo = customers.FirstOrDefault().CustomerNo,
                            CustomerName = customers.FirstOrDefault().CustomerName
                        };
                    if (TryUpdateModel(customer)) 
                        {
                            db.Entry(customer).State = EntityState.Modified;
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
        public ActionResult DeleteCustomer(IEnumerable<CustomerView> customers)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                    Customer customer = db.Customer.Find(customers.FirstOrDefault().CustomerNo);
                    db.Customer.Remove(customer);
                    if (TryUpdateModel(customer))
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
