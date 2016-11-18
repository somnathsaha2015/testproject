using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json.Linq;

namespace TBSeed
{
  public enum HaveSameKeys
  {
    Equal = 0,
    SourceCanBeMore = 1,
    DestCanBeMore = 2
  }

  public class Seed
  {
    public dynamic TableObject { get; set; }
    public dynamic TableDict { get; set; }
    public string PKeyColName { get; set; }
    public string PKeyTagName { get; set; }
    public string TableName { get; set; }
    public List<KeyValuePair<string, string>> MasterTableColNameTagNamePairs { get; set; }
    public List<KeyValuePair<string, string>> DetailsTableColNameTagNamePairs { get; set; }
    public bool IsCustomIDGenerated { get; set; }
    public List<Tuple<string, Func<dynamic, string>, dynamic>> CustomIDGenerators { get; set; } //fieldName,IdGenerator method,Parameters as dynamic
    public Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> PreSaveAction { get; set; }
    public Action<Dictionary<string, object>, Dictionary<string, object>, List<Seed>> PostSaveAction { get; set; }
  }

  public class SeedDataAccess
  {
    #region Constructor
    Dictionary<string, object> keyVault;
    private Database db;
    private string _connectionString;
    public SeedDataAccess(string connectionString)
    {
      keyVault = new Dictionary<string, object>();
      db = new SqlDatabase(connectionString);
      _connectionString = connectionString;
    }
        #endregion

    #region GetDatabase
    public Database GetDatabase()
    {
        return (db);
    } 
    #endregion

    #region LoadDataSet
    public void LoadDataSet(string sqlText, DataSet ds, string[] tableNames)
{
    db.LoadDataSet(CommandType.Text, sqlText, ds, tableNames);
}

public void LoadDataSet(string sqlText, List<SqlParameter> parms, DataSet ds, string[] tableNames)
{
    DbCommand cmd = db.GetSqlStringCommand(sqlText);
    cmd.Parameters.AddRange(parms.ToArray<SqlParameter>());
    db.LoadDataSet(cmd, ds, tableNames);
}
#endregion

    #region ExecuteDataSet
    public DataSet ExecuteDataSet(string sqlText)
    {
      DataSet ds = db.ExecuteDataSet(CommandType.Text, sqlText);
      return (ds);
    }

    public DataSet ExecuteDataSet(string sqlText, List<SqlParameter> parms)
    {
      DbCommand cmd = db.GetSqlStringCommand(sqlText);
      cmd.Parameters.AddRange(parms.ToArray<SqlParameter>());
      DataSet ds = db.ExecuteDataSet(cmd);
      return (ds);
    }
    #endregion

    #region ExecuteScalar
    public Object ExecuteScalar(string sqlText)
    {
      Object obj = db.ExecuteScalar(CommandType.Text, sqlText);
      return (obj);
    }

    public Object ExecuteScalar(string sqlText, List<SqlParameter> parms)
    {
      DbCommand cmd = db.GetSqlStringCommand(sqlText);
      cmd.Parameters.AddRange(parms.ToArray<SqlParameter>());
      Object obj = db.ExecuteScalar(cmd);
      return (obj);
    }
        #endregion

    #region ExecuteScalarAsString
        public string ExecuteScalarAsString(string sqlText)
        {
            var ret = string.Empty;
            Object obj = db.ExecuteScalar(CommandType.Text, sqlText);
            if (obj != null)
            {
                ret = obj.ToString();
            }
            return (ret);
        }

        public string ExecuteScalarAsString(string sqlText, List<SqlParameter> parms)
        {
            var ret = string.Empty;
            DbCommand cmd = db.GetSqlStringCommand(sqlText);
            cmd.Parameters.AddRange(parms.ToArray<SqlParameter>());
            Object obj = db.ExecuteScalar(cmd);
            if(obj != null)
            {
                ret = obj.ToString();
            }
            return (ret);
        }
        #endregion

    #region ExecuteNonQuery
        public int ExecuteNonQuery(string sqlText)
    {
      int iRecords = db.ExecuteNonQuery(CommandType.Text, sqlText);
      return (iRecords);
    }

    public int ExecuteNonQuery(string sqlText, List<SqlParameter> parms)
    {
      DbCommand cmd = db.GetSqlStringCommand(sqlText);
      cmd.Parameters.AddRange(parms.ToArray<SqlParameter>());
      int iRecords = db.ExecuteNonQuery(cmd);
      return (iRecords);
    }
    #endregion

    #region SaveSeeds

    public void SaveSeeds(List<Seed> seedList, Action<List<Seed>, dynamic> preCommitAction = null, dynamic param = null)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        SaveSeedList(seedList);
        if (preCommitAction != null)
        {
          preCommitAction(seedList, param);
        }
        scope.Complete();
      }
    }

    private void SaveSeedList(List<Seed> seedList)
    {
      seedList.ForEach(x =>
      {
        Dictionary<string, object> seedDict;
        if (x.TableDict == null)
        {
          seedDict = SeedUtil.GetDictFromObject(x.TableObject);
        }
        else
        {
          seedDict = x.TableDict;
        }

        List<SqlParameter> parmsList = new List<SqlParameter>();

        if (x.DetailsTableColNameTagNamePairs != null)
        {
          x.DetailsTableColNameTagNamePairs.ForEach(y =>
          {
            seedDict[y.Key] = keyVault[y.Value];
          });
        }

        if (x.CustomIDGenerators != null)
        {
          x.CustomIDGenerators.ForEach((a) =>
          {
            string myID = a.Item2.Invoke(a.Item3);
            seedDict[a.Item1] = myID;
          });
        }

        if (x.PreSaveAction != null)
        {
          x.PreSaveAction(seedDict, keyVault, seedList);
        }
        string sql = GetInsertSql(x.IsCustomIDGenerated, x.PKeyColName, x.TableName, seedDict, out parmsList);

        DbCommand command = db.GetSqlStringCommand(sql);
        command.Parameters.AddRange(parmsList.ToArray<SqlParameter>());

        object id = db.ExecuteScalar(command);

        if (!string.IsNullOrEmpty(x.PKeyTagName))
        {
          keyVault[x.PKeyTagName] = id;
        }

        if (x.MasterTableColNameTagNamePairs != null)
        {
          x.MasterTableColNameTagNamePairs.ForEach(m =>
          {
            keyVault[m.Value] = seedDict[m.Key];
          });
        }
        if (x.PostSaveAction != null)
        {
          x.PostSaveAction(seedDict, keyVault, seedList);
        }
      });
    }

    #endregion

    #region GetInsertSql
    public string GetInsertSql(bool isCustomeIdentityGenerator, string pKeyColumnName, string tableName, Dictionary<string, object> inputDict, out List<SqlParameter> parms)
    {
      if (!string.IsNullOrEmpty(pKeyColumnName))
      {
        if (!isCustomeIdentityGenerator)
        {
          inputDict.Remove(pKeyColumnName);
        }
      }
      Tuple<string, string, List<SqlParameter>> columnNameValue = GetColumnNameValue(inputDict);
      string sql = SqlTemplate.IdentityInsertSql.Replace("@tableName", tableName)
          .Replace("@columnNames", columnNameValue.Item1)
          .Replace("@columnValues", columnNameValue.Item2)
          .Replace("@pKeyColumnName", pKeyColumnName);
      parms = columnNameValue.Item3;
      return (sql);
    }
    #endregion

    #region GetColumnNameValue
    private Tuple<string, string, List<SqlParameter>> GetColumnNameValue(Dictionary<string, object> inputDict)
    {
      List<SqlParameter> parms = new List<SqlParameter>();

      Dictionary<string, object> v = new Dictionary<string, object>();
      foreach (var k in inputDict)
      {
        if (k.Value != null)
        {
          v.Add(k.Key, k.Value);
          parms.Add(new SqlParameter(k.Key, k.Value));
        }
      }
      //var v = inputDict.Where(x => x.Value != null);
      //v.ToList<KeyValuePair<string, object>>().ForEach(x => { parms.Add(new SqlParameter(x.Key, x.Value)); });
      string columnNames = string.Join(", ", v.Select(x => x.Key));
      string columnValues = string.Join(", ", v.Select(x => $"@{x.Key}"));
      return (new Tuple<string, string, List<SqlParameter>>(columnNames, columnValues, parms));
    }
    #endregion
  }

  public class SeedUtil
  {

    #region GetDictFromClass
    public static Dictionary<string, object> GetDictFromClass(Type clazz)
    {
      object theObject = Activator.CreateInstance(clazz);
      return (GetDictFromObject(theObject));
    }
    #endregion

    #region CopyValuesFromSource
    public static void CopyValuesFromSource(Dictionary<string, object> source, Dictionary<string, object> destination)
    {
      source.ToList<KeyValuePair<string, object>>().ForEach(x =>
      {
        if (destination.ContainsKey(x.Key))
        {
          destination[x.Key] = x.Value;
        }
      });
    }
    #endregion

    #region HaveSameKeys
    public static bool HaveSameKeys(Dictionary<string, object> source, Dictionary<string, object> destination, TBSeed.HaveSameKeys checkKeyOption = TBSeed.HaveSameKeys.Equal)
    {
      bool ret = true;
      IEnumerable<KeyValuePair<string, object>> v = source.Except(destination);
      if (checkKeyOption == TBSeed.HaveSameKeys.Equal || checkKeyOption == TBSeed.HaveSameKeys.DestCanBeMore)
      {
        source.ToList().ForEach(x =>
        {
          if (!destination.ContainsKey(x.Key))
          {
            ret = false;
            return;
          }
        });
      }
      if (checkKeyOption == TBSeed.HaveSameKeys.Equal || checkKeyOption == TBSeed.HaveSameKeys.SourceCanBeMore)
      {
        if (ret)
        {
          destination.ToList().ForEach(x =>
          {
            if (!source.ContainsKey(x.Key))
            {
              ret = false;
              return;
            }
          });
        }
      }
      return (ret);
    }
    #endregion

    #region GetDictFromObject
    public static Dictionary<string, object> GetDictFromObject(object source, bool isIgnoreCase = false, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
    {
      Dictionary<string, object> dict = source.GetType().GetProperties(bindingAttr).ToDictionary
      (
          propInfo => propInfo.Name,
          propInfo => propInfo.GetValue(source, null)
      );

      if (isIgnoreCase)
      {
        dict = new Dictionary<string, object>(dict, StringComparer.OrdinalIgnoreCase);
      }
      return (dict);
    }

    public static Dictionary<string, object> GetDictFromDynamicObject(dynamic obj, bool isIgnoreCase = false)
    {
      JObject jObject = JObject.FromObject(obj);
      Dictionary<string, object> dict = jObject.ToObject<Dictionary<string, object>>();
      if (isIgnoreCase)
      {
        dict = new Dictionary<string, object>(dict, StringComparer.OrdinalIgnoreCase);
      }
      return (dict);
    }
    #endregion

  }
}

