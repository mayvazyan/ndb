using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ITCreatings.Ndb.Import
{
    
    /// <summary>
    /// Importer base class
    /// </summary>
    public abstract class DbImporter
    {
        private class LinePrefix : IDisposable
        {
            public string Prefix { get; private set; }
            public string GroupPostfix { get; private set; }
            private DbImporter Importer;

            public LinePrefix(DbImporter importer, string prefix, string groupPostfix)
            {
                Importer = importer;
                Prefix = prefix;
                GroupPostfix = groupPostfix;
            }

            public void Dispose()
            {
                if (Importer != null)
                {
                    if (Importer.Prefix != this)
                        throw new Exception("Invalid Importer link");
                    
                    Importer.Prefix = null;
                    Importer.Add(GroupPostfix);
                    Importer = null;
                }
            }
        }

        private StringBuilder sb;
        private LinePrefix Prefix { get; set; }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        protected virtual void Init()
        {
            sb = new StringBuilder();

            Add("SET NOCOUNT ON");
            NewLine();
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
            string sql = Process(input);

            File.WriteAllText(outfile, sql);
        }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        public string Process(object input)
        {
            Init();

            ProcessRow(input);

            PostProcessing();

            return sb.ToString();
        }

        #region utils

        /// <summary>
        /// Adds new line.
        /// </summary>
        protected void NewLine()
        {
            Add("");
        }

        /// <summary>
        /// Adds the specified line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Add(string format, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                    args[i] = args[i].ToString().Replace("'", "''");
            }

            if (Prefix != null)
            {
                sb.Append(Prefix.Prefix);
                format = format.Replace("\r\n", "\r\n" + Prefix.Prefix);
            }

            sb.AppendLine(string.Format(format, args));
        }
        
        protected IDisposable NewPrefix(string prefix)
        {
            return NewPrefix(prefix, string.Empty, string.Empty);
        }

        protected IDisposable NewPrefix(string prefix, string groupPrefix, string groupPostfix)
        {
            if (Prefix != null)
                throw new Exception("Previous prefix wan't disposed yet");

            Add(groupPrefix);

            Prefix = new LinePrefix(this, prefix, groupPostfix);
            return Prefix;
        }
       

        #endregion

        /// <summary>
        /// Executes the process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static int ExecuteProcess(string fileName, string args)
        {
            ProcessStartInfo info = new ProcessStartInfo(fileName, args)
                                        {
                                            UseShellExecute = false
//                                            ,RedirectStandardOutput = false
                                        };

            using (var p = System.Diagnostics.Process.Start(info))
            {
                if (p == null)
                    throw new Exception("Process can't be started: " + fileName);

                p.WaitForExit();

                int exitCode = p.ExitCode;

                return exitCode;
            }
        }

        /// <summary>
        /// Executes the CMD.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static bool ExecuteCmd(string args)
        {
            return ExecuteProcess("cmd", args) == 0;
        }

        #region Database Specific Utils

        /// <summary>
        /// MsSql Specific Utils
        /// </summary>
        public static class MsSql
        {
            /// <summary>
            /// Sets the identity insert On.
            /// </summary>
            /// <param name="tableName">Name of the table.</param>
            public static string SetIdentityInsertOn(string tableName)
            {
                return string.Format("SET IDENTITY_INSERT [{0}] ON", tableName);
            }

            /// <summary>
            /// Sets the identity insert Off.
            /// </summary>
            /// <param name="tableName">Name of the table.</param>
            public static string SetIdentityInsertOff(string tableName)
            {
                return string.Format("SET IDENTITY_INSERT [{0}] OFF", tableName);
            }

            /// <summary>
            /// Sets the specified variable.
            /// </summary>
            /// <param name="name">The variable.</param>
            /// <param name="value">The value.</param>
            public static string Set(string name, object value)
            {
                return string.Format("SET {0}={1}", name, value);
            }

            /// <summary>
            /// Declares the specified variable.
            /// </summary>
            /// <param name="variable">The variable.</param>
            public static string Declare(string variable)
            {
                return "DECLARE " + variable;
            }

            /// <summary>
            /// Returns GO.
            /// </summary>
            /// <returns></returns>
            public static string Go()
            {
                return "GO";
            }
        }
        #endregion
    }
}