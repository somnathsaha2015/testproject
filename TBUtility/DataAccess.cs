using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
namespace TBUtility
{
  public class DataAccess : IDisposable
  {

    #region Variables and Constructors
    private Database db;
    private string connectionString;
    public DataAccess(string sConn)
    {
      connectionString = sConn;
      db = new SqlDatabase(sConn);
      //DatabaseProviderFactory fac = new DatabaseProviderFactory();


      //DatabaseFactory.SetDatabaseProviderFactory(fac);
      //db = DatabaseFactory.CreateDatabase();
    }

    public Database GetDB()
    {
      return (db);
    }

    public string GetConnectionString()
    {
      return (connectionString);
    }
    #endregion

    public DbConnection GetConnection()
    {

      DbConnection conn = db.CreateConnection();
      return (conn);
    }

    #region GetScalerValueAsString
    public string GetScalerValueAsString(string sql, List<SqlParameter> parameterList)
    {
      string ret;
      object obj = ExecuteScalar(false, sql, parameterList);
      if (obj == null)
        ret = string.Empty;
      else
        ret = obj.ToString();
      return (ret);
    }
    #endregion

    #region GetSqlResultAsListOfObjects
    public List<T> GetSqlResultAsListOfObjects<T>(string sql, List<SqlParameter> parameterList)
    {
      List<T> outputList = new List<T>();
      dynamic objectValue = 0;
      using (DataSet ds = new DataSet())
      {
        ExecuteDataSet(false, sql, parameterList);
        DataTable dt = ds.Tables[0];
        DataColumnCollection columnCollection = dt.Columns;
        dt.AsEnumerable().ToList<DataRow>().ForEach(x =>
        {
          dynamic myObject = typeof(T).GetConstructor(System.Type.EmptyTypes).Invoke(null);
                  //invoke constructor to get an instance of object
                  columnCollection.OfType<DataColumn>().ToList<DataColumn>().ForEach(y =>
                  {
              PropertyInfo myObjectPropertyInfo = typeof(T).GetProperty(y.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
              if (myObjectPropertyInfo != null)
              {

                var customAttributes = myObjectPropertyInfo.GetCustomAttributes(false);
                if (customAttributes.Count() > 0)
                {
                  if (customAttributes.Any(z => z.GetType().Name.Equals("PrimaryKey", StringComparison.OrdinalIgnoreCase)))
                  {
                    if (x[y] == System.DBNull.Value)
                    {
                      objectValue = 0;
                    }
                    else
                    {
                      objectValue = x[y];
                    }
                  }
                  else
                  {
                    if (x[y] == DBNull.Value)
                    {
                      objectValue = null;
                    }
                    else
                    {
                      objectValue = x[y];
                    }
                  }
                }
                else
                {
                  if (x[y] == DBNull.Value)
                  {
                    objectValue = null;
                  }
                  else
                  {
                    objectValue = x[y];
                  }
                }

                myObjectPropertyInfo.SetValue(myObject, objectValue, null); //sets the property values of the created object
                      }

            });
          outputList.Add((T)myObject);
        });
      }
      return (outputList);
    }
    #endregion

    #region GetSqlResultAsListOfListOfObjects
    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }

    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2, T3>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      typeDictionary.Add(3, typeof(T3));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }

    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2, T3, T4>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      typeDictionary.Add(3, typeof(T3));
      typeDictionary.Add(4, typeof(T4));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }

    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2, T3, T4, T5>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      typeDictionary.Add(3, typeof(T3));
      typeDictionary.Add(4, typeof(T4));
      typeDictionary.Add(5, typeof(T5));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }

    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2, T3, T4, T5, T6>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      typeDictionary.Add(3, typeof(T3));
      typeDictionary.Add(4, typeof(T4));
      typeDictionary.Add(5, typeof(T5));
      typeDictionary.Add(6, typeof(T6));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }

    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2, T3, T4, T5, T6, T7>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      typeDictionary.Add(3, typeof(T3));
      typeDictionary.Add(4, typeof(T4));
      typeDictionary.Add(5, typeof(T5));
      typeDictionary.Add(6, typeof(T6));
      typeDictionary.Add(7, typeof(T7));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }
    public List<List<dynamic>> GetSqlResultAsListOfListOfObjects<T1, T2, T3, T4, T5, T6, T7, T8>(string sql, List<SqlParameter> parameterList)
    {
      List<List<dynamic>> FinalOutputListOfList = new List<List<dynamic>>();
      Dictionary<int, Type> typeDictionary = new Dictionary<int, Type>();
      typeDictionary.Add(1, typeof(T1));
      typeDictionary.Add(2, typeof(T2));
      typeDictionary.Add(3, typeof(T3));
      typeDictionary.Add(4, typeof(T4));
      typeDictionary.Add(5, typeof(T5));
      typeDictionary.Add(6, typeof(T6));
      typeDictionary.Add(7, typeof(T7));
      typeDictionary.Add(8, typeof(T8));
      LoadObject(sql, parameterList, FinalOutputListOfList, typeDictionary);
      return (FinalOutputListOfList);
    }
    #endregion

    #region LoadObject
    private void LoadObject(string sql, List<SqlParameter> parameterList, List<List<dynamic>> FinalOutputListOfList, Dictionary<int, Type> typeDictionary)
    {
      using (DataSet ds = new DataSet())
      {
        ExecuteDataSet(false, sql, parameterList);
        int count = ds.Tables.Count;
        for (int i = 1; i <= count; i++)
        {
          DataTable dt = ds.Tables[i - 1];
          DataColumnCollection columnCollection = dt.Columns;
          List<dynamic> outputList = new List<dynamic>();
          dt.AsEnumerable().ToList<DataRow>().ForEach(x =>
          {
                      //invoke constructor to get an instance of object
                      dynamic myObject = typeDictionary[i].GetConstructor(System.Type.EmptyTypes).Invoke(null);

            columnCollection.OfType<DataColumn>().ToList<DataColumn>().ForEach(y =>
                      {

                PropertyInfo myObjectPropertyInfo = typeDictionary[i].GetProperty(y.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (myObjectPropertyInfo != null)
                {
                  myObjectPropertyInfo.SetValue(myObject, x[y] == DBNull.Value ? null : x[y], null); //sets the property values of the created object
                          }
              });
            outputList.Add(myObject);
          });
          FinalOutputListOfList.Add(outputList);
        }
      }
    }
    #endregion

    #region GetSqlResultAsObject
    public T GetSqlResultAsObject<T>(string sql, List<SqlParameter> parameterList)
    {
      T output = default(T);
      using (DataSet ds = new DataSet())
      {
        ExecuteDataSet(false, sql, parameterList);
        DataTable dt = ds.Tables[0];
        DataColumnCollection columnCollection = dt.Columns;
        if (dt.Rows.Count == 1)
        {
          DataRow dr = dt.Rows[0];
          dynamic myObject = typeof(T).GetConstructor(System.Type.EmptyTypes).Invoke(null);
          //invoke constructor to get an instance of object
          columnCollection.OfType<DataColumn>().ToList<DataColumn>().ForEach(y =>
          {
            PropertyInfo myObjectPropertyInfo = typeof(T).GetProperty(y.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (myObjectPropertyInfo != null)
            {
              myObjectPropertyInfo.SetValue(myObject, dr[y] == DBNull.Value ? null : dr[y], null); //sets the property values of the created object   
                      }
          });
          output = (T)myObject;
        }
      }
      return (output);
    }
    #endregion

    #region ExecuteNonQuery

    //No parameters only Sql Command string
    public int ExecuteNonQuery(string sCommandText)
    {
      int iRowsAffected = db.ExecuteNonQuery(CommandType.Text, sCommandText);
      return iRowsAffected;
    }

    //List of parameters and stored procedure
    public int ExecuteNonQuery(string spName, IList<SqlParameter> sqlParams)
    {
      if (String.IsNullOrEmpty(spName))
        throw new ArgumentException("Stored procedure name cannot be null or empty string.");
      int iRowsAffected = 0;
      using (DbCommand dbCommand = db.GetStoredProcCommand(spName))
      {
        dbCommand.CommandTimeout = 600;
        foreach (SqlParameter sqlParameter in sqlParams)
          dbCommand.Parameters.Add(sqlParameter);
        iRowsAffected = db.ExecuteNonQuery(dbCommand);
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in dbCommand.Parameters)
          sqlParams.Add(sqlParameter);
      }
      return iRowsAffected;
    }

    //No parameters and choice between stored procedure and simple command text
    public int ExecuteNonQuery(bool IsStoredProc, string sCommandText)
    {
      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Sql Command Name cannot be null or empty string.");
      int iRowsAffected = 0;
      List<SqlParameter> list = new List<SqlParameter>();
      if (IsStoredProc)
        iRowsAffected = ExecuteNonQuery(sCommandText, list);
      else
        iRowsAffected = ExecuteNonQuery(sCommandText);

      return (iRowsAffected);
    }

    //Most versatile. Choice between stored procedure and Sql Command Text along with list of parameters
    public int ExecuteNonQuery(bool isStoredProc, string sCommandText,
            IList<SqlParameter> sqlParams)
    {
      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Command name cannot be null or empty string.");
      int iRowsAffected = 0;
      if (isStoredProc)
        iRowsAffected = ExecuteNonQuery(sCommandText, sqlParams);
      else
      {
        using (DbCommand dbCommand = db.GetSqlStringCommand(sCommandText))
        {
          dbCommand.CommandTimeout = 600;
          foreach (SqlParameter sqlParameter in sqlParams)
            dbCommand.Parameters.Add(sqlParameter);
          iRowsAffected = db.ExecuteNonQuery(dbCommand);
          sqlParams.Clear();
          foreach (SqlParameter sqlParameter in dbCommand.Parameters)
            sqlParams.Add(sqlParameter);
        }
      }
      return iRowsAffected;
    }


    #endregion

    #region ExecuteReader

    //simple datareader from a sql command
    public IDataReader ExecuteReader(string sCommandText)
    {
      IDataReader dataReader = db.ExecuteReader(CommandType.Text, sCommandText);
      return dataReader;
    }

    //DataReader from stored proc and parameter list
    public IDataReader ExecuteReader(string spName, IList<SqlParameter> sqlParams)
    {

      if (String.IsNullOrEmpty(spName))
        throw new ArgumentException("Stored procedure name cannot be null or empty string.");

      IDataReader dataReader = null;
      using (DbCommand dbCommand = db.GetStoredProcCommand(spName))
      {

        foreach (SqlParameter sqlParameter in sqlParams)
          dbCommand.Parameters.Add(sqlParameter);
        dataReader = db.ExecuteReader(dbCommand);
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in dbCommand.Parameters)
          sqlParams.Add(sqlParameter);
      }
      return dataReader;
    }

    //Most Versatile. DataReader from a choice of Stored procedure and command text with parameter list
    public IDataReader ExecuteReader(bool IsStoredProc,
        string sCommandText, IList<SqlParameter> sqlParams)
    {
      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Command name cannot be null or empty string.");

      IDataReader dataReader = null;
      if (IsStoredProc)
        dataReader = ExecuteReader(sCommandText, sqlParams);
      else
      {
        using (DbCommand dbCommand = db.GetSqlStringCommand(sCommandText))
        {
          foreach (SqlParameter sqlParameter in sqlParams)
            dbCommand.Parameters.Add(sqlParameter);
          dataReader = db.ExecuteReader(dbCommand);
          sqlParams.Clear();
          foreach (SqlParameter sqlParameter in dbCommand.Parameters)
            sqlParams.Add(sqlParameter);
        }
      }
      return dataReader;
    }
    #endregion

    #region ExecuteScalar
    //simple Execute Scalar
    public Object ExecuteScalar(string commandText)
    {
      Object result = db.ExecuteScalar(CommandType.Text, commandText);
      return result;
    }

    public Object ExecuteScalar(string commandText, DbTransaction tran)
    {
      Object result = db.ExecuteScalar(tran, CommandType.Text, commandText);
      return result;
    }
    //Execute Scalar with stored proc and parameters
    public Object ExecuteScalar(string spName, IList<SqlParameter> sqlParams)
    {

      if (String.IsNullOrEmpty(spName))
        throw new ArgumentException("Stored procedure name cannot be null or empty string.");

      Object result = null;
      using (DbCommand dbCommand = db.GetStoredProcCommand(spName))
      {

        foreach (SqlParameter sqlParameter in sqlParams)
          dbCommand.Parameters.Add(sqlParameter);
        result = db.ExecuteScalar(dbCommand);
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in dbCommand.Parameters)
          sqlParams.Add(sqlParameter);

      }
      return result;
    }

    //Most Versatile. With Choice of stored proc and Command Text and with Parameters
    public Object ExecuteScalar(bool isStoredProc, string sCommandText, IList<SqlParameter> sqlParams)
    {

      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Command name cannot be null or empty string.");

      Object result = null;
      if (isStoredProc)
        ExecuteScalar(sCommandText, sqlParams);
      using (DbCommand dbCommand = db.GetSqlStringCommand(sCommandText))
      {
        if (sqlParams != null)
        {
          foreach (SqlParameter sqlParameter in sqlParams)
            dbCommand.Parameters.Add(sqlParameter);
        }

        result = db.ExecuteScalar(dbCommand);
        if (sqlParams != null)
        {
          sqlParams.Clear();
          foreach (SqlParameter sqlParameter in dbCommand.Parameters)
            sqlParams.Add(sqlParameter);
        }
      }
      return result;
    }
    #endregion

    #region ExecuteDataSet

    //Basic
    public DataSet ExecuteDataSet(string sCommandText)
    {
      DataSet dataSet = db.ExecuteDataSet(CommandType.Text, sCommandText);
      return dataSet;
    }

    //Stored procedure with parameters
    public DataSet ExecuteDataSet(string spName, IList<SqlParameter> sqlParams)
    {

      if (String.IsNullOrEmpty(spName))
        throw new ArgumentException("Stored procedure name cannot be null or empty string.");

      DataSet dataSet = null;
      using (DbCommand dbCommand = db.GetStoredProcCommand(spName))
      {
        foreach (SqlParameter sqlParameter in sqlParams)
          dbCommand.Parameters.Add(sqlParameter);
        dataSet = db.ExecuteDataSet(dbCommand);
        //assign the output parameter values if any.
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in dbCommand.Parameters)
          sqlParams.Add(sqlParameter);
      }
      return dataSet;
    }

    //Most Vesatile
    public DataSet ExecuteDataSet(bool IsStoredProc,
            string sCommandText, IList<SqlParameter> sqlParams)
    {
      DataSet dataSet;
      if (IsStoredProc)
        dataSet = ExecuteDataSet(sCommandText, sqlParams);
      else
      {
        SqlCommand sqlCommand = (SqlCommand)db.GetSqlStringCommand(sCommandText);
        foreach (SqlParameter sqlParameter in sqlParams)
          sqlCommand.Parameters.Add(sqlParameter);
        dataSet = db.ExecuteDataSet(sqlCommand);
        //assign the output parameter values if any.
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in sqlCommand.Parameters)
          sqlParams.Add(sqlParameter);
      }
      return dataSet;
    }
    #endregion

    #region LoadDataSet
    //Load the table array of a dataset with data.

    //Basic. Given a dataset and sql command string without parameters. Load the tableName array
    public void LoadDataSet(string sCommandText, DataSet dataset, string[] tableName)
    {
      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Sql Command Text cannot be null or empty string.");

      if (dataset == null)
        throw new ArgumentNullException("Dataset cannot be null.");

      if (tableName == null)
        throw new ArgumentException("Table names cannot be null.");

      using (DbCommand dbCommand = db.GetSqlStringCommand(sCommandText))
      {
        db.LoadDataSet(dbCommand, dataset, tableName);
      }
    }

    //Given Stored Proc Name and list of parameters
    public void LoadDataSet(string spName,
        IList<SqlParameter> sqlParams, DataSet dataset, string[] tableName)
    {
      if (String.IsNullOrEmpty(spName))
        throw new ArgumentException("Stored procedure name cannot be null or empty string.");

      if (dataset == null)
        throw new ArgumentNullException("Dataset cannot be null.");

      if (tableName == null)
        throw new ArgumentException("Table names cannot be null.");

      using (DbCommand dbCommand = db.GetStoredProcCommand(spName))
      {
        foreach (SqlParameter sqlParameter in sqlParams)
          dbCommand.Parameters.Add(sqlParameter);
        db.LoadDataSet(dbCommand, dataset, tableName);
        //assign the output parameter values if any.
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in dbCommand.Parameters)
          sqlParams.Add(sqlParameter);
      }
    }

    // Most Versatile
    public void LoadDataSet(bool isStoredProc, string sCommandText,
            IList<SqlParameter> sqlParams, DataSet dataset, string[] tableName)
    {
      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Command Text name cannot be null or empty string.");

      if (dataset == null)
        throw new ArgumentNullException("Dataset cannot be null.");

      if (tableName == null)
        throw new ArgumentException("Table names cannot be null.");

      DbCommand dbCommand;

      if (isStoredProc)
        LoadDataSet(sCommandText, sqlParams, dataset, tableName);
      else
      {
        dbCommand = db.GetSqlStringCommand(sCommandText);
        if (sqlParams != null)
        {
          foreach (SqlParameter sqlParameter in sqlParams)
            dbCommand.Parameters.Add(sqlParameter);
        }
        db.LoadDataSet(dbCommand, dataset, tableName);
        //assign the output parameter values if any.
        if (sqlParams != null)
        {
          sqlParams.Clear();
          foreach (SqlParameter sqlParameter in dbCommand.Parameters)
            sqlParams.Add(sqlParameter);
        }
        dbCommand = null;

      }

    }

    public void LoadDataSet(bool isStoredProc, string sCommandText,
            IList<KeyValuePair<string, string>> sqlParams, DataSet dataset, string[] tableName)
    {
      if (String.IsNullOrEmpty(sCommandText))
        throw new ArgumentException("Command Text name cannot be null or empty string.");

      if (dataset == null)
        throw new ArgumentNullException("Dataset cannot be null.");

      if (tableName == null)
        throw new ArgumentException("Table names cannot be null.");

      DbCommand dbCommand;

      List<SqlParameter> sqlParmList = new List<SqlParameter>();
      sqlParams.ToList().ForEach(x => sqlParmList.Add(new SqlParameter(x.Key, x.Value)));

      if (isStoredProc)
        LoadDataSet(sCommandText, sqlParmList, dataset, tableName);
      else
      {
        dbCommand = db.GetSqlStringCommand(sCommandText);
        foreach (SqlParameter sqlParameter in sqlParmList)
          dbCommand.Parameters.Add(sqlParameter);
        db.LoadDataSet(dbCommand, dataset, tableName);
        //assign the output parameter values if any.
        sqlParams.Clear();
        foreach (SqlParameter sqlParameter in dbCommand.Parameters)
          sqlParmList.Add(sqlParameter);
        dbCommand = null;

      }

    }
    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      db = null;
      GC.Collect();
    }

    #endregion

    #region AddInputParameter
    public void AddInputParameter(List<SqlParameter> list, string sParmName, object sParmValue)
    {
      if (sParmValue == null)
        sParmValue = DBNull.Value;
      list.Add(new SqlParameter(sParmName, sParmValue));
    }
    #endregion

  }
}