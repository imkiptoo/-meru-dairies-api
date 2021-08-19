using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using API.Controllers;
using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SkyCrypto;

namespace API.Utilities
{
    public class Tools
    {
        public static SqlConnection coreConnection;
        public static SqlConnection syncConnection;
        public static Service service = new Service();
        public static ILogger<SynchronizeController> _logger;

        public static readonly string ENCRYPTION_KEY = "BfKzPbBC4Jg57ZqP55eUBL6An0i0PYo2UIEb0IVjPimMv63JIa3OGvr0wJBZtMtd";
        public JObject ErrorJObject()
        {
            return new JObject();
        }

        public String ShortenAccessToken(string theAccessToken)
        {
            String strAccessToken = theAccessToken.Replace("Bearer ", "");
            try
            {
                if (strAccessToken.Length > 250)
                {
                    return strAccessToken.Substring(0, 250);
                }
                else
                {
                    return strAccessToken;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public String GetAccessToken(HttpRequestMessage request)
        {
            String strAccessToken = "";
            try
            {
                strAccessToken = request.Headers.GetValues("Authorization").First();
                strAccessToken = new Tools().ShortenAccessToken(strAccessToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return strAccessToken;
        }

        public JObject GetRequestBody(HttpRequestMessage request)
        {
            JObject joRequestBody;
            try
            {
                var content = request.Content;
                string strBody = content.ReadAsStringAsync().Result;
                joRequestBody = JObject.Parse(strBody);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return joRequestBody;
        }

        public static String SanitizePhoneNumber(String thePhoneNumber)
        {
            thePhoneNumber = Regex.Replace(thePhoneNumber, "\\s", "");
            thePhoneNumber = Regex.Replace(thePhoneNumber, "^\\+", "");
            try
            {
                if (thePhoneNumber.StartsWith("+"))
                {
                    thePhoneNumber = Regex.Replace(thePhoneNumber, "^\\+", "");
                }

                if (Regex.IsMatch(thePhoneNumber, "^2547\\d{8}$") || Regex.IsMatch(thePhoneNumber, "^2541\\d{8}$"))
                {
                    return thePhoneNumber;
                }

                if (Regex.IsMatch(thePhoneNumber, "^07\\d{8}$") || Regex.IsMatch(thePhoneNumber, "^01\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^0", "254");
                }

                if (Regex.IsMatch(thePhoneNumber, "^7\\d{8}$") || Regex.IsMatch(thePhoneNumber, "^1\\d{8}$"))
                {
                    return "254" + thePhoneNumber;
                }

                if (Regex.IsMatch(thePhoneNumber, "^25407\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^25407", "2547");
                }

                if (Regex.IsMatch(thePhoneNumber, "^25401\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^25401", "2541");
                }

                if (Regex.IsMatch(thePhoneNumber, "^254\\+254\\d{9}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^254\\+254", "254");
                }

                if (Regex.IsMatch(thePhoneNumber, "^254254\\d{9}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^254254", "254");
                }

                if (Regex.IsMatch(thePhoneNumber, "^254\\+25401\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^254\\+25401", "2541");
                }

                if (Regex.IsMatch(thePhoneNumber, "^254\\+25407\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^254\\+25407", "2547");
                }

                if (Regex.IsMatch(thePhoneNumber, "^25425401\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^25425401", "2541");
                }

                if (Regex.IsMatch(thePhoneNumber, "^25425407\\d{8}$"))
                {
                    return Regex.Replace(thePhoneNumber, "^25425407", "2547");
                }

                return thePhoneNumber;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return thePhoneNumber;
            }
        }
        
        public static string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
        
        public static void SetupSystem()
        {
            var cbConfigurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false).Build();

            var strCoreDatabaseServer = cbConfigurationRoot.GetValue<string>("CoreDatabase:Server");
            var strCoreDatabaseDatabase = cbConfigurationRoot.GetValue<string>("CoreDatabase:Database");
            var strCoreDatabaseUser = cbConfigurationRoot.GetValue<string>("CoreDatabase:User");
            var strCoreDatabasePassword = cbConfigurationRoot.GetValue<string>("CoreDatabase:Password");
            var strCoreDatabasePasswordType = cbConfigurationRoot.GetValue<string>("CoreDatabase:PasswordType");

            var strSyncDatabaseServer = cbConfigurationRoot.GetValue<string>("SyncDatabase:Server");
            var strSyncDatabaseDatabase = cbConfigurationRoot.GetValue<string>("SyncDatabase:Database");
            var strSyncDatabaseUser = cbConfigurationRoot.GetValue<string>("SyncDatabase:User");
            var strSyncDatabasePassword = cbConfigurationRoot.GetValue<string>("SyncDatabase:Password");
            var strSyncDatabasePasswordType = cbConfigurationRoot.GetValue<string>("SyncDatabase:PasswordType");

            if (strCoreDatabasePasswordType.Equals("ENCRYPTED"))
            {
                strCoreDatabasePassword = Crypto.decrypt(Tools.ENCRYPTION_KEY, strCoreDatabasePassword);
            }
            else
            {
                var strNewCoreDatabasePassword = Crypto.encrypt(Tools.ENCRYPTION_KEY, strCoreDatabasePassword);
                AddOrUpdateAppSetting("CoreDatabase:Password", strNewCoreDatabasePassword);
                AddOrUpdateAppSetting("CoreDatabase:PasswordType", "ENCRYPTED");

                cbConfigurationRoot.GetValue<string>("Database:PasswordType");
            }
            
            if (strSyncDatabasePasswordType.Equals("ENCRYPTED"))
            {
                strSyncDatabasePassword = Crypto.decrypt(Tools.ENCRYPTION_KEY, strSyncDatabasePassword);
            }
            else
            {
                var strNewSyncDatabasePassword = Crypto.encrypt(Tools.ENCRYPTION_KEY, strSyncDatabasePassword);
                AddOrUpdateAppSetting("SyncDatabase:Password", strNewSyncDatabasePassword);
                AddOrUpdateAppSetting("SyncDatabase:PasswordType", "ENCRYPTED");

                cbConfigurationRoot.GetValue<string>("Database:PasswordType");
            }
            
            var strCoreDatabaseConnection = $"Server={strCoreDatabaseServer};Database={strCoreDatabaseDatabase};User Id={strCoreDatabaseUser};Password={strCoreDatabasePassword};";

            var strSyncDatabaseConnection = $"Server={strSyncDatabaseServer};Database={strSyncDatabaseDatabase};User Id={strSyncDatabaseUser};Password={strSyncDatabasePassword};";
            
            Print.PrettyLog(theTopOnly: true);
            Print.PrettyLog("Establishing Database Connection");
            Print.PrettyLog(theBottomOnly: true);

            try
            {
                coreConnection = new SqlConnection(strCoreDatabaseConnection);
                syncConnection = new SqlConnection(strSyncDatabaseConnection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            
            Print.PrettyLog((coreConnection.State & ConnectionState.Open) == 0
                ? "Database connected SUCCESSFULLY"
                : "FAILED to establish a connection to the database");
            Print.PrettyLog(theBottomOnly: true);
            Print.PrettyLog("Current Database Connection");
            Print.PrettyLog("Server: " + strCoreDatabaseServer);
            Print.PrettyLog("Database: " + strCoreDatabaseDatabase);
            Print.PrettyLog("User: " + strCoreDatabaseUser);
            Print.PrettyLog("Password: ************");
            Print.PrettyLog(theBottomOnly: true);
            
            if ((coreConnection.State & ConnectionState.Open) == 0)
            {
                service = Servicing.GetServiceData(syncConnection);
                List<Table> lsTablesForSynchronization = TableSetup.GetTables(syncConnection);

                foreach (var tblTable in lsTablesForSynchronization)
                {
                    TableManagement.CreateDocumentLineTrigger(coreConnection, strCoreDatabaseDatabase, tblTable.Name, tblTable.Company);
                    TableManagement.CreateTableIndexes(coreConnection, strCoreDatabaseDatabase, tblTable.Name, tblTable.Company);
                }
            }
        }

        static void AddOrUpdateAppSetting<T>(string key, T value) 
        {
            try 
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                var json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                
                var sectionPath = key.Split(":")[0];

                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    if (jsonObj != null) jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    if (jsonObj != null) jsonObj[sectionPath] = value;
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
            catch (Exception e) 
            {
                Console.WriteLine("Error writing app settings: "+e.StackTrace);
            }
        }
    }
}