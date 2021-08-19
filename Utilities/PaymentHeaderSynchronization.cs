using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace API.Utilities
{
    public class PaymentHeaderSynchronization
    {
        private static SqlConnection _connection;
        public PaymentHeaderSynchronization()
        {
            if (Tools.coreConnection == null || Tools.syncConnection == null)
            {
                Tools.SetupSystem();
            }
            _connection = Tools.coreConnection;
        }

        private static bool CheckIfPaymentHeaderExists(string strPaymentHeaderNo, string strPaymentHeaderCompanyName, string strTableName)
        {
            bool blRVal = false;
            var strSqlStatement = $@"SELECT * FROM "+strTableName+" WHERE [No_] = @PaymentHeaderNo  AND [Company Name] = @PaymentHeaderCompanyName;";

            var cmdCommand = new SqlCommand(strSqlStatement, _connection);
            cmdCommand.Parameters.AddWithValue("@PaymentHeaderNo", strPaymentHeaderNo);
            cmdCommand.Parameters.AddWithValue("@PaymentHeaderCompanyName", strPaymentHeaderCompanyName);

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

        private static bool UpdatePaymentHeader(string strPaymentHeaderNo, string strPaymentHeaderCompanyName, string strTableName, JObject joPaymentHeader)
        {
            var blRVal = false;

            var strSetStatement = "";
            foreach (var selectToken in joPaymentHeader.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    string strFieldName;
                    string strFieldParameter;
                    if (selectToken != joPaymentHeader.Children().Last())
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

            var strSqlStatement = $@"UPDATE " + strTableName + " SET "+strSetStatement+" WHERE [No_] = @PaymentHeaderNo AND [Company Name] = @PaymentHeaderCompanyName;";

            var cmdCommand = new SqlCommand(strSqlStatement, _connection);

            foreach (var selectToken in joPaymentHeader.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    dynamic strValue = joPaymentHeader[selectToken.Path.Replace("['", "").Replace("']", "")].ToString();
                    Convert.ChangeType(strValue, strValue.GetType());
                    cmdCommand.Parameters.AddWithValue("@"+selectToken.Path.Replace("['", "").Replace("']", "").Replace(" ", "").Replace("-", ""), strValue);   
                }
            }
            
            cmdCommand.Parameters.AddWithValue("@PaymentHeaderNo", strPaymentHeaderNo);
            cmdCommand.Parameters.AddWithValue("@PaymentHeaderCompanyName", strPaymentHeaderCompanyName);
            
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

        private static bool InsertPaymentHeader(string strTableName, JObject joPaymentHeader)
        {
            var blRVal = false;

            var strFieldNames = "";
            var strFieldParameters = "";
            foreach (var selectToken in joPaymentHeader.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    if (selectToken != joPaymentHeader.Children().Last())
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

            foreach (var selectToken in joPaymentHeader.Children())
            {
                if (!selectToken.Path.Equals("timestamp"))
                {
                    dynamic strValue = joPaymentHeader[selectToken.Path.Replace("['", "").Replace("']", "")].ToString();
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

        public bool ProcessPaymentHeader(string strTableName, JObject joPaymentHeader)
        {
            var strHeaderNo = joPaymentHeader["No_"].ToString();
            var strPaymentHeaderCompanyName = joPaymentHeader["Company Name"].ToString();
            
            var blPaymentHeaderExists = CheckIfPaymentHeaderExists(strHeaderNo, strPaymentHeaderCompanyName, strTableName);

            var blRVal = blPaymentHeaderExists ? UpdatePaymentHeader(strHeaderNo, strPaymentHeaderCompanyName, strTableName, joPaymentHeader) : InsertPaymentHeader(strTableName, joPaymentHeader);

            return blRVal;
        }
    }
}