using CommandLine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace Feyenoord_Selligent_Integration
{
    class Program
    {
        private static string ClubID = "";
        private static string Direction = "";
        static void Main(string[] args)
        {

            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                // consume Options type properties
                if (options.Verbose)
                {
                    Console.WriteLine("ClubId: {0}", options.ClubId);
                    Console.WriteLine("Direction: {0}", options.Items[0]);


                }
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }

            Direction = options.Items[0].ToLower();
            ClubID = options.ClubId;


            DataTable dt = Sql.ap_GetConfig(ClubID);
            Config _config = new Config(dt);

            if (string.IsNullOrEmpty(_config.Host) || string.IsNullOrEmpty(_config.UserName) || string.IsNullOrEmpty(_config.Password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Config is missing in ClubConfig Table");
                Console.ResetColor();
                throw new Exception("Argument Missing please Check Host ,  User and password  ");
            }

            try
            {
                Program app = new Program();
                app.Run(_config);


                if (Properties.Settings.Default.KeepOpen == true)
                {
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Sql.ap_UpdateError(_apibatchinfoId, ex.Message.ToString());
                Sql.sendEmail(ClubID, ex.Message.ToString());


                if (Properties.Settings.Default.KeepOpen == true)
                {
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                }
            }



        }
        public void Run(Config _config)
        {

            if (Direction.ToUpper() == "TICKET")
            {
                SyncTicket(_config);
            }

           else if (Direction.ToUpper() == "CONTACT")
            {
                SyncContact(_config);
            }

            else if (Direction.ToUpper() == "MERCHANDISE")
            {
                SyncMerchandise(_config);
            }
            else if (Direction.ToUpper() == "DOWNLOAD")
            {
                DownloadFile(_config);
            }

        }


        public void SyncContact(Config _config)
        {
            Console.WriteLine("\t\nGetting All Un-process BatchID");
            Console.WriteLine("\t\n--------------------------------");

            DataTable dtBatchIDs = Sql.ap_BatchID();

            Console.WriteLine("\t\nTotal Un-process BatchID : " + dtBatchIDs.Rows.Count);
            Console.WriteLine("\t\n--------------------------------");

            foreach (DataRow Batchid in dtBatchIDs.Rows)
            {
                DataTable dtSyncData = null;
                string dtDataCSV;
                int _batchID = 0;
                string filename = BuildFileName();

                _batchID = Convert.ToInt32(Batchid["BatchID"]);
                try
                {
                    dtSyncData = Sql.ap_GetData(_batchID);

                    Console.WriteLine("\t\nProcessing BatchID " + _batchID.ToString());
                    Console.WriteLine("\t\nTotal Number of Record  for " + _batchID + " is " + dtSyncData.Rows.Count);
                    Console.WriteLine("\t\n--------------------------------");

                }
                catch (SqlException _sqlException)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (SqlError error in _sqlException.Errors)
                    {
                        sb.AppendLine(string.Format("Error Calling ap_GetData  Stored Proc : \"{0}\"", error.Message));
                    }

                    Sql.ap_UpdateError(_batchID.ToString(), sb.ToString());
                    throw new Exception(sb.ToString());
                }

                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Calling ap_GetData Stored Proc :" + _Exception.Message);
                    throw new Exception("Error Calling ap_GetData Stored Proc :" + _Exception.Message);
                }

                Console.WriteLine("\t\nConverting Data Table To CSV");
                try
                {
                    dtDataCSV = Sql.DataTableToCSVUTF8(dtSyncData, '|');
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Converting Data Table To CSV :" + _Exception.Message);
                    throw new Exception("Error Converting Data Table To CSV :" + _Exception.Message);

                }

                Console.WriteLine("\t\nSaving file in Hard Disk " + _config.FilePath);
                try
                {
                    Sql.CSVWriter(filename, _config.FilePath, dtDataCSV);
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Saving file in Hard Disk :" + _Exception.Message);
                    throw new Exception("Saving file in Hard Disk :" + _Exception.Message);

                }

                FtpWrapper _ftp = new FtpWrapper(_config);

                try
                {
                    _ftp.UploadFtpFile("IN", filename);
                    _ftp.UploadFtpFileFlag("IN", filename.Replace(".csv", ".flag"));
                    Sql.ap_UpdateSuccess(_batchID.ToString(), filename, dtSyncData.Rows.Count.ToString(), "Contact");
                    Console.WriteLine("Successfully Uploaded file in FTP");

                }
                catch (WebException e)
                {
                    String status = ((FtpWebResponse)e.Response).StatusDescription;
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Uploading File in FTP :" + status);
                    throw new Exception("Error Uploading File in FTP :" + status);
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Uploading File in FTP :" + _Exception.Message);
                    throw new Exception("Error Uploading File in FTP :" + _Exception.Message);

                }


            }
        }

        public void SyncTicket(Config _config)
        {
            Console.WriteLine("\t\nGetting All Un-process BatchID");
            Console.WriteLine("\t\n--------------------------------");

            DataTable dtBatchIDs = Sql.ap_BatchID();

            Console.WriteLine("\t\nTotal Un-process BatchID : " + dtBatchIDs.Rows.Count);
            Console.WriteLine("\t\n--------------------------------");

            foreach (DataRow Batchid in dtBatchIDs.Rows)
            {
                DataTable dtSyncData = null;
                string dtDataCSV;
                int _batchID = 0;
                string filename = BuildTixFileName();

                _batchID = Convert.ToInt32(Batchid["BatchID"]);
                try
                {
                    dtSyncData = Sql.ap_GetTixData(_batchID);

                    Console.WriteLine("\t\nProcessing BatchID " + _batchID.ToString());
                    Console.WriteLine("\t\nTotal Number of Record  for " + _batchID + " is " + dtSyncData.Rows.Count);
                    Console.WriteLine("\t\n--------------------------------");

                }
                catch (SqlException _sqlException)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (SqlError error in _sqlException.Errors)
                    {
                        sb.AppendLine(string.Format("Error Calling ap_GetTixData  Stored Proc : \"{0}\"", error.Message));
                    }

                    Sql.ap_UpdateError(_batchID.ToString(), sb.ToString());
                    throw new Exception(sb.ToString());
                }

                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Calling ap_GetTixData Stored Proc :" + _Exception.Message);
                    throw new Exception("Error Calling ap_GetTixData Stored Proc :" + _Exception.Message);
                }

                Console.WriteLine("\t\nConverting Data Table To CSV");
                try
                {
                    dtDataCSV = Sql.DataTableToCSVUTF8(dtSyncData, '|');
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Converting Data Table To CSV :" + _Exception.Message);
                    throw new Exception("Error Converting Data Table To CSV :" + _Exception.Message);

                }

                Console.WriteLine("\t\nSaving file in Hard Disk " + _config.FilePath);
                try
                {
                    Sql.CSVWriter(filename, _config.FilePath, dtDataCSV);
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Saving file in Hard Disk :" + _Exception.Message);
                    throw new Exception("Saving file in Hard Disk :" + _Exception.Message);

                }

                FtpWrapper _ftp = new FtpWrapper(_config);

                try
                {
                    _ftp.UploadFtpFile("IN", filename);
                    _ftp.UploadFtpFileFlag("IN", filename.Replace(".csv", ".flag"));
                    Sql.ap_UpdateSuccess(_batchID.ToString(), filename, dtSyncData.Rows.Count.ToString(), "Ticket");
                    Console.WriteLine("Successfully Uploaded file in FTP");

                }
                catch (WebException e)
                {
                    String status = ((FtpWebResponse)e.Response).StatusDescription;
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Uploading File in FTP :" + status);
                    throw new Exception("Error Uploading File in FTP :" + status);
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Uploading File in FTP :" + _Exception.Message);
                    throw new Exception("Error Uploading File in FTP :" + _Exception.Message);

                }


            }
        }

        public void SyncMerchandise(Config _config)
        {
            Console.WriteLine("\t\nGetting All Un-process BatchID");
            Console.WriteLine("\t\n--------------------------------");

            DataTable dtBatchIDs = Sql.ap_BatchID();

            Console.WriteLine("\t\nTotal Un-process BatchID : " + dtBatchIDs.Rows.Count);
            Console.WriteLine("\t\n--------------------------------");

            foreach (DataRow Batchid in dtBatchIDs.Rows)
            {
                DataTable dtSyncData = null;
                string dtDataCSV;
                int _batchID = 0;
                string filename = BuildMerFileName();

                _batchID = Convert.ToInt32(Batchid["BatchID"]);
                try
                {
                    dtSyncData = Sql.ap_GetMerData(_batchID);

                    Console.WriteLine("\t\nProcessing BatchID " + _batchID.ToString());
                    Console.WriteLine("\t\nTotal Number of Record  for " + _batchID + " is " + dtSyncData.Rows.Count);
                    Console.WriteLine("\t\n--------------------------------");

                }
                catch (SqlException _sqlException)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (SqlError error in _sqlException.Errors)
                    {
                        sb.AppendLine(string.Format("Error Calling ap_GetMerData  Stored Proc : \"{0}\"", error.Message));
                    }

                    Sql.ap_UpdateError(_batchID.ToString(), sb.ToString());
                    throw new Exception(sb.ToString());
                }

                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Calling ap_GetMerData Stored Proc :" + _Exception.Message);
                    throw new Exception("Error Calling ap_GetMerData Stored Proc :" + _Exception.Message);
                }

                Console.WriteLine("\t\nConverting Data Table To CSV");
                try
                {
                    dtDataCSV = Sql.DataTableToCSVUTF8(dtSyncData, '|');
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Converting Data Table To CSV :" + _Exception.Message);
                    throw new Exception("Error Converting Data Table To CSV :" + _Exception.Message);

                }

                Console.WriteLine("\t\nSaving file in Hard Disk " + _config.FilePath);
                try
                {
                    Sql.CSVWriter(filename, _config.FilePath, dtDataCSV);
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Saving file in Hard Disk :" + _Exception.Message);
                    throw new Exception("Saving file in Hard Disk :" + _Exception.Message);

                }

                FtpWrapper _ftp = new FtpWrapper(_config);

                try
                {
                    _ftp.UploadFtpFile("IN", filename);
                    _ftp.UploadFtpFileFlag("IN", filename.Replace(".csv", ".flag"));
                    Sql.ap_UpdateSuccess(_batchID.ToString(), filename, dtSyncData.Rows.Count.ToString(), "Merchandise");
                    Console.WriteLine("Successfully Uploaded file in FTP");

                }
                catch (WebException e)
                {
                    String status = ((FtpWebResponse)e.Response).StatusDescription;
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Uploading File in FTP :" + status);
                    throw new Exception("Error Uploading File in FTP :" + status);
                }
                catch (Exception _Exception)
                {
                    Sql.ap_UpdateError(_batchID.ToString(), "Error Uploading File in FTP :" + _Exception.Message);
                    throw new Exception("Error Uploading File in FTP :" + _Exception.Message);

                }


            }
        }


        public void DownloadFile(Config _config)
        {
            FtpWrapper _ftp = new FtpWrapper(_config);

            _ftp.DownloadFtpFiles("OUT");
        }


        public string BuildFileName()
        {
            return ClubID + "_MCV-Selligent_import_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        }
        public string BuildTixFileName()
        {
            return ClubID + "_Ticketing-Selligent_import_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        }
        public string BuildMerFileName()
        {
            return ClubID + "_Merchandise-Selligent_import_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        }
    }
    class Options
    {
        [Option('C', "clubid", Required = true, HelpText = "ClubId to be processed.")]
        public string ClubId { get; set; }


        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [ValueList(typeof(List<string>), MaximumElements = 1)]
        public IList<string> Items { get; set; }



        [HelpOption]
        public string GetUsage()
        {
            string usage = "usage: DOTMAILER <INTEGRATION>\n";
            usage += "[-C clubid]\n";
            usage += "[-h help]       [-v verbose]\n";
            usage += "\n";
            return usage;
        }

    }
    interface ICommandLineParser
    {
        bool ParseArguments(string[] args, object options);
    }
    class CommandLineParser : ICommandLineParser
    {
        public CommandLineParser()
        {
            parser_ = new CommandLine.Parser();
        }
        public bool ParseArguments(string[] args, object options)
        {
            return parser_.ParseArguments(args, options);
        }

        private CommandLine.Parser parser_;
    }
}
