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
    public class BarCodeWebZoneController : Controller
    {
        //
        // GET: /BarCodeWebStaff/
        private BarCodeEntities db = new BarCodeEntities();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {
            var result = db.Zone.Select(z => z);
            if (!String.IsNullOrEmpty(filter))
            {
                List<String> liFilter = filter.TrimEnd(',').Split(',').ToList();
                //int ZoneID = Convert.ToInt16(liFilter[0].ToString());
                if (liFilter[0].ToString() != " ") 
                {
                    String ZoneID = liFilter[0].ToString();
                    result = result
                        .Where(z => z.ZoneID == ZoneID);
                }
               
                //if (liFilter.Count > 1)
                if (liFilter[1].ToString() != " ") 
                {
                    String ZoneNo = liFilter[1].ToString();
                    result = result
                        .Where(z => z.ZoneNo == ZoneNo);
                }
            }

            var InfoList = result
               .OrderBy(Info => Info.ZoneID)
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
        public ActionResult CreateZone(IEnumerable<ZoneView> zones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var zone = new Zone
                    {
                        ZoneID = zones.FirstOrDefault().ZoneID,
                        ZoneNo = zones.FirstOrDefault().ZoneNo
                    };
                    if (TryUpdateModel(zone))
                    {
                        db.Zone.Add(zone);
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
        public ActionResult UpdateZone(IEnumerable<ZoneView> zones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var zone = new Zone
                    {
                        ZoneID = zones.FirstOrDefault().ZoneID,
                        ZoneNo = zones.FirstOrDefault().ZoneNo
                    };
                    if (TryUpdateModel(zone))
                    {
                        db.Entry(zone).State = EntityState.Modified;
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
        public ActionResult DeleteZone(IEnumerable<ZoneView> zones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Zone zone = db.Zone.Find(zones.FirstOrDefault().ZoneID);
                    db.Zone.Remove(zone);
                    if (TryUpdateModel(zone))
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
