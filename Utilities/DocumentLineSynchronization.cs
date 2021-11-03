using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace API.Utilities
{
    public class DocumentLineSynchronization
    {
        private static SqlConnection _connection;
        public DocumentLineSynchronization()
        {
            if (Tools.coreConnection == null || Tools.syncConnection == null)
            {
                Tools.SetupSystem();
            }
            _connection = Tools.coreConnection;
        }

        private static bool CheckIfDocumentLineExists(string strDocumentLineHeaderNo, string strDocumentLineCompanyname, string strDocumentLineAccountNo, string strTableName)
        {
            bool blRVal = false;
            var strSqlStatement = $@"SELECT * FROM "+strTableName+" WHERE [Header No_] = @DocumentLineHeaderNo AND [Account No_] = @DocumentLineAccountNo AND [Company Name] = @DocumentLineCompanyName;";

            var cmdCommand = new SqlCommand(strSqlStatement, _connection);
            cmdCommand.Parameters.AddWithValue("@DocumentLineHeaderNo", strDocumentLineHeaderNo);
            cmdCommand.Parameters.AddWithValue("@DocumentLineAccountNo", strDocumentLineAccountNo);
            cmdCommand.Parameters.AddWithValue("@DocumentLineCompanyName", strDocumentLineCompanyname);
            if ((_connection.State & ConnectionState.Open) == 0)
            {
                _connection.Open();
            }

            SqlDataReader reader = cmdCommand.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    blRVal = true;
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

            return blRVal;
        }

        private static bool UpdateDocumentLine(string strDocumentLineHeaderNo, string strDocumentLineCompanyname, string strDocumentLineAccountNo, string strTableName, JObject joDocumentLine)
        {
            var blRVal = false;

            var strSetStatement = "";
            foreach (var selectToken in joDocumentLine.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    string strFieldName;
                    string strFieldParameter;
                    if (selectToken != joDocumentLine.Children().Last())
                    {
                        strFieldName = selectToken.Path.Replace("'", "");
                        strFieldParameter = selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", "");
                        strSetStatement += strFieldName + " = @" + strFieldParameter+", ";
                    }
                    else
                    {
                        strFieldName = selectToken.Path.Replace("'", "");
                        strFieldParameter = selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", "");
                        strSetStatement += strFieldName + " = @" + strFieldParameter;
                    }
                }
            }

            var strSqlStatement = $@"UPDATE " + strTableName + " SET "+strSetStatement+" WHERE [Account No_] = @AccountNo AND [Header No_] = @DocumentLineHeaderNo AND [Company Name] = @DocumentLineCompanyName;";

            var cmdCommand = new SqlCommand(strSqlStatement, _connection);

            foreach (var selectToken in joDocumentLine.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    dynamic strValue = joDocumentLine[selectToken.Path.Replace("['", "").Replace("']", "")].ToString();
                    Convert.ChangeType(strValue, strValue.GetType());
                    cmdCommand.Parameters.AddWithValue("@"+selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", ""), strValue);   
                }
            }
            
            cmdCommand.Parameters.AddWithValue("@AccountNo", strDocumentLineAccountNo);
            cmdCommand.Parameters.AddWithValue("@DocumentLineHeaderNo", strDocumentLineHeaderNo);
            cmdCommand.Parameters.AddWithValue("@DocumentLineCompanyName", strDocumentLineCompanyname);
            
            if ((_connection.State & ConnectionState.Open) == 0)
            {
                _connection.Open();
            }
            
            try
            {
                blRVal = cmdCommand.ExecuteNonQuery() == 1;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
            }
            return blRVal;
        }

        private static bool InsertDocumentLine(string strTableName, JObject joDocumentLine)
        {
            var blRVal = false;

            var strFieldNames = "";
            var strFieldParameters = "";
            foreach (var selectToken in joDocumentLine.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    if (selectToken != joDocumentLine.Children().Last())
                    {
                        strFieldNames += selectToken.Path.Replace("'", "");
                        strFieldNames += ", ";
                        strFieldParameters += "@"+selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", "");
                        strFieldParameters += ", ";
                    }
                    else
                    {
                        strFieldNames += selectToken.Path.Replace("'", "");
                        strFieldParameters += "@"+selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", "");
                    }   
                }
            }

            var strSqlStatement = $@"INSERT INTO " + strTableName + " (" + strFieldNames + ") VALUES (" + strFieldParameters + ");";

            var cmdCommand = new SqlCommand(strSqlStatement, _connection);

            foreach (var selectToken in joDocumentLine.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    dynamic strValue = joDocumentLine[selectToken.Path.Replace("['", "").Replace("']", "")].ToString();
                    Convert.ChangeType(strValue, strValue.GetType());
                    cmdCommand.Parameters.AddWithValue("@"+selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", ""), strValue);   
                }
            }
            
            if ((_connection.State & ConnectionState.Open) == 0)
            {
                _connection.Open();
            }
            
            try
            {
                blRVal = cmdCommand.ExecuteNonQuery() == 1;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
            }
            return blRVal;
        }

        public bool ProcessDocumentLine(string strTableName, JObject joDocumentLine)
        {
            var strDocumentLineHeaderNo = joDocumentLine["Header No_"].ToString();
            var strDocumentLineAccountNo = joDocumentLine["Account No_"].ToString();
            var strDocumentLineCompanyname = joDocumentLine["Company Name"].ToString();
            
            var blDocumentLineExists = CheckIfDocumentLineExists(strDocumentLineHeaderNo, strDocumentLineCompanyname, strDocumentLineAccountNo, strTableName);
            
            var blRVal = blDocumentLineExists ? UpdateDocumentLine(strDocumentLineHeaderNo, strDocumentLineCompanyname, strDocumentLineAccountNo, strTableName, joDocumentLine) : InsertDocumentLine(strTableName, joDocumentLine);

            return blRVal;
        }
    }
}