using System.IO;
using System.Windows.Forms;

namespace FileExtensionsManager
	{
	/// <summary>
	/// Форма обеспечивает добавление расширения файла в реестр
	/// </summary>
	public partial class ExtensionRegistrator:Form
		{
		// Параметры
		private RegistryEntriesBaseManager rebm;
		private string selectedIconFile;
		private uint selectedIconNumber;

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
		/// Конструктор. Запускает добавление расширения
		/// </summary>
		/// <param name="Manager">База записей, в которую необходимо добавить расширение</param>
		public ExtensionRegistrator (RegistryEntriesBaseManager Manager)
			{
			// Инициализация
			InitializeComponent ();

			OFDialog.Title = "Выберите приложение для открытия этого типа файлов";
			OFDialog.Filter = "Приложения|*.exe";

			// Сохранение параметров
			rebm = Manager;

			// Запуск
			this.Text = ProgramDescription.AssemblyTitle + " – регистрация расширения";
			this.ShowDialog ();
			}

		// Отмена изменений
		private void Abort_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// Применение изменений
		private void Apply_Click (object sender, System.EventArgs e)
			{
			// Контроль остальных параметров
			if (FileExtension.Text.Length * FileTypeName.Text.Length * FileIcon.Text.Length * FileApplication.Text.Length == 0)
				{
				MessageBox.Show ("Одно из полей незаполнено",
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Контроль расширения
			FileExtension.Text = FileExtension.Text.ToLower ().Replace (".", "");
			char[] c = Path.GetInvalidFileNameChars ();
			for (int i = 0; i < c.Length; i++)
				{
				if (FileExtension.Text.Contains (c[i].ToString ()))
					{
					MessageBox.Show ("Поле расширения файла содержит недопустимый символ «" + c[i].ToString () + "»",
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
					}
				}

			uint a = IconsExtractor.GetIconsCount ("s");

			// Пробуем добавлять
			if (!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\." + FileExtension.Text, "", FileExtension.Text + "file")) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file", "", FileTypeName.Text)) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\DefaultIcon", "",
					selectedIconFile + "," + selectedIconNumber.ToString ())) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell", "", "open")) ||
				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell\\open", "", "&Открыть")) ||
				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell\\open", "Icon",
					((IconsExtractor.GetIconsCount (FileApplication.Text) == 0) ? selectedIconFile : FileApplication.Text) + ",0")) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell\\open\\command", "",
					"\"" + FileApplication.Text + "\" \"%1\"")))
				{
				MessageBox.Show ("Не удалось создать одну или несколько записей для регистрации расширения. Убедитесь, что у программы " +
					"есть права для работы с реестром, после чего повторите попытку",
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			confirmed = true;
			this.Close ();
			}

		// Выбор значка файла
		private void SelectIcon_Click (object sender, System.EventArgs e)
			{
			// Запуск выбора
			IconsExtractor ie = new IconsExtractor ();
			if (ie.SelectedIconNumber >= 0)
				{
				selectedIconNumber = (uint)ie.SelectedIconNumber;
				selectedIconFile = ie.SelectedIconFile;
				FileIcon.Text = selectedIconFile + ", " + selectedIconNumber.ToString ();
				}
			}

		// Выбор приложения
		private void SelectApplication_Click (object sender, System.EventArgs e)
			{
			if (OFDialog.ShowDialog () == DialogResult.OK)
				FileApplication.Text = OFDialog.FileName;
			}
		}
	}
