using Microsoft.Win32;
using System;
using System.Globalization;

namespace FileExtensionsManager
	{
	/// <summary>
	/// Класс описывает отдельную запись в реестре Windows
	/// </summary>
	public class RegistryEntry:IComparable<RegistryEntry>, ICloneable, IEquatable<RegistryEntry>
		{
		/// <summary>
		/// Полный путь к разделу реестра
		/// </summary>
		public string ValuePath
			{
			get
				{
				return valuePath;
				}
			}
		private string valuePath = "";

		// Переменная содержит эквивалентный корневой ключ реестра в терминах .NET
		private RegistryKey valueMasterKey = Registry.ClassesRoot;

		/// <summary>
		/// Название параметра реестра
		/// </summary>
		public string ValueName
			{
			get
				{
				return valueName;
				}
			}
		private string valueName = "";

		/// <summary>
		/// Значение параметра реестра
		/// </summary>
		public string ValueObject
			{
			get
				{
				return valueObject;
				}
			}
		private string valueObject = "";

		/// <summary>
		/// Тип параметра реестра
		/// </summary>
		public RegistryValueTypes ValueType
			{
			get
				{
				return valueType;
				}
			}
		private RegistryValueTypes valueType = RegistryValueTypes.REG_SZ;

		// Тип параметра реестра в терминологии .NET
		private RegistryValueKind valueTypeAsKind = RegistryValueKind.String;

		/// <summary>
		/// Конструктор. Создаёт запись реестра Windows
		/// </summary>
		/// <param name="Path">Путь к разделу</param>
		/// <param name="Name">Название параметра</param>
		/// <param name="Value">Значение параметра (без внешних кавычек)</param>
		public RegistryEntry (string Path, string Name, string Value)
			{
			CreateRegistryEntry (Path, Name, Value, RegistryValueTypes.REG_SZ, false, false);
			}

		/// <summary>
		/// Конструктор. Создаёт запись реестра Windows
		/// </summary>
		/// <param name="Path">Путь к разделу</param>
		/// <param name="Name">Название параметра</param>
		/// <param name="Value">Значение параметра (без внешних кавычек)</param>
		/// <param name="DeleteName">Флаг, указывающий на необходимость удаления указанного параметра реестра;
		/// при значении true принудительно задаёт признак '-' в параметре Value</param>
		/// <param name="DeletePath">Флаг, указывающий на необходимость удаления указанного раздела реестра;
		/// при значении true принудительно задаёт признак '-' в параметре Path</param>
		/// <param name="Type">Тип значения параметра реестра; при значении, не равном REG_SZ, принудительно
		/// задаёт тип в обход спецификации в параметре Value</param>
		public RegistryEntry (string Path, string Name, string Value, RegistryValueTypes Type, bool DeletePath, bool DeleteName)
			{
			CreateRegistryEntry (Path, Name, Value, Type, DeletePath, DeleteName);
			}

		// Метод-инициализатор экземпляра класса
		private void CreateRegistryEntry (string Path, string Name, string Value, RegistryValueTypes Type, bool DeletePath, bool DeleteName)
			{
			// Переменные
			char[] pathSplitters = new char[] { '\\' };
			char[] valuesSplitters = new char[] { '\"', '[', ']' };

			// Контроль пути
			string[] values = Path.Split (valuesSplitters, System.StringSplitOptions.RemoveEmptyEntries);
			if (values.Length != 1)		// Означает наличие недопустимых символов или пустую строку
				return;

			pathMustBeDeleted = (values[0].Substring (0, 1) == "-");		// Раздел помечен на удаление

			string s = values[0];
			if (pathMustBeDeleted)
				s = values[0].Substring (1);

			pathMustBeDeleted |= DeletePath;

			values = s.Split (pathSplitters, System.StringSplitOptions.RemoveEmptyEntries);	// Убираем лишние слэшы
			if (values.Length < 1)	// Такого не должно случиться, но вдруг там только слэшы и были
				return;

			if (values[0] == Registry.ClassesRoot.Name)
				valueMasterKey = Registry.ClassesRoot;
			/*else if (values[0] == Registry.CurrentConfig.Name)
				valueMasterKey = Registry.CurrentConfig;
			else if (values[0] == Registry.CurrentUser.Name)
				valueMasterKey = Registry.CurrentUser;
			else if (values[0] == Registry.LocalMachine.Name)
				valueMasterKey = Registry.LocalMachine;
			else if (values[0] == Registry.Users.Name)
				valueMasterKey = Registry.Users;*/		// Остальные разделы запрещены областью применения программы
			else
				return;		// Если корневой раздел не является допустимым

			// Пересобираем путь
			valuePath = values[0];
			for (int i = 1; i < values.Length; i++)
				valuePath += (pathSplitters[0].ToString () + values[i]);

			// Если путь помечен на удаление, остальные параметры можно (и нужно) проигнорировать
			if (pathMustBeDeleted)
				{
				isValid = true;
				return;
				}

			// Контроль имени
			values = Name.Split (valuesSplitters, System.StringSplitOptions.RemoveEmptyEntries);
			if (values.Length == 0)		// Пустая строка здесь допустима - для значения по умолчанию
				{
				valueName = "";
				}
			else if (values.Length > 1)
				{
				return;					// Означает наличие недопустимых символов или пустую строку
				}
			else
				{
				valueName = values[0];
				}

			if (valueName.Contains (pathSplitters[0].ToString ()))
				return;

			if (valueName == "@")
				valueName = "";

			// Контроль значения (может содержать любые символы; предполагается, что кавычки уже удалены)
			if ((Value == "-") || DeleteName)		// Параметр помечен на удаление
				{
				nameMustBeDeleted = isValid = true;
				valueObject = "-";
				return;
				}

			// Проверка на наличие спецификации типа
			if (Value.StartsWith ("hex:"))
				valueType = RegistryValueTypes.REG_BINARY;
			if (Value.StartsWith ("dword:"))
				valueType = RegistryValueTypes.REG_DWORD;
			if (Value.StartsWith ("hex(2):"))
				valueType = RegistryValueTypes.REG_EXPAND_SZ;
			if (Value.StartsWith ("hex(7):"))
				valueType = RegistryValueTypes.REG_MULTI_SZ;
			if (Value.StartsWith ("hex(0):"))
				valueType = RegistryValueTypes.REG_NONE;
			if (Value.StartsWith ("hex(b):"))
				valueType = RegistryValueTypes.REG_QWORD;

			// Обработка значения в зависимости от типа данных
			switch (valueType)
				{
				case RegistryValueTypes.REG_SZ:
					valueObject = Value;
					break;

				case RegistryValueTypes.REG_DWORD:
					try
						{
						valueObject = (Int32.Parse (Value.Substring (6), NumberStyles.HexNumber)).ToString ();
						}
					catch
						{
						return;
						}
					break;

				case RegistryValueTypes.REG_QWORD:
					try
						{
						Int64 a = Int64.Parse (Value.Substring (7).Replace (",", ""), NumberStyles.HexNumber);
						a = (ExcludeByte (a, 0) << 56) | (ExcludeByte (a, 1) << 48) |
							(ExcludeByte (a, 2) << 40) | (ExcludeByte (a, 3) << 32) |
							(ExcludeByte (a, 4) << 24) | (ExcludeByte (a, 5) << 16) |
							(ExcludeByte (a, 6) << 8) | ExcludeByte (a, 7);
						valueObject = a.ToString ();
						}
					catch
						{
						return;
						}
					break;

				default:
					return;		// Остальные типы мы пока обрабатывать не умеем
				}

			// Переопределение, если указано (делается позже остального, чтобы не вынуждать значения, 
			// пропущенные через базу, обрабатываться снова)
			if (Type != RegistryValueTypes.REG_SZ)
				{
				// Установка типа
				valueType = Type;

				switch (valueType)
					{
					case RegistryValueTypes.REG_BINARY:
						valueTypeAsKind = RegistryValueKind.Binary;
						break;
					case RegistryValueTypes.REG_DWORD:
						valueTypeAsKind = RegistryValueKind.DWord;
						break;
					case RegistryValueTypes.REG_EXPAND_SZ:
						valueTypeAsKind = RegistryValueKind.ExpandString;
						break;
					case RegistryValueTypes.REG_MULTI_SZ:
						valueTypeAsKind = RegistryValueKind.MultiString;
						break;
					case RegistryValueTypes.REG_NONE:
						valueTypeAsKind = RegistryValueKind.None;
						break;
					case RegistryValueTypes.REG_QWORD:
						valueTypeAsKind = RegistryValueKind.QWord;
						break;
					}

				// Постконтроль значения (запрет на подстановку не-чисел в числовые типы)
				try
					{
					if (valueType == RegistryValueTypes.REG_QWORD)
						{
						Int64 a = Int64.Parse (valueObject);
						}
					if (valueType == RegistryValueTypes.REG_DWORD)
						{
						Int32 b = Int32.Parse (valueObject);
						}
					}
				catch
					{
					return;
					}
				}

			// Завершено
			isValid = true;
			}

		/// <summary>
		/// Возвращает флаг валидности (корректности) реестровой записи
		/// </summary>
		public bool IsValid
			{
			get
				{
				return isValid;
				}
			}
		private bool isValid = false;

		/// <summary>
		/// Возвращает состояние пометки на удаление раздела реестра
		/// </summary>
		public bool PathMustBeDeleted
			{
			get
				{
				return pathMustBeDeleted;
				}
			}
		private bool pathMustBeDeleted = false;

		/// <summary>
		/// Возвращает состояние пометки на удаление параметра реестра
		/// </summary>
		public bool NameMustBeDeleted
			{
			get
				{
				return nameMustBeDeleted;
				}
			}
		private bool nameMustBeDeleted = false;

		// Метод извлекает значение байта из числа по его номеру
		// - тип числа сохраняется
		// - байты нумеруются с нуля от младшего к старшему
		// - из номера байта используются только первые три бита
		private Int64 ExcludeByte (Int64 Value, byte ByteNumber)
			{
			return (Value >> (8 * (ByteNumber % 8))) & 0xFFL;
			}

		/// <summary>
		/// Метод возвращает степень соответствия состояния реестра данной записи
		/// </summary>
		/// <returns>Возвращает результат проверки</returns>
		public RegistryEntryApplicationResults GetEntryApplicationState ()
			{
			// Контроль состояния
			if (!isValid)
				return RegistryEntryApplicationResults.CannotGetAccess;

			// Запрос
			string val = "";
			object obj = null;
			try
				{
				// Если раздел помечен на удаление
				if (pathMustBeDeleted)		// Возврат отсюда обязателен
					{
					if ((obj = Registry.GetValue (valuePath, valueName, "")) == null)	// При отсутствии раздела вернёт именно null
						return RegistryEntryApplicationResults.FullyApplied;

					return RegistryEntryApplicationResults.NotApplied;	// Если что-то вернулось, значит, раздел существует
					}

				// Если значение помечено на удаление
				if (nameMustBeDeleted)		// Возврат отсюда обязателен
					{
					if ((obj = Registry.GetValue (valuePath, valueName, null)) == null)	// При отсутствии параметра или раздела вернёт null
						return RegistryEntryApplicationResults.FullyApplied;

					return RegistryEntryApplicationResults.NotApplied;	// Если что-то вернулось, значит, раздел существует
					}

				if ((obj = Registry.GetValue (valuePath, valueName, null)) == null)
					return RegistryEntryApplicationResults.NotApplied;
				val = obj.ToString ();
				}
			catch
				{
				return RegistryEntryApplicationResults.CannotGetAccess;
				}

			// Определение статуса
			if (val != valueObject)
				return RegistryEntryApplicationResults.PartiallyApplied;
			else
				return RegistryEntryApplicationResults.FullyApplied;
			}

		/// <summary>
		/// Метод применяет данную реестровую запись
		/// </summary>
		/// <returns>Возвращает результат применения</returns>
		public RegistryEntryApplicationResults ApplyEntry ()
			{
			// Контроль состояния
			if (!isValid)
				return RegistryEntryApplicationResults.CannotGetAccess;

			// Применение записи
			if (nameMustBeDeleted)
				{
				RegistryKey key;
				try
					{
					key = valueMasterKey.OpenSubKey (valuePath.Replace (valueMasterKey.Name, ""), true);
					key.DeleteValue (valueName, false);
					key.Dispose ();
					}
				catch
					{
					// Пропускаем это состояние, т.к. оно может означать отсутствие ключа
					}
				}

			try
				{
				if (pathMustBeDeleted)
					valueMasterKey.DeleteSubKeyTree (valuePath.Replace (valueMasterKey.Name, ""), false);
				else if (!nameMustBeDeleted)
					Registry.SetValue (valuePath, valueName, valueObject, valueTypeAsKind);
				}
			catch
				{
				return RegistryEntryApplicationResults.CannotGetAccess;
				}

			// Контроль результата
			return GetEntryApplicationState ();
			}

		// Члены IComparable

		/// <summary>
		/// Метод выполняет сравнение указанного элемента с данным экземпляром
		/// </summary>
		/// <param name="EntryToCompare">Элемент, с которым выполняется сравнение</param>
		/// <returns>Возвращает -1, если указанный элемент следует поменять местами с данным экземпляром;
		/// 0, если элементы одинаковы; 1, если порядок правильный</returns>
		public int CompareTo (RegistryEntry EntryToCompare)
			{
			// При совпадении путей выполняется сортировка по именам параметров
			if (valuePath.CompareTo (EntryToCompare.valuePath) == 0)
				return valueName.CompareTo (EntryToCompare.valueName);

			return valuePath.CompareTo (EntryToCompare.valuePath);
			}

		// Члены ICloneable

		/// <summary>
		/// Метод возвращает копию данного экземпляра
		/// </summary>
		/// <returns></returns>
		public object Clone ()
			{
			return new RegistryEntry (valuePath, valueName, valueObject, valueType, pathMustBeDeleted, nameMustBeDeleted);
			}

		// Члены IEquatable<RegistryEntry>

		/// <summary>
		/// Метод определяет идентичность указанного элемента данному экземпляру 
		/// </summary>
		/// <param name="EntryToEquate">Элемент, с которым выполняется сравнение</param>
		/// <returns>Возвращает true, если элементы идентичны</returns>
		public bool Equals (RegistryEntry EntryToEquate)
			{
			return (valuePath == EntryToEquate.valuePath) &&
				(valueName == EntryToEquate.valueName) &&
				(valueObject == EntryToEquate.valueObject) &&
				(pathMustBeDeleted == EntryToEquate.pathMustBeDeleted) &&
				(nameMustBeDeleted == EntryToEquate.nameMustBeDeleted) &&
				(isValid == EntryToEquate.isValid) &&
				(valueType == EntryToEquate.valueType);
			}
		}
	}
