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
            _kernel.Setup(exp => exp.Execute<Item2>(It.IsAny<ComponentContainer>())).Returns(() => new List<Item2>());

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
    }
}
