using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Configuration;
namespace TBUtility
{
    public class GXModel
    {
        private static DataAccess GXDataAccessProperty;
        public static DataAccess GXDataAccess
        {
            get
            {
                if (GXDataAccessProperty == null)
                {
                    GXDataAccessProperty = new DataAccess(GetConnectionString());
                }
                return GXDataAccessProperty;
            }
            set
            {
                GXDataAccessProperty = value;
            }
        }

        static GXModel()
        {
            GXDataAccess = new DataAccess(GetConnectionString());
        }

        public static string GetConnectionString()
        {
            //return (ConfigurationManager.ConnectionStrings["TPUltimate"].ToString());
            String connString = "Server=tcp:zye0dz8stg.database.windows.net,1433;Database=WorkforceQA;User ID=netwovensa;Password=Reallongpass1;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
            return (connString);
        }

        //public static DataTable GetDataTable(string sql, string databaseTable, Dictionary<string, string> parameters = null)
        //{
        //    using (DataSet dataSet = new DataSet())
        //    {
        //        List<SqlParameter> parameterList = new List<SqlParameter>();
        //        if (parameters != null)
        //            parameters.ToList<KeyValuePair<string, string>>().ForEach(x => GXDataAccess.AddInputParameter(parameterList, x.Key, x.Value));
        //        GXDataAccess.LoadDataSet(false, sql, parameterList, dataSet, new string[] { databaseTable });
        //        return (dataSet.Tables[0]);
        //    }
        //}

        //public static DataTable GetDataTable(string sql)
        //{
        //    using (DataSet dataSet = new DataSet())
        //    {
        //        GXDataAccess.LoadDataSet(sql, dataSet, new string[] { "MyTable" });
        //        return (dataSet.Tables[0]);
        //    }
        //}
        //public static void MergeFixedData(List<string> sqlList)
        //{
        //    sqlList.ForEach
        //        (
        //        x =>
        //        {
        //            GXDataAccess.ExecuteScalar(x);
        //        }
        //        );
        //}
        //public static void SaveAlteredRowsInDataTable(DataTable dataTable, List<string> deletedIDList, Action<XElement> f = null)
        //{
        //    var dataRows = dataTable.Rows.Cast<DataRow>().Where(x => (x.RowState != DataRowState.Unchanged));

        //    XElement xElement = new XElement("root");
        //    if (f != null)
        //    {
        //        f.Invoke(xElement);
        //    }
        //    if (deletedIDList.Count > 0)
        //    {
        //        deletedIDList.ForEach
        //            (
        //                x =>
        //                {
        //                    if (!(string.IsNullOrEmpty(x) || x.Equals("0")))
        //                    {
        //                        XUtility.AddXMLForDelete(xElement, dataTable.TableName, string.Format("{0}{1}", "ID=", x));
        //                    }
        //                }
        //            );
        //    }
        //    int columnCount = dataTable.Columns.Count;
        //    dataRows.Where(x =>
        //            (x.RowState.Equals(DataRowState.Added) || x.RowState.Equals(DataRowState.Modified)))
        //                .ToList<DataRow>()
        //                .ForEach(x =>
        //                {
        //                    List<ColumnNameValue> parameterList = new List<ColumnNameValue>();
        //                    for (int i = 1; i < columnCount; i++)
        //                    {
        //                        parameterList.Add(new ColumnNameValue(x.Table.Columns[i].ColumnName, x[i].ToString()));
        //                    }
        //                    if (x.RowState == DataRowState.Added)
        //                    {
        //                        XUtility.AddXmlForInsert(xElement, dataTable.TableName, "ID", null, parameterList);
        //                    }
        //                    else if (x.RowState == DataRowState.Modified)
        //                    {
        //                        XUtility.AddXmlForUpdate(xElement, dataTable.TableName, parameterList, string.Format("{0}{1}", "ID=", x[0]));
        //                    }
        //                });
        //    XUtility.SaveXmlData(GetConnectionString(), xElement.ToString());
        //}


        //private static List<SqlParameter> CreateParameters(int companyID, int yearID, int monthID)
        //{
        //    List<SqlParameter> parameterList = new List<SqlParameter>();
        //    GXDataAccess.AddInputParameter(parameterList, "CompanyID", companyID);
        //    GXDataAccess.AddInputParameter(parameterList, "YearID", yearID);
        //    GXDataAccess.AddInputParameter(parameterList, "MonthID", monthID);
        //    return parameterList;
        //}

        //private static List<SqlParameter> GetParms(string[] parmNames, object[] parmValues)
        //{
        //    List<SqlParameter> parameterList = new List<SqlParameter>();
        //    int count = parmNames.GetLength(0);
        //    for (int i = 0; i < count; i++)
        //    {
        //        parameterList.Add(new SqlParameter(parmNames.GetValue(i).ToString(), parmValues.GetValue(i)));
        //    }
        //    return (parameterList);
        //}
    }
}
