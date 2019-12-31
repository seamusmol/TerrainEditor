

using Main.Menu;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Main.Main
{
    public class MenuManager : AppState
    {
        //public Dictionary<string, MenuContainer> MenuStorage = new Dictionary<string, MenuContainer>();
        public MenuContainer CurrentMenuContainer = null;
        public MouseCursor MouseCursor;

        public Node2D GuiNode;
        public InputManager Input;
        public MenuState CurrentMenuState;
        public Game1 Game;

        public string FileLocation = "MenuConfig";
        public long LastClick = 0;
        public long TickRate = 200;

        public int CrosshairState = 1;
        
        public enum MenuState
        {
            Inactive = 0, Menu = 1, GamePlay = 2, Editor = 3, Exit = 4
        }

        public MenuManager(Node2D Node, InputManager InputManager, Game1 MainGame)
        {
            GuiNode = Node;
            Input = InputManager;
            Game = MainGame;
            CurrentMenuState = MenuState.Menu;
            CurrentMenuContainer = ImportMenu("MainMenu");

            MouseCursor = new MouseCursor(MainGame.GetScreenWidth()/64);

            //MenuStorage.Add( "Inactive", new MenuContainer("Inactive", this));
            UpdateScreen();
            GuiNode.Attach(MouseCursor.Geometry);
            CurrentMenuContainer.AttachToGUI();
            //GuiNode.Attach(CurrentMenuContainer.ContainerNode);
        }

        public void UpdateScreen()
        {
            /*
            foreach (KeyValuePair<string, MenuContainer> entry in MenuStorage)
            {
                entry.Value.UpdateScreenSize(Game.GetScreenWidth(), Game.GetScreenHeight());
            }
            */
            CurrentMenuContainer.UpdateScreenSize(Game.GetScreenWidth(), Game.GetScreenHeight());
        }

        public override void Update(GameTime GameTime)
        {
            int ScrollVal = (int)(-Input.MouseWheelValue / Math.Abs(-Input.MouseWheelValue));
            bool HasClick = Input.HasRegisteredMouseInput("Primary");
            bool HasTick = LastClick > TickRate;

            switch (CrosshairState)
            {
                case 0:
                    MouseCursor.Update(new Point(Game.GetScreenWidth() / 2, Game.GetScreenHeight() / 2));
                    Input.SetMouseLock(true);
                    MouseCursor.Geometry.HasCull = true;
                    break;
                case 1:

                    CurrentMenuContainer.Update(Input.GetMousePosition(), ScrollVal, HasClick, HasTick, this);
                    MouseCursor.Update(Input.GetMousePosition());
                    Input.SetMouseLock(false);
                    MouseCursor.Geometry.HasCull = false;
                    break;
                case 2:
                    MouseCursor.Update(new Point(Game.GetScreenWidth() / 2, Game.GetScreenHeight() / 2));
                    Input.SetMouseLock(true);
                    MouseCursor.Geometry.HasCull = false;
                    break;
            }
            
            if (HasClick)
            {
                LastClick = 0;
            }
            LastClick += GameTime.ElapsedGameTime.Milliseconds;
            LastClick %= 10000;
        }

        
        public void MenuTransition(string MenuName)
        {
            bool HasMenuState = false;

            for (int i = 0; i < Enum.GetNames(typeof(MenuState)).Length; i++)
            {
                if (MenuName.Contains( Enum.GetNames(typeof(MenuState))[i].ToString()))
                {
                    MenuContainer NewMenu = ImportMenu(MenuName);
                    
                    if (CurrentMenuContainer != null && NewMenu != null)
                    {
                        CurrentMenuState = (MenuState)i;
                        CurrentMenuContainer.DetachFromGUI();

                        GuiNode.DetachChildNamed("MenuContainer");
                        
                        CurrentMenuContainer = ImportMenu(MenuName);
                        CurrentMenuContainer.AttachToGUI();
                        

                        HasMenuState = true;
                        break;
                    }
                    //GuiNode.Attach(CurrentMenuContainer.ContainerNode);
                }
            }
            
            if (!HasMenuState)
            {
                return;
            }

            for (int i = 0; i < Enum.GetNames(typeof(Game1.GameStates)).Length; i++)
            {
                if (MenuName.Contains(Enum.GetNames(typeof(Game1.GameStates))[i].ToString()))
                {
                    Game.ChangeGameState(MenuName);
                    break;
                }
            }
        }
        

        public void GameModeTransition()
        {


        }

        public MenuContainer ImportMenu(string MenuName)
        {
            DirectoryInfo DirectoryFolder = new DirectoryInfo(FileLocation);
            if (DirectoryFolder.Exists)
            {
                FileInfo[] ConfigFiles = DirectoryFolder.GetFiles();
                
                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Name.Equals(MenuName + ".cfg"))
                    {
                        string[] Lines = System.IO.File.ReadAllLines(File.FullName);

                        MenuContainer NewMenuContainer = new MenuContainer(File.Name.Remove(File.Name.Length - 4, 4), this);
                        foreach (string Line in Lines)
                        {
                            if (Line.StartsWith("//"))
                            {
                                continue;
                            }

                            if (Line.Length > 0)
                            {
                                string[] bits = Line.Split();
                                
                                Type ButtonType = Type.GetType("Main.Menu." + bits[0]);
                                Button NewButton = (Button)Activator.CreateInstance(ButtonType);
                                NewButton.Init(bits, Game.GetScreenWidth(), Game.GetScreenHeight(), Game.RenderManager.Shaders);
                                
                                NewButton.MenuManager = this;
                                
                                if (NewButton != null)
                                {
                                    NewMenuContainer.Attach(NewButton);
                                }
                            }
                        }
                        return NewMenuContainer;
                    }
                }
            }
            return null;
        }

        /*
        public void ImportMenuLists()
        {
           
            DirectoryInfo DirectoryFolder = new DirectoryInfo(FileLocation);
            
            if (DirectoryFolder.Exists)
            {
                FileInfo[] ConfigFiles = DirectoryFolder.GetFiles();

                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Extension.Equals(".cfg"))
                    {
                        string[] Lines = System.IO.File.ReadAllLines(File.FullName);
                        
                        MenuContainer NewMenuContainer = new MenuContainer(File.Name.Remove(File.Name.Length - 4, 4), this);
                        foreach ( string Line in Lines)
                        {
                            if (Line.Length > 0)
                            {
                                string[] bits = Line.Split();
                                Type ButtonType = Type.GetType("Main.Menu." + bits[0]);
                                Button NewButton = (Button)Activator.CreateInstance(ButtonType);
                                NewButton.Init(bits, Game.GetScreenWidth(), Game.GetScreenHeight());

                                if (NewButton != null)
                                {
                                    NewMenuContainer.Attach(NewButton);
                                }
                            }
                        }
                        MenuStorage.Add(NewMenuContainer.Name, NewMenuContainer);
                    }
                }
            }
            */
    }
}
