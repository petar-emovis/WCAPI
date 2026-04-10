using DbUp;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text.RegularExpressions;
using WC.Database.Tools.Models;
using Microsoft.Data.SqlClient;
using WC.Database.Tools.Utils;

namespace WC.Database.Tools
{
    public class DBService
    {        
        //private readonly DbHelper _dbHelper;

        public DBService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string not set. Please set database connection string.");
            }
            //_dbHelper = new DbHelper(connectionString);
        }

        public async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
       
        public async Task<bool> UpdateDatabaseAsync(string connectionString, DbLogger dbLogger)
        {            
            try
            {
                //var connectionString = ArgumentHelper.GetConnectionString(args);
                EnsureDatabase.For.SqlDatabase(connectionString);

                using var connection = DbConnectionHelper.CreateOpen(connectionString);

                DbConnectionHelper.EnsureLogTableExists(connection);

                var assembly = Assembly.Load("WC.Database.Scripts");
                var scripts = EmbeddedScriptLoader.LoadSqlScripts(assembly);

                var validation = DbUpdateValidator.Validate(scripts, connection);
                if (!validation.Success)
                {
                    dbLogger.Info(validation.ErrorMessage);
                    return false;
                }

                var pendingScripts = EmbeddedScriptLoader.GetPendingScripts(scripts, connection);

                if (pendingScripts.Count == 0)
                {
                    dbLogger.Info("No pending database scripts.");
                    return false;
                }

                dbLogger.Info("Pending scripts:");
                foreach (var script in pendingScripts)
                {
                    dbLogger.Info($" - {script.Name}");
                }

                var runner = new DbHelper(connectionString, connection);
                var result = runner.Run(pendingScripts);

                if (!result.Success)
                {
                    dbLogger.Info(result.ErrorMessage);
                    return false;
                }

                dbLogger.Info("Database upgrade completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                dbLogger.Info(ex.Message);
                dbLogger.Info(ex.StackTrace);
                return false;
            }
        }
    }
}
