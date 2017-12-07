using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MT.Web.IntegrationLogger.Models;
using MT.Web.IntegrationLogger.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MT.Web.IntegrationLogger.Controllers
{
    public class HomeController : Controller
    {
        // Оригинал: http://bootstraplovers.com/templates/float-admin-v1.1/light-version/page-faqs.html
        private const int Module = 1100;  

        public ActionResult Index()
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();
            List<OtherData> disabledLocations = new List<OtherData>();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
                disabledLocations = GetData.GetListData(1);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 20);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Index", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.UserCookie = userCookie;
            @ViewBag.DisabledLocations = disabledLocations;

            return View();
        }

        public ActionResult FastJump()
        {
            List<LayoutData> lLD = new List<LayoutData>();
            List<OtherData> listDataBaseData = new List<OtherData>();
            List<OtherData> listSupplierData = new List<OtherData>();
            //List<OtherData> listIncorrextExcelEmail = new List<OtherData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
                listDataBaseData = GetData.GetListData(0);
                listSupplierData = GetData.GetListData(4);
                //listIncorrextExcelEmail = GetData.GetListData(2);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 30);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "FastJump", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.DataBasesSelect = listDataBaseData.Where(w => w.Type == 0).ToList();
            @ViewBag.SuppliersSelect = listSupplierData.Where(w => w.Type == 4).ToList();
            //@ViewBag.IncorrectEmails = listIncorrextExcelEmail.Where(w => w.Type == 2).ToList();

            return View();
        }

        public ActionResult Changelog()
        {
            List<LayoutData> lLD = new List<LayoutData>();
            List<OtherData> listDataBaseData = new List<OtherData>();
            List<OtherData> listIncorrextExcelEmail = new List<OtherData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
                listDataBaseData = GetData.GetListData(0);
                listIncorrextExcelEmail = GetData.GetListData(2);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 50);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Changelog", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.DataBasesSelect = listDataBaseData.Where(w => w.Type == 0).ToList();
            @ViewBag.IncorrectEmails = listIncorrextExcelEmail.Where(w => w.Type == 2).ToList();

            return View();
        }

        public ActionResult Coming()
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 40);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Coming", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult ManageEmailSession(int Type, UserCookie userCookie)
        {
            if (Type == 0)
            {
                HttpCookie Email = new HttpCookie("Email");
                Email.Value = userCookie.Email;
                Email.Expires = DateTime.Now.AddMonths(12);
                Response.Cookies.Add(Email);

                Logging.WriteToLog(new Log { Module = 1101, SubModule = "Login", ObjectName = userCookie.Email });
            }
            else if (Type == 1)
            {
                HttpCookie Email = new HttpCookie("Email");
                Email.Value = userCookie.Email;
                Email.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(Email);

                Logging.WriteToLog(new Log { Module = 1101, SubModule = "Logoff", ObjectName = userCookie.Email });
            }

            return RedirectToAction("Index");
        }

        public ActionResult NotSupported()
        {
            return View();
        }

    }
}
