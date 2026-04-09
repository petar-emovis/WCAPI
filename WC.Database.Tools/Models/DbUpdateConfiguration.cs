using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Database.Tools.Models
{
    public class DbUpdateConfiguration
    {
        private string _versionTableSchema = "dbo";
        private string _versionTableName = "SchemaVersions";
        private string _migrationRoot = @"DbUpdate\";
        private string _rollBackRoot = @"RollBack\";

        public string? InititalDbCreationFilePath { get; set; }
        public string VersionTableSchema
        {
            get
            {
                return _versionTableSchema;
            }
            set
            {
                _versionTableSchema = value;
            }
        }
        public string VersionTableName
        {
            get
            {
                return _versionTableName;
            }
            set
            {
                _versionTableSchema = value;
            }
        }
        public string MigrationRoot
        {
            get
            {
                return _migrationRoot;
            }
            set
            {
                _migrationRoot = value;
            }
        }
        public string RollBackRoot
        {
            get
            {
                return _rollBackRoot;
            }
            set
            {
                _rollBackRoot = value;
            }
        }
        //public bool AlowConsoleOutput
        //{
        //    get
        //    {
        //        return DbMigrationStatics.AllowConsoleOutput;
        //    }
        //    set
        //    {
        //        DbMigrationStatics.AllowConsoleOutput = value;
        //    }
        //}
        public string? ConnectionString { get; set; }
        public DbUpdateConfiguration(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public DbUpdateConfiguration(string connectionString,
            string? versionTableSchema = null, string? versionTableName = null,
            string? migrationRoot = null, string? rollbackRoot = null)
        {
            this.ConnectionString = connectionString;
            if (!string.IsNullOrEmpty(versionTableName))
            {
                this.VersionTableName = versionTableName;
            }
            if (!String.IsNullOrEmpty(versionTableSchema))
            {
                this.VersionTableSchema = versionTableSchema;
            }
            if (!String.IsNullOrEmpty(migrationRoot))
            {
                this.MigrationRoot = migrationRoot;
            }
            if (!String.IsNullOrEmpty(rollbackRoot))
            {
                this.RollBackRoot = rollbackRoot;
            }
        }
    }
}
