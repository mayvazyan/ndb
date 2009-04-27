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
            Assert.IsNotNull(info.ForeignKeys[typeof(User)]);
            Assert.AreEqual("WorkLogs", info.TableName);
            Assert.AreEqual("SpentMinutes", info.Fields[2].Name);
            Assert.AreEqual("CreationDate", info.Fields[3].Name);
            Assert.AreEqual(6, info.Fields.Length);
            Assert.AreEqual(0, info.Childs.Count);
            Assert.AreEqual(0, info.Parents.Count);

            info = DbAttributesManager.GetRecordInfo(typeof(User)) as DbIdentityRecordInfo;
            Assert.IsNotNull(info);
            Assert.AreEqual(2, info.Childs.Count);

            var info2 = DbAttributesManager.GetRecordInfo(typeof(TasksAssignment));
            Assert.IsNotNull(info2);
            Assert.AreEqual(2, info2.Parents.Count);
        }

        [Test]
        public void GetTableNameTest()
        {
            var name = DbAttributesManager.GetTableName(typeof(TestWorkLogItem));
            Assert.AreEqual("WorkLogs", name);

            var tableName = DbAttributesManager.GetTableName(typeof(User));
            Assert.AreEqual("Users", tableName);
        }
    }
}
#endif