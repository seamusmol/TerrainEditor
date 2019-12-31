using Main.Geometry;
using Main.Main;
using Main.Menu;
using Main.StructureEditor;
using Main.Structures;
using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{
    //TODO
    //Change 
    public class WorldTerrainEditor : AppState
    {
        MenuManager Menu;
        CameraManager CameraManager { get; set; }
        
        ToggleButton NewMasteryFile;
       

        TileSelectorButton LODSelector;
        TileSelectorButton ToolSelector, TerrainToolSelector, StructureToolSelector, ShovelShapeSelector, MaterialSelector, ShovelToolSelector;
        TileSelectorButton WaterHeightToolSelector, WaterMaterialToolSelector, WaveToolSelector, FoamToolSelector, WaterColorToolSelector;
        TileSelectorButton TerrainMaterialToolSelector, TerrainMainMaterialToolSelector, TerrainSecondaryMaterialToolSelector, TerrainDecayToolSelector, DecalSelector;
        TileSelectorButton FlowMapSelector;
        TextButton BlendValueButton;
        TextButton ColorTextR, ColorTextG, ColorTextB, ColorTextA, ColorTextRV, ColorTextGV, ColorTextBV, ColorTextAV;
        ColorPickerButton WaterColorPicker;

        TextButton ShovelRadius, HeightFlow;
        TextButton FlowMapX, FlowMapY, FoamRampButton0;
        
        ToggleButton WaterHeightAdjustToggle;
        
        //TileSelectorButton ColorDebug, DepthDebug;

        TileSelectorButton WorldFileToolSelector;
        
        ToggleButton NewWorldFile;
       

        ToggleButton PropListUp, PropListDown, SelectOnCreationButton;
        ListButton PropList;
        TextButton SXDisplay, SYDisplay, SZDisplay, RXDisplay, RYDisplay, RZDisplay, SearchPlaceHolder, TBD3, TBD4, PlaneLockButton;
        TextButton WorldFilePosX, WorldFilePosY, WorldFileScale;
        ToggleButton CubeToggle, CameraMode, GridToggle, FlowToggle, ChunkLockToggle;
        
        Game1 Game;
        RenderManager Render;
        MenuContainer MenuContainer;
        InputManager Input;

        WorldFileManager WorldFileManager;
        MasterFileRenderManager FileRenderManager;

        Node3D RootNode;
        Node3D CollisionNode;

        Vector2 Zone3DA;
        Vector2 Zone3DB;

        public long LastClick = 0;
        public long LastTerrainModify = 0;
        public long TerrainClickRate = 16;
        public long TickRate = 200;
        
        float ShovelR { get; set; } = 1.0f;
        int TerrainHeightFlow = 1;
        int TerrainHeightFeather = 1;
        int AlphaValue = 1;
        int FlowX = 100;
        int FlowY = 100;
        int FoamRamp = 10;
        
        int ColorR = 0, ColorG = 0, ColorB = 0, ColorA = 0;
        float SX = 1.0f, SY = 1.0f, SZ = 1.0f, RX = 0, RY = 0, RZ = 0;
        float PSX = 1.0f, PSY = 1.0f, PSZ = 1.0f, PRX = 0, PRY = 0, PRZ = 0;

        int PlaneLock = 0;

        bool HasPropTranslateDrag = false;

        Vector3 PropOriginalPosition;
        Vector3 PropOriginalAngles;
        
        public WorldTerrainEditor(Game1 MainGame, RenderManager RenderManager, MenuManager MenuManager, MenuContainer MapContainer, InputManager inputManager)
        {
            Game = MainGame;
            Menu = MenuManager;
            Input = inputManager;
            Render = RenderManager;
            MenuContainer = MapContainer;

           
            NewMasteryFile = (ToggleButton)MapContainer.ButtonList["NewMasteryFile"];
           
            NewWorldFile = (ToggleButton)MapContainer.ButtonList["NewWorldFile"];
            
            ToolSelector = (TileSelectorButton)MapContainer.ButtonList["ToolSelector"];
            TerrainToolSelector = (TileSelectorButton)MapContainer.ButtonList["TerrainToolSelector"];
            StructureToolSelector = (TileSelectorButton)MapContainer.ButtonList["StructureToolSelector"];

            WaterMaterialToolSelector = (TileSelectorButton)MapContainer.ButtonList["WaterMaterialToolSelector"];
            WaveToolSelector = (TileSelectorButton)MapContainer.ButtonList["WaveToolSelector"];
            FoamToolSelector = (TileSelectorButton)MapContainer.ButtonList["FoamToolSelector"];
            WaterColorToolSelector = (TileSelectorButton)MapContainer.ButtonList["WaterColorToolSelector"];

            ShovelShapeSelector = (TileSelectorButton)MapContainer.ButtonList["ShovelShapeSelector"];
            ShovelToolSelector = (TileSelectorButton)MapContainer.ButtonList["ShovelToolSelector"];
            ShovelRadius = (TextButton)MapContainer.ButtonList["ShovelRadius"];

            WaterHeightToolSelector = (TileSelectorButton)MapContainer.ButtonList["WaterHeightToolSelector"];
            WaterHeightAdjustToggle = (ToggleButton)MapContainer.ButtonList["WaterHeightAdjustToggle"];
            
            FlowMapSelector = (TileSelectorButton)MapContainer.ButtonList["FlowMapSelector"];

            //ColorDebug = (TileSelectorButton)MapContainer.ButtonList["SceneColor"];
            //DepthDebug = (TileSelectorButton)MapContainer.ButtonList["SceneDepth"];

            //ColorDebug.TextureName = "SceneColorDebug";
            //ColorDebug.TextureName = "SceneDepthDebug";

            //Material
            MaterialSelector = (TileSelectorButton)MapContainer.ButtonList["MaterialSelector"];
            MaterialSelector = (TileSelectorButton)MapContainer.ButtonList["MaterialSelector"];
            TerrainMaterialToolSelector = (TileSelectorButton)MapContainer.ButtonList["TerrainMaterialToolSelector"];
            TerrainMainMaterialToolSelector = (TileSelectorButton)MapContainer.ButtonList["TerrainMainMaterialToolSelector"];
            TerrainSecondaryMaterialToolSelector = (TileSelectorButton)MapContainer.ButtonList["TerrainSecondaryMaterialToolSelector"];
            TerrainDecayToolSelector = (TileSelectorButton)MapContainer.ButtonList["TerrainDecayToolSelector"];
            DecalSelector = (TileSelectorButton)MapContainer.ButtonList["DecalSelector"];
            BlendValueButton = (TextButton)MapContainer.ButtonList["BlendValueButton"];

            PropList = (ListButton)MapContainer.ButtonList["PropList"];
            PropList.SetText(Render.Models.Keys.ToList());
            PropList.UpdateGeometry();

            SearchPlaceHolder = (TextButton)MapContainer.ButtonList["SearchPlaceHolder"];
            PropListUp = (ToggleButton)MapContainer.ButtonList["PropListUp"];
            PropListDown = (ToggleButton)MapContainer.ButtonList["PropListDown"];

            SelectOnCreationButton = (ToggleButton)MapContainer.ButtonList["SelectOnCreationButton"];
            PlaneLockButton = (TextButton)MapContainer.ButtonList["PlaneLockButton"];
            TBD3 = (TextButton)MapContainer.ButtonList["TBD3"];
            TBD4 = (TextButton)MapContainer.ButtonList["TBD4"];

            SXDisplay = (TextButton)MapContainer.ButtonList["SXDisplay"];
            SYDisplay = (TextButton)MapContainer.ButtonList["SYDisplay"];
            SZDisplay = (TextButton)MapContainer.ButtonList["SZDisplay"];
            RXDisplay = (TextButton)MapContainer.ButtonList["RXDisplay"];
            RYDisplay = (TextButton)MapContainer.ButtonList["RYDisplay"];
            RZDisplay = (TextButton)MapContainer.ButtonList["RZDisplay"];

            WorldFilePosX = (TextButton)MapContainer.ButtonList["WorldFilePosX"];
            WorldFilePosY = (TextButton)MapContainer.ButtonList["WorldFilePosY"];
            WorldFileScale = (TextButton)MapContainer.ButtonList["WorldFileScale"];

            LODSelector = (TileSelectorButton)MapContainer.ButtonList["LODSelector"];

            HeightFlow = (TextButton)MapContainer.ButtonList["HeightFlow"];

            FlowMapX = (TextButton)MapContainer.ButtonList["FlowMapX"];
            FlowMapY = (TextButton)MapContainer.ButtonList["FlowMapY"];
            FoamRampButton0 = (TextButton)MapContainer.ButtonList["FoamRampButton0"];
            

            CameraMode = (ToggleButton)MapContainer.ButtonList["CameraMode"];
            CubeToggle = (ToggleButton)MapContainer.ButtonList["CubeToggle"];
            GridToggle = (ToggleButton)MapContainer.ButtonList["GridToggle"];
            FlowToggle = (ToggleButton)MapContainer.ButtonList["FlowToggle"];
            ChunkLockToggle = (ToggleButton)MapContainer.ButtonList["ChunkLockToggle"];

            WaterColorPicker = (ColorPickerButton)MapContainer.ButtonList["WaterColorPicker"];
            
            ColorTextR = (TextButton)MapContainer.ButtonList["ColorTextR"];
            ColorTextG = (TextButton)MapContainer.ButtonList["ColorTextG"];
            ColorTextB = (TextButton)MapContainer.ButtonList["ColorTextB"];
            ColorTextA = (TextButton)MapContainer.ButtonList["ColorTextA"];
            ColorTextRV = (TextButton)MapContainer.ButtonList["ColorTextRV"];
            ColorTextGV = (TextButton)MapContainer.ButtonList["ColorTextGV"];
            ColorTextBV = (TextButton)MapContainer.ButtonList["ColorTextBV"];
            ColorTextAV = (TextButton)MapContainer.ButtonList["ColorTextAV"];

            //WorldFile Tools
            WorldFileToolSelector = (TileSelectorButton)MapContainer.ButtonList["WorldFileToolSelector"];
            

            CollisionNode = new Node3D("CollisionNode");
            RootNode = new Node3D("WorldNode");
            Render.RootNode = RootNode;

            CameraManager = new CameraManager(new Vector3(10, 10, 20), Render, Input);
            Render.Camera = CameraManager.Camera;

            WorldFileManager = new WorldFileManager(Render);
            FileRenderManager = new MasterFileRenderManager(WorldFileManager, Render, CameraManager);

            FlowMapSelector.SetPosition(FlowX, FlowY);
            Zone3DA = new Vector2(Game.GetScreenWidth() * 0.15f, Game.GetScreenHeight() * 0.15f);
            Zone3DB = new Vector2(Game.GetScreenWidth() * 0.85f, Game.GetScreenHeight() * 0.85f);
            CameraManager.SetMode(0);
            CameraMode.Toggle(true);
            GridToggle.Toggle(true);
        }

        public override void Close()
        {
            Render.Camera = null;
            Render.RootNode = new Node3D("RootNode");
        }

        public override void UpdatePreRender(GameTime GameTime)
        {
            int MX = Input.GetMousePosition().X;
            int MY = Input.GetMousePosition().Y;

            bool WithinModelZone = (MX >= Zone3DA.X && MX <= Zone3DB.X && MY >= Zone3DA.Y && MY <= Zone3DB.Y);
            CameraManager.SetMode(CameraMode.IsToggled ? 0 : 1);
            CameraManager.CanMove = WithinModelZone;
            CameraManager.Update(GameTime);
            
            FileRenderManager.UpdatePreRender(GameTime);
        }

        public override void Update(GameTime GameTime)
        {
            FileRenderManager.Update(GameTime);
            

            int MX = Input.GetMousePosition().X;
            int MY = Input.GetMousePosition().Y;
            bool WithinModelZone = (MX >= Zone3DA.X && MX <= Zone3DB.X && MY >= Zone3DA.Y && MY <= Zone3DB.Y);
            float MouseScrollValue = -Input.MouseWheelValue;
            int Val = (int)(MouseScrollValue / Math.Abs(MouseScrollValue));
            Val *= Input.HasRegisteredInput("Down") ? 5 : 1;
            Val *= Input.HasRegisteredInput("Up") ? 10 : 1;

            if (LastClick > TickRate)
            {
                bool HasClick = false;
                if (Input.HasRegisteredInput("Jump"))
                {
                    CameraManager.ToggleCubeVisibility();
                    HasClick = true;
                }
                if (Input.HasRegisteredInput("Tab"))
                {
                    Menu.CrosshairState = Menu.CrosshairState == 1 ? 2 : 1;

                    HasClick = true;
                }
                if (Input.HasRegisteredInput("Translate"))
                {
                    if (WorldFileManager.SelectedProp != null)
                    {
                        HasPropTranslateDrag = !HasPropTranslateDrag;
                        HasClick = true;
                    }
                }
                if (Input.HasRegisteredInput("AxisZ"))
                {
                    PlaneLock = PlaneLock == 1 ? 0 : 1;
                    HasClick = true;
                }
                else if (Input.HasRegisteredInput("AxisY"))
                {
                    PlaneLock = PlaneLock == 2 ? 0 : 2;
                    HasClick = true;
                }
                else if (Input.HasRegisteredInput("AxisX"))
                {
                    PlaneLock = PlaneLock == 3 ? 0 : 3;
                    HasClick = true;
                }

                int PosX = WorldFilePosX.HasFocus ? Val : 0;
                int PosY = WorldFilePosY.HasFocus ? Val : 0;
                float Scale = WorldFileScale.HasFocus ? (Val < 0 ? 0.5f : 2.0f) : 1;
                if (WorldFileManager.MasteryFile != null && !WithinModelZone && MouseScrollValue != 0)
                {
                    if (PosX != 0 || PosY != 0)
                    {
                        //move Camera to ChunkIDX
                        WorldFileManager.MoveToChunk(WorldFileManager.SelectedChunkX + PosX, WorldFileManager.SelectedChunkY + PosY);
                        CameraManager.SetPosition(new Vector3( CameraManager.Camera.Position.X +(PosX * WorldFileManager.MasteryFile.Settings.ChunkSize), CameraManager.Camera.Position.Y + (PosY * WorldFileManager.MasteryFile.Settings.ChunkSize), CameraManager.Camera.Position.Z));
                        HasClick = true;
                    }
                }
                
                if (HasClick)
                {
                    LastClick = 0;
                }
            }
            LastClick += GameTime.ElapsedGameTime.Milliseconds;
            LastClick %= 60000;
            
            if (NewMasteryFile.IsToggled)
            {
                WorldFileManager.CreateMasteryFile();
                //clear current world Files
                
                FileRenderManager.UpdateRenderTargets();
                NewMasteryFile.Toggle(false);
            }
            

            if (NewWorldFile.IsToggled)
            {
                WorldFileManager.CreateWorldFile();
                NewWorldFile.Toggle(false);
            }

            if (PropListUp.IsToggled)
            {
                PropList.MoveList(-1);
                PropListUp.Toggle(false);
            }
            if (PropListDown.IsToggled)
            {
                PropList.MoveList(1);
                PropListDown.Toggle(false);
            }

            //|| FirstMaterialSelector.OnFocus(MX,MY) || SecondMaterialSelector.OnFocus(MX,MY)
            if (MouseScrollValue != 0 && !WithinModelZone)
            {
                ShovelR += ShovelRadius.HasFocus ? Val * 0.25f : 0;
                AlphaValue += BlendValueButton.HasFocus ? Val : 0;
                TerrainHeightFlow += HeightFlow.HasFocus ? Val : 0;
                FoamRamp += FoamRampButton0.HasFocus ? Val : 0;
                
                ColorR += ColorTextRV.HasFocus ? Val : 0;
                ColorG += ColorTextGV.HasFocus ? Val : 0;
                ColorB += ColorTextBV.HasFocus ? Val : 0;
                ColorA += ColorTextAV.HasFocus ? Val : 0;
                
                ShovelR = ShovelR >= 0.25f ? ShovelR : 0.25f;
                AlphaValue = AlphaValue >= 0 ? AlphaValue : 0;
                TerrainHeightFlow = TerrainHeightFlow >= 0 ? TerrainHeightFlow : 0;
                TerrainHeightFeather = TerrainHeightFeather >= 0 ? TerrainHeightFeather : 0;

                ShovelR = ShovelR <= 200 ? ShovelR : 200;
                AlphaValue = AlphaValue <= 255 ? AlphaValue : 255;
                TerrainHeightFlow = TerrainHeightFlow <= 100 ? TerrainHeightFlow : 100;
                TerrainHeightFeather = TerrainHeightFeather <= 100 ? TerrainHeightFeather : 100;

                FlowX += FlowMapX.HasFocus ? Val : 0;
                FlowY += FlowMapY.HasFocus ? Val : 0;

                FlowX = FlowX <= 0 ? 0 : FlowX;
                FlowY = FlowY <= 0 ? 0 : FlowY;
                FoamRamp = FoamRamp <= 0 ? 0 : FoamRamp;
                
                ColorR = ColorR <= 0 ? 0 : ColorR;
                ColorG = ColorG <= 0 ? 0 : ColorG;
                ColorB = ColorB <= 0 ? 0 : ColorB;
                ColorA = ColorA <= 0 ? 0 : ColorA;
                
                FlowX = FlowX >= 255 ? 255 : FlowX;
                FlowY = FlowY >= 255 ? 255 : FlowY;
                FoamRamp = FoamRamp >= 255 ? 255 : FoamRamp;
                
                ColorR = ColorR >= 255 ? 255 : ColorR;
                ColorG = ColorG >= 255 ? 255 : ColorG;
                ColorB = ColorB >= 255 ? 255 : ColorB;
                ColorA = ColorA >= 255 ? 255 : ColorA;
 
                WaterColorPicker.SetRGBA(ColorR, ColorG, ColorB, ColorA);
                FlowMapSelector.SetPosition(FlowX, FlowY);
                
                if (WorldFileManager.SelectedProp != null)
                {
                    PSX += SXDisplay.HasFocus ? Val * 0.1f : 0;
                    PSY += SYDisplay.HasFocus ? Val * 0.1f : 0;
                    PSZ += SZDisplay.HasFocus ? Val * 0.1f : 0;
                    PRX += RXDisplay.HasFocus ? Val * 1.0f : 0;
                    PRY += RYDisplay.HasFocus ? Val * 1.0f : 0;
                    PRZ += RZDisplay.HasFocus ? Val * 1.0f : 0;
                    PSX = PSX <= 0.1f ? 0.1f : PSX;
                    PSY = PSY <= 0.1f ? 0.1f : PSY;
                    PSZ = PSZ <= 0.1f ? 0.1f : PSZ;
                    PRX = PRX < 0 ? 359 : PRX;
                    PRY = PRY < 0 ? 359 : PRY;
                    PRZ = PRZ < 0 ? 359 : PRZ;
                    PSX = PSX >= 100 ? 100 : PSX;
                    PSY = PSY >= 100 ? 100 : PSY;
                    PSZ = PSZ >= 100 ? 100 : PSZ;
                    PRX %= 360;
                    PRY %= 360;
                    PRZ %= 360;
                    
                    WorldFileManager.SelectedProp.SetRotation(PRX, PRY, PRZ);
                    WorldFileManager.SelectedProp.Scale = new Vector3(PSX,PSY,PSZ);
                }
                else
                {
                    SX += SXDisplay.HasFocus ? Val * 0.1f : 0;
                    SY += SYDisplay.HasFocus ? Val * 0.1f : 0;
                    SZ += SZDisplay.HasFocus ? Val * 0.1f : 0;
                    RX += RXDisplay.HasFocus ? Val * 1.0f : 0;
                    RY += RYDisplay.HasFocus ? Val * 1.0f : 0;
                    RZ += RZDisplay.HasFocus ? Val * 1.0f : 0;
                    SX = SX <= 0.1f ? 0.1f : SX;
                    SY = SY <= 0.1f ? 0.1f : SY;
                    SZ = SZ <= 0.1f ? 0.1f : SZ;
                    RX = RX < 0 ? 359 : RX;
                    RY = RY < 0 ? 359 : RY;
                    RZ = RZ < 0 ? 359 : RZ;
                    SX = SX >= 100 ? 100 : SX;
                    SY = SY >= 100 ? 100 : SY;
                    SZ = SZ >= 100 ? 100 : SZ;
                    RX %= 360;
                    RY %= 360;
                    RZ %= 360;
                }
            }

            if (!WithinModelZone)
            {
                if (Input.HasRegisteredMouseInput("Primary"))
                {
                    if (FlowMapSelector.HasFocus)
                    {
                        FlowX = (int)FlowMapSelector.GetValue()[0];
                        FlowY = (int)FlowMapSelector.GetValue()[1];
                    }
                }
            }

            //Selector Values
            int SelectedTool = (int)(ToolSelector.GetValue()[2]);
            int SelectedTerrainTool = (int)(TerrainToolSelector.GetValue()[2]);
            int SelectedWaterTool = (int)(WaterHeightToolSelector.GetValue()[2]);
            //Water Material Tool
            int SelectedWaterMaterialTool = (int)WaterMaterialToolSelector.GetValue()[2];

            int SelectedFlowTool = (int)WaveToolSelector.GetValue()[2];
            int SelectedFoamTool = (int)FoamToolSelector.GetValue()[2];
            int SelectedWaterColorTool = (int)WaterColorToolSelector.GetValue()[2];
            int SelectedMaterialTool = (int)TerrainMaterialToolSelector.GetValue()[2];
            int SelectedMainMaterialBrush = (int)TerrainMainMaterialToolSelector.GetValue()[2];
            int SelectedSecondaryMaterialBrush = (int)TerrainSecondaryMaterialToolSelector.GetValue()[2];
            int SelectedDecalBrush = (int)TerrainDecayToolSelector.GetValue()[2];
            
            int SelectedPropTool = (int)StructureToolSelector.GetValue()[2];
            int SelectedShape = (int)ShovelShapeSelector.GetValue()[2];

            if (WithinModelZone)
            {
                if (SelectedTool == 0)
                {
                    int[] ShapesVals = new int[] { 0, 1, 0 };
                    int[] HighLightGroup = new int[] { 0,0,1,1};
                    Vector2 Click2D = new Vector2(MX, MY);
                    Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                    FileRenderManager.UpdateBrushHighLight(CameraManager.Camera.Position, Direction, new Vector4(0, 1.0f, 0, 0.5f), HighLightGroup[SelectedTerrainTool], ShapesVals[SelectedShape], ShovelR, 75);
                }
            }
            else
            {
                FileRenderManager.ClearBrushHighLight();
            }

            if (WorldFileManager.SelectedProp != null)
            {
                if (HasPropTranslateDrag)
                {
                    Vector3 Center = WorldFileManager.SelectedProp.Position;
                    Vector3 P1, P2, P3, P4;
                    if (PlaneLock == 1)
                    {
                        P1 = new Vector3(-10, 10, 0) + Center;
                        P2 = new Vector3(-10, -10, 0) + Center;
                        P3 = new Vector3(10, 10, 0) + Center;
                        P4 = new Vector3(10, -10, 0) + Center;
                    }
                    else if (PlaneLock == 2)
                    {
                        P1 = new Vector3(0, -10, 10) + Center;
                        P2 = new Vector3(0, -10, -10) + Center;
                        P3 = new Vector3(0, 10, 10) + Center;
                        P4 = new Vector3(0, 10, -10) + Center;
                    }
                    else if (PlaneLock == 3)
                    {
                        P1 = new Vector3(-10, 0, 10) + Center;
                        P2 = new Vector3(-10, 0, -10) + Center;
                        P3 = new Vector3(10, 0, 10) + Center;
                        P4 = new Vector3(10, 0, -10) + Center;
                    }
                    else
                    {
                        P1 = new Vector3(0, 0, 0) + Center;
                        P2 = new Vector3(0, 0, 0) + Center;
                        P3 = new Vector3(0, 0, 0) + Center;
                        P4 = new Vector3(0, 0, 0) + Center;
                    }

                    Vector3 Direction = CollisionUtil.GetDirection(new Vector2(MX, MY), CameraManager.Camera, Render.Graphics);
                    CollisionQuad TranslatePlane = new CollisionQuad(P1, P2, P3, P4);
                    CollisionResults TranslateResults = TranslatePlane.CollideWith(CameraManager.Camera.Position, Direction);
                    if (TranslateResults.Count != 0)
                    {
                        WorldFileManager.SelectedProp.Position = TranslateResults.GetClosest().CollisionPoint;
                        WorldFileManager.SelectedProp.HighLight = (byte)PlaneLock;
                    }
                }
                else
                {
                    WorldFileManager.SelectedProp.HighLight = 4;
                }
            }
            
            if (LastTerrainModify > TickRate || FlowToggle.IsToggled)
            {
                bool HasModify = false;
                bool HasPrimary = Input.HasRegisteredMouseInput("Primary");
                bool HasSecondary = Input.HasRegisteredMouseInput("Secondary");
                if (WithinModelZone)
                {
                    if (HasPrimary)
                    {
                        if (SelectedTool == 0)
                        {
                            if (SelectedTerrainTool == 0)
                            {
                                //Terrain-ShovelTool
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                int CurrentChunkLOD = FileRenderManager.GetChunkLOD(CameraManager.Position);
                                CollisionResults Results = FileRenderManager.RayCastTerrain(CameraManager.Camera.Position, Direction, CurrentChunkLOD, 75, ChunkLockToggle.IsToggled);
                                if (Results.Count > 0)
                                {
                                    int[] ToolVals = new int[] { 0, 1, 2 };
                                    float[] HeightVals = new float[] { TerrainHeightFlow, TerrainHeightFlow, TerrainHeightFlow * 0.1f };

                                    WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "HeightMap", "WaterHeightMap", SelectedShape, ToolVals[(int)(ShovelToolSelector.GetValue()[2])], true, ShovelR, HeightVals[(int)(ShovelToolSelector.GetValue()[2])], 0, 2, WaterHeightAdjustToggle.IsToggled);
                                }
                            }
                            else if (SelectedTerrainTool == 1)
                            {
                                //Terrain-MaterialTool
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                int CurrentChunkLOD = FileRenderManager.GetChunkLOD(CameraManager.Position);
                                CollisionResults Results = FileRenderManager.RayCastTerrain(CameraManager.Camera.Position, Direction, CurrentChunkLOD, 75, ChunkLockToggle.IsToggled);
                                if (Results.Count > 0)
                                {
                                    //WorldFileManager.ModifyWorldFileMaterial();
                                    if (SelectedMaterialTool == 0)
                                    {
                                        //WorldFileManager.modify
                                        int[] ToolVals = new int[] { 3, 3, 3 };
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "MaterialMap", SelectedShape, ToolVals[SelectedMainMaterialBrush], true, ShovelR, 0, (int)MaterialSelector.GetValue()[2], 1);
                                    }
                                    else if (SelectedMaterialTool == 1)
                                    {
                                        //secondary material
                                        int[] MaterialTools = new int[] { 3, 3, 2 };
                                        string[] AffectedMap = new string[] { "SecondaryMaterialMap", "BlendAlphaMap", "BlendAlphaMap" };
                                        int[] BrushValue = new int[] { (int)MaterialSelector.GetValue()[2], AlphaValue, AlphaValue };
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedSecondaryMaterialBrush], SelectedShape, MaterialTools[SelectedSecondaryMaterialBrush], true, ShovelR, TerrainHeightFlow, BrushValue[SelectedSecondaryMaterialBrush], 1);
                                    }
                                    else if (SelectedMaterialTool == 2)
                                    {
                                        int[] MaterialTools = new int[] { 3, 3, 2 };
                                        string[] AffectedMap = new string[] { "DecalMaterialMap", "DecalAlphaMap", "DecalAlphaMap" };
                                        int[] BrushValue = new int[] { (int)MaterialSelector.GetValue()[2], AlphaValue, AlphaValue };
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedSecondaryMaterialBrush], SelectedShape, MaterialTools[SelectedSecondaryMaterialBrush], true, ShovelR, TerrainHeightFlow, BrushValue[SelectedSecondaryMaterialBrush], 1);
                                    }
                                }
                            }
                            else if (SelectedTerrainTool == 2)
                            {
                                //enable water tool
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                CollisionResults Results = FileRenderManager.RayCastWater(CameraManager.Camera.Position, Direction, 50, ChunkLockToggle.IsToggled);

                                if (Results.Count > 0)
                                {
                                    //has water,flatten water,raise water
                                    int[] WaterTools = new int[] { 3, 5, 1, 3, 2, 3, 2 };
                                    string[] AffectedMap = new string[] { "WaterMap", "WaterHeightMap", "WaterHeightMap", "WaveLengthMap", "WaveLengthMap", "WaveHeightMap", "WaveHeightMap" };
                                    int[] BrushValue = new int[] { 1, 1, 0, TerrainHeightFlow, TerrainHeightFlow, TerrainHeightFlow, TerrainHeightFlow };

                                    if (SelectedWaterTool == 1 || SelectedWaterTool == 2)
                                    {
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedWaterTool], "HeightMap", SelectedShape, WaterTools[SelectedWaterTool], true, ShovelR, TerrainHeightFlow, BrushValue[SelectedWaterTool], 2, true);
                                    }
                                    else
                                    {
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedWaterTool], SelectedShape, WaterTools[SelectedWaterTool], true, ShovelR, TerrainHeightFlow, BrushValue[SelectedWaterTool], 1);
                                    }
                                }
                            }
                            else if (SelectedTerrainTool == 3)
                            {
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                CollisionResults Results = FileRenderManager.RayCastWater(CameraManager.Camera.Position, Direction, 50);
                                //WaveFlow
                                //FoamFlow
                                if (Results.Count > 0)
                                {
                                    if (SelectedWaterMaterialTool == 0)
                                    {
                                        int[] WaterTools = new int[] { 3, 2, 3, 3, 3, 2 };
                                        string[] AffectedMap = new string[] { "FlowXMap", "FlowXMap", "FlowBackTimeMap", "FlowPulseSpeedMap", "WaterNormalMap", "WaterNormalMap" };
                                        int[] BrushValue = new int[] { (byte)((int)FlowMapSelector.GetValue()[0]), Math.Min( AlphaValue, 100), (byte)AlphaValue, (byte)AlphaValue, (byte)AlphaValue, Math.Min(AlphaValue, 100) };

                                        if (SelectedFlowTool == 0 || SelectedFlowTool == 1)
                                        {

                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "FlowYMap", SelectedShape, WaterTools[SelectedFlowTool], true, ShovelR, TerrainHeightFlow, (byte)((int)FlowMapSelector.GetValue()[1]), 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedFlowTool], SelectedShape, WaterTools[SelectedFlowTool], true, ShovelR, 50, BrushValue[SelectedFlowTool], 1);
                                        }
                                        else
                                        {
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedFlowTool], SelectedShape, WaterTools[SelectedFlowTool], true, ShovelR, 50, BrushValue[SelectedFlowTool], 1);
                                        }
                                    }
                                    else if (SelectedWaterMaterialTool == 1)
                                    {
                                        int[] WaterTools = new int[] { 3, 2 };
                                        string[] AffectedMap = new string[] { "FoamRampMap0", "FoamRampMap0" };
                                        int[] BrushValue = new int[] { FoamRamp, FoamRamp };

                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedFoamTool], SelectedShape, WaterTools[SelectedFoamTool], true, ShovelR, TerrainHeightFlow, BrushValue[SelectedFoamTool], 1);
                                    }
                                    else if (SelectedWaterMaterialTool == 2)
                                    {
                                        int[] WaterTools = new int[] { 3, 2, 3, 2, 3, 2 };
                                        string[] AffectedMap = new string[] { "", "", "WaterFresnelMap", "WaterFresnelMap", "WaterColorFalloffMap", "WaterColorFalloffMap" };
                                        int[] BrushValue = new int[] { 0, 0, AlphaValue, AlphaValue, AlphaValue, AlphaValue };

                                        if (SelectedWaterColorTool == 0 || SelectedWaterColorTool == 1)
                                        {
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorR", SelectedShape, WaterTools[SelectedWaterColorTool], true, ShovelR, TerrainHeightFlow, ColorR, 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorG", SelectedShape, WaterTools[SelectedWaterColorTool], true, ShovelR, TerrainHeightFlow, ColorG, 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorB", SelectedShape, WaterTools[SelectedWaterColorTool], true, ShovelR, TerrainHeightFlow, ColorB, 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorA", SelectedShape, WaterTools[SelectedWaterColorTool], true, ShovelR, TerrainHeightFlow, ColorA, 1);
                                        }
                                        else
                                        {
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedWaterColorTool], SelectedShape, WaterTools[SelectedWaterColorTool], true, ShovelR, TerrainHeightFlow, BrushValue[SelectedWaterColorTool], 1);
                                        }
                                    }
                                }
                            }
                        }
                        else if (SelectedTool == 1)
                        {
                            if (SelectedPropTool == 0)
                            {

                            }
                            else if (SelectedPropTool == 1)
                            {
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);

                                Vector3 CollisionPosition = new Vector3();
                                bool HasCollision = false;

                                CollisionResults WaterHitResults = FileRenderManager.RayCastWater(CameraManager.Camera.Position, Direction, 50);
                                if (WaterHitResults.Count != 0)
                                {
                                    CollisionPosition = WaterHitResults.GetClosest().CollisionPoint;
                                    HasCollision = true;
                                }
                                else
                                {
                                    int CurrentChunkLOD = FileRenderManager.GetChunkLOD(CameraManager.Position);
                                    CollisionResults TerrainHitResults = FileRenderManager.RayCastTerrain(CameraManager.Camera.Position, Direction, CurrentChunkLOD, 75, ChunkLockToggle.IsToggled);
                                    if (TerrainHitResults.Count != 0)
                                    {
                                        CollisionPosition = TerrainHitResults.GetClosest().CollisionPoint;
                                        HasCollision = true;
                                    }
                                }
                                

                                if (HasPropTranslateDrag)
                                {
                                    HasPropTranslateDrag = false;
                                }
                                else if (WorldFileManager.SelectedProp == null && HasCollision)
                                {
                                    WorldFileManager.CreateProp(CollisionPosition, (string)PropList.GetValue()[0], SelectOnCreationButton.IsToggled, SX, SY, SZ, RX + 90, RY, RZ + 270);
                                    if (WorldFileManager.SelectedProp != null)
                                    {
                                        WorldFileManager.SelectedProp.HighLight = 1;
                                        PSX = WorldFileManager.SelectedProp.Scale.X;
                                        PSY = WorldFileManager.SelectedProp.Scale.Y;
                                        PSZ = WorldFileManager.SelectedProp.Scale.Z;
                                        PRX = WorldFileManager.SelectedProp.Angles.X;
                                        PRY = WorldFileManager.SelectedProp.Angles.Y;
                                        PRZ = WorldFileManager.SelectedProp.Angles.Z;
                                        PropOriginalPosition = new Vector3(WorldFileManager.SelectedProp.Position.X, WorldFileManager.SelectedProp.Position.Y, WorldFileManager.SelectedProp.Position.Z);
                                        PropOriginalAngles = new Vector3(WorldFileManager.SelectedProp.Angles.X, WorldFileManager.SelectedProp.Angles.Y, WorldFileManager.SelectedProp.Angles.Z);
                                    }
                                    HasModify = true;
                                    
                                }
                            }
                        }
                        else if (SelectedTool == 2)
                        {
                            //WordFile Tools
                            Vector2 Click2D = new Vector2(MX, MY);
                            Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                            WorldFileManager.SetSelectedChunk(CameraManager.Camera.Position, Direction);
                        }
                    }
                    else if (HasSecondary)
                    {
                        if (SelectedTool == 0)
                        {
                            if (SelectedTerrainTool == 0)
                            {
                                //Terrain-ShovelTool
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                int CurrentChunkLOD = FileRenderManager.GetChunkLOD(CameraManager.Position);
                                CollisionResults Results = FileRenderManager.RayCastTerrain(CameraManager.Camera.Position, Direction, CurrentChunkLOD, 75, ChunkLockToggle.IsToggled);
                                if (Results.Count > 0)
                                {
                                    int[] ToolVals = new int[] { 0, 1, 2 };
                                    float[] HeightVals = new float[] { TerrainHeightFlow, TerrainHeightFlow, TerrainHeightFlow * 0.1f };

                                    WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "HeightMap", "WaterHeightMap", SelectedShape, ToolVals[(int)(ShovelToolSelector.GetValue()[2])], false, ShovelR, HeightVals[(int)(ShovelToolSelector.GetValue()[2])], 0,2, WaterHeightAdjustToggle.IsToggled);
                                }
                            }
                            else if (SelectedTerrainTool == 1)
                            {
                                //Terrain-MaterialTool
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                int CurrentChunkLOD = FileRenderManager.GetChunkLOD(CameraManager.Position);
                                CollisionResults Results = FileRenderManager.RayCastTerrain(CameraManager.Camera.Position, Direction, CurrentChunkLOD, 75, ChunkLockToggle.IsToggled);
                                if (Results.Count > 0)
                                {
                                    //WorldFileManager.ModifyWorldFileMaterial();
                                    if (SelectedMaterialTool == 0)
                                    {
                                        //WorldFileManager.modify
                                        int[] ToolVals = new int[] { 3, 3, 3 };
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "MaterialMap", SelectedShape, ToolVals[SelectedMainMaterialBrush], false, ShovelR, 0, (int)MaterialSelector.GetValue()[2], 1);
                                    }
                                    else if (SelectedMaterialTool == 1)
                                    {
                                        //secondary material
                                        int[] MaterialTools = new int[] { 3, 3, 2 };
                                        string[] AffectedMap = new string[] { "SecondaryMaterialMap", "BlendAlphaMap", "BlendAlphaMap" };
                                        int[] BrushValue = new int[] { (int)MaterialSelector.GetValue()[2], AlphaValue, AlphaValue };
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedSecondaryMaterialBrush], SelectedShape, MaterialTools[SelectedSecondaryMaterialBrush], false, ShovelR, TerrainHeightFlow, BrushValue[SelectedSecondaryMaterialBrush], 1);
                                    }
                                    else if (SelectedMaterialTool == 2)
                                    {
                                        int[] MaterialTools = new int[] { 3, 3, 2 };
                                        string[] AffectedMap = new string[] { "DecalMaterialMap", "DecalAlphaMap", "DecalAlphaMap" };
                                        int[] BrushValue = new int[] { (int)MaterialSelector.GetValue()[2], AlphaValue, AlphaValue };
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedSecondaryMaterialBrush], SelectedShape, MaterialTools[SelectedSecondaryMaterialBrush], false, ShovelR, TerrainHeightFlow, BrushValue[SelectedSecondaryMaterialBrush], 1);
                                    }
                                }
                            }
                            else if (SelectedTerrainTool == 2)
                            {
                                //enable water tool
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                CollisionResults Results = FileRenderManager.RayCastWater(CameraManager.Camera.Position, Direction, 50, ChunkLockToggle.IsToggled);

                                if (Results.Count > 0)
                                {
                                    //has water,flatten water,raise water
                                    int[] WaterTools = new int[] { 3, 5, 1, 3, 2, 3, 2 };
                                    string[] AffectedMap = new string[] { "WaterMap", "WaterHeightMap", "WaterHeightMap", "WaveLengthMap", "WaveLengthMap", "WaveHeightMap", "WaveHeightMap" };
                                    int[] BrushValue = new int[] { 0, 1, 0, TerrainHeightFlow, TerrainHeightFlow, TerrainHeightFlow, TerrainHeightFlow };

                                    if (SelectedWaterTool == 1 || SelectedWaterTool == 2)
                                    {
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedWaterTool], "HeightMap", SelectedShape, WaterTools[SelectedWaterTool], false, ShovelR, TerrainHeightFlow, BrushValue[SelectedWaterTool], 2, true);
                                    }
                                    else
                                    {
                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedWaterTool], SelectedShape, WaterTools[SelectedWaterTool], false, ShovelR, TerrainHeightFlow, BrushValue[SelectedWaterTool], 1);
                                    }
                                }
                            }
                            else if (SelectedTerrainTool == 3)
                            {
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                                CollisionResults Results = FileRenderManager.RayCastWater(CameraManager.Camera.Position, Direction, 50);
                                //WaveFlow
                                //FoamFlow
                                if (Results.Count > 0)
                                {
                                    if (SelectedWaterMaterialTool == 0)
                                    {
                                        int[] WaterTools = new int[] { 3, 2, 3, 3, 3, 2 };
                                        string[] AffectedMap = new string[] { "FlowXMap", "FlowXMap", "FlowBackTimeMap", "FlowPulseSpeedMap", "WaterNormalMap", "WaterNormalMap" };
                                        int[] BrushValue = new int[] { (byte)((int)FlowMapSelector.GetValue()[0]), 0, (byte)AlphaValue, (byte)AlphaValue, (byte)AlphaValue, (byte)AlphaValue };

                                        if (SelectedFlowTool == 0 || SelectedFlowTool == 1)
                                        {

                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "FlowYMap", SelectedShape, WaterTools[SelectedFlowTool], false, ShovelR, TerrainHeightFlow, (byte)((int)FlowMapSelector.GetValue()[1]), 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedFlowTool], SelectedShape, WaterTools[SelectedFlowTool], false, ShovelR, 50, BrushValue[SelectedFlowTool], 1);
                                        }
                                        else
                                        {
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedFlowTool], SelectedShape, WaterTools[SelectedFlowTool], false, ShovelR, 50, BrushValue[SelectedFlowTool], 1);
                                        }
                                    }
                                    else if (SelectedWaterMaterialTool == 1)
                                    {
                                        int[] WaterTools = new int[] { 3, 2 };
                                        string[] AffectedMap = new string[] { "FoamRampMap0", "FoamRampMap0" };
                                        int[] BrushValue = new int[] { FoamRamp, FoamRamp };

                                        WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedFoamTool], SelectedShape, WaterTools[SelectedFoamTool], false, ShovelR, TerrainHeightFlow, BrushValue[SelectedFoamTool], 1);
                                    }
                                    else if (SelectedWaterMaterialTool == 2)
                                    {
                                        int[] WaterTools = new int[] { 3, 2, 3, 2, 3, 2 };
                                        string[] AffectedMap = new string[] { "", "", "WaterFresnelMap", "WaterFresnelMap", "WaterColorFalloffMap", "WaterColorFalloffMap" };
                                        int[] BrushValue = new int[] { 0, 0, AlphaValue, AlphaValue, AlphaValue, AlphaValue };

                                        if (SelectedWaterColorTool == 0 || SelectedWaterColorTool == 1)
                                        {
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorR", SelectedShape, WaterTools[SelectedWaterColorTool], false, ShovelR, TerrainHeightFlow, ColorR, 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorG", SelectedShape, WaterTools[SelectedWaterColorTool], false, ShovelR, TerrainHeightFlow, ColorG, 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorB", SelectedShape, WaterTools[SelectedWaterColorTool], false, ShovelR, TerrainHeightFlow, ColorB, 1);
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, "WaterColorA", SelectedShape, WaterTools[SelectedWaterColorTool], false, ShovelR, TerrainHeightFlow, ColorA, 1);
                                        }
                                        else
                                        {
                                            WorldFileManager.ModifyMultipleChunks(Results.GetClosest().CollisionPoint, AffectedMap[SelectedWaterColorTool], SelectedShape, WaterTools[SelectedWaterColorTool], false, ShovelR, TerrainHeightFlow, BrushValue[SelectedWaterColorTool], 1);
                                        }
                                    }
                                }
                            }
                        }
                        else if (SelectedTool == 1)
                        {
                            if (SelectedPropTool == 0)
                            {

                            }
                            else if (SelectedPropTool == 1)
                            {
                                Vector2 Click2D = new Vector2(MX, MY);
                                Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);

                                Vector3 CollisionPosition = new Vector3();
                                bool HasPropCollision = false;

                                CollisionResults WaterHitResults = FileRenderManager.RayCastWater(CameraManager.Camera.Position, Direction, 50);
                                if (WaterHitResults.Count != 0)
                                {
                                    CollisionPosition = WaterHitResults.GetClosest().CollisionPoint;
                                }
                                else
                                {
                                    int CurrentChunkLOD = FileRenderManager.GetChunkLOD(CameraManager.Position);
                                    CollisionResults TerrainHitResults = FileRenderManager.RayCastTerrain(CameraManager.Camera.Position, Direction, CurrentChunkLOD, 75, ChunkLockToggle.IsToggled);
                                    if (TerrainHitResults.Count != 0)
                                    {
                                        CollisionPosition = TerrainHitResults.GetClosest().CollisionPoint;
                                    }
                                }

                                CollisionResults PropCollisions = FileRenderManager.RayCastProps(CameraManager.Camera.Position, Direction, 75);
                                if (PropCollisions.Count != 0)
                                {
                                    HasPropCollision = true;
                                }
                               
                                if (HasPropTranslateDrag)
                                {
                                    HasPropTranslateDrag = false;
                                    WorldFileManager.SelectedProp.Position = PropOriginalPosition;
                                    WorldFileManager.SelectedProp.SetRotation(PropOriginalAngles);
                                }
                                else if (HasPropCollision)
                                {
                                    if (WorldFileManager.SelectedProp != null)
                                    {
                                        CollisionResult ClosestProp = PropCollisions.GetClosest();
                                        Prop CollisionProp = WorldFileManager.MasteryFile.GetProp(ClosestProp.GeometryName, ClosestProp.InstanceID);
                                        if (CollisionProp != null)
                                        {

                                            if (CollisionProp != WorldFileManager.SelectedProp)
                                            {
                                                WorldFileManager.SelectedProp.HighLight = 0;
                                                WorldFileManager.SelectedProp = CollisionProp;

                                                PropOriginalPosition = CollisionProp.Position;
                                                PropOriginalAngles = CollisionProp.Angles;
                                                //set highlight
                                                WorldFileManager.SelectedProp.HighLight = 1;
                                                PSX = WorldFileManager.SelectedProp.Scale.X;
                                                PSY = WorldFileManager.SelectedProp.Scale.Y;
                                                PSZ = WorldFileManager.SelectedProp.Scale.Z;
                                                PRX = WorldFileManager.SelectedProp.Angles.X;
                                                PRY = WorldFileManager.SelectedProp.Angles.Y;
                                                PRZ = WorldFileManager.SelectedProp.Angles.Z;
                                                HasModify = true;
                                            }
                                        }
                                        else
                                        {
                                            WorldFileManager.SelectedProp.HighLight = 0;
                                            WorldFileManager.SelectedProp = null;
                                            HasModify = true;
                                        }
                                    }
                                    else
                                    {
                                        CollisionResult ClosestProp = PropCollisions.GetClosest();
                                        Prop CollisionProp = WorldFileManager.MasteryFile.GetProp(ClosestProp.GeometryName, ClosestProp.InstanceID);

                                        PropOriginalPosition = new Vector3(CollisionProp.Position.X, CollisionProp.Position.Y, CollisionProp.Position.Z);
                                        PropOriginalAngles = new Vector3(CollisionProp.Angles.X, CollisionProp.Angles.Y, CollisionProp.Angles.Z);

                                        WorldFileManager.SelectedProp = CollisionProp;
                                        WorldFileManager.SelectedProp.HighLight = 1;
                                        PSX = WorldFileManager.SelectedProp.Scale.X;
                                        PSY = WorldFileManager.SelectedProp.Scale.Y;
                                        PSZ = WorldFileManager.SelectedProp.Scale.Z;
                                        PRX = WorldFileManager.SelectedProp.Angles.X;
                                        PRY = WorldFileManager.SelectedProp.Angles.Y;
                                        PRZ = WorldFileManager.SelectedProp.Angles.Z;
                                        HasModify = true;
                                    }
                                }
                                else
                                {
                                    if (WorldFileManager.SelectedProp != null)
                                    {
                                        //deselect Prop
                                        WorldFileManager.SelectedProp.HighLight = 0;
                                        WorldFileManager.SelectedProp = null;
                                        PropOriginalPosition = new Vector3();
                                        PropOriginalAngles = new Vector3();
                                        HasModify = true;
                                    }
                                }
                            }


                        }
                        else if (SelectedTool == 2)
                        {
                            //WordFile Tools
                            Vector2 Click2D = new Vector2(MX, MY);
                            Vector3 Direction = CollisionUtil.GetDirection(Click2D, CameraManager.Camera, Render.Graphics);
                            WorldFileManager.SetSelectedChunk(CameraManager.Camera.Position, Direction);
                        }
                    }
                }
                if (HasModify)
                {
                    LastTerrainModify = 0;
                }
            }
            LastTerrainModify += GameTime.ElapsedGameTime.Milliseconds;
            LastTerrainModify %= 10000;
            
            if (WorldFileManager.CurrentWorldFile != null)
            {
                WorldFileScale.Text = "S: " + String.Format("{0:0.00}", WorldFileManager.CurrentWorldFile.TerrainScale);
                
                FoamRampButton0.Text = "Foam:" + FoamRamp;
                
                WorldFilePosX.UpdateGeometry();
                WorldFilePosY.UpdateGeometry();
                WorldFileScale.UpdateGeometry();
                FoamRampButton0.UpdateGeometry();
            }

            if (WorldFileManager.MasteryFile != null)
            {
                //move CHUNK PDX to here

                //Vector3 LookAtPosition = CameraManager.Camera.ViewDirection * (WorldFileManager.MasteryFile.Settings.MaxWorldFileSize / 2);

                //WorldFileManager.SelectedChunkX = (int)Math.Floor((CameraManager.Position.X + LookAtPosition.X) / WorldFileManager.MasteryFile.Settings.MaxWorldFileSize);
                //WorldChunkPY = (int)Math.Floor((CameraManager.Position.Y + LookAtPosition.Y) / WorldFileManager.MasteryFile.Settings.MaxWorldFileSize);
                
                WorldFileManager.MasteryFile.Settings.LODMode = (int)LODSelector.GetValue()[2];
                WorldFileManager.SwitchActiveWorldFileLOD(WorldFileManager.MasteryFile.Settings.LODMode - 2);

                WorldFilePosX.Text = "PX: " + String.Format("{0:0.00}", WorldFileManager.SelectedChunkX);
                WorldFilePosY.Text = "PY: " + String.Format("{0:0.00}", WorldFileManager.SelectedChunkY);
                WorldFilePosX.UpdateGeometry();
                WorldFilePosY.UpdateGeometry();

                //add prop selection
            }

            if (WorldFileManager.SelectedProp != null)
            {
                SXDisplay.Text = String.Format("{0:0.00}", PSX);
                SYDisplay.Text = String.Format("{0:0.00}", PSY);
                SZDisplay.Text = String.Format("{0:0.00}", PSZ);
                RXDisplay.Text = String.Format("{0:0.00}", PRX);
                RYDisplay.Text = String.Format("{0:0.00}", PRY);
                RZDisplay.Text = String.Format("{0:0.00}", PRZ);
                
            }
            else
            {
                SXDisplay.Text = String.Format("{0:0.00}", SX);
                SYDisplay.Text = String.Format("{0:0.00}", SY);
                SZDisplay.Text = String.Format("{0:0.00}", SZ);
                RXDisplay.Text = String.Format("{0:0.00}", RX);
                RYDisplay.Text = String.Format("{0:0.00}", RY);
                RZDisplay.Text = String.Format("{0:0.00}", RZ);
            }
            SXDisplay.UpdateGeometry();
            SYDisplay.UpdateGeometry();
            SZDisplay.UpdateGeometry();
            RXDisplay.UpdateGeometry();
            RYDisplay.UpdateGeometry();
            RZDisplay.UpdateGeometry();

            bool HasTerrainTool = SelectedTool == 0;
            bool HasPropTool = SelectedTool == 1;

            TerrainToolSelector.ToggleVisibility(HasTerrainTool);
            StructureToolSelector.ToggleVisibility(HasPropTool);
            ShovelShapeSelector.ToggleVisibility( HasTerrainTool || (HasPropTool && SelectedPropTool == 0));
            ShovelRadius.ToggleVisibility(HasTerrainTool || (HasPropTool && SelectedPropTool == 0));
            
            //Terrrain Tool
            //HeightMap
            ShovelToolSelector.ToggleVisibility(SelectedTerrainTool == 0 && HasTerrainTool);
            HeightFlow.ToggleVisibility( (SelectedTerrainTool == 0 || SelectedTerrainTool == 1 || SelectedTerrainTool == 2 || SelectedWaterMaterialTool == 1 || SelectedWaterMaterialTool == 2) && HasTerrainTool);
            WaterHeightAdjustToggle.ToggleVisibility(SelectedTerrainTool == 0 && HasTerrainTool);

            //Material
            BlendValueButton.ToggleVisibility( (SelectedTerrainTool == 1 || SelectedTerrainTool == 3) && SelectedWaterMaterialTool != 1 && HasTerrainTool);
            
            TerrainMaterialToolSelector.ToggleVisibility(SelectedTerrainTool == 1 && HasTerrainTool);
            TerrainMainMaterialToolSelector.ToggleVisibility(SelectedMaterialTool == 0 && SelectedTerrainTool == 1 && HasTerrainTool);
            TerrainSecondaryMaterialToolSelector.ToggleVisibility(SelectedMaterialTool == 1 && SelectedTerrainTool == 1 && HasTerrainTool);
            TerrainDecayToolSelector.ToggleVisibility(SelectedMaterialTool == 2 && SelectedTerrainTool == 1 && HasTerrainTool);

            MaterialSelector.ToggleVisibility((SelectedMaterialTool == 0 || SelectedMaterialTool == 1) && SelectedTerrainTool == 1 && HasTerrainTool);
            DecalSelector.ToggleVisibility(SelectedMaterialTool == 2 && SelectedTerrainTool == 1 && HasTerrainTool);

            //Water
            WaterHeightToolSelector.ToggleVisibility(SelectedTerrainTool == 2 && HasTerrainTool);
            WaterMaterialToolSelector.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            //Flow
            FlowMapSelector.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            FlowMapX.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            FlowMapY.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            //Foam
            WaveToolSelector.ToggleVisibility(SelectedWaterMaterialTool == 0 && SelectedTerrainTool == 3 && HasTerrainTool);
            FoamToolSelector.ToggleVisibility(SelectedWaterMaterialTool == 1 && SelectedTerrainTool == 3 && HasTerrainTool);
            WaterColorToolSelector.ToggleVisibility(SelectedWaterMaterialTool == 2 && SelectedTerrainTool == 3 && HasTerrainTool);
            
            FoamRampButton0.ToggleVisibility(SelectedWaterMaterialTool == 1 && SelectedTerrainTool == 3 && HasTerrainTool);
            //WaterColor
            ColorTextR.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextG.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextB.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextA.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextRV.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextGV.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextBV.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            ColorTextAV.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            WaterColorPicker.ToggleVisibility(SelectedTerrainTool == 3 && HasTerrainTool);
            //End Terrain Tool

            //Structure/Prop Tool
            SXDisplay.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            SYDisplay.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            SZDisplay.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            RXDisplay.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            RYDisplay.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            RZDisplay.ToggleVisibility(HasPropTool && SelectedPropTool == 1);

            SelectOnCreationButton.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            PlaneLockButton.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            TBD3.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            TBD4.ToggleVisibility(HasPropTool && SelectedPropTool == 1);

            PropList.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            SearchPlaceHolder.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            PropListUp.ToggleVisibility(HasPropTool && SelectedPropTool == 1);
            PropListDown.ToggleVisibility(HasPropTool && SelectedPropTool == 1);

            //EndStructure/Prop Tool

            WorldFileToolSelector.ToggleVisibility(SelectedTool == 2);
            NewWorldFile.ToggleVisibility(SelectedTool == 2);

            string[] PlaneLockAxis = new string[] { "--", "Z", "Y", "X" };
            PlaneLockButton.Text = PlaneLockAxis[PlaneLock];
            
            BlendValueButton.Text = "A:" + AlphaValue;

            ShovelRadius.Text = "R:" + String.Format("{0:0.00}", ShovelR);
            HeightFlow.Text = "Flow:" + TerrainHeightFlow;

            FlowMapX.Text = "X:" + (FlowX - 127);
            FlowMapY.Text = "Y:" + (FlowY - 127);

            ColorR = WaterColorPicker.RX;
            ColorG = WaterColorPicker.GX;
            ColorB = WaterColorPicker.BX;
            ColorA = WaterColorPicker.AX;

            ColorTextRV.Text = ColorR + "";
            ColorTextGV.Text = ColorG + "";
            ColorTextBV.Text = ColorB + "";
            ColorTextAV.Text = ColorA + "";

            ColorTextRV.UpdateGeometry();
            ColorTextGV.UpdateGeometry();
            ColorTextBV.UpdateGeometry();
            ColorTextAV.UpdateGeometry();

            PlaneLockButton.UpdateGeometry();
            BlendValueButton.UpdateGeometry();
            ShovelRadius.UpdateGeometry();
            HeightFlow.UpdateGeometry();
            FlowMapX.UpdateGeometry();
            FlowMapY.UpdateGeometry();
            WaterColorPicker.UpdateGeometry();
        
            Menu.CrosshairState = !CameraMode.IsToggled ? Menu.CrosshairState : 1;
            WorldFileManager.ToggleGridVisibility(GridToggle.IsToggled);
            CameraManager.LookatCube.Geom.HasCull = !CubeToggle.IsToggled;
            


        }
    }
}
