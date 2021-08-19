using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace API.Utilities
{
    public class TableManagement
    {
        public static void CreateDocumentLineTrigger(SqlConnection connection, string strDatabaseName, string strTableName, string strCompanyName)
        {
            try
            {
                var strFilters = "";
                switch (strTableName)
                {
                    case "Payment Header":
                    {
                        strFilters = $"i.[No_] = t.[No_]";
                        break;
                    }
                    case "Document Line":
                    {
                        strFilters = $"i.[Header No_] = t.[Header No_] AND i.[Account No_] = t.[Account No_]";
                        break;
                    }
                    case "Receipt Header":
                    {
                        strFilters = $"i.[No_] = t.[No_]";
                        break;
                    }
                }

                
                var strSqlStatement = $@"IF NOT OBJECT_ID('[{strCompanyName}${strTableName} Update Trigger]') IS NOT NULL
                    EXEC dbo.sp_executesql @statement = N'
                    CREATE TRIGGER [{strCompanyName}${strTableName} Update Trigger]
                        ON [{strDatabaseName}].dbo.[{strCompanyName}${strTableName}]
                        AFTER UPDATE
                    AS
                    SET NOCOUNT ON;
                    IF NOT UPDATE ([Synchronization Status])
                    BEGIN
                        UPDATE [{strDatabaseName}].dbo.[{strCompanyName}${strTableName}] SET [Synchronization Status] = ''PENDING''
                        FROM [{strDatabaseName}].dbo.[{strCompanyName}${strTableName}] AS t
                        INNER JOIN inserted i ON ({strFilters})
                    END'
                ;";
            
                var cmdCommand = new SqlCommand(strSqlStatement, connection);
                if ((connection.State & ConnectionState.Open) == 0)
                {
                    connection.Open();
                }
                
                cmdCommand.ExecuteNonQuery();
                
            }
            catch (Exception e)
            {
                Console.Write("Error "+ e.StackTrace);
            }
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
            catch (Exception)
            {
                //Console.Write(e.StackTrace);
                //Worker._logger.LogError("Error {Error}:", e.StackTrace);
            }
        }
    }
}