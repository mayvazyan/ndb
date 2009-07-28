using System;
using System.Reflection;
using ITCreatings.Ndb.Accessors;
using ITCreatings.Ndb.Core;

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
    ///         Export&lt;WorkLog&gt;("WorkLogsSheetName");
    ///         Export&lt;User&gt;("UsersSheetName");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class DbExcelExport
    {
        /// <summary>
        /// Gets or sets the target DbGateway.
        /// </summary>
        /// <value>The target.</value>
        protected DbGateway Target { get; private set; }

        /// <summary>
        /// Gets or sets the source ExcelAccessor.
        /// </summary>
        /// <value>The source.</value>
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

        /// <summary>
        /// Exports the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="ExportWithClean">if set to <c>true</c> [export with clean].</param>
        public void Export(Type[] types, bool ExportWithClean)
        {
            string methodName = ExportWithClean ? "ExportWithClean" : "Export";
            MethodInfo method = GetType().GetMethod(methodName, new Type[]{} );

            DbStructureGateway structureGateway = new DbStructureGateway(Source);

            foreach (Type type in types)
            {
                string tableName = DbAttributesManager.GetTableName(type);
                
                if (structureGateway.IsTableExists(tableName))
                    method.MakeGenericMethod(type).Invoke(this, null);
            }
        }
    }
}
