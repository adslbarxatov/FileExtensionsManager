# File extensions manager: user guide
> **ƒ** &nbsp;RD AAOW FDL; 27.04.2023; 0:44



### Page contents

- [General information](#general-information)
- [Download links](https://adslbarxatov.github.io/DPArray#file-extensions-manager)
- [Русская версия](https://adslbarxatov.github.io/FileExtensionsManager/ru)

---

### General information

Utility allows you to store and apply custom settings for file extensions registered in operating system. List of records
displays registry mappings stored in currently selected base of records.

Green :green_square: indicates entries that already exist in Windows registry,
blue :large_blue_circle: indicates entries that partially correspond to user settings,
and red :red_circle: indicates missing entries.
If record is marked as deleted, then entries that are absent in registry are marked in green, otherwise they are marked in red.
Presence of grey :white_circle: entries may indicate a lack of access to Windows registry.

You can change record by double-clicking on corresponding line, and add it using corresponding button. In both cases,
information is edited in a special window, and their correctness is controlled by the application. You can also add entries
from Windows registry file.

You can apply entries one at a time or all at once (all that not applied). The “Exit” button saves all changes in all bases.
If changes need to be undone, the utility must be closed with a cross in the upper right corner of window.

Version 1.3 adds quick registration of file extension. Registry entries are created in currently selected base that are
necessary for correct mapping of the extension and the application for working with it. In addition, utility allows you to
select an icon for the created file type using graphical interface. Function of icons viewing contained in executables,
libraries, control panel components or icons is available from the main interface.

For now, utility only works with entries in registry branch `HKEY_CLASSES_ROOT` and only with parameter types `REG_SZ`,
`REG_DWORD`, and `REG_QWORD`
