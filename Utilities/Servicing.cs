using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using API.Services;

namespace API.Utilities
{
   public static class Servicing
    {
        public static  Service GetServiceData(SqlConnection connection)
        {
            Service svRVal = new Service();
            var strSqlStatement = $@"SELECT * FROM services FOR JSON AUTO;".Trim();

            var cmdCommand = new SqlCommand(strSqlStatement, connection);
            if ((connection.State & ConnectionState.Open) == 0)
            {
                connection.Open();
            }

            SqlDataReader reader = cmdCommand.ExecuteReader();
            try
            {
                if(reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var strJsonResult = reader[0];
                        var jaJsonResult = JArray.Parse(strJsonResult.ToString() ?? string.Empty);

                        var joJsonResult = jaJsonResult.First;
                        joJsonResult = JObject.Parse(joJsonResult?.ToString() ?? string.Empty);

                        svRVal.Name = joJsonResult["service_name"]?.ToString();
                        svRVal.MainCompanyName = joJsonResult["main_company_name"]?.ToString();
                        svRVal.MainCompanySyncEnabled = joJsonResult["main_company_sync_enabled"]?.ToString() == "ENABLED";
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
            }

            if (!reader.IsClosed)
            {
                reader.Close();   
            }

            return svRVal;
        }
        
        public static void CreateTableIndexes(SqlConnection connection, string strDatabaseName, string strTableName, string strCompanyName)
        {
            try
            {
                var strSqlStatement = $@"
                    IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = '[{strCompanyName}${strTableName}SyncStatus]' AND object_id = OBJECT_ID('[{strDatabaseName}].[dbo].[{strCompanyName}${strTableName}]'))
                    BEGIN CREATE INDEX [{strCompanyName}${strTableName}SyncStatus] ON [{strDatabaseName}].[dbo].[{strCompanyName}${strTableName}] ([Synchronization Status]); END;";
            
                var cmdCommand = new SqlCommand(strSqlStatement, connection);
                if ((connection.State & ConnectionState.Open) == 0)
                {
                    connection.Open();
                }
                
                cmdCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                //Worker._logger.LogError("Error {Error}:", e.StackTrace);
            }
        }
    }
}