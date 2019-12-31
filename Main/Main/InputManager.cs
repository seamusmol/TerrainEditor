using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Main.Main
{
    public class InputManager : AppState
    {
        private Dictionary<string, Keys> RegisteredKeys = new Dictionary<string, Keys>();
        private Dictionary<string, string> RegisteredButtons = new Dictionary<string, string>();

        public Dictionary<string, Keys> ActiveRegisteredKeys { get; private set; } = new Dictionary<string,Keys>();
        public Dictionary<string, Keys> AllActiveKeys { get; private set; } = new Dictionary<string, Keys>();
        public Dictionary<string, string> ActiveRegisteredMouseButtons { get; private set; } = new Dictionary<string, string>();

        Game1 Game;
        public Vector2 MouseMovement { get; set; } = new Vector2();
        public Vector2 PreviousMouseLocation { get; set; } = new Vector2();

        public float MouseWheelValue { get; set; } = 0.0f;
        public float PreviousMouseWheelValue = 0.0f;

        public bool HasChange = false;

        public bool HasMouseLock { get; private set; } = false;
        public Point LockPosition = new Point();
        
        String FileLocation = "Config";
        
        public InputManager(Game1 MainGame)
        {
            Game = MainGame;
            ImportSettings();
            LockPosition = new Point(Game.GetScreenWidth() / 2, Game.GetScreenHeight() / 2);
        }
        
        public void SetMouseLock(bool Val)
        {
            if (Val != HasMouseLock)
            {
                HasChange = true;
            }
            HasMouseLock = Val;
        }

        public override void Update(GameTime GameTime)
        {
            ActiveRegisteredKeys = new Dictionary<string, Keys>();
            AllActiveKeys = new Dictionary<string, Keys>();
            ActiveRegisteredMouseButtons = new Dictionary<string, string>();
            
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            Point MousePosition = mouse.Position;
            int CX = Game.GetScreenWidth()/2;
            int CY = Game.GetScreenHeight()/2;
            if ( !HasMouseLock)
            {
                //menu
                MouseMovement = new Vector2(MousePosition.X - PreviousMouseLocation.X, MousePosition.Y - PreviousMouseLocation.Y);
                PreviousMouseLocation = new Vector2(MousePosition.X, MousePosition.Y);
            }
            else
            {
                //locked for crosshair/gameplay
                if (HasChange)
                {
                    MouseMovement = new Vector2();
                }
                else
                {
                    MouseMovement = new Vector2(MousePosition.X - CX, MousePosition.Y - CY);
                }
                if (Game.IsActive)
                {
                    Mouse.SetPosition( LockPosition.X, LockPosition.Y);
                }
            }
            HasChange = false;

            MouseWheelValue = PreviousMouseWheelValue - mouse.ScrollWheelValue;
            PreviousMouseWheelValue = mouse.ScrollWheelValue;

            if (!Game.IsActive)
            {
                return;
            }

            Keys[] pressedKeys = keyboard.GetPressedKeys();
            foreach ( Keys Key in pressedKeys)
            {
                if (RegisteredKeys.ContainsValue( Key))
                {
                    string name = RegisteredKeys.First(x => x.Value.Equals( Key)).Key;
                    if (!ActiveRegisteredKeys.ContainsKey(Key.ToString().Substring(0, 1)))
                    {
                        ActiveRegisteredKeys.Add(name, Key);
                    }
                }
                //collect text
                if ( !AllActiveKeys.ContainsKey(Key.ToString().Substring(0, 1)))
                {
                    AllActiveKeys.Add(Key.ToString().Substring(0, 1), Key);
                }
            }
            
            if (RegisteredButtons.ContainsValue("Mouse.LeftButton") && mouse.LeftButton == ButtonState.Pressed)
            {
                ActiveRegisteredMouseButtons.Add(RegisteredButtons.First(x => x.Value == "Mouse.LeftButton").Key, "Mouse.LeftButton");
            }
            if (RegisteredButtons.ContainsValue("Mouse.RightButton") && mouse.RightButton == ButtonState.Pressed)
            {
                ActiveRegisteredMouseButtons.Add(RegisteredButtons.First(x => x.Value == "Mouse.RightButton").Key, "Mouse.RightButton");
            }
            if (RegisteredButtons.ContainsValue("Mouse.MiddleButton") && mouse.MiddleButton == ButtonState.Pressed)
            {
                ActiveRegisteredMouseButtons.Add(RegisteredButtons.First(x => x.Value == "Mouse.MiddleButton").Key, "Mouse.MiddleButton");
            }
            if (RegisteredButtons.ContainsValue("Mouse.Button4") && mouse.XButton1 == ButtonState.Pressed)
            {
                ActiveRegisteredMouseButtons.Add(RegisteredButtons.First(x => x.Value == "Mouse.Button4").Key, "Mouse.Button4");
            }
            if (RegisteredButtons.ContainsValue("Mouse.Button5") && mouse.XButton2 == ButtonState.Pressed)
            {
                ActiveRegisteredMouseButtons.Add(RegisteredButtons.First(x => x.Value == "Mouse.Button5").Key, "Mouse.Button5");
            }
        }
        
        public Point GetMousePosition()
        {
            return Mouse.GetState().Position;
        }

        public bool HasInput()
        {
            return ActiveRegisteredMouseButtons.Count > 0 || ActiveRegisteredKeys.Count > 0;
        }

        public bool HasRegisteredMouseInput(string Name)
        {
            return ActiveRegisteredMouseButtons.ContainsKey(Name);
        }

        public bool HasRegisteredInput(string Name)
        {
            return ActiveRegisteredKeys.ContainsKey(Name);
        }
        
        public bool HasKey( string Name)
        {
            return AllActiveKeys.ContainsKey(Name);
        }

        public void ImportSettings()
        {
            DirectoryInfo DirectoryFolder = new DirectoryInfo(FileLocation);
            
            if (DirectoryFolder.Exists)
            {
                FileInfo[] ConfigFiles = DirectoryFolder.GetFiles();
                
                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Name == "Input.cfg")
                    {
                        string[] input = System.IO.File.ReadAllLines(File.FullName);
                        
                        for (int i = 0; i < input.Length; i++)
                        {
                            string[] bits = input[i].Split();
                            string name = bits[0];
                            string key = bits[1].Replace("Key.", "");

                            string[] EnumNames = Enum.GetNames(typeof(Keys));
                            /*
                            foreach (string Testy in EnumNames)
                            {
                                Debug.WriteLine(Testy);
                            }
                            */
                            if (Enum.GetNames(typeof(Keys)).Contains(key))
                            {
                                Keys keytest = (Keys)Enum.Parse(typeof(Keys), key);
                                RegisteredKeys.Add(name, keytest);
                            }
                            else if (key.Contains("Mouse."))
                            {
                                RegisteredButtons.Add(name, key);
                            }
                            
                        }
                    }
                }
            }
        }
    }
}
