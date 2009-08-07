using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Accessors.DataReaders
{
    /// <summary>
    /// Fixed Width Text Files DataReader
    /// </summary>
    public class FixedWidthTextFilesDataReader : IDataReader
    {
        private readonly List<string> names;
        private string[] args;
        private StreamReader sr;
        private string[] cellLength;
        private long totalLineLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedWidthTextFilesDataReader"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public FixedWidthTextFilesDataReader(string fileName)
        {
            sr = new StreamReader(fileName);
            string namesLine = sr.ReadLine();
            string cellsLine = sr.ReadLine();
            totalLineLength = cellsLine.Length;
            cellLength = cellsLine.Split(' '); //TODO: store only length - not whole string :)
            names = new List<string>(Split(namesLine));
        }

        private string [] Split(string line)
        {
            string[] chunks = new string[cellLength.Length];
            int startIndex = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                int len = cellLength[i].Length;
                string chunk = startIndex + len > line.Length // can occurs besides spaces trimming
                                   ? line.Substring(startIndex) 
                                   : line.Substring(startIndex, len);
                chunks[i] = chunk.Trim();
                startIndex += len + 1;
            }
            return chunks;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            if (disposing && sr != null)
            {
                sr.Dispose();
                sr = null;
            }
        }

        #endregion

        #region Implementation of IDataRecord

        public string GetName(int i)
        {
            throw new System.NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new System.NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new System.NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new System.NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new System.NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            return DataReaderUtils.GetOrdinal(names, name);
        }

        public bool GetBoolean(int i)
        {
            throw new System.NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new System.NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new System.NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new System.NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new System.NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new System.NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new System.NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new System.NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new System.NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new System.NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(int i)
        {
            return args[i];
        }

        public decimal GetDecimal(int i)
        {
            throw new System.NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new System.NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return false;
        }

        public int FieldCount
        {
            get { return args.Length; }
        }

        object IDataRecord.this[int i]
        {
            get { return args[i]; }
        }

        object IDataRecord.this[string name]
        {
            get { return ((IDataRecord)this)[GetOrdinal(name)]; }
        }

        #endregion

        #region Implementation of IDataReader

        public void Close()
        {
            Dispose();
        }

        public DataTable GetSchemaTable()
        {
            throw new System.NotImplementedException();
        }

        public bool NextResult()
        {
            return false;
        }

        private StringBuilder sb = new StringBuilder();

        public bool Read()
        {
            if (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Length != totalLineLength) // so we have multiline data 
                {
                    throw new NdbException("Multiline rows aren't supported");
//                    string line2 = "";
//                    while (!sr.EndOfStream && (line2.Length + line.Length < totalLineLength))
//                    {
//                        line2 += sr.ReadLine();
//                    }
//                    long missedLen = totalLineLength - line.Length;
//                    line += line2.Substring((int) (line2.Length - missedLen));
                }
                args = Split(line);
                return true;

            }
            return false;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                sb.Append(line);

                if (sb.Length == totalLineLength)
                {
                    
                }
                if (sb.Length > totalLineLength)
                {
                    args = Split(line);
                    sb.Remove(0, sb.Length);
                    return true;
                }
                Debug.WriteLine(line);
            }
            return false;
        }

        public int Depth
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsClosed
        {
            get { return sr == null; }
        }

        public int RecordsAffected
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}