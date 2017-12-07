using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using MT.Common;
using MT.Common.Mail;
using MT.Common.Mail.Templates;
using System.IO;
using System.Net.Mail;
using System.Data;
using System.Data.Linq;
using System.Diagnostics;
using MT.Web.IntegrationLogger.Common;
using MT.Web.IntegrationLogger.Models;

namespace MT.Web.IntegrationLogger.Endpoint.IncorrectFileNotification
{
    public class SendNotification
    {
        private const int Module = 2001;

        public static void IncorrectFileNotification(string Type, string ReceivePortName, string Path, string FileName, string Error, string Emails, string DataBaseName, int LoggerID)
        {
            DateTime currentDate = DateTime.Now;
            IMailMessage sendMessage = null;

            try
            {
                Path = Path.Substring(0, Path.LastIndexOf('\\'));
            }
            catch { }

            sendMessage = new BizTalkPipelineError()
            {
                Subject = "[" + DataBaseName + "][Ошибки]В папку робота был загружен " + Type + " файл, который не был обработан",
                Path = Path,
                FileName = FileName,
                Error = Error,
                LoggerURL = "http://logger.integration.mjr.ru/Home/DetailLink?dbName=" + DataBaseName + "&amp;ID=" + LoggerID.ToString()
            };

            try
            {
                if (sendMessage != null)
                {
                    string filePath = Path + @"\ERROR\" + FileName;

                    if (File.Exists(filePath))
                    {
                        Attachment attach = new Attachment(filePath);

                        MailEngine.SendMailWithAttachments(
                            Emails,
                            "",
                            new Attachment[] { attach },
                            sendMessage);
                    }
                    else
                    {
                        MailEngine.SendMail(
                            Emails,
                            "",
                            sendMessage);
                    }
                }
            }

            catch (Exception exc)
            {
                //EventLog.WriteEntry("MT.IncorrectFileNotification.Excel", "Ошибка отправки на почту " + Emails + ". Ошибка: " + ex.Message, EventLogEntryType.Error, 10001);
                Logging.WriteToLog(new Log { Module = Module, SubModule = "IncorrectFileNotification", Exception = exc.Message });
            }
        }

        public static void SendException(int Type, string ReceivePortName, string Path, string FileName, string Error)
        {
            string fileType = "";

            try
            {
                using (DataContext DCMaster = Common.GetData.GetConnect())
                {
                    fileType = DCMaster.ExecuteQuery<string>("SELECT Name FROM [dbo].[Directory_ReceivePort_Types] WHERE Type = {0}", Type).FirstOrDefault();
                }

                IMailMessage ExcMessage;
                ExcMessage = new CriticalExceptions()
                {
                    Subject = "Ошибка настроек для уведомлений об ошибках в момент обработки файла роботом BizTalk",
                    Title = "Ошибка настроек",
                    ExcMessage = "Необходимо настроить обработку данной ошибки для типа [<b>" + fileType + "</b>], что бы сотрудникам уходили уведомления на почту и в MT.BizTalk Logger были записи об ошибках. Сделать это можно на странице: <a href=\"http:////manage.integration.mjr.ru//Messages//IncorrectNotification\">http://manage.integration.mjr.ru/Messages/IncorrectNotification</a><br />" +
                                       "<br /><h3>Трассировка сообщения:</h3> " +
                                       "Type = <b>" + fileType +
                                       "</b><br />ReceivePortName = <b>" + ReceivePortName +
                                       "</b><br />Path = <b>" + Path +
                                       "</b><br />FileName = <b>" + FileName +
                                       "</b><br /> Error = <b>" + Error + "</b>"
                };
                MailEngine.SendMail(
                            "wms-support@mjr.ru",
                            "alekhin.d@mjr.ru;moshkov@mjr.ru",
                            ExcMessage);
            }
            catch (Exception exc)
            {
                Logging.WriteToLog(new Log { Module = Module, SubModule = "SendException", Exception = exc.Message });
            }
        }
    }
}