using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiGoController
{
    class KeyboardState
    {
        bool[] keyPressed = new bool[256];
        //Remembers last button pressed to only trigger bindings that match the most recently pressed button
        short risingEdge = 0;
        public void PressKey(short key)
        {
            //set rising edge
            risingEdge = key;
            this.keyPressed[key] = true;
        }

        public void ReleaseKey(short key)
        {
            this.keyPressed[key] = false;
        }

        public bool KeysDown(short[] keys)
        {
            bool hasRisingEdge = false;
            foreach (var key in keys)
            {
                if (!this.keyPressed[key])
                    return false;
                if (key == risingEdge)
                    hasRisingEdge = true;
            }
            //Only return true if one of the keys being queried is the most recently pressed button
            return hasRisingEdge;
        }
    }
}
