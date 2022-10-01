using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Форма обеспечивает создание или редактирование записи в базе
	/// </summary>
	public partial class RegistryEntryEditor: Form
		{
		// Параметры
		private SupportedLanguages al;

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
		/// <param name="Entry">Запись реестра</param>
		/// <param name="InterfaceLanguage">Язык интерфейса</param>
		public RegistryEntryEditor (RegistryEntry Entry, SupportedLanguages InterfaceLanguage)
			{
			// Инициализация
			InitializeComponent ();
			al = InterfaceLanguage;
			this.AcceptButton = Apply;
			this.CancelButton = Abort;

			// Сохранение параметров
			editedEntry = Entry;

			// Настройка контролов
			KeyType.Items.Add (RegistryValueTypes.REG_SZ.ToString () +
				Localization.GetText ("RegistryValue_String", al));
			KeyType.Items.Add (RegistryValueTypes.REG_DWORD.ToString () +
				Localization.GetText ("RegistryValue_Int32", al));
			KeyType.Items.Add (RegistryValueTypes.REG_QWORD.ToString () +
				Localization.GetText ("RegistryValue_Int64", al));

			if ((int)editedEntry.ValueType < KeyType.Items.Count)
				KeyType.Text = KeyType.Items[(int)editedEntry.ValueType].ToString ();
			else
				KeyType.Text = KeyType.Items[0].ToString ();

			// Загрузка записи
			KeyPath.Text = editedEntry.ValuePath;
			KeyName.Text = editedEntry.ValueName;
			KeyObject.Text = editedEntry.ValueObject;
			PathMustBeDeleted.Checked = editedEntry.PathMustBeDeleted;
			NameMustBeDeleted.Checked = editedEntry.NameMustBeDeleted;

			// Локализация
			this.Text = ProgramDescription.AssemblyTitle + Localization.GetText ("REE_Title", al);

			Localization.SetControlsText (this, al);
			Apply.Text = Localization.GetText ("ApplyButton", al);
			Abort.Text = Localization.GetText ("AbortButton", al);

			// Запуск
			this.ShowDialog ();
			}

		// Изменение состояния флага удаления раздела
		private void PathMustBeDeleted_CheckedChanged (object sender, EventArgs e)
			{
			KeyName.Enabled = KeyObject.Enabled = KeyType.Enabled = NameMustBeDeleted.Enabled =
				!PathMustBeDeleted.Checked;
			}

		// Отмена изменений
		private void Abort_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Применение изменений
		private void Apply_Click (object sender, EventArgs e)
			{
			// Проверка записи на корректность
			RegistryEntry re = new RegistryEntry (KeyPath.Text, KeyName.Text, KeyObject.Text,
				(RegistryValueTypes)KeyType.SelectedIndex, PathMustBeDeleted.Checked, NameMustBeDeleted.Checked);

			if (!re.IsValid)
				{
				MessageBox.Show (Localization.GetText ("EntryIsIncorrect", al), ProgramDescription.AssemblyTitle,
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
