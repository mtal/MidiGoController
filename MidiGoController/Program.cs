using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using MidiGoController.Properties;

namespace MidiGoController
{
    class Program
    {
        static KeyboardState state;
        static CursorState cursor;
        static KeyBindings bindings;
        static void Main(string[] args)
        {

            state = new KeyboardState();
            Console.WriteLine("Keyboard state initialized");
            //Load configs
            try
            {
                cursor = new CursorState(Settings.Default.cursorSettingFile);
                cursor.Move(0, 0);
                Console.WriteLine("Cursor state initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading cursor config");
                Console.ReadLine();
                return;
            }
            try
            {
                bindings = new KeyBindings(Settings.Default.keyBindingFile);
                Console.WriteLine("Keybindings loaded");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading keybindings");
                Console.ReadLine();
                return;
            }
            var deviceList = InputDevice.GetAll().ToList();
            //If there are no midi devices fall back into a demo mode
            if(deviceList.Count == 0)
            {
                Console.WriteLine("No midi input device found");
                Console.ReadLine();
                Console.WriteLine("Doing cursor move demo");
                cursor.Move(1, 0);
                Console.ReadLine();
                cursor.Move(0, 1);
                Console.ReadLine();
                cursor.Move(-1, 0);
                Console.ReadLine();
                cursor.Move(0, -1);
                Console.ReadLine();
                Console.WriteLine("Doing binding test");
                Console.WriteLine("Pressing key 62");
                Console.ReadLine();
                state.PressKey(64);
                bindings.ProcessState(state, cursor);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Using midi device:");
            //Get first available midi device and start event loop, wait for keypress to avoid quitting the program without busy loop
            for (int i = 0; i < deviceList.Count; i++)
            {
                try
                {
                    Console.WriteLine(deviceList[i].ProductIdentifier);
                    Console.WriteLine(deviceList[i].Name);
                    using (var inputDevice = deviceList[i])
                    {
                        inputDevice.EventReceived += OnEventReceived;
                        inputDevice.StartEventsListening();

                        Console.ReadLine();
                    }
                }
                catch(Exception e)
                {

                }
            }
        }
        private static void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            var midiDevice = (MidiDevice)sender;

            NoteEvent ev = e.Event as NoteEvent;
            if (ev != null)
            {
                if (e.Event.EventType == MidiEventType.NoteOn && ev.Velocity>0)
                {
                    Console.WriteLine("Key " + ev.NoteNumber.ToString() + " pressed");
                    state.PressKey(ev.NoteNumber);
                    //Process keybinding events when a key is pressed
                    bindings.ProcessState(state, cursor);
                }
                else if(ev.Velocity == 0)
                {
                    Console.WriteLine("Key " + ev.NoteNumber.ToString() + " released");
                    state.ReleaseKey(ev.NoteNumber);
                }
            }
        }
    }
}
