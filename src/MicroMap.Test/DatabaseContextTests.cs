using Moq;
using NUnit.Framework;
using System.Linq;

namespace MicroMap.UnitTest
{
    [TestFixture]
    public class DatabaseContextTests
    {
        [Test]
        public void DatabaseContext_CheckCompiler()
        {
            var context = new DatabaseConnection().Open();

            Assert.IsNotNull(context.Compiler);
        }

        [Test]
        public void DatabaseContext_CheckExecutionContext()
        {
            var context = new DatabaseConnection().Open();

            Assert.IsNotNull(context.ExecutionContext);
        }
        
        [Test]
        public void DatabaseContext_Mock()
        {
            var compiler = new Mock<IQueryCompiler>();
            var executor = new Mock<IExecutionContext>();
            var context = new DatabaseConnection(compiler.Object, executor.Object).Open();

            // test is not meaningful enough
            Assert.IsNotNull(context);
        }

        [Test]
        public void DatabaseContext_MockProperties()
        {
            var context = new DatabaseConnection().Open();
            context.Compiler = new Mock<IQueryCompiler>().Object;
            context.ExecutionContext = new Mock<IExecutionContext>().Object;

            // test is not meaningful enough
            Assert.IsNotNull(context);
        }

        [Test]
        public void DatabaseContext_From_GenericWithExpression()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>(i => i.ID == 1);

            Assert.IsNotNull(query);
        }

        [Test]
        public void DatabaseContext_From_GenericWithExpression_CheckRestrictionEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>(i => i.ID == 1);
            
            Assert.That(query.Components.Any(c => c.Type == SyntaxComponent.Restriction && c.Expression == "WHERE (Item.ID = 1)"));
        }

        [Test]
        public void DatabaseContext_From_GenericWithExpression_CheckSingleRestrictionEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>(i => i.ID == 1);
            
            Assert.IsNotNull(query.Components.Single(c => c.Type == SyntaxComponent.Restriction));
        }

        [Test]
        public void DatabaseContext_From_GenericWithExpression_CheckKeytableEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>(i => i.ID == 1);
            
            Assert.That(query.Components.Any(c => c.Type == SyntaxComponent.Keytable && c.Expression == "Item"));
        }

        [Test]
        public void DatabaseContext_From_GenericWithExpression_CheckSingleKeytableEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>(i => i.ID == 1);

            Assert.IsNotNull(query.Components.Single(c => c.Type == SyntaxComponent.Keytable));
        }

        [Test]
        public void DatabaseContext_From_StringExpression()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From("WHERE ID = 1");

            Assert.IsNotNull(query);
        }

        [Test]
        public void DatabaseContext_From_StringExpression_CheckRestrictionEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From("WHERE ID = 1");

            Assert.That(query.Components.Any(c => c.Type == SyntaxComponent.Restriction && c.Expression == "WHERE ID = 1"));
        }

        [Test]
        public void DatabaseContext_From_StringExpression_CheckSingleRestrictionEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From("WHERE ID = 1");

            Assert.IsNotNull(query.Components.Single(c => c.Type == SyntaxComponent.Restriction));
        }

        [Test]
        public void DatabaseContext_From_StringExpression_CheckKeytableEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From("WHERE ID = 1");
            
            Assert.IsNull(query.Components.FirstOrDefault(c => c.Type == SyntaxComponent.Keytable));
        }
        
        [Test]
        public void DatabaseContext_From_Generic()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>();

            Assert.IsNotNull(query);
        }

        [Test]
        public void DatabaseContext_From_Generic_CheckRestrictionEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>();

            Assert.IsNull(query.Components.FirstOrDefault(c => c.Type == SyntaxComponent.Restriction));
        }
        
        [Test]
        public void DatabaseContext_From_Generic_CheckKeytableEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>();

            Assert.That(query.Components.Any(c => c.Type == SyntaxComponent.Keytable && c.Expression == "Item"));
        }

        [Test]
        public void DatabaseContext_From_Generic_CheckSingleKeytableEntry()
        {
            var context = new DatabaseConnection().Open();

            var query = context.From<Item>();

            Assert.IsNotNull(query.Components.Single(c => c.Type == SyntaxComponent.Keytable));
        }

        public class Item
        {
            public int ID { get; set; }
        }
    }
}
