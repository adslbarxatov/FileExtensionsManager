# FileExtensionsManager v 1.3
A tool for managing Windows registry settings for file extensions

Инструмент управления пользовательскими настройками расширений файлов в Windows
#
Программа позволяет хранить и применять пользовательские настройки расширений файлов, зарегистрированных в системе.
В списке записей отображаются реестровые сопоставления, хранящиеся в текущей выбранной базе записей. Зелёным цветом
обозначены записи, уже имеющиеся в реестре Windows, синим – записи, неполностью соответствующие пользовательским
настройкам, а красным – отсутствующие записи. Если запись помечена как удаляемая, то зелёным помечаются отсутствующие в
реестре записи, а красным – присутствующие. Наличие серых записей может говорить об отсутствии доступа к реестру Windows.

Изменить запись можно по двойному щелчку мыши на соответствующей строке, добавить – с помощью соответствующей кнопки. 
В обоих случаях сведения редактируются в специальном окне, а их корректность контролируется программой. Добавить записи 
можно также и из файла реестра Windows.

Применять записи можно по одной или все сразу (все неприменённые). Кнопка «Выход» 
сохраняет все изменения во всех базах. Если изменения нужно отменить, программу нужно закрыть крестиком в верхнем правом 
углу окна.

В версии 1.3 добавлена быстрая регистрация расширения файла; при этом в текущей выбранной базе создаются записи реестра,
необходимые для корректного сопоставления расширения и программы для работы с ним. Кроме того, программа позволяет
выбрать значок для создаваемого типа файлов с помощью графического интерфейса. Функция просмотра значков (иконок),
содержащихся в файле приложения, библиотеки, компонента панели управления или значка, доступна и из основного
интерфейса программы.

На данный момент программа работает только с записями в ветке реестра HKEY_CLASSES_ROOT и только с типами параметров 
REG_SZ, REG_DWORD и REG_QWORD
#
Needs Windows XP and newer, Framework 4.0 and newer. Interface language: ru_ru

Требуется ОС Windows XP и новее, Framework 4.0 и новее. Язык интерфейса: ru_ru
