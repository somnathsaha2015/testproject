using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TBSeed;

namespace KVConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //SendMail();
            //RandomAlphaNumeric();
            //AddUser();
            //CheckException();
            //CheckJObject();
            SaveOrder();
        }

        private static void SaveOrder()
        {
            var connString = "server=(local);Database=KistlerDB;Integrated Security=SSPI";
            SeedDataAccess seedDataAccess = new SeedDataAccess(connString);

            var orderMaster = GetOrderMasterData();
            var orderDetails = GetOrderDetailsData();

            List<Seed> seedList = new List<Seed>();
            //KeyValuePair<string, string> kvMaster = new KeyValuePair<string, string>("Id","OrderId");
            //List<KeyValuePair<string, string>> kvListMaster = new List<KeyValuePair<string, string>>();
            //kvListMaster.Add(kvMaster);

            KeyValuePair<string, string> kvDetails = new KeyValuePair<string, string>("OrderId", "order");
            List<KeyValuePair<string, string>> kvListDetails = new List<KeyValuePair<string, string>>();
            kvListDetails.Add(kvDetails);
            var getMaxOrderNumberSql = "select max(IntValue) from Setup where MKey = 'MaxOrderNumber'";
            var setMaxOrderNumberSql = "update Setup set IntValue = @value where MKey = 'MaxOrderNumber'";
            Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> preSaveAction = (d1, d2, l) =>
                {
                    string maxOrderNo = seedDataAccess.ExecuteScalarAsString(getMaxOrderNumberSql);
                    int maxNo = int.Parse(maxOrderNo);
                    d1["OrderNo"] = maxNo + 1;
                };

            Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> postSaveAction = (d1, d2, l) =>
            {
                var value = d1["OrderNo"];
                List<SqlParameter> parms = new List<SqlParameter>();
                parms.Add(new SqlParameter("value", value));
                seedDataAccess.ExecuteScalar(setMaxOrderNumberSql, parms);
            };

            Seed orderMasterSeed = new Seed()
            {
                TableName = "OrderMaster",
                TableDict = SeedUtil.GetDictFromDynamicObject(orderMaster),
                PKeyColName = "Id",
                IsCustomIDGenerated = false,
                PKeyTagName="order",
                PreSaveAction = preSaveAction,
                PostSaveAction = postSaveAction
            };
            seedList.Add(orderMasterSeed);

            orderDetails.ForEach(x=>
            {
                Seed details = new Seed()
                {
                    PKeyColName="Id",
                    TableName="OrderDetails",
                    TableDict = SeedUtil.GetDictFromDynamicObject(x),
                    IsCustomIDGenerated=false,
                    DetailsTableColNameTagNamePairs = kvListDetails                    
                };
                seedList.Add(details);
            });

            seedDataAccess.SaveSeeds(seedList);
        }

        private static dynamic GetOrderMasterData()
        {
            dynamic orderMaster = new ExpandoObject();
            //orderMaster.OrderNo = "1";
            orderMaster.UserId = 1;
            orderMaster.Descr = "Test";
            orderMaster.isDelivered = false;
            return (orderMaster);
        }

        private static List<dynamic> GetOrderDetailsData()
        {
            List<dynamic> orderDetails = new List<dynamic>();
            dynamic details1 = new ExpandoObject();
            details1.OfferId = 1;
            details1.OrderQty = 2;
            orderDetails.Add(details1);

            dynamic details2 = new ExpandoObject();
            details2.OfferId = 2;
            details2.OrderQty = 6;
            orderDetails.Add(details2);

            return (orderDetails);
        }

        private static void CheckJObject()
        {
            dynamic wineObject = new ExpandoObject();
            wineObject.parms= new ExpandoObject();
            wineObject.parms.item = "Wine";
            wineObject.parms.name = "Cresando";
            wineObject.parms.price = 250;
            wineObject.parms.qty = 2;
            JObject jObject = JObject.FromObject(wineObject);
            JToken jParms = jObject.SelectToken("parms");
            //jParms.
        }

        private static void CheckException()
        {
            try
            {
                Exception ex = new Exception("New");
                ex.Data.Add("101", "test");
                throw ex;
                //throw new Exception("Res Exception");

            }
            catch (Exception ex)
            {
                var v = ex.Data.Keys.Cast<List<object>>();
                var dictList = ex.Data.Cast<DictionaryEntry>();
               Console.WriteLine(ex.Message);
            }
        }

        private static void AddUser()
        {
            var connString = "server=(local);Database=KistlerDB;Integrated Security=SSPI";
            SeedDataAccess seedDataAccess = new SeedDataAccess(connString);
            dynamic user = new ExpandoObject();
            var email = "sa@gmail.com";
            var hash = "abcd";
            user.Email = email;
            user.PwdHash = hash;
            user.Role = "U";
            var kvUser = new KVUser()
            {
                Email = email,
                PwdHash = hash,
                Role = 'U'
            };
            Dictionary<string, object> kvUserDict = SeedUtil.GetDictFromObject(kvUser);
            Dictionary<string, object> userDict = SeedUtil.GetDictFromDynamicObject(user);
            var seed = new Seed()
            {
                TableName = "UserMaster",
                TableDict = userDict,
                //TableObject = "user",
                IsCustomIDGenerated = false,
                PKeyColName = "Id"
            };
            List<Seed> seedList = new List<Seed>();
            seedList.Add(seed);
            try
            {
                seedDataAccess.SaveSeeds(seedList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private static void RandomAlphaNumeric()
        {
            var alph = Guid.NewGuid().ToString().Substring(0, 8);
        }
        private static void SendMail()
        {

            MailMessage mess = new MailMessage();
            mess.To.Add("sagarwal@netwoven.com");
            mess.From = new MailAddress("capitalch2@gmail.com", "Email header", System.Text.Encoding.UTF8);
            mess.Subject = "This mail is send as test";
            mess.SubjectEncoding = System.Text.Encoding.UTF8;
            mess.Body = "This is Email Body";
            mess.BodyEncoding = System.Text.Encoding.UTF8;
            mess.IsBodyHtml = true;
            mess.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("capitalch2@gmail.com", "su$hant123");
            client.Port = 587;
            //client.Port = 465;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            try
            {
                client.Send(mess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
