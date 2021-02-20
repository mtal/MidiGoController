# MidiGoController README

## Common
Actions only trigger when a key is pressed, not when one is released.
The program will move your mouse and simulate a mouse click to achieve
placing the mouse and stones. In between playing notes you can still use the mouse normally,
but it will teleport to the board position when you play a key on the midi keyboard.

## Config
In the "MidiGoController.exe.config" file you can set which config files
should be loaded for the cursor and the keybindings.
The idea is that you will probably want different ones for 19x19 and 9x9 or on different servers.

## Cursor config format
###Example
19,19
100,100
20,20

### Description
maxX,maxY
topX,topY
xStep,yStep

First line is maximum number of steps in x and y direction
Second line is top left corner of the go board on the screen
Third line is how far each board coordinate is apart to step

## Keybinding config format
### Example
false,1,0,60
false,-1,0,61
false,0,1,62
false,0,-1,63
true,1,0,64+65;66

### Description
click,xDiff,yDiff,bindings

Each line describes a binding.
A binding can determine how far along the x and y axes the cursor should go and
wether the mouse should click to place a stone.

The bindings part is the most complicated.
If multiple keys need to be pressed at the same time, you can combine them with a plus.
64+65 means that if key 64 AND key 65 are pressed this action will trigger
If you want multiple distinct keys or combinations of keys to be able to trigger actions you can list them up separated by semicolons.
So 65;66;67 would mean that if any of 65,66 or 67 are pressed the action will trigger.