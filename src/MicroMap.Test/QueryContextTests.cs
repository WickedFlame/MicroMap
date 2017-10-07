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
    public class QueryContextTests
    {
        private Mock<IExecutionKernel> _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = new Mock<IExecutionKernel>();
        }

        [Test]
        public void QueryContext_Add()
        {
            var context = new QueryContext(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Command, ""));
                        
            Assert.That(context.Components.All(c => c.Type == SyntaxComponent.Command));
        }

        [Test]
        public void QueryContext_Add_AlreadyAddedType()
        {
            var context = new QueryContext(_kernel.Object);
            context.Add(new QueryComponent(SyntaxComponent.Command, ""));
            Assert.Throws<InvalidOperationException>(() => context.Add(new QueryComponent(SyntaxComponent.Command, "")));
        }
    }
}
