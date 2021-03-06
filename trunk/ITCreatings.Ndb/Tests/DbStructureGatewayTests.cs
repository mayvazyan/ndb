﻿#if TESTS
using System.Reflection;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests
{
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
            Assert.IsFalse(gateway.IsTableExists(typeof(User)), "Table Exists test failed");

            gateway.CreateTable(typeof(User));
            Assert.IsTrue(gateway.IsTableExists(typeof(User)));
        }

        [Test]
        public void CreateTableTest()
        {
            Assert.IsFalse(gateway.IsTableExists(typeof(User)));

            gateway.CreateTable(typeof (User));
            Assert.IsTrue(gateway.IsTableExists(typeof(User)));

            gateway.DropTable(typeof(User));
            Assert.IsFalse(gateway.IsTableExists(typeof(User)));

        }

        [Test]
        public void ColumnTypesTest()
        {
            gateway.CreateTable(typeof(User));
            gateway.CreateTable(typeof(TestWorkLogItem));

            Assert.IsFalse(gateway.IsValid(typeof(TestWorkLogItem2)));

            if (gateway.Accessor.IsMySql)
                Assert.AreEqual(
                    "UserId in ITCreatings.Ndb.Tests.Data.TestWorkLogItem2 is System.Int16 but column is bigint(20)",
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
            gateway.CreateTable(typeof(User));

            User user = TestData.TestUser;
            dbGateway.Insert(user);

            try
            {
                dbGateway.Insert(user);
                Assert.Fail();
            }
            catch
            {
            }
        }

/*
        [Test]
        public void PrimaryKey2ColumnsTest()
        {
            gateway.CreateTable(typeof(Task));
            gateway.CreateTable(typeof(User));
            gateway.CreateTable(typeof(TasksAssignment2));

            User user = TestData.User;
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