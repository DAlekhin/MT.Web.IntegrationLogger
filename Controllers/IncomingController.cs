using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MT.Web.IntegrationLogger.Models;
using MT.Web.IntegrationLogger.Common;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace MT.Web.IntegrationLogger.Controllers
{
    public class IncomingController : Controller
    {
        // GET: /Incoming/
        private const int Module = 1200;  

        public ActionResult Database(string Name, string Date)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                Logging.WriteToLog(new Log { Module = 1202, SubModule = "Database", ObjectTarget = Name, ObjectSource = userCookie.Email });
                lLD = GetData.GetLayoutData(userCookie);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 50);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Database", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.dbName = Name;
            @ViewBag.SearchDate = Date;

            return View();
        }

        public ActionResult Customer(string Name, string Date)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                Logging.WriteToLog(new Log { Module = 1203, SubModule = "Customer", ObjectTarget = Name, ObjectSource = userCookie.Email });
                lLD = GetData.GetLayoutData(userCookie);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 60);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Customer", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.Customer = Name;
            @ViewBag.SearchDate = Date;

            return View();
        }

        public ActionResult Supplier(string Name, string Date)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                Logging.WriteToLog(new Log { Module = 1204, SubModule = "Supplier", ObjectTarget = Name, ObjectSource = userCookie.Email });
                lLD = GetData.GetLayoutData(userCookie);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 80);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Supplier", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.Supplier = Name;
            @ViewBag.SearchDate = Date;

            return View();
        }

        public ActionResult LinkFile(string Name, string Date)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                Logging.WriteToLog(new Log { Module = 1205, SubModule = "LinkFile", ObjectTarget = Name, ObjectSource = userCookie.Email });
                lLD = GetData.GetLayoutData(userCookie);
            }
            catch (Exception exc)
            {
               // EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 120);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "LinkFile", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.dbName = Name;
            @ViewBag.SearchDate = Date;

            return View();
        }

        public ActionResult Detail(string dbName, int ID)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            DetailInfo di = new DetailInfo();
            DataBaseData headerData = new DataBaseData();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                Logging.WriteToLog(new Log { Module = 1206, SubModule = "Detail", ObjectTarget = dbName, ObjectName = ID.ToString(), ObjectSource = userCookie.Email });
                lLD = GetData.GetLayoutData(userCookie);
                di = GetData.GetDetailInfo(dbName, ID);
                headerData = GetData.GetDatabaseData(4, dbName, di.TableDate.Substring(0, 4) + "-" + di.TableDate.Substring(4, 2) + "-" + di.TableDate.Substring(6, 2), "", ID).FirstOrDefault();
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 50);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Detail", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();

            @ViewBag.HeaderData = headerData;

            if (System.IO.File.Exists(di.FilePath))
            {
                Regex rFile = new Regex("[^\\\\/:*?\"<>|\\x00-\\x1F]+\\.\\w+$");
                Match mFile = rFile.Match(di.FilePath);

                if (mFile.Success)
                {
                    di.FileName = Regex.Replace(mFile.Value, @"(_\{[\w-]+\}){0,}(?=\.\w+)", "");
                }
            }

            return View(di);
        }

        [HttpPost]
        public JsonResult GetDataBaseJsonData(int Type, string Name, string Date)
        {
            List<DataBaseData> dbData = new List<DataBaseData>();

            try
            {
                dbData = GetData.GetDatabaseData(Type, Name, Date, GetUserCookie().Email);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 110);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetDataBaseJsonData", Exception = exc.Message });
            }

            return new LargeJsonResult() { Data = dbData };
        }

        public ActionResult DownloadFile(string UNCFilePath, string FileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(UNCFilePath);

                if (fileInfo.Exists)
                    return RedirectToAction("SendFile", new { UNCFilePath = UNCFilePath, FileName = FileName });
                else
                    throw new Exception();
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "DownloadFile", Exception = exc.Message });
                return Redirect(Request.UrlReferrer.ToString());
            } 
        }

        public FileResult SendFile(string UNCFilePath, string FileName)
        {
            Logging.WriteToLog(new Log { Module = 1201, SubModule = "DownloadFile", ObjectName = FileName });
            FileStream fileStream = new FileStream(UNCFilePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", FileName);
        }

        [HttpPost]
        public JsonResult GetLogJsonData(string dbName, int ID, string TableDate)
        {
            List<DetailLog> detailLog = new List<DetailLog>();

            try
            {
                detailLog = GetData.GetDetailLog(dbName, ID, TableDate);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 140);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetLogJsonData", Exception = exc.Message });
            }

            return new LargeJsonResult() { Data = detailLog };
        }

        public UserCookie GetUserCookie()
        {
            UserCookie userCookie = new UserCookie();

            if (Request.Cookies["Email"] != null)
                userCookie.Email = Request.Cookies["Email"].Value;
            else
                userCookie.Email = "";

            return userCookie;
        }

        [HttpPost]
        public JsonResult RetrySendFileToBizTalk(string FileURL, string ReceivePortName)
        {
            string resultError = "";
            
            try
            {
                FileURL = HttpUtility.UrlDecode(FileURL);
                resultError = GetData.RetrySendFileToBizTalk(FileURL, ReceivePortName);
            }
            catch (Exception exc)
            {
                resultError = exc.Message;

                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 150);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "RetrySendFileToBizTalk", Exception = exc.Message });
            }

            return Json(new { Success = (string.IsNullOrEmpty(resultError) == true ? true : false), ResultText = resultError });
        }

        public PartialViewResult _CancelLoading(string DataBaseName, int LoggerID, string Date)
        {
            DataBaseData headerData = new DataBaseData();

            try
            {
                if (string.IsNullOrEmpty(Date))
                    Date = DateTime.Now.ToString("dd.MM.yyyy");

                headerData = GetData.GetDatabaseData(4, DataBaseName, Date.Substring(6, 4) + "-" + Date.Substring(3, 2) + "-" + Date.Substring(0, 2), "", LoggerID).FirstOrDefault();
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "_CancelLoading", Exception = exc.Message });
            }

            return PartialView(headerData);
        }

        [HttpPost]
        public JsonResult CancelLoading(string DataBaseName, int SPID, int LoggerID)
        {
            string resultError = "";

            try
            {
                resultError = GetData.KillProcess(DataBaseName, SPID, LoggerID);
            }
            catch (Exception exc)
            {
                resultError = exc.Message;

                Logging.WriteToLog(new Log { Module = Module, SubModule = "CancelLoading", Exception = exc.Message });
            }

            return Json(new { Success = (string.IsNullOrEmpty(resultError) == true ? true : false), ResultText = resultError });
        }
    }
}
