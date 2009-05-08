#if DEBUG
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Tests.Data;
using ITCreatings.Ndb.Utils;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests
{
    [TestFixture]
    [Explicit("Not completed yet")]
    public class DbCodeGeneratorTests
    {
        DbCodeGenerator generator;

        [SetUp]
        public void SetUp()
        {
            DbStructureGateway sgateway = DbStructureGateway.Instance;
            sgateway.DropTables(typeof(User).Assembly);
            sgateway.CreateTables(typeof(User).Assembly);

            TestData data = new TestData(sgateway.Accessor);
            long user = data.CreateUser();
            generator = new DbCodeGenerator(sgateway) {Namespace = "ITCreatings.Ndb.GeneratedObjects"};
        }

        [Test]
        public void CompileTest()
        {
            var classToGenerate = "GeneratedUser";
            string generateClass = generator.GenerateClass("Users", classToGenerate);
            Assembly assembly = DbCompilerUtils.Compile(generateClass);
            Type type = assembly.GetType(generator.Namespace + "." + classToGenerate);

            ulong count = DbGateway.Instance.LoadCount(type);
            Assert.AreEqual(1, count);

            DbRecordInfo info1 = DbAttributesManager.GetRecordInfo(typeof(User));
            DbRecordInfo info2 = DbAttributesManager.GetRecordInfo(type);

            Assert.AreEqual(info1.TableName, info2.TableName, "TableName");


            DbFieldInfo pk = (info1 as DbIdentityRecordInfo).PrimaryKey;
            Assert.AreEqual(pk.FieldType, info2.Fields[0].FieldType);
            Assert.AreEqual(pk.Name, info2.Fields[0].Name);
            
            Assert.AreEqual(typeof(short), info2.Fields[1].FieldType);
            for (int i = 1; i < info2.Fields.Length-1; i++)
            {
                Assert.AreEqual(info1.Fields[i], info2.Fields[i+1], "Fields");    
            }
        }

        [Test]
        public void ParseTest()
        {
            Match m  = DbCodeGenerator.ParseSqlType("varchar(254)");
            Assert.IsTrue(m.Success);
            Assert.AreEqual("varchar", m.Groups[1].Value);
            Assert.AreEqual("254", m.Groups[2].Value);

            int count = m.Groups.Count;
            Assert.AreEqual(3, count);
        }
    }
}
#endif