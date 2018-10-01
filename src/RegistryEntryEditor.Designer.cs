namespace FileExtensionsManager
	{
	partial class RegistryEntryEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistryEntryEditor));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.KeyPath = new System.Windows.Forms.TextBox();
			this.KeyName = new System.Windows.Forms.TextBox();
			this.KeyObject = new System.Windows.Forms.TextBox();
			this.KeyType = new System.Windows.Forms.ComboBox();
			this.PathMustBeDeleted = new System.Windows.Forms.CheckBox();
			this.NameMustBeDeleted = new System.Windows.Forms.CheckBox();
			this.Apply = new System.Windows.Forms.Button();
			this.Abort = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(143, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Путь к разделу реестра:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Имя параметра:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(131, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Значение параметра:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 15);
			this.label4.TabIndex = 3;
			this.label4.Text = "Тип параметра:";
			// 
			// KeyPath
			// 
			this.KeyPath.Location = new System.Drawing.Point(161, 15);
			this.KeyPath.MaxLength = 255;
			this.KeyPath.Name = "KeyPath";
			this.KeyPath.Size = new System.Drawing.Size(329, 21);
			this.KeyPath.TabIndex = 4;
			// 
			// KeyName
			// 
			this.KeyName.Location = new System.Drawing.Point(161, 42);
			this.KeyName.MaxLength = 255;
			this.KeyName.Name = "KeyName";
			this.KeyName.Size = new System.Drawing.Size(329, 21);
			this.KeyName.TabIndex = 5;
			// 
			// KeyObject
			// 
			this.KeyObject.Location = new System.Drawing.Point(161, 69);
			this.KeyObject.MaxLength = 255;
			this.KeyObject.Name = "KeyObject";
			this.KeyObject.Size = new System.Drawing.Size(329, 21);
			this.KeyObject.TabIndex = 6;
			// 
			// KeyType
			// 
			this.KeyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.KeyType.FormattingEnabled = true;
			this.KeyType.Location = new System.Drawing.Point(161, 96);
			this.KeyType.Name = "KeyType";
			this.KeyType.Size = new System.Drawing.Size(329, 23);
			this.KeyType.TabIndex = 7;
			// 
			// PathMustBeDeleted
			// 
			this.PathMustBeDeleted.AutoSize = true;
			this.PathMustBeDeleted.Location = new System.Drawing.Point(12, 134);
			this.PathMustBeDeleted.Name = "PathMustBeDeleted";
			this.PathMustBeDeleted.Size = new System.Drawing.Size(137, 19);
			this.PathMustBeDeleted.TabIndex = 8;
			this.PathMustBeDeleted.Text = "Удаляемый раздел";
			this.PathMustBeDeleted.UseVisualStyleBackColor = true;
			this.PathMustBeDeleted.CheckedChanged += new System.EventHandler(this.PathMustBeDeleted_CheckedChanged);
			// 
			// NameMustBeDeleted
			// 
			this.NameMustBeDeleted.AutoSize = true;
			this.NameMustBeDeleted.Location = new System.Drawing.Point(161, 134);
			this.NameMustBeDeleted.Name = "NameMustBeDeleted";
			this.NameMustBeDeleted.Size = new System.Drawing.Size(152, 19);
			this.NameMustBeDeleted.TabIndex = 9;
			this.NameMustBeDeleted.Text = "Удаляемый параметр";
			this.NameMustBeDeleted.UseVisualStyleBackColor = true;
			// 
			// Apply
			// 
			this.Apply.Location = new System.Drawing.Point(148, 175);
			this.Apply.Name = "Apply";
			this.Apply.Size = new System.Drawing.Size(100, 25);
			this.Apply.TabIndex = 10;
			this.Apply.Text = "&Применить";
			this.Apply.UseVisualStyleBackColor = true;
			this.Apply.Click += new System.EventHandler(this.Apply_Click);
			// 
			// Abort
			// 
			this.Abort.Location = new System.Drawing.Point(254, 175);
			this.Abort.Name = "Abort";
			this.Abort.Size = new System.Drawing.Size(100, 25);
			this.Abort.TabIndex = 11;
			this.Abort.Text = "&Отмена";
			this.Abort.UseVisualStyleBackColor = true;
			this.Abort.Click += new System.EventHandler(this.Abort_Click);
			// 
			// RegistryEntryEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 212);
			this.Controls.Add(this.Abort);
			this.Controls.Add(this.Apply);
			this.Controls.Add(this.NameMustBeDeleted);
			this.Controls.Add(this.PathMustBeDeleted);
			this.Controls.Add(this.KeyType);
			this.Controls.Add(this.KeyObject);
			this.Controls.Add(this.KeyName);
			this.Controls.Add(this.KeyPath);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RegistryEntryEditor";
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
		private System.Windows.Forms.TextBox KeyPath;
		private System.Windows.Forms.TextBox KeyName;
		private System.Windows.Forms.TextBox KeyObject;
		private System.Windows.Forms.ComboBox KeyType;
		private System.Windows.Forms.CheckBox PathMustBeDeleted;
		private System.Windows.Forms.CheckBox NameMustBeDeleted;
		private System.Windows.Forms.Button Apply;
		private System.Windows.Forms.Button Abort;
		}
	}