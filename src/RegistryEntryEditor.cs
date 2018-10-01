using System.Windows.Forms;

namespace FileExtensionsManager
	{
	/// <summary>
	/// Форма обеспечивает создание или редактирование записи в базе
	/// </summary>
	public partial class RegistryEntryEditor:Form
		{
		// Параметры

		/// <summary>
		/// Возвращает изменённую запись
		/// </summary>
		public RegistryEntry EditedEntry
			{
			get
				{
				return editedEntry;
				}
			}
		private RegistryEntry editedEntry;

		/// <summary>
		/// Флаг указывает, что изменение записи было подтверждено
		/// </summary>
		public bool Confirmed
			{
			get
				{
				return confirmed;
				}
			}
		private bool confirmed = false;

		/// <summary>
		/// Конструктор. Запускает создание или редактирование записи
		/// </summary>
		public RegistryEntryEditor (RegistryEntry Entry)
			{
			// Инициализация
			InitializeComponent ();

			// Сохранение параметров
			editedEntry = Entry;

			// Настройка контролов
			KeyType.Items.Add (RegistryValueTypes.REG_SZ.ToString () + " (строковый)");
			KeyType.Items.Add (RegistryValueTypes.REG_DWORD.ToString () + " (целочисленный, 4 байта)");
			KeyType.Items.Add (RegistryValueTypes.REG_QWORD.ToString () + " (целочисленный, 8 байт)");
			if ((int)editedEntry.ValueType <= 2)
				{
				KeyType.Text = KeyType.Items[(int)editedEntry.ValueType].ToString ();
				}
			else
				{
				KeyType.Text = KeyType.Items[0].ToString ();
				}

			// Загрузка записи
			KeyPath.Text = editedEntry.ValuePath;
			KeyName.Text = editedEntry.ValueName;
			KeyObject.Text = editedEntry.ValueObject;
			PathMustBeDeleted.Checked = editedEntry.PathMustBeDeleted;
			NameMustBeDeleted.Checked = editedEntry.NameMustBeDeleted;

			// Запуск
			this.ShowDialog ();
			}

		// Изменение состояния флага удаления раздела
		private void PathMustBeDeleted_CheckedChanged (object sender, System.EventArgs e)
			{
			KeyName.Enabled = KeyObject.Enabled = KeyType.Enabled = NameMustBeDeleted.Enabled = !PathMustBeDeleted.Checked;
			}

		// Отмена изменений
		private void Abort_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// Применение изменений
		private void Apply_Click (object sender, System.EventArgs e)
			{
			// Проверка записи на корректность
			RegistryEntry re = new RegistryEntry (KeyPath.Text, KeyName.Text, KeyObject.Text,
				(RegistryValueTypes)KeyType.SelectedIndex, PathMustBeDeleted.Checked, NameMustBeDeleted.Checked);
			if (!re.IsValid)
				{
				MessageBox.Show ("Запись некорректна. Убедитесь, что путь к разделу реестра корректен, имя параметра не содержит " +
					"символ '\\', а значение соответствует указанному типу, после чего повторите попытку", ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Успешно
			editedEntry = (RegistryEntry)re.Clone ();
			confirmed = true;
			this.Close ();
			}
		}
	}
