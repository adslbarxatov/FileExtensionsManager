using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает интерфейс отображения сведений о программе
	/// </summary>
	public partial class AboutForm: Form
		{
		// Переменные
		private string projectLink, updatesLink, userManualLink;
		private SupportedLanguages al;
		private string updatesMessage = "", description = "", policyLoaderCaption = "";

		private const string adpLink = "https://vk.com/@rdaaow_fupl-adp";           // Ссылка на Политику
		private const string defaultGitLink = "https://github.com/adslbarxatov/";   // Мастер-ссылка проекта
		private const string gitUpdatesSublink = "/releases";                       // Часть пути для перехода к релизам
		private const string devLink = "mailto://adslbarxatov@gmail.com";           // Разработчик
		private string versionDescription = "";

		private bool accepted = false;                                              // Флаг принятия Политики
		private string adpRevision = "";                                            // Последняя версия ADP

		/// <summary>
		/// Ключ реестра, хранящий версию, на которой отображалась справка
		/// </summary>
		public const string LastShownVersionKey = "HelpShownAt";

		// Ключ реестра, хранящий последнюю принятую версию ADP
		private const string ADPRevisionKey = "ADPRevision";
		private const string ADPRevisionPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\DPModule";

		/// <summary>
		/// Конструктор. Инициализирует форму
		/// </summary>
		/// <param name="UserManualLink">Ссылка на страницу руководства пользователя;
		/// кнопка отключается, если это значение не задано</param>
		/// <param name="AppIcon">Значок приложения</param>
		public AboutForm (string UserManualLink, Icon AppIcon)
			{
			// Инициализация
			InitializeComponent ();

			// Получение параметров
			userManualLink = (UserManualLink == null) ? "" : UserManualLink;

			projectLink = defaultGitLink + ProgramDescription.AssemblyMainName;
			updatesLink = defaultGitLink + ProgramDescription.AssemblyMainName + gitUpdatesSublink;

			// Загрузка окружения
			AboutLabel.Text = ProgramDescription.AssemblyTitle + "\n" + ProgramDescription.AssemblyDescription + "\n\n" +
				ProgramDescription.AssemblyCopyright + "\nv " + ProgramDescription.AssemblyVersion +
				"; " + ProgramDescription.AssemblyLastUpdate;

			if (AppIcon != null)
				{
				IconBox.BackgroundImage = AppIcon.ToBitmap ();
				OtherIconBox.BackgroundImage = this.Icon.ToBitmap ();
				}
			else
				{
				IconBox.BackgroundImage = this.Icon.ToBitmap ();
				}

			// Завершение
			UserManualButton.Enabled = (userManualLink != "");
			ProjectPageButton.Enabled = (projectLink != "");
			//UpdatesPageButton.Enabled = (updatesLink != "");
			}

		/// <summary>
		/// Метод отображает справочное окно приложения
		/// </summary>
		/// <param name="InterfaceLanguage">Язык интерфейса</param>
		/// <param name="Description">Описание программы и/или справочная информация</param>
		/// <param name="StartupMode">Флаг, указывающий, что справка не должна отображаться, если
		/// она уже была показана для данной версии приложения</param>
		/// <returns>Возвращает 1, если справка уже отображалась для данной версии (при StartupMode == true);
		/// Другое значение, если окно справки было отображено</returns>
		public int ShowAbout (SupportedLanguages InterfaceLanguage, string Description, bool StartupMode)
			{
			al = InterfaceLanguage;
			description = Description;

			return LaunchForm (StartupMode, false);
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
					// Исправлен некорректный порядок вызовов
					adpRevision = Registry.GetValue (ADPRevisionPath, ADPRevisionKey, "").ToString ();
					helpShownAt = Registry.GetValue (ProgramDescription.AssemblySettingsKey,
						LastShownVersionKey, "").ToString ();
					}
				catch
					{
					}

				// Если поле пустое, устанавливается минимальное значение
				if (adpRevision == "")
					{
					adpRevision = "rev. 8!";
					try
						{
						Registry.SetValue (ADPRevisionPath, ADPRevisionKey, adpRevision);
						}
					catch { }
					}
				}

			// Контроль
			if (StartupMode && (helpShownAt == ProgramDescription.AssemblyVersion) ||   // Справка уже отображалась
				AcceptMode && (!adpRevision.EndsWith ("!")))                            // Политика уже принята
				return 1;

			// Настройка контролов
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					UserManualButton.Text = "Руководство";
					ProjectPageButton.Text = "О проекте";
					UpdatesPageButton.Text = "Поиск обновлений...";
					ADPButton.Text = AcceptMode ? "Открыть в браузере" : "Политика и EULA";
					ExitButton.Text = AcceptMode ? "&Принять" : "&ОК";
					AskDeveloper.Text = "Разработчик";
					MisacceptButton.Text = "О&тклонить";
					DescriptionBox.Text = AcceptMode ? "Не удалось получить текст Политики. " +
						"Попробуйте использовать кнопку перехода в браузер" : description;
					policyLoaderCaption = "Подготовка к запуску...";

					this.Text = AcceptMode ? "Политика разработки и соглашение пользователя" : "О программе";
					break;

				default:    // en_us
					UserManualButton.Text = "User manual";
					ProjectPageButton.Text = "Project webpage";
					UpdatesPageButton.Text = "Checking updates...";
					ADPButton.Text = AcceptMode ? "Open in browser" : "Policy and EULA";
					ExitButton.Text = AcceptMode ? "&Accept" : "&OK";
					AskDeveloper.Text = "Ask developer";
					MisacceptButton.Text = "&Decline";
					DescriptionBox.Text = AcceptMode ? "Failed to get Policy text. Try button to open it in browser" :
						description;
					policyLoaderCaption = "Preparing for launch...";

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
					{
					DescriptionBox.Text = html;

					int left = html.IndexOf ("rev");
					int right = html.IndexOf ("\r", left);
					if ((left >= 0) && (right >= 0))
						adpRevision = html.Substring (left, right - left);
					}
				}

			// Настройка контролов
			UserManualButton.Visible = ProjectPageButton.Visible = UpdatesPageButton.Visible =
				AskDeveloper.Visible = !AcceptMode;
			MisacceptButton.Visible = AcceptMode;

			// Запуск
			this.ShowDialog ();

			// Запись версий по завершению
			try
				{
				if (StartupMode)
					Registry.SetValue (ProgramDescription.AssemblySettingsKey, LastShownVersionKey,
						ProgramDescription.AssemblyVersion);
				if (AcceptMode && accepted)
					Registry.SetValue (ADPRevisionPath, ADPRevisionKey, adpRevision.Replace ("!", ""));
				// В случае невозможности загрузки Политики признак необходимости принятия до этого момента
				// не удаляется из строки версии. Поэтому требуется страховка
				}
			catch { }

			// Завершение
			return accepted ? 0 : -1;
			}

		// Метод получает Политику разработки
		private void PolicyLoader (object sender, DoWorkEventArgs e)
			{
			string html = GetHTML (adpLink);
			int textLeft = 0, textRight = 0;
			string header = "";
			if (((textLeft = html.IndexOf ("<h1")) >= 0) && ((textRight = html.IndexOf ("</h1", textLeft)) >= 0))
				{
				// Обрезка
				header = html.Substring (textLeft, textRight - textLeft);
				}

			if (((textLeft = html.IndexOf ("<h3")) >= 0) && ((textRight = html.IndexOf ("<script", textLeft)) >= 0))
				{
				// Обрезка
				html = header + "\r\n\r\n" + html.Substring (textLeft, textRight - textLeft);

				// Формирование абзацных отступов
				html = html.Replace ("<br/>", "\r\n\r\n").Replace ("</p>", "\r\n\r\n").Replace ("</li>", "\r\n\r\n").
					Replace ("</h1>", "\r\n\r\n").Replace ("<h3", "\r\n<h3").Replace ("</h3>", "\r\n\r\n");

				// Удаление вложенных тегов
				while (((textLeft = html.IndexOf ("<")) >= 0) && ((textRight = html.IndexOf (">", textLeft)) >= 0))
					html = html.Replace (html.Substring (textLeft, textRight - textLeft + 1), "");

				// Удаление двойных пробелов
				while (html.IndexOf ("  ") >= 0)
					html = html.Replace ("  ", " ");
				html = html.Replace ("\n ", "");
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
			DescriptionBox.Width = this.Width - 31;
			ExitButton.Left = this.Width - 120;

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

		private void AskDeveloper_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (devLink + ("?subject=Wish, advice or bug in " +
					ProgramDescription.AssemblyTitle).Replace (" ", "%20"));
				}
			catch
				{
				}
			}

		// Метод-исполнитель проверки обновлений
		private string[][] ucReplacements = new string[][] {
			new string[] { "<p>", "\r\n\r\n" },
			new string[] { "<li>", "\r\n• " },
			new string[] { "</p>", "\r\n" },
			new string[] { "<br>", "\r\n" },

			new string[] { "</li>", "" },
			new string[] { "<ul>", "" },
			new string[] { "</ul>", "" },
			new string[] { "<em>", "" },
			new string[] { "</em>", "" },
			new string[] { "<code>", "" },
			new string[] { "</code>", "" }
			};

		private void UpdatesChecker (object sender, DoWorkEventArgs e)
			{
			// Запрос обновлений пакета
			string html = GetHTML (projectLink);

			// Разбор ответа (извлечение версии)
			string version = "";
			string[] htmlMarkers = { "</a>" + ProgramDescription.AssemblyMainName, "</h1>",
								   "markdown-body\">", "</div>" };

			int i = html.IndexOf (htmlMarkers[0]);
			if (i < 0)
				goto htmlError;

			i += htmlMarkers[0].Length;

			int j = html.IndexOf (htmlMarkers[1], i);
			if ((j < 0) || (j <= i))
				goto htmlError;

			version = html.Substring (i, j - i).Trim ();

			// Запрос описания пакета
			html = GetHTML (updatesLink);

			// Разбор ответа (извлечение версии)
			i = html.IndexOf (htmlMarkers[2]);
			if (i < 0)
				goto htmlError;

			i += htmlMarkers[2].Length;

			j = html.IndexOf (htmlMarkers[3], i);
			if ((j < 0) || (j <= i))
				goto htmlError;

			versionDescription = html.Substring (i, j - i);
			for (int r = 0; r < ucReplacements.Length; r++)
				versionDescription = versionDescription.Replace (ucReplacements[r][0], ucReplacements[r][1]);

			// Отображение результата
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					if (ProgramDescription.AssemblyTitle.EndsWith (version))
						updatesMessage = "Обновлений нет";
					else
						updatesMessage = "Доступна " + version;
					break;

				default:    // en_us
					if (ProgramDescription.AssemblyTitle.EndsWith (version))
						updatesMessage = "No updates";
					else
						updatesMessage = version + " available";
					break;
				}

			// Получение обновлений Политики (ошибки игнорируются)
			html = GetHTML (adpLink);
			if (((i = html.IndexOf ("<title")) >= 0) && ((j = html.IndexOf ("</title", i)) >= 0))
				{
				// Обрезка
				html = html.Substring (i, j - i);

				if ((i = html.IndexOf ("rev")) >= 0)
					{
					html = html.Substring (i);

					if (!html.StartsWith (adpRevision))
						{
						// Сброс версии для вызова Политики при следующем старте
						try
							{
							Registry.SetValue (ADPRevisionPath, ADPRevisionKey, html + "!");
							}
						catch { }
						}
					}
				}

			e.Result = 0;
			return;

// Есть проблема при загрузке страницы. Отмена
htmlError:
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					updatesMessage = "Недоступны";
					break;

				default:    // en_us
					updatesMessage = "Unavailable";
					break;
				}

			e.Result = -2;
			return;
			}

#if !SIMPLE_HWE
		/// <summary>
		/// Метод-исполнитель загрузки пакета обновлений
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void PackageLoader (object sender, DoWorkEventArgs e)
			{
			// Разбор аргументов
			string[] paths = (string[])e.Argument;

			// Настройка безопасности соединения
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xFC0;
			// Принудительно открывает TLS1.0, TLS1.1 и TLS1.2; блокирует SSL3

			// Запрос обновлений
			HttpWebRequest rq;
			try
				{
				rq = (HttpWebRequest)WebRequest.Create (paths[0]);
				}
			catch
				{
				e.Result = -1;
				return;
				}
			rq.Method = "GET";
			rq.KeepAlive = false;
			rq.Timeout = 10000;

			// Инициализация полосы загрузки
			SupportedLanguages al = Localization.CurrentLanguage;
			string report = Localization.GetText ("PackageDownload", al) + Path.GetFileName (paths[1]);
			((BackgroundWorker)sender).ReportProgress ((int)HardWorkExecutor.ProgressBarSize, report);

			// Отправка запроса
			HttpWebResponse resp = null;
			try
				{
				resp = (HttpWebResponse)rq.GetResponse ();
				}
			catch
				{
				// Любая ошибка здесь будет означать необходимость прекращения проверки
				e.Result = -2;
				return;
				}

			// Создание файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (paths[1], FileMode.Create);
				}
			catch
				{
				e.Result = -3;
				return;
				}

			// Чтение ответа
			Stream SR = resp.GetResponseStream ();

			int b;

			long length = 0, current = 0;
			try
				{
				if (paths[2].StartsWith ("0x"))
					length = long.Parse (paths[2].Substring (2), NumberStyles.AllowHexSpecifier);
				else
					length = long.Parse (paths[2]);
				}
			catch { }

			while ((b = SR.ReadByte ()) >= 0)
				{
				FS.WriteByte ((byte)b);

				if ((length != 0) && (current++ % 0x1000 == 0))
					((BackgroundWorker)sender).ReportProgress ((int)(HardWorkExecutor.ProgressBarSize * current / length),
						report);  // Возврат прогресса

				// Завершение работы, если получено требование от диалога
				if (((BackgroundWorker)sender).CancellationPending)
					{
					SR.Close ();
					FS.Close ();
					resp.Close ();

					e.Result = 1;
					e.Cancel = true;
					return;
					}
				}

			SR.Close ();
			FS.Close ();
			resp.Close ();

			// Завершено. Отображение сообщения
			((BackgroundWorker)sender).ReportProgress (0, Localization.GetText ("PackageSuccess", al));
			Thread.Sleep (1000);

			e.Result = 0;
			return;
			}
#endif

		// Контроль сообщения об обновлении
		private void UpdatesTimer_Tick (object sender, EventArgs e)
			{
			if (updatesMessage != "")
				{
				// Включение
				if (UpdatesPageButton.Text == "")
					{
					UpdatesPageButton.Text = updatesMessage;

					// Включение кнопки и установка интервала
					if (!UpdatesPageButton.Enabled)
						{
						if (updatesMessage.Contains ("."))
							{
							UpdatesTimer.Interval = 1000;
							UpdatesPageButton.Enabled = true;
							UpdatesPageButton.Font = new Font (UpdatesPageButton.Font, FontStyle.Bold);
							}

						// Отключение таймера, если обновлений нет
						else
							{
							UpdatesTimer.Enabled = false;
							}
						}
					}

				// Выключение
				else
					{
					UpdatesPageButton.Text = "";

					// Получение описания версии
					if (versionDescription != "")
						{
						DescriptionBox.Text += versionDescription;
						versionDescription = "";
						}
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
			try
				{
				html = SR.ReadToEnd ();
				}
			catch
				{
				html = "";  // Почему-то иногда исполнение обрывается на этом месте
				}
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

		/// <summary>
		/// Метод выполняет регистрацию указанного расширения файла и привязывает его к текущему приложению
		/// </summary>
		/// <param name="FileExtension">Расширение файла без точки</param>
		/// <param name="FileTypeName">Название типа файла</param>
		/// <param name="Openable">Флаг указывает, будет ли файл доступен для открытия в приложении</param>
		/// <param name="ShowWarning">Флаг указывает, что необходимо отобразить предупреждение перед регистрацией</param>
		/// <param name="FileIcon">Ресурс, хранящий значок формата файла</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public static bool RegisterFileExtension (string FileExtension, string FileTypeName, Icon FileIcon,
			bool Openable, bool ShowWarning)
			{
			// Подготовка
			string fileExt = FileExtension.ToLower ().Replace (".", "");

			// Контроль
			if (ShowWarning)
				{
				string msg = "Warning: required file extensions will be registered using current app location.\n\n" +
					"Make sure you will not change location of this application before using this feature.\n\n" +
					"Do you want to continue?";

				if (Localization.CurrentLanguage == SupportedLanguages.ru_ru)
					msg = "Предупреждение: необходимые расширения файлов будут зарегистрированы с использованием " +
						"текущего местоположения приложения.\n\nУбедитесь, что вы не будете менять расположение " +
						"этого приложения перед использованием этой функции.\n\nВы хотите продолжить?";

				if (MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.YesNo,
					MessageBoxIcon.Exclamation) == DialogResult.No)
					return false;
				}

			// Выполнение
			try
				{
				// Запись значка
				FileStream FS = new FileStream (fileExt + ".ico", FileMode.Create);
				FileIcon.Save (FS);
				FS.Close ();

				// Запись значений реестра
				Registry.SetValue ("HKEY_CLASSES_ROOT\\." + fileExt, "", fileExt + "file");
				Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file", "", FileTypeName);
				Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\DefaultIcon", "", AppStartupPath +
					fileExt + ".ico");

				if (Openable)
					{
					Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell", "", "open");
					Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell\\open", "Icon",
						AppStartupPath + fileExt + ".ico");
					Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell\\open\\command", "",
						"\"" + Application.ExecutablePath + "\" \"%1\"");
					}
				else
					{
					Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file", "NoOpen", "");
					}
				}
			catch
				{
				return false;
				}

			return true;
			}

		/// <summary>
		/// Возвращает путь, из которого запущен данный экземпляр приложения,
		/// с завершающим backslash
		/// </summary>
		public static string AppStartupPath
			{
			get
				{
				string s = Application.StartupPath; // В случае запуска из корня диска таки дорисовывает слэш
				if (s.EndsWith ("\\"))
					return s;

				return (s + "\\");
				}
			}
		}
	}
