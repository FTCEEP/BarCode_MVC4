using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Net;
using System.Web.Security;
using System.Text.RegularExpressions;

using Microsoft.VisualBasic;

using BarCode_MVC4.Models;
using System.Data.SqlClient;

namespace BarCode_MVC4.Models
{
    public class DBOperation
    {
        //宣告資料庫實體模型物件
        private BarCodeEntities db = new BarCodeEntities();
        ////宣告一個DBOperation類別物件
        //DBOperation oDB = new DBOperation();
        //登入帳密確認
        public string LoginCheck(string Account, string Password)
        {
            //從資料庫中取得傳入帳號的使用者資料
            Staff UserInfo = db.Staff.Find(Account);
            //判斷是否有此帳號
            if (UserInfo != null)
            {
                //密碼檢查方法
                if (PasswordCheck(UserInfo, Password))
                {
                    return "";
                }
                else
                {
                    return "密碼輸入錯誤。";
                }
            }
            else
            {
                return "無此使用者。";
            }
        }
        //密碼檢查
        public bool PasswordCheck(Staff UserInfo, string Password)
        {
            //判斷使用者輸入的密碼是否與資料庫的密碼一致
            bool bResult = UserInfo.Password.Equals(Password);
            return bResult;
        }
        //取Role的資料
        public String RoleInfo(string Account) 
        {
            String sResult = "";
            var result = db.Staff.Where(s => s.Account == Account);
            sResult = result.FirstOrDefault().Role;
            return sResult;
        }
        public bool BarCodeStaffCheck(String sStaff) 
        {
            bool bResult = false;
            if (db.Staff.Find(sStaff) != null)
            {
                bResult = true;
            }
            else
            {
                bResult = false; 
            }
            return bResult;
        }
        public bool BarCodeZoneCheck(String sZone)
        {
            bool bResult = false;
            if (db.Zone.Where(z => z.ZoneNo == sZone).Count() > 0)
            {
                bResult = true;
            }
            else
            {
                bResult = false;
            }
            return bResult;
        }
        public bool BarCodeGoodsCheck(ref List<String> InfoList, String sCopper)
        {
            //sCopper:1表示二維，2表示掃一維捲裝，3表示掃一維片裝，4松電工條碼，5日系一般條碼
            //二維銅箔條碼格式驗證
            bool bResult= false;
            if (sCopper == "1") 
            {
                for (int i = 0; i < 12; i++) 
                {
                    switch (i)
                    {
                        case 0:
                            bResult = (InfoList[0].Length <= 9) ? true : false;
                            break;
                        case 1:
                            bResult = (InfoList[1].Length <= 3) ? true : false;
                            break;
                        case 2:
                            bResult = InfoList[2].Length.Equals(1);
                            break;
                        case 3:
                            bResult = (InfoList[3].Length <= 4) ? true : false;
                            break;
                        case 4:
                            bResult = (InfoList[4].Length <= 4) ? true : false;
                            break;
                        case 5:
                            bResult = (InfoList[5].Length <= 5) ? true : false;
                            break;
                        case 6:
                            bResult = InfoList[6].Length.Equals(1);
                            break;
                        case 7:
                            bResult = (InfoList[7].Length <= 8) ? true : false;
                            break;
                        case 8:
                            bResult = (InfoList[8].Length <= 5) ? true : false;
                            break;
                        case 9:
                            bResult = (InfoList[9].Length <= 4) ? true : false;
                            break;
                        case 10:
                            bResult = (InfoList[10].Length <= 6) ? true : false;
                            break;
                        case 11:
                            bResult = (InfoList[11].Length <= 15) ? true : false;
                            break;
                    }
                    if (bResult == false) 
                    {
                        break;
                    }
                }
            }
            //一維銅箔條碼格式驗證
            //一維捲裝銅箔
            else if (sCopper == "2") 
            {
               
                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0:
                            bResult = InfoList[0].Length.Equals(8);
                            break;
                        case 1:
                            bResult = InfoList[1].Length.Equals(1);
                            break;
                        case 2:
                            bResult = InfoList[2].Length.Equals(4);
                            break;
                        case 3:
                            bResult = InfoList[3].Length.Equals(4);
                            break;
                        case 4:
                            bResult = InfoList[4].Length.Equals(5);
                            break;
                        //捲裝是1碼，片裝是4碼
                        case 5:
                            bResult = InfoList[5].Length.Equals(1);
                            break;
                        case 6:
                            bResult = InfoList[6].Length.Equals(6);
                            break;
                        case 7:
                            bResult = InfoList[7].Length.Equals(2);
                            break;
                        //產品種類不固定
                        //case 8:
                        //    bResult = InfoList[8].Length.Equals(0);
                        //    break;
                    }
                    if (bResult == false)
                    {
                        break;
                    }
                }               
            }
            //一維片裝銅箔
            else if (sCopper == "3")
            {
                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0:
                            bResult = InfoList[0].Length.Equals(8);
                            break;
                        case 1:
                            bResult = InfoList[1].Length.Equals(1);
                            break;
                        case 2:
                            bResult = InfoList[2].Length.Equals(4);
                            break;
                        case 3:
                            bResult = InfoList[3].Length.Equals(4);
                            break;
                        case 4:
                            bResult = InfoList[4].Length.Equals(5);
                            break;
                        case 5:
                            bResult = InfoList[5].Length.Equals(4);
                            break;
                        case 6:
                            bResult = InfoList[6].Length.Equals(6);
                            break;
                        case 7:
                            bResult = InfoList[7].Length.Equals(2);
                            break;
                        //產品種類不固定
                        //case 8:
                        //    bResult = InfoList[8].Length.Equals(0);
                        //    break;
                    }
                    if (bResult == false)
                    {
                        break;
                    }
                }
            }
            //松電工
            else if (sCopper == "4") 
            {
                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0:
                            bResult = (InfoList[0].Length <= 15) ? true : false;
                            break;
                        case 1:
                            bResult = (InfoList[1].Length <= 5) ? true : false;
                            break;
                        case 2:
                            bResult = (InfoList[2].Length <= 5) ? true : false;
                            break;
                        case 3:
                            bResult = (InfoList[3].Length <= 9) ? true : false;
                            break;
                        case 4:
                            bResult = (InfoList[4].Length <= 3) ? true : false;
                            break;
                        case 5:
                            bResult = InfoList[5].Length.Equals(1);
                            break;
                        case 6:
                            bResult = (InfoList[6].Length <= 4) ? true : false;
                            break;
                        case 7:
                            bResult = (InfoList[7].Length <= 4) ? true : false;
                            break;
                        case 8:
                            bResult = InfoList[8].Length.Equals(1);
                            break;
                        case 9:
                            bResult = (InfoList[9].Length <= 8) ? true : false;
                            break;
                        case 10:
                            bResult = (InfoList[10].Length <= 4) ? true : false;
                            break;
                        case 11:
                            bResult = (InfoList[11].Length <= 6) ? true : false;
                            break;
                    }
                    if (bResult == false)
                    {
                        break;
                    }
                }
            }
            //日系一般條碼
            else if (sCopper == "5") 
            {
                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0:
                            bResult = (InfoList[0].Length <= 8) ? true : false;
                            break;
                        case 1:
                            bResult = (InfoList[1].Length <= 3) ? true : false;
                            break;
                        case 2:
                            bResult = (InfoList[2].Length <= 4) ? true : false;
                            break;
                        case 3:
                            bResult = (InfoList[3].Length <= 4) ? true : false;
                            break;
                        case 4:
                            bResult = (InfoList[4].Length <= 4) ? true : false;
                            break;
                        case 5:
                            bResult = (InfoList[5].Length <= 6) ? true : false;
                            break;
                        case 6:
                            bResult = (InfoList[6].Length <= 5) ? true : false;
                            break;
                        case 7:
                            bResult = (InfoList[7].Length <= 10) ? true : false;
                            break;
                        case 8:
                            bResult = (InfoList[8].Length <= 2) ? true : false;
                            break;
                    }
                    if (bResult == false)
                    {
                        break;
                    }
                }
            }
            return bResult;
        }
        //入庫
        public String ReceiptIntoDB(String sStaffID, String sZoneNo, String sZoneSN, String sFuncCode, List<String> InfoList, String sCopper, String sCustomer, String sCtlNo1, String sCtlNo2) 
        {
            String sResult = "";
            //取得ZoneID
            String sZoneID = GetZoneID(sZoneNo);
            //取得松電工的支號
            String sProd = (sCopper.Equals("4")) ? InfoList[3].ToString() : InfoList[0].ToString();
            var Info = new Inventory();
            try
            {
                if (db.Inventory.Where(i => i.Prod == sProd).Count() > 0)
                {
                    sResult = sProd + "，此筆資料，已入庫!!";
                }
                else 
                {
                    if (InfoList.Count == 12 && sCopper.Equals("1"))
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[7].Substring(0, 4)),
                                                  int.Parse(InfoList[7].Substring(4, 2)),
                                                  int.Parse(InfoList[7].Substring(6, 2)));
                        Info = new Inventory
                        {
                            Thickness = InfoList[2],
                            Widt = InfoList[3],
                            Type = InfoList[10],
                            Prod = InfoList[0],
                            Leng = int.Parse((InfoList[4])),
                            NewWeight = float.Parse(InfoList[5]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            Splice = InfoList[6],
                            Ptno = InfoList[11],
                            CustomerNO = sCustomer,
                            PackNo = InfoList[9],
                            Thickness2 = InfoList[1],
                            CtlNo1 = sCtlNo1,
                            CtlNo2 = sCtlNo2
                        };
                    }
                    
                    //松電工
                    else if (InfoList.Count == 12 && sCopper.Equals("4")) 
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[9].Substring(0, 4)),
                                                  int.Parse(InfoList[9].Substring(4, 2)),
                                                  int.Parse(InfoList[9].Substring(6, 2)));
                        Info = new Inventory
                        {
                            Thickness = InfoList[5],
                            Widt = InfoList[6],
                            Type = InfoList[11],
                            Prod = InfoList[3],
                            Leng = int.Parse((InfoList[7])),
                            NewWeight = float.Parse(InfoList[2]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            Splice = InfoList[8],
                            Ptno = InfoList[0],
                            CustomerNO = sCustomer,
                            PackNo = InfoList[10],
                            Thickness2 = InfoList[4],
                            CtlNo1 = sCtlNo1,
                            CtlNo2 = sCtlNo2
                        };
                    }
                    //日系一般
                    else if (InfoList.Count == 9 && sCopper.Equals("5"))
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[5].Substring(0, 2)) + 2000,
                                                  int.Parse(InfoList[5].Substring(2, 2)),
                                                  int.Parse(InfoList[5].Substring(4, 2)));
                        Info = new Inventory
                        {
                            //Thickness = InfoList[1],日系一般沒有此項資料
                            Widt = InfoList[2],
                            Type = InfoList[6],
                            Prod = InfoList[0],
                            Leng = int.Parse((InfoList[3])),
                            NewWeight = float.Parse(InfoList[4]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            //Splice = InfoList[5],沒有接頭數
                            CustomerNO = sCustomer,
                            CtlNo1 = sCtlNo1,
                            CtlNo2 = sCtlNo2,
                            Thickness2 = InfoList[1],
                            Ptno = InfoList[7]
                            //特殊記項為，InfoList[8]，尚無用到
                        };
                    }
                    else if (InfoList.Count == 9)
                    //else
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[6].Substring(0, 2)) + 2000,
                                                  int.Parse(InfoList[6].Substring(2, 2)),
                                                  int.Parse(InfoList[6].Substring(4, 2)));
                        Info = new Inventory
                        {
                            //Thickness = InfoList[2],
                            //Widt = InfoList[3],
                            //Type = InfoList[9],
                            //Prod = InfoList[1],
                            //Leng = int.Parse((InfoList[4])),
                            //NewWeight = float.Parse(InfoList[5]),
                            //SrnmType = sFuncCode,
                            //ProductDate = dt,
                            //TransactionDate = DateTime.Now,
                            //StaffID = sStaffID,
                            //ZoneID = sZoneID,
                            //ZoneSN = sZoneSN,
                            //Splice = InfoList[6],
                            ////Ptno = InfoList[11]

                            Thickness = InfoList[1],
                            Widt = InfoList[2],
                            Type = InfoList[8],
                            Prod = InfoList[0],
                            Leng = int.Parse((InfoList[3])),
                            NewWeight = float.Parse(InfoList[4]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            Splice = InfoList[5],
                            CustomerNO = sCustomer,
                            CtlNo1 = sCtlNo1,
                            CtlNo2 = sCtlNo2
                            //Ptno = InfoList[11]
                        };
                    }
                    db.Inventory.Add(Info);
                    db.SaveChanges();
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
                        StaffID = sStaffID,
                        TransactionDate = Convert.ToDateTime(Info.TransactionDate),
                        SrnmType = sFuncCode,
                        NewWeight = Info.NewWeight,
                        ZoneSN = Info.ZoneSN,
                        ZoneID = Info.ZoneID,
                        PackNo = Info.PackNo,
                        Thickness2 = Info.Thickness2
                    };
                    db.HistoryInventory.Add(HistoryInfo);
                    db.SaveChanges();
                    sResult = "true";
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                sResult = ex.ToString();
            };
            return sResult;
        }
        //出庫
        public String IssuanceIntoDB(String sStaffID, String sProd, String sFuncCode)
        {
            String sResult = "";
            try
            {
                //Inventory Info = db.Inventory.Find(ClearInfo.FirstOrDefault().Prod);

                if (db.Inventory.Where(i => i.Prod == sProd).Count() > 0)
                {
                    var Info = db.Inventory.Where(i => i.Prod == sProd).FirstOrDefault();
                    //先刪除庫存資料
                    db.Inventory.Remove(Info);
                    db.SaveChanges();
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
                        StaffID = sStaffID,
                        TransactionDate = Convert.ToDateTime(Info.TransactionDate),
                        SrnmType = sFuncCode,
                        NewWeight = Info.NewWeight,
                        ZoneSN = Info.ZoneSN,
                        ZoneID = Info.ZoneID,
                        PackNo = Info.PackNo,
                        Thickness2 = Info.Thickness2
                    };
                    db.HistoryInventory.Add(HistoryInfo);
                    db.SaveChanges(); 
                    sResult = "true";
                }
                else
                {
                    sResult = "無"+ sProd +"此筆資料，此筆資料可能已出庫!!";
                }
            }
            catch (Exception ex)
            {
                sResult = ex.ToString();
            }
            return sResult;
        }
        
        //盤點
        public String DeviceInventoryIntoDB(String sStaffID, String sZoneNo, String sZoneSN, String sFuncCode, List<String> InfoList, String sCopper)
        {
            //取得松電工的支號
            String sProd = (sCopper.Equals("4")) ? InfoList[3].ToString() : InfoList[0].ToString();
            ////二維條碼支號為InfoList[0]，一維條碼支號為InfoList[1]
            //String sProd = (InfoList.Count == 12) ? InfoList[0].ToString() : InfoList[1].ToString();
           
            String sResult = "";
            try
            {
                //表示盤點資料與庫存資料相符
                if (db.Inventory.Where(i => i.Prod == sProd).Count() > 0)
                {
                    //如果地點不同，代表地點條碼掃瞄有誤
                    //if (db.Inventory.Where(i => i.Prod == sProd && i.ZoneID == sZoneID).Count() == 0)
                    //{
                    //    sResult = "此銅箔的庫位資料比對不符!!";
                    //}
                    //else 
                    //{
                    //取得ZoneID
                    //String sZoneID;
                    var data = db.Inventory.Where(i => i.Prod == sProd).FirstOrDefault();
                    
                        sResult = sProd + "，庫存與實際盤點資料相符!!";
                        //將盤點資料，寫入tempinventory資料表
                        //且要此筆資料尚未盤點過，表示尚未寫入tempinventory
                        if (db.TempInventory.Where(i => i.Prod == sProd).Count() == 0)
                        {
                            InsertTempInventory(InfoList, data.ZoneID, sStaffID, sFuncCode, sZoneSN, sCopper);
                        }
                    //}
                }
                else 
                {
                    //取得ZoneID
                    //String sZoneID = GetZoneID(sZoneNo);
                    var Info = new ImportInventory();
                    //二維條碼
                    if (InfoList.Count == 12 && sCopper.Equals("1"))
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[7].Substring(0, 4)),
                                                  int.Parse(InfoList[7].Substring(4, 2)),
                                                  int.Parse(InfoList[7].Substring(6, 2)));
                        Info = new ImportInventory
                        {
                            Thickness = InfoList[2],
                            Widt = InfoList[3],
                            Type = InfoList[10],
                            Prod = InfoList[0],
                            Leng = int.Parse((InfoList[4])),
                            NewWeight = float.Parse(InfoList[5]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            //ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            Splice = InfoList[6],
                            Ptno = InfoList[11],
                            //CustomerNO = "b",
                            PackNo = InfoList[9],
                            Thickness2 = InfoList[1]
                        };
                        //將盤點資料寫入tempinventory資料表
                        //且要此筆資料尚未盤點過
                        if (db.TempInventory.Where(i => i.Prod == sProd).Count() == 0)
                        {
                            InsertTempInventory(InfoList, "", sStaffID, sFuncCode, sZoneSN, sCopper);
                        }
                    }
                    //松電工
                    else if (InfoList.Count == 12 && sCopper.Equals("4"))
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[9].Substring(0, 4)),
                                                  int.Parse(InfoList[9].Substring(4, 2)),
                                                  int.Parse(InfoList[9].Substring(6, 2)));
                        Info = new ImportInventory
                        {
                            Thickness = InfoList[5],
                            Widt = InfoList[6],
                            Type = InfoList[11],
                            Prod = InfoList[3],
                            Leng = int.Parse((InfoList[7])),
                            NewWeight = float.Parse(InfoList[2]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            //ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            Splice = InfoList[8],
                            Ptno = InfoList[0],
                            //CustomerNO = "b",
                            PackNo = InfoList[10],
                            Thickness2 = InfoList[4]
                        };
                        //將盤點資料寫入tempinventory資料表
                        //且要此筆資料尚未盤點過
                        if (db.TempInventory.Where(i => i.Prod == sProd).Count() == 0)
                        {
                            InsertTempInventory(InfoList, "", sStaffID, sFuncCode, sZoneSN, sCopper);
                        }
                    }
                    //日系一般
                    else if (InfoList.Count == 9 && sCopper.Equals("5"))
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[5].Substring(0, 2)) + 2000,
                                                  int.Parse(InfoList[5].Substring(2, 2)),
                                                  int.Parse(InfoList[5].Substring(4, 2)));
                        Info = new ImportInventory
                        {
                            //Thickness = InfoList[1],日系一般沒有此項資料
                            Widt = InfoList[2],
                            Type = InfoList[6],
                            Prod = InfoList[0],
                            Leng = int.Parse((InfoList[3])),
                            NewWeight = float.Parse(InfoList[4]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            //ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            //Splice = InfoList[5],沒有接頭數
                            //CustomerNO = sCustomer,
                            //CtlNo1 = sCtlNo1,
                            //CtlNo2 = sCtlNo2,
                            Thickness2 = InfoList[1],
                            Ptno = InfoList[7]
                            //特殊記項為，InfoList[8]，尚無用到
                        };
                        //將盤點資料寫入tempinventory資料表
                        //且要此筆資料尚未盤點過
                        if (db.TempInventory.Where(i => i.Prod == sProd).Count() == 0)
                        {
                            InsertTempInventory(InfoList, "", sStaffID, sFuncCode, sZoneSN, sCopper);
                        }
                    }
                    //一維條碼
                    else if (InfoList.Count == 9)
                    //else
                    {
                        //設定日期格式
                        DateTime dt = SetDateTime(int.Parse(InfoList[6].Substring(0, 2)) + 2000,
                                                  int.Parse(InfoList[6].Substring(2, 2)),
                                                  int.Parse(InfoList[6].Substring(4, 2)));
                        Info = new ImportInventory
                        {
                            //Thickness = InfoList[2],
                            //Widt = InfoList[3],
                            //Type = InfoList[9],
                            //Prod = InfoList[1],
                            //Leng = int.Parse((InfoList[4])),
                            //NewWeight = float.Parse(InfoList[5]),
                            //SrnmType = sFuncCode,
                            //ProductDate = dt,
                            //TransactionDate = DateTime.Now,
                            //StaffID = sStaffID,
                            //ZoneID = sZoneID,
                            //ZoneSN = sZoneSN,
                            //Splice = InfoList[6],
                            ////Ptno = InfoList[11]

                            Thickness = InfoList[1],
                            Widt = InfoList[2],
                            Type = InfoList[8],
                            Prod = InfoList[0],
                            Leng = int.Parse((InfoList[3])),
                            NewWeight = float.Parse(InfoList[4]),
                            SrnmType = sFuncCode,
                            ProductDate = dt,
                            TransactionDate = DateTime.Now,
                            StaffID = sStaffID,
                            //ZoneID = sZoneID,
                            ZoneSN = sZoneSN,
                            Splice = InfoList[5],
                            //CustomerNO = "b"
                            //Ptno = InfoList[11]
                        };
                        //將盤點資料寫入tempinventory資料表
                        //且要此筆資料尚未盤點過
                        if (db.TempInventory.Where(i => i.Prod == sProd).Count() == 0)
                        {
                            InsertTempInventory(InfoList, "", sStaffID, sFuncCode, sZoneSN, sCopper);
                        }
                    }
                    //將差異資料寫入ImportInventory資料表
                    //且要此筆資料尚未盤點過
                    if (db.ImportInventory.Where(i => i.Prod == sProd).Count() == 0)
                    {
                        db.ImportInventory.Add(Info);
                        db.SaveChanges();
                        sResult = "true";
                    }
                    else 
                    {
                        sResult = sProd + "，已盤點過，請將此筆資料匯入庫存資料庫!!";
                    }
                    
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex) 
            {
                sResult = ex.ToString();
            }
            return sResult;
        }
        //將盤點資料寫入TempInventory資料表
        public void InsertTempInventory(List<String> InfoList, String sZoneID, String sStaffID, String sFuncCode, String sZoneSN, String sCopper)
        {
            //取得ZoneID
            //String sZoneID = GetZoneID(sZoneNo);
            var Info = new TempInventory();
            //二維條碼
            if (InfoList.Count == 12 && sCopper.Equals("1"))
            {
                //設定日期格式
                DateTime dt = SetDateTime(int.Parse(InfoList[7].Substring(0, 4)),
                                          int.Parse(InfoList[7].Substring(4, 2)),
                                          int.Parse(InfoList[7].Substring(6, 2)));
                Info = new TempInventory
                {
                    Thickness = InfoList[2],
                    Widt = InfoList[3],
                    Type = InfoList[10],
                    Prod = InfoList[0],
                    Leng = int.Parse((InfoList[4])),
                    NewWeight = float.Parse(InfoList[5]),
                    SrnmType = sFuncCode,
                    ProductDate = dt,
                    TransactionDate = DateTime.Now,
                    StaffID = sStaffID,
                    ZoneID = sZoneID,
                    ZoneSN = sZoneSN,
                    Splice = InfoList[6],
                    Ptno = InfoList[11],
                    //CustomerNO = "b",
                    PackNo = InfoList[9],
                    Thickness2 = InfoList[1]
                };
                //寫入盤點資料寫入tempinventory資料表

            }
            //松電工
            else if (InfoList.Count == 12 && sCopper.Equals("4"))
            {
                //設定日期格式
                DateTime dt = SetDateTime(int.Parse(InfoList[9].Substring(0, 4)),
                                          int.Parse(InfoList[9].Substring(4, 2)),
                                          int.Parse(InfoList[9].Substring(6, 2)));
                Info = new TempInventory
                {
                    Thickness = InfoList[5],
                    Widt = InfoList[6],
                    Type = InfoList[11],
                    Prod = InfoList[3],
                    Leng = int.Parse((InfoList[7])),
                    NewWeight = float.Parse(InfoList[2]),
                    SrnmType = sFuncCode,
                    ProductDate = dt,
                    TransactionDate = DateTime.Now,
                    StaffID = sStaffID,
                    ZoneID = sZoneID,
                    ZoneSN = sZoneSN,
                    Splice = InfoList[8],
                    Ptno = InfoList[0],
                    //CustomerNO = "b",
                    PackNo = InfoList[10],
                    Thickness2 = InfoList[4]
                };
            }
            //日系一般
            else if (InfoList.Count == 9 && sCopper.Equals("5"))
            {
                //設定日期格式
                DateTime dt = SetDateTime(int.Parse(InfoList[5].Substring(0, 2)) + 2000,
                                          int.Parse(InfoList[5].Substring(2, 2)),
                                          int.Parse(InfoList[5].Substring(4, 2)));
                Info = new TempInventory
                {
                    //Thickness = InfoList[1],//日系一般沒有此項資料
                    Widt = InfoList[2],
                    Type = InfoList[6],
                    Prod = InfoList[0],
                    Leng = int.Parse((InfoList[3])),
                    NewWeight = float.Parse(InfoList[4]),
                    SrnmType = sFuncCode,
                    ProductDate = dt,
                    TransactionDate = DateTime.Now,
                    StaffID = sStaffID,
                    ZoneID = sZoneID,
                    ZoneSN = sZoneSN,
                    //Splice = InfoList[5],沒有接頭數
                    //CustomerNO = sCustomer,
                    //CtlNo1 = sCtlNo1,
                    //CtlNo2 = sCtlNo2,
                    Thickness2 = InfoList[1],
                    Ptno = InfoList[7]
                    //特殊記項為，InfoList[8]，尚無用到
                };
            }
            //一維條碼
            else if (InfoList.Count == 9)
            //else
            {
                //設定日期格式
                DateTime dt = SetDateTime(int.Parse(InfoList[6].Substring(0, 2)) + 2000,
                                          int.Parse(InfoList[6].Substring(2, 2)),
                                          int.Parse(InfoList[6].Substring(4, 2)));
                Info = new TempInventory
                {
                    Thickness = InfoList[1],
                    Widt = InfoList[2],
                    Type = InfoList[8],
                    Prod = InfoList[0],
                    Leng = int.Parse((InfoList[3])),
                    NewWeight = float.Parse(InfoList[4]),
                    SrnmType = sFuncCode,
                    ProductDate = dt,
                    TransactionDate = DateTime.Now,
                    StaffID = sStaffID,
                    ZoneID = sZoneID,
                    ZoneSN = sZoneSN,
                    Splice = InfoList[5],
                    //CustomerNO = "b"
                    //Ptno = InfoList[11]
                };
            }
            //將差異資料寫入ImportInventory資料表
            db.TempInventory.Add(Info);
            db.SaveChanges();
        }
        //出庫或盤點時，庫位資料比對
        public String ZoneInfo_DBCheck(String sZoneNo, String sProd) 
        {
            //取得ZoneID
            String sZoneID = GetZoneID(sZoneNo);
            String sResult = "";
            //先判斷是否有此支銅箔
            if (db.Inventory.Where(i => i.Prod == sProd).Count() == 0)
            {
                sResult = "無" + sProd + "此筆資料，此筆資料可能已出庫!!";
            }
            else 
            {
                //如果地點不同，代表地點條碼掃瞄有誤
                if (db.Inventory.Where(i => i.Prod == sProd && i.ZoneID == sZoneID).Count() == 0)
                {
                    sResult = "此銅箔的庫位資料比對不符!!";
                }
                else
                {
                    sResult = "true";
                }
            }
            
            return sResult;
        }
        public String MakeSN()
        {
            //依日期時間產生流水碼
            return DateTime.Now.ToString("ddHHmmss");
        }
        //密碼加密
        public string EncryptString(string sPwd)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(sPwd, "MD5");
        }
        //取得ZoneID
        public string GetZoneID(String sZoneNo) 
        {
            return db.Zone.Where(z => z.ZoneNo == sZoneNo).FirstOrDefault().ZoneID;
        }
        //取得ZoneNo
        public string GetZoneNo(String sZoneID) 
        {
            return db.Zone.Where(z => z.ZoneID == sZoneID).FirstOrDefault().ZoneNo;
        }
        //判斷是否為大寫英文字母
        public bool CheckEngStr(String sEngletter)
        {
            Regex Pattern = new Regex("^[A-Z]+$");
            return Pattern.IsMatch(sEngletter);
        }
        //設定製造日期的時間
        public DateTime SetDateTime(int iYear, int iMonth, int iDay) 
        {
            //設定日期格式
            DateTime dt = new DateTime(iYear, iMonth, iDay);
            return dt;
        }
        //新增異動資料method，使用IEnumerable
        public void InsertHistoryInfo(Inventory oInfo, String sAccount, String sType)
        {
            using(SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BarCode"].ConnectionString))
            {
                string sCommStr = "INSERT INTO HistoryInventory ( Type," + 
                                                               "Prod," + 
                                                               "Thickness," + 
                                                               "Widt," +
                                                               "Leng," + 
                                                               "Splice," + 
                                                               "Ptno," + 
                                                               "ProductDate," + 
                                                               
                                                               "StaffID," +
                                                               "TransactionDate," + 
                                                               "SrnmType," + 
                                                               
                                                               "NewWeight," + 
                                                             
                                                               "ZoneSN," +
                                                               "ZoneID," +

                                                               "Thickness2 ) " +
                                  "VALUES ( @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param11, @param12, @param13, @param18, @param20, @param21 , @param22) ";
                conn.Open();    
                SqlCommand scmd = new SqlCommand(sCommStr, conn);
                scmd.Parameters.Add(new SqlParameter("@param1", oInfo.Type));
                scmd.Parameters.Add(new SqlParameter("@param2", oInfo.Prod));
                scmd.Parameters.Add(new SqlParameter("@param3", oInfo.Thickness));
                scmd.Parameters.Add(new SqlParameter("@param4", oInfo.Widt));
                scmd.Parameters.Add(new SqlParameter("@param5", oInfo.Leng));
                scmd.Parameters.Add(new SqlParameter("@param6", oInfo.Splice));
                scmd.Parameters.Add(new SqlParameter("@param7", oInfo.Ptno));
                scmd.Parameters.Add(new SqlParameter("@param8", oInfo.ProductDate));
             
                scmd.Parameters.Add(new SqlParameter("@param11", sAccount));
                scmd.Parameters.Add(new SqlParameter("@param12", DateTime.Now));
                scmd.Parameters.Add(new SqlParameter("@param13", sType));
               
                scmd.Parameters.Add(new SqlParameter("@param18", oInfo.NewWeight));
                
                scmd.Parameters.Add(new SqlParameter("@param20", oInfo.ZoneSN));
                scmd.Parameters.Add(new SqlParameter("@param21", oInfo.ZoneID));

                scmd.Parameters.Add(new SqlParameter("@param22", oInfo.Thickness2));
                scmd.ExecuteNonQuery();
                conn.Close();
            }
        }
     
    }
}