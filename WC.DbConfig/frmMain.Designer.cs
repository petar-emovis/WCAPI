namespace WC.Database
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtConnectionString = new TextBox();
            lblConnectionString = new Label();
            btnTestConnection = new Button();
            lblTestIcon = new Label();
            lblTestInfo = new Label();
            pnlContent = new Panel();
            pnlLoading = new Panel();
            txtLog = new TextBox();
            lblUpdateInfo = new Label();
            lblUpdateIcon = new Label();
            btnUpdate = new Button();
            label1 = new Label();
            cbxUpgrade = new ComboBox();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // txtConnectionString
            // 
            txtConnectionString.Location = new Point(10, 31);
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.Size = new Size(371, 23);
            txtConnectionString.TabIndex = 0;
            // 
            // lblConnectionString
            // 
            lblConnectionString.AutoSize = true;
            lblConnectionString.Location = new Point(10, 13);
            lblConnectionString.Name = "lblConnectionString";
            lblConnectionString.Size = new Size(102, 15);
            lblConnectionString.TabIndex = 2;
            lblConnectionString.Text = "Connection string";
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new Point(10, 60);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(115, 23);
            btnTestConnection.TabIndex = 3;
            btnTestConnection.Text = "Test connection";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // lblTestIcon
            // 
            lblTestIcon.AutoSize = true;
            lblTestIcon.Location = new Point(131, 64);
            lblTestIcon.Name = "lblTestIcon";
            lblTestIcon.Size = new Size(14, 15);
            lblTestIcon.TabIndex = 4;
            lblTestIcon.Text = "X";
            lblTestIcon.Visible = false;
            // 
            // lblTestInfo
            // 
            lblTestInfo.AutoSize = true;
            lblTestInfo.Location = new Point(151, 64);
            lblTestInfo.Name = "lblTestInfo";
            lblTestInfo.Size = new Size(101, 15);
            lblTestInfo.TabIndex = 5;
            lblTestInfo.Text = "Failed to connect!";
            lblTestInfo.Visible = false;
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(pnlLoading);
            pnlContent.Controls.Add(txtLog);
            pnlContent.Controls.Add(lblUpdateInfo);
            pnlContent.Controls.Add(lblUpdateIcon);
            pnlContent.Controls.Add(btnUpdate);
            pnlContent.Controls.Add(label1);
            pnlContent.Controls.Add(cbxUpgrade);
            pnlContent.Controls.Add(lblConnectionString);
            pnlContent.Controls.Add(lblTestInfo);
            pnlContent.Controls.Add(txtConnectionString);
            pnlContent.Controls.Add(lblTestIcon);
            pnlContent.Controls.Add(btnTestConnection);
            pnlContent.Location = new Point(1, 1);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(401, 284);
            pnlContent.TabIndex = 6;
            // 
            // pnlLoading
            // 
            pnlLoading.BackgroundImage = Properties.Resources.loading_image;
            pnlLoading.BackgroundImageLayout = ImageLayout.Center;
            pnlLoading.Location = new Point(161, 124);
            pnlLoading.Name = "pnlLoading";
            pnlLoading.Size = new Size(75, 70);
            pnlLoading.TabIndex = 6;
            pnlLoading.Visible = false;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(11, 131);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(370, 143);
            txtLog.TabIndex = 12;
            // 
            // lblUpdateInfo
            // 
            lblUpdateInfo.AutoSize = true;
            lblUpdateInfo.Location = new Point(151, 106);
            lblUpdateInfo.Name = "lblUpdateInfo";
            lblUpdateInfo.Size = new Size(95, 15);
            lblUpdateInfo.TabIndex = 11;
            lblUpdateInfo.Text = "Failed to update!";
            lblUpdateInfo.Visible = false;
            // 
            // lblUpdateIcon
            // 
            lblUpdateIcon.AutoSize = true;
            lblUpdateIcon.Location = new Point(131, 106);
            lblUpdateIcon.Name = "lblUpdateIcon";
            lblUpdateIcon.Size = new Size(14, 15);
            lblUpdateIcon.TabIndex = 10;
            lblUpdateIcon.Text = "X";
            lblUpdateIcon.Visible = false;
            // 
            // btnUpdate
            // 
            btnUpdate.Enabled = false;
            btnUpdate.Location = new Point(11, 102);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(115, 23);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "Update database";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpgrade_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(260, 85);
            label1.Name = "label1";
            label1.Size = new Size(106, 15);
            label1.TabIndex = 8;
            label1.Text = "Version to upgrade";
            label1.Visible = false;
            // 
            // cbxUpgrade
            // 
            cbxUpgrade.FormattingEnabled = true;
            cbxUpgrade.Location = new Point(260, 103);
            cbxUpgrade.Name = "cbxUpgrade";
            cbxUpgrade.Size = new Size(121, 23);
            cbxUpgrade.TabIndex = 7;
            cbxUpgrade.Visible = false;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(404, 287);
            Controls.Add(pnlContent);
            Name = "frmMain";
            Text = "DbConfig";
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtConnectionString;
        private Label lblConnectionString;
        private Button btnTestConnection;
        private Label lblTestIcon;
        private Label lblTestInfo;
        private Panel pnlContent;
        private Panel pnlLoading;
        private Button btnUpdate;
        private Label label1;
        private ComboBox cbxUpgrade;
        private Label lblUpdateInfo;
        private Label lblUpdateIcon;
        private TextBox txtLog;
    }
}