using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace RsmgrImporter
{
    class Logger
    {
        public Logger()
        {
            // if location not set then set default
            if (!File.Exists(logs)) { File.Create(logs); }
            if (!File.Exists(exports)) { File.Create(exports); }
            if (ConfigurationManager.AppSettings["logfilepath"] == null)
            {
                AddUpdateAppSettings("logfilepath", "System.AppDomain.CurrentDomain.BaseDirectory");
                AddUpdateAppSettings("exportpath", "System.AppDomain.CurrentDomain.BaseDirectory");
            }

        }
        
        private static string logpath = ConfigurationManager.AppSettings["logfilepath"];
        private static string logname = @"rsmgr_log.txt";
        private static string logs = logpath + logname;
        private static string exportpath = ConfigurationManager.AppSettings["exportpath"];
        private static string exportname = @"rsmgr_export.txt";
        private static string exports = exportpath + exportname;
       
        public void writeToLog(string message)
        {
            try { File.AppendAllText(logs, DateTime.Now.ToString() + " | " + message + "\n"); }
            catch(Exception e)
            {
                PopUp popup = new PopUp("Importer Message", "Error writing to log. " + e.Message);
                popup.Show();
            }
            
        }

        public void writeToImports(string content)
        {
            try { File.AppendAllText(exports,content); }
            catch (Exception e) { writeToLog("Unable to Write to Imports. " + content + " : " + e.Message); }
        }

        private string ReadSetting(string key) // Read from App.config
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = "";
            try
            { result = appSettings[key] ?? "Not Found"; }
            catch (ConfigurationErrorsException e)
            {
                result = "";
                writeToLog("Unable to read from AppConfig Settings. " + e.Message);
            }
            return result;
        }

        private void AddUpdateAppSettings(string key, string value) // Adds value to App.config
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException e)
            {
                writeToLog("Error writing app settings. " + e.Message);
            }
        }
    }
}
