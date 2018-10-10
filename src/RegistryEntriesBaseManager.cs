using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileExtensionsManager
	{
	/// <summary>
	/// Класс описывает интерфейс базы управляемых реестровых записей
	/// </summary>
	public class RegistryEntriesBaseManager
		{
		// Переменные и константы
		private List<RegistryEntry> entries = new List<RegistryEntry> ();	// База реестровых записей

		private Encoding registryFileEncoding = Encoding.Unicode;			// Кодировки файлов
		private Encoding baseFileEncoding = Encoding.GetEncoding (1251);
		private string registryFileSplitter = "=";							// Сплиттер параметра и значения в файле реестра
		private char[] baseFileSplitters = new char[] { '\x1' };			// Сплиттер записей в базе

		private bool changed = true;							// Флаг указывает, что в базу были внесены изменения
		private List<string> ebp = new List<string> ();			// Список представлений записей в базе
		private List<RegistryEntryApplicationResults> esp = new List<RegistryEntryApplicationResults> ();

		private FileStream FS = null;							// Файловые дескрипторы
		private StreamReader SR = null;
		private StreamWriter SW = null;

		private const string newBaseFileName = "Новая база";	// Имя файла новой базы реестровых записей

		/// <summary>
		/// Расширение имени файла базы реестровых записей
		/// </summary>
		public const string BaseFileExtension = ".reb";

		/// <summary>
		/// Конструктор. Загружает базу реестровых записей
		/// </summary>
		/// <param name="BaseName">Имя создаваемой базы</param>
		public RegistryEntriesBaseManager (string BaseName)
			{
			// Сохранение параметров
			baseName = BaseName;

			// Загрузка базы
			if (!LoadBase ())		// Если загрузка завершается с ошибкой
				{
				if (!SaveBase ())	// Пробуем создать базу
					{
					return;			// Если не получается, прерываем загрузку
					}
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
			if (!SaveBase ())	// Пробуем создать базу
				return;			// Если не получается, прерываем загрузку

			isInited = true;
			}

		// Метод загружает базу
		private bool LoadBase ()
			{
			// Попытка открытия файла
			try
				{
				FS = new FileStream (baseName + BaseFileExtension, FileMode.Open);
				}
			catch
				{
				return false;
				}
			SR = new StreamReader (FS, baseFileEncoding);

			// Чтение файла
			SR.ReadLine ();		// Версия

			while (!SR.EndOfStream)
				{
				string s = SR.ReadLine ();
				string[] values = s.Split (baseFileSplitters);	// Пустые поля не удалять, т.к. они отвечают за значения по умолчанию
				if (values.Length != 6)
					continue;	// Если вдруг попадётся битая запись, пропустить её

				RegistryValueTypes valueType = RegistryValueTypes.REG_SZ;
				try
					{
					valueType = (RegistryValueTypes)uint.Parse (values[3]);
					}
				catch
					{
					}
				RegistryEntry re = new RegistryEntry (values[0], values[1], values[2], valueType, values[4] != "0", values[5] != "0");
				if (!re.IsValid)
					continue;	// Аналогично

				// В противном случае добавляем запись
				entries.Add (re);
				}

			// Завершено
			SR.Close ();
			FS.Close ();
			return true;
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

			// Попытка открытия базы
			try
				{
				FS = new FileStream (baseName + BaseFileExtension, FileMode.Create);	// Перезаписывает недоступный файл при необходимости
				}
			catch
				{
				return false;
				}
			SW = new StreamWriter (FS, baseFileEncoding);

			// Запись
			SW.Write (ProgramDescription.AssemblyTitle + "; timestamp: " + DateTime.Now.ToString ("dd.MM.yyyy, HH:mm") + "\n");
			for (int i = 0; i < entries.Count; i++)
				{
				SW.Write (entries[i].ValuePath + baseFileSplitters[0].ToString () +
					entries[i].ValueName + baseFileSplitters[0].ToString () +
					entries[i].ValueObject + baseFileSplitters[0].ToString () +
					((uint)(entries[i].ValueType)).ToString () + baseFileSplitters[0].ToString () +
					(entries[i].PathMustBeDeleted ? "1" : "0") + baseFileSplitters[0].ToString () +
					(entries[i].NameMustBeDeleted ? "1" : "0") + "\n");
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
				if (isInited && changed)	// Выполнять обновление, только если в базу вносились изменения
					{
					// Сортировка
					entries.Sort ();

					// Сборка списка
					ebp.Clear ();
					for (int i = 0; i < entries.Count; i++)
						{
						ebp.Add ("[" + entries[i].ValuePath + "]: " + ((entries[i].ValueName == "") ? "@" : entries[i].ValueName) + " = " +
							entries[i].ValueObject +
							((entries[i].ValueType != RegistryValueTypes.REG_SZ) ? " (" + entries[i].ValueType.ToString () + ")" : "") +
							(entries[i].PathMustBeDeleted ? "; удаляемый раздел" : "") +
							(entries[i].NameMustBeDeleted ? "; удаляемый параметр" : ""));
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
				if (isInited)	// Выполнять обновление всегда, когда возможно, и без сортировки
					{
					// Сборка списка
					esp.Clear ();
					for (int i = 0; i < entries.Count; i++)
						{
						esp.Add (entries[i].GetEntryApplicationState ());
						}
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

			return (RegistryEntry)entries[(int)EntryNumber].Clone ();	// Защита от неотслеживаемого доступа
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
		/// <param name="FileName"></param>
		/// <returns></returns>
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
			SR = new StreamReader (FS, registryFileEncoding);

			// Контроль состава файла
			string s = SR.ReadLine ().ToLower (), s2;
			if (!s.Contains ("regedit") && !s.Contains ("windows registry"))	// Заодно исключает возможные проблемы с кодировкой
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
					if (re.IsValid)		// В таком варианте запись будет валидна только и исключительно в случае наличия пометки
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
					if ((indexOfSplitter <= 0) || (indexOfSplitter >= s.Length - 1))	// Сплиттер отсутствует или стоит на краю строки
						continue;														// (или в обработку попала некорректная строка)

					// Пробуем собрать запись
					s2 = s.Substring (indexOfSplitter + 1);	// Сначала нужно обрубить лишние кавычки
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
