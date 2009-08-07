using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Base class for Importers which generate an textfile
    /// </summary>
    public class DbTextFileGenerator
    {
        protected StreamWriter StreamWriter;
        private readonly Stack<LinePrefix> Prefixes = new Stack<LinePrefix>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTextFileGenerator"/> class.
        /// </summary>
        protected DbTextFileGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTextFileGenerator"/> class.
        /// </summary>
        /// <param name="streamWriter">The stream writer.</param>
        protected DbTextFileGenerator(StreamWriter streamWriter)
        {
            StreamWriter = streamWriter;
        }

        private LinePrefix Prefix
        {
            get
            {
                if (Prefixes.Count > 0)
                    return Prefixes.Peek();

                return null;
            }
            set
            {
                LinePrefix prefix = value;

                if (Prefix != null)
                    prefix.Prefix += Prefix.Prefix;
                
                Prefixes.Push(prefix);
            }
        }

        private void RemovePrefix()
        {
            if (Prefixes.Count > 0)
                Prefixes.Pop();
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
                StreamWriter.Write(Prefix.Prefix);
                format = format.Replace("\r\n", "\r\n" + Prefix.Prefix);
            }

            StreamWriter.WriteLine(string.Format(format, args));
        }
        
        /// <summary>
        /// Adds the new prefix.
        /// <example>
        /// <code>
        /// Add(@"IF (NOT EXISTS(SELECT OrganizationId FROM Organizations WHERE OrganizationId=777))");
        /// using (NewPrefix("\t"))
        /// {
        ///     Add("INSERT INTO Organizations (OrganizationId, OrganizationName) VALUES (777,'{0}')", "Some Organization");
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        protected IDisposable NewPrefix(string prefix)
        {
            return NewPrefix(prefix, string.Empty, string.Empty);
        }

        /// <summary>
        /// Adds the new prefix.
        /// <example>
        /// <code>
        /// Add(@"IF (NOT EXISTS(SELECT OrganizationId FROM Organizations WHERE OrganizationId=777))");
        /// using (NewPrefix("\t", "BEGIN", "END"))
        /// {
        ///     Add("INSERT INTO Organizations (OrganizationId, OrganizationName) VALUES (777,'{0}')", "Some Organization");
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="groupPrefix">The group prefix.</param>
        /// <param name="groupPostfix">The group postfix.</param>
        /// <returns></returns>
        protected IDisposable NewPrefix(string prefix, string groupPrefix, string groupPostfix)
        {
            Add(groupPrefix);

            Prefix = new LinePrefix(this, prefix, groupPostfix);
            return Prefix;
        }
       

        #endregion

        #region Execute Process

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

            using (var p = Process.Start(info))
            {
                if (p == null)
                    throw new NdbException("Process can't be started: " + fileName);

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

        #endregion

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
                return string.Format("SET IDENTITY_INSERT {0} ON", tableName);
            }

            /// <summary>
            /// Sets the identity insert Off.
            /// </summary>
            /// <param name="tableName">Name of the table.</param>
            public static string SetIdentityInsertOff(string tableName)
            {
                return string.Format("SET IDENTITY_INSERT {0} OFF", tableName);
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

        #region LinePrefix class

        private class LinePrefix : IDisposable
        {
            public string Prefix;
            public string GroupPostfix { get; private set; }
            private DbTextFileGenerator textFileGenerator;

            public LinePrefix(DbTextFileGenerator textFileGenerator, string prefix, string groupPostfix)
            {
                this.textFileGenerator = textFileGenerator;
                Prefix = prefix;
                GroupPostfix = groupPostfix;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing)
            {
                if (disposing && textFileGenerator != null)
                {
                    if (textFileGenerator.Prefix != this)
                        throw new Exception("Invalid textFileGenerator link");
                    
                    textFileGenerator.RemovePrefix();
                    textFileGenerator.Add(GroupPostfix);
                    textFileGenerator = null;
                }
            }
        }

        #endregion
    }
}