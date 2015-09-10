MemHack
Disclaimer: The first thing I have to say is use this at your own risk. This program's purpose is to modify the memory of other running programs. This is a dangerous thing to do! So if you fuck up your machine or do something else with horrendous repercussions, you've been warned and I'm not responsible. 

This little program was inspired by Lionheart. Frankly I hate the combat system in it and it got in the way of the story. Even after ubering up my character, some combat situations were still unbalanced. It's like you weren't ever really meant to leave Barcelona. I'm not finished the game yet, but at least I'm making progress instead of moving one unit, seeing if any monsters notice me, attacking it a bit, running away, attack a bit more, run away, etc with healing bouts in between. Also it seems that the game becomes much more playable if you not only put skill points into combat instead of the social/thieving skills which was the direction I took, but dump craploads into magic, cause your mana pool grows based on the number of skill points invested in magic based skills. 

MemHack was also inspired by Cheat32. However, Cheat32 seems to have a bug where when you set a value for an address, the value is used as the address and the value, not the address that you selected. You could be unexpectedly setting the value at some other region of the game much to the surprise of the game and player. 

There isn't much documentation to it other than what you see here. The program is pretty simple, choose a running application, click select, it will show you a screen with a Find First, Find Next, and Set buttons. Find First is for the initial search, after that you change the value in the game and do a Find Next for the new value. Repeat until you have one address left. I'd recommend changing the value a couple more times to verify that indeed is the address you are looking for. Select the address then in the address list and click Set. Set it to the new value you want. The new value should appear in the address list promptly. 

Both the process list and the address list periodically update themselves (currently 1 second and 2 seconds respectively). 

Some other notes: 

If you exit a process while you are trying to edit it, any found addresses will disappear (since that memory gets deallocated and it is a requirement that the memory in question exists). In fact if the memory is deallocated while the game is running for a found address, it should disappear as well. 
Some of the dialog and progress bar placement is still a little inconsistent. 
There isn't an option to freeze a value a la Cheat32. I might do that one day if there is demand for it. 
I filter the modifiable regions of memory based on whether they are actually memory regions (not memory mapped files for example), that they are read/write, and they are in use by the process being modified. 
Requirements: 

.NET Framework 1.1 (haven't tried it on 1.0, it might work, no guarantees) 
psapi.dll 
Known working OS's: 

Windows XP (all I've tried it on at this point) 
10/17/2003: Some fixes went in to clean up some UI problems I noticed in the intial dialog where if a window popped up for an application and changed the title of the window, it wouldn't be reflected in the dialog box listing running programs. I also did a little touch up of the progress bar too during Find Next. If you see any wierdness their (like the progress bar not going away) and have consistently reproducable steps that I can use to figure out what might be failing there, let me know. I've also included an installer and the source code. 

10/19/2003: Made the address list and process list windows resizable. This makes them dramatically more useful. Included the current search size in the address list window. 

06/12/2005: Made some massive changes in numerous algorithms, made it localisable, addressed some bugs, added a help file and tool tips. 

