using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using BarCode_MVC4.Models;
using System.Data.Entity;

namespace BarCode_MVC4.Controllers.App
{
    public class BarCodeInfoController : Controller
    {
        //
        // GET: /BarCodeInfo/
        
        //加入log
        private Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //加入oDB物件
        DBOperation oDB = new DBOperation();
        String sResult = "";
        //加入單、雙支寫入資料庫的次數的記錄參數，初始設定為1
        int iTempCount = 1;
        //web.config設定的資料庫Entities
        BarCodeEntities db = new BarCodeEntities();
        //盤點參數
        string sInventoryStatus="";
        public ActionResult Index()
        {
            try
            {
                //讓手持端能擷取網頁訊息，網頁參數的判斷，顯示訊息
                if (Request.QueryString["CheckResult"] != null) 
                {
                    ViewBag.msg = Request.QueryString["Msg"];
                }

                //判斷是否有重新挑選作業別(1.出庫、2.入庫、4.盤點)，有則清空全部的網頁session
                if (Request.QueryString["Restart"] != null)
                {
                    //清空全部的session
                    Session.RemoveAll();

                    //判斷是否有傳盤點程序結束參數
                    int test1 = db.ClearInventory.Count();
                    int test2 = db.ImportInventory.Count();
                    if (Request.QueryString["InventoryProcess"] != null && (db.ClearInventory.Count() != 0 || db.ImportInventory.Count() != 0)) 
                    {
                        //"false"代表：此次的盤點程序結束;"true"代表：尚在盤點中
                        try
                        {
                            if (ModelState.IsValid)
                            {
                                var SysPara = db.SysPara.Where(r => r.ParaName == "Inventory");
                                if (SysPara != null)
                                {
                                    SysPara.FirstOrDefault().ParaValue = Request.QueryString["InventoryProcess"].ToString();
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                        };
                    }

                }

                //當sInventoryStatus為false，則無法盤點
                if (db.SysPara.Where(r => r.ParaName == "Inventory").FirstOrDefault().ParaValue.Equals("false") && Request.QueryString["FuncCode"].Equals("4"))
                {
                    ViewBag.msg = "請先核對前次盤點的差異資料!!";

                }
                else
                {
                    //只有在入庫，才會需要用到double參數
                    Session["Double"] = (Request.QueryString["FuncCode"] == "2") ? Request.QueryString["Double"] : "";
                    //只有在Double參數為0或1時，表示目前為單支銅箔或雙支銅箔的第一支，要產生一個流水號，2為雙支銅箔的第二支，延用1的流水號
                    Session["ZoneSN"] = (Session["Double"].ToString() == "2") ? Session["ZoneSN"] : oDB.MakeSN();

                    //當傳入條碼資訊時，包含人、地、物條碼資料
                    if (Request.QueryString["Info"] != null)
                    {
                        List<String> Info = new List<String>();
                        List<String> GoodsList = new List<String>();
                        //拆解三種條碼
                        Info = Request.QueryString["Info"].ToString().TrimEnd(';').Split(';').ToList();
                        //驗證人的條碼
                        bool bCheckPeople = oDB.BarCodeStaffCheck(Info[0].ToString());
                        if (bCheckPeople)
                        {
                            //入庫才需要，驗證地的條碼
                            //bool bCheckZone = oDB.BarCodeZoneCheck(Info[1].ToString());
                            if ((Request.QueryString["FuncCode"] == "2") ? oDB.BarCodeZoneCheck(Info[1].ToString()) : true)
                            {
                                //傳過的來的條碼資料為二維或松電工
                                if (Request.QueryString["Copper"].ToString().Equals("1") || Request.QueryString["Copper"].ToString().Equals("4"))
                                {
                                    try
                                    {
                                        //GoodsList = Info[2].ToString().TrimEnd(',').Split(',').ToList();
                                        GoodsList = Info[2].ToString().Split(',').ToList();
                                    }
                                    catch (Exception ex)
                                    {
                                        ViewBag.msg = "條碼解析有誤，請確認條碼的正確性!!";
                                    }
                                }
                                //一維捲裝
                                else if (Request.QueryString["Copper"].ToString().Equals("2"))
                                {
                                    try
                                    {
                                        GoodsList.Add(Info[2].ToString().Substring(0, 8));
                                        GoodsList.Add(Info[2].ToString().Substring(8, 1));
                                        GoodsList.Add(Info[2].ToString().Substring(9, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(13, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(17, 5));
                                        GoodsList.Add(Info[2].ToString().Substring(22, 1));
                                        GoodsList.Add(Info[2].ToString().Substring(23, 6));
                                        GoodsList.Add(Info[2].ToString().Substring(29, 2));
                                        GoodsList.Add(Info[2].ToString().Substring(31, Info[2].ToString().Length - 31));
                                    }
                                    catch (Exception ex) 
                                    {
                                        ViewBag.msg = "條碼解析有誤，請確認條碼的正確性!!";
                                    }
                                }
                                //一維片裝
                                else if (Request.QueryString["Copper"].ToString().Equals("3"))
                                {
                                    try 
                                    {
                                        GoodsList.Add(Info[2].ToString().Substring(0, 8));
                                        GoodsList.Add(Info[2].ToString().Substring(8, 1));
                                        GoodsList.Add(Info[2].ToString().Substring(9, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(13, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(17, 5));
                                        GoodsList.Add(Info[2].ToString().Substring(22, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(26, 6));
                                        GoodsList.Add(Info[2].ToString().Substring(32, 2));
                                        GoodsList.Add(Info[2].ToString().Substring(34, Info[2].ToString().Length - 34));
                                    }
                                    catch (Exception ex) 
                                    {
                                        ViewBag.msg = "條碼解析有誤，請確認條碼的正確性!!";
                                    }
                                    
                                }
                                //日系一般
                                else if (Request.QueryString["Copper"].ToString().Equals("5"))
                                {
                                    try
                                    {
                                        GoodsList.Add(Info[2].ToString().Substring(0, 8));
                                        GoodsList.Add(Info[2].ToString().Substring(8, 3));
                                        GoodsList.Add(Info[2].ToString().Substring(11, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(15, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(19, 4));
                                        GoodsList.Add(Info[2].ToString().Substring(23, 6));
                                        GoodsList.Add(Info[2].ToString().Substring(29, 5));
                                        GoodsList.Add(Info[2].ToString().Substring(34, 10));
                                        GoodsList.Add(Info[2].ToString().Substring(44, 2));
                                    }
                                    catch (Exception ex) 
                                    {
                                        ViewBag.msg = "條碼解析有誤，請確認條碼的正確性!!";
                                    }
                                }


                                //驗證物品條碼的格式是否正確，使用者尚未按下手持裝置的「確認」按鈕
                                bool CheckResult = oDB.BarCodeGoodsCheck(ref GoodsList, Request.QueryString["Copper"].ToString());
                                
                                string sResultMsg = "";
                                if (CheckResult == true)
                                {
                                   
                                    //等到使用者掃第二支後，才會傳OK
                                    if (Request.QueryString["Confirm"].ToString().Equals("NO"))
                                    {
                                        
                                        //雙支銅箔的訊息狀況
                                        if (Session["Double"].Equals("1") || Session["Double"].Equals("2"))
                                        {   
                                            //如果是雙支銅箔需要先暫存第一支銅箔資訊，待使用者掃第二支後才將兩支筒箔資料寫入
                                            if (Session["Double"].Equals("1"))
                                            {
                                                //ViewBag.msg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "條碼格式正確!!" : GoodsList[0].ToString() + "條碼格式正確，請掃第二支銅箔資料!!";
                                                sResultMsg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "條碼格式正確!!" : GoodsList[0].ToString() + "條碼格式正確，請掃第二支銅箔資料!!";
                                                Session["GoodsList1"] = GoodsList;
                                                //Response.Redirect("http://localhost:2080/BarCode_MVC4/BarCodeInfo/Index?CheckResult=true&Msg=" + sResultMsg);
                                                Response.Redirect("http://10.115.83.22/BarCode_MVC4/BarCodeInfo/Index?Msg=" + sResultMsg + "&CheckResult=true");
                                            }
                                            else
                                            {
                                                //ViewBag.msg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "條碼格式正確!!" : GoodsList[0].ToString() + "條碼格式正確，雙支銅箔掃瞄完成!!";
                                                sResultMsg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "條碼格式正確!!" : GoodsList[0].ToString() + "條碼格式正確，雙支銅箔掃瞄完成!!";
                                                //Response.Redirect("http://localhost:2080/BarCode_MVC4/BarCodeInfo/Index?CheckResult=true&Msg=" + sResultMsg);
                                                Response.Redirect("http://10.115.83.22/BarCode_MVC4/BarCodeInfo/Index?Msg=" + sResultMsg + "&CheckResult=true");
                                            }
                                        }
                                        else
                                        {
                                            //ViewBag.msg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "條碼格式正確!!" : GoodsList[0].ToString() + "條碼格式正確!!";
                                            sResultMsg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "條碼格式正確!!" : GoodsList[0].ToString() + "條碼格式正確!!";
                                            //讓手持端能擷取網頁端的訊息，用Response.Redirect
                                            //Response.Redirect("http://localhost:2080/BarCodeInfo/Index?CheckResult=true&Msg=" + sResultMsg);
                                            Response.Redirect("http://10.115.83.22/BarCode_MVC4/BarCodeInfo/Index?Msg=" + sResultMsg + "&CheckResult=true");
                                        }
                                    }

                                    //三個條碼都驗證成功且使用者按下手持裝置的「確認」按鈕，才對資料庫進行動作
                                    //*if (Session["StaffInfo"] != null && Session["ZoneInfo"] != null && Session["GoodsInfo"] != null)
                                    String sMsg = "";

                                    if (GoodsList != null && Request.QueryString["Confirm"].ToString().Equals("OK"))
                                    {
                                        //如果為雙支銅箔的話，iTempCount要為2
                                        if (Session["Double"].Equals("2"))
                                        {
                                            iTempCount = 2;
                                        }
                                        for (int i = 1; i <= iTempCount; i++)
                                        {
                                            //當i到2時，要將Session["GoodsList1"]的資料取出，寫入DB
                                            if (i == 2)
                                            {
                                                GoodsList = Session["GoodsList1"] as List<string>;
                                            }
                                            switch (Request.QueryString["FuncCode"])
                                            {
                                                //出庫
                                                case "1":
                                                    //出庫不檢查庫位資料
                                                    //sResult = oDB.ZoneInfo_DBCheck(Info[1].ToString(),
                                                    //    (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString());
                                                    //if (sResult.Equals("true"))
                                                    //{
                                                    //Info[0]存人的條碼資訊
                                                    //只有松電工的支號為GoodList[3]，其它都是GoodList[0]
                                                    sResult = oDB.IssuanceIntoDB(Info[0].ToString(),
                                                                                 (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString(),
                                                        //如果是二維條碼則是GoodsList[0]為支號，一維則GoodList[1]為支號
                                                        //(GoodsList.Count == 12) ? GoodsList[0].ToString() : GoodsList[1].ToString(),
                                                        //*Session["FuncCode"].ToString());
                                                                                 Request.QueryString["FuncCode"]);
                                                    if (sResult == "true")
                                                    {
                                                        logger.Trace("[" + DateTime.Now.ToShortDateString() + "] | [出庫作業] | " +
                                                                     "[支號：" + ((Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString()) + "]");
                                                        //"[支號：" + ((GoodsList.Count == 12) ? GoodsList[0].ToString() : GoodsList[1].ToString()) + "]");
                                                    }
                                                    else
                                                    {
                                                        logger.Trace("[" + DateTime.Now.ToShortDateString() + "] | [出庫作業訊息：" + sResult + "]");
                                                    }
                                                    sMsg = "出庫";
                                                    //}
                                                    //else 
                                                    //{
                                                    //    sMsg = sResult;
                                                    //}
                                                    break;
                                                //入庫
                                                case "2":
                                                    //*sResult = oDB.ReceiptIntoDB(Session["StaffInfo"].ToString(),
                                                    //*                      Session["ZoneInfo"].ToString(),
                                                    //Info[0]存人的條碼資料、Info[1]存地的條碼資料
                                                    sResult = oDB.ReceiptIntoDB(Info[0].ToString(),
                                                                                Info[1].ToString(),
                                                                                Session["ZoneSN"].ToString(),
                                                        //*Session["FuncCode"].ToString(),
                                                                                Request.QueryString["FuncCode"],
                                                                                GoodsList,
                                                                                Request.QueryString["Copper"].ToString(),
                                                                                Request.QueryString["Customer"].ToString(),
                                                                                Request.QueryString["SpecOne"].ToString(),
                                                                                Request.QueryString["SpecTwo"].ToString());
                                                    if (sResult == "true")
                                                    {
                                                        logger.Trace("[" + DateTime.Now.ToShortDateString() + "] | [入庫作業] | " +
                                                                     "[支號：" + ((Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString()) + "]" +
                                                            //"[支號：" + ((GoodsList.Count == 12) ? GoodsList[0].ToString() : GoodsList[1].ToString()) + "]" +
                                                                     "[流水號：" + Session["ZoneSN"].ToString() + "]");
                                                    }
                                                    else
                                                    {
                                                        logger.Trace("[" + DateTime.Now.ToShortDateString() + "] | [入庫作業訊息：" + sResult + "]");
                                                    }
                                                    sMsg = "入庫";
                                                    break;
                                                //盤點  
                                                case "4":
                                                    //if (oDB.ZoneInfo_DBCheck(Info[1].ToString(),
                                                    //    (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString()).Equals("true"))
                                                    //{
                                                    //*sResult = oDB.DeviceInventoryIntoDB(Session["StaffInfo"].ToString(),
                                                    //*                    Session["ZoneInfo"].ToString(),
                                                    //Info[0]存人的條碼資料、Info[1]存地的條碼資料
                                                    sResult = oDB.DeviceInventoryIntoDB(Info[0].ToString(),
                                                                                        Info[1].ToString(),
                                                                                        Session["ZoneSN"].ToString(),
                                                        //Session["FuncCode"].ToString(),
                                                                                        Request.QueryString["FuncCode"],
                                                                                        GoodsList,
                                                                                        Request.QueryString["Copper"].ToString());
                                                    if (sResult == "true")
                                                    {
                                                        logger.Trace("[" + DateTime.Now.ToShortDateString() + "] | [盤點作業] | " +
                                                                     "[支號：" + ((Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString()) + "]" +
                                                            //"[支號：" + ((GoodsList.Count == 12) ? GoodsList[0].ToString() : GoodsList[1].ToString()) + "]" +
                                                                     "[流水號：" + Session["ZoneSN"].ToString() + "]");
                                                    }
                                                    else
                                                    {
                                                        logger.Trace("[" + DateTime.Now.ToShortDateString() + "] | [盤點作業訊息：" + sResult + "]");
                                                    }
                                                    sMsg = "盤盈匯入";
                                                    //}
                                                    //else
                                                    //{
                                                    //    sMsg = sResult;
                                                    //}
                                                    break;
                                            }
                                            //資料成功寫入資料庫後，系統畫面的結果訊息。
                                            if (sResult == "true")
                                            {
                                                if (iTempCount.Equals(2))
                                                {
                                                    ViewBag.msg = "雙支銅箔掃瞄成功!!";
                                                }
                                                else
                                                {
                                                    ViewBag.msg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() + "，" + sMsg + "掃瞄成功!!" : GoodsList[0].ToString() + "，" + sMsg + "掃瞄成功!!";
                                                }
                                            }
                                            //其它訊息
                                            else
                                            {
                                                ViewBag.msg = sResult;
                                            }
                                            //清空GoodsLIst
                                            GoodsList.Clear();
                                        }
                                    }
                                    ////資料成功寫入資料庫後，系統畫面的結果訊息。
                                    //if (sResult == "true")
                                    //{
                                    //    ViewBag.msg = (Request.QueryString["Copper"].ToString().Equals("4")) ? GoodsList[3].ToString() : GoodsList[0].ToString() + "，" + sMsg + "掃瞄成功!!";
                                    //}
                                    ////其它訊息
                                    //else
                                    //{
                                    //    ViewBag.msg = sResult;
                                    //}
                                    ////清空GoodsLIst
                                    //GoodsList.Clear();
                                }
                                //驗證失敗
                                else
                                {
                                    //呈現物品條碼掃瞄錯誤的訊息
                                    //ViewBag.msg = "物品條碼有誤，請重新掃瞄";
                                    sResultMsg = "物品條碼有誤，請重新掃瞄";
                                    Response.Redirect("http://10.115.83.22/BarCode_MVC4/BarCodeInfo/Index?Msg=" + sResultMsg + "&CheckResult=false");
                                    //清空GoodsLIst
                                    GoodsList.Clear();
                                }
                                //ViewData["ZoneInfo"] = (Session["FuncCode"].ToString() != "1") ? Session["ZoneInfo"] + "_" + Session["ZoneSN"] : Session["ZoneInfo"];
                                //ViewData["StaffInfo"] = Session["StaffInfo"];
                            }
                            else
                            {
                                ViewBag.msg = "此庫位尚未建立，請重新掃瞄";
                            }
                        }
                        else
                        {
                            ViewBag.msg = "無此人員資料，請重新掃瞄";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Trace(DateTime.Now.ToString() + "產生：[" + ex.ToString()+ "]");
            }
            return View();
        }
    }
}
