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
using WC.Database.Tools.Utils;

namespace WC.Database
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

                PrepareInfo();
                _success = await _dbService.TestConnectionAsync(txtConnectionString.Text);

                if (_success)
                {
                    InfoSuccessful();
                }
                else
                {
                    InfoFailed();
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
        private void PrepareInfo()
        {
            lblTestIcon.Text = "";
            lblTestIcon.Visible = false;
            lblTestInfo.Visible = false;
            lblUpdateIcon.Visible = false;
            lblUpdateInfo.Visible = false;
            SetLoading(true);
        }
        private void InfoSuccessful(bool test = true)
        {
            if (test)
            {
                lblTestIcon.Text = "✓";
                lblTestIcon.ForeColor = Color.Green;
                lblTestIcon.Visible = lblTestInfo.Visible = true;
                lblTestInfo.Text = "Test successful!";
                btnUpdate.Enabled = true;
            }
            else
            {
                lblUpdateIcon.Text = "✓";
                lblUpdateIcon.ForeColor = Color.Green;
                lblUpdateIcon.Visible = lblUpdateInfo.Visible = true;
                lblUpdateInfo.Text = "Update successful!";
            }
        }
        private void InfoFailed(bool test = true)
        {
            if (test)
            {
                lblTestIcon.Text = "X";
                lblTestIcon.ForeColor = Color.Red;
                lblTestIcon.Visible = lblTestInfo.Visible = true;
                lblTestInfo.Text = "Test failed!";
                btnUpdate.Enabled = false;
            }
            else
            {
                lblUpdateIcon.Text = "X";
                lblUpdateIcon.ForeColor = Color.Red;
                lblUpdateIcon.Visible = lblUpdateInfo.Visible = true;
                lblUpdateInfo.Text = "Update failed!";
            }
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            if (_dbService == null)
            {
                InfoFailed(false);
                return;
            }

            try
            {
                PrepareInfo();

                DbLogger dbLogger = new DbLogger("UpdateDatabaseAsync started");

                //_success = await _dbService.UpdateDatabaseAsync();
                _success = await _dbService.UpdateDatabaseAsync(txtConnectionString.Text, dbLogger);

                if (dbLogger != null)
                    txtLog.Text = dbLogger.Text;

                if (_success)
                {
                    InfoSuccessful(false);
                }
                else
                {
                    InfoFailed(false);
                }
            }
            finally
            {
                SetLoading(false);
            }
        }
    }
}
