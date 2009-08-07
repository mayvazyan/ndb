using System.Data;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Data Reader Typed Importer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DbDataReaderImporter<T> : DbDataReaderImporter where T : new()
    {
        /// <summary>
        /// Contains imported row
        /// </summary>
        protected T row;

        /// <summary>
        /// Reads the next entry from the underlyed IDataReader and binds this data to "row" field
        /// </summary>
        /// <param name="args">The args.</param>
        protected sealed override void ProcessLine(IDataRecord args)
        {
            row = new T();
            DbGateway.Bind(row, args);
            ProcessLine();
        }

        /// <summary>
        /// Called for every row in IDataReader, corresponding object can be found in "row" field
        /// </summary>
        protected abstract void ProcessLine();
    }


    /// <summary>
    /// Data Reader Importer base class
    /// </summary>
    public abstract class DbDataReaderImporter : DbPerItemImporter
    {
        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <param name="row">The row.</param>
        protected abstract void ProcessLine(IDataRecord row);

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        protected sealed override void ProcessRow(object input)
        {
            using (IDataReader reader = (IDataReader)input)
            {
                while (reader.Read())
                {
                    ProcessLine(reader);
                }
            }
        }
    }
}