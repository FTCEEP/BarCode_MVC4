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
    public class BarCodeWebNoteController : Controller
    {
        //
        // GET: /BarCodeWebNote/
        private BarCodeEntities db = new BarCodeEntities();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {
            var result = db.Note.Select(c => c);
            if (!String.IsNullOrEmpty(filter))
            {
                List<String> liFilter = filter.TrimEnd(',').Split(',').ToList();
                if (liFilter[0].ToString() != " ")
                {
                    String CtlNo = liFilter[0].ToString();
                    result = result
                        .Where(c => c.CtlNo == CtlNo);
                }
                //if (liFilter.Count > 1) 
                if (liFilter[1].ToString() != " ")
                {
                    String CtlName = liFilter[1].ToString();
                    result = result
                        .Where(c => c.CtlName == CtlName);
                }
            }

            var InfoList = result
               .OrderBy(Info => Info.CtlNo)
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
        public ActionResult CreateNote(IEnumerable<NoteView> notes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var note = new Note
                    {
                        CtlNo = notes.FirstOrDefault().CtlNo,
                        CtlName = notes.FirstOrDefault().CtlName
                    };
                    if (TryUpdateModel(note))
                    {
                        db.Note.Add(note);
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
        public ActionResult UpdateNote(IEnumerable<NoteView> notes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var note = new Note
                    {
                        CtlNo = notes.FirstOrDefault().CtlNo,
                        CtlName = notes.FirstOrDefault().CtlName
                    };
                    if (TryUpdateModel(note))
                    {
                        db.Entry(note).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
            };
            return Json(null);
        }
        //AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult DeleteNote(IEnumerable<NoteView> notes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Note note = db.Note.Find(notes.FirstOrDefault().CtlNo);
                    db.Note.Remove(note);
                    if (TryUpdateModel(note))
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
