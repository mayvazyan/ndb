using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Importer base class
    /// </summary>
    public abstract class Importer
    {
        private StringBuilder sb;

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

            sb.AppendLine(string.Format(format, args));
        }

        /// <summary>
        /// Allows the identity insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        protected void AllowIdentityInsert(string tableName)
        {
            Add("set IDENTITY_INSERT {0} on", tableName);
        }

        /// <summary>
        /// Sets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected void Set(string name, object value)
        {
            Add("SET {0}={1}", name, value);
        }

        /// <summary>
        /// Declares the specified desc.
        /// </summary>
        /// <param name="desc">The desc.</param>
        protected void Declare(string desc)
        {
            Add("DECLARE " + desc);
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
    }
}