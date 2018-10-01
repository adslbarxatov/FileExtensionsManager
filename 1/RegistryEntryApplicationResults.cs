namespace FileExtensionsManager
	{
	/// <summary>
	/// Результаты применения реестровой записи
	/// </summary>
	public enum RegistryEntryApplicationResults
		{
		/// <summary>
		/// Запись не применялась, или результат её применения полностью утерян
		/// </summary>
		NotApplied = 0,

		/// <summary>
		/// Запись применялась, но результат её применения частично утерян, или реестр частично соответствует записи
		/// </summary>
		PartiallyApplied = 1,

		/// <summary>
		/// Реестр полностью соответствует записи
		/// </summary>
		FullyApplied = 2,

		/// <summary>
		/// Не удаётся получить доступ к записи
		/// </summary>
		CannotGetAccess = -1
		}
	}
