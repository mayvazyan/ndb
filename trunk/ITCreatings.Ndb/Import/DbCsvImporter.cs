using System.IO;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Csv Importer base class
    /// </summary>
    public abstract class DbCsvImporter : DbPerItemImporter
    {
        private readonly char delimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCsvImporter"/> class.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        protected DbCsvImporter(char delimiter)
        {
            this.delimiter = delimiter;
        }

        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <param name="args">The args.</param>
        protected abstract void ReadLine(string [] args);

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        protected sealed override void ProcessRow(object input)
        {
            using (StreamReader sr = File.OpenText((string)input))
            {
                while (!sr.EndOfStream)
                {
                    ReadLine(sr.ReadLine().Split(delimiter));
                }
            }
        }
    }
}