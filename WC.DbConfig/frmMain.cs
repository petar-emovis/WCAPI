using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WC.DbConfig
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            lblTestIcon.Text = "";
            lblTestIcon.Visible = false;
            lblTestInfo.Visible = false;

            if (TestConnection(txtConnectionString.Text))
            {
                lblTestIcon.Text = "✓";
                lblTestIcon.ForeColor = Color.Green;
                lblTestIcon.Visible = lblTestInfo.Visible = true;
                lblTestInfo.Text = "Test successful!";
            }
            else
            {
                lblTestIcon.Text = "X";
                lblTestIcon.ForeColor = Color.Red;
                lblTestIcon.Visible = lblTestInfo.Visible = true;
                lblTestInfo.Text = "Test failed!";
            }
        }

        private bool TestConnection(string conn)
        {
            try
            {
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(conn);
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
