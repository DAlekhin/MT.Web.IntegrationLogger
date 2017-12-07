using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace MT.Web.IntegrationLogger.Endpoint
{
    [ServiceContract]
    public interface IReceiver
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetLog",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            Method = "POST")]
        string GetLog(WCFServiceLog json);

        [OperationContract]
        [WebInvoke(UriTemplate = "/IncorrectFileNotification",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            Method = "POST")]
        string IncorrectFileNotification(FileNotification fileNotification);
    }

    [DataContract]
    public class WCFServiceLog
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Customer { get; set; }
        [DataMember]
        public string MessageID { get; set; }
        [DataMember]
        public string Module { get; set; }
        [DataMember]
        public string InsertDate { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public string SendDate { get; set; }
        [DataMember]
        public string LinkFile { get; set; }
        [DataMember]
        public string Project { get; set; }
        [DataMember]
        public string Stock { get; set; }
        [DataMember]
        public string Supplier { get; set; }
        [DataMember]
        public string DataBase { get; set; }
        [DataMember]
        public List<WCFLog> LogRows { get; set; }
        [DataMember]
        public string Result { get; set; }
        [DataMember]
        public string BizTalkReceivePortName { get; set; }
        [DataMember]
        public int SPID { get; set; }
    }

    public class WCFLog
    {
        [DataMember]
        public int LoggerSessionID { get; set; }
        [DataMember]
        public string InsertDateTime { get; set; }
        [DataMember]
        public string Function { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Exception { get; set; }
    }

    [DataContract]
    public class FileNotification
    {
        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public string ReceivePortName { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Error { get; set; }
    }

    public class ReceivePort
    {
        public string DataBaseName { get; set; }
        public string ReceivePortName { get; set; }
        public string Emails { get; set; }
    }
}
