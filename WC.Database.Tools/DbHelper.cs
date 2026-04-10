using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WC.Database.Tools.Models;

namespace WC.Database.Tools
{
    public class DbHelper
    {
        private readonly string _connectionString;
        private readonly SqlConnection _dbConnection;

        public DbHelper(string connectionString, SqlConnection dbConnection)
        {
            _connectionString = connectionString;
            _dbConnection = dbConnection;
        }

        public UpgradeResult Run(IReadOnlyList<DbUpdateScript> pendingScripts)
        {
            //using var connection = new SqlConnection(_connectionString);
            //connection.Open();

            foreach (var script in pendingScripts)
            {
                try
                {                    
                    var sw = Stopwatch.StartNew();
                    
                    using var transaction = _dbConnection.BeginTransaction();

                    foreach (var batch in SplitBatches(script.Sql))
                    {
                        using (var cmd = new SqlCommand(batch, _dbConnection, transaction))
                        //using (var cmd = new SqlCommand(script.Sql, connection, transaction))
                        {
                            cmd.CommandTimeout = 120;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    
                    transaction.Commit();
                    sw.Stop();

                    InsertLog(script, sw.ElapsedMilliseconds, true);
                }
                catch (Exception ex)
                {
                    InsertLog(script, null, false); //možda posli dodat try catch
                    return UpgradeResult.Fail($"Script '{script.Name}' failed. {ex.Message}");
                }
            }

            return UpgradeResult.Ok();
        }

        private void InsertLog(DbUpdateScript script, long? elapsedMs, bool success)
        {
            const string sql = """
                            INSERT INTO dbo.DbUpdateLog
                            (
                                ScriptName,
                                ScriptVersion,
                                ChecksumSha256,
                                AppliedOnUtc,
                                AppliedBy,
                                ExecutionMs,
                                Success
                            )
                            VALUES
                            (
                                @ScriptName,
                                @ScriptVersion,
                                @ChecksumSha256,
                                @AppliedOnUtc,
                                @AppliedBy,
                                @ExecutionMs,
                                @Success
                            )
                            """;

            using var cmd = new SqlCommand(sql, _dbConnection);
            cmd.Parameters.AddWithValue("@ScriptName", script.Name);
            cmd.Parameters.AddWithValue("@ScriptVersion", script.Version.ToString());
            cmd.Parameters.AddWithValue("@ChecksumSha256", script.ChecksumSha256);
            cmd.Parameters.AddWithValue("@AppliedOnUtc", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@AppliedBy", Environment.UserName);
            cmd.Parameters.AddWithValue("@ExecutionMs", (object?)elapsedMs ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Success", success);

            cmd.ExecuteNonQuery();
        }
        private static IEnumerable<string> SplitBatches(string sql)
        {
            return Regex.Split(
                sql,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }
    }
}
public sealed record UpgradeResult(bool Success, string? ErrorMessage)
{
    public static UpgradeResult Ok() => new(true, null);
    public static UpgradeResult Fail(string message) => new(false, message);
}
