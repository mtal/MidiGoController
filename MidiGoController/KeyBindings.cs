using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.ExceptionServices;

namespace MidiGoController
{
    class KeyBinding
    {
        public bool click;
        public int xDiff, yDiff;
        List<short[]> bindings;
        public KeyBinding(string line)
        {
            //Split line by commas
            var parts = line.Split(',');
            //First three parameters determine wether a mouseclick should be sent and the x and y movement on the board grid
            click = bool.Parse(parts[0]);
            xDiff = int.Parse(parts[1]);
            yDiff = int.Parse(parts[2]);
            //Split binding string by semicolons for alternative key bindings
            var bindingStrings = parts[3].Split(';');
            bindings = new List<short[]>();
            foreach(var bindingString in bindingStrings)
            {
                //Split individual bindings by + characters to enable requiring multiple keys to be pressed at once
                bindings.Add(bindingString.Split('+').Select((s) => short.Parse(s)).ToArray());
            }
        }

        public bool MatchesState(KeyboardState state)
        {
            //Check if any of the bindings match the current keyboard state
            foreach (var binding in bindings)
            {
                if (state.KeysDown(binding))
                    return true;
            }
            return false;
        }
    }
    class KeyBindings
    {
        List<KeyBinding> bindings;
        public KeyBindings(string fileName)
        {
            bindings = new List<KeyBinding>();
            //Read config file line by line and convert each line to a binding
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    bindings.Add(new KeyBinding(line));
                }
            }
        }

        public void ProcessState(KeyboardState state, CursorState cursor)
        {
            foreach(var binding in bindings)
            {
                //Go through all bindings sequentially and execute their effects if they match the current keyboard state
                if(binding.MatchesState(state))
                {
                    if (binding.click)
                        cursor.Click();
                    cursor.Move(binding.xDiff, binding.yDiff);
                }
            }
        }
    }
}
