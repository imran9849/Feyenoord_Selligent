using System;
using System.Data;

namespace Feyenoord_Selligent_Integration
{
    public class Config
    {

        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FilePath { get; set; }
        public int Port { get; set; }

        //SELLIGENT
        public Config(DataTable dt)
        {

            foreach (DataRow rowsConfig in dt.Rows)
            {
                if (rowsConfig.ItemArray[0].ToString().ToUpper() == "SELLIGENT_FTP_HOST")
                {
                    Host = rowsConfig.ItemArray[1].ToString();
                }
                else if (rowsConfig.ItemArray[0].ToString().ToUpper() == "SELLIGENT_FTP_USERNAME")
                {
                    UserName = rowsConfig.ItemArray[1].ToString();
                }
                else if (rowsConfig.ItemArray[0].ToString().ToUpper() == "SELLIGENT_FTP_PASSWORD")
                {
                    Password = rowsConfig.ItemArray[1].ToString();
                }
                else if (rowsConfig.ItemArray[0].ToString().ToUpper() == "SELLIGENT_FTP_PORT")
                {
                    Port = Convert.ToInt32(rowsConfig.ItemArray[1]);
                }
                else if (rowsConfig.ItemArray[0].ToString().ToUpper() == "SELLIGENT_FTP_FILEPATH")
                {
                    FilePath = rowsConfig.ItemArray[1].ToString();
                }
            }
        }
    }
}
