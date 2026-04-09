using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WC.Database.Tools
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public T ExecuteScalar<T>(string commandText, params SqlParameter[] parameters)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, cnn)
                {
                    CommandType = CommandType.Text
                };
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cnn.Open();

                return (T)cmd.ExecuteScalar();
            }
        }
        public bool ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(commandText, cnn)
                    {
                        CommandType = CommandType.Text
                    };
                    foreach (SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                //DbMigrationStatics.OutputMessage(ex.Message);
                return false;
            }
        }
        public bool ExecuteCommand(string commandText, string customConnString = "")
        {
            try
            {
                Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string[] subCommands = regex.Split(commandText);

                using (SqlConnection connection = new SqlConnection(String.IsNullOrEmpty(customConnString) ? _connectionString : customConnString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.Connection = connection;
                        cmd.Transaction = transaction;

                        foreach (string command in subCommands)
                        {
                            if (command.Trim().Length <= 0)
                                continue;

                            cmd.CommandText = command;
                            cmd.CommandType = CommandType.Text;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException migrationException)
                            {
                                //DbMigrationStatics.OutputMessage(migrationException.Message);
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //DbMigrationStatics.OutputMessage(ex.Message);
                return false;
            }
        }
        public string RemoveDatabaseFromConnectionString(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (connectionStringBuilder.ContainsKey("Initial Catalog"))
            {
                connectionStringBuilder.Remove("Initial Catalog");
            }
            if (connectionStringBuilder.ContainsKey("Database"))
            {
                connectionStringBuilder.Remove("Database");
            }
            return connectionStringBuilder.ToString();
        }
        public string GetDatabaseNameFromConnectionString(string connectionString)
        {
            SqlConnection sql = new SqlConnection(connectionString);
            return sql.Database;
        }
    }
}
