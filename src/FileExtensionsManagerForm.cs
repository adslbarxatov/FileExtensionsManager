using System;
using System.Collections.Generic;
using System.Drawing;
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
			if (!RDGenerics.IsRegistryAccessible)
				this.Text += Localization.GetDefaultText (LzDefaultTextValues.Message_LimitedFunctionality);

			RDGenerics.LoadWindowDimensions (this);

			LanguageCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)Localization.CurrentLanguage;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}

			MainTable.Columns.Add ("Entries", "Entries");
			MainTable.ContextMenu = new ContextMenu (new MenuItem[] {
				new MenuItem (EditRecord.Text, EditRecord_Click),
				new MenuItem (Apply.Text, Apply_Click),
				new MenuItem (DeleteRecord.Text, DeleteRecord_Click),
				});

			// Инициализация баз реестровых записей
			if (Directory.Exists (RDGenerics.AppStartupPath + RegistryEntriesBaseManager.BasesSubdirectory))
				{
				List<string> files = new List<string> ();
				files.AddRange (Directory.GetFiles (RDGenerics.AppStartupPath +
				   RegistryEntriesBaseManager.BasesSubdirectory,
				   "*" + RegistryEntriesBaseManager.OldBaseFileExtension));
				files.AddRange (Directory.GetFiles (RDGenerics.AppStartupPath +
				   RegistryEntriesBaseManager.BasesSubdirectory,
				   "*" + RegistryEntriesBaseManager.NewBaseFileExtension));

				for (int i = 0; i < files.Count; i++)
					{
					RegistryEntriesBaseManager re =
						new RegistryEntriesBaseManager (Path.GetFileNameWithoutExtension (files[i]));
					if (re.IsInited)
						rebm.Add (re);
					}
				}

			if (rebm.Count == 0)
				{
				RDGenerics.MessageBox (RDMessageTypes.Information_Left,
					Localization.GetText ("BasesNotFound") + "." + Localization.RNRN +
					Localization.GetText ("NewBaseAdded"));

				RegistryEntriesBaseManager _ = new RegistryEntriesBaseManager ();

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
			MainTable.Height = this.Height - 196;

			ButtonsPanel.Top = this.Height - 147;
			ButtonsPanel.Left = (this.Width - ButtonsPanel.Width) / 2 - 4;
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
			Applied.Text = Localization.GetText ("AppliedText") + applied.ToString ();
			PartiallyApplied.Text = Localization.GetText ("PartiallyAppliedText") + partiallyApplied.ToString ();
			NotApplied.Text = Localization.GetText ("NotAppliedText") + notApplied.ToString ();
			NoAccess.Text = Localization.GetText ("NoAccessText") + noAccess.ToString ();
			}

		// Выход из программы
		private void Exit_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		private void FileExtensionsManagerForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			RDMessageButtons res = RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center,
				"SaveBasesMessage", LzDefaultTextValues.Button_Yes, LzDefaultTextValues.Button_No,
				LzDefaultTextValues.Button_Cancel);

			// Сохранение баз
			if (res == RDMessageButtons.ButtonOne)
				{
				for (int i = 0; i < rebm.Count; i++)
					rebm[i].SaveBase ();
				}

			// Выход
			RDGenerics.SaveWindowDimensions (this);
			e.Cancel = (res == RDMessageButtons.ButtonThree);
			}

		// Удаление записи
		private void DeleteRecord_Click (object sender, EventArgs e)
			{
			// Контроль
			if (MainTable.SelectedRows.Count <= 0)
				return;

			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center, "RemoveEntry",
				LzDefaultTextValues.Button_YesNoFocus, LzDefaultTextValues.Button_No) != RDMessageButtons.ButtonOne)
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

			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center, "ApplyEntry",
				LzDefaultTextValues.Button_Yes, LzDefaultTextValues.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;

			// Применение записей
			for (int i = 0; i < MainTable.SelectedRows.Count; i++)
				{
				string msg = "";
				switch (rebm[BasesCombo.SelectedIndex].GetRegistryEntry
					((uint)MainTable.SelectedRows[i].Index).ApplyEntry ())
					{
					case RegistryEntryApplicationResults.CannotGetAccess:
						msg = Localization.GetText ("EntryIsUnavailable");
						break;

					case RegistryEntryApplicationResults.PartiallyApplied:
					case RegistryEntryApplicationResults.NotApplied:
						msg = Localization.GetText ("EntryNotApplied");
						break;
					}

				if (!string.IsNullOrWhiteSpace (msg))
					RDGenerics.MessageBox (RDMessageTypes.Warning_Center, msg);
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
			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "ApplyAllEntries",
				LzDefaultTextValues.Button_YesNoFocus, LzDefaultTextValues.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;

			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "ApplyAllEntries2",
				LzDefaultTextValues.Button_Yes, LzDefaultTextValues.Button_No) !=
				RDMessageButtons.ButtonTwo)
				return;

			// Применение записей
			uint res = rebm[BasesCombo.SelectedIndex].ApplyAllEntries ();

			RDGenerics.MessageBox (RDMessageTypes.Success_Center,
				string.Format (Localization.GetText ("EntriesApplied"), res,
				rebm[BasesCombo.SelectedIndex].EntriesCount));

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

			RDGenerics.MessageBox (RDMessageTypes.Success_Center,
				Localization.GetText ("EntriesAdded") + res.ToString ());

			// Обновление таблицы
			int row = 0;
			if (MainTable.SelectedRows.Count > 0)
				row = MainTable.SelectedRows[0].Index;

			UpdateTable ();
			if (MainTable.SelectedRows.Count > 0)
				MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
			}

		// Сохранение в файл реестра
		private void SaveRegFile_Click (object sender, EventArgs e)
			{
			// Контроль
			if (MainTable.SelectedRows.Count <= 0)
				return;

			// Извлечение выборки
			List<int> idx = new List<int> ();
			foreach (DataGridViewRow r in MainTable.SelectedRows)
				idx.Add (r.Index);
			idx.Sort ();

			// Запрос сохранения
			if (SFDialog.ShowDialog () != DialogResult.OK)
				return;

			// Выполнение
			if (!rebm[BasesCombo.SelectedIndex].SaveRegistryFile (SFDialog.FileName, idx))
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					Localization.GetFileProcessingMessage (SFDialog.FileName,
					LzFileProcessingMessageTypes.Save_Failure));
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
				ree = new RegistryEntryEditor (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)row), true);
				}
			else
				{
				ree = new RegistryEntryEditor (new RegistryEntry ("HKEY_CLASSES_ROOT\\", "", ""), true);
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
		private void EditRecord_Click (object sender, EventArgs e)
			{
			EditRecord_Click (null, null);
			}

		private void EditRecord_Click (object sender, DataGridViewCellEventArgs e)
			{
			// Контроль
			if (MainTable.SelectedRows.Count <= 0)
				return;

			// Редактирование
			int row = MainTable.SelectedRows[0].Index;
			RegistryEntryEditor ree = new RegistryEntryEditor
				(rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)row), false);
			if (!ree.Confirmed)
				return;

			rebm[BasesCombo.SelectedIndex].DeleteEntry ((uint)row);
			rebm[BasesCombo.SelectedIndex].AddEntry (ree.EditedEntry);

			// Обновление таблицы
			UpdateTable ();
			MainTable.CurrentCell = MainTable.Rows[row].Cells[0];

			// Запрос на применение
			Apply_Click (null, null);
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

			RDGenerics.LocalizedMessageBox (RDMessageTypes.Success_Center, "NewBaseAdded");
			}

		// Запрос справки
		private void GetHelp_Click (object sender, EventArgs e)
			{
			RDGenerics.ShowAbout (false);
			}

		// Вызов меню
		private void MainTable_CellContextClick (object sender, DataGridViewCellMouseEventArgs e)
			{
			if (e.Button == MouseButtons.Right)
				MainTable.ContextMenu.Show ((Control)sender, new Point (e.X,
					e.Y + (e.RowIndex - MainTable.FirstDisplayedScrollingRowIndex) * MainTable.RowTemplate.Height));
			}

		// Просмотр иконок
		private void FindIcon_Click (object sender, EventArgs e)
			{
			IconsExtractor ie = new IconsExtractor ();
			}

		// Регистрация расширения
		private void RegExtension_Click (object sender, EventArgs e)
			{
			ExtensionRegistrator er = new ExtensionRegistrator (rebm[BasesCombo.SelectedIndex]);
			if (er.Confirmed)
				UpdateTable ();
			}

		// Локализация формы
		private void LanguageCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Сохранение языка
			Localization.CurrentLanguage = (SupportedLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			OFDialog.Title = SFDialog.Title = Localization.GetText ("FEMF_OFDialogTitle");
			OFDialog.Filter = SFDialog.Filter = Localization.GetText ("FEMF_OFDialogFilter");

			Localization.SetControlsText (this);
			Localization.SetControlsText (ButtonsPanel);
			AddRecord.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Add);
			EditRecord.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Edit);
			Exit.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Exit);

			UpdateResults ();
			}
		}
	}
