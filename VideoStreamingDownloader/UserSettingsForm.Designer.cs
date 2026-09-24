namespace VideoStreamingDownloader
{
    partial class UserSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserSettingsForm));
            this.SaveButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tbTempPath = new System.Windows.Forms.TextBox();
            this.nudSimultaneousDownloads = new System.Windows.Forms.NumericUpDown();
            this.tbDefaultPath = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btDefaultFolder = new System.Windows.Forms.Button();
            this.DowloadDirLabel = new System.Windows.Forms.Label();
            this.btTempFolder = new System.Windows.Forms.Button();
            this.SimultaneousDownloadsLabel = new System.Windows.Forms.Label();
            this.TempDirLabel = new System.Windows.Forms.Label();
            this.LanguageLabel = new System.Windows.Forms.Label();
            this.LanguageComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSimultaneousDownloads)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(503, 470);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(226, 47);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Guardar i tancar";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // tbTempPath
            // 
            this.tbTempPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTempPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTempPath.Location = new System.Drawing.Point(6, 43);
            this.tbTempPath.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.tbTempPath.Name = "tbTempPath";
            this.tbTempPath.ReadOnly = true;
            this.tbTempPath.Size = new System.Drawing.Size(666, 36);
            this.tbTempPath.TabIndex = 1;
            // 
            // nudSimultaneousDownloads
            // 
            this.nudSimultaneousDownloads.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSimultaneousDownloads.Location = new System.Drawing.Point(6, 213);
            this.nudSimultaneousDownloads.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.nudSimultaneousDownloads.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudSimultaneousDownloads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSimultaneousDownloads.Name = "nudSimultaneousDownloads";
            this.nudSimultaneousDownloads.Size = new System.Drawing.Size(96, 36);
            this.nudSimultaneousDownloads.TabIndex = 2;
            this.nudSimultaneousDownloads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tbDefaultPath
            // 
            this.tbDefaultPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDefaultPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDefaultPath.Location = new System.Drawing.Point(6, 128);
            this.tbDefaultPath.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.tbDefaultPath.Name = "tbDefaultPath";
            this.tbDefaultPath.ReadOnly = true;
            this.tbDefaultPath.Size = new System.Drawing.Size(666, 36);
            this.tbDefaultPath.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.Controls.Add(this.btDefaultFolder, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.DowloadDirLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btTempFolder, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.SimultaneousDownloadsLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbTempPath, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbDefaultPath, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.TempDirLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.nudSimultaneousDownloads, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.LanguageLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.LanguageComboBox, 0, 7);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(717, 452);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btDefaultFolder
            // 
            this.btDefaultFolder.BackgroundImage = global::VideoStreamingDownloader.Properties.Resources._5994710;
            this.btDefaultFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btDefaultFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btDefaultFolder.Location = new System.Drawing.Point(678, 128);
            this.btDefaultFolder.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.btDefaultFolder.Name = "btDefaultFolder";
            this.btDefaultFolder.Size = new System.Drawing.Size(36, 36);
            this.btDefaultFolder.TabIndex = 6;
            this.btDefaultFolder.UseVisualStyleBackColor = true;
            this.btDefaultFolder.Click += new System.EventHandler(this.btDefaultFolder_Click);
            // 
            // DowloadDirLabel
            // 
            this.DowloadDirLabel.AutoSize = true;
            this.DowloadDirLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DowloadDirLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DowloadDirLabel.Location = new System.Drawing.Point(0, 85);
            this.DowloadDirLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.DowloadDirLabel.Name = "DowloadDirLabel";
            this.DowloadDirLabel.Size = new System.Drawing.Size(672, 40);
            this.DowloadDirLabel.TabIndex = 6;
            this.DowloadDirLabel.Text = "Directori de descàrrega per defecte";
            this.DowloadDirLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btTempFolder
            // 
            this.btTempFolder.BackgroundImage = global::VideoStreamingDownloader.Properties.Resources._5994710;
            this.btTempFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btTempFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btTempFolder.Location = new System.Drawing.Point(678, 43);
            this.btTempFolder.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.btTempFolder.Name = "btTempFolder";
            this.btTempFolder.Size = new System.Drawing.Size(36, 36);
            this.btTempFolder.TabIndex = 5;
            this.btTempFolder.UseVisualStyleBackColor = true;
            this.btTempFolder.Click += new System.EventHandler(this.btTempFolder_Click);
            // 
            // SimultaneousDownloadsLabel
            // 
            this.SimultaneousDownloadsLabel.AutoSize = true;
            this.SimultaneousDownloadsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SimultaneousDownloadsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SimultaneousDownloadsLabel.Location = new System.Drawing.Point(0, 170);
            this.SimultaneousDownloadsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.SimultaneousDownloadsLabel.Name = "SimultaneousDownloadsLabel";
            this.SimultaneousDownloadsLabel.Size = new System.Drawing.Size(675, 40);
            this.SimultaneousDownloadsLabel.TabIndex = 7;
            this.SimultaneousDownloadsLabel.Text = "Descàrregues simultànies";
            this.SimultaneousDownloadsLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // TempDirLabel
            // 
            this.TempDirLabel.AutoSize = true;
            this.TempDirLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TempDirLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TempDirLabel.Location = new System.Drawing.Point(0, 0);
            this.TempDirLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TempDirLabel.Name = "TempDirLabel";
            this.TempDirLabel.Size = new System.Drawing.Size(675, 40);
            this.TempDirLabel.TabIndex = 5;
            this.TempDirLabel.Text = "Directori temporal";
            this.TempDirLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // LanguageLabel
            // 
            this.LanguageLabel.AutoSize = true;
            this.LanguageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LanguageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LanguageLabel.Location = new System.Drawing.Point(0, 255);
            this.LanguageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.LanguageLabel.Name = "LanguageLabel";
            this.LanguageLabel.Size = new System.Drawing.Size(675, 40);
            this.LanguageLabel.TabIndex = 8;
            this.LanguageLabel.Text = "Language";
            this.LanguageLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // LanguageComboBox
            // 
            this.LanguageComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LanguageComboBox.FormattingEnabled = true;
            this.LanguageComboBox.Location = new System.Drawing.Point(3, 298);
            this.LanguageComboBox.Name = "LanguageComboBox";
            this.LanguageComboBox.Size = new System.Drawing.Size(669, 37);
            this.LanguageComboBox.TabIndex = 9;
            // 
            // UserSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(741, 529);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.SaveButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "UserSettingsForm";
            this.Text = "Preferències";
            ((System.ComponentModel.ISupportInitialize)(this.nudSimultaneousDownloads)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox tbTempPath;
        private System.Windows.Forms.NumericUpDown nudSimultaneousDownloads;
        private System.Windows.Forms.TextBox tbDefaultPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label TempDirLabel;
        private System.Windows.Forms.Label DowloadDirLabel;
        private System.Windows.Forms.Label SimultaneousDownloadsLabel;
        private System.Windows.Forms.Button btDefaultFolder;
        private System.Windows.Forms.Button btTempFolder;
        private System.Windows.Forms.Label LanguageLabel;
        private System.Windows.Forms.ComboBox LanguageComboBox;
    }
}