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

namespace MT.Web.IntegrationLogger.Common
{
    public class Logging
    {
        private const int Module = 1400;   

        public static void WriteToLog(Log log)
        {

            using (DataContext DC = GetData.GetConnect())
            {
                WriteToLog(DC, log);
            }
        }

        public static void WriteToLog(DataContext DC, Log log)
        {
            string exception = "";

            try
            {
                if (string.IsNullOrEmpty(log.Exception)) 
                    log.Exception = "";

                if (string.IsNullOrEmpty(log.ObjectName)) 
                    log.ObjectName = "";

                if (string.IsNullOrEmpty(log.ObjectSource))
                    log.ObjectSource = "";

                if (string.IsNullOrEmpty(log.ObjectTarget))
                    log.ObjectTarget = "";

                DC.ExecuteCommand("EXEC [dbo].[adm_AddModuleLog] @Module = {0}, @SubModule = {1}, @Exception = {2}, @ObjectName = {3}, @ObjectSource = {4}, @ObjectTarget = {5}"
                    , log.Module, log.SubModule, log.Exception, log.ObjectName, log.ObjectSource, log.ObjectTarget);
            }
            catch (Exception exc)
            {
                DC.ExecuteCommand("EXEC [dbo].[adm_AddModuleLog] @Module = {0}, @SubModule = {1}, @Exception = {2}", Module, "WriteToLog", exc.Message);
                EventLog.WriteEntry("MT.Web.IntegrationLogger.Logging", exception, EventLogEntryType.Error, Module);
            }
            finally
            {
            }
            
        }
    }
}