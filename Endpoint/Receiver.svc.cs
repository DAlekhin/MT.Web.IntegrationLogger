using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Diagnostics;
using Newtonsoft.Json;
using MT.Web.IntegrationLogger;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using MT.Web.IntegrationLogger.Common;
using MT.Web.IntegrationLogger.Models;
using MT.Web.IntegrationLogger.Endpoint.IncorrectFileNotification;

namespace MT.Web.IntegrationLogger.Endpoint
{
    public class Receiver : IReceiver
    {
        private const int Module = 2000;        

        public string GetLog(WCFServiceLog json)
        {
            //string diagnosticResult = JsonConvert.SerializeObject(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            //if (diagnosticResult.Length > 30000)
            //    EventLog.WriteEntry("MT.Common.Misc.AsyncSendToBAMClass", diagnosticResult.Substring(0, 30000));
            //else
            //    EventLog.WriteEntry("MT.Common.Misc.AsyncSendToBAMClass", diagnosticResult);
            
            try
            {
                using (DataContext DCMaster = Common.GetData.GetConnect())
                {
                    if (json.LogRows == null)
                    {
                        DCMaster.ExecuteCommand("EXEC [dbo].[LogHeader] @DataBaseName = {0}, @StartProcessedDateTime = {1}, @EndProcessedDateTime = {2}, @CustomerID = {3}, @ProjectID = {4}, @StockID = {5}, @SupplierID = {6}, @FilePath = {7}, @ClosedDate = {8}, @LoggerSessionID = {9}, @MessageID = {10}, @ResultStatus = {11}, @ReceivePort = {12}, @SPID = {13}"
                            , json.DataBase, json.InsertDate, "17530101", json.Customer ?? "", json.Project ?? "", json.Stock ?? "", json.Supplier ?? "", json.LinkFile ?? "", DateTime.Now.Date, json.ID, json.MessageID ?? "", "", json.BizTalkReceivePortName ?? "", json.SPID);
                    }
                    else
                    {
                        DCMaster.ExecuteCommand("EXEC [dbo].[LogHeader] @DataBaseName = {0}, @StartProcessedDateTime = {1}, @EndProcessedDateTime = {2}, @CustomerID = {3}, @ProjectID = {4}, @StockID = {5}, @SupplierID = {6}, @FilePath = {7}, @ClosedDate = {8}, @LoggerSessionID = {9}, @MessageID = {10}, @ResultStatus = {11}, @ReceivePort = {12}, @SPID = 0"
                            , json.DataBase, json.InsertDate, json.EndDate, json.Customer ?? "", json.Project ?? "", json.Stock ?? "", json.Supplier ?? "", json.LinkFile ?? "", DateTime.Now.Date, json.ID, json.MessageID ?? "", json.Result, json.BizTalkReceivePortName ?? "");

                        string TableName = DCMaster.ExecuteQuery<string>("EXEC dbo.[GenerateNewTable]").ToList()[0];

                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("DataBaseName", typeof(string));
                        dataTable.Columns.Add("LoggerSessionID", typeof(int));
                        dataTable.Columns.Add("InsertDate", typeof(string));
                        dataTable.Columns.Add("Function", typeof(string));
                        dataTable.Columns.Add("Level", typeof(int));
                        dataTable.Columns.Add("Message", typeof(string));
                        dataTable.Columns.Add("Exception", typeof(string));

                        foreach (WCFLog wcfLog in json.LogRows)
                        {
                            dataTable.Rows.Add(json.DataBase, wcfLog.LoggerSessionID, wcfLog.InsertDateTime, wcfLog.Function, wcfLog.Level, wcfLog.Message, wcfLog.Exception);
                        }

                        string connectionString = DCMaster.Connection.ConnectionString;

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity))
                        {
                            bulkCopy.DestinationTableName = "[dbo].[" + TableName + "]";
                            bulkCopy.ColumnMappings.Clear();
                            bulkCopy.ColumnMappings.Add("DataBaseName", "DataBaseName");
                            bulkCopy.ColumnMappings.Add("LoggerSessionID", "LoggerSessionID");
                            bulkCopy.ColumnMappings.Add("InsertDate", "InsertDate");
                            bulkCopy.ColumnMappings.Add("Level", "Level");
                            bulkCopy.ColumnMappings.Add("Function", "Function");
                            bulkCopy.ColumnMappings.Add("Message", "Message");
                            bulkCopy.ColumnMappings.Add("Exception", "Exception");

                            bulkCopy.WriteToServer(dataTable);
                        };
                    }
                }
            }
            catch (Exception exc)
            {
                EventLog.WriteEntry("MT.Web.IntegrationLogger.Endpoint", exc.Message, EventLogEntryType.Error);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetLog", Exception = exc.Message });
            }

            return string.Format("OK");
        }

        public string IncorrectFileNotification(FileNotification fileNotification)
        {
            ReceivePort receivePort = new ReceivePort();

            using (DataContext DCMaster = Common.GetData.GetConnect())
            {
                receivePort = DCMaster.ExecuteQuery<ReceivePort>("SELECT DataBaseName, ReceivePortName, Emails FROM [dbo].[Directory_ReceivePort] WHERE ReceivePortName = {0} AND Type = {1}", fileNotification.ReceivePortName, fileNotification.Type).FirstOrDefault();
            }

            if (receivePort == null)
            {
                SendNotification.SendException(fileNotification.Type, fileNotification.ReceivePortName, fileNotification.Path, fileNotification.FileName, fileNotification.Error);

                return string.Format("OK");
            }

            int LoggerID = int.Parse(DateTime.Now.ToString("MMddHHmmss"));
            string dateTimeNow = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            #region Отправка в логгер
            try
            {
                List<WCFLog> wcfRow = new List<WCFLog>();
                wcfRow.Add(new WCFLog
                {
                    LoggerSessionID = LoggerID,
                    InsertDateTime = dateTimeNow,
                    Level = 6,
                    Function = "",
                    Message = fileNotification.Error,
                    Exception = fileNotification.Error
                });

                WCFServiceLog wcfLog = new WCFServiceLog
                {
                    ID = LoggerID,
                    DataBase = receivePort.DataBaseName,
                    InsertDate = dateTimeNow,
                    EndDate = dateTimeNow,
                    SendDate = dateTimeNow,
                    Customer = "",
                    Project = "",
                    Stock = "",
                    Supplier = "",
                    LinkFile = @"\\10.8.1.66\c$\WMS\Archive\" + fileNotification.FileName,
                    Module = "",
                    MessageID = "Ошибка при обработке файла",
                    Result = "FATAL",
                    LogRows = wcfRow,
                    BizTalkReceivePortName = fileNotification.ReceivePortName
                };

                GetLog(wcfLog);
            }
            catch (Exception exc)
            {
                EventLog.WriteEntry("MT.Web.IntegrationLogger.Endpoint", exc.Message, EventLogEntryType.Error);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "IncorrectFileNotification.GetLog", Exception = exc.Message });
            }
            #endregion

            switch (fileNotification.Type)
            {
                case 0:
                    SendNotification.IncorrectFileNotification("Excel", fileNotification.ReceivePortName, fileNotification.Path, fileNotification.FileName, fileNotification.Error, receivePort.Emails, receivePort.DataBaseName, LoggerID);
                    break;
                case 1:
                    SendNotification.IncorrectFileNotification("CSV", fileNotification.ReceivePortName, fileNotification.Path, fileNotification.FileName, fileNotification.Error, receivePort.Emails, receivePort.DataBaseName, LoggerID);
                    break;
                case 2:
                    SendNotification.IncorrectFileNotification("XML", fileNotification.ReceivePortName, fileNotification.Path, fileNotification.FileName, fileNotification.Error, receivePort.Emails, receivePort.DataBaseName, LoggerID);
                    break;
                case 3:
                    SendNotification.IncorrectFileNotification("EDIFACT", fileNotification.ReceivePortName, fileNotification.Path, fileNotification.FileName, fileNotification.Error, receivePort.Emails, receivePort.DataBaseName, LoggerID);
                    break;
                case 4:
                    SendNotification.IncorrectFileNotification("EMAIL", fileNotification.ReceivePortName, fileNotification.Path, fileNotification.FileName, fileNotification.Error, receivePort.Emails, receivePort.DataBaseName, LoggerID);
                    break;
                default:
                    break;
            }

            return string.Format("OK");
        }
    }
}
