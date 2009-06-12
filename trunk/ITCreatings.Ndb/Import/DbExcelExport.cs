using ITCreatings.Ndb.Accessors;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Excel Export helper
    /// <example>
    /// <code>
    /// public class GeneralExport : DbExcelExport
    /// {
    ///     public GeneralExport() : base("GeneralData", "GeneralDb")
    ///     {
    ///     }
    /// 
    ///     public void Run()
    ///     {
    ///         Export<WorkLog>("WorkLogsSheetName");
    ///         Export<User>("UsersSheetName");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class DbExcelExport
    {
        protected DbGateway Target { get; private set; }
        protected ExcelAccessor Source { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExcelExport"/> class.
        /// </summary>
        /// <param name="sourceConnectionStringName">Name of the source connection string.</param>
        /// <param name="targetConnectionStringName">Name of the target connection string.</param>
        public DbExcelExport(string sourceConnectionStringName, string targetConnectionStringName)
        {
            Source = (ExcelAccessor)DbAccessor.Create(sourceConnectionStringName);
            Target = new DbGateway(DbAccessor.Create(targetConnectionStringName));
        }

        /// <summary>
        /// Exports the specified sheet to target source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName">Name of the sheet.</param>
        public virtual void Export<T>(string sheetName) where T : class, new()
        {
            Source.Export<T>(Target, sheetName);
        }

        /// <summary>
        /// Exports the specified sheet to target source. Removes any records already presented in database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName">Name of the sheet.</param>
        public void ExportWithClean<T>(string sheetName) where T : class, new()
        {
            Source.ExportWithClean<T>(Target, sheetName);
        }

        /// <summary>
        /// Exports the specified sheet to target source. Removes any records already presented in database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ExportWithClean<T>() where T : class, new()
        {
            Source.ExportWithClean<T>(Target);
        }

        

        /// <summary>
        /// Exports the specified sheet to target source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Export<T>() where T : class, new()
        {
            Source.Export<T>(Target);
        }
    }
}
