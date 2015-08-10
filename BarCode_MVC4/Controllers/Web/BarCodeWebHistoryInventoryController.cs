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


namespace BarCode_MVC4.Controllers.Web
{
    public class BarCodeWebHistoryInventoryController : Controller
    {
        //
        // GET: /BarCodeWebInventory/
        private BarCodeEntities db = new BarCodeEntities();
        static List<String> liFilter;
        public ActionResult Index()
        {
            var Srnm = db.Srnm.OrderBy(i => i.SrnmType).ToList();
            ViewBag.SrnmList = new SelectList
                (
                    items:Srnm,
                    dataTextField: "SrnmName",
                    dataValueField: "SrnmType"
                );
            return View();
        }

        public JsonResult Read(int take, int skip, string filter) //int page, int pageSize,
        {
            var result = db.HistoryInventory_View.Select(i => i);
            if (!String.IsNullOrEmpty(filter))
            {
                liFilter = filter.TrimEnd(',').Split(',').ToList();
                if (liFilter[0].ToString() != " ")
                {
                    int Year = int.Parse(liFilter[0]);
                    result = result
                        .Where(i => i.TransactionDate.Value.Year == Year);
                }
                if (liFilter[1].ToString() != " ")
                {
                    int Month = int.Parse(liFilter[1]);
                    result = result
                        .Where(i => i.TransactionDate.Value.Month == Month);
                }
                if (liFilter[2].ToString() != " ")
                {
                    int Day = int.Parse(liFilter[2]);
                    result = result
                        .Where(i => i.TransactionDate.Value.Day == Day);
                }
                if (liFilter[3].ToString() != " ")
                {
                    String SrnmType = liFilter[3].ToString();
                    result = result
                        .Where(i => i.SrnmType == SrnmType);
                }
            }

            var InfoList = result
               .OrderBy(Info => Info.Prod)
               .Skip(skip)
               .Take(take);

            var InfoCount = result.Count();

            return Json(new { InfoList, InfoCount });
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
                HSSFSheet st = (NPOI.HSSF.UserModel.HSSFSheet)workbook.CreateSheet(DateTime.Now.ToString("yyyyMMdd") + "庫存異動報表");
                //先查詢要匯出的資料
                var result = db.HistoryInventory_View.Select(i => i);
                if (liFilter != null)
                {
                    if (liFilter[0].ToString() != " ")
                    {
                        int Year = int.Parse(liFilter[0]);
                        result = result
                            .Where(i => i.TransactionDate.Value.Year == Year);
                    }
                    if (liFilter[1].ToString() != " ")
                    {
                        int Month = int.Parse(liFilter[1]);
                        result = result
                            .Where(i => i.TransactionDate.Value.Month == Month);
                    }
                    if (liFilter[2].ToString() != " ")
                    {
                        int Day = int.Parse(liFilter[2]);
                        result = result
                            .Where(i => i.TransactionDate.Value.Day == Day);
                    }
                    if (liFilter[3].ToString() != " ")
                    {
                        String SrnmType = liFilter[3].ToString();
                        result = result
                            .Where(i => i.SrnmType == SrnmType);
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
                dtt.Columns.Add(new DataColumn("SrnmName", typeof(string)));
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
                foreach (HistoryInventory_View oInfo in result)
                {
                    DataRow row = dtt.NewRow();
                    row["Thickness"] = oInfo.Thickness;
                    row["Widt"] = oInfo.Widt;
                    row["Type"] = oInfo.Type;
                    row["Prod"] = oInfo.Prod;
                    row["Leng"] = oInfo.Leng;
                    row["NewWeight"] = oInfo.NewWeight;
                    row["SrnmName"] = oInfo.SrnmName;
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
                        case "SrnmName":
                            //作業別
                            title.CreateCell(i).SetCellValue("作業別");
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
                            title.CreateCell(i).SetCellValue("異動日期");
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
                ////建立excel的總計欄位
                //HSSFRow total = (NPOI.HSSF.UserModel.HSSFRow)st.CreateRow(iCount + 1);
                //total.CreateCell(0).SetCellValue("庫存總計" + dtt.Rows.Count + "筆");
                workbook.Write(ms);
                workbook = null;

                return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + "庫存異動報表.xls");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
