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
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MT.Web.IntegrationLogger.Controllers
{
    public class ProjectController : Controller
    {
        private const int Module = 1400;

        public ActionResult Schlumberger()
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();
            List<SchlumbergerData> lSLB = new List<SchlumbergerData>();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
                lSLB = GetData.GetSchlumbergerData();
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Schlumberger", Exception = exc.Message });
            }

            @ViewBag.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();

            return View(lSLB);
        }

        [HttpPost]
        public JsonResult Schlumberger(int ActionType, string Object)
        {
            try
            {
                Logging.WriteToLog(new Log { Module = 1402, SubModule = "Schlumberger", ObjectTarget = Object, ObjectSource = ActionType.ToString() });
                GetData.SetSchlumbergerData(ActionType, Object);
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "Schlumberger", Exception = exc.Message });
            }

            return Json(new { Success = "true" });
        }

        public ActionResult SchlumbergerTrips()
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
                //lSLB = GetData.GetSchlumbergerData();
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "SchlumbergerTrips", Exception = exc.Message });
            }

            @ViewBag.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();

            return View();
        }

        public ActionResult ECommerceLPP(string Date, string Order)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();
            List<ECommerceLPPData> lLPP = new List<ECommerceLPPData>();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
                lLPP = GetData.GetEcommerceLPPData(Order, Date, null);
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "ECommerceLPP", Exception = exc.Message });
            }

            @ViewBag.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();
            @ViewBag.SearchDate = Date;
            @ViewBag.Order = Order;

            return View(lLPP);
        }

        public PartialViewResult _ECommerceData(string Order, int Type, int StepID, int AttempIndex)
        {
            List<ECommerceLPPData> lLPP = new List<ECommerceLPPData>();
            string Result = "";
            XDocument xResult = new XDocument();

            try
            {
                lLPP = GetData.GetEcommerceLPPData(Order, null, AttempIndex);

                if(Type == 0)
                {
                    Result = lLPP.Where(w => w.Step == StepID).Select(s => s.RequestBody).FirstOrDefault();
                }
                else
                {
                    Result = lLPP.Where(w => w.Step == StepID).Select(s => s.ResponseBody).FirstOrDefault();
                }

                xResult = XDocument.Parse(Result);
                Result = xResult.Root.Value;

                if (Result.Trim().Substring(0, 1).Contains("<"))
                {
                    XElement xElem = XElement.Parse(Result);

                    Result = xElem.ToString();
                }
                else if (Result.Trim().Substring(0, 3).Contains("xml"))
                {
                    XElement xElem = XElement.Parse(Result.Substring(13, Result.Length - 14));

                    Result = xElem.ToString();
                }
                else
                {
                    Result = Result.Replace("\n", "").Replace("\r", "");
                }
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "_ECommerceData", Exception = exc.Message });
            }

            return PartialView("_ECommerceData", Result);
        }

        [HttpPost]
        public JsonResult ECommerceCheckShippingAgent(int Type)
        {
            string resultError = null;

            {
                try
                {
                    switch(Type)
                    {
                        case 1:
                            resultError = GetData.CheckWebAddress(@"ws.dpd.ru");
                            break;
                        case 2:
                            resultError = GetData.CheckWebAddress(@"integration.cdek.ru");
                            break;
                        case 3:
                            resultError = GetData.CheckWebAddress(@"otpravka-api.pochta.ru");
                            break;
                    }
                    
                }
                catch (Exception exc)
                {
                    Logging.WriteToLog(new Log { Module = Module, SubModule = "ECommerceCheckShippingAgent", Exception = exc.Message });
                }
            }

            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();

            return Json(new { Success = (string.IsNullOrEmpty(resultError) == true ? true : false), ResultText = resultError });
        }

        public ActionResult LPP()
        {
            List<LayoutData> lLD = new List<LayoutData>();
            UserCookie userCookie = new UserCookie();
            List<LPPData> lLPP = new List<LPPData>();

            try
            {
                if (Request.Cookies["Email"] != null)
                    userCookie.Email = Request.Cookies["Email"].Value;
                else
                    userCookie.Email = "";

                lLD = GetData.GetLayoutData(userCookie);
                lLPP = GetData.GetLPPData();
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "LPP", Exception = exc.Message });
            }

            @ViewBag.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            @ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            @ViewBag.DataBases = lLD.Where(w => w.Type == 0 && w.Ignore == 0).ToList();
            @ViewBag.Customers = lLD.Where(w => w.Type == 1 && w.Ignore == 0).ToList();
            @ViewBag.Suppliers = lLD.Where(w => w.Type == 2 && w.Ignore == 0).ToList();
            @ViewBag.LinkFile = lLD.Where(w => w.Type == 3 && w.Ignore == 0).ToList();

            return View(lLPP);
        }
    }
}
