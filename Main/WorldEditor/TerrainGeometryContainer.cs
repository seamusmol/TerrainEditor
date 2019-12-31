using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Main.Geometry;

namespace Main.WorldEditor
{
    public class TerrainGeometryContainer
    {
        
        public WorldFile WorldFile { get; }

        Texture2D MaterialData0;
        Texture2D MaterialData1;

        //TextureTask GenerateNormals;
        
        public Texture2D HeightMap { get; private set; }

        public Geometry3D TerrainGeometry;
        
        string Name;
        public bool IsActive { get; private set; } = true;

        public TerrainGeometryContainer(RenderManager RenderManager, SettingsContainer WorldSettings, WorldFile File)
        {
            WorldFile = File;
            Name = "Terrain: " + File.FileName;
            InitializeGeometry(RenderManager, WorldSettings);

            MaterialData0 = new Texture2D(RenderManager.Graphics, WorldFile.MaterialMap.GetLength(0), File.MaterialMap.GetLength(1));
            MaterialData1 = new Texture2D(RenderManager.Graphics, WorldFile.MaterialMap.GetLength(0), File.MaterialMap.GetLength(1));
            HeightMap = new Texture2D(RenderManager.Graphics, WorldFile.SX + 1, WorldFile.SY + 1, false, SurfaceFormat.Color);
            
            //GenerateNormals = new TextureTask(Name + "-GenerateNormals",TaskType.GenerateNormals, TaskStage.PreRenderTarget, TaskUsage.FrameLimit, TaskSwitchType.Ping);
            //GenerateNormals.AddParameter(TextureParameter.InputTexture, HeightMap);
            //GenerateNormals.AddParameter(FloatParameter.TextureWidth0, WorldFile.SX + 1);
            //GenerateNormals.AddParameter(FloatParameter.TextureHeight0, WorldFile.SY + 1);

            //GenerateNormals.SetRenderTarget(NormalMap);
            //GenerateNormals.AttachOutPut("NormalMap", TerrainGeometry.Shader);

            //RenderManager.AddTextureModificationTask(GenerateNormals);

        }

        public void SetIsActive(bool Value)
        {
            IsActive = Value;
            TerrainGeometry.HasCull = !Value;
        }

        
        public void Update(RenderManager Render, SettingsContainer WorldSettings, Vector3 Position)
        {
            int[] LOD = WorldSettings.LOD;
            Vector3 FilePosition = WorldFile.GetPosition();
            float Distance = Math.Min(Math.Abs(Position.X - FilePosition.X), Math.Abs(Position.Y - FilePosition.Y));

            /*
            int LODIndex = 0;
            for (int j = 0; j < LOD.Length; j++)
            {
                if (LOD[j] < Distance)
                {
                    LODIndex = j;
                }
            }
            

            if (LODIndex != CurrentLOD)
            {
                GenerateGeometry(Graphics, WorldSettings, Position);
            }
            */

            if (MaterialData0.Width != WorldFile.MaterialMap.GetLength(0) || MaterialData0.Height != WorldFile.MaterialMap.GetLength(1))
            {
                MaterialData0.Dispose();
                MaterialData1.Dispose();
                MaterialData0 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.MaterialMap, WorldFile.SecondaryMaterialMap, WorldFile.DecalMaterialMap, WorldFile.DecalMaterialMap);
                MaterialData1 = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, WorldFile.BlendAlphaMap, WorldFile.DecalAlphaMap, WorldFile.DecalAlphaMap, WorldFile.DecalAlphaMap);
            }

            if (WorldFile.HasTerrainUpdate)
            {
                TerrainGeometry.DepthShader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
                TerrainGeometry.Shader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
                TerrainGeometry.DepthShader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
                TerrainGeometry.DepthShader.Parameters["TerrainHeight"].SetValue(WorldFile.SY);
               
                TerrainGeometry.DepthShader.Parameters["HeightScale"].SetValue(WorldFile.HeightScale);
                TerrainGeometry.Shader.Parameters["HeightScale"].SetValue(WorldFile.HeightScale);

                //HeightMap = GraphicsUtil.MaterialMapsToTexture(Render.Graphics, SurfaceFormat.Color, WorldFile.HeightMap);
                
                GraphicsUtil.FillTexture(HeightMap, WorldFile.HeightMap);
                
                GraphicsUtil.FillTexture(MaterialData0, WorldFile.MaterialMap, WorldFile.SecondaryMaterialMap, WorldFile.DecalMaterialMap, WorldFile.DecalMaterialMap);
                GraphicsUtil.FillTexture(MaterialData1, WorldFile.BlendAlphaMap, WorldFile.DecalAlphaMap, WorldFile.DecalAlphaMap, WorldFile.DecalAlphaMap);
                //
                TerrainGeometry.Shader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
                TerrainGeometry.Shader.Parameters["TerrainHeight"].SetValue(WorldFile.SY);
                TerrainGeometry.Shader.Parameters["MaterialWidth"].SetValue(WorldFile.MaterialMap.GetLength(0));
                TerrainGeometry.Shader.Parameters["MaterialHeight"].SetValue(WorldFile.MaterialMap.GetLength(1));

                TerrainGeometry.Shader.Parameters["MaterialData0"].SetValue(MaterialData0);
                TerrainGeometry.Shader.Parameters["MaterialData1"].SetValue(MaterialData1);
                //add Material Map
                TerrainGeometry.Shader.Parameters["HeightMap"].SetValue(HeightMap);
                TerrainGeometry.DepthShader.Parameters["HeightMap"].SetValue(HeightMap);
                //GenerateNormals.FrameLimit++;
                //GenerateNormals.AddParameter(TextureParameter.InputTexture, HeightMap);
                //Render.AddTextureModificationTask(GenerateNormals);
            }
            
            //Vector3 ChunkPosition = new Vector3(WorldFile.IDX * WorldFile.SX * WorldFile.TerrainScale, WorldFile.IDY * WorldFile.SY * WorldFile.TerrainScale, 0);
            //TerrainGeometry.Shader.Parameters["Position"].SetValue(ChunkPosition);

            Vector3 LightPosition = new Vector3(0,0,20);

            TerrainGeometry.Shader.Parameters["CameraPosition"].SetValue(Position);
            TerrainGeometry.Shader.Parameters["LightPosition"].SetValue(WorldSettings.DebugLightPosition);
            TerrainGeometry.Shader.Parameters["LightColor"].SetValue(WorldSettings.DebugDirectLightColor);
            TerrainGeometry.Shader.Parameters["AmbientLightColor"].SetValue(WorldSettings.DebugAmbientLightColor);
            TerrainGeometry.Position = WorldFile.GetPosition();
        }

        public void SetHighLight(Vector3 BrushPosition, Vector4 Color, int BrushShape,  float Range)
        {
            TerrainGeometry.Shader.Parameters["BrushPosition"].SetValue(BrushPosition);
            TerrainGeometry.Shader.Parameters["BrushHighLightColor"].SetValue(Color);
            TerrainGeometry.Shader.Parameters["BrushShape"].SetValue(BrushShape);
            TerrainGeometry.Shader.Parameters["BrushDistance"].SetValue(Range);
        }
        
        public void InitializeGeometry(RenderManager RenderManager, SettingsContainer WorldSettings)
        {
            TerrainGeometry = new Geometry3D(Name);

            TerrainGeometry.Shader = RenderManager.Shaders["Terrain"].Clone();
            TerrainGeometry.DepthShader = RenderManager.Shaders["Depth-Terrain"].Clone();

            TerrainGeometry.Shader.Parameters["TextureSize"].SetValue(1.0f / WorldSettings.TextureSize);

            TerrainGeometry.DepthShader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
            TerrainGeometry.Shader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
            
            TerrainGeometry.Shader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
            TerrainGeometry.Shader.Parameters["TerrainHeight"].SetValue(WorldFile.SY);
            TerrainGeometry.DepthShader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
            TerrainGeometry.DepthShader.Parameters["TerrainHeight"].SetValue(WorldFile.SY);

            TerrainGeometry.Shader.Parameters["MaterialWidth"].SetValue(WorldFile.MaterialMap.GetLength(0));
            TerrainGeometry.Shader.Parameters["MaterialHeight"].SetValue(WorldFile.MaterialMap.GetLength(1));

            TerrainGeometry.Shader.Parameters["DiffuseMap"].SetValue(RenderManager.Textures["Terrain-Diffuse"]);
            TerrainGeometry.Shader.Parameters["RoughnessMap"].SetValue(RenderManager.Textures["Terrain-Roughness"]);
            TerrainGeometry.Shader.Parameters["MaterialNormalMap"].SetValue(RenderManager.Textures["Terrain-Normal"]);
            TerrainGeometry.Shader.Parameters["MetalnessMap"].SetValue(RenderManager.Textures["Terrain-Metalness"]);
            
            TerrainGeometry.Shader.Parameters["DecalDiffuseMap"].SetValue(RenderManager.Textures["TerrainDecal-Diffuse"]);
            TerrainGeometry.Shader.Parameters["DecalMetalnessMap"].SetValue(RenderManager.Textures["TerrainDecal-Roughness"]);
            TerrainGeometry.Shader.Parameters["DecalNormalMap"].SetValue(RenderManager.Textures["TerrainDecal-Normal"]);
            TerrainGeometry.Shader.Parameters["DecalRoughnessMap"].SetValue(RenderManager.Textures["TerrainDecal-Roughness"]);
            TerrainGeometry.Shader.Parameters["DecalToleranceMap"].SetValue(RenderManager.Textures["TerrainDecal-Tolerance"]);
            
            Vector3 ChunkPosition = new Vector3(WorldFile.IDX * WorldFile.SX * WorldFile.TerrainScale, WorldFile.IDY * WorldFile.SY * WorldFile.TerrainScale, 0);
            TerrainGeometry.Shader.Parameters["Position"].SetValue(ChunkPosition);
            TerrainGeometry.DepthShader.Parameters["Position"].SetValue(ChunkPosition);

            TerrainGeometry.HasCull = false;
            TerrainGeometry.Position = WorldFile.GetPosition();
            TerrainGeometry.RenderBucket = Geometry3D.RenderQueue.Solid;
            TerrainGeometry.CurrentRenderMode = Geometry3D.RenderMode.NonIndexBufferless;
            TerrainGeometry.VertexCount = (WorldFile.HeightMap.GetLength(0) - 1) * (WorldFile.HeightMap.GetLength(1) - 1) * 6;

        }
    }
}
