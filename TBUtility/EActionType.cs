using System;
using System.Runtime.Serialization;
namespace TBUtility
{
    [DataContract]
    public enum EActionType
    {
        [EnumMember]
        Add,
        [EnumMember]
        Update,
        [EnumMember]
        Delete,
        [EnumMember]
        None,
        [EnumMember]
        Submit,
        [EnumMember]
        Refresh
    }
}
