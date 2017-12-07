using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MT.Web.IntegrationLogger.Models
{
    public class SchlumbergerData
    {
        public int Type { get; set; }
        public string Object { get; set; }
        public string Value { get; set; }
        public string Class { get; set; }
        public DateTime? Datetime { get; set; }
        public string Equipment { get; set; }
        public string ServiceProvider { get; set; }
    }

    public class ECommerceLPPData
    {
        public string Order { get; set; }
        public string ShippingAgent { get; set; }
        public int Step { get; set; }
        public string StepName { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int AttempIndex { get; set; }
    }

    public class LPPData
    {
        public int Type { get; set; }
        public string LocationCode { get; set; }
        public string Brand { get; set; }
        public int Count { get; set; }
        public int Sum { get; set; }
        public string Data { get; set; }
    }
}