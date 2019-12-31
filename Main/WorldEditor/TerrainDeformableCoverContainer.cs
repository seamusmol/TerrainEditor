using Main.Geometry;
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

namespace Main.WorldEditor
{
    public class TerrainDeformableCoverContainer
    {
        TerrainGeometryContainer TerrainBottom;
        
        SceneRenderTarget FromBelowDepthTarget;
        OffScreenTarget DisplacementSimulator;

        RenderTarget2D DisplacementTarget;
        
        public WorldFile WorldFile { get; private set; }
        public SettingsContainer WorldSettings { get; private set; }
        public Geometry3D Geom;
        public string Name { get; private set; }
        public bool IsActive { get; set; } = true;

        public TerrainDeformableCoverContainer(RenderManager RenderManager, WorldFile File, TerrainGeometryContainer TerrainGeometry, SettingsContainer WorldSettings, SceneRenderTarget FromBelowDepth)
        {
            WorldFile = File;
            Name = "Cover: " + File.FileName;
            InitializeGeometry(RenderManager);

            FromBelowDepthTarget = FromBelowDepth;
            
            TerrainBottom = TerrainGeometry;

            DisplacementTarget = new RenderTarget2D(RenderManager.Graphics, FromBelowDepthTarget.GetWidth(), FromBelowDepthTarget.GetHeight(), false, SurfaceFormat.Single,DepthFormat.Depth24Stencil8);

            DisplacementSimulator = new OffScreenTarget("Displacement", RenderManager.Graphics, RenderManager.Shaders["TerrainCoverDisplacement"].Clone());
            
            DisplacementSimulator.SetTargetPing( DisplacementTarget);

            DisplacementSimulator.AttachOutPut("HeightMap", Geom.Shader);
            DisplacementSimulator.AddParameter("HeightMap", TerrainGeometry.HeightMap);
            DisplacementSimulator.AddParameter("FarPlane", WorldSettings.FromBelowFarPlane);
            DisplacementSimulator.AddParameter("DepthOffset", WorldFile.TerrainDepth);

            DisplacementSimulator.DebugTextureName = "SceneDepthDebug";

            FromBelowDepthTarget.Attach(DisplacementSimulator.Shader);
            RenderManager.AddOffScreenTarget(DisplacementSimulator);
        }
        
        public void Update()
        {
            if (WorldFile.HasTerrainUpdate)
            {
                //set HeightMap
                Geom.Shader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
                Geom.Shader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
                Geom.Shader.Parameters["TerrainHeight"].SetValue(WorldFile.SY);
                //Geom.Shader.Parameters["HeightMap"].SetValue(TerrainBottom.HeightMap);
                Geom.Shader.Parameters["HeightScale"].SetValue(WorldFile.HeightScale);
                
            }
        }

        public void SetIsActive(bool Value)
        {
            IsActive = Value;
            Geom.HasCull = !Value;
        }

        public void InitializeGeometry(RenderManager RenderManager)
        {
            Geom = new Geometry3D(Name);

            Geom.Shader = RenderManager.Shaders["TerrainCover"].Clone();
            
            Geom.Shader.Parameters["TerrainScale"].SetValue(WorldFile.TerrainScale);
            Geom.Shader.Parameters["TerrainWidth"].SetValue(WorldFile.SX);
            Geom.Shader.Parameters["TerrainHeight"].SetValue(WorldFile.SY);
            
            Geom.Shader.Parameters["HeightScale"].SetValue(WorldFile.HeightScale);
           
            Vector3 ChunkPosition = new Vector3(WorldFile.IDX * WorldFile.SX * WorldFile.TerrainScale, WorldFile.IDY * WorldFile.SY * WorldFile.TerrainScale, 0);
            Geom.Shader.Parameters["Position"].SetValue(ChunkPosition);

            Geom.HasCull = false;
            Geom.Position = WorldFile.GetPosition();
            Geom.RenderBucket = Geometry3D.RenderQueue.Transparent;
            Geom.CurrentRenderMode = Geometry3D.RenderMode.NonIndexBufferless;
            Geom.VertexCount = (WorldFile.HeightMap.GetLength(0) - 1) * (WorldFile.HeightMap.GetLength(1) - 1) * 6;

        }

        
        

    }
}
