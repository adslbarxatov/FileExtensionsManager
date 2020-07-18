using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
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
		private SupportedLanguages al;
		private string updatesMessage = "";

		/// <summary>
		/// Основная Git-ссылка
		/// </summary>
		public const string DefaultGitLink = "https://github.com/adslbarxatov/";

		/// <summary>
		/// Часть ссылки на раздел Git-обновлений
		/// </summary>
		public const string GitUpdatesSublink = "/releases";

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
		public AboutForm (SupportedLanguages InterfaceLanguage, string ProjectLink,
			string UpdatesLink, string UserManualLink, string Description)
			{
			// Инициализация
			InitializeComponent ();
			al = InterfaceLanguage;

			// Настройка контролов
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					UserManualButton.Text = "Руководство";
					ProjectPageButton.Text = "О проекте";
					UpdatesPageButton.Text = "Обновления";
					ADPButton.Text = "Политика и EULA";
					AvailableUpdatesLabel.Text = "проверяются...";

					this.Text = "О программе";
					break;

				default:	// en_us
					UserManualButton.Text = "User manual";
					ProjectPageButton.Text = "Project webpage";
					UpdatesPageButton.Text = "Updates webpage";
					ADPButton.Text = "Policy and EULA";
					AvailableUpdatesLabel.Text = "checking...";

					this.Text = "About application";
					break;
				}

			// Получение параметров
			userManualLink = ((UserManualLink == null) ? "" : UserManualLink);

			if (ProjectLink == null)
				projectLink = "";
			else if (ProjectLink == "*")
				projectLink = DefaultGitLink + ProgramDescription.AssemblyMainName;
			else
				projectLink = ProjectLink;

			if (UpdatesLink == null)
				updatesLink = "";
			else if (UpdatesLink == "*")
				updatesLink = DefaultGitLink + ProgramDescription.AssemblyMainName + GitUpdatesSublink;
			else
				updatesLink = UpdatesLink;

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

			// Запуск проверки обновлений
			HardWorkExecutor hwe = new HardWorkExecutor (UpdatesChecker, null, "");
			UpdatesTimer.Enabled = true;

			// Запуск
			this.ShowDialog ();
			}

		/// <summary>
		/// Конструктор. Открывает указанную ссылку без запуска формы
		/// </summary>
		/// <param name="Link">Ссылка для отображения (если null, запускается стандартная)</param>
		public AboutForm (string Link)
			{
			try
				{
				if (Link == null)
					Process.Start (DefaultGitLink + ProgramDescription.AssemblyMainName + GitUpdatesSublink);
				else
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

		private void ADP_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start ("https://vk.com/@rdaaow_fupl-adp");
				}
			catch
				{
				}
			}

		// Метод-исполнитель проверки обновлений
		private void UpdatesChecker (object sender, DoWorkEventArgs e)
			{
			// Настройка безопасности соединения
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xFC0;
			// Принудительно открывает TLS1.0, TLS1.1 и TLS1.2; блокирует SSL3

			// Запрос обновлений пакета
			HttpWebRequest rq = (HttpWebRequest)WebRequest.Create (projectLink);
			rq.Method = "GET";
			rq.KeepAlive = false;
			rq.Timeout = 10000;

			// Отправка запроса
			HttpWebResponse resp = null;
			string html = "";
			try
				{
				resp = (HttpWebResponse)rq.GetResponse ();
				}
			catch
				{
				goto htmlError;
				}

			// Чтение ответа
			StreamReader SR = new StreamReader (resp.GetResponseStream ());
			html = SR.ReadToEnd ();
			SR.Close ();
			resp.Close ();

			// Разбор ответа (извлечение версий и PCC)
			string version = "";
			string[] htmlMarkers = { "</a>" + ProgramDescription.AssemblyMainName, "</h1>" };

			int i = html.IndexOf (htmlMarkers[0]);
			if (i < 0)
				goto htmlError;

			i += htmlMarkers[0].Length;

			int j = html.IndexOf (htmlMarkers[1], i);
			if ((j < 0) || (j <= i))
				goto htmlError;

			version = html.Substring (i, j - i).Trim ();

			// Отображение результата
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					if (ProgramDescription.AssemblyTitle.EndsWith (version))
						updatesMessage = "обновлений нет";
					else
						updatesMessage = "доступна " + version;
					break;

				default:	// en_us
					if (ProgramDescription.AssemblyTitle.EndsWith (version))
						updatesMessage = "no updates";
					else
						updatesMessage = version + " available";
					break;
				}


			e.Result = 0;
			return;

			// Есть проблема при загрузке страницы. Отмена
htmlError:
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					updatesMessage = "недоступны";
					break;

				default:	// en_us
					updatesMessage = "unavailable";
					break;
				}

			e.Result = -2;
			return;
			}

		// Контроль сообщения об обновлении
		private void UpdatesTimer_Tick (object sender, EventArgs e)
			{
			if (updatesMessage != "")
				{
				if (AvailableUpdatesLabel.Text == "")
					{
					AvailableUpdatesLabel.Text = updatesMessage;
					if (!updatesMessage.Contains ("."))
						{
						UpdatesTimer.Enabled = false;
						}
					else
						{
						UpdatesTimer.Interval = 1000;
						}
					}
				else
					{
					AvailableUpdatesLabel.Text = "";
					}
				}
			}
		}
	}
