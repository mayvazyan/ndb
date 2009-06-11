using System;
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
        protected readonly T row = new T();

        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <param name="args">The args.</param>
        protected sealed override void ReadLine(IDataRecord args)
        {
            DbGateway.Bind(row, args);
            ReadLine();
        }

        /// <summary>
        /// Reads the line.
        /// </summary>
        protected abstract void ReadLine();
    }


    /// <summary>
    /// Data Reader Importer base class
    /// </summary>
    public abstract class DbDataReaderImporter : DbImporter
    {
        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <param name="row">The row.</param>
        protected abstract void ReadLine(IDataRecord row);

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
                    ReadLine(reader);
                }
            }
        }
    }
}