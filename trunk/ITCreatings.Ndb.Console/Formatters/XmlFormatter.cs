using System;
using System.Xml;

namespace ITCreatings.Ndb.NdbConsole.Formatters
{
    /// <summary>
    /// XmlFormatter base class
    /// </summary>
    public abstract class XmlFormatter : IDisposable
    {
        protected XmlWriter writer;

        protected XmlFormatter(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings { CloseOutput = true, Indent = true};
            writer = XmlWriter.Create(path, settings);
        }

        protected abstract void WriteSummary();
        public abstract void AppendUnitTestResult(string testName, Outcome outcome, string message);

        public void Dispose()
        {
            if (writer != null)
            {
                writer.WriteEndElement();
                WriteSummary();
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();

                writer = null;
            }
        }
    }
}
