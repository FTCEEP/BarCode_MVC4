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
    public class BarCodeWebPackagingMaterialController : Controller
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
            var result = db.PackagingMaterial.Select(m => m);
            if (!String.IsNullOrEmpty(filter))
            {
                List<String> liFilter = filter.TrimEnd(',').Split(',').ToList();
                if (liFilter[0].ToString() != " ")
                {
                    String PackNo = liFilter[0].ToString();
                    result = result
                        .Where(m => m.PackNo == PackNo);
                }
                //if (liFilter.Count > 1)
                if (liFilter[1].ToString() != " ") 
                {
                    String PackName = liFilter[1].ToString();
                    result = result
                        .Where(m => m.PackName == PackName);
                }
            }

            var InfoList = result
               .OrderBy(Info => Info.PackNo)
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
        public ActionResult CreatePack(IEnumerable<PackagingMaterialView> packs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pack= new PackagingMaterial
                    {
                        PackNo = packs.FirstOrDefault().PackNo,
                        PackName = packs.FirstOrDefault().PackName
                    };
                    if (TryUpdateModel(pack))
                    {
                        db.PackagingMaterial.Add(pack);
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
        public ActionResult UpdatePack(IEnumerable<PackagingMaterialView> packs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pack = new PackagingMaterial
                    {
                        PackNo = packs.FirstOrDefault().PackNo,
                        PackName = packs.FirstOrDefault().PackName
                    };
                    if (TryUpdateModel(pack))
                    {
                        db.Entry(pack).State = EntityState.Modified;
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
        public ActionResult DeletePack(IEnumerable<PackagingMaterialView> packs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   PackagingMaterial pack = db.PackagingMaterial.Find(packs.FirstOrDefault().PackNo);
                   db.PackagingMaterial.Remove(pack);
                    if (TryUpdateModel(pack))
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
