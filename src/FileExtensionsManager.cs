using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс-описатель программы
	/// </summary>
	public static class FileExtensionsManagerProgram
		{
		/// <summary>
		/// Главная точка входа для приложения
		/// </summary>
		[STAThread]
		public static void Main ()
			{

			// Инициализация
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// Язык интерфейса и контроль XPR
			SupportedLanguages al = Localization.CurrentLanguage;
			if (!Localization.IsXPRClassAcceptable)
				return;

			// Проверка запуска единственной копии
			if (!RDGenerics.IsThisInstanceUnique (al == SupportedLanguages.ru_ru))
				return;

			// Отображение справки и запроса на принятие Политики
			if (!RDGenerics.AcceptEULA ())
				return;
			if (!RDGenerics.ShowAbout (true))
				ProgramDescription.RegisterAppExtensions ();

			// Запуск
			Application.Run (new FileExtensionsManagerForm ());
			}
		}
	}
