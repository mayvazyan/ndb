using System;

namespace ITCreatings.Ndb.Attributes
{
    ///<summary>
    /// Marks property as child records holder. Required in order to use ORM functionality.
    ///</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DbChildRecordsAttribute : Attribute
    {
    }
}