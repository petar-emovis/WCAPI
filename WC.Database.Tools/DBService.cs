using DbUp;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text.RegularExpressions;
using WC.Database.Tools.Models;
using Microsoft.Data.SqlClient;

namespace WC.Database.Tools
{
    public class DBService
    {        
        private readonly DbHelper _dbHelper;
        private readonly DbUpdateConfiguration _dbConfig;

        public DBService()
        {

        }
        public DBService(string connectionString)
        {
            _dbConfig = new DbUpdateConfiguration(connectionString);
            if (string.IsNullOrEmpty(_dbConfig.ConnectionString))
            {
                throw new Exception("Connection string not set. Please set database connection string.");
            }
            _dbHelper = new DbHelper(connectionString);
        }

        public async Task<bool> TestConnectionAsync(string conn)
        {
            try
            {
                await using var connection = new Microsoft.Data.SqlClient.SqlConnection(conn);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateDatabaseAsync()
        {
            if (String.IsNullOrEmpty(_dbConfig.ConnectionString))
            {
                throw new Exception("Connection string not set. Please set database connection string.");
            }

            string message = "";

            var output = new List<string>();
            var currentVersion = GetCurrentVersion();
            message = $"Current DB schema version is {currentVersion}";

            //IReadOnlyList<DbUpdate> migrations = GetNewMigrations(currentVersion);
            //OutputMessage($"{migrations.Count} migration(s) found.");

            //var duplicatedVersion = GetDuplicatedVersion(migrations);
            //if (duplicatedVersion != null)
            //{
            //    OutputMessage($"Non-unique migration found: {duplicatedVersion}.");
            //    return false;
            //}

            //foreach (Migration migration in migrations)
            //{
            //    if (!_dbHelper.ExecuteCommand(migration.GetContent()))
            //    {
            //        DbMigrationStatics.OutputMessage($"DB upgrade failed trying to update to {migration.Version}.\r\n" +
            //            $" Execution of script {migration.FilePath} failed!");
            //        return false;
            //    }
            //    UpdateVersion(migration.Version);
            //    OutputMessage($"Database updated to version {migration.Version}.");
            //    OutputMessage($"Executed migration: {migration.Name}.");
            //}

            //if (!migrations.Any())
            //{
            //    OutputMessage("No updates for the current schema version.");
            //}
            //else
            //{
            //    var newVersion = migrations.Last().Version;
            //    OutputMessage($"New DB schema version is {newVersion}.");
            //}

            return true;
        }

        private int GetCurrentVersion()
        {
            if (!DoesVersionTableExists())
            {
                //CreateVersionTable();
                return 0;
            }
            return GetCurrentVersionFromSettingsTable();
        }

        private bool DoesVersionTableExists()
        {
            string query = $@"
                IF (OBJECT_ID('{_dbConfig.VersionTableSchema}.{_dbConfig.VersionTableName}', 'table') IS NULL)
                SELECT 0
                ELSE SELECT 1";

            return _dbHelper.ExecuteScalar<int>(query) == 1;
        }
        private int GetCurrentVersionFromSettingsTable()
        {
            var version = _dbHelper.ExecuteScalar<int>($"SELECT ScriptName FROM " +
                $"{_dbConfig.VersionTableSchema}.{_dbConfig.VersionTableName}" +
                $" WHERE Name = 'Version'");

            return version;
        }
        private IReadOnlyList<DbUpdate> GetNewMigrations(int currentVersion)
        {
            var regex = new Regex(@"^V\d+_\d+_\d+__.*\.sql$");

            return new DirectoryInfo(_dbConfig.MigrationRoot)
                .GetFiles()
                .Where(x => regex.IsMatch(x.Name))
                .Select(x => new DbUpdate(x))
                .Where(x => x.Version > currentVersion)
                .OrderBy(x => x.Version)
                .ToList();
        }





        /////
        ///
        public static int Main(string[] args)
        {
            var connectionString = GetConnectionString(args);

            EnsureDatabaseExists(connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(
                        Assembly.Load("WC.Database.Scripts"),
                        s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                    .LogToConsole()
                    .Build();

            var scriptsToExecute = upgrader.GetScriptsToExecute();

            Console.WriteLine("Pending scripts:");
            foreach (var script in scriptsToExecute)
            {
                Console.WriteLine($" - {script.Name}");
            }

            var validation = DbVersionValidator.Validate(scriptsToExecute.Select(s => s.Name).ToList(), connectionString);
            if (!validation.Success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(validation.ErrorMessage);
                Console.ResetColor();
                return -2;
            }

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database upgrade completed successfully.");
            Console.ResetColor();

            return 0;
        }

        private static string GetConnectionString(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                throw new ArgumentException("Connection string argument is missing.");

            return args[0];
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            //DbUp.Engine.Transactions.DatabaseUpgradeExtensions.EnsureDatabase.For.SqlDatabase(connectionString);
        }
    }
}
