namespace WC.DbConfig
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
            SuspendLayout();
            // 
            // txtConnectionString
            // 
            txtConnectionString.Location = new Point(12, 32);
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.Size = new Size(371, 23);
            txtConnectionString.TabIndex = 0;
            // 
            // lblConnectionString
            // 
            lblConnectionString.AutoSize = true;
            lblConnectionString.Location = new Point(12, 14);
            lblConnectionString.Name = "lblConnectionString";
            lblConnectionString.Size = new Size(102, 15);
            lblConnectionString.TabIndex = 2;
            lblConnectionString.Text = "Connection string";
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new Point(12, 61);
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
            lblTestIcon.Location = new Point(133, 65);
            lblTestIcon.Name = "lblTestIcon";
            lblTestIcon.Size = new Size(14, 15);
            lblTestIcon.TabIndex = 4;
            lblTestIcon.Text = "X";
            lblTestIcon.Visible = false;
            // 
            // lblTestInfo
            // 
            lblTestInfo.AutoSize = true;
            lblTestInfo.Location = new Point(153, 65);
            lblTestInfo.Name = "lblTestInfo";
            lblTestInfo.Size = new Size(101, 15);
            lblTestInfo.TabIndex = 5;
            lblTestInfo.Text = "Failed to connect!";
            lblTestInfo.Visible = false;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(404, 287);
            Controls.Add(lblTestInfo);
            Controls.Add(lblTestIcon);
            Controls.Add(btnTestConnection);
            Controls.Add(lblConnectionString);
            Controls.Add(txtConnectionString);
            Name = "frmMain";
            Text = "DbConfig";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtConnectionString;
        private Label lblConnectionString;
        private Button btnTestConnection;
        private Label lblTestIcon;
        private Label lblTestInfo;
    }
}