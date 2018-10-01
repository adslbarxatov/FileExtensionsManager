namespace FileExtensionsManager
	{
	partial class MainForm
		{
		/// <summary>
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose (bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose ();
				}
			base.Dispose (disposing);
			}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Обязательный метод для поддержки конструктора - не изменяйте
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent ()
			{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.MainTable = new System.Windows.Forms.DataGridView();
			this.AddRecord = new System.Windows.Forms.Button();
			this.DeleteRecord = new System.Windows.Forms.Button();
			this.LoadRegFile = new System.Windows.Forms.Button();
			this.Apply = new System.Windows.Forms.Button();
			this.ApplyAll = new System.Windows.Forms.Button();
			this.Exit = new System.Windows.Forms.Button();
			this.Applied = new System.Windows.Forms.Label();
			this.NotApplied = new System.Windows.Forms.Label();
			this.NoAccess = new System.Windows.Forms.Label();
			this.PartiallyApplied = new System.Windows.Forms.Label();
			this.OFDialog = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.BasesCombo = new System.Windows.Forms.ComboBox();
			this.AddBase = new System.Windows.Forms.Button();
			this.GetHelp = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.MainTable)).BeginInit();
			this.SuspendLayout();
			// 
			// MainTable
			// 
			this.MainTable.AllowUserToAddRows = false;
			this.MainTable.AllowUserToDeleteRows = false;
			this.MainTable.AllowUserToResizeColumns = false;
			this.MainTable.AllowUserToResizeRows = false;
			this.MainTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.MainTable.BackgroundColor = System.Drawing.SystemColors.Control;
			this.MainTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MainTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.MainTable.ColumnHeadersVisible = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Yellow;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.MainTable.DefaultCellStyle = dataGridViewCellStyle1;
			this.MainTable.GridColor = System.Drawing.SystemColors.ControlText;
			this.MainTable.Location = new System.Drawing.Point(12, 43);
			this.MainTable.MultiSelect = false;
			this.MainTable.Name = "MainTable";
			this.MainTable.ReadOnly = true;
			this.MainTable.RowHeadersVisible = false;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.MainTable.RowsDefaultCellStyle = dataGridViewCellStyle2;
			this.MainTable.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.MainTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.MainTable.Size = new System.Drawing.Size(468, 214);
			this.MainTable.TabIndex = 0;
			this.MainTable.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MainTable_CellDoubleClick);
			// 
			// AddRecord
			// 
			this.AddRecord.Location = new System.Drawing.Point(12, 305);
			this.AddRecord.Name = "AddRecord";
			this.AddRecord.Size = new System.Drawing.Size(130, 25);
			this.AddRecord.TabIndex = 5;
			this.AddRecord.Text = "&Добавить";
			this.AddRecord.UseVisualStyleBackColor = true;
			this.AddRecord.Click += new System.EventHandler(this.AddRecord_Click);
			// 
			// DeleteRecord
			// 
			this.DeleteRecord.Location = new System.Drawing.Point(148, 305);
			this.DeleteRecord.Name = "DeleteRecord";
			this.DeleteRecord.Size = new System.Drawing.Size(130, 25);
			this.DeleteRecord.TabIndex = 6;
			this.DeleteRecord.Text = "&Удалить";
			this.DeleteRecord.UseVisualStyleBackColor = true;
			this.DeleteRecord.Click += new System.EventHandler(this.DeleteRecord_Click);
			// 
			// LoadRegFile
			// 
			this.LoadRegFile.Location = new System.Drawing.Point(284, 305);
			this.LoadRegFile.Name = "LoadRegFile";
			this.LoadRegFile.Size = new System.Drawing.Size(196, 25);
			this.LoadRegFile.TabIndex = 7;
			this.LoadRegFile.Text = "&Загрузить из файла реестра";
			this.LoadRegFile.UseVisualStyleBackColor = true;
			this.LoadRegFile.Click += new System.EventHandler(this.LoadRegFile_Click);
			// 
			// Apply
			// 
			this.Apply.Location = new System.Drawing.Point(12, 336);
			this.Apply.Name = "Apply";
			this.Apply.Size = new System.Drawing.Size(130, 25);
			this.Apply.TabIndex = 2;
			this.Apply.Text = "&Применить";
			this.Apply.UseVisualStyleBackColor = true;
			this.Apply.Click += new System.EventHandler(this.Apply_Click);
			// 
			// ApplyAll
			// 
			this.ApplyAll.Location = new System.Drawing.Point(148, 336);
			this.ApplyAll.Name = "ApplyAll";
			this.ApplyAll.Size = new System.Drawing.Size(196, 25);
			this.ApplyAll.TabIndex = 3;
			this.ApplyAll.Text = "Применить &все";
			this.ApplyAll.UseVisualStyleBackColor = true;
			this.ApplyAll.Click += new System.EventHandler(this.ApplyAll_Click);
			// 
			// Exit
			// 
			this.Exit.Location = new System.Drawing.Point(350, 336);
			this.Exit.Name = "Exit";
			this.Exit.Size = new System.Drawing.Size(130, 25);
			this.Exit.TabIndex = 4;
			this.Exit.Text = "В&ыход";
			this.Exit.UseVisualStyleBackColor = true;
			this.Exit.Click += new System.EventHandler(this.Exit_Click);
			// 
			// Applied
			// 
			this.Applied.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.Applied.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.Applied.ForeColor = System.Drawing.Color.Green;
			this.Applied.Location = new System.Drawing.Point(12, 262);
			this.Applied.Name = "Applied";
			this.Applied.Size = new System.Drawing.Size(200, 15);
			this.Applied.TabIndex = 7;
			this.Applied.Text = "Применены";
			this.Applied.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// NotApplied
			// 
			this.NotApplied.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
			this.NotApplied.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.NotApplied.ForeColor = System.Drawing.Color.Maroon;
			this.NotApplied.Location = new System.Drawing.Point(12, 282);
			this.NotApplied.Name = "NotApplied";
			this.NotApplied.Size = new System.Drawing.Size(200, 15);
			this.NotApplied.TabIndex = 8;
			this.NotApplied.Text = "Не применены";
			this.NotApplied.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// NoAccess
			// 
			this.NoAccess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.NoAccess.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.NoAccess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.NoAccess.Location = new System.Drawing.Point(281, 282);
			this.NoAccess.Name = "NoAccess";
			this.NoAccess.Size = new System.Drawing.Size(200, 15);
			this.NoAccess.TabIndex = 10;
			this.NoAccess.Text = "Недоступны";
			this.NoAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// PartiallyApplied
			// 
			this.PartiallyApplied.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.PartiallyApplied.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.PartiallyApplied.ForeColor = System.Drawing.Color.Navy;
			this.PartiallyApplied.Location = new System.Drawing.Point(281, 262);
			this.PartiallyApplied.Name = "PartiallyApplied";
			this.PartiallyApplied.Size = new System.Drawing.Size(200, 15);
			this.PartiallyApplied.TabIndex = 9;
			this.PartiallyApplied.Text = "Применены частично";
			this.PartiallyApplied.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// OFDialog
			// 
			this.OFDialog.RestoreDirectory = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 15);
			this.label1.TabIndex = 11;
			this.label1.Text = "Текущая база:";
			// 
			// BasesCombo
			// 
			this.BasesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.BasesCombo.FormattingEnabled = true;
			this.BasesCombo.Location = new System.Drawing.Point(106, 12);
			this.BasesCombo.Name = "BasesCombo";
			this.BasesCombo.Size = new System.Drawing.Size(197, 23);
			this.BasesCombo.TabIndex = 1;
			this.BasesCombo.SelectedIndexChanged += new System.EventHandler(this.BasesCombo_SelectedIndexChanged);
			// 
			// AddBase
			// 
			this.AddBase.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.AddBase.Location = new System.Drawing.Point(309, 11);
			this.AddBase.Name = "AddBase";
			this.AddBase.Size = new System.Drawing.Size(23, 23);
			this.AddBase.TabIndex = 8;
			this.AddBase.Text = "+";
			this.AddBase.UseVisualStyleBackColor = true;
			this.AddBase.Click += new System.EventHandler(this.AddBase_Click);
			// 
			// GetHelp
			// 
			this.GetHelp.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.GetHelp.Location = new System.Drawing.Point(338, 11);
			this.GetHelp.Name = "GetHelp";
			this.GetHelp.Size = new System.Drawing.Size(23, 23);
			this.GetHelp.TabIndex = 12;
			this.GetHelp.Text = "?";
			this.GetHelp.UseVisualStyleBackColor = true;
			this.GetHelp.Click += new System.EventHandler(this.GetHelp_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(492, 373);
			this.Controls.Add(this.GetHelp);
			this.Controls.Add(this.AddBase);
			this.Controls.Add(this.BasesCombo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.NoAccess);
			this.Controls.Add(this.PartiallyApplied);
			this.Controls.Add(this.NotApplied);
			this.Controls.Add(this.Applied);
			this.Controls.Add(this.Exit);
			this.Controls.Add(this.ApplyAll);
			this.Controls.Add(this.Apply);
			this.Controls.Add(this.LoadRegFile);
			this.Controls.Add(this.DeleteRecord);
			this.Controls.Add(this.AddRecord);
			this.Controls.Add(this.MainTable);
			this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(500, 400);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.MainTable)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.DataGridView MainTable;
		private System.Windows.Forms.Button AddRecord;
		private System.Windows.Forms.Button DeleteRecord;
		private System.Windows.Forms.Button LoadRegFile;
		private System.Windows.Forms.Button Apply;
		private System.Windows.Forms.Button ApplyAll;
		private System.Windows.Forms.Button Exit;
		private System.Windows.Forms.Label Applied;
		private System.Windows.Forms.Label NotApplied;
		private System.Windows.Forms.Label NoAccess;
		private System.Windows.Forms.Label PartiallyApplied;
		private System.Windows.Forms.OpenFileDialog OFDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox BasesCombo;
		private System.Windows.Forms.Button AddBase;
		private System.Windows.Forms.Button GetHelp;

		}
	}

