using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using KVConnector.Properties;
using TBSeed;
using System.Collections;

namespace KVConnector
{
    public class KVConnection
    {
        static SeedDataAccess seedDataAccess;
        public Dictionary<string, Func<object, Task<object>>> RoutingDictionary;

        #region KVConnection
        public KVConnection()
        {
            if (RoutingDictionary == null)
            {
                RoutingDictionary = new Dictionary<string, Func<object, Task<object>>>();
                RoutingDictionary.Add("init", InitAsync);
                RoutingDictionary.Add("authenticate", AuthenticateAsync);
                RoutingDictionary.Add("isEmailExist", IsEmailExistAsync);
                RoutingDictionary.Add("change:password", ChangePasswordAsync);
                RoutingDictionary.Add("new:password", NewPasswordAsync);
                RoutingDictionary.Add("create:account", CreateAccountAsync);
                RoutingDictionary.Add("sql:query", ExecuteSqlQueryAsync);
                RoutingDictionary.Add("save:order", SaveOrderAsync);
                RoutingDictionary.Add("update:insert:profile", UpdateOrInsertProfileAsync);
                RoutingDictionary.Add("update:insert:address", UpdateOrInsertAddressesAsync);
                RoutingDictionary.Add("sql:non:query", ExecuteSqlNonQueryAsync);
                RoutingDictionary.Add("insert:credit:card", InsertCreditCardAsync);
                RoutingDictionary.Add("sql:scalar", ExecuteScalarAsync);
            }
        }
        #endregion

        #region Invoke
        public async Task<object> Invoke(object input)
        {
            IDictionary<string, object> payload = (IDictionary<string, object>)input;
            string action = payload["action"].ToString();
            Task<object> t = RoutingDictionary[action](payload);
            return await t;
        }
        #endregion

        #region InitAsync
        private async Task<object> InitAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    if ((objDictionary.ContainsKey("conn")))
                    {
                        var connString = objDictionary["conn"].ToString();
                        //Util.setConnString(connString); // to be removed
                        seedDataAccess = new SeedDataAccess(connString);
                        Console.WriteLine("Connection string successfully set");
                    }
                }
                catch (Exception ex)
                {
                    Util.SetError(result, 403, Resources.ErrInitFailed, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region AuthenticateAsync
        public async Task<object> AuthenticateAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    bool success = false;

                    if (objDictionary.ContainsKey("auth"))
                    {
                        string auth = objDictionary["auth"].ToString();
                        byte[] authBytes = Convert.FromBase64String(auth);
                        auth = Encoding.UTF8.GetString(authBytes);
                        if (auth.IndexOf(':') > 0)
                        {
                            string[] splitAuth = auth.Split(':');
                            if (splitAuth.Length == 2)
                            {
                                string email = splitAuth[0];
                                string hash = splitAuth[1];
                                List<SqlParameter> paramsList = new List<SqlParameter>();
                                paramsList.Add(new SqlParameter("email", email));
                                DataSet ds = seedDataAccess.ExecuteDataSet(Properties.SqlResource.GetHashAndRole, paramsList);
                                //DataSet ds = seedDataAccess.ExecuteDataSet(false, Properties.SqlResource.GetHashAndRole, paramsList);
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        var pwdHash = ds.Tables[0].Rows[0]["PwdHash"].ToString();
                                        var role = ds.Tables[0].Rows[0]["Role"].ToString();
                                        var userId = ds.Tables[0].Rows[0]["id"].ToString();
                                        if (hash == pwdHash)
                                        {
                                            success = true;
                                            result.authenticated = true;
                                            dynamic user = new ExpandoObject();
                                            user.email = email;
                                            user.userId = userId;
                                            user.role = role;
                                            result.user = user;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!success)
                    {
                        result.authenticated = false;
                        Util.SetError(result, 401, Resources.ErrAuthenticationFailure, Resources.MessAuthenticationFailed);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region IsEmailExistAsync
        public async Task<object> IsEmailExistAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    bool success = false;

                    if (objDictionary.ContainsKey("email"))
                    {
                        string email = objDictionary["email"].ToString();
                        result.email = email;
                        List<SqlParameter> paramsList = new List<SqlParameter>();
                        paramsList.Add(new SqlParameter("email", email));
                        var isExist = seedDataAccess.ExecuteScalar(SqlResource.IsEmailExist, paramsList);
                        if (isExist != null)
                        {
                            success = true;
                        }
                    }
                    if (success)
                    {
                        result.status = 200;
                        result.isEmailExist = true;
                    }
                    else
                    {
                        Util.SetError(result, 404, Resources.ErrResourceNotFound, Resources.ErrResourceNotFound);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region ChangePasswordAsync
        public async Task<object> ChangePasswordAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    bool success = false;

                    if (objDictionary.ContainsKey("auth"))
                    {
                        string auth = objDictionary["auth"].ToString();
                        byte[] authBytes = Convert.FromBase64String(auth);
                        auth = Encoding.UTF8.GetString(authBytes);
                        if (auth.IndexOf(':') > 0)
                        {
                            string[] splitAuth = auth.Split(':');
                            if (splitAuth.Length == 3)
                            {
                                string email = splitAuth[0];
                                string oldPwdHash = splitAuth[1];
                                string newPwdHash = splitAuth[2];
                                List<SqlParameter> paramsList = new List<SqlParameter>();
                                paramsList.Add(new SqlParameter("email", email));
                                paramsList.Add(new SqlParameter("oldPwdHash", oldPwdHash));
                                var isExist = seedDataAccess.ExecuteScalar(SqlResource.IsEmailExist, paramsList);
                                if (isExist != null) // email and password hash exists in database. Now change password Hash
                                {
                                    paramsList = new List<SqlParameter>();
                                    paramsList.Add(new SqlParameter("email", email));
                                    paramsList.Add(new SqlParameter("oldPwdHash", oldPwdHash));
                                    paramsList.Add(new SqlParameter("newPwdHash", newPwdHash));
                                    if (objDictionary.ContainsKey("emailItem"))
                                    {
                                        int ret = seedDataAccess.ExecuteNonQuery(SqlResource.ChangePasswordHash, paramsList);
                                        if (ret == 1)
                                        {
                                            var emailItem = (dynamic)objDictionary["emailItem"];
                                            MailItem item = new MailItem()
                                            {
                                                From = emailItem.fromUser,
                                                FromName = emailItem.fromUserName,
                                                Host = emailItem.host,
                                                IsBodyHtml = true,
                                                Body = emailItem.htmlBody,
                                                Password = emailItem.fromUserPassword,
                                                Port = emailItem.port,
                                                Subject = emailItem.subject,
                                                To = email
                                            };
                                            try
                                            {
                                                Util.SendMail(item);
                                                success = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                //failure,  revert back
                                                paramsList = new List<SqlParameter>();
                                                paramsList.Add(new SqlParameter("email", email));
                                                paramsList.Add(new SqlParameter("newPwdHash", oldPwdHash));
                                                ret = seedDataAccess.ExecuteNonQuery(SqlResource.NewPasswordHash, paramsList);
                                                throw ex;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (success)
                    {
                        result.status = 200;
                        result.changedPwdHash = true;
                    }
                    else
                    {
                        Util.SetError(result, 405, Resources.ErrGenericError, Resources.MessGenericError);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region NewPasswordAsync
        public async Task<object> NewPasswordAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            string oldPwdHash = string.Empty;
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    bool success = false;

                    string alph = Guid.NewGuid().ToString().Substring(0, 8);
                    string hash = Util.GetMd5Hash(alph);
                    if (objDictionary.ContainsKey("data"))
                    {
                        dynamic emailObject = (dynamic)objDictionary["data"];
                        string email = emailObject.to;
                        emailObject.htmlBody = emailObject.htmlBody.Replace("@pwd", alph);
                        if (!string.IsNullOrEmpty(email))
                        {
                            {
                                List<SqlParameter> paramsList = new List<SqlParameter>();
                                paramsList.Add(new SqlParameter("email", email));
                                oldPwdHash = seedDataAccess.ExecuteScalarAsString(SqlResource.GetPwdHash, paramsList);
                                if (!string.IsNullOrEmpty(oldPwdHash)) // email exists in database. Now change password Hash
                                {
                                    paramsList = new List<SqlParameter>();
                                    paramsList.Add(new SqlParameter("email", email));
                                    paramsList.Add(new SqlParameter("newPwdHash", hash));
                                    int ret = seedDataAccess.ExecuteNonQuery(SqlResource.NewPasswordHash, paramsList);
                                    if (ret > 0)
                                    {
                                        MailItem item = new MailItem()
                                        {
                                            From = emailObject.fromUser,
                                            FromName = emailObject.fromUserName,
                                            Host = emailObject.host,
                                            IsBodyHtml = true,
                                            Body = emailObject.htmlBody,
                                            Password = emailObject.fromUserPassword,
                                            Port = emailObject.port,
                                            Subject = emailObject.subject,
                                            To = emailObject.to
                                        };
                                        try
                                        {
                                            Util.SendMail(item);
                                            success = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            //revert back
                                            paramsList = new List<SqlParameter>();
                                            paramsList.Add(new SqlParameter("email", email));
                                            paramsList.Add(new SqlParameter("newPwdHash", oldPwdHash));
                                            ret = seedDataAccess.ExecuteNonQuery(SqlResource.NewPasswordHash, paramsList);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (success)
                    {
                        result.status = 200;
                        result.changedPwdHash = true;
                    }
                    else
                    {
                        Util.SetError(result, 405, Resources.ErrGenericError, Resources.MessGenericError);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region CreateAccountAsync
        public async Task<object> CreateAccountAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    bool success = false;

                    if (objDictionary.ContainsKey("account"))
                    {
                        dynamic account = (dynamic)objDictionary["account"];
                        var email = account.email;
                        var hash = account.hash;
                        List<SqlParameter> paramsList = new List<SqlParameter>();
                        paramsList.Add(new SqlParameter("email", email));
                        var isExist = seedDataAccess.ExecuteScalar(SqlResource.IsEmailExist, paramsList);
                        if (isExist == null)
                        {
                            dynamic user = new ExpandoObject();
                            user.Email = email;
                            user.PwdHash = hash;
                            user.Role = "u";
                            Dictionary<string, object> userDict = TBSeed.SeedUtil.GetDictFromDynamicObject(user);
                            var seed = new Seed()
                            {
                                TableName = "UserMaster",
                                TableDict = userDict,
                                IsCustomIDGenerated = false,
                                PKeyColName = "Id"
                            };
                            List<Seed> seeds = new List<Seed>();
                            seeds.Add(seed);
                            seedDataAccess.SaveSeeds(seeds);
                            success = true;
                        }
                        else
                        {
                            Exception exception = new Exception();
                            exception.Data.Add("403", Resources.ErrEmailAlreadyExists);
                            throw exception;
                        }
                    }
                    if (success)
                    {
                        result.status = 200;
                        result.changedPwdHash = true;
                    }
                    else
                    {
                        Util.SetError(result, 405, Resources.ErrGenericError, Resources.MessGenericError);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    if (ex.Data.Keys.Count > 0)
                    {
                        var entryList = ex.Data.Cast<DictionaryEntry>();
                        int errorNo = 0;
                        int.TryParse(entryList.ElementAt(0).Key.ToString(), out errorNo);
                        if (errorNo == 0)
                        {
                            errorNo = 520;
                        }
                        //int errorNo = int.Parse(entryList.ElementAt(0).Key.ToString());
                        string errorMessage = entryList.ElementAt(0).Value.ToString();
                        Util.SetError(result, errorNo, errorMessage, errorMessage);
                    }
                    else
                    {
                        Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                    }
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region ExecuteSqlQueryAsync
        private async Task<object> ExecuteSqlQueryAsync(dynamic obj)
        {
            dynamic results = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    string sqlKey = objDictionary["sqlKey"].ToString();
                    IDictionary<string, object> parmsDict = (IDictionary<string, object>)objDictionary["sqlParms"];
                    List<SqlParameter> paramsList = new List<SqlParameter>();
                    parmsDict.ToList<KeyValuePair<string, object>>().ForEach(x =>
                    {
                        paramsList.Add(new SqlParameter(x.Key, x.Value));
                    });
                    string sql = SqlResource.ResourceManager.GetString(sqlKey);
                    DataSet ds = new DataSet();
                    ds = seedDataAccess.ExecuteDataSet(sql, paramsList);
                    results = JsonConvert.SerializeObject(ds);
                    ds.Dispose();
                }
                catch (Exception ex)
                {
                    Util.SetError(results, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (results);
            });
            results = await t;
            return (results);
        }
        #endregion

        #region SaveOrderAsync
        public async Task<object> SaveOrderAsync(dynamic obj)
        {

            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;

                    if (objDictionary.ContainsKey("order") && (objDictionary.ContainsKey("email")))
                    {
                        string email = objDictionary["email"].ToString();
                        dynamic order = objDictionary["order"];
                        List<Seed> saveOrderSeedList = Util.GetSaveOrderSeedList(seedDataAccess, email, order);
                        seedDataAccess.SaveSeeds(saveOrderSeedList);
                        result.status = 200;
                        result.saveOrder = true;
                    }
                    else
                    {
                        Util.SetError(result, 406, Resources.ErrInputDataWrong, Resources.ErrInputDataWrong);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region UpdateOrInsertProfileAsync
        public async Task<object> UpdateOrInsertProfileAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;

                    if (objDictionary.ContainsKey("profile") && (objDictionary.ContainsKey("email")))
                    {
                        string email = objDictionary["email"].ToString();
                        dynamic profile = objDictionary["profile"];
                        bool isUpdate = (bool)objDictionary["isUpdate"];
                        if (isUpdate)
                        {
                            List<SqlParameter> parms = new List<SqlParameter>();
                            parms.Add(new SqlParameter("firstName", profile.firstName));
                            parms.Add(new SqlParameter("lastName", profile.lastName));
                            parms.Add(new SqlParameter("phone", profile.phone));
                            parms.Add(new SqlParameter("birthDay", profile.birthDay));
                            parms.Add(new SqlParameter("mailingAddress1", profile.mailingAddress1));
                            parms.Add(new SqlParameter("mailingAddress2", profile.mailingAddress2));
                            parms.Add(new SqlParameter("mailingCity", profile.mailingCity));
                            parms.Add(new SqlParameter("mailingState", profile.mailingState));
                            parms.Add(new SqlParameter("mailingZip", profile.mailingZip));
                            parms.Add(new SqlParameter("id", profile.id));
                            var i = seedDataAccess.ExecuteNonQuery(SqlResource.UpdateProfile, parms);
                            if (i == 0)
                            {
                                Exception exception = new Exception();
                                exception.Data.Add("501", Resources.ErrUpdateError);
                                throw exception;
                            }
                        }
                        else
                        {
                            profile.UserId = Util.GetUserIdFromEmail(seedDataAccess, email);
                            List<Seed> saveProfileSeedList = new List<Seed>();
                            Seed seed = new Seed()
                            {
                                PKeyColName = "Id",
                                IsCustomIDGenerated = false,
                                TableName = "UserProfiles",
                                TableDict = SeedUtil.GetDictFromDynamicObject(profile)
                            };
                            saveProfileSeedList.Add(seed);
                            seedDataAccess.SaveSeeds(saveProfileSeedList);
                        }
                        result.status = 200;
                        result.success = true;
                    }
                    else
                    {
                        Util.SetError(result, 406, Resources.ErrInputDataWrong, Resources.ErrInputDataWrong);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    if (ex.Data.Keys.Count > 0)
                    {
                        var entryList = ex.Data.Cast<DictionaryEntry>();
                        int errorNo = 0;
                        int.TryParse(entryList.ElementAt(0).Key.ToString(), out errorNo);
                        if (errorNo == 0)
                        {
                            errorNo = 520;
                        }
                        //int errorNo = int.Parse(entryList.ElementAt(0).Key.ToString());
                        string errorMessage = entryList.ElementAt(0).Value.ToString();
                        Util.SetError(result, errorNo, errorMessage, errorMessage);
                    }
                    else
                    {
                        Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                    }
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region UpdateOrInsertAddressesAsync
        public async Task<object> UpdateOrInsertAddressesAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;

                    if (objDictionary.ContainsKey("addresses") && (objDictionary.ContainsKey("email")))
                    {
                        string email = objDictionary["email"].ToString();
                        dynamic addresses = objDictionary["addresses"];
                        foreach (var address in addresses)
                        {
                            IDictionary<string, object> dict = SeedUtil.GetDictFromDynamicObject(address);
                            if (dict.ContainsKey("id"))
                            {
                                List<SqlParameter> parms = new List<SqlParameter>();
                                parms.Add(new SqlParameter("address1", address.address1));
                                parms.Add(new SqlParameter("zip", address.zip));
                                parms.Add(new SqlParameter("city", address.city));
                                parms.Add(new SqlParameter("street", address.street));
                                parms.Add(new SqlParameter("isDefault", address.isDefault));
                                parms.Add(new SqlParameter("id", address.id));
                                var i = seedDataAccess.ExecuteNonQuery(SqlResource.UpdateAddress, parms);
                                if (i == 0)
                                {
                                    Exception exception = new Exception();
                                    exception.Data.Add("501", Resources.ErrUpdateError);
                                    throw exception;
                                }
                            }
                            else
                            {
                                dict["userId"] = Util.GetUserIdFromEmail(seedDataAccess, email);
                                dict.Remove("isEdit");
                                dict.Remove("isDirty");
                                dict.Remove("isNew");
                                List<Seed> saveProfileSeedList = new List<Seed>();
                                Seed seed = new Seed()
                                {
                                    PKeyColName = "Id",
                                    IsCustomIDGenerated = false,
                                    TableName = "ShippingAddresses",
                                    TableDict = dict
                                };
                                saveProfileSeedList.Add(seed);
                                seedDataAccess.SaveSeeds(saveProfileSeedList);
                            }
                        }
                        result.status = 200;
                        result.success = true;
                    }
                    else
                    {
                        Util.SetError(result, 406, Resources.ErrInputDataWrong, Resources.ErrInputDataWrong);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 520, Resources.ErrGenericError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region ExecuteSqlNonQueryAsync
        public async Task<object> ExecuteSqlNonQueryAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    string sqlKey = objDictionary["sqlKey"].ToString();
                    IDictionary<string, object> parmsDict = (IDictionary<string, object>)objDictionary["sqlParms"];
                    List<SqlParameter> paramsList = new List<SqlParameter>();
                    parmsDict.ToList<KeyValuePair<string, object>>().ForEach(x =>
                    {
                        paramsList.Add(new SqlParameter(x.Key, x.Value));
                    });
                    string sql = SqlResource.ResourceManager.GetString(sqlKey);
                    int i = seedDataAccess.ExecuteNonQuery(sql, paramsList);
                    if (i > 0)
                    {
                        result.status = 200;
                        result.success = true;
                    }
                    else
                    {
                        Util.SetError(result, 520, Resources.ErrGenericError, Resources.MessGenericError);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region ExecuteScalarAsync
        public async Task<object> ExecuteScalarAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    string sqlKey = objDictionary["sqlKey"].ToString();
                    IDictionary<string, object> parmsDict = (IDictionary<string, object>)objDictionary["sqlParms"];
                    List<SqlParameter> paramsList = new List<SqlParameter>();
                    parmsDict.ToList<KeyValuePair<string, object>>().ForEach(x =>
                    {
                        paramsList.Add(new SqlParameter(x.Key, x.Value));
                    });
                    string sql = SqlResource.ResourceManager.GetString(sqlKey);
                    var res = seedDataAccess.ExecuteScalarAsString(sql, paramsList);
                    if (string.IsNullOrEmpty(res))
                    {
                        Util.SetError(result, 520, Resources.ErrGenericError, Resources.MessGenericError);
                    }
                    else
                    {
                        result.status = 200;
                        result.success = true;
                        result.result = res;
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region InsertCreditCardAsync
        public async Task<object> InsertCreditCardAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;

                    if (objDictionary.ContainsKey("card") && (objDictionary.ContainsKey("email")))
                    {
                        string email = objDictionary["email"].ToString();
                        dynamic card = objDictionary["card"];
                        List<Seed> seedList = new List<Seed>();

                        IDictionary<string, object> dict = SeedUtil.GetDictFromDynamicObject(card);
                        dict["userId"] = Util.GetUserIdFromEmail(seedDataAccess, email);
                        dict.Remove("isNew");
                        Seed seed = new Seed()
                        {
                            PKeyColName = "Id",
                            PKeyTagName = "Card",
                            IsCustomIDGenerated = false,
                            TableName = "CreditCards",
                            TableDict = dict
                        };
                        object id = null;
                        Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> action = (d1, d2, s) =>
                        {
                            id = d2["Card"];
                        };
                        seed.PostSaveAction = action;                       
                        seedList.Add(seed);

                        seedDataAccess.SaveSeeds(seedList);
                        result.status = 200;
                        result.success = true;
                        result.id = id;
                    }
                    else
                    {
                        Util.SetError(result, 406, Resources.ErrInputDataWrong, Resources.ErrInputDataWrong);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion

        #region EmptyMethodAsync
        public async Task<object> EmptyMethodAsync(dynamic obj)
        {
            dynamic result = new ExpandoObject();
            Task<object> t = Task.Run<object>(() =>
            {
                try
                {
                    IDictionary<string, object> objDictionary = (IDictionary<string, object>)obj;
                    bool success = false;

                    if (objDictionary.ContainsKey("data"))
                    {
                        dynamic emailObject = (dynamic)objDictionary["data"];
                    }
                    if (success)
                    {
                        result.status = 200;
                        result.changedPwdHash = true;
                    }
                    else
                    {
                        Util.SetError(result, 405, Resources.ErrGenericError, Resources.MessGenericError);
                    }
                }
                catch (Exception ex)
                {
                    result = new ExpandoObject();
                    Util.SetError(result, 500, Resources.ErrInternalServerError, ex.Message);
                }
                return (result);
            });
            result = await t;
            return (result);
        }
        #endregion
    }
}
