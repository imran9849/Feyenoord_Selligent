using RestSharp;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Feyenoord_Selligent_Integration
{
    public class SelligentAPIWrapper
    {
        private static string url = "https://feyenoord.slgnt.eu/restapi/api/sync/lists/2/segments";
        private static string secret = "0977A8BC29304D03897703C65521573EF7FED7F74F974052B910FF7F7783BE1C";
        private static string username = "RESTAPI_USER_SPORTS.ALLIANCE_DO_NOT_MODIFY";
        private static string unixTimeStamp;
        private static string prehash;

        public SelligentAPIWrapper()
        {
            unixTimeStamp = (GetunixTimestamp()).ToString();


            var saiful_hash_Utf8 = CreateToken("1548252012-GET-/restapi/api/sync/lists/2/segments", secret);
            var saiful_hash_ASCII = CreateHash("1548252012-GET-/restapi/api/sync/lists/2/segments", secret);
            var saiful_hash_Jake = GeterateAthorizationHeader("GET", "/restapi/api/sync/lists/2/segments");





            prehash = unixTimeStamp + "-GET-" + "/restapi/api/sync/lists/2/segments";
            string method = "GET";
            string hash = CreateHash(prehash, secret);

            var client = new RestClient("https://feyenoord.slgnt.eu/restapi/api/sync/lists/2/segments");
            var request = new RestRequest(Method.GET);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            //Trust all certificates
            ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);



            request.AddHeader("Authorization", "hmac RESTAPI_USER_SPORTS.ALLIANCE_DO_NOT_MODIFY:" + hash.ToUpper() + ":" + unixTimeStamp.ToString());


            IRestResponse response = client.Execute(request);


        }





        private static string CreateHash(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

                return BitConverter.ToString(hashmessage).Replace("-", string.Empty);
                //return Convert.ToBase64String(hashmessage);
            }





        }

        private static string CreateToken(string message, string secret)
        {
            secret = secret ?? "";

            Encoding encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }


        private static Int32 GetunixTimestamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static string GeterateAthorizationHeader(string requestMethod, string requestUrl)
        {
            if (string.IsNullOrWhiteSpace(requestMethod))
            {
                throw new ArgumentNullException(nameof(requestMethod));
            }

            if (string.IsNullOrWhiteSpace(requestUrl))
            {
                throw new ArgumentNullException(nameof(requestUrl));
            }

            var startDate = new DateTime(1970, 1, 1);
            TimeSpan sinceStartDate = (DateTime.Now.ToUniversalTime() - startDate);
            long timestamp = (long)(sinceStartDate.TotalMilliseconds + 0.5) / 1000;

            string prehash = $"{timestamp}-{requestMethod}-{requestUrl}",
                   secret = "0977A8BC29304D03897703C65521573EF7FED7F74F974052B910FF7F7783BE1C",
                   username = "RESTAPI_USER_SPORTS.ALLIANCE_DO_NOT_MODIFY",
                   hash = null;

            Encoding encoding = new UTF8Encoding();

            using (var hmac = new HMACSHA256(encoding.GetBytes(secret)))
            {
                byte[] preHashBytes = encoding.GetBytes(prehash);
                string encodedRequest = Convert.ToBase64String(preHashBytes).Replace('+', '-').Replace('/', '_').Replace("=", "");

                byte[] hashBytes = hmac.ComputeHash(encoding.GetBytes(encodedRequest));
                hash = Convert.ToBase64String(hashBytes).Replace('+', '-').Replace('/', '_').Replace("=", "");
            }

            return $"hmac {username}:{hash.ToUpper()}:{timestamp}";
        }


    }
}
