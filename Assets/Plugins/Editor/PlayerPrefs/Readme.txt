PLAYER PREFS EDITOR

Copyright (c) 2013 Fuzzy Logic (info@fuzzy-logic.co.za)

OVERVIEW
- View, edit, add and remove "PlayerPrefs"
- Easy filtering of entries based on key or value


DESCRIPTION
The Player Prefs Inspector is a dockable editor window that will display a list of all "PlayerPrefs" entries associated with your project.
The search filter fields allows you to easily find entries based on name or value.

Any entry can simply be changed by typing in a new value and hitting enter. Changes are immediately applied. Entries can also easily be deleted with a single click.

New entries can be added by choosing a type, specifying the key and the value.

INSTRUCTIONS
- The window can be opened by clicking on Window->PlayerPrefs in the Unity Editor menu bar
- Type in any of the search filter fields to filter values based on that column
- Entry values can be changed while in stopped mode, and are immediately applied
- An entry can be deleted by pressing the "X" button (it will ask you to confirm first!)
- To add an entry, expand the "Add" section, specify the type, key and value, and press the "Add" button
- By default changes are not allowed during Play mode, however this can be enabled via the 
"Editable in Play Mode" checkbox


VERSION HISTORY

v1.4
- "Add" support added
- In some cases the system would see floats as doubles. These are now converted as required.

v1.3
- Fixed hang when encountering unsupported types (only int, float and string are supported)
- When an entry of unsupported type is encountered, the entry is still displayed but its value can't be edited. 
Instead, a message is displayed in the form of "(editing of type X not supported)." If you feel this should be supported, 
please email us at info@fuzzy-logic.co.za with details about the type.

v1.2
- First release