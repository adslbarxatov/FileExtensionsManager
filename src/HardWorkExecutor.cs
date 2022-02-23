using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс предоставляет интерфейс визуализации прогресса установки/удаления программы
	/// </summary>
	public partial class HardWorkExecutor:Form
		{
		// Переменные
		private bool allowClose = false;                        // Запрет выхода из формы до окончания работы
		private Bitmap progress, frameGreenGrey, frameBack;     // Объекты-отрисовщики
		private Graphics g, gp;
		private int currentXOffset = 0, oldPercentage = 0, newPercentage = 0;
		private object parameters;                              // Параметры инициализации потока
		private bool alwaysOnTop = false;                       // Флаг принудительного размещения поверх всех окон

		// Цветовая схема
		private Color backColor = Color.FromArgb (224, 224, 224);
		private Color foreColor = Color.FromArgb (32, 32, 32);
		private Color greenColor = Color.FromArgb (0, 160, 80);
		private Color greyColor = Color.FromArgb (160, 160, 160);

		/// <summary>
		/// Длина шкалы прогресса
		/// </summary>
		public const uint ProgressBarSize = 1000;

		/// <summary>
		/// Возвращает объект-обвязку исполняемого процесса
		/// </summary>
		public BackgroundWorker Worker
			{
			get
				{
				return bw;
				}
			}
		private BackgroundWorker bw = new BackgroundWorker ();

		/// <summary>
		/// Возвращает результат установки/удаления
		/// </summary>
		public int ExecutionResult
			{
			get
				{
				return executionResult;
				}
			}
		private int executionResult = 0;

		/// <summary>
		/// Возвращает результат выполняемых операций
		/// </summary>
		public string Result
			{
			get
				{
				return result;
				}
			}
		private string result = "";

		// Инициализация ProgressBar
		private void InitializeProgressBar ()
			{
			// Настройка контролов
			InitializeComponent ();
			this.BackColor = backColor;
			StateLabel.ForeColor = AbortButton.ForeColor = foreColor;
			AbortButton.BackColor = backColor;

			// Инициализация
			progress = new Bitmap (this.Width - 20, 30);
			g = Graphics.FromHwnd (this.Handle);
			gp = Graphics.FromImage (progress);

			// Формирование стрелок
			Point[] frame = new Point[] {
					new Point (0, 0),
					new Point (this.Width / 4, 0),
					new Point (this.Width / 4 + progress.Height / 2, progress.Height / 2),
					new Point (this.Width / 4, progress.Height),
					new Point (0, progress.Height),
					new Point (progress.Height / 2, progress.Height / 2)
					};

			// Подготовка дескрипторов
			SolidBrush green = new SolidBrush (greenColor),
				grey = new SolidBrush (greyColor),
				back = new SolidBrush (backColor);

			frameGreenGrey = new Bitmap (10 * this.Width / 4, progress.Height);
			frameBack = new Bitmap (10 * this.Width / 4, progress.Height);
			Graphics g1 = Graphics.FromImage (frameGreenGrey),
				g2 = Graphics.FromImage (frameBack);

			// Сборка
			for (int i = 0; i < 8; i++)
				{
				for (int j = 0; j < frame.Length; j++)
					{
					frame[j].X += this.Width / 4;
					}

				g1.FillPolygon ((i % 2 == 0) ? green : grey, frame);
				g2.FillPolygon (back, frame);
				}

			// Объём
			for (int i = 0; i < frameGreenGrey.Height; i++)
				{
				Pen p = new Pen (Color.FromArgb (200 - (int)(200.0 * Math.Sin (Math.PI * (double)i /
					(double)frameGreenGrey.Height)), this.BackColor));
				g1.DrawLine (p, 0, i, frameGreenGrey.Width, i);
				p.Dispose ();
				}

			// Освобождение ресурсов
			g1.Dispose ();
			g2.Dispose ();
			green.Dispose ();
			grey.Dispose ();
			back.Dispose ();

			// Запуск таймера
			DrawingTimer.Interval = 1;
			DrawingTimer.Enabled = true;
			}

#if DPMODULE

		/// <summary>
		/// Конструктор. Выполняет настройку и запуск процесса установки/удаления
		/// </summary>
		/// <param name="HardWorkProcess">Процесс, выполняющий установку/удаление</param>
		/// <param name="SetupPath">Путь установки/удаления</param>
		/// <param name="Flags">Флаги процедуры: b0 = режим удаления;
		///										 b1 = разрешено завершение работающих процессов;</param>
		///										 b2 = разрешён запуск приложения по завершении
		/// <param name="PackagePath">Путь к пакету развёртки</param>
		public HardWorkExecutor (DoWorkEventHandler HardWorkProcess, string SetupPath, string PackagePath, uint Flags)
			{
			// Инициализация
			string[] arguments = new string[] { SetupPath, PackagePath, Flags.ToString () };
			HardWorkExecutor_Init (HardWorkProcess, arguments, " ", false, true);
			}

		/// <summary>
		/// Конструктор. Выполняет проверку доступной версии обновления в скрытом режиме
		/// </summary>
		/// <param name="HardWorkProcess">Выполняемый процесс</param>
		/// <param name="PackageVersion">Версия пакета развёртки для сравнения</param>
		public HardWorkExecutor (DoWorkEventHandler HardWorkProcess, string PackageVersion)
			{
			HardWorkExecutor_Init (HardWorkProcess, PackageVersion, null, true, false);
			}

		/// <summary>
		/// Конструктор. Выполняет указанное действие с указанными параметрами
		/// </summary>
		/// <param name="HardWorkProcess">Выполняемый процесс</param>
		/// <param name="Parameters">Параметры, передаваемые в процесс; может быть null</param>
		/// <param name="WindowCaption">Строка, отображаемая при инициализации окна прогресса;
		/// если null, окно прогресса не отображается</param>
		/// <param name="CaptionInTheMiddle">Флаг указывает, что подпись будет выравниваться посередине</param>
		/// <param name="AllowOperationAbort">Флаг указывает, разрешена ли отмена операции</param>
		/// <param name="AlwaysOnTop">Флаг указывает на принудительное размежение поверх всех окон</param>
		public HardWorkExecutor (DoWorkEventHandler HardWorkProcess, object Parameters, string WindowCaption,
			bool CaptionInTheMiddle, bool AllowOperationAbort, bool AlwaysOnTop)
			{
			alwaysOnTop = AlwaysOnTop;
			HardWorkExecutor_Init (HardWorkProcess, Parameters, WindowCaption, CaptionInTheMiddle,
				AllowOperationAbort);
			}

#else

		/// <summary>
		/// Конструктор. Выполняет указанное действие с указанными параметрами
		/// </summary>
		/// <param name="HardWorkProcess">Выполняемый процесс</param>
		/// <param name="Parameters">Параметры, передаваемые в процесс; может быть null</param>
		/// <param name="WindowCaption">Строка, отображаемая при инициализации окна прогресса;
		/// если null, окно прогресса не отображается</param>
		/// <param name="CaptionInTheMiddle">Флаг указывает, что подпись будет выравниваться посередине</param>
		/// <param name="AllowOperationAbort">Флаг указывает, разрешена ли отмена операции</param>
		public HardWorkExecutor (DoWorkEventHandler HardWorkProcess, object Parameters, string WindowCaption,
			bool CaptionInTheMiddle, bool AllowOperationAbort)
			{
			HardWorkExecutor_Init (HardWorkProcess, Parameters, WindowCaption, CaptionInTheMiddle,
				AllowOperationAbort);
			}

#endif

		/// <summary>
		/// Конструктор. Выполняет загрузку файла по URL
		/// </summary>
		/// <param name="HardWorkProcess">Выполняемый процесс</param>
		/// <param name="Length">Размер пакета</param>
		/// <param name="TargetPath">Путь создаваемого файла</param>
		/// <param name="URL">Ссылка для загрузки</param>
		public HardWorkExecutor (DoWorkEventHandler HardWorkProcess, string URL, string TargetPath, string Length)
			{
			// Инициализация
			string[] arguments = new string[] { URL, TargetPath, Length };
			HardWorkExecutor_Init (HardWorkProcess, arguments, " ", true, true);
			}

		// Общий метод подготовки исполнителя заданий
		private void HardWorkExecutor_Init (DoWorkEventHandler HWProcess, object Parameters,
			string Caption, bool CaptionInTheCenter, bool AllowAbort)
			{
			// Настройка BackgroundWorker
			bw.WorkerReportsProgress = true;        // Разрешает возвраты изнутри процесса
			bw.WorkerSupportsCancellation = true;   // Разрешает завершение процесса

			bw.DoWork += ((HWProcess != null) ? HWProcess : DoWork);
			bw.RunWorkerCompleted += RunWorkerCompleted;

			// Донастройка окна
			if (string.IsNullOrEmpty (Caption))
				{
				bw.RunWorkerAsync (Parameters);
				}
			else
				{
				bw.ProgressChanged += ProgressChanged;
				parameters = Parameters;

				InitializeProgressBar ();
				newPercentage = oldPercentage = (int)ProgressBarSize;

				AbortButton.Visible = AbortButton.Enabled = AllowAbort;
				StateLabel.Text = Caption;
				if (CaptionInTheCenter)
					StateLabel.TextAlign = ContentAlignment.MiddleCenter;

				// Запуск
				this.ShowDialog ();
				}
			}

		// Метод запускает выполнение процесса
		private void HardWorkExecutor_Shown (object sender, EventArgs e)
			{
			if (alwaysOnTop)
				{
				this.Activate ();
				this.TopMost = true;
				}

			// Запуск отрисовки
			const int roundingSize = 20;
			Bitmap bm = new Bitmap (this.Width, this.Height);
			Graphics gr = Graphics.FromImage (bm);

			SolidBrush br = new SolidBrush (this.TransparencyKey);
			gr.FillRectangle (br, 0, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, this.Width - roundingSize / 2, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, 0, this.Height - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, this.Width - roundingSize / 2, this.Height - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			br.Dispose ();

			br = new SolidBrush (this.BackColor);
			gr.FillEllipse (br, 0, 0, roundingSize, roundingSize);
			gr.FillEllipse (br, this.Width - roundingSize - 1, 0, roundingSize, roundingSize);
			gr.FillEllipse (br, 0, this.Height - roundingSize - 1, roundingSize, roundingSize);
			gr.FillEllipse (br, this.Width - roundingSize - 1, this.Height - roundingSize - 1, roundingSize, roundingSize);
			br.Dispose ();
			gr.Dispose ();

			this.BackgroundImage = bm;

			// Запуск
			bw.RunWorkerAsync (parameters);
			}

		// Метод обрабатывает изменение состояния процесса
		private void ProgressChanged (object sender, ProgressChangedEventArgs e)
			{
			// Обновление ProgressBar
			if (e.ProgressPercentage > ProgressBarSize)
				newPercentage = (int)ProgressBarSize;
			else if (e.ProgressPercentage < 0)
				newPercentage = oldPercentage = 0;  // Скрытие шкалы
			else
				newPercentage = e.ProgressPercentage;

			StateLabel.Text = (string)e.UserState;
			}

		// Метод обрабатывает завершение процесса
		private void RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
			{
			// Завершение работы исполнителя
			try
				{
				if (e.Result != null)
					{
					result = e.Result.ToString ();
					executionResult = int.Parse (e.Result.ToString ());
					}
				}
			catch
				{
				executionResult = -100;
				}
			bw.Dispose ();

			// Закрытие окна
			allowClose = true;
			this.Close ();
			}

		// Кнопка инициирует остановку процесса
		private void AbortButton_Click (object sender, EventArgs e)
			{
			bw.CancelAsync ();
			}

		// Образец метода, выполняющего длительные вычисления
		private void DoWork (object sender, DoWorkEventArgs e)
			{
			// Собственно, выполняемый процесс
			for (int i = 0; i < ProgressBarSize; i++)
				{
				System.Threading.Thread.Sleep (500);
				((BackgroundWorker)sender).ReportProgress (i);  // Возврат прогресса

				// Завершение работы, если получено требование от диалога
				if (((BackgroundWorker)sender).CancellationPending)
					{
					e.Cancel = true;
					return;
					}
				}

			// Завершено
			e.Result = null;
			}

		// Закрытие формы
		private void HardWorkExecutor_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = !allowClose;
			DrawingTimer.Enabled = false;

			if (g != null)
				g.Dispose ();
			if (gp != null)
				gp.Dispose ();
			if (progress != null)
				progress.Dispose ();
			if (frameGreenGrey != null)
				frameGreenGrey.Dispose ();
			if (frameBack != null)
				frameBack.Dispose ();
			}

		// Отрисовка прогресс-бара
		private void DrawingTimer_Tick (object sender, EventArgs e)
			{
			// Отрисовка текущей позиции
			int recalcPercentage = (int)(oldPercentage + (newPercentage - oldPercentage) / 4);
			gp.DrawImage (frameGreenGrey, currentXOffset, 0);
			gp.DrawImage (frameBack, -9 * this.Width / 4, 0);
			gp.DrawImage (frameBack, recalcPercentage *
				(progress.Width - progress.Height) / ProgressBarSize - this.Width / 4, 0);
			oldPercentage = recalcPercentage;

			g.DrawImage (progress, 18, StateLabel.Top + StateLabel.Height + 10);
			// Почему 18? Да хрен его знает. При ожидаемом x = 10 получается левое смещение

			// Смещение
			if (currentXOffset++ >= -2 * this.Width / 4)
				currentXOffset = -4 * this.Width / 4;
			}
		}
	}
