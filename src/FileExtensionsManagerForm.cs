using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Главная форма программы
	/// </summary>
	public partial class FileExtensionsManagerForm: Form
		{
		// Переменные
		private List<RegistryEntriesBaseManager> rebm = new List<RegistryEntriesBaseManager> ();
		private SupportedLanguages al = Localization.CurrentLanguage;
		private uint applied = 0, partiallyApplied = 0, notApplied = 0, noAccess = 0;

		/// <summary>
		/// Конструктор. Инициализирует главную форму приложения
		/// </summary>
		public FileExtensionsManagerForm ()
			{
			// Инициализация основных элементов
			InitializeComponent ();
			}

		private void MainForm_Load (object sender, EventArgs e)
			{
			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;

			LanguageCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)al;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}

			MainTable.Columns.Add ("Entries", "Entries");

			// Инициализация баз реестровых записей
			if (Directory.Exists (RDGenerics.AppStartupPath + RegistryEntriesBaseManager.BasesSubdirectory))
				{
				string[] files = Directory.GetFiles (RDGenerics.AppStartupPath + RegistryEntriesBaseManager.BasesSubdirectory,
					"*" + RegistryEntriesBaseManager.BaseFileExtension);
				for (int i = 0; i < files.Length; i++)
					{
					RegistryEntriesBaseManager re = new RegistryEntriesBaseManager (Path.GetFileNameWithoutExtension (files[i]));
					if (re.IsInited)
						rebm.Add (re);
					}
				}

			if (rebm.Count == 0)
				{
				MessageBox.Show (Localization.GetText ("BasesNotFound", al) + "\n\n" + Localization.GetText ("NewBaseAdded", al),
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

				RegistryEntriesBaseManager re = new RegistryEntriesBaseManager ();

				this.Close ();
				return;
				}

			// Загрузка списка
			for (int i = 0; i < rebm.Count; i++)
				BasesCombo.Items.Add (rebm[i].BaseName);
			BasesCombo.SelectedIndex = 0;

			// Обновление таблицы
			UpdateTable ();
			}

		// Изменение размера окна
		private void MainForm_Resize (object sender, EventArgs e)
			{
			MainTable.Width = this.Width - 32;
			MainTable.Height = this.Height - 186;

			Applied.Top = PartiallyApplied.Top = this.Height - 138;
			NotApplied.Top = NoAccess.Top = this.Height - 118;

			AddRecord.Top = DeleteRecord.Top = LoadRegFile.Top = RegExtension.Top = this.Height - 95;
			Apply.Top = ApplyAll.Top = Exit.Top = FindIcon.Top = this.Height - 64;
			}

		// Обновление таблицы
		private void UpdateTable ()
			{
			// Запрос
			List<string> presentation = rebm[BasesCombo.SelectedIndex].EntriesBasePresentation;
			List<RegistryEntryApplicationResults> statuses = rebm[BasesCombo.SelectedIndex].EntriesStatusesPresentation;

			// Обновление
			applied = 0;
			partiallyApplied = 0;
			notApplied = 0;
			noAccess = 0;
			MainTable.Rows.Clear ();

			for (int i = 0; i < presentation.Count; i++)
				{
				MainTable.Rows.Add ();
				MainTable.Rows[i].Cells[0].Value = presentation[i];
				switch (statuses[i])
					{
					case RegistryEntryApplicationResults.CannotGetAccess:
						MainTable.Rows[i].DefaultCellStyle.BackColor = NoAccess.ForeColor;
						MainTable.Rows[i].DefaultCellStyle.SelectionBackColor = NoAccess.BackColor;
						noAccess++;
						break;

					case RegistryEntryApplicationResults.FullyApplied:
						MainTable.Rows[i].DefaultCellStyle.BackColor = Applied.ForeColor;
						MainTable.Rows[i].DefaultCellStyle.SelectionBackColor = Applied.BackColor;
						applied++;
						break;

					case RegistryEntryApplicationResults.PartiallyApplied:
						MainTable.Rows[i].DefaultCellStyle.BackColor = PartiallyApplied.ForeColor;
						MainTable.Rows[i].DefaultCellStyle.SelectionBackColor = PartiallyApplied.BackColor;
						partiallyApplied++;
						break;

					case RegistryEntryApplicationResults.NotApplied:
						MainTable.Rows[i].DefaultCellStyle.BackColor = NotApplied.ForeColor;
						MainTable.Rows[i].DefaultCellStyle.SelectionBackColor = NotApplied.BackColor;
						notApplied++;
						break;
					}
				}

			// Результаты
			UpdateResults ();
			}

		private void UpdateResults ()
			{
			Applied.Text = Localization.GetText ("AppliedText", al) + applied.ToString ();
			PartiallyApplied.Text = Localization.GetText ("PartiallyAppliedText", al) + partiallyApplied.ToString ();
			NotApplied.Text = Localization.GetText ("NotAppliedText", al) + notApplied.ToString ();
			NoAccess.Text = Localization.GetText ("NoAccessText", al) + noAccess.ToString ();
			}

		// Выход из программы
		private void Exit_Click (object sender, EventArgs e)
			{
			// Сохранение баз
			for (int i = 0; i < rebm.Count; i++)
				rebm[i].SaveBase ();

			// Выход
			this.Close ();
			}

		// Удаление записи
		private void DeleteRecord_Click (object sender, EventArgs e)
			{
			// Контроль
			if (MainTable.SelectedRows.Count <= 0)
				return;

			if (MessageBox.Show (Localization.GetText ("RemoveEntry", al), ProgramDescription.AssemblyTitle,
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			// Удаление
			List<int> idx = new List<int> ();
			foreach (DataGridViewRow r in MainTable.SelectedRows)
				idx.Add (r.Index);
			idx.Sort ();

			for (int i = 0; i < idx.Count; i++)
				rebm[BasesCombo.SelectedIndex].DeleteEntry ((uint)(idx[i] - i));

			// Обновление таблицы
			UpdateTable ();
			if (rebm[BasesCombo.SelectedIndex].EntriesCount > 0)
				{
				if (rebm[BasesCombo.SelectedIndex].EntriesCount <= idx[idx.Count - 1])
					idx[idx.Count - 1] = (int)rebm[BasesCombo.SelectedIndex].EntriesCount - 1;

				MainTable.CurrentCell = MainTable.Rows[idx[idx.Count - 1]].Cells[0];
				}
			}

		// Применение выбранной записи
		private void Apply_Click (object sender, EventArgs e)
			{
			// Контроль
			if (MainTable.SelectedRows.Count <= 0)
				return;

			if (MessageBox.Show (Localization.GetText ("ApplyEntry", al), ProgramDescription.AssemblyTitle,
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
				return;

			// Применение записей
			for (int i = 0; i < MainTable.SelectedRows.Count; i++)
				{
				string msg = "";
				switch (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)MainTable.SelectedRows[i].Index).ApplyEntry ())
					{
					case RegistryEntryApplicationResults.CannotGetAccess:
						msg = Localization.GetText ("EntryIsUnavailable", al);
						break;

					case RegistryEntryApplicationResults.PartiallyApplied:
					case RegistryEntryApplicationResults.NotApplied:
						msg = Localization.GetText ("EntryNotApplied", al);
						break;
					}

				if (msg != "")
					MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

			// Обновление таблицы
			int row = MainTable.SelectedRows[MainTable.SelectedRows.Count - 1].Index;
			UpdateTable ();
			MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
			}

		// Применение всех записей
		private void ApplyAll_Click (object sender, EventArgs e)
			{
			// Контроль
			if (MessageBox.Show (Localization.GetText ("ApplyAllEntries", al), ProgramDescription.AssemblyTitle,
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
				return;

			if (MessageBox.Show (Localization.GetText ("ApplyAllEntries2", al), ProgramDescription.AssemblyTitle,
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
				return;

			// Применение записей
			uint res = rebm[BasesCombo.SelectedIndex].ApplyAllEntries ();

			MessageBox.Show (string.Format (Localization.GetText ("EntriesApplied", al), res,
				rebm[BasesCombo.SelectedIndex].EntriesCount),
				ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// Обновление таблицы
			int row = 0;
			if (MainTable.SelectedRows.Count > 0)
				row = MainTable.SelectedRows[0].Index;
			UpdateTable ();
			if (MainTable.SelectedRows.Count > 0)
				MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
			}

		// Загрузка из файла реестра
		private void LoadRegFile_Click (object sender, EventArgs e)
			{
			// Контроль
			if (OFDialog.ShowDialog () != DialogResult.OK)
				return;

			// Загрузка
			uint res = rebm[BasesCombo.SelectedIndex].LoadRegistryFile (OFDialog.FileName);

			MessageBox.Show (Localization.GetText ("EntriesAdded", al) + res.ToString (),
				ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// Обновление таблицы
			int row = 0;
			if (MainTable.SelectedRows.Count > 0)
				row = MainTable.SelectedRows[0].Index;
			UpdateTable ();
			if (MainTable.SelectedRows.Count > 0)
				MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
			}

		// Добавление записи
		private void AddRecord_Click (object sender, EventArgs e)
			{
			// Добавление
			int row = 0;
			RegistryEntryEditor ree;

			if (MainTable.SelectedRows.Count > 0)
				{
				row = MainTable.SelectedRows[0].Index;
				ree = new RegistryEntryEditor (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)row), al);
				}
			else
				{
				ree = new RegistryEntryEditor (new RegistryEntry ("HKEY_CLASSES_ROOT\\", "", ""), al);
				}

			if (ree.Confirmed)
				{
				rebm[BasesCombo.SelectedIndex].AddEntry (ree.EditedEntry);

				// Обновление таблицы
				UpdateTable ();
				if (MainTable.SelectedRows.Count > 0)
					MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
				}
			}

		// Редактирование записей
		private void EditRecord_Click (object sender, DataGridViewCellEventArgs e)
			{
			// Контроль
			if (MainTable.SelectedRows.Count <= 0)
				return;

			// Редактирование
			int row = MainTable.SelectedRows[0].Index;
			RegistryEntryEditor ree = new RegistryEntryEditor (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)row), al);
			if (ree.Confirmed)
				{
				rebm[BasesCombo.SelectedIndex].DeleteEntry ((uint)row);
				rebm[BasesCombo.SelectedIndex].AddEntry (ree.EditedEntry);

				// Обновление таблицы
				UpdateTable ();
				MainTable.CurrentCell = MainTable.Rows[row].Cells[0];

				// Запрос на применение
				Apply_Click (null, null);
				}
			}

		private void MainTable_KeyDown (object sender, KeyEventArgs e)
			{
			switch (e.KeyCode)
				{
				case Keys.Return:
					EditRecord_Click (null, null);
					break;

				case Keys.Insert:
					AddRecord_Click (null, null);
					break;

				case Keys.Delete:
					DeleteRecord_Click (null, null);
					break;
				}
			}

		// Выбор текущей базы
		private void BasesCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			UpdateTable ();
			}

		// Добавление базы
		private void AddBase_Click (object sender, EventArgs e)
			{
			RegistryEntriesBaseManager re = new RegistryEntriesBaseManager ();

			MessageBox.Show (Localization.GetText ("NewBaseAdded", al), ProgramDescription.AssemblyTitle,
				 MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

		// Запрос справки
		private void GetHelp_Click (object sender, EventArgs e)
			{
			ProgramDescription.ShowAbout (false);
			}

		// Просмотр иконок
		private void FindIcon_Click (object sender, EventArgs e)
			{
			IconsExtractor ie = new IconsExtractor (al);
			}

		// Регистрация расширения
		private void RegExtension_Click (object sender, EventArgs e)
			{
			ExtensionRegistrator er = new ExtensionRegistrator (rebm[BasesCombo.SelectedIndex], al);
			if (er.Confirmed)
				UpdateTable ();
			}

		// Локализация формы
		private void LanguageCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Сохранение языка
			Localization.CurrentLanguage = al = (SupportedLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			OFDialog.Title = Localization.GetText ("FEMF_OFDialogTitle", al);
			OFDialog.Filter = Localization.GetText ("FEMF_OFDialogFilter", al);

			Localization.SetControlsText (this, al);
			UpdateResults ();
			}
		}
	}
