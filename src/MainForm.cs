using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FileExtensionsManager
	{
	/// <summary>
	/// Главная форма программы
	/// </summary>
	public partial class MainForm:Form
		{
		// Переменные
		private List<RegistryEntriesBaseManager> rebm = new List<RegistryEntriesBaseManager> ();

		/// <summary>
		/// Конструктор. Инициализирует главную форму приложения
		/// </summary>
		public MainForm ()
			{
			InitializeComponent ();
			}

		private void MainForm_Load (object sender, System.EventArgs e)
			{
			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;

			MainTable.Columns.Add ("Entries", "Записи");

			OFDialog.Title = "Выберите файл реестра для загрузки";
			OFDialog.Filter = "Файлы реестра Windows (*.reg)|*.reg";

			// Инициализация баз реестровых записей
			string[] files = Directory.GetFiles (Application.StartupPath, "*" + RegistryEntriesBaseManager.baseFileExtension);
			for (int i = 0; i < files.Length; i++)
				{
				RegistryEntriesBaseManager re = new RegistryEntriesBaseManager (Path.GetFileNameWithoutExtension (files[i]));
				if (re.IsInited)
					rebm.Add (re);
				}

			if (rebm.Count == 0)
				{
				MessageBox.Show ("Не найдена ни одна база реестровых записей\n\nПрограмма создаст пустой файл-образец. " +
					"Дайте образцу нужное имя, после чего запустите программу снова",
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

				RegistryEntriesBaseManager re = new RegistryEntriesBaseManager ();

				this.Close ();
				return;
				}

			// Загрузка списка
			for (int i = 0; i < rebm.Count; i++)
				{
				BasesCombo.Items.Add (rebm[i].BaseName);
				}
			BasesCombo.SelectedIndex = 0;

			UpdateTable ();
			}

		// Изменение размера окна
		private void MainForm_Resize (object sender, System.EventArgs e)
			{
			MainTable.Width = this.Width - 32;
			MainTable.Height = this.Height - 186;

			Applied.Top = PartiallyApplied.Top = this.Height - 138;
			NotApplied.Top = NoAccess.Top = this.Height - 118;

			AddRecord.Top = DeleteRecord.Top = LoadRegFile.Top = this.Height - 95;
			Apply.Top = ApplyAll.Top = Exit.Top = this.Height - 64;
			}

		// Обновление таблицы
		private void UpdateTable ()
			{
			// Запрос
			List<string> presentation = rebm[BasesCombo.SelectedIndex].EntriesBasePresentation;
			List<RegistryEntryApplicationResults> statuses = rebm[BasesCombo.SelectedIndex].EntriesStatusesPresentation;

			// Обновление
			uint applied = 0,
				partiallyApplied = 0,
				notApplied = 0,
				noAccess = 0;
			MainTable.Rows.Clear ();
			for (int i = 0; i < presentation.Count; i++)
				{
				MainTable.Rows.Add (/*new DataGridViewRow ()*/);
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
			Applied.Text = "Применены: " + applied.ToString ();
			PartiallyApplied.Text = "Применены частично: " + partiallyApplied.ToString ();
			NotApplied.Text = "Не применены: " + notApplied.ToString ();
			NoAccess.Text = "Недоступны: " + noAccess.ToString ();
			}

		// Выход из программы
		private void Exit_Click (object sender, System.EventArgs e)
			{
			// Сохранение баз
			for (int i = 0; i < rebm.Count; i++)
				rebm[i].SaveBase ();

			// Выход
			this.Close ();
			}

		// Удаление записи
		private void DeleteRecord_Click (object sender, System.EventArgs e)
			{
			if (MainTable.SelectedRows.Count > 0)
				if (MessageBox.Show ("Удалить выбранную запись?", ProgramDescription.AssemblyTitle,
					MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
					int row = MainTable.SelectedRows[0].Index;

					rebm[BasesCombo.SelectedIndex].DeleteEntry ((uint)row);
					UpdateTable ();

					if (rebm[BasesCombo.SelectedIndex].EntriesCount > 0)
						{
						if (rebm[BasesCombo.SelectedIndex].EntriesCount == row)
							row--;

						MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
						}
					}
			}

		// Применение выбранной записи
		private void Apply_Click (object sender, System.EventArgs e)
			{
			if (MainTable.SelectedRows.Count > 0)
				if (MessageBox.Show ("Применить выбранную запись?", ProgramDescription.AssemblyTitle,
					MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
					// Применение записи
					string msg = "Запись успешно применена";

					switch (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)MainTable.SelectedRows[0].Index).ApplyEntry ())
						{
						case RegistryEntryApplicationResults.CannotGetAccess:
							msg = "Запись не может быть применена: раздел реестра недоступен";
							break;

						case RegistryEntryApplicationResults.PartiallyApplied:
						case RegistryEntryApplicationResults.NotApplied:
							msg = "Не удалось применить запись";
							break;
						}

					MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Обновление таблицы
					int row = MainTable.SelectedRows[0].Index;
					UpdateTable ();
					MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
					}
			}

		// Применение всех записей
		private void ApplyAll_Click (object sender, System.EventArgs e)
			{
			if (MessageBox.Show ("Применить все записи?", ProgramDescription.AssemblyTitle,
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				if (MessageBox.Show ("Ваше решение изменилось?", ProgramDescription.AssemblyTitle,
					MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
					{
					// Применение записей
					uint res = rebm[BasesCombo.SelectedIndex].ApplyAllEntries ();

					MessageBox.Show ("Применено записей: " + res.ToString () + " из " +
						rebm[BasesCombo.SelectedIndex].EntriesCount.ToString (),
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Обновление таблицы
					int row = 0;
					if (MainTable.SelectedRows.Count > 0)
						row = MainTable.SelectedRows[0].Index;
					UpdateTable ();
					if (MainTable.SelectedRows.Count > 0)
						MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
					}
			}

		// Загрузка из файла реестра
		private void LoadRegFile_Click (object sender, System.EventArgs e)
			{
			if (OFDialog.ShowDialog () == DialogResult.OK)
				{
				// Загрузка
				uint res = rebm[BasesCombo.SelectedIndex].LoadRegistryFile (OFDialog.FileName);

				MessageBox.Show ("Добавлено записей: " + res.ToString (),
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

				// Обновление таблицы
				int row = 0;
				if (MainTable.SelectedRows.Count > 0)
					row = MainTable.SelectedRows[0].Index;
				UpdateTable ();
				if (MainTable.SelectedRows.Count > 0)
					MainTable.CurrentCell = MainTable.Rows[row].Cells[0];
				}
			}

		// Добавление записи
		private void AddRecord_Click (object sender, System.EventArgs e)
			{
			// Добавление
			int row = 0;
			RegistryEntryEditor ree;

			if (MainTable.SelectedRows.Count > 0)
				{
				row = MainTable.SelectedRows[0].Index;
				ree = new RegistryEntryEditor (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)row));
				}
			else
				{
				ree = new RegistryEntryEditor (new RegistryEntry ("HKEY_CLASSES_ROOT\\", "", ""));
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

		// Редактирование записи
		private void MainTable_CellDoubleClick (object sender, DataGridViewCellEventArgs e)
			{
			if (MainTable.SelectedRows.Count > 0)
				{
				// Редактирование
				int row = MainTable.SelectedRows[0].Index;
				RegistryEntryEditor ree = new RegistryEntryEditor (rebm[BasesCombo.SelectedIndex].GetRegistryEntry ((uint)row));
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
			}

		// Выбор текущей базы
		private void BasesCombo_SelectedIndexChanged (object sender, System.EventArgs e)
			{
			UpdateTable ();
			}

		// Добавление базы
		private void AddBase_Click (object sender, System.EventArgs e)
			{
			RegistryEntriesBaseManager re = new RegistryEntriesBaseManager ();

			MessageBox.Show ("Программа создала пустой файл-образец базы реестровых записей. " +
				"Дайте образцу нужное имя, после чего перезапустите программу", ProgramDescription.AssemblyTitle,
				 MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

		// Запрос справки
		private void GetHelp_Click (object sender, System.EventArgs e)
			{
			MessageBox.Show (ProgramDescription.AssemblyTitle + "\n" + ProgramDescription.AssemblyDescription + "\n" +
				ProgramDescription.AssemblyCopyright +

				"\n\nПрограмма позволяет хранить и применять пользовательские настройки расширений файлов, зарегистрированных в системе. " +
				"В списке записей отображаются реестровые сопоставления, хранящиеся в текущей выбранной базе записей. Зелёным цветом " +
				"обозначены записи, уже имеющиеся в реестре Windows, синим – записи, неполностью соответствующие пользовательским " +
				"настройкам, а красным – отсутствующие записи. Если запись помечена как удаляемая, то зелёным помечаются отсутствующие в " +
				"реестре записи, а красным – присутствующие. Наличие серых записей может говорить об отсутствии доступа к реестру Windows.\n\n" +

				"Изменить запись можно по двойному щелчку мыши на соответствующей строке, добавить – с помощью соответствующей кнопки. " +
				"В обоих случаях сведения редактируются в специальном окне, а их корректность контролируется программой. Добавить записи " +
				"можно также и из файла реестра Windows.\n\n" +

				"Применять записи можно по одной или все сразу (все неприменённые). Кнопка «Выход» " +
				"сохраняет все изменения во всех базах. Если изменения нужно отменить, программу нужно закрыть крестиком в верхнем правом " +
				"углу окна.\n\n" +

				"На данный момент программа работает только с записями в ветке реестра HKEY_CLASSES_ROOT и только с типами параметров " +
				"REG_SZ, REG_DWORD и REG_QWORD",
				
				ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
