using System;

namespace ITCreatings.Ndb.Attributes
{
    /// <summary>
    /// Marks property as parent record holder (optional)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DbParentRecordAttribute : Attribute
    {
    }
}