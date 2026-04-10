using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WC.Database.Tools.Models;

namespace WC.Database.Tools.Utils
{
    internal static class EmbeddedScriptLoader
    {
        private static readonly Regex NameRegex =
            new(@"(?<major>\d+)_(?<minor>\d+)_(?<patch>\d+)__.+\.sql$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static IReadOnlyList<DbUpdateScript> LoadSqlScripts(Assembly assembly)
        {
            var resourceNames = assembly
                .GetManifestResourceNames()
                .Where(x => x.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var result = new List<DbUpdateScript>();

            foreach (var resourceName in resourceNames)
            {
                using var stream = assembly.GetManifestResourceStream(resourceName)
                    ?? throw new InvalidOperationException($"Cannot open resource stream for '{resourceName}'.");

                using var reader = new StreamReader(stream, Encoding.UTF8);
                var sql = reader.ReadToEnd();

                var fileName = resourceName.Split('.').Reverse().Take(4).Reverse();
                var reconstructedFileName = string.Join(".", fileName);

                var version = ParseVersion(reconstructedFileName);

                result.Add(new DbUpdateScript
                {
                    Name = reconstructedFileName,
                    ResourceName = resourceName,
                    Sql = sql,
                    Version = version,
                    ChecksumSha256 = ComputeSha256(sql)
                });
            }

            return result.OrderBy(x => x.Version).ThenBy(x => x.Name).ToList();
        }

        private static Version ParseVersion(string fileName)
        {
            var match = NameRegex.Match(fileName);
            if (!match.Success)
                throw new InvalidOperationException(
                    $"Migration file '{fileName}' does not match required format V{{major}}_{{minor}}_{{patch}}__Description.sql");

            var major = int.Parse(match.Groups["major"].Value);
            var minor = int.Parse(match.Groups["minor"].Value);
            var patch = int.Parse(match.Groups["patch"].Value);

            return new Version(major, minor, patch);
        }

        private static string ComputeSha256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        public static IReadOnlyList<DbUpdateScript> GetPendingScripts(
        IReadOnlyList<DbUpdateScript> allScripts,
        SqlConnection connection)
        {
            var appliedNames = DbConnectionHelper.GetAppliedScripts(connection)
                .Select(x => x.ScriptName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return allScripts
                .Where(x => !appliedNames.Contains(x.Name))
                .OrderBy(x => x.Version)
                .ThenBy(x => x.Name)
                .ToList();
        }
    }
}
