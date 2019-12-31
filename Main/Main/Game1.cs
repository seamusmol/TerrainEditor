using Main.Main;
using Main.StructureEditor;
using Main.WorldEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Main
{
    
    public class Game1 : Game
    {
        public enum GameStates
        {
            MainMenu , LoadingScreen, GamePlay, MapEditor, Exit, StructureModelEditor, VoxelPatternEditor, WorldTerrainEditor
        }

        GraphicsDeviceManager Graphics;
        //SpriteBatch spriteBatch;

        public AppStateManager AppStateManager { get; private set; }
        public RenderManager RenderManager { get; private set; }
        public InputManager InputManager { get; private set; }
        public MenuManager MenuManager { get; private set; }

        //GamePlayManager GamePlayManager;
        //MapEditorManager MapEditorManager;
        //VoxelPatternEditor PatternEditor;
        WorldTerrainEditor WorldEditorManager;

        public GameStates GameState { get; set; } = GameStates.MainMenu;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            InitGraphics();
        }
        
        public void InitGraphics()
        {
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.75f);
            Graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f);
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            if (Graphics.IsFullScreen)
            {
                Graphics.ToggleFullScreen();
            }

            Graphics.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            
            Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            GameState = GameStates.MainMenu;

            AppStateManager = new AppStateManager(this);
            
            RenderManager = new RenderManager(Content, Graphics.GraphicsDevice);
            InputManager = new InputManager(this);
            MenuManager = new MenuManager(RenderManager.GuiNode, InputManager, this);

            AppStateManager.Attach(InputManager);
            AppStateManager.Attach(MenuManager);
            Graphics.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            Graphics.ApplyChanges();
            base.Initialize();
        }

        public void SetToPreserve(object Sender, PreparingDeviceSettingsEventArgs Eventargs)
        {
            Eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            Graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            AppStateManager.Update(gameTime);
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime GameTime)
        {
            AppStateManager.UpdatePreRender(GameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            RenderManager.Update(GameTime);
            
            base.Draw(GameTime);
        }

        public void ChangeGameState(string GameStateName)
        {
            bool HasMenuState = false;
            for (int i = 0; i < Enum.GetNames(typeof(GameStates)).Length; i++)
            {
                if (GameStateName.Contains(Enum.GetNames(typeof(GameStates))[i].ToString()))
                {
                    GameState = (GameStates)Enum.Parse(typeof(GameStates), Enum.GetNames(typeof(GameStates))[i]);
                    HasMenuState = true;
                    break;
                }
            }

            if (!HasMenuState)
            {
                return;
            }
            
            if (GameStateName == "Exit")
            {
                this.Exit();
            }

            /*
            if (GameStateName.Contains("GamePlay") && GamePlayManager == null)
            {
                GamePlayManager = new GamePlayManager(this, RenderManager, InputManager, MenuManager);
                AppStateManager.Attach(GamePlayManager);
            }
            else if (GameStateName == "MapEditor" && MapEditorManager == null)
            {
                MapEditorManager = new MapEditorManager(MenuManager.CurrentMenuContainer, InputManager);
                AppStateManager.Attach(MapEditorManager);
            }
            else if (GameStateName == "StructureModelEditor" && StructureModelEditor == null)
            {
                StructureModelEditor = new StructureModelEditor(this,  RenderManager, MenuManager.CurrentMenuContainer, InputManager);
                AppStateManager.Attach(StructureModelEditor);
            }
            else if (GameStateName == "MainMenu")
            {
                if (GamePlayManager != null)
                {
                    //GamePlayManager.Close();
                    AppStateManager.Detach(GamePlayManager);
                    GamePlayManager = null;
                }
                if (MapEditorManager != null)
                {
                    //MapEditorManager.Close();
                    AppStateManager.Detach(MapEditorManager);
                    MapEditorManager = null;
                }
                if (StructureModelEditor != null)
                {
                    StructureModelEditor.Close();
                    AppStateManager.Detach(StructureModelEditor);
                    StructureModelEditor = null;
                }
                if (PatternEditor != null)
                {
                    PatternEditor.Close();
                    AppStateManager.Detach(PatternEditor);
                    PatternEditor = null;
                }
                if (WorldEditorManager != null)
                {
                    WorldEditorManager.Close();
                    AppStateManager.Detach(WorldEditorManager);
                    WorldEditorManager = null;
                }
            }
            else if ( GameStateName == "VoxelPatternEditor")
            {
                PatternEditor = new VoxelPatternEditor(MenuManager.CurrentMenuContainer, RenderManager, InputManager);
                AppStateManager.Attach(PatternEditor);
            }
            */
            if (GameStateName == "WorldTerrainEditor")
            {
                WorldEditorManager = new WorldTerrainEditor(this, RenderManager, MenuManager,MenuManager.CurrentMenuContainer, InputManager);
                AppStateManager.Attach(WorldEditorManager);
            }
        }

        public void RefreshTexture(string FolderName, string TextureName)
        {
            RenderManager.RefreshImage(FolderName, TextureName);
        }

        public void LoadTextureFolder(string FolderName, string Prefix)
        {
            RenderManager.LoadTexturesFromFile(FolderName, Prefix);
        }

        public int GetScreenWidth()
        {
            return Graphics.GraphicsDevice.Viewport.Width;
        }

        public int GetScreenHeight()
        {
            return Graphics.GraphicsDevice.Viewport.Height;
        }

    }
}
