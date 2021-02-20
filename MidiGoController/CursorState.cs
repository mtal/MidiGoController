using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiGoController
{
    class CursorState
    {
        int x, y;
        int maxX, maxY;
        int topX, topY;
        int xStep, yStep;

        public CursorState(string filename)
        {
            x = 0; y = 0;
            using(StreamReader file = new StreamReader(filename))
            {
                //First line is board size
                var line = file.ReadLine();
                var maxParts = line.Split(',');
                maxX = int.Parse(maxParts[0]);
                maxY = int.Parse(maxParts[1]);

                //Second line is top left corner of the board in pixels on the screen
                line = file.ReadLine();
                var topParts = line.Split(',');
                topX = int.Parse(topParts[0]);
                topY = int.Parse(topParts[1]);

                //Third line is gridsize in pixels for the board
                line = file.ReadLine();
                var stepParts = line.Split(',');
                xStep = int.Parse(stepParts[0]);
                yStep = int.Parse(stepParts[1]);
            }
        }

        public void Move(int xdiff, int ydiff)
        {
            x += xdiff;
            y += ydiff;

            //Clamp values to stay within grid limits
            if (x < 0)
                x = 0;
            if (x >= maxX)
                x = maxX-1;
            if (y < 0)
                y = 0;
            if (y >= maxY)
                y = maxY-1;
            //Set mouse position
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(topX + x * xStep, topY + y * yStep);
        }

        public void Click()
        {
            //Execute a click at the current mouse position
            //Update position in case mouse was moved by user since last setting of mouse coordinate
            ClickPoint(topX + x * xStep, topY + y * yStep);
        }

        public static void ClickPoint(int x, int y)
        {
            // Set the cursor position
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
    }
}
