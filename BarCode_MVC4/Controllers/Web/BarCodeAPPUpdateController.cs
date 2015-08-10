using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BarCode_MVC4.Models;
using System.Data.Entity;

namespace BarCode_MVC4.Controllers.Web
{
    public class BarCodeAPPUpdateController : Controller
    {
        //web.config的連線字串
        private BarCodeEntities db = new BarCodeEntities();
        
        public ActionResult Index()
        {
            IEnumerable<APKFile> result = from Info in db.APKFile 
                                          select Info;

            return View(result);
        }

        public ActionResult GetFilePath(string FileID)
        {
            string result = (from Info in db.APKFile
                             where Info.FileID.Equals(FileID)
                             select Info.FilePath).FirstOrDefault();

            if (!string.IsNullOrEmpty(result) && System.IO.File.Exists(Server.MapPath(result)))
                return File(System.IO.File.ReadAllBytes(Server.MapPath(result)),
                 "application/unknown",
                 HttpUtility.UrlEncode(System.IO.Path.GetFileName(result)));
            else
                return View();
        }

    }
}
