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

        #region Test 1
        /*
        public async Task<bool> UpdateDatabaseAsync2()
        {
            if (String.IsNullOrEmpty(_dbConfig.ConnectionString))
            {
                throw new Exception("Connection string not set. Please set database connection string.");
            }

            string message = "";

            var output = new List<string>();
            var currentVersion = GetCurrentVersion();
            message = $"Current DB schema version is {currentVersion}";

            IReadOnlyList<DbUpdate> dbUpdates = GetNewDbUpdates(currentVersion);
            //OutputMessage($"{migrations.Count} migration(s) found.");

            var duplicatedVersion = GetDuplicatedVersion(dbUpdates);
            if (duplicatedVersion != null)
            {
                //OutputMessage($"Non-unique migration found: {duplicatedVersion}.");
                return false;
            }

            foreach (DbUpdate dbUpdate in dbUpdates)
            {
                if (!_dbHelper.ExecuteCommand(dbUpdate.GetContent()))
                {
                    //DbMigrationStatics.OutputMessage($"DB upgrade failed trying to update to {migration.Version}.\r\n" +
                    //    $" Execution of script {migration.FilePath} failed!");
                    return false;
                }
                UpdateVersion(dbUpdate.Version);
                //OutputMessage($"Database updated to version {migration.Version}.");
                //OutputMessage($"Executed migration: {migration.Name}.");
            }

            if (!dbUpdates.Any())
            {
                //OutputMessage("No updates for the current schema version.");
            }
            else
            {
                var newVersion = dbUpdates.Last().Version;
                //OutputMessage($"New DB schema version is {newVersion}.");
            }

            return true;
        }

        public async Task<bool> RollbackDatabaseAsync(int rollbackVersion)
        {
            var output = new List<string>();
            var currentVersion = GetCurrentVersion();
            //OutputMessage($"Current DB schema version is {currentVersion}");

            IReadOnlyList<DbUpdate> dbUpdates = GetRollbacks(rollbackVersion, currentVersion);
            //OutputMessage($"{migrations.Count} rollback(s) found.");

            var duplicatedVersion = GetDuplicatedVersion(dbUpdates);
            if (duplicatedVersion != null)
            {
                //OutputMessage($"Non-unique rollback found: {duplicatedVersion}.");
                return false;
            }

            foreach (DbUpdate dbUpdate in dbUpdates)
            {
                if (!_dbHelper.ExecuteCommand(dbUpdate.GetContent()))
                {
                    //DbMigrationStatics.OutputMessage($"DB downgrade failed trying to downgrade to {migration.Version - 1}.\r\n" +
                    //    $" Execution of script {migration.FilePath} failed!");
                    return false;
                }
                UpdateVersion(dbUpdate.Version - 1);
                //OutputMessage($"Database downgraded to version {migration.Version - 1}.");
                //OutputMessage($"Executed rollback: {migration.Name}.");
            }

            if (!dbUpdates.Any())
            {
                //OutputMessage($"No rollbacks for the requested schema version {rollbackVersion}.");
            }
            else
            {
                var newVersion = GetCurrentVersion();
                //OutputMessage($"New DB schema version is {newVersion}.");
            }

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
            var version = _dbHelper.ExecuteScalar<int>($"SELECT TOP 1 ScriptVersion FROM " +
                $"{_dbConfig.VersionTableSchema}.{_dbConfig.VersionTableName}" +
                $" ORDER BY Id DESC'");

            return version;
        }
        private IReadOnlyList<DbUpdate> GetNewDbUpdates(int currentVersion)
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
        private IReadOnlyList<DbUpdate> GetRollbacks(int rollbackVersion, int currentVersion)
        {
            var regex = new Regex(@"^(\d)*_(.*)(sql)$");

            return new DirectoryInfo(_dbConfig.RollBackRoot)
                .GetFiles()
                .Where(x => regex.IsMatch(x.Name))
                .Select(x => new DbUpdate(x))
                .Where(x => x.Version > rollbackVersion && x.Version <= currentVersion)
                .OrderByDescending(x => x.Version)
                .ToList();
        }

        private int? GetDuplicatedVersion(IReadOnlyList<DbUpdate> migrations)
        {
            var duplicatedVersion = migrations
                .GroupBy(x => x.Version)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .FirstOrDefault();

            return duplicatedVersion == 0 ? (int?)null : duplicatedVersion;
        }

        private bool UpdateVersion(int newVersion)
        {
            try
            {
                _dbHelper.ExecuteNonQuery($@"UPDATE {_dbConfig.VersionTableSchema}.{_dbConfig.VersionTableName} 
            SET Value = @Version WHERE Name = 'Version'",
              new SqlParameter("Version", newVersion));

                return true;
            }
            catch (Exception ex)
            {
                //DbMigrationStatics.OutputMessage(ex.Message);
                return false;
            }
        }
        */
        #endregion

        /////
        ///
        public async Task<int> UpdateDatabaseAsync()
        {
            if (String.IsNullOrEmpty(_dbConfig.ConnectionString))
            {
                throw new Exception("Connection string not set. Please set database connection string.");
            }
            //var connectionString = GetConnectionString(_dbConfig.ConnectionString);

            EnsureDatabaseExists(_dbConfig.ConnectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(_dbConfig.ConnectionString)
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

            var pendingScriptNames = scriptsToExecute.Select(s => s.Name).ToList();

            var validation = DbVersionValidator.Validate(pendingScriptNames, _dbConfig.ConnectionString);
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

            var skripte = pendingScriptNames
                .Select(DbVersionValidator.ParseVersion)
                .OrderBy(x => x.Version)
                .ToList();

            //LogScripts(skripte.Select(i=>i.Version.))

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database upgrade completed successfully.");
            Console.ResetColor();

            return 0;
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);
        }

        private bool LogScripts(List<string> scripts)
        {
            try
            {
            //    _dbHelper.ExecuteNonQuery($@"UPDATE {_dbConfig.VersionTableSchema}.{_dbConfig.VersionTableName} 
            //SET Value = @Version WHERE Name = 'Version'",
            //  new SqlParameter("Version", newVersion));

                return true;
            }
            catch (Exception ex)
            {
                //DbMigrationStatics.OutputMessage(ex.Message);
                return false;
            }
        }
    }
}
