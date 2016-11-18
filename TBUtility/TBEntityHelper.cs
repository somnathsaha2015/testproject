using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
namespace TBUtility
{
    public class TBEntityHelper
    {
        private readonly static List<string> attributeList;
        //private static string connectionString;
        static TBEntityHelper()
        {
            attributeList = new List<string>();
            attributeList.Add("PrimaryKey");
            attributeList.Add("ForeignKey");
            attributeList.Add("KeyName");
            attributeList.Add("Exclude");
        }
        //public static void SetConnectionString(string sConn)
        //{
        //    connectionString = sConn;
        //}
        //This method Can have valid XElements as entity
        public static void SaveAndCommit(List<dynamic> entities,string connectionString)
        {
            
            XElement xRoot = new XElement("root");
            entities.ForEach
                (
                entity =>
                {
                    Type entityType = entity.GetType();
                    if (entityType.Name.Equals(typeof(XElement).Name))
                    {
                        xRoot.Add((entity as XElement).Elements().First());
                    }
                    else
                    {
                        ProcessEntity(entity, xRoot);
                    }
                }
                );
            XUtility.SaveXmlData(connectionString, xRoot.ToString());
        }

        public static XElement GetXElement(List<dynamic> entities)
        {
            XElement xRoot = new XElement("root");
            entities.ForEach(entity => {
                ProcessEntity(entity, xRoot);
            });
            
            return (xRoot);
        }

        private static void ProcessEntity(dynamic entity, XElement xElement)
        {
            ActualTableName actualTableNameAttr = null;
            //find out whether insert, update or delete
            PropertyInfo[] infos = entity.GetType().GetProperties();
            EActionType actionType = (EActionType)infos.Single(x => x.Name.Equals("ActionType")).GetValue(entity, null);
            //find out table name. If attribute ActualTableName is specified then that is taken otherwith entity name is taken as table name
            int classCustomAttributeCount = entity.GetType().GetCustomAttributesData().Count;
            string tableName = null;
            if (classCustomAttributeCount > 0)
            {
                var actualTableNameAttrs = entity.GetType().GetCustomAttributes(typeof(ActualTableName), true);

                if ((actualTableNameAttrs != null) && (actualTableNameAttrs.Length != 0))
                {
                    actualTableNameAttr = actualTableNameAttrs[0];
                    tableName = (actualTableNameAttr as ActualTableName).TableName;
                }
            }
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = string.Format("{0}.{1}",entity.DomainName, entity.GetType().Name);
                //tableName = string.Format(entity.GetType().Name);
            }

            if (actionType == EActionType.Add)
            {
                GetXMLForInsert(entity, xElement, infos, tableName);
            }
            else if (actionType.Equals(EActionType.Update))
            {
                GetXMLForUpdate(entity, xElement, infos, tableName);
            }
            else if (actionType.Equals(EActionType.Delete))
            {
                GetXMLForDelete(entity, xElement, infos, tableName);
            }
        }

        private static string GetIDColumnName(PropertyInfo[] infos)
        {
            string idColumnName = null;
            infos.ToList<PropertyInfo>().ForEach
            (
               x =>
               {
                   var customAttributes = x.GetCustomAttributes(false);
                   if (customAttributes.Count() > 0)
                   {
                       var attributeName = customAttributes.SingleOrDefault(y => y.GetType().Name.Equals("PrimaryKey", StringComparison.OrdinalIgnoreCase)); //customAttributes.ElementAt(0).GetType().Name;
                       if (attributeName != null)
                       {
                           idColumnName = x.Name;
                       }
                   }
               }
            );
            return (idColumnName);
        }

        private static void GetXMLForDelete(dynamic entity, XElement xElement, PropertyInfo[] infos, string tableName)
        {
            string idColumnName = GetIDColumnName(infos);
            string idColumnValue = infos.Single(y => y.Name.Equals(idColumnName)).GetValue(entity, null).ToString();
            XUtility.AddXMLForDelete(xElement, tableName, string.Format("{0}={1}", idColumnName, idColumnValue));
        }

        private static void GetXMLForUpdate(dynamic entity, XElement xElement, PropertyInfo[] infos, string tableName)
        {
            string idColumnName = string.Empty;
            Type t = entity.GetType();
            List<ColumnNameValue> nameValueList = new List<ColumnNameValue>();
            infos.ToList<PropertyInfo>().ForEach
               (
               x =>
               {
                   var customAttributes = x.GetCustomAttributes(false);
                   if (customAttributes.Count() > 0)
                   {
                       if (customAttributes.Any(y => y.GetType().Name == "PrimaryKey"))
                       {
                           idColumnName = x.Name;
                       }
                       else if
                     (!customAttributes.Any(y => attributeList.Any(z => z.Equals(y.GetType().Name, StringComparison.OrdinalIgnoreCase))))
                       {
                           CreateNameValuePair(entity, nameValueList, x);
                       }
                   }
                   else
                   {
                       CreateNameValuePair(entity, nameValueList, x);
                   }
               }
               );

            string idColumnValue = infos.Single(y => y.Name.Equals(idColumnName)).GetValue(entity, null).ToString();
            XUtility.AddXmlForUpdate(xElement, tableName, nameValueList, string.Format("{0}={1}", idColumnName, idColumnValue));
        }

        private static void GetXMLForInsert(dynamic entity, XElement xElement, PropertyInfo[] infos, string tableName)
        {
            string idColumnName = string.Empty;
            string keyName = string.Empty;
            Type t = entity.GetType();
            List<ColumnNameValue> nameValueList = new List<ColumnNameValue>();
            infos.ToList<PropertyInfo>().ForEach
                (
                x =>
                {
                    var customAttributes = x.GetCustomAttributes(false);
                    if (customAttributes.Count() > 0)
                    {
                        if (customAttributes.Any(y => y.GetType().Name.Equals("PrimaryKey", StringComparison.OrdinalIgnoreCase)))
                        {
                            idColumnName = x.Name;
                        }

                        else if (customAttributes.Any(y => y.GetType().Name.Equals("ForeignKey", StringComparison.OrdinalIgnoreCase)))
                        {
                            var value = x.GetValue(entity, null);
                            //if (value == null || value == (new Guid()) || value == 0)
                            if (value == null || value == 0)
                            {
                                ColumnNameValue nv = new ColumnNameValue(x.Name, null, true);
                                nameValueList.Add(nv);
                            }
                            else
                            {
                                ColumnNameValue nv = new ColumnNameValue(x.Name, value.ToString());
                                nameValueList.Add(nv);
                            }
                        }
                        else if (customAttributes.Any(y => y.GetType().Name.Equals("KeyName", StringComparison.OrdinalIgnoreCase)))
                        {
                            keyName = x.GetValue(entity, null);
                        }
                        else if (!customAttributes.Any(y => y.GetType().Name.Equals("Exclude", StringComparison.OrdinalIgnoreCase)))
                        {
                            CreateNameValuePair(entity, nameValueList, x);
                        }
                    }
                    else
                    {
                        CreateNameValuePair(entity, nameValueList, x);
                    }
                }
                );
            XUtility.AddXmlForInsert(xElement, tableName, idColumnName, keyName, nameValueList);
        }

        private static void CreateNameValuePair(dynamic entity, List<ColumnNameValue> nameValueList, PropertyInfo x)
        {
            var value = x.GetValue(entity, null);
            if (value != null)
            {
                ColumnNameValue nv;
                if (value.GetType().Name == "DateTime")
                {
                    int year = value.Year;
                    int month = value.Month;
                    int day = value.Day;
                    nv = new ColumnNameValue(x.Name, string.Format("{0}-{1}-{2}", year, month, day));
                }
                else
                {
                    nv = new ColumnNameValue(x.Name, value.ToString());
                }
                nameValueList.Add(nv);
            }
        }
    }
}