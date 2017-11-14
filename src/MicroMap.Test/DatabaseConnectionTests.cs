using Moq;
using NUnit.Framework;

namespace MicroMap.UnitTest
{
    [TestFixture]
    public class DatabaseConnectionTests
    {
        [Test]
        public void DatabaseConnection_Open_ExternalObjects()
        {
            var compiler = new QueryCompiler();
            var executor = new ExecutionContext(null);

            var connection = new DatabaseConnection(compiler, executor);
            var context = connection.Open();

            Assert.AreSame(context.Compiler, compiler);
            Assert.AreSame(context.ExecutionContext, executor);
        }

        [Test]
        public void DatabaseConnection_Open_ExternalObjects_CompilerNull()
        {
            var executor = new ExecutionContext(null);

            var connection = new DatabaseConnection(null, executor);
            var context = connection.Open();

            Assert.IsNotNull(context.Compiler);
            Assert.AreSame(context.ExecutionContext, executor);
        }

        [Test]
        public void DatabaseConnection_Open_ExternalObjects_ExecutorNull()
        {
            var compiler = new QueryCompiler();

            var connection = new DatabaseConnection(compiler, null);
            var context = connection.Open();

            Assert.AreSame(context.Compiler, compiler);
            Assert.IsNotNull(context.ExecutionContext);
        }

        [Test]
        public void DatabaseConnection_Open_MockedObjects()
        {
            var compiler = new Mock<IQueryCompiler>();
            var executor = new Mock<IExecutionContext>();

            var connection = new DatabaseConnection(compiler.Object, executor.Object);
            var context = connection.Open();

            Assert.AreSame(context.Compiler, compiler.Object);
            Assert.AreSame(context.ExecutionContext, executor.Object);
        }

        [Test]
        public void DatabaseConnection_Open_MockedContext()
        {
            var compiler = new Mock<IQueryCompiler>();
            var executor = new Mock<IExecutionContext>();

            var connection = new DatabaseConnection(compiler.Object, executor.Object);
            var context = connection.Open();

            Assert.IsNotNull(context);
        }
    }
}
