using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BarCode_MVC4.Models;
using BarCode_MVC4.Controllers;

namespace BarCode_MVC4.Controllers.Web
{
    public class BarCodeWebClearController : Controller
    {
        //
        // GET: /BarCodeWebClear/
        private DBOperation oDB = new DBOperation();
        private BarCodeEntities db = new BarCodeEntities();
        //static List<String> liFilter;
        public ActionResult Index()
        {         
            return View();
        }
        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {           
            var TempResult = db.TempInventory.Select(s => s).ToList();

            //判斷TempInventory是否有資料，TempInventory是用來存盤點的資料
            if (TempResult.Count != 0)
            {
                //只撈取庫存table有資料，但實際盤點不到的銅箔資料
                var result = db.Inventory_View.Where(i => !db.TempInventory.Any(t => t.Prod == i.Prod)).ToList();
                //將資料寫入clearinventory
                foreach (var Info in result) 
                {
                    var ClearInfo = new ClearInventory
                    {
                        Type = Info.Type,
                        Prod = Info.Prod,
                        Thickness = Info.Thickness,
                        Widt = Info.Widt,
                        Leng = Info.Leng,
                        Splice = Info.Splice,
                        Ptno = Info.Ptno,
                        ProductDate = Convert.ToDateTime(Info.ProductDate),
                        CustomerNO = Info.CustomerNO,
                        StaffID = Session["Account"].ToString(),
                        TransactionDate = Convert.ToDateTime(Info.TransactionDate),
                        SrnmType = "5",
                        NewWeight = Info.NewWeight,
                        ZoneSN = Info.ZoneSN,
                        ZoneID = Info.ZoneID,
                        PackNo = Info.PackNo,
                        Thickness2 = Info.Thickness2
                    };
                    db.ClearInventory.Add(ClearInfo);
                    db.SaveChanges(); 
                }
                //刪除tempinventory的所有資料
                db.TempInventory.RemoveRange(TempResult);
                db.SaveChanges();
                //將盤虧資料丟到前端的gridview
                var InfoList = db.ClearInventory_View
                   .OrderBy(Info => Info.Prod)
                   .Skip(skip)
                   .Take(take);

                var InfoCount = InfoList.Count();
                return Json(new { InfoList, InfoCount });
            }
            else 
            {
                //將盤虧資料丟到前端的gridview
                var InfoList = db.ClearInventory_View
                   .OrderBy(Info => Info.Prod)
                   .Skip(skip)
                   .Take(take);

                var InfoCount = InfoList.Count();
                return Json(new { InfoList, InfoCount });
            }
        }
        public JsonResult DeleteInfo(IEnumerable<ClearInventoryView> ClearInfo) 
        {
            try
            { //需要盤點程序結束的狀態且是ClearInventory有資料的情況下，才能進行盈虧刪除
                if (db.SysPara.Where(r => r.ParaName == "Inventory").FirstOrDefault().ParaValue.Equals("false") && db.ClearInventory.Count() > 0) 
                {
                    Inventory Info = db.Inventory.Find(ClearInfo.FirstOrDefault().Prod);
                    ClearInventory Clear_Info = db.ClearInventory.Find(ClearInfo.FirstOrDefault().Prod);
                    if (ModelState.IsValid)
                    {
                        //盤虧刪除
                        //先刪除庫存資料
                        db.Inventory.Remove(Info);
                        //再刪除ClearInventory的資料
                        db.ClearInventory.Remove(Clear_Info);
                        if (TryUpdateModel(Info))
                        {
                            db.SaveChanges();
                        }
                    }
                    //將異動資料寫入歷史記錄資料表
                    var HistoryInfo = new HistoryInventory
                    {
                        Type = Info.Type,
                        Prod = Info.Prod,
                        Thickness = Info.Thickness,
                        Widt = Info.Widt,
                        Leng = Info.Leng,
                        Splice = Info.Splice,
                        Ptno = Info.Ptno,
                        ProductDate = Convert.ToDateTime(Info.ProductDate),
                        CustomerNO = Info.CustomerNO,
                        StaffID = Session["Account"].ToString(),
                        TransactionDate = Convert.ToDateTime(Info.TransactionDate),
                        SrnmType = "5",
                        NewWeight = Info.NewWeight,
                        ZoneSN = Info.ZoneSN,
                        ZoneID = Info.ZoneID,
                        PackNo = Info.PackNo,
                        Thickness2 = Info.Thickness2
                    };
                    db.HistoryInventory.Add(HistoryInfo);
                    db.SaveChanges(); 
                }

                //當ClearInventory與ImportInventory資料都為0，
                //表示此盤點程序與庫存資料已經核對完畢，則將db的參數改為true
                if (db.ClearInventory.Count() == 0 && db.ImportInventory.Count() == 0) 
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var SysPara = db.SysPara.Where(r => r.ParaName == "Inventory");
                            if (SysPara != null)
                            {
                                SysPara.FirstOrDefault().ParaValue = "true";
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    };           
                }
            }
            catch(Exception ex)
            {
            
            }
            return Json(null);
        }
    }
}
