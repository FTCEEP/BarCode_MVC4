using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;

using BarCode_MVC4.Models;
using BarCode_MVC4.Controllers;

using KendoGridBinder;
using Kendo.Mvc.UI;
using Kendo.DynamicLinq;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using NPOI;

namespace BarCode_MVC4.Controllers.Web
{
    public class BarCodeWebReceiptController : Controller
    {
        //
        // GET: /BarCodeWebReceipt/
        private DBOperation oDB = new DBOperation();
        private BarCodeEntities db = new BarCodeEntities();
        static List<String> liFilter;

        public ActionResult Index()
        {
            var Zone = db.Zone.OrderBy(i => i.ZoneID).ToList();
            ViewBag.ZoneList = new SelectList
                (
                    items: Zone,
                    dataTextField: "ZoneNo",
                    dataValueField: "ZoneID"
                );
            ViewBag.Account = Session["Account"];
            return View();
        }

        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {
            //2：入庫、4：盤盈匯入
            var result = db.Inventory_View.Select(i => i).Where(i => i.SrnmType == "2" || i.SrnmType == "4");
            if (!String.IsNullOrEmpty(filter))
            {
                liFilter = filter.TrimEnd(',').Split(',').ToList();
                if (liFilter[0].ToString() != " ")
                {
                    String Thickness = liFilter[0].ToString();
                    result = result
                        .Where(i => i.Thickness == Thickness);
                }
                if (liFilter[1].ToString() != " ")
                {
                    String Type = liFilter[1].ToString();
                    result = result
                        .Where(i => i.Type == Type);
                }
                if (liFilter[2].ToString() != " ")
                {
                    String Prod = liFilter[2].ToString();
                    result = result
                        .Where(i => i.Prod == Prod);
                }
                if (liFilter[3].ToString() != " ")
                {
                    String ZoneID = liFilter[3].ToString();
                    result = result
                        .Where(i => i.ZoneID == ZoneID);
                }
            }

            var InfoList = result
               .OrderBy(Info => Info.Prod)
               .Skip(skip)
               .Take(take);

            var InfoCount = result.Count();

            return Json(new { InfoList, InfoCount });
        }

        [HttpPost]
        public ActionResult CreateReceiptInfo(IEnumerable<InventoryView> receiptInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {             
                     //前端使用者輸入庫位名稱，透過此方法取得ZoneID
                    //String sZoneID = oDB.GetZoneID(receiptInfo.FirstOrDefault().ZoneNo);
                    //寫入庫存table
                    var Inventory = new Inventory
                    {
                        Type = receiptInfo.FirstOrDefault().Type,
                        Prod = receiptInfo.FirstOrDefault().Prod,
                        Thickness = receiptInfo.FirstOrDefault().Thickness,
                        Widt = receiptInfo.FirstOrDefault().Widt,
                        Leng = receiptInfo.FirstOrDefault().Leng,
                        Splice = receiptInfo.FirstOrDefault().Splice,
                        Ptno = receiptInfo.FirstOrDefault().Ptno,
                        ProductDate = Convert.ToDateTime(receiptInfo.FirstOrDefault().ProductDate),
                        CustomerNO = receiptInfo.FirstOrDefault().CustomerName,//前端客戶名稱欄位用來存放使用者輸入的客戶代碼
                        TransactionDate = Convert.ToDateTime(receiptInfo.FirstOrDefault().TransactionDate),
                        SrnmType = "2",
                        StaffID = receiptInfo.FirstOrDefault().StaffID,
                        NewWeight = receiptInfo.FirstOrDefault().NewWeight,
                        ZoneSN = oDB.MakeSN(),
                        ZoneID = oDB.GetZoneID(receiptInfo.FirstOrDefault().ZoneNo),
                        PackNo = receiptInfo.FirstOrDefault().PackNo,
                        Thickness2 = receiptInfo.FirstOrDefault().Thickness2,
                        CtlNo1 = receiptInfo.FirstOrDefault().CtlNo1,
                        CtlNo2 = receiptInfo.FirstOrDefault().CtlNo2,
                    };
                    if (TryUpdateModel(Inventory))
                    {
                        db.Inventory.Add(Inventory);
                        db.SaveChanges();
                    }
                    //將資料寫入歷史資料庫
                    //oDB.InsertHistoryInfo(Inventory, Session["Account"].ToString(), "2");
                    //將資料寫入歷史資料庫
                    var HistoryInfo = new HistoryInventory
                    {
                        Type = Inventory.Type,
                        Prod = Inventory.Prod,
                        Thickness = Inventory.Thickness,
                        Widt = Inventory.Widt,
                        Leng = Inventory.Leng,
                        Splice = Inventory.Splice,
                        Ptno = Inventory.Ptno,
                        ProductDate = Convert.ToDateTime(Inventory.ProductDate),
                        CustomerNO = Inventory.CustomerNO,
                        TransactionDate = Convert.ToDateTime(Inventory.TransactionDate),
                        SrnmType = "2",
                        StaffID = Inventory.StaffID,
                        NewWeight = Inventory.NewWeight,
                        ZoneSN = Inventory.ZoneSN,
                        ZoneID = Inventory.ZoneID,
                        PackNo = Inventory.PackNo,
                        Thickness2 = Inventory.Thickness2,
                        CtlNo1 = Inventory.CtlNo1,
                        CtlNo2 = Inventory.CtlNo2,
                    };
                    if (TryUpdateModel(HistoryInfo))
                    {
                        db.HistoryInventory.Add(HistoryInfo);
                        db.SaveChanges(); 
                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
            };
            return Json(null);
        }

        [HttpPost]
        public ActionResult UpdateReceiptInfo(IEnumerable<HistoryInventoryView> receiptInfo)
        {
            try
            {
                //先修改庫存資料
                if (ModelState.IsValid)
                {
                    var Inventory = new Inventory
                    {
                        Thickness = receiptInfo.FirstOrDefault().Thickness,
                        Widt = receiptInfo.FirstOrDefault().Widt,
                        Type = receiptInfo.FirstOrDefault().Type,
                        Prod = receiptInfo.FirstOrDefault().Prod,
                        Leng = receiptInfo.FirstOrDefault().Leng,
                        NewWeight = receiptInfo.FirstOrDefault().NewWeight,
                        ProductDate = receiptInfo.FirstOrDefault().ProductDate,
                        TransactionDate = receiptInfo.FirstOrDefault().TransactionDate,
                        CtlNo1 = receiptInfo.FirstOrDefault().CtlNo1,
                        CtlNo2 = receiptInfo.FirstOrDefault().CtlNo2,
                        //CtlName1 = receiptInfo.FirstOrDefault().CtlName1,
                        //CtlName2 = receiptInfo.FirstOrDefault().CtlName2
                    };
                    if (TryUpdateModel(Inventory))
                        {
                            db.Inventory.Attach(Inventory);
                            db.Entry(Inventory).Property(r => r.Thickness).IsModified = true;
                            db.Entry(Inventory).Property(r => r.Widt).IsModified = true;
                            db.Entry(Inventory).Property(r => r.Type).IsModified = true;
                            db.Entry(Inventory).Property(r => r.Leng).IsModified = true;
                            db.Entry(Inventory).Property(r => r.NewWeight).IsModified = true;
                            db.Entry(Inventory).Property(r => r.ProductDate).IsModified = true;
                            db.Entry(Inventory).Property(r => r.TransactionDate).IsModified = true;
                            db.Entry(Inventory).Property(r => r.CtlNo1).IsModified = true;
                            db.Entry(Inventory).Property(r => r.CtlNo2).IsModified = true;
                            //db.Entry(Inventory).Property(r => r.CtlName1).IsModified = true;
                            //db.Entry(Inventory).Property(r => r.CtlName2).IsModified = true;
                            db.SaveChanges();
                        }
                    //將異動資料寫入資料庫
                    oDB.InsertHistoryInfo(Inventory, Session["Account"].ToString(),"3");
                }
            }
            catch (Exception ex)
            {
            };
            return Json(null);
        }
        public ActionResult ExportExcel()
        {
            try
            {
                //建立workbook
                HSSFWorkbook workbook = new HSSFWorkbook();
                //建立串流
                MemoryStream ms = new MemoryStream();
                //建立Sheet
                HSSFSheet st = (NPOI.HSSF.UserModel.HSSFSheet)workbook.CreateSheet(DateTime.Now.ToString("yyyyMMdd") + "庫存報表");
                //先查詢要匯出的資料
                var result = db.Inventory_View.Select(i => i);
                if (liFilter != null)
                {
                    if (liFilter[0].ToString() != " ")
                    {
                        String Thickness = liFilter[0].ToString();
                        result = result
                            .Where(i => i.Thickness == Thickness);
                    }
                    if (liFilter[1].ToString() != " ")
                    {
                        String Type = liFilter[1].ToString();
                        result = result
                            .Where(i => i.Type == Type);
                    }
                    if (liFilter[2].ToString() != " ")
                    {
                        String Prod = liFilter[2].ToString();
                        result = result
                            .Where(i => i.Prod == Prod);
                    }
                    if (liFilter[3].ToString() != " ")
                    {
                        String ZoneID = liFilter[3].ToString();
                        result = result
                            .Where(i => i.ZoneID == ZoneID);
                    }
                }
                //先建立datatable存放資料
                DataTable dtt = new DataTable();
                dtt.Columns.Add(new DataColumn("Thickness", typeof(string)));
                dtt.Columns.Add(new DataColumn("Widt", typeof(string)));
                dtt.Columns.Add(new DataColumn("Type", typeof(string)));
                dtt.Columns.Add(new DataColumn("Prod", typeof(string)));
                dtt.Columns.Add(new DataColumn("Leng", typeof(string)));
                dtt.Columns.Add(new DataColumn("NewWeight", typeof(string)));
                dtt.Columns.Add(new DataColumn("StaffID", typeof(string)));
                dtt.Columns.Add(new DataColumn("ProductDate", typeof(string)));
                dtt.Columns.Add(new DataColumn("TransactionDate", typeof(string)));
                dtt.Columns.Add(new DataColumn("ZoneNo", typeof(string)));
                dtt.Columns.Add(new DataColumn("ZoneSN", typeof(string)));
                dtt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
                dtt.Columns.Add(new DataColumn("CtlNo1", typeof(string)));
                dtt.Columns.Add(new DataColumn("CtlName1", typeof(string)));
                dtt.Columns.Add(new DataColumn("CtlNo2", typeof(string)));
                dtt.Columns.Add(new DataColumn("CtlName2", typeof(string)));
                foreach (Inventory_View oInfo in result)
                {
                    DataRow row = dtt.NewRow();
                    row["Thickness"] = oInfo.Thickness;
                    row["Widt"] = oInfo.Widt;
                    row["Type"] = oInfo.Type;
                    row["Prod"] = oInfo.Prod;
                    row["Leng"] = oInfo.Leng;
                    row["NewWeight"] = oInfo.NewWeight;
                    row["StaffID"] = oInfo.StaffID;
                    row["ProductDate"] = string.Format("{0:d}", oInfo.ProductDate);
                    row["TransactionDate"] = string.Format("{0:d}", oInfo.TransactionDate);
                    row["ZoneNo"] = oInfo.ZoneNo;
                    row["ZoneSN"] = oInfo.ZoneSN;
                    row["CustomerName"] = oInfo.CustomerName;
                    row["CtlNo1"] = oInfo.CtlNo1;
                    row["CtlName1"] = oInfo.CtlName1;
                    row["CtlNo2"] = oInfo.CtlNo2;
                    row["CtlName2"] = oInfo.CtlName2;
                    dtt.Rows.Add(row);
                }
                //取得欄位的總數
                int icolCount = dtt.Columns.Count;

                //建立excel的title
                HSSFRow title = (NPOI.HSSF.UserModel.HSSFRow)st.CreateRow(0);
                //找出DeviceInventory的所有欄位(屬性)
                for (int i = 0; i < icolCount; i++)
                {
                    st.SetColumnWidth(i, 20 * 150);

                    switch (dtt.Columns[i].ColumnName)
                    {
                        case "Thickness":
                            //基重
                            title.CreateCell(i).SetCellValue("基重");
                            break;
                        case "Widt":
                            //幅寬
                            title.CreateCell(i).SetCellValue("幅寬");
                            break;
                        case "Type":
                            //種類
                            title.CreateCell(i).SetCellValue("種類");
                            break;
                        case "Prod":
                            //支號
                            title.CreateCell(i).SetCellValue("支號");
                            break;
                        case "Leng":
                            //長度
                            title.CreateCell(i).SetCellValue("長度");
                            break;
                        case "NewWeight":
                            //淨重
                            title.CreateCell(i).SetCellValue("淨重");
                            break;
                        case "StaffID":
                            //盤點人
                            title.CreateCell(i).SetCellValue("盤點人");
                            break;
                        case "ProductDate":
                            //生產日期
                            title.CreateCell(i).SetCellValue("生產日期");
                            break;
                        case "TransactionDate":
                            //盤點日期
                            title.CreateCell(i).SetCellValue("盤點日期");
                            break;
                        case "ZoneNo":
                            //庫位
                            title.CreateCell(i).SetCellValue("庫位");
                            break;
                        case "ZoneSN":
                            //庫位流水號
                            title.CreateCell(i).SetCellValue("庫位流水號");
                            break;
                        case "CustomerName":
                            //客戶名稱
                            title.CreateCell(i).SetCellValue("客戶名稱");
                            break;
                        case "CtlNo1":
                            //管制1
                            title.CreateCell(i).SetCellValue("管制1");
                            break;
                        case "CtlName1":
                            //管制(1)
                            title.CreateCell(i).SetCellValue("管制(1)");
                            break;
                        case "CtlNo2":
                            //管制2
                            title.CreateCell(i).SetCellValue("管制2");
                            break;
                        case "CtlName2":
                            //管制(2)
                            title.CreateCell(i).SetCellValue("管制(2)");
                            break;
                    }
                }
                int iCount = 1;
                foreach (DataRow dr in dtt.Rows)
                {
                    //建立excel的dataRow
                    HSSFRow dataRow = (NPOI.HSSF.UserModel.HSSFRow)st.CreateRow(iCount);
                    for (int i = 0; i < icolCount; i++)
                    {
                        dataRow.CreateCell(i).SetCellValue(dr[i].ToString());
                    }
                    iCount++;
                }
                //建立excel的總計欄位
                HSSFRow total = (NPOI.HSSF.UserModel.HSSFRow)st.CreateRow(iCount + 1);
                total.CreateCell(0).SetCellValue("庫存總計" + dtt.Rows.Count + "筆");
                workbook.Write(ms);
                workbook = null;

                return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + "庫存報表.xls");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
