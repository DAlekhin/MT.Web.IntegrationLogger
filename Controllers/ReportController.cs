using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MT.Web.IntegrationLogger.Models;
using MT.Web.IntegrationLogger.Common;
using System.Diagnostics;

namespace MT.Web.IntegrationLogger.Controllers
{
    public class ReportController : Controller
    {
        // GET: /Report/
        private const int Module = 1300;

        public ActionResult Database()
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
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 20);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Database", Exception = exc.Message });
            }

            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.UserCookie = userCookie;

            return View();
        }

    }
}
