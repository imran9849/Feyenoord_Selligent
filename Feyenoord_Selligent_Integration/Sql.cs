using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace Feyenoord_Selligent_Integration
{
    public class Sql
    {
        public static DataTable ap_GetConfig(string ClubID)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_GetConfig";
            cmd.Parameters.AddWithValue("@ClubID", ClubID);
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;

        }

        public static DataTable ap_GetData(int BatchID)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_GetData";
            cmd.Parameters.AddWithValue("@BatchID", BatchID);
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;

        }

        public static DataTable ap_GetTixData(int BatchID)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_GetTixData";
            cmd.Parameters.AddWithValue("@BatchID", BatchID);
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;

        }
        public static DataTable ap_GetMerData(int BatchID)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_GetMerData";
            cmd.Parameters.AddWithValue("@BatchID", BatchID);
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;


        }

        public static DataTable ap_GetActiveTagData(int BatchID)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_GetActiveTagData";
            cmd.Parameters.AddWithValue("@BatchID", BatchID);
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;


        }
        public static DataTable ap_GetDownloadedfileName()
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_GetDownloadedfileName";
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;

        }

        public static DataTable ap_BatchID()
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_BatchID";
            cmd.Connection = sqlconn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable ds = new DataTable();
            da.Fill(ds);

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
            return ds;

        }


        public static void sendEmail(string _clubid, string _errormessage)
        {
            MailMessage oMsg = new MailMessage();

            oMsg.From = new MailAddress("integration.emails@sportsalliance.com");
            oMsg.To.Add("wvandijk@sportsalliance.com");
            oMsg.CC.Add("analyst@sportsalliance.com");
            oMsg.CC.Add("sislam@sportsalliance.com");
            oMsg.Subject = _clubid + " website call Failure Notification For ";

            oMsg.IsBodyHtml = true;
            oMsg.Body = "<HTML><BODY>" + _errormessage + "</BODY></HTML>";

            // ADD AN ATTACHMENT.
            //String sFile = Properties.Settings.Default.CorpCusLog;
            //Attachment oAttch = new Attachment(sFile);

            //oMsg.Attachments.Add(oAttch);

            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.hs20.net");
            smtp.Credentials = new System.Net.NetworkCredential("integration.emails@sportsalliance.com", "pennyblack");
            smtp.Send(oMsg);

            oMsg = null;
            //oAttch = null;
        }

        public static string DataTableToCSVUTF8(DataTable datatable, char seperator)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                sb.Append("\"" + datatable.Columns[i] + "\"");
                if (i < datatable.Columns.Count - 1)
                    sb.Append(seperator);
            }
            sb.AppendLine();
            foreach (DataRow dr in datatable.Rows)
            {
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    string datatype = GetDatacolumnType(datatable, dr, i);
                    string columname = dr.Table.Columns[i].ColumnName;

                    if (datatype.ToUpper() == "BOOLEAN")
                    {
                        if (dr[i].ToString().ToUpper() == "TRUE")
                        {
                            sb.Append("\"" + "1" + "\"");
                        }
                        else
                        {
                            sb.Append("\"" + "0" + "\"");
                        }
                    }
                    else if (datatype.ToUpper() == "DATETIME" && columname.ToUpper() != "GEBOORTEDATUM")
                    {

                        var TEMP = dr[i].ToString();

                        if (!String.IsNullOrEmpty(TEMP))
                        {
                            sb.Append("\"" + Convert.ToDateTime(dr[i]).ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                        }
                        else
                        {
                            sb.Append("\"" + dr[i].ToString() + "\"");
                        }



                    }
                    else if (datatype.ToUpper() == "DATETIME" && columname.ToUpper() == "GEBOORTEDATUM")
                    {
                        var TEMP = dr[i].ToString();

                        if (!String.IsNullOrEmpty(TEMP))
                        {
                            sb.Append("\"" + Convert.ToDateTime(dr[i]).ToString("dd-MM-yyyy") + "\"");
                        }
                        else
                        {
                            sb.Append("\"" + dr[i].ToString() + "\"");
                        }



                    }
                    else
                    {
                        sb.Append("\"" + dr[i].ToString() + "\"");
                    }






                    if (i < datatable.Columns.Count - 1)
                        sb.Append(seperator);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static void CSVWriter(string filename, string filepath, string csv)
        {
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            var invalids = Path.GetInvalidFileNameChars();

            foreach (char invalid in invalids)
            {
                filename = filename.Replace(invalid.ToString(), "");
            }


            string filefullpath = filepath + filename;
            File.WriteAllText(filefullpath, csv.ToString(), Encoding.UTF8);
        }


        public static void ap_UpdateError(string batchiD, string _errormessage)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_UpdateError";
            cmd.Connection = sqlconn;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@BatchID", batchiD);
            cmd.Parameters.Add("@ErrorMessage", _errormessage);
            cmd.ExecuteNonQuery();

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
        }

        public static void ap_UpdateSuccess(string batchiD, string FileName, string RC, string FileType)
        {
            #region establish sql connection
            SqlConnection sqlconn = new SqlConnection(Properties.Settings.Default.SQLConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            #endregion

            #region open connection
            if (sqlconn.State != ConnectionState.Open)
                sqlconn.Open();
            #endregion
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ap_UpdateSuccess";
            cmd.Connection = sqlconn;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@BatchID", batchiD);
            cmd.Parameters.Add("@FileName", FileName);
            cmd.Parameters.Add("@FileType", FileType);

            cmd.Parameters.Add("@RC", RC);
            cmd.ExecuteNonQuery();

            #region close connection
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();
            #endregion
        }


        public static string GetDatacolumnType(DataTable dt, DataRow dr, int ordinal)
        {

            string columnname = dr.Table.Columns[ordinal].ToString();
            var columnnameinfo = dt.Columns[columnname];

            return columnnameinfo.DataType.Name;



        }
    }



}
