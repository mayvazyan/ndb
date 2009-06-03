using System;
using System.Data;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Data Reader Typed Importer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataReaderImporter<T> : DataReaderImporter
    {
        protected readonly T row = Activator.CreateInstance<T>();

        protected override void ReadLine(IDataRecord args)
        {
            DbGateway.Bind(row, args);
            ReadLine();
        }

        protected abstract void ReadLine();
    }


    /// <summary>
    /// Data Reader Importer base class
    /// </summary>
    public abstract class DataReaderImporter : Importer
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
        protected override void ProcessRow(object input)
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