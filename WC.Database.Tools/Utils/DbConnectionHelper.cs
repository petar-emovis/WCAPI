using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WC.Database.Tools.Models;

namespace WC.Database.Tools.Utils
{
    internal static class DbConnectionHelper
    {
        public static SqlConnection CreateOpen(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static void EnsureLogTableExists(SqlConnection connection)
        {
            const string sql = """
        IF NOT EXISTS (
            SELECT 1
            FROM sys.tables t
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE t.name = 'DbUpdateLog' AND s.name = 'dbo'
        )
        BEGIN
            CREATE TABLE dbo.DbUpdateLog
            (
                Id               INT IDENTITY(1,1) PRIMARY KEY,
                ScriptName       NVARCHAR(255) NOT NULL,
                ScriptVersion    NVARCHAR(50) NOT NULL,
                ChecksumSha256   NVARCHAR(64) NOT NULL,
                AppliedOnUtc     DATETIME2 NOT NULL,
                AppliedBy        NVARCHAR(128) NULL,
                ExecutionMs      INT NULL,
                Success          BIT NOT NULL
            );

            CREATE UNIQUE INDEX UX_DbUpdateLog_ScriptName
                ON dbo.DbUpdateLog (ScriptName);
        END
        """;

            using var cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        public static IReadOnlyList<AppliedScriptInfo> GetAppliedScripts(SqlConnection connection)
        {
            const string sql = """
        SELECT ScriptName, ScriptVersion, ChecksumSha256
        FROM dbo.DbUpdateLog
        WHERE Success = 1
        ORDER BY Id
        """;

            using var cmd = new SqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            var result = new List<AppliedScriptInfo>();

            while (reader.Read())
            {
                result.Add(new AppliedScriptInfo
                {
                    ScriptName = reader.GetString(0),
                    Version = Version.Parse(reader.GetString(1)),
                    ChecksumSha256 = reader.GetString(2)
                });
            }

            return result;
        }

    }
}
