using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Controller
{
    public class LinkManager_API
    {
        SqlConnection Cnn = new SqlConnection();

        public LinkManager_API()
        {

            if (!string.IsNullOrEmpty(Cnn.ConnectionString))
                return;
            SqlConnectionStringBuilder ConnBuilder = new SqlConnectionStringBuilder();
            //string ProviderName = "System.Data.SqlClient";

            ConnBuilder.DataSource = "clusql16a";
            ConnBuilder.InitialCatalog = "LinkManagement";
            ConnBuilder.MultipleActiveResultSets = true;
            ConnBuilder.PersistSecurityInfo = true;
            ConnBuilder.IntegratedSecurity = false;
            //ConnBuilder.UserID = "LinkMngUser"; //
            //ConnBuilder.Password = "LinkUser@23335";
            ConnBuilder.UserID = "LinkMngAdm";
            ConnBuilder.Password = "Linker@23335";
            Cnn.ConnectionString = ConnBuilder.ConnectionString;
        }
        //-------------------------------------------------------------------
        public string CreateLink(string ApplicationName, string ApplicationPass, string TargetApplicationName, string UserName, ref string LinkPassword, ref long LoginRequestID, string Data = "", string RequestIP = "", bool JoinMode = false, bool TestMode = false)
        {
            try
            {
                //SetConnectection("nisocsqla", "HSE_Total", "technicaldb", "Fn4333!2036");
                if (Cnn.State == System.Data.ConnectionState.Closed) Cnn.Open();
                String CommandText = "exec dbo.AddLoginRequest'" + ApplicationName + "','" + ApplicationPass + "','" + TargetApplicationName
                                    + "','" + UserName + "',N'" + Data + "','" + RequestIP + "'," + JoinMode + "," + TestMode;
                SqlCommand sqlCmd = new SqlCommand(CommandText, Cnn);
                SqlDataReader Reader = sqlCmd.ExecuteReader();
                string Result = "";
                if (Reader.HasRows)
                {
                    Reader.Read();
                    Result = Reader["Result"].ToString();
                    LoginRequestID = long.Parse(Reader["LoginRequestID"].ToString());
                    LinkPassword = Reader["LinkPassword"].ToString();
                }
                if (Cnn.State == System.Data.ConnectionState.Open) Cnn.Close();
                
                return Result;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                throw;
            }
        }
        //-------------------------------------------------------------------
        public string CheckLinkPassword(string ApplicationName, string UserName, string Password, ref long LoginRequestID, ref String Data, string RequestIP = "", string SourceApplicationName = "")
        {
            try
            {
                //SetConnectection("nisocsqla", "HSE_Total", "technicaldb", "Fn4333!2036");
                if (Cnn.State == System.Data.ConnectionState.Closed) Cnn.Open();
                String CommandText = "exec dbo.ValidateRequest '" + ApplicationName + "','" + UserName + "'" +
                                       ",N'" + Password + "','" + RequestIP + "','" + SourceApplicationName + "'";

                SqlCommand sqlCmd = new SqlCommand(CommandText, Cnn);
                SqlDataReader Reader = sqlCmd.ExecuteReader();

                if (!Reader.HasRows)
                    return "درخواست معتبر نیست";

                Reader.Read();

                string Result = Reader["Result"].ToString();
                if (Result == "OK")
                {
                    LoginRequestID = long.Parse(Reader["ID"].ToString());
                    Data = Reader["Data"].ToString();
                }
                if (Cnn.State == System.Data.ConnectionState.Open) Cnn.Close();

                return Result;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                throw;
            }

        }
   
        //-------------------------------------------------------------------
        public int SetResponse(long LoginRequestID, string Response)
        {
            if (Cnn.State == System.Data.ConnectionState.Closed) Cnn.Open();

            String CommandText = "exec [dbo].[SetResponse] " + LoginRequestID + ",'" + Response + "'";

            SqlCommand sqlCmd = new SqlCommand(CommandText, Cnn);
            int Result = sqlCmd.ExecuteNonQuery();
            return Result;
        }
        //-------------------------------------------------------------------
        public string GetResponse(long LoginRequestID, string Password)
        {
            if (Cnn.State == System.Data.ConnectionState.Closed) Cnn.Open();

            String CommandText = "exec [dbo].[GetResponse] " + LoginRequestID + ",N'" + Password + "'";

            SqlCommand sqlCmd = new SqlCommand(CommandText, Cnn);
            SqlDataReader Reader;
            string Response = null;
            DateTime Start = DateTime.Now;
            while (DateTime.Now.Subtract(Start).Minutes <= 2)
            {
                Reader = sqlCmd.ExecuteReader();
                if (!Reader.HasRows)
                    return null;
                Reader.Read();
                Response = Reader["Response"].ToString();
                Reader.Close();
                if (!string.IsNullOrEmpty(Response))
                    break;
            }
            return Response;
        }


    }
}
