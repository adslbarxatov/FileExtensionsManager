using System;
using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Форма обеспечивает добавление расширения файла в реестр
	/// </summary>
	public partial class ExtensionRegistrator: Form
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
			this.AcceptButton = Apply;
			this.CancelButton = Abort;

			OFDialog.Title = Localization.GetText ("ER_OFDialogText");
			OFDialog.Filter = Localization.GetText ("ER_OFDialogFilter");

			// Сохранение параметров
			rebm = Manager;

			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle + Localization.GetText ("ER_Title");

			Localization.SetControlsText (this);
			Apply.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Apply);
			Abort.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel);

			// Запуск
			this.ShowDialog ();
			}

		// Отмена изменений
		private void Abort_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Применение изменений
		private void Apply_Click (object sender, EventArgs e)
			{
			// Контроль остальных параметров
			if (FileExtension.Text.Length * FileTypeName.Text.Length * FileIcon.Text.Length *
				FileApplication.Text.Length == 0)
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning, "SomeFieldsAreEmpty");
				return;
				}

			// Контроль расширения
			FileExtension.Text = FileExtension.Text.ToLower ().Replace (".", "");
			char[] c = Path.GetInvalidFileNameChars ();
			for (int i = 0; i < c.Length; i++)
				{
				if (FileExtension.Text.Contains (c[i].ToString ()))
					{
					RDGenerics.MessageBox (RDMessageTypes.Warning,
						string.Format (Localization.GetText ("UnsupportedCharacter"), c[i].ToString ()));
					return;
					}
				}

			uint a = IconsExtractor.GetIconsCount ("s");

			// Пробуем добавлять
			if (!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\." + FileExtension.Text, "",
					FileExtension.Text + "file")) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file", "",
					FileTypeName.Text)) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\DefaultIcon",
					"", selectedIconFile + "," + selectedIconNumber.ToString ())) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell", "",
					"open")) ||
				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell\\open",
					"", Localization.GetDefaultText (LzDefaultTextValues.Button_Open))) ||
				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text + "file\\shell\\open",
					"Icon",
				((IconsExtractor.GetIconsCount (FileApplication.Text) == 0) ? selectedIconFile :
					FileApplication.Text) + ",0")) ||

				!rebm.AddEntry (new RegistryEntry ("HKEY_CLASSES_ROOT\\" + FileExtension.Text +
					"file\\shell\\open\\command", "", "\"" + FileApplication.Text + "\" \"%1\"")))
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning, "ExtensionRegFailed");
				return;
				}

			confirmed = true;
			this.Close ();
			}

		// Выбор значка файла
		private void SelectIcon_Click (object sender, EventArgs e)
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
		private void SelectApplication_Click (object sender, EventArgs e)
			{
			if (OFDialog.ShowDialog () == DialogResult.OK)
				FileApplication.Text = OFDialog.FileName;
			}
		}
	}
