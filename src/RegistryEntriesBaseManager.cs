using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает интерфейс базы управляемых реестровых записей
	/// </summary>
	public class RegistryEntriesBaseManager
		{
		// База реестровых записей
		private List<RegistryEntry> entries = new List<RegistryEntry> ();

		// Кодировка старого формата базы записей
		private Encoding oldBaseFileEncoding = Encoding.GetEncoding (1251);

		// Сплиттер параметра и значения в файле реестра
		private string registryFileSplitter = "=";

		// Сплиттер записей в базе
		private char[] baseFileSplitters = new char[] { '\x1' };

		// Флаг указывает, что в базу были внесены изменения
		private bool changed = true;

		// Список представлений записей в базе
		private List<string> ebp = new List<string> ();
		private List<RegistryEntryApplicationResults> esp = new List<RegistryEntryApplicationResults> ();

		// Файловые дескрипторы
		private FileStream FS = null;
		private StreamReader SR = null;
		private StreamWriter SW = null;

		// Имя файла новой базы реестровых записей
		private const string newBaseFileName = "New base";

		/// <summary>
		/// Старое расширение имени файла базы реестровых записей
		/// </summary>
		public const string OldBaseFileExtension = ".reb";

		/// <summary>
		/// Новое расширение имени файла базы реестровых записей
		/// </summary>
		public const string NewBaseFileExtension = ".reu";

		/// <summary>
		/// Субдиректория для хранения сохранённых баз реестровых записей
		/// </summary>
		public const string BasesSubdirectory = "REBases";

		/// <summary>
		/// Конструктор. Загружает базу реестровых записей
		/// </summary>
		/// <param name="BaseName">Имя создаваемой базы</param>
		public RegistryEntriesBaseManager (string BaseName)
			{
			// Сохранение параметров
			baseName = BaseName;

			// Загрузка базы
			if (LoadBase () != 0)   // Если загрузка завершается с ошибкой
				{
				if (!SaveBase ())   // Пробуем создать / пересохранить базу
					return;         // Если не получается, прерываем загрузку
				}

			isInited = true;
			}

		/// <summary>
		/// Конструктор. Создаёт пустую базу реестровых записей
		/// </summary>
		public RegistryEntriesBaseManager ()
			{
			// Сохранение параметров
			baseName = newBaseFileName;

			// Загрузка базы
			if (!SaveBase ())   // Пробуем создать базу
				return;         // Если не получается, прерываем загрузку

			isInited = true;
			}

		// Метод загружает базу
		// Возвращает 0 в случае успеха, -1 в случае ошибки, 1 при необходимости пересохранить базу
		private int LoadBase ()
			{
			// Попытка открытия старого файла
			bool old = false;
			try
				{
				FS = new FileStream (RDGenerics.AppStartupPath + BasesSubdirectory + "\\"
					+ baseName + OldBaseFileExtension, FileMode.Open);
				SR = new StreamReader (FS, oldBaseFileEncoding);
				old = true;
				}
			catch
				{
				try
					{
					FS = new FileStream (RDGenerics.AppStartupPath + BasesSubdirectory + "\\"
						+ baseName + NewBaseFileExtension, FileMode.Open);
					SR = new StreamReader (FS, RDGenerics.GetEncoding (RDEncodings.UTF8));
					}
				catch
					{
					return -1;
					}
				}

			// Чтение файла
			SR.ReadLine ();     // Версия

			while (!SR.EndOfStream)
				{
				string s = SR.ReadLine ();
				string[] values = s.Split (baseFileSplitters);
				// Пустые поля не удалять, т.к. они отвечают за значения по умолчанию

				if (values.Length != 6)
					continue;   // Если вдруг попадётся битая запись, пропустить её

				RegistryValueTypes valueType = RegistryValueTypes.REG_SZ;
				try
					{
					valueType = (RegistryValueTypes)uint.Parse (values[3]);
					}
				catch { }

				RegistryEntry re = new RegistryEntry (values[0], values[1], values[2], valueType,
					values[4] != "0", values[5] != "0");
				if (!re.IsValid)
					continue;   // Аналогично

				// В противном случае добавляем запись
				entries.Add (re);
				}

			// Завершено
			SR.Close ();
			FS.Close ();

			// Постобработка
			if (old)
				{
				try
					{
					File.Move (RDGenerics.AppStartupPath + BasesSubdirectory + "\\"
						+ baseName + OldBaseFileExtension,
						RDGenerics.AppStartupPath + BasesSubdirectory + "\\"
						+ baseName + ".bak");
					}
				catch { }
				}

			return (old ? 1 : 0);
			}

		/// <summary>
		/// Метод выполняет сохранение базы в файл
		/// </summary>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool SaveBase ()
			{
			// Постсортировка
			if (changed)
				entries.Sort ();

			// Контроль наличия субдиректории
			if (!Directory.Exists (BasesSubdirectory))
				{
				try
					{
					Directory.CreateDirectory (BasesSubdirectory);
					}
				catch
					{
					return false;
					}
				}

			// Попытка открытия базы
			try
				{
				FS = new FileStream (RDGenerics.AppStartupPath + BasesSubdirectory + "\\" +
					baseName + NewBaseFileExtension, FileMode.Create);
				// Перезаписывает недоступный файл при необходимости
				}
			catch
				{
				return false;
				}
			SW = new StreamWriter (FS, RDGenerics.GetEncoding (RDEncodings.UTF8));

			// Запись
			SW.Write (ProgramDescription.AssemblyTitle + "; timestamp: " +
				DateTime.Now.ToString ("dd.MM.yyyy, HH:mm") + RDLocale.RN);

			for (int i = 0; i < entries.Count; i++)
				{
				SW.Write (entries[i].ValuePath + baseFileSplitters[0].ToString () +
					entries[i].ValueName + baseFileSplitters[0].ToString () +
					entries[i].ValueObject + baseFileSplitters[0].ToString () +
					((uint)(entries[i].ValueType)).ToString () + baseFileSplitters[0].ToString () +
					(entries[i].PathMustBeDeleted ? "1" : "0") + baseFileSplitters[0].ToString () +
					(entries[i].NameMustBeDeleted ? "1" : "0") + RDLocale.RN);
				}

			// Завершено
			SW.Close ();
			FS.Close ();
			return true;
			}

		/// <summary>
		/// Возвращает статус инициализации класса
		/// </summary>
		public bool IsInited
			{
			get
				{
				return isInited;
				}
			}
		private bool isInited = false;

		/// <summary>
		/// Имя базы реестровых записей
		/// </summary>
		public string BaseName
			{
			get
				{
				return baseName;
				}
			}
		private string baseName = "";

		/// <summary>
		/// Возвращает представление списка записей базы
		/// </summary>
		public List<string> EntriesBasePresentation
			{
			get
				{
				if (isInited && changed)    // Выполнять обновление, только если в базу вносились изменения
					{
					// Сортировка
					entries.Sort ();

					// Сборка списка
					ebp.Clear ();
					for (int i = 0; i < entries.Count; i++)
						{
						ebp.Add ("[" + entries[i].ValuePath + "]: " + ((entries[i].ValueName == "") ? "@" :
							entries[i].ValueName) + " = " +
							entries[i].ValueObject +
							((entries[i].ValueType != RegistryValueTypes.REG_SZ) ? " (" +
							entries[i].ValueType.ToString () + ")" : "") +
							(entries[i].PathMustBeDeleted ? "; -RB-" : "") +
							(entries[i].NameMustBeDeleted ? "; -RV-" : ""));
						}

					changed = false;
					}

				return ebp;
				}
			}

		/// <summary>
		/// Возвращает представление списка статусов записей базы
		/// </summary>
		public List<RegistryEntryApplicationResults> EntriesStatusesPresentation
			{
			get
				{
				if (isInited)   // Выполнять обновление всегда, когда возможно, и без сортировки
					{
					// Сборка списка
					esp.Clear ();
					for (int i = 0; i < entries.Count; i++)
						esp.Add (entries[i].GetEntryApplicationState ());
					}

				return esp;
				}
			}

		/// <summary>
		/// Метод возвращает ссылку на реестровую запись для редактирования
		/// </summary>
		/// <param name="EntryNumber">Номер записи в списке</param>
		/// <returns>Реестровая запись</returns>
		public RegistryEntry GetRegistryEntry (uint EntryNumber)
			{
			// Если параметр некорректен, вернуть некорректную запись
			if (!isInited || (EntryNumber > entries.Count))
				return new RegistryEntry ("", "", "");

			return (RegistryEntry)entries[(int)EntryNumber].Clone ();   // Защита от неотслеживаемого доступа
			}

		/// <summary>
		/// Возвращает количество записей в базе
		/// </summary>
		public uint EntriesCount
			{
			get
				{
				return (uint)entries.Count;
				}
			}

		/// <summary>
		/// Метод загружает файл реестра в базу
		/// </summary>
		/// <param name="FileName">Путь к файлу для загрузки</param>
		public uint LoadRegistryFile (string FileName)
			{
			// Контроль
			if (!isInited)
				return 0;

			// Попытка открытия файла
			try
				{
				FS = new FileStream (FileName, FileMode.Open);
				}
			catch
				{
				return 0;
				}
			SR = new StreamReader (FS, RDGenerics.GetEncoding (RDEncodings.Unicode16));

			// Контроль состава файла
			string s = SR.ReadLine ().ToLower (), s2;
			if (!s.Contains ("regedit") && !s.Contains ("windows registry"))
				// Заодно исключает возможные проблемы с кодировкой
				return 0;

			// Начало чтения
			uint entriesCounter = 0;
			string currentPath = "";
			while (!SR.EndOfStream)
				{
				s = SR.ReadLine ();
				if (s.Length == 0)
					continue;

				// Определение путей к разделам реестра
				if (s.Substring (0, 1) == "[")
					{
					currentPath = s;

					// Проверяем, не содержит ли путь пометку на удаление
					RegistryEntry re = new RegistryEntry (s, "\\", "");

					// В таком варианте запись будет валидна только и исключительно в случае наличия пометки
					if (re.IsValid)
						{
						entries.Add (re);
						entriesCounter++;
						}

					// Здесь больше ничего нет
					continue;
					}

				// Определение параметров реестра
				if (s.Length > 0)
					{
					// Проверяем наличие сплиттера
					int indexOfSplitter = s.IndexOf (registryFileSplitter);

					// Сплиттер отсутствует или стоит на краю строки
					// (или в обработку попала некорректная строка)
					if ((indexOfSplitter <= 0) || (indexOfSplitter >= s.Length - 1))
						continue;

					// Пробуем собрать запись
					s2 = s.Substring (indexOfSplitter + 1); // Сначала нужно обрубить лишние кавычки
					if (s2.Length >= 2)
						{
						if (s2.Substring (0, 1) == "\"")
							s2 = s2.Substring (1);
						if (s2.Substring (s2.Length - 1, 1) == "\"")
							s2 = s2.Substring (0, s2.Length - 1);
						}
					s2 = s2.Replace ("\\\"", "\"");
					s2 = s2.Replace ("\\\\", "\\");

					RegistryEntry re = new RegistryEntry (currentPath, s.Substring (0, indexOfSplitter), s2);
					if (!re.IsValid)
						continue;

					// Успешно. Добавляем в базу
					entries.Add (re);
					entriesCounter++;
					}
				}

			// Завершено
			SR.Close ();
			FS.Close ();
			changed = (entriesCounter != 0);
			return entriesCounter;
			}

		/// <summary>
		/// Метод сохраняет выбранные записи файл реестра
		/// </summary>
		/// <param name="FileName">Путь к файлу для сохранения</param>
		/// <param name="PositionsToSave">Индексы позиций для сохранения</param>
		public bool SaveRegistryFile (string FileName, List<int> PositionsToSave)
			{
			// Контроль
			if (!isInited)
				return false;

			// Попытка открытия файла
			try
				{
				FS = new FileStream (FileName, FileMode.Create);
				}
			catch
				{
				return false;
				}
			SW = new StreamWriter (FS, RDGenerics.GetEncoding (RDEncodings.Unicode16));

			// Запись
			SW.WriteLine ("Windows Registry Editor Version 5.00");
			SW.WriteLine ("");

			for (int i = 0; i < PositionsToSave.Count; i++)
				{
				// Защита
				int p = PositionsToSave[i];
				if ((p >= entries.Count) || (p < 0))
					continue;
				if (!entries[p].IsValid || (entries[p].ValueType != RegistryValueTypes.REG_SZ))
					continue;

				// Сборка строки
				SW.Write ("[");
				if (entries[p].PathMustBeDeleted)
					SW.Write ("-");
				SW.Write (entries[p].ValuePath);
				SW.WriteLine ("]");
				if (entries[p].PathMustBeDeleted)
					continue;

				if (string.IsNullOrWhiteSpace (entries[p].ValueName))
					SW.Write ("@");
				else
					SW.Write ("\"" + entries[p].ValueName + "\"");
				SW.Write ("=");
				if (entries[p].NameMustBeDeleted)
					SW.WriteLine ("-");
				else
					SW.WriteLine ("\"" + entries[p].ValueObject.Replace ("\"", "\\\"") + "\"");
				SW.WriteLine ("");
				}

			// Завершено
			SW.Close ();
			FS.Close ();

			return true;
			}

		/// <summary>
		/// Метод добавляет реестровую запись в базу
		/// </summary>
		/// <param name="Entry">Реестровая запись для добавления</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool AddEntry (RegistryEntry Entry)
			{
			if (!isInited || !Entry.IsValid)
				return false;

			entries.Add (Entry);
			return (changed = true);
			}

		/// <summary>
		/// Метод удаляет реестровую запись из базы
		/// </summary>
		/// <param name="EntryNumber">Номер записи в базе</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool DeleteEntry (uint EntryNumber)
			{
			// Если параметр некорректен, вернуть некорректную запись
			if (!isInited || (EntryNumber > entries.Count))
				return false;

			entries.RemoveAt ((int)EntryNumber);
			return (changed = true);
			}

		/// <summary>
		/// Метод применяет все реестровые записи
		/// </summary>
		/// <returns>Возвращает количество успешно применённых записей</returns>
		public uint ApplyAllEntries ()
			{
			uint counter = 0;

			if (isInited)
				{
				for (int i = 0; i < entries.Count; i++)
					{
					// Применяем только те, которые есть смысл применять
					if (entries[i].GetEntryApplicationState () != RegistryEntryApplicationResults.FullyApplied)
						if (entries[i].ApplyEntry () == RegistryEntryApplicationResults.FullyApplied)
							counter++;
					}
				}

			return counter;
			}
		}
	}
