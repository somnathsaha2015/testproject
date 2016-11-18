using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Linq;
using KVConnector.Properties;
using TBSeed;
//using TBUtility;

namespace KVConnector
{
    public class Util
    {
        public static List<Seed> GetSaveOrderSeedList(SeedDataAccess seedDataAccess,string email, dynamic order)
        {
            List<Seed> seedList = new List<Seed>();
            AttachOrderMaster(seedDataAccess, email, seedList);
            AttachOrderDetails(order,seedList);
            return (seedList);
        }

        private static void AttachOrderDetails(dynamic order, List<Seed> seedList)
        {
            object[] orders = (object[])order;
            List<dynamic> orderList = orders.ToList();
            KeyValuePair<string, string> kvDetails = new KeyValuePair<string, string>("OrderId", "order");
            List<KeyValuePair<string, string>> kvListDetails = new List<KeyValuePair<string, string>>();
            kvListDetails.Add(kvDetails);
            
            orderList.ForEach(x =>
            {
                //x.OfferId = x.Id;
                //x.Id = null;
                Seed seed = new Seed()
                {
                    PKeyColName = "Id",
                    TableName = "OrderDetails",
                    TableDict = SeedUtil.GetDictFromDynamicObject(x),
                    IsCustomIDGenerated = false,
                    DetailsTableColNameTagNamePairs = kvListDetails
                };
                seedList.Add(seed);
            });
        }

        private static void AttachOrderMaster(SeedDataAccess seedDataAccess, string email,List<Seed> seedList)
        {
            Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> preSaveAction = (d1, d2, l) =>
            {
                string maxOrderNo = seedDataAccess.ExecuteScalarAsString(SqlResource.GetMaxOrderNumber);
                int maxNo = int.Parse(maxOrderNo);
                d1["OrderNo"] = maxNo + 1;
            };

            Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> postSaveAction = (d1, d2, l) =>
            {
                var value = d1["OrderNo"];
                List<SqlParameter> parms = new List<SqlParameter>();
                parms.Add(new SqlParameter("value", value));
                seedDataAccess.ExecuteScalar(SqlResource.SetMaxOrderNumber, parms);
            };

            dynamic orderMaster = new ExpandoObject();
            orderMaster.UserId = GetUserIdFromEmail(seedDataAccess, email);
            Seed seed = new Seed()
            {
                TableName = "OrderMaster",
                TableDict = SeedUtil.GetDictFromDynamicObject(orderMaster),
                PKeyColName = "Id",
                IsCustomIDGenerated = false,
                PKeyTagName = "order",
                PreSaveAction = preSaveAction,
                PostSaveAction = postSaveAction
            };
            seedList.Add(seed);
        }

        #region GetUserIdFromEmail
        public static int GetUserIdFromEmail(SeedDataAccess seedDataAccess, string email)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("email", email));
            string userId = seedDataAccess.ExecuteScalarAsString(SqlResource.GetUserIdFromEmail, parms);
            return (int.Parse(userId));
        } 
        #endregion

        #region GetMd5Hash
        public static string GetMd5Hash(string input)
        {
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));                
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }
        #endregion

        #region setError      
        public static void SetError(dynamic results, int status, string errorMessage, string details)
        {
            results.error = new ExpandoObject();
            results.error.status = status;
            results.error.message = errorMessage;
            results.error.details = details;
        }
        #endregion        

        #region SendMail   
        public static void SendMail(MailItem mailItem)
        {
            MailMessage mess = new MailMessage();
            mess.To.Add(mailItem.To);
            mess.From = new MailAddress(mailItem.From,null,Encoding.UTF8);
            mess.Subject = mailItem.Subject;
            mess.SubjectEncoding = Encoding.UTF8;
            mess.Body = mailItem.Body;
            mess.BodyEncoding = Encoding.UTF8;
            mess.IsBodyHtml = true;
            mess.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(mailItem.From, mailItem.Password);
            client.Port = mailItem.Port;
            client.Host = mailItem.Host;
            client.EnableSsl = true;
            client.Send(mess);
            //mess.To.Add("sagarwal@netwoven.com");
            //mess.From = new MailAddress("capitalch2@gmail.com", "Email header", System.Text.Encoding.UTF8);
            //mess.Subject = "This mail is send as test";
            //mess.SubjectEncoding = System.Text.Encoding.UTF8;
            //mess.Body = "This is Email Body";
            //mess.BodyEncoding = System.Text.Encoding.UTF8;
            //mess.IsBodyHtml = true;
            //mess.Priority = MailPriority.High;
            //SmtpClient client = new SmtpClient();
            //client.Credentials = new NetworkCredential("capitalch2@gmail.com", "su$hant123");
            //client.Port = 587;
            ////client.Port = 465;
            //client.Host = "smtp.gmail.com";
            //client.EnableSsl = true;
        }
        #endregion 


    }
}
