using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using WC.Database.Tools.Models;

namespace WC.Database.Tools.Utils
{
    public static class DbUpdateValidator
    {
        public static ValidationResult Validate(IReadOnlyList<DbUpdateScript> allScripts, SqlConnection connection)
        {
            if (allScripts.Count == 0)
                return ValidationResult.Fail("No SQL migration scripts were found.");

            var duplicateVersions = allScripts
                .GroupBy(x => x.Version)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key.ToString())
                .ToList();

            if (duplicateVersions.Count > 0)
            {
                return ValidationResult.Fail(
                    $"Duplicate migration versions found: {string.Join(", ", duplicateVersions)}");
            }

            var applied = DbConnectionHelper.GetAppliedScripts(connection);

            foreach (var appliedScript in applied)
            {
                var currentScript = allScripts.FirstOrDefault(x => x.Name.Equals(appliedScript.ScriptName, StringComparison.OrdinalIgnoreCase));
                if (currentScript is null)
                    continue;

                if (!string.Equals(currentScript.ChecksumSha256, appliedScript.ChecksumSha256, StringComparison.OrdinalIgnoreCase))
                {
                    return ValidationResult.Fail(
                        $"Checksum mismatch for already applied script '{appliedScript.ScriptName}'. " +
                        $"The file appears to have been modified after deployment.");
                }
            }

            if (applied.Count > 0)
            {
                var maxApplied = applied.Max(x => x.Version);

                var invalidPending = allScripts
                    .Where(x => applied.All(a => !a.ScriptName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                    .Where(x => x.Version <= maxApplied)
                    .Select(x => $"{x.Name} ({x.Version})")
                    .ToList();

                if (invalidPending.Count > 0)
                {
                    return ValidationResult.Fail(
                        "Found not-yet-applied migration(s) older than or equal to current database version. " +
                        $"Latest applied version is {maxApplied}. Invalid pending scripts: {string.Join(", ", invalidPending)}");
                }
            }

            return ValidationResult.Ok();
        }

        public sealed record ValidationResult(bool Success, string? ErrorMessage)
        {
            public static ValidationResult Ok() => new(true, null);
            public static ValidationResult Fail(string error) => new(false, error);
        }
    }
}
