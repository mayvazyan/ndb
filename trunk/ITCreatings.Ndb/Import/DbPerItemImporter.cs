using System.IO;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Importer helper for "per row" processing approach
    /// </summary>
    public abstract class DbPerItemImporter : DbTextFileGenerator
    {
        /// <summary>
        /// Inits this instance.
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        protected abstract void ProcessRow(object input);

        /// <summary>
        /// For custom proccessing purposes
        /// </summary>
        protected virtual void PostProcessing()
        {
        }

        /// <summary>
        /// Processes the specified input and saves result to out file.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="outfile">The outfile.</param>
        public void Process(object input, string outfile)
        {
            using (StreamWriter sw = new StreamWriter(outfile))
            {
                StreamWriter = sw;

                try
                {
                    Process(input);
                }
                finally
                {
                    StreamWriter = null;
                }
            }
        }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        private void Process(object input)
        {
            Init();

            ProcessRow(input);

            PostProcessing();
        }
    }
}