namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Excpression Types
    /// </summary>
    public enum DbExpressionType
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal = 10,

        /// <summary>
        /// Not Equal
        /// </summary>
        NotEqual = 11,

        /// <summary>
        /// Contains
        /// </summary>
        Contains = 20,

        /// <summary>
        /// Starts With
        /// </summary>
        StartsWith = 30,

        /// <summary>
        /// Ends With
        /// </summary>
        EndsWith = 40,

        /// <summary>
        /// Greater
        /// </summary>
        Greater = 50,

        /// <summary>
        /// Less
        /// </summary>
        Less = 60,

        /// <summary>
        /// Is Not Null
        /// </summary>
        IsNotNull = 70,

        /// <summary>
        /// Is Null
        /// </summary>
        IsNull = 80,
    }
}