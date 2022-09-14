# <img alt="Reminders icon" height="32" src="icon.png" title="Reminders" width="32" align="center"/> Reminders
**_Reminders_ is a command line C# app for creating and managing reminders**.  
It is feature-rich while being lightweight, not relying on any external libraries. 
Reminders also has the unique feature of printing due reminders in the console while the user is typing something at the same time.

## Features
- Create, read, update and delete reminders
- Due reminders will be immediately shown in the console - even if you are typing at that exact moment. Alternatively, a notification pop-up window can be shown
- Clean, structured and tidy text interface
- Update only certain properties of reminders or directly edit their text, directly in the console
- List reminders for all kinds time intervals or search for multiple terms by using powerful parameter options
- Mark reminders as read so they will not be shown anymore, but are also not permanently deleted
- Short forms of commands available for less typing and more efficiency
- Persistent settings in a separate config file
- Lightweight and small program with no external libraries or any other bloat or dependencies outside .net core

## Installation
Just download the Reminders.exe file and start it, that's it!  
Two additional files will be created in the same directory: a data.rmdr file which stores the reminders and a config.txt which contains several settings. 
The data file can be moved to any other directory as long as its current location is saved in the config file ("path"). The config file always stays in the same folder as the executable.

## Instructions / Commands
A set of commands is used to manage reminders and control the program.
Here is a list of all available commands, their parameters and descriptions:

<details><summary>List of commands</summary>

- `create` date time timespan content
  - creates a new reminder with mandatory and optional values, then shows its id
- `delete` id
  - deletes a reminder permanently by id
- `update` id date time timespan content
  - update a reminder by id, you can optionally update every individual property of it
- `edit` id
  - you can directly edit the content text of the reminder, press enter to save it
- `read` id
  - marks a reminder as read so it does not pop up as due every time the program is started
- `show` id / status startdate enddate / date / timespan
  - shows either the complete details of one reminder by id or a list of reminders defined by a date, between two dates or in a timespan from now
- `search` terms
  - lets you search for reminders containing specific terms, multiple terms have to be in "" and are separated by spaces
- `config` setting / reset
  - shows the config and values, lets you set the values or resets the whole config
- `help`: shows a help page with a list of available commands
- `commands`: shows a detailed list of commands with possible parameters
- `exit`: stops the program and closes the window

Below is also a detailed list of all possible and alternative parameters of the above commands:

<details><summary>Detailed list of all possible parameters</summary>

- `create[/c] {dd(.)mm(.)(yy)yy} ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) {text}`
- `delete[/del/d] {id}`
- `update[/u] {id} ({dd(.)mm(.)(yy)yy}) ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) ({text})`
- `edit[/e] {id}`
- `read[/r] {id}`
- `show[/s] ([un]read[/[u/]r]) {dd(.)mm(.)(yy)yy)}[/today[/t]/tomorrow[/to]/yesterday[/ye](last[/l])/week[/w]/month[/m]/year[/y]/{x}d/{x}w/{x}y/]`
  - `/ show[/s] ([un]read[/[u/]r]) (s{dd(.)mm(.)(yy)yy)}) (e{dd(.)mm(.)(yy)yy)})`
  - `/ show[/s] {id}`
- `search[/se] {term} (\"{term2..n}\")`
- `config[/co/settings] {parameter} {value}`
  - `/ config[/co/settings] reset`
- `exit`

</details>

</details>

## Config
Reminders has several persistent setting which are saved in a config.txt file always located in the same directory as the .exe file.
Here is a list of all parameters, possible values and a short explanation:

<details><summary>List of settings</summary>

- `path = [directory]`: path of the directory where the data.rmdr data file will be saved
- `autostart = [bool]`: whether an autostart shortcut for Reminders will be created
- `upcomingRemindersTime = [-1..n days]`: amount of days reminders will be shown for at startup of program. A value of 0 will show today and a value of -1 all upcoming reminders
- `devMode = [bool]`: activating devmode will shown additional log messages in the console
- `notification = [bool]`: whether a notification window with sound will pop up every time a reminder is due
- `quickEdit = [bool]`: a standard windows console feature which is normally activated and allows for marking, copying, etc. of console text, but interferes with the implemented simultaneousness of console input and output. 
If you decide to reactivate quickedit, make sure to not click anywhere in the console and leaving it alone, as this will freeze the console and not show any due reminders until you "unclick" the console

</details>
