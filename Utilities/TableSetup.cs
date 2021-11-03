using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using API.Services;
using Newtonsoft.Json.Linq;

namespace API.Utilities
{
    public static class TableSetup
    {
        public static  List<Table> GetTables(SqlConnection connection)
        {
            var lsTables = new List<Table>();
            var strSqlStatement = $@"SELECT * FROM main_company_tables FOR JSON AUTO;";

            var cmdCommand = new SqlCommand(strSqlStatement, connection);

            if ((connection.State & ConnectionState.Open) == 0)
            {
                connection.Open();
            }

            SqlDataReader reader = cmdCommand.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var strJsonResult = reader[0];
                        var jaJsonResult = JArray.Parse(strJsonResult.ToString() ?? string.Empty);

                        foreach (var joJsonResult in jaJsonResult)
                        {
                            var table = new Table
                            {
                                Name = joJsonResult["table_name"]?.ToString(),
                                Company = joJsonResult["table_company"]?.ToString()
                            };

                            lsTables.Add(table);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                //Worker._logger.LogError("Error {Error}:", e.StackTrace);
            }
            if (!reader.IsClosed)
            {
                reader.Close();   
            }

            return lsTables;
        }
        
        private static  bool EncryptUserPassword(SqlConnection connection, string theUserName, string thePassphrase)
        {
            var strSqlStatement = $@"UPDATE users SET passphrase_type = @PassphraseType, passphrase = @Passphrase WHERE username = @UserName;";

            var cmdCommand = new SqlCommand(strSqlStatement, connection);
            cmdCommand.Parameters.AddWithValue("@PassphraseType", "ENCRYPTED");
            cmdCommand.Parameters.AddWithValue("@Passphrase", thePassphrase);
            cmdCommand.Parameters.AddWithValue("@UserName", theUserName);

            if ((connection.State & ConnectionState.Open) == 0)
            {
                connection.Open();
            }

            return cmdCommand.ExecuteNonQuery() == 0;
        }
    }
}