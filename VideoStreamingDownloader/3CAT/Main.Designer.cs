namespace VideoStreamingDownloader._3CAT
{
    partial class Main
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SubsLabel = new System.Windows.Forms.Label();
            this.AudioLabel = new System.Windows.Forms.Label();
            this.VideoLabel = new System.Windows.Forms.Label();
            this.flpSub = new System.Windows.Forms.FlowLayoutPanel();
            this.flpVideo = new System.Windows.Forms.FlowLayoutPanel();
            this.flpAudio = new System.Windows.Forms.FlowLayoutPanel();
            this.Title = new System.Windows.Forms.Label();
            this.AddToDownloadButton = new System.Windows.Forms.Button();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.Select = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(18, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1233, 359);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.SubsLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.AudioLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.VideoLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flpSub, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.flpVideo, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flpAudio, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1227, 338);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // SubsLabel
            // 
            this.SubsLabel.AutoSize = true;
            this.SubsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SubsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SubsLabel.Location = new System.Drawing.Point(3, 200);
            this.SubsLabel.Name = "SubsLabel";
            this.SubsLabel.Size = new System.Drawing.Size(1221, 30);
            this.SubsLabel.TabIndex = 11;
            this.SubsLabel.Text = "Subtítols:";
            // 
            // AudioLabel
            // 
            this.AudioLabel.AutoSize = true;
            this.AudioLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AudioLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AudioLabel.Location = new System.Drawing.Point(3, 100);
            this.AudioLabel.Name = "AudioLabel";
            this.AudioLabel.Size = new System.Drawing.Size(1221, 30);
            this.AudioLabel.TabIndex = 10;
            this.AudioLabel.Text = "Àudio:";
            // 
            // VideoLabel
            // 
            this.VideoLabel.AutoSize = true;
            this.VideoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VideoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VideoLabel.Location = new System.Drawing.Point(3, 0);
            this.VideoLabel.Name = "VideoLabel";
            this.VideoLabel.Size = new System.Drawing.Size(1221, 30);
            this.VideoLabel.TabIndex = 9;
            this.VideoLabel.Text = "Vídeo:";
            // 
            // flpSub
            // 
            this.flpSub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSub.Location = new System.Drawing.Point(3, 233);
            this.flpSub.Name = "flpSub";
            this.flpSub.Size = new System.Drawing.Size(1221, 102);
            this.flpSub.TabIndex = 1;
            // 
            // flpVideo
            // 
            this.flpVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpVideo.Location = new System.Drawing.Point(3, 33);
            this.flpVideo.Name = "flpVideo";
            this.flpVideo.Size = new System.Drawing.Size(1221, 64);
            this.flpVideo.TabIndex = 0;
            // 
            // flpAudio
            // 
            this.flpAudio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpAudio.Location = new System.Drawing.Point(3, 133);
            this.flpAudio.Name = "flpAudio";
            this.flpAudio.Size = new System.Drawing.Size(1221, 64);
            this.flpAudio.TabIndex = 1;
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(12, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(86, 32);
            this.Title.TabIndex = 9;
            this.Title.Text = "3CAT";
            // 
            // AddToDownloadButton
            // 
            this.AddToDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddToDownloadButton.Enabled = false;
            this.AddToDownloadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddToDownloadButton.Location = new System.Drawing.Point(851, 471);
            this.AddToDownloadButton.Name = "AddToDownloadButton";
            this.AddToDownloadButton.Size = new System.Drawing.Size(400, 45);
            this.AddToDownloadButton.TabIndex = 10;
            this.AddToDownloadButton.Text = "AddToDownload";
            this.AddToDownloadButton.UseVisualStyleBackColor = true;
            this.AddToDownloadButton.Click += new System.EventHandler(this.Download_Click);
            // 
            // LoadImage
            // 
            this.LoadImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LoadImage.Image = global::VideoStreamingDownloader.Properties.Resources.load;
            this.LoadImage.InitialImage = null;
            this.LoadImage.Location = new System.Drawing.Point(612, 165);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(99, 100);
            this.LoadImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LoadImage.TabIndex = 12;
            this.LoadImage.TabStop = false;
            this.LoadImage.Visible = false;
            // 
            // Select
            // 
            this.Select.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Select.Enabled = false;
            this.Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Select.Location = new System.Drawing.Point(851, 420);
            this.Select.Name = "Select";
            this.Select.Size = new System.Drawing.Size(400, 45);
            this.Select.TabIndex = 13;
            this.Select.Text = "Select";
            this.Select.UseVisualStyleBackColor = true;
            this.Select.Click += new System.EventHandler(this.Select_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1263, 528);
            this.Controls.Add(this.Select);
            this.Controls.Add(this.LoadImage);
            this.Controls.Add(this.AddToDownloadButton);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label SubsLabel;
        private System.Windows.Forms.Label AudioLabel;
        private System.Windows.Forms.Label VideoLabel;
        private System.Windows.Forms.FlowLayoutPanel flpSub;
        private System.Windows.Forms.FlowLayoutPanel flpVideo;
        private System.Windows.Forms.FlowLayoutPanel flpAudio;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Button AddToDownloadButton;
        private System.Windows.Forms.PictureBox LoadImage;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button Select;
    }
}