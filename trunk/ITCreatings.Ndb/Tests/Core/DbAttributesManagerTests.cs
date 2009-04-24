#if DEBUG
using System;
using System.Reflection;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Core
{
    [TestFixture]
    public class DbAttributesManagerTests
    {
        [Test]
        public void LoadDbRecordTypes()
        {
            Type[] list = DbAttributesManager.LoadDbRecordTypes(Assembly.GetExecutingAssembly());
            Assert.Less(0, list.Length);
        }

        [Test]
        public void GetRecordInfoTest()
        {
            var info = DbAttributesManager.GetRecordInfo(typeof(TestWorkLogItem)) as DbIdentityRecordInfo;
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.PrimaryKey);
            Assert.AreEqual(1, info.ForeignKeys.Count);
            Assert.IsNotNull(info.ForeignKeys[typeof(TestUser)]);
            Assert.AreEqual("WorkLogs", info.TableName);
            Assert.AreEqual("SpentMinutes", info.Fields[2].Name);
            Assert.AreEqual("CreationDate", info.Fields[3].Name);
            Assert.AreEqual(6, info.Fields.Length);
        }

        [Test]
        public void GetTableNameTest()
        {
            var name = DbAttributesManager.GetTableName(typeof(TestWorkLogItem));
            Assert.AreEqual("WorkLogs", name);

            var tableName = DbAttributesManager.GetTableName(typeof(TestUser));
            Assert.AreEqual("TestUsers", tableName);
        }
    }
}
#endif