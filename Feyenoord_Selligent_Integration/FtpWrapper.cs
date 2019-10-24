using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace Feyenoord_Selligent_Integration
{
    public class FtpWrapper
    {
        private string username { get; set; }
        private string password { get; set; }
        private string host { get; set; }
        private int port { get; set; }
        private string Filepath { get; set; }

        public FtpWrapper(Config config)
        {
            username = config.UserName;
            password = config.Password;
            host = config.Host;
            port = config.Port;
            Filepath = config.FilePath;
        }

        public void UploadFtpFile(string folderName, string absoluteFileName)
        {

            FtpWebRequest request;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);




            request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/{2}", this.host + ":" + this.port, folderName, absoluteFileName))) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.EnableSsl = true;

            request.Credentials = new NetworkCredential(this.username, this.password);






            using (FileStream fs = File.OpenRead(this.Filepath + absoluteFileName))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();
            }
        }
        public void DownloadFtpFiles(string folderName)
        {

            List<String> FileAvailableinFtps = GetFtpFileList(folderName);
            Console.WriteLine("Total file in FTP: "+ FileAvailableinFtps.Count);

            DataTable Fileallreadydownloadeds = Sql.ap_GetDownloadedfileName();


            if (FileAvailableinFtps.Count > 0)
            {
                foreach (string FileAvailableinFtp in FileAvailableinFtps)
                {
                    DataRow[] result = Fileallreadydownloadeds.Select("SourceFileName  = '" + FileAvailableinFtp + "'");

                    if (result.Length > 0)
                    {
                        Console.WriteLine("File is already downloaded");
                    }
                    else
                    {
                        Console.WriteLine("Downloading : "+ FileAvailableinFtp);
                        DownloadFtpFile(FileAvailableinFtp, folderName);
                    }



                }




            }



        }
        public void DownloadFtpFile(string file, string folderName)
        {
            string downloadpath = this.Filepath.Replace("Export", "Import");

            if (!Directory.Exists(downloadpath))
            {
                Directory.CreateDirectory(downloadpath);
            }



            FtpWebRequest request;
            FileStream writeStream = new FileStream(downloadpath + "\\" + file, FileMode.Create);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);




            request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/{2}", this.host + ":" + this.port, folderName, file))) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.EnableSsl = true;

            request.Credentials = new NetworkCredential(this.username, this.password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream ftpStream = response.GetResponseStream();

            long c1 = response.ContentLength;
            int buffersize = 1;
            int readcount;
            byte[] buffer = new byte[1];
            readcount = ftpStream.Read(buffer, 0, buffersize);
            while (readcount > 0)
            {
                writeStream.Write(buffer, 0, buffersize);
                readcount = ftpStream.Read(buffer, 0, buffersize);
            }

            ftpStream.Close();
            writeStream.Close();


        }

        public List<String> GetFtpFileList(string folderName)
        {
            List<String> list = new List<String>();
            FtpWebRequest request;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);




            request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/", this.host + ":" + this.port, folderName))) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            request.EnableSsl = true;

            request.Credentials = new NetworkCredential(this.username, this.password);

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string line = reader.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                list.Add(line);
                line = reader.ReadLine();
            }

            reader.Close();


            return list;
        }
        public void UploadFtpFileFlag(string folderName, string absoluteFileName)
        {

            FtpWebRequest request;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);




            request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/{2}", this.host + ":" + this.port, folderName, absoluteFileName))) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.EnableSsl = true;

            request.Credentials = new NetworkCredential(this.username, this.password);






            using (FileStream fs = File.OpenRead(this.Filepath.Replace("Export", "Program") + @"flag.txt"))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();
            }
        }

        public void UploadFtpFile_v2(string folderName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);



            FtpWebRequest request;

            string absoluteFileName = "Test by Sports alliance.txt";

            string ftppath = string.Format(@"ftp://{0}/{1}/{2}", this.host + ":" + this.port, folderName, absoluteFileName);

            request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/{2}", this.host + ":" + this.port, folderName, absoluteFileName))) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;


            request.Credentials = new NetworkCredential(this.username, this.password);
            request.EnableSsl = true;






            using (FileStream fs = File.OpenRead(@"C:\Users\Sports\Desktop\Test by Sports alliance.txt"))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();
            }
        }
    }
}




