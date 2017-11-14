using MicroMap.UnitTest.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap.UnitTest.Integration
{
    [TestFixture]
    public class SelectTests
    {
        private LocalDbManager _dbManager;

        [OneTimeSetUp]
        public void Setup()
        {
            _dbManager = new LocalDbManager(null, @"(localdb)\mssqllocaldb");
            _dbManager.CreateDatabase();

            var sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE Awesome(ID int, Value varchar(20));");
            sb.AppendLine("INSERT Awesome (ID, Value) VALUES (1, 'one')");
            sb.AppendLine("INSERT Awesome (ID, Value) VALUES (2, 'two')");
            _dbManager.ExecuteString(sb.ToString());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _dbManager.DetachDatabase();
        }

        [Test]
        public void Micromap_Integration_Select()
        {
            var provider = new DatabaseConnection(_dbManager.ConnectionString);
            using (var context = provider.Open())
            {
                //var sql = string.Empty;
                //context.BeforeExecute(c => sql = c.Query);

                var one = context.From<Awesome>().Select();
                // SELECT ID, Value FROM Awesome

                Assert.That(one.First().ID == 1 && one.First().Value == "one");
                Assert.That(one.Last().ID == 2 && one.Last().Value == "two");
            }
        }

        [Test]
        public void Micromap_Integration_Select_WithStringConverter()
        {
            var provider = new DatabaseConnection(_dbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var one = context.From<Awesome>().Select<Awesome2>("ID as IDs");
                // SELECT ID as IDs FROM Awesome

                Assert.That(one.First().IDs == 1);
                Assert.That(one.Last().IDs == 2);
            }
        }

        [Test]
        public void Micromap_Integration_Select_WithStringConverterAndConstraint()
        {
            var provider = new DatabaseConnection(_dbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var one = context.From<Awesome>(a => a.ID == 1).Select<Awesome2>("ID as IDs");
                // SELECT ID as IDs FROM Awesome WHERE Awesome.ID = 1

                Assert.That(one.First().IDs == 1);
            }
        }

        [Test]
        public void Micromap_Integration_Select_WithConstraint()
        {
            var provider = new DatabaseConnection(_dbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var one = context.From<Awesome>(a => a.ID == 1).Select();
                // SELECT ID, Value FROM Awesome WHERE Awesome.ID = 1

                Assert.That(one.Single().ID == 1 && one.Single().Value == "one");
            }
        }

        [Test]
        public void Micromap_Integration_Select_WithStringBody()
        {
            var provider = new DatabaseConnection(_dbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var one = context.From<Awesome>().Select<Awesome2>("MAX(ID) as IDs");
                // SELECT ID, Value FROM Awesome WHERE Awesome.ID = 1

                Assert.That(one.Single().IDs == 2);
            }
        }

        [Test]
        public void Micromap_Integration_Select_WithStringBodyAndConstraint()
        {
            var provider = new DatabaseConnection(_dbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var one = context.From<Awesome>(a => a.ID == 1).Select<Awesome2>("MAX(ID) as IDs");
                // SELECT ID, Value FROM Awesome WHERE Awesome.ID = 1

                Assert.That(one.Single().IDs == 1);
            }
        }

        public class Awesome
        {
            public int ID { get; set; }

            public string Value { get; set; }
        }

        public class Awesome2
        {
            public int IDs { get; set; }
        }

        public class Cool
        {
            public int AwesomeID { get; set; }

            public string CoolValue { get; set; }
        }
    }
}
