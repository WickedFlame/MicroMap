using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MicroMap.UnitTest.Utils
{
    public class LocalDbManager : IDisposable
    {
        private string _databaseName;
        private string _databaseDirectory = "Data";

        public LocalDbManager()
            : this(null)
        {
        }

        public LocalDbManager(string databaseName)
            : this(databaseName, @"(LocalDB)\v11.0")
        {
        }

        public LocalDbManager(string databaseName, string dataSource)
        {
            if (string.IsNullOrEmpty(dataSource))
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            DataSource = dataSource;
            DatabaseName = string.IsNullOrWhiteSpace(databaseName) ? "MicroMap_" + Guid.NewGuid().ToString().Normalize().Replace("-", string.Empty) : databaseName;
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~LocalDbManager()
        {
            Dispose(false);
        }
        
        public string DatabaseDirectory
        {
            get
            {
                return _databaseDirectory;
            }
            set
            {
                _databaseDirectory = value;
                SetFilePath();
            }
        }

        public string ConnectionString { get; private set; }

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                _databaseName = value;
                SetFilePath();
            }
        }
        
        public string OutputFolder { get; private set; }
        
        public string DatabaseMdfPath { get; private set; }
        
        public string DatabaseLogPath { get; private set; }

        public string DataSource { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing">Indicates if the object is in disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    DetachDatabase();

                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public void ExecuteString(string script)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                ExecuteNonQuery(connection, script);
                connection.Close();
            }
        }

        public bool DatabaseExists()
        {
            var connectionString = $@"Data Source={DataSource};Initial Catalog=master;Integrated Security=True";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '{DatabaseName}' OR name = '{DatabaseName}')";
                    var exists = cmd.ExecuteScalar();

                    return exists != null;
                }
            }
        }

        public void CreateDatabase()
        {
            SetFilePath();

            ConnectionString = $@"Data Source={DataSource};AttachDBFileName={DatabaseMdfPath};Initial Catalog={DatabaseName};Integrated Security=True;";

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            // If the database does not already exist, create it.
            var connectionString = $@"Data Source={DataSource};Initial Catalog=master;Integrated Security=True";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    var sb = new StringBuilder(100);
                    sb.AppendLine($"EXECUTE (N'CREATE DATABASE {DatabaseName}");
                    sb.AppendLine($"ON PRIMARY (NAME = N''{DatabaseName}'', FILENAME = ''{DatabaseMdfPath}'')");
                    sb.AppendLine($"LOG ON (NAME = N''{DatabaseName}_log'',  FILENAME = ''{DatabaseLogPath}'')')");

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();

                    // Sql sometimes caches the tables when a db is created multiple times
                    // delete all tables to get a clean database
                    cmd.CommandText = "EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'  EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL' EXEC sp_MSForEachTable 'DELETE FROM ?'  EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'  EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DetachDatabase()
        {
            try
            {
                // detatch the database
                var connectionString = $@"Data Source={DataSource};Initial Catalog=master;Integrated Security=True";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = $"ALTER DATABASE {DatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{DatabaseName}'";
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);

                            cmd.CommandText = $"DROP DATABASE {DatabaseName}";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                Trace.WriteLine($"DatabaseManager detatched the Local Database {Path.Combine(DatabaseDirectory, DatabaseName)}");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                if (File.Exists(DatabaseMdfPath))
                {
                    File.Delete(DatabaseMdfPath);
                }

                if (File.Exists(DatabaseLogPath))
                {
                    File.Delete(DatabaseLogPath);
                }
            }
        }

        private static string RemoveCommentsFromQuery(string query)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(query, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                    {
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";
                    }

                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);

            return noComments;
        }
        
        private int ExecuteNonQuery(SqlConnection connection, string query)
        {
            query = RemoveCommentsFromQuery(query);

            // SqlCommand can't handle go breakes so split all go
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(query);

            var transaction = connection.BeginTransaction();
            var affectedRows = 0;
            using (var command = connection.CreateCommand())
            {
                try
                {
                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            command.CommandText = line;
                            command.Transaction = transaction;

                            affectedRows = command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    Trace.WriteLine(e);

                    throw new Exception("LocalDbManager caused an exception when calling ExecuteNonQuery", e);
                }
            }

            transaction.Commit();

            return affectedRows;
        }

        private void SetFilePath()
        {
            OutputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DatabaseDirectory);
            var mdfFilename = $"{DatabaseName}.mdf";
            DatabaseMdfPath = Path.Combine(OutputFolder, mdfFilename);
            DatabaseLogPath = Path.Combine(OutputFolder, $"{DatabaseName}_log.ldf");
        }
    }
}
