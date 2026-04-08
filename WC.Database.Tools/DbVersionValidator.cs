using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WC.Database.Tools
{
    public static class DbVersionValidator
    {
        private static readonly Regex VersionRegex =
            new(@"V(?<major>\d+)_(?<minor>\d+)_(?<patch>\d+)__.+\.sql$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static ValidationResult Validate(IReadOnlyList<string> pendingScriptNames, string connectionString)
        {
            var parsedPending = pendingScriptNames
                .Select(ParseVersion)
                .OrderBy(x => x.Version)
                .ToList();

            if (parsedPending.Count != pendingScriptNames.Count)
                return ValidationResult.Fail("One or more migration files do not follow naming convention V{major}_{minor}_{patch}__Description.sql");

            var duplicateVersions = parsedPending
                .GroupBy(x => x.Version)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateVersions.Count > 0)
                return ValidationResult.Fail($"Duplicate migration versions found: {string.Join(", ", duplicateVersions)}");

            var appliedVersions = LoadAppliedVersions(connectionString);

            if (appliedVersions.Count == 0)
                return ValidationResult.Ok();

            var maxApplied = appliedVersions.Max();

            var olderPending = parsedPending
                .Where(x => x.Version <= maxApplied)
                .Select(x => $"{x.Name} ({x.Version})")
                .ToList();

            if (olderPending.Count > 0)
            {
                return ValidationResult.Fail(
                    "Found pending migration(s) older than or equal to already applied database version. " +
                    $"Latest applied version: {maxApplied}. Invalid pending migrations: {string.Join(", ", olderPending)}");
            }

            return ValidationResult.Ok();
        }

        private static ParsedMigration? ParseVersion(string scriptName)
        {
            var fileName = Path.GetFileName(scriptName);
            var match = VersionRegex.Match(fileName);

            if (!match.Success)
                return null;

            var major = int.Parse(match.Groups["major"].Value);
            var minor = int.Parse(match.Groups["minor"].Value);
            var patch = int.Parse(match.Groups["patch"].Value);

            return new ParsedMigration(new Version(major, minor, patch), fileName);
        }

        private static List<Version> LoadAppliedVersions(string connectionString)
        {
            var result = new List<Version>();

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            if (!SchemaVersionsTableExists(connection))
                return result;

            const string sql = """
            SELECT [ScriptName]
            FROM [dbo].[SchemaVersions]
            """;

            using var cmd = new SqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var scriptName = reader.GetString(0);
                var parsed = ParseVersion(scriptName);
                if (parsed is not null)
                    result.Add(parsed.Version);
            }

            return result;
        }

        private static bool SchemaVersionsTableExists(SqlConnection connection)
        {
            const string sql = """
            SELECT CASE 
                     WHEN EXISTS (
                         SELECT 1
                         FROM sys.tables t
                         INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                         WHERE t.name = 'SchemaVersions' AND s.name = 'dbo'
                     ) 
                     THEN 1 ELSE 0 
                   END
            """;

            using var cmd = new SqlCommand(sql, connection);
            return (int)cmd.ExecuteScalar()! == 1;
        }

        public sealed record ParsedMigration(Version Version, string Name);

        public sealed record ValidationResult(bool Success, string? ErrorMessage)
        {
            public static ValidationResult Ok() => new(true, null);
            public static ValidationResult Fail(string error) => new(false, error);
        }
    }
}
