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
    public class QueryContextOfTTests
    {
        private Mock<IExecutionKernel> _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = new Mock<IExecutionKernel>();
        }

        [Test]
        public void QueryContextOfT_Add()
        {
            var context = new QueryContext<Item>(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Command, ""));
                        
            Assert.That(context.Components.All(c => c.Type == SyntaxComponent.Command));
        }

        [Test]
        public void QueryContextOfT_Add_AlreadyAddedType()
        {
            var context = new QueryContext<Item>(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Command, ""));
            Assert.Throws<InvalidOperationException>(() => context.Add(new QueryComponent(SyntaxComponent.Command, "")));
        }

        [Test]
        public void QueryContextOfT_Select()
        {
            _kernel.Setup(exp => exp.Execute<Item>(It.IsAny<ComponentContainer>())).Returns(() => new List<Item>());

            var context = new QueryContext<Item>(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Keytable, "Item"));

            var items = context.Select();

            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Command && c.Expression == "SELECT"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Keyword && c.Expression == "FROM"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.FieldList && c.Expression == "ID, Name"));
            
            _kernel.Verify(exp => exp.Execute<Item>(It.IsAny<ComponentContainer>()), Times.Once);

            Assert.IsNotNull(items);
        }

        [Test]
        public void QueryContextOfT_Select_Generic()
        {
            _kernel.Setup(exp => exp.Execute<Item2>(It.IsAny<ComponentContainer>())).Returns(() => new List<Item2> { new Item2 { ID = 5, Name = "n" } });

            // Execute
            var context = new QueryContext<Item>(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Keytable, "Item"));

            var items = context.Select<Item2>();

            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Command && c.Expression == "SELECT"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Keyword && c.Expression == "FROM"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.FieldList && c.Expression == "ID, Name"));

            _kernel.Verify(exp => exp.Execute<Item2>(It.IsAny<ComponentContainer>()), Times.Once);

            Assert.IsNotNull(items);
        }

        [Test]
        public void QueryContextOfT_Select_AnonymOutput()
        {
            //_kernel.Setup(exp => exp.Execute<object>(It.IsAny<ComponentContainer>())).Returns(() => new List<Item> { new Item { ID = 5, Name = "n" } });
            //Setup(_kernel, new { ID = 5, Name = "n" });

            var item = new { IDs = 5, N = "n" };
            var data = new[] { item };
            var executor = new Mock<IExecutionContext>();
            executor.Setup(exp => exp.Execute(It.IsAny<CompiledQuery>())).Returns(() => new DataReaderContext(new MockedDataReader(data, item.GetType())));

            var compiler = new Mock<IQueryCompiler>();
            compiler.Setup(exp => exp.Compile(It.IsAny<ComponentContainer>())).Returns(() => new CompiledQuery());

            var kernel = new ExecutionKernel(compiler.Object, executor.Object);

            // Execute
            //var context = new QueryContext<Item>(_kernel.Object);
            var context = new QueryContext<Item>(kernel);
            context.Add(new QueryComponent(SyntaxComponent.Keytable, "Item"));
            
            var items = context.Select(a => new { IDs = a.ID, N = a.Name });

            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Command && c.Expression == "SELECT"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Keyword && c.Expression == "FROM"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.FieldList && c.Expression == "Item.ID AS IDs, Item.Name AS N"));

            Assert.That(items.Any());
            Assert.That(items.First().IDs == 5);
            Assert.That(items.First().N == "n");
        }

        [Test]
        public void QueryContextOfT_Select_StringExpression()
        {
            _kernel.Setup(exp => exp.Execute<Item3>(It.IsAny<ComponentContainer>())).Returns(() => new List<Item3>());

            var context = new QueryContext<Item>(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Keytable, "Item"));
            
            var items = context.Select<Item3>("MAX(ID) as IDs");

            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Command && c.Expression == "SELECT"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.Keyword && c.Expression == "FROM"));
            Assert.IsNotNull(context.Components.Single(c => c.Type == SyntaxComponent.FieldList && c.Expression == "MAX(ID) as IDs"));

            _kernel.Verify(exp => exp.Execute<Item3>(It.IsAny<ComponentContainer>()), Times.Once);

            Assert.IsNotNull(items);
        }

        private void Setup<T>(Mock<IExecutionKernel> kernel, T item)
        {
            _kernel.Setup(exp => exp.Execute<T>(It.IsAny<ComponentContainer>())).Returns(() => new List<T> { item });
        }

        public class Item
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }

        public class Item2
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }

        public class Item3
        {
            public int IDs { get; set; }

            public string Name { get; set; }
        }
    }
}
