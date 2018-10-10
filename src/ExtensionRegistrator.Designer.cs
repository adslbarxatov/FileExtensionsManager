namespace FileExtensionsManager
	{
	partial class ExtensionRegistrator
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtensionRegistrator));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.FileExtension = new System.Windows.Forms.TextBox();
			this.FileTypeName = new System.Windows.Forms.TextBox();
			this.FileIcon = new System.Windows.Forms.TextBox();
			this.Apply = new System.Windows.Forms.Button();
			this.Abort = new System.Windows.Forms.Button();
			this.FileApplication = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SelectIcon = new System.Windows.Forms.Button();
			this.SelectApplication = new System.Windows.Forms.Button();
			this.OFDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(187, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Расширение файла (без точки):";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(135, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Название типа файла:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Значок файла:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(166, 15);
			this.label4.TabIndex = 3;
			this.label4.Text = "Открывающее приложение:";
			// 
			// FileExtension
			// 
			this.FileExtension.Location = new System.Drawing.Point(205, 15);
			this.FileExtension.MaxLength = 10;
			this.FileExtension.Name = "FileExtension";
			this.FileExtension.Size = new System.Drawing.Size(108, 21);
			this.FileExtension.TabIndex = 4;
			// 
			// FileTypeName
			// 
			this.FileTypeName.Location = new System.Drawing.Point(184, 42);
			this.FileTypeName.MaxLength = 255;
			this.FileTypeName.Name = "FileTypeName";
			this.FileTypeName.Size = new System.Drawing.Size(329, 21);
			this.FileTypeName.TabIndex = 5;
			// 
			// FileIcon
			// 
			this.FileIcon.Location = new System.Drawing.Point(184, 69);
			this.FileIcon.Name = "FileIcon";
			this.FileIcon.ReadOnly = true;
			this.FileIcon.Size = new System.Drawing.Size(329, 21);
			this.FileIcon.TabIndex = 6;
			// 
			// Apply
			// 
			this.Apply.Location = new System.Drawing.Point(179, 186);
			this.Apply.Name = "Apply";
			this.Apply.Size = new System.Drawing.Size(100, 25);
			this.Apply.TabIndex = 10;
			this.Apply.Text = "&Применить";
			this.Apply.UseVisualStyleBackColor = true;
			this.Apply.Click += new System.EventHandler(this.Apply_Click);
			// 
			// Abort
			// 
			this.Abort.Location = new System.Drawing.Point(285, 186);
			this.Abort.Name = "Abort";
			this.Abort.Size = new System.Drawing.Size(100, 25);
			this.Abort.TabIndex = 11;
			this.Abort.Text = "&Отмена";
			this.Abort.UseVisualStyleBackColor = true;
			this.Abort.Click += new System.EventHandler(this.Abort_Click);
			// 
			// FileApplication
			// 
			this.FileApplication.Location = new System.Drawing.Point(184, 96);
			this.FileApplication.MaxLength = 255;
			this.FileApplication.Name = "FileApplication";
			this.FileApplication.ReadOnly = true;
			this.FileApplication.Size = new System.Drawing.Size(329, 21);
			this.FileApplication.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.label5.Location = new System.Drawing.Point(12, 129);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(540, 48);
			this.label5.TabIndex = 13;
			this.label5.Text = "Внимание! Некоторые приложения могут не поддерживать команду «Открыть с помощью» " +
    "или поддерживать её нестандартным образом. В этих случаях данная функция может р" +
    "аботать некорректно";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// SelectIcon
			// 
			this.SelectIcon.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.SelectIcon.Location = new System.Drawing.Point(519, 69);
			this.SelectIcon.Name = "SelectIcon";
			this.SelectIcon.Size = new System.Drawing.Size(33, 21);
			this.SelectIcon.TabIndex = 14;
			this.SelectIcon.Text = "...";
			this.SelectIcon.UseVisualStyleBackColor = true;
			this.SelectIcon.Click += new System.EventHandler(this.SelectIcon_Click);
			// 
			// SelectApplication
			// 
			this.SelectApplication.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.SelectApplication.Location = new System.Drawing.Point(519, 96);
			this.SelectApplication.Name = "SelectApplication";
			this.SelectApplication.Size = new System.Drawing.Size(33, 21);
			this.SelectApplication.TabIndex = 15;
			this.SelectApplication.Text = "...";
			this.SelectApplication.UseVisualStyleBackColor = true;
			this.SelectApplication.Click += new System.EventHandler(this.SelectApplication_Click);
			// 
			// OFDialog
			// 
			this.OFDialog.RestoreDirectory = true;
			// 
			// ExtensionRegistrator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(564, 223);
			this.Controls.Add(this.SelectApplication);
			this.Controls.Add(this.SelectIcon);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.FileApplication);
			this.Controls.Add(this.Abort);
			this.Controls.Add(this.Apply);
			this.Controls.Add(this.FileIcon);
			this.Controls.Add(this.FileTypeName);
			this.Controls.Add(this.FileExtension);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExtensionRegistrator";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RegistryEntryEditor";
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox FileExtension;
		private System.Windows.Forms.TextBox FileTypeName;
		private System.Windows.Forms.TextBox FileIcon;
		private System.Windows.Forms.Button Apply;
		private System.Windows.Forms.Button Abort;
		private System.Windows.Forms.TextBox FileApplication;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button SelectIcon;
		private System.Windows.Forms.Button SelectApplication;
		private System.Windows.Forms.OpenFileDialog OFDialog;
		}
	}