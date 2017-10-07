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
    public class QueryCompilerTests
    {
        [Test]
        public void QueryCompiler_Compile()
        {
            var container = new ComponentContainer()
                .Add(new QueryComponent(SyntaxComponent.Keytable, "Item"))
                .Add(new QueryComponent(SyntaxComponent.Command, "SELECT"))
                .Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"))
                .Add(new QueryComponent(SyntaxComponent.FieldList, "ID, Name"));

            var compiler = new QueryCompiler();
            var query = compiler.Compile<Item>(container);

            Assert.AreEqual("SELECT ID, Name FROM Item", query.Query);
        }

        public class Item
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }
    }
}
