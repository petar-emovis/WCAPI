namespace WC.Database.Tools
{
    public class DBService
    {
        private readonly string? _connectionString;
        public DBService()
        {

        }
        public DBService(string connectionString)
        {
            _connectionString = connectionString;
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
            //if (String.IsNullOrEmpty(_connectionString))
            //{
            //    throw new Exception("Connection string not set. Please set database connection string.");
            //}

            //string message = "";

            //var output = new List<string>();
            //var currentVersion = GetCurrentVersion();
            //$"Current DB schema version is {currentVersion}");

            //IReadOnlyList<Migration> migrations = GetNewMigrations(currentVersion);
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

        //private int GetCurrentVersion()
        //{
        //    if (!DoesVersionTableExists())
        //    {
        //        CreateVersionTable();
        //        return 0;
        //    }
        //    return GetCurrentVersionFromSettingsTable();
        //}
    }
}
