using System;
using System.Runtime.Serialization;
using System.ComponentModel;
namespace TBUtility
{
    [DataContract]
    public partial class GXEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        [Exclude()]
        [DataMember]
        public EActionType ActionType { get; set; }//For insert or update
        [Exclude()]
        [DataMember]
        public bool IsDataChanged { get; set; }
        [Exclude]
        [DataMember]
        public string DomainName{get;set;}
    }

    [AttributeUsage(AttributeTargets.Property)]
    [DataContract]
    public class PrimaryKey : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    [DataContract]
    public class ForeignKey : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    [DataContract]
    public class KeyName : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    [DataContract]
    public class Exclude : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class)]
    [DataContract]
    public class ActualTableName : Attribute
    {
        [DataMember]
        public string TableName { get; set; }
        public ActualTableName(string tableName)
        {
            TableName = tableName;
        }
    }
}