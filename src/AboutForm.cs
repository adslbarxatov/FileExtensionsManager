using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает интерфейс отображения сведений о программе
	/// </summary>
	public partial class AboutForm:Form
		{
		// Переменные
		private string projectLink, updatesLink, userManualLink;

		/// <summary>
		/// Конструктор. Запускает форму
		/// </summary>
		/// <param name="InterfaceLanguage">Язык интерфейса</param>
		/// <param name="Description">Описание программы и/или справочная информация</param>
		/// <param name="ProjectLink">Ссылка на страницу проекта;
		/// кнопка отключается, если это значение не задано</param>
		/// <param name="UpdatesLink">Ссылка на страницу обновлений;
		/// кнопка отключается, если это значение не задано</param>
		/// <param name="UserManualLink">Ссылка на страницу руководства пользователя;
		/// кнопка отключается, если это значение не задано</param>
		public AboutForm (SupportedLanguages InterfaceLanguage,
			string ProjectLink, string UpdatesLink, string UserManualLink,
			string Description)
			{
			// Инициализация
			InitializeComponent ();

			// Настройка контролов
			switch (InterfaceLanguage)
				{
				case SupportedLanguages.ru_ru:
					UserManualButton.Text = "Руководство пользователя";
					ProjectPageButton.Text = "Веб-страница проекта";
					UpdatesPageButton.Text = "Веб-страница обновлений";
					this.Text = "О программе";
					break;

				default:	// en_us
					UserManualButton.Text = "User manual";
					ProjectPageButton.Text = "Project webpage";
					UpdatesPageButton.Text = "Updates webpage";
					this.Text = "About application";
					break;
				}

			// Получение параметров
			userManualLink = ((UserManualLink == null) ? "" : UserManualLink);
			projectLink = ((ProjectLink == null) ? "" : ProjectLink);
			updatesLink = ((UpdatesLink == null) ? "" : UpdatesLink);

			DescriptionBox.Text = ((Description == null) ? "" : Description);

			// Загрузка окружения
			AboutLabel.Text = ProgramDescription.AssemblyTitle + "\n" + ProgramDescription.AssemblyDescription + "\n\n" +
				ProgramDescription.AssemblyCopyright + "\nv " + ProgramDescription.AssemblyVersion +
				"; " + ProgramDescription.AssemblyLastUpdate;

			IconBox.BackgroundImage = Icon.ExtractAssociatedIcon (Assembly.GetExecutingAssembly ().Location).ToBitmap ();
			OtherIconBox.BackgroundImage = this.Icon.ToBitmap ();

			// Завершение
			UserManualButton.Enabled = (userManualLink != "");
			ProjectPageButton.Enabled = (projectLink != "");
			UpdatesPageButton.Enabled = (updatesLink != "");

			// Запуск
			this.ShowDialog ();
			}

		/// <summary>
		/// Конструктор. Открывает указанную ссылку без запуска формы
		/// </summary>
		/// <param name="Link">Ссылка для отображения</param>
		public AboutForm (string Link)
			{
			try
				{
				Process.Start (Link);
				}
			catch
				{
				}
			}

		// Закрытие окна
		private void ExitButton_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Изменение размера окна
		private void AboutForm_Resize (object sender, EventArgs e)
			{
			this.Width = this.MinimumSize.Width;

			DescriptionBox.Height = this.Height - 225;
			ExitButton.Top = this.Height - 63;
			}

		// Запуск ссылок
		private void UserManualButton_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (userManualLink);
				}
			catch
				{
				}
			}

		private void ProjectPageButton_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (projectLink);
				}
			catch
				{
				}
			}

		private void UpdatesPageButton_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (updatesLink);
				}
			catch
				{
				}
			}
		}
	}
