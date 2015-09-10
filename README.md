MemHack

Disclaimer: The first thing I have to say is use this at your own risk. This program's purpose is to modify the memory of other running programs. This is a dangerous thing to do! So if you fuck up your machine or do something else with horrendous repercussions, you've been warned and I'm not responsible. 

There isn't much documentation to it other than what you see here. The program is pretty simple, choose a running application, click select, it will show you a screen with a Find First, Find Next, and Set buttons. Find First is for the initial search, after that you change the value in the game and do a Find Next for the new value. Repeat until you have one address left. I'd recommend changing the value in your application a couple more times to verify that indeed is the address you are looking for (MemHack will poll the memory periodically and should update to the new value pretty quickly). Select the address then in the address list and click Set. Set it to the new value you want. The new value should appear in the address list promptly. 

Both the process list and the address list periodically update themselves (currently 1 second and 2 seconds respectively). 

Some other notes: 
* If you exit a process while you are trying to edit it, any found addresses will disappear (since that memory gets deallocated and it is a requirement that the memory in question exists). In fact if the memory is deallocated while the game is running for a found address, it should disappear as well. 
* Some of the dialog and progress bar placement is still a little inconsistent. 
* There isn't an option to freeze a value a la Cheat32. I might do that one day if there is demand for it. 
* I filter the modifiable regions of memory based on whether they are actually memory regions (not memory mapped files for example), that they are read/write, and they are in use by the process being modified. 

Requirements to run: 
* .NET Framework 4.5.2
* psapi.dll 

Requirements for source:
* Visual Studio 2015 Community

Known working OS's: 
* Windows 10
