﻿#if DEBUG
using System.Reflection;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests
{
    public class DbTestFixture
    {
        private readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        protected readonly DbStructureGateway sgateway;
        protected readonly DbGateway gateway;

        protected TestData TestData;
        protected DbTestUtils DbTestUtils;

        public DbTestFixture() : this(DbAccessor.Instance)
        {
        }

        public DbTestFixture(DbAccessor accessor)
        {
            sgateway = new DbStructureGateway(accessor);
            gateway = new DbGateway(accessor);
        }

        public bool IsGuidSupported
        {
            get
            {
                DbAccessor accessor = gateway.Accessor;
                return accessor.IsMySql || accessor.IsMsSql;
            }
        }

        [SetUp]
        public void SetUp()
        {
            TestData = new TestData(gateway.Accessor);
            DbTestUtils = new DbTestUtils(gateway.Accessor);

            sgateway.DropTables(Assembly);

            CreateTables();

            TestData.CreateUser();
        }

        public void CreateTables()
        {
            //TODO: Add GUID support to Postgree and SQLite and remove below fix
            var types = (IsGuidSupported)
                            ? new[]
                                  {
                                      typeof (TestGuidRecord),
                                      typeof (TestStringLength),
                                      typeof (BinaryDataRecord),
                                      typeof (User),
                                      typeof (TasksAssignment),
                                      typeof (Task),
                                      typeof (Event),
                                      typeof (WorkLog)
                                  }
                            : new[]
                                  {
                                      typeof (TestStringLength),
                                      typeof (BinaryDataRecord),
                                      typeof (User),
                                      typeof (TasksAssignment),
                                      typeof (Task),
                                      typeof (Event),
                                      typeof (WorkLog)
                                  };

            sgateway.CreateTables(types);
        }

        [TearDown]
        public void TearDown()
        {
            sgateway.DropTables(Assembly);
        }
    }
}
#endif