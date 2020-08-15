using Microsoft.Win32;
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
		private string updatesMessage = "", description = "", policyLoaderCaption = "";

		private const string adpLink = "https://vk.com/@rdaaow_fupl-adp";			// Ссылка на Политику
		private const string defaultGitLink = "https://github.com/adslbarxatov/";	// Мастер-ссылка проекта
		private const string gitUpdatesSublink = "/releases";						// Часть пути для перехода к релизам

		private const string lastShownVersionKey = "HelpShownAt";		// Ключ реестра, хранящий версию, на которой отображалась справка

		private bool accepted = false;									// Флаг принятия Политики

		/// <summary>
		/// Конструктор. Инициализирует форму
		/// </summary>
		/// <param name="ProjectLink">Ссылка на страницу проекта;
		/// кнопка отключается, если это значение не задано</param>
		/// <param name="UpdatesLink">Ссылка на страницу обновлений;
		/// кнопка отключается, если это значение не задано</param>
		/// <param name="UserManualLink">Ссылка на страницу руководства пользователя;
		/// кнопка отключается, если это значение не задано</param>
		public AboutForm (string ProjectLink, string UpdatesLink, string UserManualLink)
			{
			// Инициализация
			InitializeComponent ();

			// Получение параметров
			userManualLink = ((UserManualLink == null) ? "" : UserManualLink);

			if (ProjectLink == null)
				projectLink = "";
			else if (ProjectLink == "*")
				projectLink = defaultGitLink + ProgramDescription.AssemblyMainName;
			else
				projectLink = ProjectLink;

			if (UpdatesLink == null)
				updatesLink = "";
			else if (UpdatesLink == "*")
				updatesLink = defaultGitLink + ProgramDescription.AssemblyMainName + gitUpdatesSublink;
			else
				updatesLink = UpdatesLink;

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
			}

		/// <summary>
		/// Метод отображает справочное окно приложения
		/// </summary>
		/// <param name="InterfaceLanguage">Язык интерфейса</param>
		/// <param name="Description">Описание программы и/или справочная информация</param>
		/// <param name="StartupMode">Флаг, указывающий, что справка не должна отображаться, если
		/// она уже была показана для данной версии приложения</param>
		public void ShowAbout (SupportedLanguages InterfaceLanguage, string Description, bool StartupMode)
			{
			al = InterfaceLanguage;
			description = Description;
			LaunchForm (StartupMode, false);
			}

		/// <summary>
		/// Метод запускает окно в режиме принятия Политики
		/// </summary>
		/// <param name="InterfaceLanguage">Язык интерфейса</param>
		/// <returns>Возвращает 0, если Политика принята;
		/// 1, если Политика уже принималась ранее;
		/// -1, если Политика отклонена</returns>
		public int AcceptEULA (SupportedLanguages InterfaceLanguage)
			{
			al = InterfaceLanguage;
			return LaunchForm (false, true);
			}

		// Основной метод запуска окна
		private int LaunchForm (bool StartupMode, bool AcceptMode)
			{
			// Запрос последней версии
			string helpShownAt = "";
			if (StartupMode || AcceptMode)
				{
				try
					{
					helpShownAt = Registry.GetValue (ProgramDescription.AssemblySettingsKey,
						lastShownVersionKey, "").ToString ();
					}
				catch
					{
					}
				}

			// Контроль
			if (StartupMode && (helpShownAt == ProgramDescription.AssemblyVersion) ||	// Справка уже отображалась
				AcceptMode && (helpShownAt != ""))			// Политика уже принята
				return 1;

			// Настройка контролов
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					UserManualButton.Text = "Руководство";
					ProjectPageButton.Text = "О проекте";
					UpdatesPageButton.Text = "Обновления";
					ADPButton.Text = AcceptMode ? "Открыть в браузере" : "Политика и EULA";
					AvailableUpdatesLabel.Text = "проверяются...";
					ExitButton.Text = AcceptMode ? "&Принять" : "&ОК";
					MisacceptButton.Text = "О&тклонить";
					DescriptionBox.Text = AcceptMode ? "Не удалось получить текст Политики. " +
						"Попробуйте использовать кнопку перехода в браузер" : description;
					policyLoaderCaption = "Подготовка к первому запуску...";

					this.Text = AcceptMode ? "Политика разработки и соглашение пользователя" : "О программе";
					break;

				default:	// en_us
					UserManualButton.Text = "User manual";
					ProjectPageButton.Text = "Project webpage";
					UpdatesPageButton.Text = "Updates webpage";
					ADPButton.Text = AcceptMode ? "Open in browser" : "Policy and EULA";
					AvailableUpdatesLabel.Text = "checking...";
					ExitButton.Text = AcceptMode ? "&Accept" : "&OK";
					MisacceptButton.Text = "&Decline";
					DescriptionBox.Text = AcceptMode ? "Failed to get Policy text. Try button to open it in browser" : description;
					policyLoaderCaption = "Preparing for first launch...";

					this.Text = AcceptMode ? "Development policy and user agreement" : "About application";
					break;
				}

			// Запуск проверки обновлений
			HardWorkExecutor hwe;
			if (!AcceptMode)
				{
				hwe = new HardWorkExecutor (UpdatesChecker, null, "");
				UpdatesTimer.Enabled = true;
				}

			// Получение Политики
			else
				{
				hwe = new HardWorkExecutor (PolicyLoader, null, policyLoaderCaption);

				string html = hwe.Result.ToString ();
				if (html != "")
					DescriptionBox.Text = html;
				}

			// Настройка контролов
			UserManualButton.Visible = ProjectPageButton.Visible = UpdatesPageButton.Visible =
				AvailableUpdatesLabel.Visible = !AcceptMode;
			MisacceptButton.Visible = AcceptMode;

			// Запуск
			this.ShowDialog ();

			// Запись версии по завершению
			if (StartupMode)
				{
				try
					{
					Registry.SetValue (ProgramDescription.AssemblySettingsKey, lastShownVersionKey,
						ProgramDescription.AssemblyVersion);
					}
				catch
					{
					}
				}

			// Завершение
			return accepted ? 0 : -1;
			}

		// Метод получает Политику разработки
		private void PolicyLoader (object sender, DoWorkEventArgs e)
			{
			string html = GetHTML (adpLink);
			int textLeft = 0, textRight = 0;
			if (((textLeft = html.IndexOf ("<h3")) >= 0) && ((textRight = html.IndexOf ("<script", textLeft)) >= 0))
				{
				// Обрезка
				html = html.Substring (textLeft, textRight - textLeft);

				// Формирование абзацных отступов
				html = html.Replace ("<br/>", "\r\n\r\n").Replace ("</p>", "\r\n\r\n").
					Replace ("</li>", "\r\n\r\n").Replace ("</h1>", "\r\n\r\n").Replace ("</h3>", "\r\n\r\n");

				// Удаление вложенных тегов
				while (((textLeft = html.IndexOf ("<")) >= 0) && ((textRight = html.IndexOf (">", textLeft)) >= 0))
					html = html.Replace (html.Substring (textLeft, textRight - textLeft + 1), "");

				// Удаление двойных пробелов
				while (html.IndexOf ("  ") >= 0)
					html = html.Replace ("  ", " ");
				}

			e.Result = html;
			return;
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
					Process.Start (defaultGitLink + ProgramDescription.AssemblyMainName + gitUpdatesSublink);
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
			accepted = true;
			UpdatesTimer.Enabled = false;
			this.Close ();
			}

		// Изменение размера окна
		private void AboutForm_Resize (object sender, EventArgs e)
			{
			this.Width = this.MinimumSize.Width;

			DescriptionBox.Height = this.Height - 225;
			ExitButton.Top = MisacceptButton.Top = this.Height - 63;
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
				Process.Start (adpLink);
				}
			catch
				{
				}
			}

		// Метод-исполнитель проверки обновлений
		private void UpdatesChecker (object sender, DoWorkEventArgs e)
			{
			// Запрос обновлений пакета
			string html = GetHTML (projectLink);

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

		/// <summary>
		/// Метод загружает веб-страницу по указанной ссылке
		/// </summary>
		/// <param name="PageLink">Ссылка на страницу</param>
		/// <returns>HTML-разметка страницы или пустая строка в случае ошибки</returns>
		public static string GetHTML (string PageLink)
			{
			// Настройка безопасности соединения
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xFC0;
			// Принудительно открывает TLS1.0, TLS1.1 и TLS1.2; блокирует SSL3

			// Запрос обновлений
			HttpWebRequest rq;
			try
				{
				rq = (HttpWebRequest)WebRequest.Create (PageLink);
				}
			catch
				{
				return "";
				}
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
				// Любая ошибка здесь будет означать необходимость прекращения проверки
				return "";
				}

			// Чтение ответа
			StreamReader SR = new StreamReader (resp.GetResponseStream (), true);
			html = SR.ReadToEnd ();
			SR.Close ();
			resp.Close ();

			// Завершено
			return html;
			}

		// Непринятие Политики
		private void MisacceptButton_Click (object sender, EventArgs e)
			{
			accepted = false;
			this.Close ();
			}
		}
	}
