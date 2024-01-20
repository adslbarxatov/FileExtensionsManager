using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Форма позволяет выполнять обзор иконок, содержащихся в исполняемом файле или библиотеке
	/// </summary>
	public partial class IconsExtractor: Form
		{
		// Ширина поля иконки
		private const uint iconPositionWidth = 35;

		// Высота иконки
		private const uint iconHeight = 32;

		// Высота поля иконки
		private const uint iconPositionHeight = 48;

		// Количество иконок в строке
		private const uint iconsHorizontalCount = 20;

		// Количество иконок в столбце
		private const uint iconsVerticalCount = 10;

		// Конечная страница с иконками
		private Bitmap iconsView;

		// Количество найденных иконок
		private UInt32 iconsCount;

		// Флаг разрешения закрытия окна
		private bool allowExit = true;

		// Кисть для выбранной иконки
		private Brush selectionBrush = new SolidBrush (Color.FromArgb (128, 255, 255, 0));

		/// <summary>
		/// Возвращает номер выбранной иконки, начиная с 0, или -1, если иконка не была выбрана
		/// </summary>
		public int SelectedIconNumber
			{
			get
				{
				return selectedIconNumber;
				}
			}
		private int selectedIconNumber = -1;    // Выбранная иконка
		private decimal currentPage = 0;        // Страница, на которой она расположена

		/// <summary>
		/// Возвращает путь к файлу, в котором содержится выбранная иконка
		/// </summary>
		public string SelectedIconFile
			{
			get
				{
				return OFDialog.FileName;
				}
			}

		/// <summary>
		/// Функция WinAPI, позволяющая извлекать значки из файлов приложений и библиотек
		/// </summary>
		/// <param name="FileName">Имя файла, содержащего значки</param>
		/// <param name="IconIndex">Номер значка, начиная с 0; -1, если требуется получить количество значков</param>
		/// <param name="IconLarge">Указатель для получения больших значков</param>
		/// <param name="IconSmall">Указатель для получения маленьких значков</param>
		/// <param name="IconsCount">Количество получаемых значков (большой и маленький значок с одним номером 
		/// считаются за один значок)</param>
		/// <returns>Количество успешно полученных значков (большой и маленький значок с одним номером считаются 
		/// за два значка)</returns>
		[DllImport ("shell32.dll")]
		private static extern UInt32 ExtractIconExA (string FileName, Int32 IconIndex, ref IntPtr IconLarge,
			ref IntPtr IconSmall, UInt32 IconsCount);

		/// <summary>
		/// Конструктор. Запускает форму
		/// </summary>
		public IconsExtractor ()
			{
			// Инициализация
			InitializeComponent ();
			this.AcceptButton = SelectButton;
			this.CancelButton = AbortButton;

			// Настройка контролов
			OFDialog.Title = RDLocale.GetText ("IE_OFDialogTitle");
			OFDialog.Filter = RDLocale.GetText ("IE_OFDialogFilter");

			MainPicture.Width = (int)(iconPositionWidth * iconsHorizontalCount + 4);
			MainPicture.Height = (int)(iconPositionHeight * iconsVerticalCount + 4);

			this.Width = MainPicture.Width + 40;
			this.Height = MainPicture.Height + 90;

			PageNumber.Top = PageLabel.Top = TotalLabel.Top = SelectButton.Top = AbortButton.Top = this.Height - 70;
			TotalLabel.Left = MainPicture.Left + MainPicture.Width - TotalLabel.Width;

			SelectButton.Left = this.Width / 2 - SelectButton.Width - 6;
			AbortButton.Left = this.Width / 2 + 6;

			AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
			PageLabel.Text = RDLocale.GetText ("IE_PageLabel");

			// Запуск
			this.Text = ProgramDescription.AssemblyTitle + RDLocale.GetText ("IE_Title");
			this.ShowDialog ();
			}

		private void IconsExtractor_Load (object sender, EventArgs e)
			{
			// Запрос исполняемого файла
			if (OFDialog.ShowDialog () != DialogResult.OK)
				{
				this.Close ();
				return;
				}

			// Проверка на наличие иконок
			IntPtr bigIcon = IntPtr.Zero,
				smallIcon = IntPtr.Zero;
			iconsCount = ExtractIconExA (OFDialog.FileName, -1, ref bigIcon, ref smallIcon, 1);
			if (iconsCount == 0)
				{
				RDGenerics.MessageBox (RDMessageTypes.Information_Center,
					string.Format (RDLocale.GetText ("IconsNotFound"), OFDialog.FileName));
				this.Close ();
				return;
				}

			// Извлечение иконок
			List<Bitmap> icons = new List<Bitmap> ();
			for (UInt32 i = 0; i < iconsCount; i++)
				{
				ExtractIconExA (OFDialog.FileName, (Int32)i, ref bigIcon, ref smallIcon, 1);
				icons.Add (Icon.FromHandle (bigIcon).ToBitmap ());      // Если делать проще, теряется альфа-канал
				}

			// Сборка изображения
			iconsView = new Bitmap ((int)(iconPositionWidth * iconsHorizontalCount),
				(int)((decimal)iconPositionHeight * Math.Ceiling ((decimal)iconsCount / (decimal)iconsHorizontalCount)),
				PixelFormat.Format32bppArgb);

			Graphics g = Graphics.FromImage (iconsView);
			Brush b = new SolidBrush (Color.FromArgb (0, 0, 0));
			Font f = new Font ("Arial", 10);
			SizeF sz;

			for (int i = 0; i < icons.Count; i++)
				{
				g.DrawImage (icons[i], iconPositionWidth * (i % iconsHorizontalCount), iconPositionHeight * (i / iconsHorizontalCount));
				sz = g.MeasureString (i.ToString (), f);
				g.DrawString (i.ToString (), f, b, iconPositionWidth * (i % iconsHorizontalCount) + (iconPositionWidth - sz.Width) / 2,
					iconPositionHeight * (i / iconsHorizontalCount) + iconHeight);
				icons[i].Dispose ();
				}

			// Завершение
			b.Dispose ();
			g.Dispose ();
			f.Dispose ();
			icons.Clear ();

			// Отображение
			MainPicture.BackgroundImage = (Bitmap)iconsView.Clone ();
			PageNumber.Maximum = Math.Ceiling ((decimal)iconsCount /
				((decimal)iconsVerticalCount * (decimal)iconsHorizontalCount));
			TotalLabel.Text = RDLocale.GetText ("IE_TotalLabelText") + iconsCount.ToString ();
			allowExit = false;
			this.Activate ();
			}

		// Смена страницы
		private void PageNumber_ValueChanged (object sender, EventArgs e)
			{
			int offset = (int)((PageNumber.Value - 1) * iconsVerticalCount * iconPositionHeight);
			int selectedNumber = selectedIconNumber % (int)(iconsVerticalCount * iconsHorizontalCount);

			// Замена изображения
			MainPicture.BackgroundImage.Dispose ();
			MainPicture.BackgroundImage = iconsView.Clone (new Rectangle (0, offset, iconsView.Width,
				iconsView.Height - offset), iconsView.PixelFormat);

			// Отображение выделения
			if ((selectedNumber >= 0) && (currentPage == PageNumber.Value))
				{
				Graphics g = Graphics.FromImage (MainPicture.BackgroundImage);
				g.FillRectangle (selectionBrush, (selectedNumber % iconsHorizontalCount) * iconPositionWidth,
					(selectedNumber / iconsHorizontalCount) * iconPositionHeight, iconPositionWidth,
					iconPositionHeight);
				g.Dispose ();
				}
			}

		// Выбор иконки
		private void MainPicture_MouseClick (object sender, MouseEventArgs e)
			{
			// Расчёт позиции выбора
			uint selectedNumber = ((uint)PageNumber.Value - 1) * iconsVerticalCount * iconsHorizontalCount +
				((uint)e.Y / iconPositionHeight) * iconsHorizontalCount + (uint)e.X / iconPositionWidth;

			// Контроль
			if (selectedNumber < iconsCount)
				{
				SelectButton.Enabled = true;
				SelectButton.Text = RDLocale.GetText ("IE_SelectButtonText") + selectedNumber.ToString ();

				selectedIconNumber = (int)selectedNumber;
				currentPage = PageNumber.Value;
				PageNumber_ValueChanged (null, null);   // Перерисовка
				}
			}

		// Обработка выхода
		private void IconsExtractor_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = !allowExit;
			}

		private void SelectButton_Click (object sender, EventArgs e)
			{
			allowExit = true;
			this.Close ();
			}

		private void AbortButton_Click (object sender, EventArgs e)
			{
			selectedIconNumber = -1;
			allowExit = true;
			this.Close ();
			}

		/// <summary>
		/// Метод возвращает количество иконок, доступных в указанном файле
		/// </summary>
		/// <param name="FileName">Файл для поиска иконок</param>
		/// <returns>Количество иконок</returns>
		public static uint GetIconsCount (string FileName)
			{
			IntPtr bigIcon = IntPtr.Zero,
				smallIcon = IntPtr.Zero;
			return ExtractIconExA (FileName, -1, ref bigIcon, ref smallIcon, 1);
			}
		}
	}
