using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using MT.Web.IntegrationLogger.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace MT.Web.IntegrationLogger.Common
{
    public class GetData
    {
        private const int Module = 1000;

        public static DataContext GetConnect()
        {
            DataContext dc = new DataContext("Data Source=m9-wms-biztalk;Initial Catalog=BizTalkActivityMonitor;Integrated Security=True");
            //DataContext dc = new DataContext("Data Source=10.8.1.66;Initial Catalog=BizTalkActivityMonitor;Integrated Security=True");

            return dc;
        }

        public static List<LayoutData> GetLayoutData(UserCookie userCookie)
        {
            List<LayoutData> lLD = new List<LayoutData>();
            string Date;

            try
            {
                DataContext dc = GetConnect();

                Date = DateTime.Now.ToString("yyyy-MM-dd");

                lLD.AddRange(dc.ExecuteQuery<LayoutData>("EXEC GetLayoutData @Type = 0, @Date = '" + Date + "', @Email = '" + userCookie.Email + "'").ToList());
                lLD.AddRange(dc.ExecuteQuery<LayoutData>("EXEC GetLayoutData @Type = 1, @Date = '" + Date + "', @Email = '" + userCookie.Email + "'").ToList());
                lLD.AddRange(dc.ExecuteQuery<LayoutData>("EXEC GetLayoutData @Type = 2, @Date = '" + Date + "', @Email = '" + userCookie.Email + "'").ToList());
                lLD.AddRange(dc.ExecuteQuery<LayoutData>("EXEC GetLayoutData @Type = 3, @Date = '" + Date + "', @Email = '" + userCookie.Email + "'").ToList());
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetLayoutData", Exception = exc.Message });
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 10);
            }

            return lLD;
        }

        public static List<OtherData> GetListData(int Type)
        {
            List<OtherData> lLD = new List<OtherData>();
            XDocument xmlDocument;

            try
            {
                DataContext dc = GetConnect();

                switch(Type)
                {
                    case 2:
                        xmlDocument = XDocument.Load(@"\\10.8.1.66\c$\WMS\Settings\settingMailUnknownTemplate.xml");

                        foreach (XElement settingsRow in xmlDocument.Root.Descendants("Destination"))
                        {
                            lLD.Add(new OtherData
                            {
                                Type = 2,
                                Value1 = settingsRow.Attribute("ReceivePortName").Value,
                                Value2 = settingsRow.Attribute("Emails").Value,
                                Value3 = settingsRow.Attribute("TargetDB").Value
                            });
                        }

                        break;
                    default:
                        lLD.AddRange(dc.ExecuteQuery<OtherData>("EXEC GetListData @Type = " + Type.ToString()).ToList());
                        break;
                }
               
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 160);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetListData", Exception = exc.Message });
            }

            return lLD;
        }

        public static List<DataBaseData> GetDatabaseData(int Type, string Name, string Date, string Email, int LoggerID = 0)
        {
            List<DataBaseData> dbData = new List<DataBaseData>();

            try
            {
                DataContext dc = GetConnect();

                try
                {
                    if(!string.IsNullOrEmpty(Date))
                    {
                        DateTime dateParse = DateTime.Parse(Date);

                        Date = dateParse.ToString("yyyy-MM-dd");
                    }
                    else
                        Date = DateTime.Now.ToString("yyyy-MM-dd");
                }
                catch
                {
                    Date = DateTime.Now.ToString("yyyy-MM-dd");
                }

                switch (Type)
                {
                    case 0:
                        {
                            dbData.AddRange(dc.ExecuteQuery<DataBaseData>("EXEC GetFullDateDetail_New @StartDate = '" + Date + "', @EndDate = '" + Date + "', @DataBaseName = '" + Name + "', @Email = '" + Email + "'").ToList());
                            break;
                        }
                    case 1:
                        {
                            dbData.AddRange(dc.ExecuteQuery<DataBaseData>("EXEC GetFullDateDetail_New @StartDate = '" + Date + "', @EndDate = '" + Date + "', @Customer = '" + Name + "', @Email = '" + Email + "'").ToList());
                            break;
                        }
                    case 2:
                        {
                            dbData.AddRange(dc.ExecuteQuery<DataBaseData>("EXEC GetFullDateDetail_New @StartDate = '" + Date + "', @EndDate = '" + Date + "', @Supplier = '" + Name + "', @Email = '" + Email + "'").ToList());
                            break;
                        }
                    case 3:
                        {
                            dbData.AddRange(dc.ExecuteQuery<DataBaseData>("EXEC GetFullDateDetail_New @StartDate = '" + Date + "', @EndDate = '" + Date + "', @DataBaseName = '" + Name + "', @LinkFile = 1, @Email = '" + Email + "'").ToList());
                            break;
                        }
                    case 4:
                        {
                            dbData.AddRange(dc.ExecuteQuery<DataBaseData>("EXEC GetFullDateDetail_New @StartDate = '" + Date + "', @EndDate = '" + Date + "', @DataBaseName = '" + Name + "', @LoggerID = " + LoggerID).ToList());
                            break;
                        }
                };

                foreach(DataBaseData data in dbData)
                {
                    int proccessTime;
                    Regex rFile = new Regex("[^\\\\/:*?\"<>|\\x00-\\x1F]+\\.\\w+$");
                    Match mFile = rFile.Match(data.File);

                    proccessTime = int.Parse(data.TimerProcessingSeconds);

                    if (proccessTime > 0)
                    {
                        if (proccessTime / 1000 > 60)
                            data.TimerProcessingSeconds = string.Format("{0} мин", Math.Round(TimeSpan.FromMilliseconds(proccessTime).TotalMinutes, 2));
                        else
                            data.TimerProcessingSeconds = string.Format("{0} сек", Math.Round(TimeSpan.FromMilliseconds(proccessTime).TotalSeconds, 2));
                    }
                    else if (proccessTime == 0 && !string.IsNullOrEmpty(data.ResultStatus))
                        data.TimerProcessingSeconds = string.Format("0,01 сек");

                    if (mFile.Success)
                    {
                        data.FileName = Regex.Replace(mFile.Value, @"(_\{[\w-]+\}){0,}(?=\.\w+)", "");
                    }
                }
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 100);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetDatabaseData", Exception = exc.Message });
            }

            return dbData;
        }

        public static DetailInfo GetDetailInfo(string dbName, int ID)
        {
            DetailInfo di = new DetailInfo();

            try
            {
                DataContext dc = GetConnect();

                di = dc.ExecuteQuery<DetailInfo>("EXEC GetDetailInfo @dbName = '" + dbName + "', @ID = " + ID).FirstOrDefault();
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 130);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetDetailInfo", Exception = exc.Message });
            }

            return di;
        }

        public static List<DetailLog> GetDetailLog(string dbName, int ID, string TableDate)
        {
            List<DetailLog> dl = new List<DetailLog>();

            try
            {
                DataContext dc = GetConnect();

                //Logging.WriteToLog(dc, new Log { Module = 1002, SubModule = "GetDetailLog" });
                dl = dc.ExecuteQuery<DetailLog>("EXEC GetDetailLog @DataBaseName = '" + dbName + "', @ID = " + ID + ", @TableDate = '" + TableDate + "'").ToList();
            }
            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 130);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetDetailLog", Exception = exc.Message });
            }

            return dl;
        }

        public static string RetrySendFileToBizTalk(string FileURL, string ReceivePortName)
        {
            string resultError = "";
            string selectReceiveLocation = "";
            FileStructure fileStructure = new FileStructure();
            List<string> receiveLocations = new List<string>();
            //Regex rFile = new Regex("[^\\\\/:*?\"<>|\\x00-\\x1F]+\\.\\w+$");

            try
            {
                fileStructure = GetFileName(FileURL);
                FileInfo fileInfo = new FileInfo(FileURL);

                //Логгирование
                Logging.WriteToLog(new Log { Module = 1001, SubModule = "RetrySendFileToBizTalk", ObjectName = fileStructure.FullFileName, ObjectTarget = ReceivePortName });

                if (!fileInfo.Exists)
                    return "Не найден файл [" + fileStructure.FullFileName + "] для повторного отправления. Повторно отправить файл невозможно. Загрузите данный файл ручным способом.";

                try
                {
                    DataContext dc = GetConnect();

                    receiveLocations = dc.ExecuteQuery<string>("EXEC [dbo].[GetBizTalkLocation] @ReceivePort = {0}", ReceivePortName).ToList();

                    if (receiveLocations.Count == 0)
                        return "Входящая локация [" + ReceivePortName + "] не найдена.";

                    //Match mFile = rFile.Match(FileURL);
                    //fileStructure = GetFileName(Regex.Replace(mFile.Value, @"(_\{[\w-]+\}){0,}(?=\.\w+)", ""));
                    
                    foreach (string receiveLocation in receiveLocations)
                    {
                        //Match mReceiveLoc = rFile.Match(receiveLocation);
                        FileStructure locationFileStructure = GetFileName(receiveLocation);

                        if ((fileStructure.FileExtension == locationFileStructure.FileExtension)
                            || ((!string.IsNullOrEmpty(locationFileStructure.FileExtension) && fileStructure.FileExtension.Contains(locationFileStructure.FileExtension))
                                || locationFileStructure.FileExtension == "*"))
                        {
                            selectReceiveLocation = receiveLocation;
                            break;
                        }
                    }

                    if(string.IsNullOrEmpty(selectReceiveLocation))
                        return "Для файла [" + fileStructure.FullFileName + "] не была найдена локация для загрузки";

                    string saveFile = selectReceiveLocation.Substring(0, selectReceiveLocation.LastIndexOf(@"\")).Trim() + @"\" + fileStructure.FullFileName;

                    File.Copy(FileURL, saveFile);
                    
                }
                catch (Exception exc)
                {
                    resultError = exc.Message;

                    //EventLog.WriteEntry("MT.Web.IntegrationLogger", exc.Message, EventLogEntryType.Error, 140);
                    Logging.WriteToLog(new Log { Module = Module, SubModule = "RetrySendFileToBizTalk", Exception = exc.Message });
                }

                return resultError;
            }
            catch (Exception exc)
            {
                return "Возникла критическая ошибка: " + exc.Message;
            }
        }

        public static FileStructure GetFileName(string FileName)
        {
            FileStructure fileStructure = new FileStructure();

            try
            {
                Regex R = new Regex(@"(\\)?(?<filename>[^\\]+)\.(?<fileext>(\w+|\*))$");
                Match M = R.Match(FileName);

                fileStructure.FileExtension = M.Groups["fileext"].Value;
                fileStructure.FileName = M.Groups["filename"].Value;
                fileStructure.FullFileName = M.Groups["filename"].Value + "." + M.Groups["fileext"].Value;
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetFileName", Exception = exc.Message });
            }



            return fileStructure;
        }

        public static string KillProcess(string DataBaseName, int SPID, int LoggerID)
        {
            string resultError = "";

            try
            {
                Logging.WriteToLog(new Log { Module = 1002, SubModule = "KillProcess", ObjectName = SPID.ToString(), ObjectTarget = DataBaseName });

                DataContext dc = GetConnect();

                resultError = dc.ExecuteQuery<string>("EXEC [dbo].[KillDatabaseProcess] @DataBaseName = {0}, @SPID = {1}, @LoggerID = {2}", DataBaseName, SPID, LoggerID).FirstOrDefault();

                if (!string.IsNullOrEmpty(resultError))
                    throw new Exception(resultError);
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "KillProcess", Exception = exc.Message });

                if (string.IsNullOrEmpty(resultError))
                    resultError = "Процесс загрузки не будет отменен! " + exc.Message;
            }

            return resultError;
        }

        public static List<SchlumbergerData> GetSchlumbergerData()
        {
            List<SchlumbergerData> lSLB = new List<SchlumbergerData>();

            try
            {
                DataContext dc = GetConnect();

                lSLB.AddRange(dc.ExecuteQuery<SchlumbergerData>("EXEC GetProjectData_Schlumberger").ToList());
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetSchlumbergerData", Exception = exc.Message });
            }

            return lSLB;
        }

        public static void SetSchlumbergerData(int ActionType, string Object)
        {
            try
            {
                DataContext dc = GetConnect();
                XDocument slbReport = new XDocument();

                //if (ActionType == 3)
                //{
                //    dc.ExecuteCommand("EXEC SetProjectData_Schlumberger @ActionType = {0}, @Object = {1}", ActionType.ToString(), "");
                //}
                //else
                //{
                    dc.ExecuteCommand("EXEC SetProjectData_Schlumberger @ActionType = {0}, @Object = {1}", ActionType.ToString(), Object);
                //}
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "SetSchlumbergerData", Exception = exc.Message });
            }
        }

        public static List<ECommerceLPPData> GetEcommerceLPPData(string Order, string Date, int? AttempIndex)
        {
            List<ECommerceLPPData> lLPP = new List<ECommerceLPPData>();

            try
            {
                try
                {
                    if (!string.IsNullOrEmpty(Date))
                    {
                        DateTime dateParse = DateTime.Parse(Date);

                        Date = dateParse.ToString("yyyy-MM-dd");
                    }
                    else
                        Date = DateTime.Now.ToString("yyyy-MM-dd");
                }
                catch
                {
                    Date = DateTime.Now.ToString("yyyy-MM-dd");
                }

                DataContext dc = GetConnect();

                lLPP.AddRange(dc.ExecuteQuery<ECommerceLPPData>("EXEC GetProjectData_LPPECommerce @Order = {0}, @Date = {1}, @AttempIndex = {2}", Order ?? "", Date ?? "", AttempIndex ?? 0).ToList());
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetEcommerceLPPData", Exception = exc.Message });
            }

            return lLPP;
        }

        public static List<LPPData> GetLPPData()
        {
            List<LPPData> lLPP = new List<LPPData>();
            DateTime maxDateTime = DateTime.Today;

            try
            {
                DataContext dc = GetConnect();

                List<string> lppFiles = Directory.GetFiles(@"\\tlh.local\mt\FTP\ret_production\Inbound\.done\transfers_socre\" + DateTime.Now.ToString("yyyyMMdd")).ToList();
                List<string> lppTransfers = new List<string>();

                foreach (string lppFile in lppFiles)
                {
                    FileInfo fi = new FileInfo(lppFile);

                    if (maxDateTime < fi.CreationTime)
                        maxDateTime = fi.CreationTime;

                    string lppFileValue = fi.Name.Substring(16, 10);

                    if (!lppTransfers.Contains(lppFileValue))
                        lppTransfers.Add(lppFileValue);
                }

                XDocument lppTransfersXML = new XDocument();

                lLPP.Add(new LPPData
                {
                    Type = 1003,
                    Data = maxDateTime.ToString("HH:mm:ss")
                });

                lppTransfersXML.Add(new XElement("Root", 
                                        lppTransfers.Select(s => new XElement("Transfer", s))));

                lLPP.AddRange(dc.ExecuteQuery<LPPData>("EXEC GetProjectData_LPP @TransfersList={0}", lppTransfersXML.ToString()).ToList());
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "GetLPPData", Exception = exc.Message });
            }

            return lLPP;
        }

        public static string CheckWebAddress(string WebAddress)
        {
            string resultError = "";

            try
            {
                try
                {
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

                    Task<PingReply> task = Task.Run(() => ping.Send(WebAddress));

                    if (task.Wait(TimeSpan.FromSeconds(5)))
                        resultError = "";
                    else
                        resultError = "TIMEOUT";
                }
                catch (Exception exc)
                {
                    resultError = exc.Message;

                    Logging.WriteToLog(new Log { Module = Module, SubModule = "CheckWebAddress", Exception = exc.Message });
                }

                return resultError;
            }
            catch (Exception exc)
            {
                return "Возникла критическая ошибка: " + exc.Message;
            }
        }
    }
}