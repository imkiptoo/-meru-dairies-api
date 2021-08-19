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

                        svRVal.Id = joJsonResult["service_id"]?.ToString();
                        svRVal.Name = joJsonResult["service_name"]?.ToString();
                        svRVal.SynchronizationFrequency = int.Parse(joJsonResult["synchronization_frequency"]?.ToString()!);
                        svRVal.HeartBeatFrequency = int.Parse(joJsonResult["heartbeat_frequency"]?.ToString()!);
                        svRVal.MainServerHost = joJsonResult["main_server_host"]?.ToString();
                        svRVal.MainServerPort = int.Parse(joJsonResult["main_server_port"]?.ToString()!);
                        svRVal.ConnectionTimeout = int.Parse(joJsonResult["main_server_connection_timeout"]?.ToString()!);
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
    }
}