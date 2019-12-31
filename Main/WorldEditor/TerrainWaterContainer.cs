using Main.Geometry;
using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{
    //TODO
    //Add System for coloring water
    //Add Blend map for debris floating on water
    //Add System for water movement(flow map)

    //Water Normal Map

    //use Texture for
    //1 byte: FlowX
    //2 byte: FlowY
    //3 byte: Noise
    //4 

    //TODO
    //Shader Steps
    //Implement Bump Map to Color
    //Add movement to BumpMap using Time

    public class TerrainWaterContainer
    {
        public WorldFile WorldFile { get; }
        public SettingsContainer WorldSettings { get; set; }
        public TerrainGeometryContainer Terrain {get;set;}
        public Geometry3D Geom;

        public bool IsActive { get; private set; } = true;

        public int CurrentLOD { get; private set; } = 0;
        string Name;

        float TimeCycle = 0;
        public SceneRenderTarget ReflectionRenderTarget { get; private set; }
        public SceneRenderTarget RefractionRenderTarget { get; private set; }
        public SceneRenderTarget DepthMapRenderTarget { get; private set; }
        
        Texture2D WaterData0;
        Texture2D WaterData1;
        Texture2D WaterColorData0;
        Texture2D WaterColorData1;
        //Texture2D FoamDataMap0;
        Texture2D WaveData0;
        
        public Texture2D WaterHeightMap { get; set; }
        public Texture2D WaterClipMap { get; set; }

        //TextureTask MergeDepth;
        //TextureTask SetHeight;

        //OffScreenTarget MergeDepth;
        //OffScreenTarget WaterSimulation;
        //OffScreenTarget NormalGenerator;
        
        SceneRenderTarget FromBelowDepth;

        //RenderTarget2D DepthTarget0;
        //RenderTarget2D DepthTarget1;

       // RenderTarget2D NormalMap;

        public Vector3 Position { get; private set; }
        //bool FrameCycle = false;
        
        public TerrainWaterContainer(RenderManager RenderManager,TerrainGeometryContainer TerrainGeometry, Node3D ReflectionNode, Node3D RefractionNode, Node3D PropNode, WorldFile File, SettingsContainer Settings, Vector3 Pos, SceneRenderTarget ReflectionTarget, SceneRenderTarget RefractionTarget, SceneRenderTarget DepthTarget, SceneRenderTarget FromBelowTarget)
        {
            WorldFile = File;
            WorldSettings = Settings;
            Terrain = TerrainGeometry;
            Name = "Water: " + File.FileName;
            Position = Pos;
            
            ReflectionRenderTarget = ReflectionTarget;
            RefractionRenderTarget = RefractionTarget;
            DepthMapRenderTarget = DepthTarget;
            
            WaterData0 = new Texture2D(RenderManager.Graphics, WorldFile.FlowXMap.GetLength(0), WorldFile.FlowXMap.GetLength(1));
            WaterData1 = new Texture2D(RenderManager.Graphics, WorldFile.WaterMap.GetLength(0), WorldFile.WaterMap.GetLength(1), false, SurfaceFormat.Alpha8);

            WaterColorData0 = new Texture2D(RenderManager.Graphics, WorldFile.WaterColorR.GetLength(0), WorldFile.WaterColorR.GetLength(1));
            WaterColorData1 = new Texture2D(RenderManager.Graphics, WorldFile.WaterNormalMap.GetLength(0), WorldFile.WaterNormalMap.GetLength(1));
            //FoamDataMap0 = new Texture2D(RenderManager.Graphics, WorldFile.FoamRampMap0.GetLength(0), WorldFile.FoamRampMap0.GetLength(1));
            WaveData0 = new Texture2D(RenderManager.Graphics, WorldFile.WaveHeightMap.GetLength(0), WorldFile.WaveHeightMap.GetLength(1));

            InitializeGeometry(RenderManager);
            ReflectionRenderTarget.Attach(Geom);
            RefractionRenderTarget.Attach(Geom);
            DepthMapRenderTarget.Attach(Geom);
            
            //Depth Input
            FromBelowDepth = FromBelowTarget;
            
            //DepthTarget0 = new RenderTarget2D(RenderManager.Graphics, 1025, 1025, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
            //DepthTarget1 = new RenderTarget2D(RenderManager.Graphics, 1025, 1025, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);

            //SetHeight = new TextureTask(Name + "SetHeight", TaskType.FillFloat, TaskStage.PreRenderTarget, TaskUsage.FrameLimit,TaskSwitchType.PingPongOut);
            //SetHeight.AddParameter(FloatParameter.ColorFloat, WorldFile.TerrainDepth / Settings.FromBelowFarPlane);
            //SetHeight.SetRenderTarget(DepthTarget0, true);
            //SetHeight.SetRenderTarget(DepthTarget1, false);
            //SetHeight.FrameLimit = 2;
            
            //Begin PingPong
            //Depth 
            //MergeDepth = new TextureTask("MergeDepth", TaskType.MergeMinFloat, TaskStage.PreOffScreenTarget, TaskUsage.Preserve, TaskSwitchType.PingPongIn);
           // MergeDepth.AddParameter(TextureParameter.InputTexture, FromBelowDepth.RenderTarget);

            //MergeDepth.AddParameter(TextureParameter.InputTexture2, (Texture2D)DepthTarget0, true);
            //MergeDepth.AddParameter(TextureParameter.InputTexture2, (Texture2D)DepthTarget1, false);
            //Current Frame Texture
            //MergeDepth.SetRenderTarget(DepthTarget1, true);
            //MergeDepth.SetRenderTarget(DepthTarget0, false);
            
            //WaterSimulation = new OffScreenTarget(RenderManager.Graphics, RenderManager.Shaders["WaterSimulation"].Clone());
            //NormalGenerator = new OffScreenTarget(RenderManager.Graphics, RenderManager.Shaders["WaterSimulation"].Clone());
            //NormalMap = new RenderTarget2D(RenderManager.Graphics, 1025, 1025);
            
            //FromBelowDepth.Attach(MergeDepth);
            //WaterSimulation.Attach(NormalGenerator.Shader);
            //WaterSimulation.Attach(Geom.Shader);
            WaterHeightMap = new Texture2D(RenderManager.Graphics, WorldFile.WaterHeightMap.GetLength(0), WorldFile.WaterHeightMap.GetLength(1) , false, SurfaceFormat.Color);
            WaterClipMap = new Texture2D(RenderManager.Graphics, WorldFile.WaterHeightMap.GetLength(0), WorldFile.WaterHeightMap.GetLength(1), false, SurfaceFormat.Color);
            //MergeDepth.DebugTextureName = "SceneWaterReflectionDebug";

            //NormalGenerator.Attach(Geom.Shader);
            //RenderManager.AddTextureModificationTask(SetHeight);
            //RenderManager.AddTextureModificationTask(MergeDepth);
            
            //RenderManager.AddOffScreenTarget(WaterSimulation);
            //RenderManager.AddOffScreenTarget(NormalGenerator);
            
        }
        
        public void UpdatePreRender(Vector3 NewPosition, Vector3 ViewDirection, float Time)
        {
            TimeCycle = Time;
            TimeCycle %= WorldSettings.WaterRepeatTime * WorldSettings.WaterSpeed * WorldSettings.TextureSize * 60.0f;
            
            Geom.Shader.Parameters["LightPosition"].SetValue(WorldSettings.DebugLightPosition);
            Geom.Shader.Parameters["CameraPosition"].SetValue(NewPosition);
            Geom.Shader.Parameters["Time"].SetValue(TimeCycle);
            
            GenerateWaterReflectionPoint(NewPosition, ViewDirection);

            //Depth Target Positions

            int PX = (int)Math.Floor( NewPosition.X / (WorldFile.SX * WorldFile.TerrainScale));
            int PY = (int)Math.Floor( NewPosition.Y / (WorldFile.SY * WorldFile.TerrainScale));
            
            if (PX == WorldFile.IDX && PY == WorldFile.IDY)
            {
                FromBelowDepth.SetPositionDirection(Position + new Vector3(WorldSettings.ChunkSize/2, WorldSettings.ChunkSize / 2, -WorldFile.TerrainDepth), Vector3.Normalize( new Vector3(0, 0, 1)), new Vector3(0, -1, 0), true);
            }
        }

        public void GenerateWaterReflectionPoint(Vector3 CameraPosition, Vector3 ViewDirection)
        {
            float WaterHeight = (WorldFile.HeightMap[WorldFile.HeightMap.GetLength(0) / 2, WorldFile.HeightMap.GetLength(1) / 2] + WorldFile.WaterHeightMap[WorldFile.WaterHeightMap.GetLength(0) / 2, WorldFile.WaterHeightMap.GetLength(1) / 2]) * WorldFile.HeightScale;
            float ReflectionHeight = WaterHeight - (CameraPosition.Z - WaterHeight);

            Vector3 CameraPositionB = new Vector3(CameraPosition.X, CameraPosition.Y, ReflectionHeight);
            Vector3 ViewDirectionB = new Vector3(ViewDirection.X, ViewDirection.Y, ViewDirection.Z * -1.0f);

            ReflectionRenderTarget.SetPositionDirection(CameraPositionB, ViewDirectionB, new Vector3(0, 0, 1));
            RefractionRenderTarget.SetPositionDirection(CameraPosition, ViewDirection, new Vector3(0, 0, 1));

            DepthMapRenderTarget.SetPositionDirection(CameraPosition, ViewDirection, new Vector3(0, 0, 1));
        }

        public void SetIsActive(bool Value)
        {
            IsActive = Value;
            Geom.HasCull = !Value;
        }

        public void Update(RenderManager Render, Vector3 Position, Vector3 ViewDirection, float Time)
        {
            int[] LOD = WorldSettings.LOD;
            Vector3 FilePosition = WorldFile.GetPosition();
            float Distance = Math.Min(Math.Abs(Position.X - FilePosition.X), Math.Abs(Position.Y - FilePosition.Y));

            //TODO 
            if (WaterData0.Width != WorldFile.WaterMap.GetLength(0) || WaterData0.Height != WorldFile.WaterMap.GetLength(1))
            {
                WaterData0.Dispose();
                WaterData1.Dispose();
                WaterColorData0.Dispose();
                WaterColorData1.Dispose();

                WaterData0 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.FlowXMap, WorldFile.FlowYMap, WorldFile.FlowPulseSpeedMap, WorldFile.FlowBackTimeMap);
                WaterData1 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.WaterMap);

                WaterColorData0 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.WaterColorR, WorldFile.WaterColorG, WorldFile.WaterColorB, WorldFile.WaterColorA);
                WaterColorData1 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.WaterNormalMap, WorldFile.WaterFresnelMap, WorldFile.FoamRampMap0, WorldFile.WaterColorFalloffMap);
                WaveData0 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.WaveLengthMap, WorldFile.WaveHeightMap, WorldFile.WaveHeightMap, WorldFile.WaveLengthMap);
            }

            if (WorldFile.HasTerrainUpdate)
            {
                Geom.Shader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
                Geom.Shader.Parameters["HeightScale"].SetValue(WorldFile.HeightScale);

                Geom.Shader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
                Geom.Shader.Parameters["TerrainLength"].SetValue(WorldFile.SY);

                Geom.Shader.Parameters["DataWidth"].SetValue(WorldFile.FlowXMap.GetLength(0));
                Geom.Shader.Parameters["DataHeight"].SetValue(WorldFile.FlowXMap.GetLength(1));

                GraphicsUtil.FillTexture(WaterData0, WorldFile.FlowXMap, WorldFile.FlowYMap, WorldFile.FlowPulseSpeedMap, WorldFile.FlowBackTimeMap);
                GraphicsUtil.FillTexture(WaterData1, WorldFile.WaterMap);
                GraphicsUtil.FillTexture(WaterColorData0, WorldFile.WaterColorR, WorldFile.WaterColorG, WorldFile.WaterColorB, WorldFile.WaterColorA);
                GraphicsUtil.FillTexture(WaterColorData1, WorldFile.WaterNormalMap, WorldFile.WaterFresnelMap, WorldFile.FoamRampMap0, WorldFile.WaterColorFalloffMap);
                GraphicsUtil.FillTexture(WaveData0, WorldFile.WaveLengthMap, WorldFile.WaveHeightMap, WorldFile.FoamRampMap0, WorldFile.WaterColorFalloffMap);

                Geom.Shader.Parameters["WaterDataMap0"].SetValue(WaterData0);
                Geom.Shader.Parameters["WaterDataMap1"].SetValue(WaterData1);
                Geom.Shader.Parameters["ColorDataMap0"].SetValue(WaterColorData0);
                Geom.Shader.Parameters["ColorDataMap1"].SetValue(WaterColorData1);
                Geom.Shader.Parameters["WaveDataMap0"].SetValue(WaveData0);
                
                Geom.Shader.Parameters["HeightMap"].SetValue(Terrain.HeightMap);

                WaterHeightMap.Dispose();
                WaterHeightMap = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.WaterHeightMap);

                //GraphicsUtil.FillTexture(WaterHeightMap, WorldFile.WaterHeightMap);
                
                Geom.Shader.Parameters["WaterHeightMap"].SetValue(WaterHeightMap);
                
                WaterClipMap.Dispose();
                WaterClipMap = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, VoxelUtil.AddValues(WorldFile.HeightMap, WorldFile.WaterHeightMap));

                ReflectionRenderTarget.SetClipPlaneMap(Name, WaterClipMap);
                RefractionRenderTarget.SetClipPlaneMap(Name, WaterClipMap);
                DepthMapRenderTarget.SetClipPlaneMap(Name, WaterClipMap);
            }

            Geom.Shader.Parameters["ShineDamper"].SetValue(WorldSettings.WaterShineDamper);
            Geom.Shader.Parameters["Reflectivity"].SetValue(WorldSettings.WaterReflectivity);
            Geom.Shader.Parameters["MinSpecular"].SetValue(WorldSettings.MinimumSpecular);

            Geom.Shader.Parameters["WaterRepeatTime"].SetValue(WorldSettings.WaterRepeatTime);
            Geom.Shader.Parameters["WaterSpeed"].SetValue(WorldSettings.WaterSpeed);

            Geom.PassSetting = WorldSettings.WaterPassSetting;
            Geom.PassMipSetting = WorldSettings.WaterPassMipSetting;

            Geom.Shader.Parameters["TexSize"].SetValue(WorldSettings.TextureSize);
            Vector3 ChunkPosition = new Vector3(WorldFile.IDX * WorldFile.SX * WorldFile.TerrainScale, WorldFile.IDY * WorldFile.SX * WorldFile.TerrainScale, 0);
            Geom.Shader.Parameters["Position"].SetValue(ChunkPosition);

            Geom.Position = WorldFile.GetPosition();
        }
        
        public void InitializeGeometry(RenderManager RenderManager)
        {
            Geom = new Geometry3D(Name);

            Geom.Shader = RenderManager.Shaders["Water"].Clone();
            Geom.Shader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
            Geom.Shader.Parameters["HeightScale"].SetValue(WorldFile.HeightScale);

            Geom.Shader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
            Geom.Shader.Parameters["TerrainLength"].SetValue(WorldFile.SY);

            Geom.Shader.Parameters["WaveMap"].SetValue(RenderManager.Textures["WaveMap1"]);
            Geom.Shader.Parameters["WaveMap2"].SetValue(RenderManager.Textures["WaveMap2"]);
            Geom.Shader.Parameters["FoamMap"].SetValue(RenderManager.Textures["Foam"]);
            Geom.Shader.Parameters["NoiseMap"].SetValue(RenderManager.Textures["Noise"]);

            Geom.Shader.Parameters["NearPlane"].SetValue(RenderManager.Camera.NearPlaneClip);
            Geom.Shader.Parameters["FarPlane"].SetValue(RenderManager.Camera.FarPlaneClip);
            

            ReflectionRenderTarget.AddClipMap(Name, new Texture2D(RenderManager.Graphics, WorldFile.SX + 1, WorldFile.SY + 1), new Vector3(Position.X, Position.Y, Position.Z), new Vector4(WorldFile.SX * WorldFile.TerrainScale, WorldFile.SY * WorldFile.TerrainScale, WorldFile.HeightScale, 0));
            RefractionRenderTarget.AddClipMap(Name, new Texture2D(RenderManager.Graphics, WorldFile.SX + 1, WorldFile.SY + 1), new Vector3(Position.X, Position.Y, Position.Z), new Vector4(WorldFile.SX * WorldFile.TerrainScale, WorldFile.SY * WorldFile.TerrainScale, WorldFile.HeightScale, 0));
            DepthMapRenderTarget.AddClipMap(Name, new Texture2D(RenderManager.Graphics, WorldFile.SX + 1, WorldFile.SY + 1), new Vector3(Position.X, Position.Y, Position.Z), new Vector4(WorldFile.SX * WorldFile.TerrainScale, WorldFile.SY * WorldFile.TerrainScale, WorldFile.HeightScale, 0));

            ReflectionRenderTarget.SetClipMapPosition(Name, Position);
            RefractionRenderTarget.SetClipMapPosition(Name, Position);
            DepthMapRenderTarget.SetClipMapPosition(Name, Position);

            Geom.HasCull = false;
            Geom.Position = WorldFile.GetPosition();
            Geom.RenderBucket = Geometry3D.RenderQueue.Transparent;
            Geom.CurrentRenderMode = Geometry3D.RenderMode.NonIndexBufferless;
            Geom.VertexCount = (WorldFile.WaterHeightMap.GetLength(0) - 1) * (WorldFile.WaterHeightMap.GetLength(1) - 1) * 6;
        }
        
        public void SetHighLight(Vector3 BrushPosition, Vector4 Color, int BrushShape, float Range)
        {
            Geom.Shader.Parameters["BrushPosition"].SetValue(BrushPosition);
            Geom.Shader.Parameters["BrushHighLightColor"].SetValue(Color);
            Geom.Shader.Parameters["BrushShape"].SetValue(BrushShape);
            Geom.Shader.Parameters["BrushDistance"].SetValue(Range);
        }
        
    }
}
