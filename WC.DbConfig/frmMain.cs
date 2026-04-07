using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
    }
}
