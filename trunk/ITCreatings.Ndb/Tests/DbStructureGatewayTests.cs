#if DEBUG
using System;
using System.Data.Common;
using System.Reflection;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests
{
    [TestFixture]
    public class DbStructureGatewayTests
    {
        private readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        private DbStructureGateway gateway;

        internal DbStructureGatewayTests(DbAccessor accessor)
        {
            gateway = new DbStructureGateway(accessor);
        }

        public DbStructureGatewayTests() : this(DbAccessor.Instance)
        {
        }

        [SetUp]
        public void SetUp()
        {
            gateway.DropTables(Assembly);
        }

        [TearDown]
        public void TearDown()
        {
            gateway.DropTables(Assembly);
        }

        [Test]
        public void OverloadTest()
        {
            gateway.DropTable(typeof(NewsItemCatChild));

            Assert.IsFalse(gateway.IsValid(typeof(NewsItemCat)));

            gateway.CreateTable(typeof(NewsItemCatChild));

            Assert.IsTrue(gateway.IsValid(typeof(NewsItemCatChild)), gateway.LastError);
            Assert.IsTrue(gateway.IsValid(typeof(NewsItemCat)), gateway.LastError);
        }


        [Test]
        public virtual void AlterTableTest()
        {
            gateway.CreateTable(typeof(NewsItem));
            Assert.IsTrue(gateway.IsValid(typeof (NewsItem)), gateway.LastError);
            Assert.IsFalse(gateway.IsValid(typeof (NewsItem2)));

            gateway.AlterTable(typeof (NewsItem2));
            Assert.IsTrue(gateway.IsValid(typeof(NewsItem2)), gateway.LastError);
            Assert.IsFalse(gateway.IsValid(typeof(NewsItem)));
        }


        [Test]
        public void TableExistsTest()
        {
            Assert.IsFalse(gateway.IsTableExists(typeof(TestUser)), "Table Exists test failed");

            gateway.CreateTable(typeof(TestUser));
            Assert.IsTrue(gateway.IsTableExists(typeof(TestUser)));
        }

        [Test]
        public void CreateTableTest()
        {
            Assert.IsFalse(gateway.IsTableExists(typeof(TestUser)));

            gateway.CreateTable(typeof (TestUser));
            Assert.IsTrue(gateway.IsTableExists(typeof(TestUser)));

            gateway.DropTable(typeof(TestUser));
            Assert.IsFalse(gateway.IsTableExists(typeof(TestUser)));

        }

        [Test]
        public void ColumnTypesTest()
        {
            if (gateway.Accessor.IsSqLite)
                Assert.Ignore("SQLite has poor types collection");

            gateway.CreateTable(typeof(TestUser));
            gateway.CreateTable(typeof(TestWorkLogItem));

            Assert.IsFalse(gateway.IsValid(typeof(TestWorkLogItem2)));

            if (gateway.Accessor.IsMySql)
                Assert.AreEqual(
//                "UserId in ITCreatings.Ndb.Tests.Data.TestWorkLogItem2 is System.UInt32 but column is System.UInt64",
                    "UserId in ITCreatings.Ndb.Tests.Data.TestWorkLogItem2 is System.Int16 but column is bigint(20) unsigned",
                    gateway.LastError);
            
            Assert.IsFalse(gateway.IsValid(typeof(TestWorkLogItem3)));

            if (gateway.Accessor.IsMySql)
                Assert.AreEqual(
                    "Description2 in ITCreatings.Ndb.Tests.Data.TestWorkLogItem3 (System.String) isn't present in db",
                    gateway.LastError);

            Assert.IsTrue(gateway.IsValid(typeof(TestWorkLogItem)));
        }

        [Test]
        public void UniqueTest()
        {
            TestData TestData = new TestData(gateway.Accessor);
            DbGateway dbGateway = new DbGateway(gateway.Accessor);
            gateway.CreateTable(typeof(TestUser));

            TestUser user = TestData.TestUser;
            dbGateway.Insert(user);

            try
            {
                dbGateway.Insert(user);
                Assert.IsFalse(true);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

/*
        [Test]
        public void PrimaryKey2ColumnsTest()
        {
            gateway.CreateTable(typeof(Task));
            gateway.CreateTable(typeof(TestUser));
            gateway.CreateTable(typeof(TasksAssignment2));

            TestUser user = TestData.TestUser;
            DbGateway.Instance.Insert(user);

            var oTask = TestData.CreateTask("title #1");
            TestData.Assign(oTask);

            oTask = TestData.CreateTask("title #2");
            TestData.Assign(oTask);

            try
            {
                TestData.Assign(oTask);

                Assert.IsFalse(true);
            }
            catch (DbException ex)
            {
                Assert.IsTrue(true);
            }
        }
*/
    }
}
#endif