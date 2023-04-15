using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Типы сообщений для пользователя
	/// </summary>
	public enum RDMessageTypes
		{
		/// <summary>
		/// Информационное (обычное)
		/// </summary>
		Information,

		/// <summary>
		/// Вопрос к пользователю
		/// </summary>
		Question,

		/// <summary>
		/// Предупреждение
		/// </summary>
		Warning,

		/// <summary>
		/// Ошибка
		/// </summary>
		Error,

		/// <summary>
		/// Сообщение об успешном результате
		/// </summary>
		Success,

		/// <summary>
		/// Окно выбора языка интерфейса
		/// </summary>
		LanguageSelector,
		}

	/// <summary>
	/// Возможные результирующие кнопки сообщения для пользователя
	/// </summary>
	public enum RDMessageButtons
		{
		/// <summary>
		/// Кнопка не была нажата (окно закрыто иным способом)
		/// </summary>
		NotSelected = -1,

		/// <summary>
		/// Первая кнопка
		/// </summary>
		ButtonOne = 1,

		/// <summary>
		/// Вторая кнопка
		/// </summary>
		ButtonTwo = 2,

		/// <summary>
		/// Третья кнопка
		/// </summary>
		ButtonThree = 3
		}

	/// <summary>
	/// Форма является универсальным интерфейсом сообщений для пользователя
	/// </summary>
	public partial class RDMessageForm: Form
		{
		// Прочее
		private bool exitAllowed = false;

		/// <summary>
		/// Возвращает тип окна сообщения
		/// </summary>
		public RDMessageTypes WindowType
			{
			get
				{
				return windowType;
				}
			}
		private RDMessageTypes windowType = RDMessageTypes.Information;

		/// <summary>
		/// Возвращает выбранную кнопку в сообщении
		/// </summary>
		public RDMessageButtons ResultButton
			{
			get
				{
				return resultButton;
				}
			}
		private RDMessageButtons resultButton = RDMessageButtons.NotSelected;

		/// <summary>
		/// Конструктор. Запускает простейшую форму сообщения для пользователя (с таймаутом автозакрытия)
		/// </summary>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		/// <param name="Timeout">Таймаут (в миллисекундах), по истечении которого 
		/// сообщение автоматически закроется (от 100 до 60000 мс)</param>
		public RDMessageForm (RDMessageTypes Type, string Message, uint Timeout)
			{
			uint to = Timeout;
			if (to < 100)
				to = 100;
			if (to > 60000)
				to = 60000;

			RDMessageFormInit (Type, Message, "-", null, null, SupportedLanguages.en_us, to);
			}

		/// <summary>
		/// Конструктор. Запускает форму выбора языка приложения
		/// </summary>
		/// <param name="CurrentInterfaceLanguage">Текущий язык интерфейса</param>
		public RDMessageForm (SupportedLanguages CurrentInterfaceLanguage)
			{
			RDMessageFormInit (RDMessageTypes.LanguageSelector,
				Localization.GetDefaultText (LzDefaultTextValues.Message_LanguageSelection),
				Localization.GetDefaultText (LzDefaultTextValues.Button_Apply),
				Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel),
				null, CurrentInterfaceLanguage, 0);
			}

		/// <summary>
		/// Конструктор. Запускает простейшую форму сообщения для пользователя (с одной кнопкой подтверждения)
		/// </summary>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message)
			{
			RDMessageFormInit (Type, Message, Localization.GetDefaultText (LzDefaultTextValues.Button_OK),
				null, null, SupportedLanguages.en_us, 0);
			}

		/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с одной кнопкой
		/// </summary>
		/// <param name="ButtonOneName">Название кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message, string ButtonOneName)
			{
			RDMessageFormInit (Type, Message, ButtonOneName, null, null, SupportedLanguages.en_us, 0);
			}

		/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с двумя кнопками
		/// </summary>
		/// <param name="ButtonOneName">Название первой кнопки</param>
		/// <param name="ButtonTwoName">Название второй кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message, string ButtonOneName, string ButtonTwoName)
			{
			RDMessageFormInit (Type, Message, ButtonOneName, ButtonTwoName, null, SupportedLanguages.en_us, 0);
			}

		/// <summary>
		/// Конструктор. Запускает форму сообщения для пользователя с тремя кнопками
		/// </summary>
		/// <param name="ButtonOneName">Название первой кнопки</param>
		/// <param name="ButtonTwoName">Название второй кнопки</param>
		/// <param name="ButtonThreeName">Название третьей кнопки</param>
		/// <param name="Message">Сообщение для пользователя</param>
		/// <param name="Type">Тип создаваемого окна</param>
		public RDMessageForm (RDMessageTypes Type, string Message, string ButtonOneName, string ButtonTwoName,
			string ButtonThreeName)
			{
			RDMessageFormInit (Type, Message, ButtonOneName, ButtonTwoName, ButtonThreeName,
				SupportedLanguages.en_us, 0);
			}

		// Основная инициализация формы
		private void RDMessageFormInit (RDMessageTypes Type, string Message, string ButtonOneName, string ButtonTwoName,
			string ButtonThreeName, SupportedLanguages CurrentInterfaceLanguage, uint Timeout)
			{
			// Инициализация
			InitializeComponent ();

			// Запрет на побочное определение (не через собственный конструктор)
			windowType = Type;
			if (windowType == RDMessageTypes.LanguageSelector)
				{
				LanguagesCombo.Visible = true;
				LanguagesCombo.Items.AddRange (Localization.LanguagesNames);
				try
					{
					LanguagesCombo.SelectedIndex = (int)CurrentInterfaceLanguage;
					}
				catch
					{
					LanguagesCombo.SelectedIndex = 0;
					}
				}

			this.Text = ProgramDescription.AssemblyTitle;

			if (!string.IsNullOrWhiteSpace (Message))
				{
				Label01.Text = Message.Replace ("\n", "\r\n").Replace ("\r\r", "\r");
				Label01.SelectionLength = 0;

				if (windowType != RDMessageTypes.LanguageSelector)
					{
					// Определение ширины
					if ((Message.Replace ("\n", "").Replace ("\r", "").Length > 150) &&
						(TextRenderer.MeasureText (Message, Label01.Font).Width > Label01.Width))
						{
						Label01.Width += this.Width;
						this.Width *= 2;
						}

					// Определение высоты
					Label01.Height = (Label01.GetLineFromCharIndex (int.MaxValue) + 2) *
						TextRenderer.MeasureText ("X", Label01.Font).Height;

					this.Height = Label01.Height;
					if (Timeout > 0)
						{
						this.Height += 17;
						}
					else
						{
						this.Height += 57;
						Button01.Top = Button02.Top = Button03.Top = this.Height - 48;
						}
					}
				}

			if (!string.IsNullOrWhiteSpace (ButtonOneName))
				{
				Button01.Text = ButtonOneName;
				if (Button01.Text.EndsWith ("\t"))
					{
					Button01.TabStop = false;
					Button01.Text = Button01.Text.Trim ();
					}
				}
			else
				{
				Button01.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_OK);
				}

			if (!string.IsNullOrWhiteSpace (ButtonTwoName))
				{
				Button02.Text = ButtonTwoName;
				if (Button02.Text.EndsWith ("\t"))
					{
					Button02.TabStop = false;
					Button02.Text = Button02.Text.Trim ();
					}

				if (!string.IsNullOrWhiteSpace (ButtonThreeName))
					{
					Button03.Text = ButtonThreeName;
					if (Button03.Text.EndsWith ("\t"))
						{
						Button03.TabStop = false;
						Button03.Text = Button03.Text.Trim ();
						}
					}
				}

			switch (windowType)
				{
				case RDMessageTypes.Information:
				default:
					SystemSounds.Asterisk.Play ();
					this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultBackground);
					break;

				case RDMessageTypes.LanguageSelector:
					this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultBackground);
					break;

				case RDMessageTypes.Question:
					SystemSounds.Question.Play ();
					this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.QuestionMessage);
					break;

				case RDMessageTypes.Warning:
					SystemSounds.Exclamation.Play ();
					this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.WarningMessage);
					break;

				case RDMessageTypes.Error:
					SystemSounds.Hand.Play ();
					this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.ErrorMessage);
					break;

				case RDMessageTypes.Success:
					SystemSounds.Asterisk.Play ();
					this.BackColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.SuccessMessage);
					break;
				}

			AlignButtons ();
			Label01.ForeColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultText);
			Label01.BackColor = this.BackColor;

			if (windowType == RDMessageTypes.LanguageSelector)
				{
				LanguagesCombo.ForeColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultText);
				LanguagesCombo.BackColor = this.BackColor;
				}

			if (Timeout > 0)
				MainTimer.Interval = (int)Timeout;

			// Запуск
			this.StartPosition = FormStartPosition.CenterParent;
			this.ShowDialog ();
			}

		private void RDMessageForm_Shown (object sender, EventArgs e)
			{
			// Отмена центрирования на родительское окно, если это невозможно
			if (this.Left == 0)
				this.CenterToScreen ();

			// Запуск отрисовки
			CreateBackground (this);

			// Запуск таймера, если предусмотрен
			if (MainTimer.Interval > 75)
				MainTimer.Enabled = true;
			}

		// Выбор размера
		private void Button01_Click (object sender, EventArgs e)
			{
			// Извлечение номера
			string buttonName = ((Button)sender).Name;
			uint buttonNumber = uint.Parse (buttonName.Substring (buttonName.Length - 2, 2));
			resultButton = (RDMessageButtons)buttonNumber;

			// Возврат
			if ((windowType == RDMessageTypes.LanguageSelector) && (resultButton == RDMessageButtons.ButtonOne))
				Localization.CurrentLanguage = (SupportedLanguages)LanguagesCombo.SelectedIndex;

			// Завершение
			exitAllowed = true;
			this.Close ();
			}

		// Выравнивание кнопок
		private void AlignButtons ()
			{
			// Настройка
			if (Button01.Text != "-")
				{
				Button01.Enabled = Button01.Visible = true;
				Button01.BackColor = this.BackColor;
				Button01.ForeColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultText);
				}
			if (Button01.Enabled && (Button02.Text != "-"))
				{
				Button02.Enabled = Button02.Visible = true;
				Button02.BackColor = this.BackColor;
				Button02.ForeColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultText);
				}
			if (Button02.Enabled && (Button03.Text != "-"))
				{
				Button03.Enabled = Button03.Visible = true;
				Button03.BackColor = this.BackColor;
				Button03.ForeColor = RDGenerics.GetInterfaceColor (RDInterfaceColors.DefaultText);
				}

			// Выравнивание
			if (!Button02.Enabled)
				{
				Button01.Left = (this.Width - Button01.Width) / 2;
				return;
				}

			if (!Button03.Enabled)
				{
				Button01.Left = this.Width / 2 - Button01.Width - 3;
				Button02.Left = this.Width / 2 + 3;
				return;
				}

			Button02.Left = (this.Width - Button02.Width) / 2;
			Button01.Left = Button02.Left - Button01.Width - 6;
			Button03.Left = Button02.Left + Button02.Width + 6;
			}

		// Запрет на закрытие окна "крестиком"
		private void RDMessageForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = !exitAllowed;
			}

		/// <summary>
		/// Метод формирует фон диалогового окна для указанной формы
		/// </summary>
		/// <param name="DialogForm">Форма для создаия фона</param>
		public static void CreateBackground (Form DialogForm)
			{
			// Начало
			const int roundingSize = 20;
			Bitmap bm = new Bitmap (DialogForm.Width, DialogForm.Height);
			Graphics gr = Graphics.FromImage (bm);

			// Отрисовка фона
			SolidBrush br = new SolidBrush (DialogForm.TransparencyKey);
			int w = DialogForm.Width;
			int h = DialogForm.Height;

			gr.FillRectangle (br, 0, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, w - roundingSize / 2, 0, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, 0, h - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			gr.FillRectangle (br, w - roundingSize / 2, h - roundingSize / 2, roundingSize / 2, roundingSize / 2);
			br.Dispose ();

			br = new SolidBrush (DialogForm.BackColor);
			gr.FillEllipse (br, 0, 0, roundingSize, roundingSize);
			gr.FillEllipse (br, w - roundingSize - 1, 0, roundingSize, roundingSize);
			gr.FillEllipse (br, 0, h - roundingSize - 1, roundingSize, roundingSize);
			gr.FillEllipse (br, w - roundingSize - 1, h - roundingSize - 1,
				roundingSize, roundingSize);

			// Отрисовка контура
			Pen pDark = new Pen (Color.FromArgb (2 * DialogForm.BackColor.R / 3, 2 * DialogForm.BackColor.G / 3,
				2 * DialogForm.BackColor.B / 3), 1);
			w = DialogForm.Width - (int)pDark.Width;
			h = DialogForm.Height - (int)pDark.Width;

			gr.DrawLine (pDark, roundingSize / 2, 0, w - roundingSize / 2, 0);
			gr.DrawArc (pDark, w - roundingSize, 0, roundingSize, roundingSize, 0, -90);
			gr.DrawLine (pDark, w, roundingSize / 2, w, h - roundingSize / 2);
			gr.DrawArc (pDark, w - roundingSize, h - roundingSize, roundingSize, roundingSize, 0, 90);

			gr.DrawLine (pDark, roundingSize / 2, h, w - roundingSize / 2, h);
			gr.DrawArc (pDark, 0, h - roundingSize, roundingSize, roundingSize, 180, -90);
			gr.DrawLine (pDark, 0, roundingSize / 2, 0, h - roundingSize / 2);
			gr.DrawArc (pDark, 0, 0, roundingSize, roundingSize, 180, 90);

			// Финализация
			br.Dispose ();
			gr.Dispose ();
			pDark.Dispose ();
			DialogForm.BackgroundImage = bm;
			}

		// Автоматическое закрытие по таймеру
		private void MainTimer_Tick (object sender, EventArgs e)
			{
			resultButton = RDMessageButtons.ButtonOne;
			exitAllowed = true;
			this.Close ();
			}
		}
	}
