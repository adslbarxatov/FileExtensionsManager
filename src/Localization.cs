using System.Globalization;
#if !ANDROID
	using System.Windows.Forms;
#endif

namespace RD_AAOW
	{
	/// <summary>
	/// Поддерживаемые языки приложения
	/// </summary>
	public enum SupportedLanguages
		{
		/// <summary>
		/// Русский
		/// </summary>
		ru_ru,

		/// <summary>
		/// Английский (США)
		/// </summary>
		en_us
		}

	/// <summary>
	/// Класс обеспечивает доступ к языковым настройкам приложения
	/// </summary>
	public static class Localization
		{
		/// <summary>
		/// Название параметра, хранящего текущий язык интерфейса
		/// </summary>
		public const string LanguageValueName = "Language";

		/// <summary>
		/// Возвращает список доступных языков интерфейса
		/// </summary>
		public static string[] LanguagesNames
			{
			get
				{
				return languagesNames;
				}
			}
		private static string[] languagesNames = new string[] { "Русский", "English (US)" };

		/// <summary>
		/// Возвращает или задаёт текущий язык интерфейса приложения
		/// </summary>
		public static SupportedLanguages CurrentLanguage
			{
			// Запрос
			get
				{
				// Получение значения (один раз за запуск приложения)
				if (currentLanguage == "")
					currentLanguage = GetCurrentLanguage ();

				// При пустом значении пробуем получить язык от системы
				if (currentLanguage == "")
					{
					CultureInfo ci = CultureInfo.InstalledUICulture;

					switch (ci.ToString ().ToLower ())
						{
						case "ru-ru":
							currentLanguage = SupportedLanguages.ru_ru.ToString ();
							return SupportedLanguages.ru_ru;

						default:
							currentLanguage = SupportedLanguages.en_us.ToString ();
							return SupportedLanguages.en_us;
						}
					}

				// Определение
				switch (currentLanguage)
					{
					// Уже задан
					case "ru_ru":
						return SupportedLanguages.ru_ru;

					// Состояние неизвестно
					default:
						return SupportedLanguages.en_us;
					}
				}

			// Установка
			set
				{
				currentLanguage = value.ToString ();
				RDGenerics.SetAppSettingsValue (LanguageValueName, currentLanguage);
				}
			}
		private static string currentLanguage = "";

		/// <summary>
		/// Возвращает факт предыдущей установки языка приложения
		/// </summary>
		public static bool IsCurrentLanguageSpecified
			{
			get
				{
				return (GetCurrentLanguage () != "");
				}
			}

		// Метод запрашивает настройку из реестра
		private static string GetCurrentLanguage ()
			{
			return RDGenerics.GetAppSettingsValue (LanguageValueName);
			}

		/// <summary>
		/// Метод возвращает локализованный текст по указанному идентификатору
		/// </summary>
		/// <param name="TextName">Идентификатор текстового фрагмента</param>
		/// <param name="Language">Требуемый язык локализации</param>
		/// <returns>Локализованный текстовый фрагмент</returns>
		public static string GetText (string TextName, SupportedLanguages Language)
			{
			switch (Language)
				{
				default:
					return ProgramDescription.AssemblyLocalizationRMs[0].GetString (TextName);

				case SupportedLanguages.ru_ru:
					return ProgramDescription.AssemblyLocalizationRMs[1].GetString (TextName);
				}
			}

#region Extended

#if !ANDROID

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа Form</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Form Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа TabPage</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (TabPage Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех пунктов, входящих в состав 
		/// указанного меню (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа MenuStrip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (MenuStrip Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Items.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Items[i].Name, Language) != null)
					{
					Container.Items[i].Text = GetControlText (Container.Name, Container.Items[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех пунктов, входящих в состав 
		/// указанного меню (Text и ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа ToolStripMenuItem</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (ToolStripMenuItem Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.DropDownItems.Count; i++)
				{
				if (GetControlText (Container.Name, Container.DropDownItems[i].Name, Language) != null)
					{
					Container.DropDownItems[i].Text = GetControlText (Container.Name,
						Container.DropDownItems[i].Name, Language);
					}
				if (GetControlText (Container.Name, Container.DropDownItems[i].Name + "_TT", Language) != null)
					{
					Container.DropDownItems[i].ToolTipText = GetControlText (Container.Name,
						Container.DropDownItems[i].Name + "_TT", Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа TabControl</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (TabControl Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.TabPages.Count; i++)
				{
				if (GetControlText (Container.Name, Container.TabPages[i].Name + "_TT", Language) != null)
					{
					Container.TabPages[i].ToolTipText = GetControlText (Container.Name,
						Container.TabPages[i].Name + "_TT", Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа Form</param>
		/// <param name="TextContainer">Контейнер подписей типа ToolTip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Form Container, ToolTip TextContainer, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name + "_TT", Language) != null)
					{
					TextContainer.SetToolTip (Container.Controls[i], GetControlText (Container.Name,
						Container.Controls[i].Name + "_TT", Language));
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа TabPage</param>
		/// <param name="TextContainer">Контейнер подписей типа ToolTip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (TabPage Container, ToolTip TextContainer, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name + "_TT", Language) != null)
					{
					TextContainer.SetToolTip (Container.Controls[i], GetControlText (Container.Name,
						Container.Controls[i].Name + "_TT", Language));
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (Text и ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа Panel</param>
		/// <param name="TextContainer">Контейнер подписей типа ToolTip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Panel Container, ToolTip TextContainer, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}

				if (GetControlText (Container.Name, Container.Controls[i].Name + "_TT", Language) != null)
					{
					TextContainer.SetToolTip (Container.Controls[i], GetControlText (Container.Name,
						Container.Controls[i].Name + "_TT", Language));
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа Panel</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Panel Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав 
		/// указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа GroupBox</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (GroupBox Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

#endif

		/// <summary>
		/// Метод возвращает локализованную подпись указанного контрола
		/// </summary>
		/// <param name="FormName">Имя формы, содержащей контрол</param>
		/// <param name="ControlName">Имя контрола</param>
		/// <param name="Language">Требуемый язык локализации</param>
		/// <returns>Подпись или текст контрола</returns>
		public static string GetControlText (string FormName, string ControlName, SupportedLanguages Language)
			{
			return GetText (FormName + "_" + ControlName, Language);
			}

#endregion
		}
	}
