using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
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
		private string updatesMessage = "", updatesMessageForText = "", description = "",
			policyLoaderCaption = "", registryFail = "",
			dpModuleAbsence = "", startDownload = "", packageFail = "", fileWriteFail = "";

		private string versionDescription = "";

		private bool accepted = false;                                              // Флаг принятия Политики
		private string adpRevision = "";                                            // Последняя версия ADP

		/// <summary>
		/// Ключ реестра, хранящий версию, на которой отображалась справка
		/// </summary>
		public const string LastShownVersionKey = "HelpShownAt";

		// Ключ реестра, хранящий последнюю принятую версию ADP
		private const string ADPRevisionKey = "ADPRevision";
		private const string ADPRevisionPath = RDGenerics.AssemblySettingsStorage + "DPModule";

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
			this.AcceptButton = ExitButton;
			this.CancelButton = MisacceptButton;

			// Получение параметров
			userManualLink = (UserManualLink == null) ? "" : UserManualLink;

			projectLink = RDGenerics.AssemblyGitLink + ProgramDescription.AssemblyMainName;
			updatesLink = RDGenerics.AssemblyGitLink + ProgramDescription.AssemblyMainName + RDGenerics.GitUpdatesSublink;

			// Загрузка окружения
			AboutLabel.Text = ProgramDescription.AssemblyTitle + "\n" + ProgramDescription.AssemblyDescription + "\n\n" +
				RDGenerics.AssemblyCopyright + "\nv " + ProgramDescription.AssemblyVersion +
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
			UserManualButton.Enabled = !string.IsNullOrWhiteSpace (userManualLink);
			ProjectPageButton.Enabled = !string.IsNullOrWhiteSpace (projectLink);
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
					helpShownAt = Registry.GetValue (RDGenerics.AssemblySettingsKey,
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
					UserManualButton.Text = "&Руководство";
					ProjectPageButton.Text = "&О проекте";
					UpdatesPageButton.Text = "Поиск обновлений...";
					ADPButton.Text = AcceptMode ? "Открыть в &браузере" : "&Политика и EULA";
					ExitButton.Text = AcceptMode ? "&Принять" : "&ОК";
					AskDeveloper.Text = "Спросить ра&зработчика";
					MisacceptButton.Text = "О&тклонить";

					if (!desciptionHasBeenUpdated)
						DescriptionBox.Text = AcceptMode ? "Не удалось получить текст Политики. " +
						"Попробуйте использовать кнопку перехода в браузер" :
						"[Проверка обновлений...]\r\n\r\n" + description;

					policyLoaderCaption = "Подготовка к запуску...";
					registryFail = ProgramDescription.AssemblyMainName +
						" не может сохранить настройки в реестре Windows. Оно не будет работать корректно.\n\n" +
						"Попробуйте выполнить следующие изменения в свойствах исполняемого файла:\n" +
						"• разблокируйте приложение в общих свойствах (кнопка «Разблокировать»);\n" +
						"• включите запуск от имени администратора для всех пользователей в настройках совместимости.\n\n" +
						"После этого перезапустите программу и повторите попытку";

					dpModuleAbsence = "Инструмент развёртки пакетов DPModule не найден на этом ПК. Перейти к его загрузке?" +
						"\n\nВы можете обновить этот продукт прямо из DPModule или вернуться сюда после его установки. " +
						"Также Вы можете ознакомиться с презентацией DPModule на YouTube, нажав кнопку «Нет»";
					packageFail = "Не удалось загрузить пакет развёртки. Проверьте Ваше подключение к Интернету";
					fileWriteFail = "Не удалось сохранить пакет развёртки. Проверьте Ваши права доступа";
					startDownload = "Начать загрузку пакета?\n\nПакет развёртки будет сохранён на Рабочем столе";

					this.Text = AcceptMode ? "Политика разработки и соглашение пользователя" : "О приложении";
					break;

				default:    // en_us
					UserManualButton.Text = "&User manual";
					ProjectPageButton.Text = "Project’s &webpage";
					UpdatesPageButton.Text = "Checking updates...";
					ADPButton.Text = AcceptMode ? "Open in &browser" : "&Policy and EULA";
					ExitButton.Text = AcceptMode ? "&Accept" : "&OK";
					AskDeveloper.Text = "Ask &developer";
					MisacceptButton.Text = "&Decline";

					if (!desciptionHasBeenUpdated)
						DescriptionBox.Text = AcceptMode ? "Failed to get Policy text. Try button to open it in browser" :
							"[Checking for updates...]\r\n\r\n" + description;

					policyLoaderCaption = "Preparing for launch...";
					registryFail = ProgramDescription.AssemblyMainName +
						" cannot save settings in the Windows registry. It will not work properly.\n\n" +
						"Try the following changes to properties of the executable file:\n" +
						"• unblock the app in general properties (“Unblock” button);\n" +
						"• enable running as administrator for all users in compatibility settings.\n\n" +
						"Then restart the program and try again";

					dpModuleAbsence = "DPModule, the packages deployment tool isn’t installed on this PC. " +
						"Download it?\n\nYou can update this product directly from DPModule or come back here " +
						"after installing it. Also you can view the DPModule presentation on YouTube by pressing “No” button";
					packageFail = "Failed to download deployment package. Check your internet connection";
					fileWriteFail = "Failed to save deployment package. Check your user access rights";
					startDownload = "Download the package?\n\nThe deployment package will be saved on the Desktop";

					this.Text = AcceptMode ? "Development policy and user agreement" : "About the application";
					break;
				}
			ToLaboratory.Text = RDGenerics.AssemblyCompany;

			// Запуск проверки обновлений
			HardWorkExecutor hwe;
			if (!AcceptMode)
				{
#if DPMODULE
				hwe = new HardWorkExecutor (UpdatesChecker, null, null, false, false, false);
#else
				hwe = new HardWorkExecutor (UpdatesChecker, null, null, false, false);
#endif
				UpdatesTimer.Enabled = true;
				}

			// Получение Политики
			else
				{
#if DPMODULE
				hwe = new HardWorkExecutor (PolicyLoader, null, policyLoaderCaption, true, false, true);
#else
				hwe = new HardWorkExecutor (PolicyLoader, null, policyLoaderCaption, true, false);
#endif

				string html = hwe.Result.ToString ();
				if (!string.IsNullOrWhiteSpace (html))
					{
					DescriptionBox.Text = html;

					int left = html.IndexOf ("rev");
					int right = html.IndexOf ("\n", left);
					if ((left >= 0) && (right >= 0))
						adpRevision = html.Substring (left, right - left);
					}
				}

			// Настройка контролов
			UserManualButton.Visible = ProjectPageButton.Visible = UpdatesPageButton.Visible =
				AskDeveloper.Visible = ToLaboratory.Visible = !AcceptMode;
#if DPMODULE
			UpdatesPageButton.Visible = false;
#endif
			MisacceptButton.Visible = AcceptMode;

			// Запуск
			this.ShowDialog ();

			// Запись версий по завершению
			try
				{
				if (StartupMode)
					{
					Registry.SetValue (RDGenerics.AssemblySettingsKey, LastShownVersionKey,
						ProgramDescription.AssemblyVersion);

					// Контроль доступа к реестру
					WindowsIdentity identity = WindowsIdentity.GetCurrent ();
					WindowsPrincipal principal = new WindowsPrincipal (identity);
					if (!principal.IsInRole (WindowsBuiltInRole.Administrator))
						{
						MessageBox.Show (registryFail, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation);
						}
					}

				// В случае невозможности загрузки Политики признак необходимости принятия до этого момента
				// не удаляется из строки версии. Поэтому требуется страховка
				if (AcceptMode && accepted)
					Registry.SetValue (ADPRevisionPath, ADPRevisionKey, adpRevision.Replace ("!", ""));
				}
			catch
				{
				MessageBox.Show (registryFail, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
				}

			// Завершение
			return accepted ? 0 : -1;
			}

		// Метод получает Политику разработки
		private void PolicyLoader (object sender, DoWorkEventArgs e)
			{
			string html = GetHTML (RDGenerics.ADPLink);
			int textLeft = 0, textRight = 0;

			if (((textLeft = html.IndexOf ("code\">")) >= 0) &&
				((textRight = html.IndexOf ("id=\"changes-log---", textLeft)) >= 0))
				{
				// Обрезка
				textLeft += 6;
				html = html.Substring (textLeft, textRight - textLeft);

				// Формирование
				html = ApplyReplacements (html);
				html = html.Substring (0, html.Length - 12);
				}
			else
				{
				e.Result = "";
				return;
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
				if (string.IsNullOrWhiteSpace (Link))
					Process.Start (RDGenerics.AssemblyGitLink + ProgramDescription.AssemblyMainName +
						RDGenerics.GitUpdatesSublink);
				else
					Process.Start (Link);
				}
			catch { }
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
			catch { }
			}

		private void ProjectPageButton_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (projectLink);
				}
			catch { }
			}

		private void ADP_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (RDGenerics.ADPLink);
				}
			catch { }
			}

		private void ToLaboratory_Click (object sender, EventArgs e)
			{
			string msg;
			if (al == SupportedLanguages.ru_ru)
				msg = "• «Да» – перейти на страницу Лаборатории в ВКонтакте;\n" +
					"• «Нет» – перейти на начальную страницу Лаборатории";
			else
				msg = "• “Yes” – go to Laboratory’s page in Telegram;\n" +
					"• “No” – go to Laboratory’s welcome page";

			string link;
			switch (MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question))
				{
				case DialogResult.Yes:
					link = (al == SupportedLanguages.ru_ru) ? RDGenerics.LabVKLink : RDGenerics.LabTGLink;
					break;

				case DialogResult.No:
					link = RDGenerics.DPModuleLink;
					break;

				default:
					return;
				}

			try
				{
				Process.Start (link);
				}
			catch { }
			}

		private void AskDeveloper_Click (object sender, EventArgs e)
			{
			try
				{
				Process.Start (RDGenerics.LabMailLink + ("?subject=Wish, advice or bug in " +
					ProgramDescription.AssemblyTitle).Replace (" ", "%20"));
				}
			catch { }
			}

		// Загрузка пакета обновления изнутри приложения
		private void UpdatesPageButton_Click (object sender, EventArgs e)
			{
			// Контроль наличия DPModule
			string dpmv = "";
			try
				{
				dpmv = Registry.GetValue (RDGenerics.AssemblySettingsStorage + "DPModule",
					LastShownVersionKey, "").ToString ();
				}
			catch { }

			string downloadLink = RDGenerics.DPModuleStorageLink;
			string packagePath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + "\\";
			if (string.IsNullOrWhiteSpace (dpmv))
				{
				// Выбор варианта обработки
				switch (MessageBox.Show (dpModuleAbsence, ProgramDescription.AssemblyTitle,
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
					{
					case DialogResult.Cancel:
						return;

					case DialogResult.No:
						try
							{
							Process.Start (RDGenerics.DPModuleUserManualLink);
							}
						catch { }
						return;
					}

				downloadLink = RDGenerics.DPModuleDirectLink;
				packagePath += "DPModule.sfx.exe";
				}
			else
				{
				if (MessageBox.Show (startDownload, ProgramDescription.AssemblyTitle,
					MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					return;

				downloadLink += ("/raw/master/Packages/" + ProgramDescription.AssemblyMainName + ".dp");
				packagePath += (ProgramDescription.AssemblyMainName + ".dp");
				}

			// Запуск загрузки
			HardWorkExecutor hwe = new HardWorkExecutor (PackageLoader, downloadLink, packagePath, "0");

			// Разбор ответа
			string msg = "";
			switch (hwe.ExecutionResult)
				{
				case 0:
					break;

				case -1:
				case -2:
					msg = packageFail;
					break;

				case -3:
					msg = fileWriteFail;
					break;

				default: // Отмена
					try
						{
						File.Delete (packagePath);
						}
					catch { }
					return;
				}

			if (!string.IsNullOrWhiteSpace (msg))
				{
				MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Запуск пакета
			try
				{
				Process.Start (packagePath);
				}
			catch { }
			}

		// Метод-исполнитель проверки обновлений
		private static string[][] htmlReplacements = new string[][] {
			new string[] { "<p", "\r\n<" },
			new string[] { "<li>", "\r\n• " },
			new string[] { "</p>", "\r\n" },
			new string[] { "<br", "\r\n<" },

			new string[] { "<h1", "\r\n<" },
			new string[] { "</h1>", "\r\n" },
			new string[] { "<h3", "\r\n<" },

			new string[] { "&gt;", "›" },
			new string[] { "&lt;", "‹" },
			new string[] { "&#39;", "’" }
			};

		/// <summary>
		/// Метод выполняет пост-обработку текста лога или политики после загрузки
		/// </summary>
		/// <param name="Source">Исходный текст</param>
		/// <returns>Текст с применёнными заменами символов форматирования</returns>
		public static string ApplyReplacements (string Source)
			{
			string res = Source;

			// Замена элементов разметки
			for (int i = 0; i < htmlReplacements.Length; i++)
				res = res.Replace (htmlReplacements[i][0], htmlReplacements[i][1]);

			// Удаление вложенных тегов
			int textLeft = 0, textRight = 0;
			while (((textLeft = res.IndexOf ("<")) >= 0) && ((textRight = res.IndexOf (">", textLeft)) >= 0))
				res = res.Replace (res.Substring (textLeft, textRight - textLeft + 1), "");

			return res;
			}

		/// <summary>
		/// Левый маркер лога изменений
		/// </summary>
		public const string ChangeLogMarkerLeft = "markdown-body my-3\">";

		/// <summary>
		/// Правый маркер лога изменений
		/// </summary>
		public const string ChangeLogMarkerRight = "</div>";

		private void UpdatesChecker (object sender, DoWorkEventArgs e)
			{
			// Запрос обновлений пакета
			string html = GetHTML (projectLink);
			bool htmlError = true;  // Сбрасывается при успешной загрузке

			// Разбор ответа (извлечение версии)
			string[] htmlMarkers = { "</a>" + ProgramDescription.AssemblyMainName, "</h1>",
								   ChangeLogMarkerLeft, ChangeLogMarkerRight };

			int i = html.IndexOf (htmlMarkers[0]);
			if (i < 0)
				goto policy;

			i += htmlMarkers[0].Length;

			int j = html.IndexOf (htmlMarkers[1], i);
			if ((j < 0) || (j <= i))
				goto policy;

			string version = html.Substring (i, j - i).Trim ();

			// Запрос описания пакета
			html = GetHTML (updatesLink);

			// Разбор ответа (извлечение версии)
			i = html.IndexOf (htmlMarkers[2]);
			if (i < 0)
				goto policy;

			i += htmlMarkers[2].Length;

			j = html.IndexOf (htmlMarkers[3], i);
			if ((j < 0) || (j <= i))
				goto policy;

			versionDescription = html.Substring (i, j - i);
			versionDescription = "\r\n" + ApplyReplacements (versionDescription);

			// Отображение результата
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					if (ProgramDescription.AssemblyTitle.EndsWith (version))
						{
						updatesMessage = "Версия актуальна";
						updatesMessageForText = "[Версия актуальна, см. описание в конце]";
						}
					else
						{
						updatesMessage = "&Доступна " + version;
						updatesMessageForText = "[Доступна " + version + ", см. описание в конце]";
						}
					break;

				default:    // en_us
					if (ProgramDescription.AssemblyTitle.EndsWith (version))
						{
						updatesMessage = "App is up-to-date";
						updatesMessageForText = "[Version is up to date, see description below]";
						}
					else
						{
						updatesMessage = version + " a&vailable";
						updatesMessageForText = "[" + version + " is available, see description below]";
						}
					break;
				}
			htmlError = false;

// Получение обновлений Политики (ошибки игнорируются)
policy:
			html = GetHTML (RDGenerics.ADPLink);
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

			// Не было проблем с загрузкой страницы
			if (!htmlError)
				{
				e.Result = 0;
				return;
				}

			// Есть проблема при загрузке страницы. Отмена
			switch (al)
				{
				case SupportedLanguages.ru_ru:
					updatesMessage = "Недоступны";
					updatesMessageForText = "[Страница обновлений недоступна]";
					break;

				default:    // en_us
					updatesMessage = "Unavailable";
					updatesMessageForText = "[Updates page is unavailable]";
					break;
				}

			e.Result = -2;
			return;
			}

		/// <summary>
		/// Метод-исполнитель загрузки пакета обновлений
		/// </summary>
		public static void PackageLoader (object sender, DoWorkEventArgs e)
			{
			// Разбор аргументов
			string[] paths = (string[])e.Argument;

			// Инициализация полосы загрузки
			SupportedLanguages al = Localization.CurrentLanguage;
			string downloadMessage = "Downloading deployment package:",
				downloadSuccess = "Success";
			if (al == SupportedLanguages.ru_ru)
				{
				downloadMessage = "Загрузка установочного пакета:";
				downloadSuccess = "Успешно";
				}

			string report = downloadMessage + "\n" + Path.GetFileName (paths[1]);
			((BackgroundWorker)sender).ReportProgress ((int)HardWorkExecutor.ProgressBarSize, report);

			// Отдельная обработка ModDB
			if (paths[0].Contains ("www.moddb.com"))
				{
				string html = GetHTML (paths[0]);

				int left, right;
				if ((html == "") || ((left = html.IndexOf ("<a href=\"")) < 0) ||
					((right = html.IndexOf ("/?", left)) < 0))
					{
					e.Result = -1;
					return;
					}

				paths[0] = "https://www.moddb.com" + html.Substring (left + 9, right - left - 9);
				}

			// Настройка безопасности соединения
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xFC0;
			// Принудительно открывает TLS1.0, TLS1.1 и TLS1.2; блокирует SSL3

			// Запрос файла
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
				resp.Close ();
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
			((BackgroundWorker)sender).ReportProgress (-1, downloadSuccess);
			Thread.Sleep (500);

			e.Result = 0;
			return;
			}

		// Контроль сообщения об обновлении
		private bool desciptionHasBeenUpdated = false;
		private void UpdatesTimer_Tick (object sender, EventArgs e)
			{
			if (string.IsNullOrWhiteSpace (updatesMessage))
				return;

			// Получение описания версии
			if (!string.IsNullOrWhiteSpace (versionDescription))
				{
				description += versionDescription;
				versionDescription = "";
				}

			// Обновление состояния
			if (!desciptionHasBeenUpdated)
				{
				DescriptionBox.Text = updatesMessageForText + "\r\n\r\n" + description;
				desciptionHasBeenUpdated = true;
				}

			// Включение текста кнопки
			if (string.IsNullOrWhiteSpace (UpdatesPageButton.Text))
				{
				UpdatesPageButton.Text = updatesMessage;

				// Включение кнопки и установка интервала
				if (!UpdatesPageButton.Enabled)
					{
					// Исключение задвоения
					if (updatesMessage.Contains (" "))    // Интернет доступен
						{
						UpdatesTimer.Interval = 1000;
						UpdatesPageButton.Enabled = true;

						if (updatesMessage.Contains ("."))
							UpdatesPageButton.Font = new Font (UpdatesPageButton.Font, FontStyle.Bold);
						else
							UpdatesTimer.Enabled = false;
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
			string html = "";
			try
				{
				rq = (HttpWebRequest)WebRequest.Create (PageLink);
				}
			catch
				{
				return html;
				}
			rq.Method = "GET";
			rq.KeepAlive = false;
			rq.Timeout = 10000;

			// Отправка запроса
			HttpWebResponse resp = null;
			try
				{
				resp = (HttpWebResponse)rq.GetResponse ();
				}
			catch //(Exception e)
				{
				// Любая ошибка здесь будет означать необходимость прекращения проверки
				return html;
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
			UpdatesTimer.Enabled = false;
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
				Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\DefaultIcon", "", RDGenerics.AppStartupPath +
					fileExt + ".ico");

				if (Openable)
					{
					Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell", "", "open");
					Registry.SetValue ("HKEY_CLASSES_ROOT\\" + fileExt + "file\\shell\\open", "Icon",
						RDGenerics.AppStartupPath + fileExt + ".ico");
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
		}
	}
