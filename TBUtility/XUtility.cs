using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Data.SqlClient;
using System.Collections;

namespace TBUtility
{
    #region class ColumnNameValue
    public class ColumnNameValue
    {
        private readonly string sColumnName = string.Empty;
        private readonly string sColumnValue = string.Empty;
        private readonly bool bFKey;
        public ColumnNameValue(string sColName, string sColValue, bool bIsFKey)
        {
            sColumnName = sColName;
            sColumnValue = sColValue;
            bFKey = bIsFKey;
        }
        public ColumnNameValue(string sColName, string sColValue)
        {
            sColumnName = sColName;
            sColumnValue = sColValue;
        }
        public string ColumnName
        {
            get { return (sColumnName); }
        }
        public string ColumnValue
        {
            get { return (sColumnValue); }
        }
        public bool FKey
        {
            get { return (bFKey); }
        }
    }
    #endregion

    public class XUtility
    {
        #region private methods    

        private static string GetColumnName(XElement xElement)
        {
            StringBuilder sColumnName = new StringBuilder();
            foreach (XElement myXElement in xElement.Elements())
                sColumnName.Append(myXElement.Name.LocalName + ",");
            //remove last comma and return
            return (sColumnName.ToString().Remove(sColumnName.Length - 1));
        }

        private static string GetParm(XElement xElement)
        {
            StringBuilder sParmName = new StringBuilder();
            foreach (XElement myXElement in xElement.Elements())
                sParmName.Append(String.Format("@{0},", myXElement.Name.LocalName));
            //remove last comma and return
            return (sParmName.ToString().Remove(sParmName.Length - 1));
        }

        private static void AddParValue(XElement xElement, SqlCommand sqlCommand, System.Collections.Hashtable hashTable)
        {
            sqlCommand.Parameters.Clear();
            string sParmName = string.Empty;
            foreach (XElement xElementChild in xElement.Elements())
            {
                if (xElementChild.Attributes("FKey").Any())
                {
                    sqlCommand.Parameters.AddWithValue("@" + xElementChild.Name.LocalName, hashTable[xElementChild.Name.LocalName]);
                }
                else
                    if (xElementChild.Name.LocalName.ToLower() != "where")
                    {
                        sqlCommand.Parameters.AddWithValue("@" + xElementChild.Name.LocalName, xElementChild.Value);
                    }
            }
        }
        //overloaded method
        private static SqlCommand AddParValue(XElement xElement, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.Clear();
            string sParmName = string.Empty;
            foreach (XElement xElementChild in xElement.Elements())
            {
                if (xElementChild.Name.LocalName.ToLower() != "where")
                    sqlCommand.Parameters.AddWithValue("@" + xElementChild.Name.LocalName, xElementChild.Value);
            }
            return (sqlCommand);
        }
        private static string GetUpdateCommand(XElement xElement)
        {
            string sUpdateCommand = " set ";
            foreach (XElement xElementChild in xElement.Elements())
            {
                if (xElementChild.Name.LocalName.ToLower() != "where")
                {
                    sUpdateCommand += String.Format("{0}=@{0},", xElementChild.Name.LocalName);
                }
                else //process where clause
                {
                    //remove last comma
                    sUpdateCommand = sUpdateCommand.Remove(sUpdateCommand.Length - 1);
                    sUpdateCommand += " where " + xElementChild.Value;
                }
            }
            return (sUpdateCommand);
        }

        private static string GetDeleteCommand(XElement xElement)
        {
            string sDeleteCommand = " where " + xElement.Value;
            return (sDeleteCommand);

        }

        //Executes a stored procedure inside sql server
        private static void ExecuteSP(XElement xElement, SqlCommand sqlCommand, Hashtable hashTable)
        {
            //In case of stored procedures No ID values are stored back in the XML.
            //Keyname and FKey attributes are not removed.
            string sTargetName = string.Empty, sIDColumnName = string.Empty, sIDColumnValue = string.Empty;
            SqlParameter sqlParameter = null;
            //IDColumnName is the output parameter of stored procedure
            if (xElement.Attributes("IDColumnName").Any())
            {
                sqlParameter = new SqlParameter();
                sqlParameter.Direction = System.Data.ParameterDirection.Output;
                sqlParameter.Value = 0;
                sqlParameter.ParameterName = "@" + xElement.Attribute("IDColumnName").Value;
                sqlCommand.Parameters.Add(sqlParameter);
            }
            object oNull = DBNull.Value;
            foreach (XElement xElementChild in xElement.Elements())
            {
                if (xElementChild.Attributes("FKey").Any())
                    sqlCommand.Parameters.AddWithValue("@" + xElementChild.Name.LocalName, hashTable[xElementChild.Value]);
                else
                {
                    if (xElementChild.Value.Trim() == "")//null value
                        sqlCommand.Parameters.AddWithValue("@" + xElementChild.Name.LocalName, oNull);
                    else
                        sqlCommand.Parameters.AddWithValue("@" + xElementChild.Name.LocalName, xElementChild.Value);
                }
            }
            sqlCommand.ExecuteNonQuery();
            //add the KeyName and its value in hashTable if KeyName exists in XML
            if (xElement.Attributes("KeyName").Any())
            {
                sIDColumnValue = Convert.ToString(sqlCommand.Parameters["@" + sIDColumnName].Value);
                hashTable.Add(xElement.Attribute("KeyName").Value, sIDColumnValue);
            }
        }
        #endregion

        #region public methods
        
        public static void 
            SaveXmlData(string connectionString,string sXmlDocument)
        {
            SqlConnection sqlConnection = null;
            SqlTransaction myTransaction = null;
            XElement xElement;
            try
            {
                //hashtable is used to store the KeyName and its value
                Hashtable hashTable = new Hashtable();
                string sTargetName = string.Empty;
                string sIDColumnName = string.Empty, sIDColumnValue = string.Empty, sKeyName = string.Empty;
                StringBuilder stringBuilderSql,stringBuilderSelectID;
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    myTransaction = sqlConnection.BeginTransaction("MyTransaction");
                    SqlCommand sqlCommand = null;
                    xElement = XElement.Parse(sXmlDocument);
                    foreach (XElement myXElement in xElement.Elements())
                    {
                        stringBuilderSql = new StringBuilder();
                        //stringBuilderTempTable = new StringBuilder("Declare @Temp table(ID int);");
                        stringBuilderSelectID = new StringBuilder();
                        sTargetName = myXElement.Attribute("Target").Value;
                        switch (myXElement.Name.LocalName.ToLower())
                        {
                            case "insert":
                                sIDColumnName = myXElement.Attribute("IDColumnName").Value;
                                
                                //stringBuilderSql.Append(String.Format("insert into {0}({1}) output inserted.{2} values ({3});", sTargetName, GetColumnName(myXElement), sIDColumnName, GetParm(myXElement)));
                                if (string.IsNullOrEmpty(sIDColumnName))
                                {
                                    stringBuilderSql.Append(String.Format("insert into {0}({1}) values ({3});", sTargetName, GetColumnName(myXElement), sIDColumnName, GetParm(myXElement)));
                                }
                                else
                                {
                                    //stringBuilderSql.Append(String.Format("Declare @Temp table({2} UNIQUEIDENTIFIER);insert into {0}({1}) output inserted.{2} into @Temp values ({3}); select {2} from @Temp", sTargetName, GetColumnName(myXElement), sIDColumnName, GetParm(myXElement)));
                                    stringBuilderSql.Append(String.Format("Declare @Temp table({2} int);insert into {0}({1}) output inserted.{2} into @Temp values ({3}); select {2} from @Temp", sTargetName, GetColumnName(myXElement), sIDColumnName, GetParm(myXElement)));
                                }
                                sqlCommand = new SqlCommand(stringBuilderSql.ToString(), sqlConnection);
                                sqlCommand.Transaction = myTransaction;
                                AddParValue(myXElement, sqlCommand, hashTable);

                                //using output clause in insert command gives Identity value
                                var id = sqlCommand.ExecuteScalar();
                                if (id == null)
                                {
                                    sIDColumnValue = string.Empty;
                                }
                                else
                                {
                                    sIDColumnValue = id.ToString();// sqlCommand.ExecuteScalar().ToString();
                                }
                                if (myXElement.Attributes("KeyName").Any())
                                {
                                    string key = myXElement.Attribute("KeyName").Value;
                                    if (hashTable.ContainsKey(key))
                                    {
                                        hashTable.Remove(key);
                                    }
                                        hashTable.Add(key, sIDColumnValue);
                                }
                                break;
                            case "update":
                                stringBuilderSql.Append(String.Format("update {0} {1};", sTargetName, GetUpdateCommand(myXElement)));
                                sqlCommand = new SqlCommand(stringBuilderSql.ToString(), sqlConnection);
                                sqlCommand.Transaction = myTransaction;
                                AddParValue(myXElement, sqlCommand, hashTable);
                                sqlCommand.ExecuteNonQuery();
                                break;
                            case "delete":
                                stringBuilderSql.Append(String.Format("delete from {0} {1};", sTargetName, GetDeleteCommand(myXElement)));
                                sqlCommand = new SqlCommand(stringBuilderSql.ToString(), sqlConnection);
                                sqlCommand.Transaction = myTransaction;
                                AddParValue(myXElement, sqlCommand, hashTable);
                                sqlCommand.ExecuteNonQuery();
                                break;
                            case "sp":
                                //In case of stored procedures No ID values are stored back in the XML.
                                //Keyname and FKey attributes are not removed.
                                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                sqlCommand.CommandText = sTargetName;
                                //ExecuteSP(ref xmlNode, sqlCommand, hashTable);
                                break;
                            case "sql":
                                sqlCommand.CommandType = System.Data.CommandType.Text;
                                sqlCommand.CommandText = xElement.Value;
                                sqlCommand.ExecuteNonQuery();
                                break;
                        }
                    }
                    myTransaction.Commit();
                }
            }
            catch (Exception err)
            {
                //myTransaction.Rollback("MyTransaction");
                throw (err);
                //Microsoft.SqlServer.Server.SqlContext.Pipe.Send("Error" + err.Message);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }
        #endregion

        #region XML generation methods at front end
        /******************************************************************
         * Following methods are used for generation of XML at front end
         *******************************************************************/
        /// <summary>
        /// Using Linq adds the insert parameters in the XElement parameter
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="sTableName"></param>
        /// <param name="sIDColumnName"></param>
        /// <param name="sKeyName"></param>
        /// <param name="listColumnNameValue"></param>
        public static void AddXmlForInsert(XElement xElement, string sTargetName, string sIDColumnName, string sKeyName, IList<ColumnNameValue> listColumnNameValue)
        {
            XElement myXElement = new XElement("Insert", new XAttribute("Target", sTargetName), new XAttribute("IDColumnName", sIDColumnName));
            if (!string.IsNullOrEmpty(sKeyName))
                myXElement.Add(new XAttribute("KeyName", sKeyName));
            XElement innerXElement;
            foreach (ColumnNameValue columnNameValue in listColumnNameValue)
            {
                innerXElement = new XElement(columnNameValue.ColumnName, columnNameValue.ColumnValue);
                if (columnNameValue.FKey)
                    innerXElement.Add(new XAttribute("FKey", "Y"));
                myXElement.Add(innerXElement);
            }
            xElement.Add(myXElement);
        }
        public static void AddXmlForUpdate(XElement xElement, string sTargetName, IList<ColumnNameValue> listColumnNameValue, string sWhereClause)
        {
            XElement myXElement = new XElement("Update", new XAttribute("Target", sTargetName));
            foreach (ColumnNameValue columnNameValue in listColumnNameValue)
                myXElement.Add(new XElement(columnNameValue.ColumnName, columnNameValue.ColumnValue));
            myXElement.Add(new XElement("Where", new XCData(sWhereClause)));
            xElement.Add(myXElement);
        }
        public static void AddXMLForDelete(XElement xElement, string sTargetName, string sWhereClause)
        {
            XElement myXElement = new XElement("Delete", new XAttribute("Target", sTargetName));
            myXElement.Add(new XElement("Where", new XCData(sWhereClause)));
            xElement.Add(myXElement);
        }
        #endregion

    }
}
