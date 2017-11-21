using System;
using NUnit.Framework;
using MicroMap;
using System.Collections.Generic;
using System.Linq;
using Moq;
using MicroMap.UnitTest.Datareader;
using MicroMap.Reader;

namespace MicroMap.Test
{
    [TestFixture]
    public class SqlIntegrationTests
    {
        [Test]
        [Ignore("test")]
        public void SqlIntegration()
        {
            var awesomes = new List<Awesome>
            {
                new Awesome { ID = 1 }
            };

            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.IsAny<CompiledQuery>())).Returns(() => new DataReaderContext(new MockedDataReader(awesomes, typeof(Awesome))));




            var provider = new DatabaseConnection(null, executor.Object);
            using (var context = provider.Open())
            {

                //context.From<Awesome>(a => a.ID == 1).Select<Awesome2>("MAX(ID) as ID", a => new { IDs = a.ID });
                
                // EXPERIMENTAL
                context.From<Awesome>(a => a.ID == 1).Join<Cool>((a, c) => c.AwesomeID == a.ID).Select((a, c) => new { c.CoolValue, a.ID });
                //context.From<Awesome>(a => a.ID == 1, cfg => cfg.Join<Cool>((a, c) => c.AwesomeID == a.ID)).Select(() => new { CoolValue = string.Empty, ID = 0 });



                context.From<Awesome>().Single();
                // SELECT ID, Value FROM Awesome

                context.From<Awesome>().Single(a => new { IDs = a.ID });
                // SELECT ID FROM Awesome

                context.From<Awesome>(a => a.ID == 1).Single<Awesome2>(a => new { IDs = a.ID });
                // SELECT ID FROM Awesome WHERE ID = 1

                context.From<Awesome>(a => a.ID == 1).Single<Awesome2>();
                // SELECT ID, Value FROM Awesome WHERE ID = 1

                context.From<Awesome>(a => a.ID == 1).Single<int>("MAX(ID)"); // scalar
                // SELECT TOP 1 MAX(ID) FROM Awesome WHERE ID = 1
                // SELECT MAX(ID) FROM Awesome WHERE ID = 1









                context.From<Awesome>(a => a.ID == 1).Update(new { Value = "Updated" });
                // UPDATE Awesome SET Value = 'Updated' WHERE ID = 1
                context.From<Awesome>(a => a.ID == 1).Update(new Awesome { Value = "Updated" });

                context.From<Awesome>().Update(new { Value = "Updated" });
                // UPDATE Awesome SET Value = 'Updated'

                context.From<Awesome>(a => a.ID == 1).Delete();
                // DELETE FROM Awesome WHERE ID = 1


                context.From<Awesome>().Insert<Awesome>(() => new Awesome { ID = 1, Value = "inserted" });

                context.From<Awesome>().Insert(() => new { IDs = 1, Value = "inserted" });



                context.From("select * from awesome where id = 1").Select<Awesome>();
                // select * from awesome where id = 1

                context.From("select * from awesome where id = 1").Select(() => new { IDs = 0 });
                // select * from awesome where id = 1
                context.From("select ID, Value from awesome where id = 1").Select(() => new { ID = 0, Value = "" }, b => new { IDs = b.ID, SomeValue = b.Value });

                var lst = new List<Awesome>();
                var tmp3 = from a in lst where a.ID == 1 select new { a.ID, Name = a.ID };
            }
        }

        [Test]
        public void SqlIntegration_Select_Mocked()
        {
            var awesomes = new List<Awesome>
            {
                new Awesome { ID = 1 }
            };

            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.IsAny<CompiledQuery>())).Returns(() => new DataReaderContext(new MockedDataReader(awesomes, typeof(Awesome))));
            
            var provider = new DatabaseConnection(null, executor.Object);
            using (var context = provider.Open())
            {
                context.From<Awesome>().Select();
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT ID, Value FROM Awesome")), Times.Once);

                context.From<Awesome>(a => a.ID == 1).Select<Awesome2>();
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT ID, Value FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);

                context.From<Awesome>().Select<Awesome2>("MAX(ID) as IDs");
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT MAX(ID) as IDs FROM Awesome")), Times.Once);

                context.From<Awesome>(a => a.ID == 1).Select<Awesome2>("MAX(ID) as IDs");
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT MAX(ID) as IDs FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);

                context.From<Awesome>().Select<Awesome2>("ID as IDs");
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT ID as IDs FROM Awesome")), Times.Once);

                context.From<Awesome>(a => a.ID == 1).Select<Awesome2>("ID as IDs");
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT ID as IDs FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);

                context.From<Awesome>().Select(a => new { IDs = a.ID, Name = a.Value });
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT Awesome.ID AS IDs, Awesome.Value AS Name FROM Awesome")), Times.Once);

                context.From<Awesome>(a => a.ID == 1).Select(a => new { IDs = a.ID });
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT Awesome.ID AS IDs FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);

                executor.ResetCalls();

                context.From<Awesome>(a => a.ID == 1).Select<Awesome2>(b => new Awesome2 { IDs = b.ID });
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT Awesome.ID AS IDs FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);


                context.From<Awesome>(a => a.ID == 1).Select<Awesome>(b => new { b.ID, b.Value });
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT Awesome.ID, Awesome.Value FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);

                executor.ResetCalls();

                context.From<Awesome>(a => a.ID == 1).Select<Awesome>(b => new Awesome { ID = b.ID, Value = b.Value });
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT Awesome.ID, Awesome.Value FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);

                context.From<Awesome>(a => a.ID == 1).Select<Awesome2>(a => new { IDs = a.ID });
                executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT Awesome.ID AS IDs FROM Awesome WHERE (Awesome.ID = 1)")), Times.Once);
            }
        }

        //[Test]
        //public void SqlIntegration_Mock_Kernel_Sample()
        //{
        //    var warrior = new
        //    {
        //        ID = 1,
        //        Name = "Olaf",
        //        WeaponID = 1,
        //        Race = "Elf"
        //    };

        //    var warriors = new[]
        //    {
        //        warrior
        //    }.ToList();

        //    var executor = new Mock<IExecutionContext>();
        //    executor.Setup(exp => exp.Execute(It.IsAny<CompiledQuery>())).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warrior.GetType())));

        //    var provider = new DatabaseConnection(new Mock<IQueryCompiler>().Object, executor.Object);
        //    using (var context = provider.Open())
        //    {
        //        var result = context.From<Warrior>().Select();
        //        Assert.That(result.First().ID == warrior.ID);
        //        Assert.That(result.First().Name == warrior.Name);
        //    }
        //}

        [Test]
        public void SqlIntegration_Mock_Reader_Sample_Select()
        {
            var warrior = new
            {
                ID = 1,
                Name = "Olaf",
                WeaponID = 1,
                Race = "Elf"
            };

            var warriors = new[]
            {
                warrior
            }.ToList();
            
            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.IsAny<CompiledQuery>())).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warrior.GetType())));

            //var provider = new DatabaseConnection(new Mock<IQueryCompiler>().Object, executor.Object);
            var provider = new DatabaseConnection(null, executor.Object);
            using (var context = provider.Open())
            {
                var result = context.From<Warrior>().Select();
                Assert.That(result.First().ID == warrior.ID);
                Assert.That(result.First().Name == warrior.Name);
            }
        }

        [Test]
        public void SqlIntegration_Mock_Reader_Sample_Select_Generic()
        {
            var warrior = new
            {
                ID = 1,
                Name = "Olaf",
                WeaponID = 1,
                Race = "Elf"
            };

            var warriors = new[]
            {
                warrior
            }.ToList();

            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.IsAny<CompiledQuery>())).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warrior.GetType())));

            //var provider = new DatabaseConnection(new Mock<IQueryCompiler>().Object, executor.Object);
            var provider = new DatabaseConnection();
            using (var context = provider.Open())
            {
                context.ExecutionContext = executor.Object;

                var result = context.From<Warrior>().Select<Warrior2>();
                Assert.That(result.First().ID == warrior.ID);
                Assert.That(result.First().Name == warrior.Name);

                Assert.IsInstanceOf<Warrior2>(result.First());
            }
        }

        [Test]
        public void SqlIntegration_Mock_Reader_Sample_Select_String()
        {
            var warrior = new
            {
                ID = 1,
                Name = "Olaf",
                WeaponID = 1,
                Race = "Elf"
            };

            var warriors = new[]
            {
                warrior
            }.ToList();

            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.Is<CompiledQuery>(c => c.Query == "SELECT MAX(ID) as ID FROM Warrior"))).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warrior.GetType())));
            
            var provider = new DatabaseConnection();
            using (var context = provider.Open())
            {
                context.ExecutionContext = executor.Object;

                var result = context.From<Warrior>().Select<Warrior2>("MAX(ID) as ID");
                Assert.That(result.First().ID == warrior.ID);
                Assert.That(result.First().Name == warrior.Name);

                Assert.IsInstanceOf<Warrior2>(result.First());
            }
        }

        public class Warrior
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int WeaponID { get; set; }
            public string Race { get; set; }
        }

        public class Warrior2
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int WeaponID { get; set; }
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
