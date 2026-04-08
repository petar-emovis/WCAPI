using DbUp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WC.Database.Tools;

namespace WC.DbConfig
{
    public partial class frmMain : Form
    {
        private DBService? _dbService;
        private bool _success;
        public frmMain()
        {
            InitializeComponent();
            //_dbService = new DBService();
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                _dbService = new DBService(txtConnectionString.Text);

                TestPrepare();
                _success = await _dbService.TestConnectionAsync(txtConnectionString.Text);

                if (_success)
                {
                    TestSuccessful();
                }
                else
                {
                    TestFailed();
                }
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool isLoading)
        {
            pnlContent.Enabled = !isLoading;
            pnlLoading.Visible = isLoading;
            Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
        }
        private void TestPrepare()
        {
            lblTestIcon.Text = "";
            lblTestIcon.Visible = false;
            lblTestInfo.Visible = false;
            SetLoading(true);
        }
        private void TestSuccessful()
        {
            lblTestIcon.Text = "✓";
            lblTestIcon.ForeColor = Color.Green;
            lblTestIcon.Visible = lblTestInfo.Visible = true;
            lblTestInfo.Text = "Test successful!";
            btnUpgrade.Enabled = true;
        }
        private void TestFailed()
        {
            lblTestIcon.Text = "X";
            lblTestIcon.ForeColor = Color.Red;
            lblTestIcon.Visible = lblTestInfo.Visible = true;
            lblTestInfo.Text = "Test failed!";
            btnUpgrade.Enabled = false;
        }

        private async Task btnUpgrade_Click(object sender, EventArgs e)
        {
            if (_dbService == null)
            {
                TestFailed();
                return;
            }

            try
            {
                TestPrepare();

                _success = await _dbService.UpdateDatabaseAsync();

                if (_success)
                {
                    TestSuccessful();
                }
                else
                {
                    TestFailed();
                }
            }
            finally
            {
                SetLoading(false);
            }
        }


        public static int Main(string[] args)
        {
            var connectionString = GetConnectionString(args);

            EnsureDatabaseExists(connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
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

            var validation = DbVersionValidator.Validate(scriptsToExecute.Select(s => s.Name).ToList(), connectionString);
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database upgrade completed successfully.");
            Console.ResetColor();

            return 0;
        }

        private static string GetConnectionString(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                throw new ArgumentException("Connection string argument is missing.");

            return args[0];
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            //DbUp.Engine.Transactions.DatabaseUpgradeExtensions.EnsureDatabase.For.SqlDatabase(connectionString);
        }

    }
}
