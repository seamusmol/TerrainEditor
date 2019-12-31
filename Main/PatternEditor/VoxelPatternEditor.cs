using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Geometry;
using Main.Menu;
using Main.PatternEditor;
using Main.Util;
using Microsoft.Xna.Framework;

namespace Main.Main
{
    public class VoxelPatternEditor : AppState
    {
        MenuContainer GUIContainer;

        FileLoaderButton Load;
        SaveFileButton SaveLookup;
        ToggleButton Save;
        ToggleButton New;

        TextButton WallSizeXDisplay;
        TextButton WallSizeYDisplay;
        TextButton WallSizeX;
        TextButton WallSizeY;

        ToggleButton VoxelDisplay;
        ToggleButton VoxelDisplayBackground;

        InputManager Input;

        PatternModel CurrentModel;
        TileSelectorButton MaterialSelector;
        
        RenderManager Render;

        Node2D GUINode;
        Bitmap TextureLookup;

        public VoxelPatternEditor(MenuContainer Container, RenderManager RenderManager, InputManager inputManager)
        {
            Input = inputManager;
            GUIContainer = Container;
            
            Load = (FileLoaderButton)GUIContainer.ButtonList["Load"];
            SaveLookup = (SaveFileButton)GUIContainer.ButtonList["SaveLookup"];
            Save = (ToggleButton)GUIContainer.ButtonList["Save"];
            New = (ToggleButton)GUIContainer.ButtonList["New"];

            WallSizeXDisplay = (TextButton)GUIContainer.ButtonList["WallSizeXDisplay"];
            WallSizeYDisplay = (TextButton)GUIContainer.ButtonList["WallSizeYDisplay"];

            WallSizeX = (TextButton)GUIContainer.ButtonList["WallSizeX"];
            WallSizeY = (TextButton)GUIContainer.ButtonList["WallSizeY"];

            VoxelDisplay = (ToggleButton)GUIContainer.ButtonList["VoxelDisplay"];
            VoxelDisplay.SetGeometryTexture("Missing", "Missing");

            VoxelDisplayBackground = (ToggleButton)GUIContainer.ButtonList["VoxelDisplayBackground"]; ;
            VoxelDisplayBackground.SetGeometryTexture("MenuBackground", "MenuBackground");
            MaterialSelector = (TileSelectorButton)GUIContainer.ButtonList["MaterialSelector"];
            
            Render = RenderManager;
            GUINode = Render.GuiNode;

            TextureLookup = new Bitmap("GFX/Structure.png");
        }

        public override void Update(GameTime GameTime)
        {
            int MX = Input.GetMousePosition().X;
            int MY = Input.GetMousePosition().Y;

            if (New.IsToggled)
            {
                if (CurrentModel != null)
                {
                    CurrentModel.Close();
                }
                CurrentModel = new PatternModel( (int)(VoxelDisplay.SX * VoxelDisplay.ScreenWidth), (int)(VoxelDisplay.SY * VoxelDisplay.ScreenHeight), new LockBitMap(TextureLookup));

                Render.RefreshImage("VoxelPattern", CurrentModel.PatternImage);
                VoxelDisplay.SetGeometryTexture("VoxelPattern", "VoxelPattern");

                New.Toggle(false);
            }
            if (CurrentModel != null)
            {
                if (SaveLookup.FileName != "")
                {
                    CurrentModel.SaveName = SaveLookup.FileName;

                    PatternIOUtil.Export(CurrentModel.SaveName, CurrentModel.Voxels);

                    SaveLookup.FileName = "";
                }

                if (Save.IsToggled)
                {
                    if (CurrentModel != null)
                    {
                        if (CurrentModel.SaveName != "")
                        {
                            PatternIOUtil.Export(CurrentModel.SaveName, CurrentModel.Voxels);

                        }
                    }
                    Save.Toggle(false);
                }
            }
            
            if (Load.FileName != "")
            {
                if (CurrentModel != null)
                {
                    CurrentModel.Close();
                }

                int[,] Voxels = PatternIOUtil.Import(Load.FileName);
                CurrentModel = new PatternModel(Load.FileName, (int)(VoxelDisplay.SX * VoxelDisplay.ScreenWidth), (int)(VoxelDisplay.SY * VoxelDisplay.ScreenHeight), Voxels, new LockBitMap(TextureLookup));
                Render.RefreshImage("VoxelPattern", CurrentModel.PatternImage);
                VoxelDisplay.SetGeometryTexture("VoxelPattern", "VoxelPattern");

                Load.FileName = "";
            }


            if (CurrentModel != null)
            {
                if (VoxelDisplay.HasFocus)
                {
                    if (Input.HasRegisteredMouseInput("Primary"))
                    {
                        if (CurrentModel.OnClick(VoxelDisplay.GetClickPosition(Input.GetMousePosition()), 0))
                        {
                            Render.RefreshImage("VoxelPattern", CurrentModel.PatternImage);
                        }
                    }

                    if (Input.HasRegisteredMouseInput("Secondary"))
                    {
                        if (CurrentModel.OnClick(VoxelDisplay.GetClickPosition(Input.GetMousePosition()), (int)MaterialSelector.GetValue()[2]))
                        {
                            Render.RefreshImage("VoxelPattern", CurrentModel.PatternImage);
                        }
                    }
                }
            }
            

            float MouseScrollValue = -Input.MouseWheelValue;

            if (MouseScrollValue != 0 && (WallSizeXDisplay.HasFocus || WallSizeYDisplay.HasFocus))
            {
                int Val = (int)(MouseScrollValue / Math.Abs(MouseScrollValue));
                int AX = WallSizeXDisplay.HasFocus ? Val : 0;
                int AY = WallSizeYDisplay.HasFocus ? Val : 0;

                CurrentModel.UpdateSize(AX, AY, (int)MaterialSelector.GetValue()[2]);
                Render.RefreshImage("VoxelPattern", CurrentModel.PatternImage);
            }
            
            bool IsPatternVisible = CurrentModel != null;

            VoxelDisplay.ToggleVisibility(IsPatternVisible);
            VoxelDisplayBackground.ToggleVisibility(false);
            MaterialSelector.ToggleVisibility(IsPatternVisible);

            WallSizeXDisplay.ToggleVisibility(IsPatternVisible);
            WallSizeYDisplay.ToggleVisibility(IsPatternVisible);
            WallSizeX.ToggleVisibility(IsPatternVisible);
            WallSizeY.ToggleVisibility(IsPatternVisible);

            if (CurrentModel != null)
            {
                WallSizeXDisplay.Text = CurrentModel.VX + "";
                WallSizeYDisplay.Text = CurrentModel.VY + "";

                WallSizeXDisplay.UpdateGeometry();
                WallSizeYDisplay.UpdateGeometry();
            }

        }
        


    }
}
