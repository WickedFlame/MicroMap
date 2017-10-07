using MicroMap.Reader;
using MicroMap.UnitTest.Datareader;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap.UnitTest
{
    [TestFixture]
    public class ExecutionKernelTests
    {
        [Test]
        public void ExecutionKernel_Execute()
        {
            var container = new ComponentContainer()
                .Add(new QueryComponent(SyntaxComponent.Keytable, "Item"))
                .Add(new QueryComponent(SyntaxComponent.Command, "SELECT"))
                .Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"))
                .Add(new QueryComponent(SyntaxComponent.FieldList, "ID, Name"));

            var compiledQuery = new CompiledQuery { Query = "SELECT ID, Name FROM Item" };

            var items = new List<Item>
            {
                new Item{ID=1,Name="name" }
            };

            var compiler = new Mock<IQueryCompiler>();
            compiler.Setup(exp => exp.Compile<Item>(It.Is<ComponentContainer>(c => c == container))).Returns(() => compiledQuery);
            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.Is<CompiledQuery>(c => c == compiledQuery))).Returns(() => new DataReaderContext(new MockedDataReader(items, typeof(Item))));

            // Execute
            var kernel = new ExecutionKernel(compiler.Object, executor.Object);
            var result = kernel.Execute<Item>(container);

            compiler.Verify(exp => exp.Compile<Item>(It.Is<ComponentContainer>(c => c == container)), Times.Once);
            executor.Verify(exp => exp.Execute(It.Is<CompiledQuery>(c => c == compiledQuery)), Times.Once);

            Assert.That(result.Count() == 1);
        }

        [Test]
        [Ignore("not implemented")]
        public void ExecutionKernel_ExecuteNonQuery()
        {
            var kernel = new ExecutionKernel(new Mock<IQueryCompiler>().Object, new Mock<IExecutionContext>().Object);
            kernel.ExecuteNonQuery(new ComponentContainer());

            Assert.Fail("assert is not implemented correctly");
        }

        public class Item
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }
    }
}
