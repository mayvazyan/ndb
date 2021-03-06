#if TESTS
using System;
using System.Text;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("BinaryDataRecords")]
    public class BinaryDataRecord
    {
        [DbPrimaryKeyField] public int Id;

        [DbField] public byte[] Data;

        public int DataAsInt { get { return BitConverter.ToInt32(Data, 0); } }
        public string DataAsString { get { return Encoding.UTF8.GetString(Data); } }
    }
}
#endif