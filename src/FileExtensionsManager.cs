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

			// Отображение справки и запроса на принятие Политики
			if (!ProgramDescription.AcceptEULA ())
				return;
			ProgramDescription.ShowAbout (true);

			// Запуск
			Application.Run (new FileExtensionsManagerForm ());
			}
		}
	}
