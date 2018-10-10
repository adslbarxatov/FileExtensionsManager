namespace FileExtensionsManager
	{
	partial class IconsExtractor
		{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose ();
				}
			base.Dispose (disposing);
			}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
			{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IconsExtractor));
			this.MainPicture = new System.Windows.Forms.PictureBox();
			this.OFDialog = new System.Windows.Forms.OpenFileDialog();
			this.PageNumber = new System.Windows.Forms.NumericUpDown();
			this.TotalLabel = new System.Windows.Forms.Label();
			this.PageLabel = new System.Windows.Forms.Label();
			this.SelectButton = new System.Windows.Forms.Button();
			this.AbortButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.MainPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PageNumber)).BeginInit();
			this.SuspendLayout();
			// 
			// MainPicture
			// 
			this.MainPicture.BackColor = System.Drawing.Color.White;
			this.MainPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.MainPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MainPicture.Location = new System.Drawing.Point(12, 12);
			this.MainPicture.Name = "MainPicture";
			this.MainPicture.Size = new System.Drawing.Size(354, 484);
			this.MainPicture.TabIndex = 0;
			this.MainPicture.TabStop = false;
			this.MainPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainPicture_MouseClick);
			// 
			// OFDialog
			// 
			this.OFDialog.RestoreDirectory = true;
			// 
			// PageNumber
			// 
			this.PageNumber.Location = new System.Drawing.Point(84, 502);
			this.PageNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.PageNumber.Name = "PageNumber";
			this.PageNumber.Size = new System.Drawing.Size(60, 21);
			this.PageNumber.TabIndex = 0;
			this.PageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.PageNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.PageNumber.ValueChanged += new System.EventHandler(this.PageNumber_ValueChanged);
			// 
			// TotalLabel
			// 
			this.TotalLabel.Font = new System.Drawing.Font("Arial", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.TotalLabel.Location = new System.Drawing.Point(251, 502);
			this.TotalLabel.Name = "TotalLabel";
			this.TotalLabel.Size = new System.Drawing.Size(115, 21);
			this.TotalLabel.TabIndex = 2;
			this.TotalLabel.Text = "label1";
			this.TotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// PageLabel
			// 
			this.PageLabel.AutoSize = true;
			this.PageLabel.Location = new System.Drawing.Point(12, 505);
			this.PageLabel.Name = "PageLabel";
			this.PageLabel.Size = new System.Drawing.Size(66, 15);
			this.PageLabel.TabIndex = 3;
			this.PageLabel.Text = "Страница:";
			// 
			// SelectButton
			// 
			this.SelectButton.Enabled = false;
			this.SelectButton.Location = new System.Drawing.Point(372, 392);
			this.SelectButton.Name = "SelectButton";
			this.SelectButton.Size = new System.Drawing.Size(130, 25);
			this.SelectButton.TabIndex = 1;
			this.SelectButton.Text = "&Выбрать";
			this.SelectButton.UseVisualStyleBackColor = true;
			this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
			// 
			// AbortButton
			// 
			this.AbortButton.Location = new System.Drawing.Point(372, 423);
			this.AbortButton.Name = "AbortButton";
			this.AbortButton.Size = new System.Drawing.Size(130, 25);
			this.AbortButton.TabIndex = 2;
			this.AbortButton.Text = "От&мена";
			this.AbortButton.UseVisualStyleBackColor = true;
			this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
			// 
			// IconsExtractor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(499, 536);
			this.ControlBox = false;
			this.Controls.Add(this.AbortButton);
			this.Controls.Add(this.SelectButton);
			this.Controls.Add(this.PageLabel);
			this.Controls.Add(this.TotalLabel);
			this.Controls.Add(this.PageNumber);
			this.Controls.Add(this.MainPicture);
			this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "IconsExtractor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IconsExtractor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IconsExtractor_FormClosing);
			this.Load += new System.EventHandler(this.IconsExtractor_Load);
			((System.ComponentModel.ISupportInitialize)(this.MainPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PageNumber)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.PictureBox MainPicture;
		private System.Windows.Forms.OpenFileDialog OFDialog;
		private System.Windows.Forms.NumericUpDown PageNumber;
		private System.Windows.Forms.Label TotalLabel;
		private System.Windows.Forms.Label PageLabel;
		private System.Windows.Forms.Button SelectButton;
		private System.Windows.Forms.Button AbortButton;
		}
	}