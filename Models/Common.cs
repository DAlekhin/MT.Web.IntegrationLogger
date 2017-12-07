using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MT.Web.IntegrationLogger.Models
{
    public class LayoutData
    {
        public int Type { get; set; }
        public string Data { get; set; }
        public int Ignore { get; set; }
    }

    public class DataBaseData
    {
        public int LoggerID { get; set; }
        public string insertedDateTime { get; set; }
        public string TimerProcessingSeconds { get; set; }
        public string CustomerID { get; set; }
        public string ProjectID { get; set; }
        public string StockID { get; set; }
        public string SupplierID { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
        public string MessageID { get; set; }
        public string ResultStatus { get; set; }
        public string DataBaseName { get; set; }
        public string ReceivePortName { get; set; }
        //public string CustomerName { get; set; }
        public int SPID { get; set; }
    }

    public class DetailInfo
    {
        public string DataBaseName { get; set; }
        public string TableDate { get; set; }
        public int ID { get; set; }
        public string MessageID { get; set; }
        public string Exception { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }

    public class DetailLog
    {
        public int ID { get; set; }
        public string DataBaseName { get; set; }
        public string InsertDate { get; set; }
        public int Level { get; set; }
        public string Function { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }

    public class UserCookie
    {
        [Required(ErrorMessage = "Введите E-mail адрес")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@mjr\.ru$",
        ErrorMessage = "Введите корректный E-mail адрес с доменом @mjr.ru")]
        public string Email { get; set; }
    }

    public class OtherData
    {
        public int Type { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public DateTime Date1 { get; set; }
        public int Int1 { get; set; }
    }

    public class ReceivePortTypes
    {
        public int Type { get; set; }
        public string Name { get; set; }
    }

    public class FileStructure
    {
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public string FileExtension { get; set; }
    }

    public class Log
    {
        public int Module { get; set; }
        public string SubModule { get; set; }
        public string Exception { get; set; }
        public string ObjectName { get; set; }
        public string ObjectSource { get; set; }
        public string ObjectTarget { get; set; }
    }
}