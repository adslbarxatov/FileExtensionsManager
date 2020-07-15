# FileExtensionsManager v 1.4u

A tool for managing Windows registry settings for file extensions

Инструмент управления пользовательскими настройками расширений файлов в Windows

## Описание

Программа позволяет хранить и применять пользовательские настройки расширений файлов, зарегистрированных в операционной системе.
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

В версии 1.3 добавлена быстрая регистрация расширения файла. В текущей выбранной базе создаются записи реестра,
необходимые для корректного сопоставления расширения и программы для работы с ним. Кроме того, утилита позволяет
выбрать значок для создаваемого типа файлов с помощью графического интерфейса. Функция просмотра значков (иконок),
содержащихся в файле приложения, библиотеки, компонента панели управления или значка, доступна и из основного
интерфейса программы.

На данный момент программа работает только с записями в ветке реестра HKEY_CLASSES_ROOT и только с типами параметров
REG_SZ, REG_DWORD и REG_QWORD



## About

Utility allows you to store and apply custom settings for file extensions registered in operating system. List of records
displays registry mappings stored in currently selected base of records. Green indicates entries that already exist in
Windows registry, blue indicates entries that partially correspond to user settings, and red indicates missing entries.
If record is marked as deleted, then entries that are absent in registry are marked in green, otherwise they are marked
in red. Presence of gray entries may indicate a lack of access to Windows registry.

You can change record by double-clicking on corresponding line, and add it using corresponding button. In both cases,
information is edited in a special window, and their correctness is controlled by the application. You can also add entries
from Windows registry file.

You can apply entries one at a time or all at once (all that not applied). The “Exit” button saves all changes in all bases.
If changes need to be undone, the utility must be closed with a cross in the upper right corner of window.

Version 1.3 adds quick registration of file extension. Registry entries are created in currently selected base that are
necessary for correct mapping of the extension and the application for working with it. In addition, utility allows you to
select an icon for the created file type using graphical interface. Function of icons viewing contained in executables,
libraries, control panel components or icons is available from the main interface.

For now, utility only works with entries in registry branch HKEY_CLASSES_ROOT and only with parameter types REG_SZ,
REG_DWORD, and REG_QWORD



## Requirements / Требования

Needs Windows XP and newer, Framework 4.0 and newer. Interface language: ru_ru, en_us

Требуется ОС Windows XP и новее, Framework 4.0 и новее. Язык интерфейса: ru_ru, en_us



## Development policy and EULA / Политика разработки и EULA

This [Policy (ADP)](https://vk.com/@rdaaow_fupl-adp), its positions, conclusion, EULA and application methods
describes general rules that we follow in all of our development processes, released applications and implemented
ideas.
**It must be acquainted by participants and users before using any of laboratory's products.
By downloading them, you agree to this Policy**

Данная [Политика (ADP)](https://vk.com/@rdaaow_fupl-adp), её положения, заключение, EULA и способы применения
описывают общие правила, которым мы следуем во всех наших процессах разработки, вышедших в релиз приложениях
и реализованных идеях.
**Обязательна к ознакомлению всем участникам и пользователям перед использованием любого из продуктов лаборатории.
Загружая их, вы соглашаетесь с этой Политикой**
