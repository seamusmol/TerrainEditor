using Main.Geometry;
using Main.Main;
using Main.StructureEditor;
using Main.Structures;
using Main.Util;
using Main.WorldStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{
    //TODO
    //X-Terrain Geometry
    //Water Geometry

    //Vegetation Geometry
    //Terrain Cover Geometry
    
    //Prop Rendering
    //Structure Rendering
    
    class MasterFileRenderManager
    {
        RenderManager Render;
        WorldFileManager FileManager;
        public SettingsContainer Settings { get; set; }
        
        Node3D RenderNode { get; set; }
        Node3D RefractionNode { get; set; }
        Node3D ReflectionNode { get; set; }
        Node3D PropNode { get; set; }
        
        List<TerrainGeometryContainer> TerrainGeometries = new List<TerrainGeometryContainer>();
        List<TerrainWaterContainer> WaterGeometries = new List<TerrainWaterContainer>();

        List<TerrainDeformableCoverContainer> DeformedTerrainGeometries = new List<TerrainDeformableCoverContainer>();

        SkyBoxModel SkyBox;

        WorldPropStructureContainer PropStructureContainer;

        //Use Global Refraction/Reflection Render Target
        SceneRenderTarget ReflectionRenderTarget;
        SceneRenderTarget RefractionRenderTarget;
        SceneRenderTarget DepthMapRenderTarget;
        SceneRenderTarget FromBelowDepthTarget;
        
        Dictionary<string, int> ChunkLODStates = new Dictionary<string, int>();
        List<Node3D> RenderNodeLayers = new List<Node3D>();
        //Add Structure/Prop Support
        CameraManager Camera { get; set; }
        
        public MasterFileRenderManager(WorldFileManager WorldFileManager, RenderManager RenderManager, CameraManager CameraManager)
        {
            Render = RenderManager;
            FileManager = WorldFileManager;
            RenderNode = Render.RootNode;
            Camera = CameraManager;
            
            ReflectionNode = new Node3D("WorldReflection");
            RefractionNode = new Node3D("WorldRefraction");

            PropNode = new Node3D("PropNode");

            SkyBox = new SkyBoxModel(Render, 1000, "SkyBox");
            RenderNode.Attach(SkyBox.Geom);
            ReflectionNode.Attach(SkyBox.Geom);

        }
        
        public void UpdateRenderTargets()
        {
            List<Geometry3D> ReflectionUpdateList = new List<Geometry3D>();
            List<Geometry3D> RefractionUpdateList = new List<Geometry3D>();
            List<Geometry3D> DepthUpdateList = new List<Geometry3D>();
            
            if (ReflectionRenderTarget != null)
            {
                ReflectionUpdateList.AddRange(ReflectionRenderTarget.GeometryUpdateList);
                Render.DetachRenderTarget(ReflectionRenderTarget);
                //ReflectionRenderTarget.Dispose();
            }
            if (RefractionRenderTarget != null)
            {
                RefractionUpdateList.AddRange(RefractionRenderTarget.GeometryUpdateList);
                Render.DetachRenderTarget(RefractionRenderTarget);
                //RefractionRenderTarget.Dispose();
            }
            if (DepthMapRenderTarget != null)
            {
                DepthUpdateList.AddRange(DepthMapRenderTarget.GeometryUpdateList);
                Render.DetachRenderTarget(DepthMapRenderTarget);
                //DepthMapRenderTarget.Dispose();
            }
            
            
            Settings = FileManager.MasteryFile.Settings;

            PropStructureContainer = new WorldPropStructureContainer(Render);

            List<Geometry3D> PropBatches = PropStructureContainer.GetGeometries(-1);

            RenderNode.AttachRange(PropBatches);
            ReflectionNode.AttachRange(PropBatches);
            RefractionNode.AttachRange(PropBatches);
            PropNode.AttachRange(PropBatches);

            ReflectionRenderTarget = new SceneRenderTarget("ReflectionMap", ReflectionNode, Render.Graphics, Render.Camera.Position, Settings.CameraClosePlane, Settings.CameraFarPlane, Settings.WaterReflectionPlaneDirection, Settings.WaterReflectionResolutionX, Settings.WaterReflectionResolutionY, SceneRenderTarget.RenderType.SingleColor);
            RefractionRenderTarget = new SceneRenderTarget("RefractionMap", RefractionNode, Render.Graphics, Render.Camera.Position, Settings.CameraClosePlane, Settings.CameraFarPlane, Settings.WaterRefractionPlaneDirection, Settings.WaterReflectionResolutionX, Settings.WaterReflectionResolutionY, SceneRenderTarget.RenderType.SingleColor);
            DepthMapRenderTarget = new SceneRenderTarget("DepthMap", RefractionNode, Render.Graphics, Render.Camera.Position, Settings.CameraClosePlane, Settings.CameraFarPlane, Settings.WaterRefractionPlaneDirection, Settings.WaterReflectionResolutionX, Settings.WaterReflectionResolutionY, SceneRenderTarget.RenderType.SingleDepth);


            FromBelowDepthTarget = new SceneRenderTarget("DepthMap", RefractionNode, Render.Graphics, new Vector3(), Settings.FromBelowClosePlane, Settings.FromBelowFarPlane, 0,
                Settings.FromBelowResolutionX, Settings.FromBelowResolutionY, SceneRenderTarget.RenderType.SingleDepth, Settings.ChunkSize, Settings.ChunkSize);


            ReflectionRenderTarget.ClipOffset = Settings.ClipOffset;
            RefractionRenderTarget.ClipOffset = Settings.ClipOffset;
            DepthMapRenderTarget.ClipOffset = Settings.ClipOffset;
            
            FromBelowDepthTarget.DebugTextureName = "SceneColorDebug";
            //DepthMapRenderTarget.DebugTextureName = "SceneDepthDebug";

            Render.AddRenderTarget(ReflectionRenderTarget);
            Render.AddRenderTarget(RefractionRenderTarget);
            Render.AddRenderTarget(DepthMapRenderTarget);
            Render.AddRenderTarget(FromBelowDepthTarget);

            ReflectionRenderTarget.AttachRange(ReflectionUpdateList);
            RefractionRenderTarget.AttachRange(RefractionUpdateList);
            DepthMapRenderTarget.AttachRange(DepthUpdateList);
            
        }
        
        public void UpdatePreRender(GameTime GameTime)
        {
            if (FileManager.MasteryFile == null)
            {
                return;
            }

            float Time = (float)GameTime.TotalGameTime.TotalSeconds; 
            
            for (int i = 0; i < WaterGeometries.Count; i++)
            {
                if (WaterGeometries[i].IsActive && WaterGeometries[i].CurrentLOD == 0)
                {
                    WaterGeometries[i].UpdatePreRender(Camera.Position, Camera.Camera.ViewDirection, Time);
                }
            }
            PropStructureContainer.UpdatePreRender(Settings, Camera.Position, Camera.Camera.ViewDirection, Time);

        }
        
        
        public void Update(GameTime GameTime)
        {
            if (FileManager.MasteryFile == null)
            {
                return;
            }

            Settings = FileManager.MasteryFile.Settings;

            //World File Management
            RemoveInactiveChunks();
            GenerateLODStates();
            CalculateActiveNodes();
            CalculateActiveLODChunks();
            UpdateActiveLodChunks(GameTime);
            //Rendering Management
            GenerateTerrainGeometries();
            
            PropStructureContainer.Update(Render.Graphics, FileManager.MasteryFile.Props, FileManager.MasteryFile.Structures);
        }
        
        private void CalculatePropLOD()
        {
            List<Prop> PropList = FileManager.MasteryFile.Props;
            List<Structure> StructureList = FileManager.MasteryFile.Structures;

            foreach (Prop Prop in PropList)
            { 
                if (Settings.LODMode == 0)
                {
                    return;
                }
                else if (Settings.LODMode == 1)
                {
                    float Distance = Vector3.Distance(Prop.Position, Camera.Position);
                    int[] PropLOD = Settings.PropLOD;
                    for (int j = 0; j < PropLOD.Length; j++)
                    {
                        if (Distance < PropLOD[j])
                        {
                            Prop.LOD = j;
                            break;
                        }
                    }
                }
                else
                {
                    Prop.LOD = Settings.LODMode - 2 <= Settings.PropLOD.Length-1 ? Settings.LODMode - 2 : Settings.PropLOD.Length-1;
                }
            }
        }
        
        private void RemoveInactiveChunks()
        {
            int PX = (int)Math.Floor(Camera.Position.X / (Settings.ChunkSize * Settings.MinWorldScale));
            int PY = (int)Math.Floor(Camera.Position.Y / (Settings.ChunkSize * Settings.MinWorldScale));

            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;

            for (int i = 0; i < ActiveWorldFiles.Count; i++)
            {
                if (Math.Abs(PX - ActiveWorldFiles[i].IDX) > Settings.ChunkDistance || Math.Abs(PY - ActiveWorldFiles[i].IDY) > Settings.ChunkDistance)
                {
                    ActiveWorldFiles.Remove(ActiveWorldFiles[i]);
                }
            }
        }

        //Add loading for unloaded chunks
        public void GenerateLODStates()
        {
            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;
            for (int i = 0; i < ActiveWorldFiles.Count; i++)
            {
                string Key = ActiveWorldFiles[i].IDX + " " + ActiveWorldFiles[i].IDY;
                if (!ChunkLODStates.ContainsKey(Key))
                {
                    ChunkLODStates.Add(Key, ActiveWorldFiles[i].LODID);
                }
            }

            List<string> RemovalStrings = new List<string>();

            //remove Inactive Chunk LOD States
            foreach (KeyValuePair<string, int> entry in ChunkLODStates)
            {
                int IDX = Int32.Parse(entry.Key.Split(' ')[0]);
                int IDY = Int32.Parse(entry.Key.Split(' ')[1]);

                bool HasChunk = false;
                for (int j = 0; j < ActiveWorldFiles.Count; j++)
                {
                    if (ActiveWorldFiles[j].IDX == IDX && ActiveWorldFiles[j].IDY == IDY)
                    {
                        HasChunk = true;
                        break;
                    }

                }
                if (!HasChunk)
                {
                    RemovalStrings.Add(entry.Key);
                }
            }

            for (int i = 0; i < RemovalStrings.Count; i++)
            {
                ChunkLODStates.Remove(RemovalStrings[i]);
            }

            if (Settings.LODMode == 0)
            {
                return;
            }
            else if (Settings.LODMode == 1)
            {
                int[] LOD = Settings.LOD;
                for (int i = 0; i < ActiveWorldFiles.Count; i++)
                {
                    string Key = ActiveWorldFiles[i].IDX + " " + ActiveWorldFiles[i].IDY;
                    float Distance = Vector3.Distance(ActiveWorldFiles[i].GetCenterPoint(), Camera.Position);
                    ChunkLODStates[Key] = LOD.Count();
                    for (int j = 0; j < LOD.Length; j++)
                    {
                        if (Distance < LOD[j])
                        {
                            ChunkLODStates[Key] = j;
                            break;
                        }
                    }
                }
            }
            else
            {
                int LOD = Settings.LODMode - 2;

                for (int i = 0; i < ChunkLODStates.Keys.Count; i++)
                {
                    ChunkLODStates[ChunkLODStates.Keys.ElementAt(i)] = LOD;
                }
            }
        }
        
        /**
         * Adds/Removes Nodes based on LOD Setting
         **/
        private void CalculateActiveNodes()
        {
            if (FileManager.MasteryFile == null)
            {
                //clear all nodes
                RenderNodeLayers.Clear();
                ChunkLODStates.Clear();
            }
            else if(ChunkLODStates.Count > 0)
            {   
                int LayerCount = Settings.LOD.Length;

                if (LayerCount > RenderNodeLayers.Count)
                {
                    //add nodes
                    for (int i = 0; i < LayerCount; i++)
                    {
                        bool HasLayer = false;
                        for (int j = 0; j < RenderNodeLayers.Count; j++)
                        {
                            int LayerLOD = Int32.Parse(RenderNodeLayers[j].Name.Replace("Layer:", ""));
                            if (i == LayerLOD)
                            {
                                HasLayer = true;
                                break;
                            }
                        }
                        if (!HasLayer)
                        {
                            Node3D NewNode = new Node3D("Layer:" + i);
                            RenderNode.Attach(NewNode);
                            RenderNodeLayers.Add(NewNode);
                        }
                    }
                }
                else if(LayerCount < RenderNodeLayers.Count)
                {
                    //remove nodes
                    for (int i = 0; i < RenderNodeLayers.Count; i++)
                    {
                        int LayerLOD = Int32.Parse(RenderNodeLayers[i].Name.Replace("Layer:", ""));
                        if (LayerLOD > LayerCount)
                        {
                            RenderNodeLayers.RemoveAt(i);
                        }
                    }
                }
            }
            
        }
        
        private void CalculateActiveLODChunks()
        {
            for (int i = 0; i < TerrainGeometries.Count; i++)
            {
                string Key = TerrainGeometries[i].WorldFile.IDX + " " + TerrainGeometries[i].WorldFile.IDY;
                if (ChunkLODStates.ContainsKey(Key))
                {
                    TerrainGeometries[i].SetIsActive(ChunkLODStates[Key] == TerrainGeometries[i].WorldFile.LODID);
                    WaterGeometries[i].SetIsActive(ChunkLODStates[Key] == WaterGeometries[i].WorldFile.LODID);
                    //DeformedTerrainGeometries[i].SetIsActive(ChunkLODStates[Key] == DeformedTerrainGeometries[i].WorldFile.LODID);
                }
            }
            
        }
        
        private void UpdateActiveLodChunks(GameTime GameTime)
        {
            for (int i = 0; i < TerrainGeometries.Count; i++)
            {
                if (TerrainGeometries[i].IsActive)
                {
                    TerrainGeometries[i].Update(Render, FileManager.MasteryFile.Settings, Camera.Position);
                }
            }

            for (int i = 0; i < WaterGeometries.Count; i++)
            {
                if (WaterGeometries[i].IsActive)
                {
                    WaterGeometries[i].Update(Render, Camera.Position, Camera.Camera.ViewDirection, GameTime.ElapsedGameTime.Milliseconds);
                }
            }

            for (int i = 0; i < DeformedTerrainGeometries.Count; i++)
            {
                if (DeformedTerrainGeometries[i].IsActive)
                {
                    DeformedTerrainGeometries[i].Update();
                }
            }


            for (int i = 0; i < FileManager.MasteryFile.ActiveWorldFiles.Count; i++)
            {

                FileManager.MasteryFile.ActiveWorldFiles[i].HasTerrainUpdate = false;
            }
        }

        private void GenerateTerrainGeometries()
        {
            SettingsContainer Settings = FileManager.MasteryFile.Settings;
            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;
            
            for (int i = 0; i < ActiveWorldFiles.Count; i++)
            {
                bool HasGeometry = false;
                for (int j = 0; j < TerrainGeometries.Count; j++ )
                {
                    if (ActiveWorldFiles[i] == TerrainGeometries[j].WorldFile)
                    {
                        HasGeometry = true;
                    }
                }

                if (!HasGeometry)
                {
                    Node3D Node = null;
                    for (int j = 0; j < RenderNodeLayers.Count; j++)
                    {
                        if (RenderNodeLayers[j].Name.Replace("Layer:", "") == ActiveWorldFiles[i].LODID + "")
                        {
                            Node = RenderNodeLayers[j];
                        }
                    }

                    if (Node != null)
                    {
                        TerrainGeometryContainer NewContainer = new TerrainGeometryContainer(Render, Settings, ActiveWorldFiles[i]);

                        //TerrainDeformableCoverContainer NewDeformedCover = new TerrainDeformableCoverContainer(Render,ActiveWorldFiles[i],NewContainer, Settings,FromBelowDepthTarget);
                        
                        TerrainWaterContainer WaterContainer = new TerrainWaterContainer(
                            Render, NewContainer, ReflectionNode, RefractionNode, PropNode, ActiveWorldFiles[i], Settings, ActiveWorldFiles[i].GetPosition(), ReflectionRenderTarget, RefractionRenderTarget, DepthMapRenderTarget, FromBelowDepthTarget);
                        
                        DepthMapRenderTarget.DebugTextureName = "SceneWaterDepthDebug";

                        TerrainGeometries.Add(NewContainer);
                        //DeformedTerrainGeometries.Add(NewDeformedCover);

                        WaterGeometries.Add(WaterContainer);

                        ReflectionNode.Attach(NewContainer.TerrainGeometry);
                        RefractionNode.Attach(NewContainer.TerrainGeometry);
                       
                        Node.Attach(WaterContainer.Geom);
                        //Node.Attach(NewDeformedCover.Geom);
                        Node.Attach(NewContainer.TerrainGeometry);
                    }   
                }
            }
        }
        
        public int GetChunkLOD(Vector3 Position)
        {
            if (FileManager.MasteryFile == null)
            {
                return -1;
            }

            int PX = (int)Math.Floor(Position.X / (Settings.ChunkSize * Settings.MinWorldScale));
            int PY = (int)Math.Floor(Position.Y / (Settings.ChunkSize * Settings.MinWorldScale));
            
            return ChunkLODStates.ContainsKey(PX + "-" + PY) ? ChunkLODStates[PX + "-" + PY] : -1;
        }

        public int GetChunkLOD(int PX, int PY)
        {
            return ChunkLODStates.ContainsKey(PX + "-" + PY) ? -1 : ChunkLODStates[PX + "-" + PY];
        }

        public CollisionResults RayCastProps(Vector3 Origin, Vector3 Direction, float Range)
        {
            if (FileManager.MasteryFile == null)
            {
                return new CollisionResults();
            }
            return PropStructureContainer.CollideWith(Origin, Direction, Range);
        }

        public CollisionResults RayCastTerrain(Vector3 Origin, Vector3 Direction, int LOD, float Range)
        {
            return RayCastTerrain(Origin, Direction,LOD, Range, false);
        }

        public CollisionResults RayCastTerrain(Vector3 Origin, Vector3 Direction, float Range)
        {
            return RayCastTerrain(Origin, Direction,-1, Range, false);
        }

        public CollisionResults RayCastTerrain(Vector3 Origin, Vector3 Direction, float Range, TerrainGeometryContainer TerrainGeometry)
        {
            return RayCastTerrain(Origin,Direction,Range, TerrainGeometry.WorldFile);
        }

        public static int[] Indices =
        {
            3,1,0, 0,2,3
        };

        public CollisionResults RayCastTerrain(Vector3 Origin, Vector3 Direction, float Range, WorldFile File)
        {
            if (FileManager.MasteryFile == null)
            {
                return new CollisionResults();
            }

            SettingsContainer Settings = FileManager.MasteryFile.Settings;
            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;

            List<Vector2> QuadCheck = new List<Vector2>();
            CollisionResults Results = new CollisionResults();
            
            float TerrainScale = File.TerrainScale;
            float HeighScale = File.HeightScale;
            int[,] HeightMap = File.HeightMap;

            float Precision = TerrainScale * 0.5f;

            for (float j = 0; j < Range; j += Precision)
            {
                Vector3 CurrentPosition = Origin + (Direction * j);
                Vector3 CurrentVertexPosition = (CurrentPosition - File.GetPosition()) / TerrainScale;

                int VX = (int)Math.Floor(CurrentVertexPosition.X);
                int VY = (int)Math.Floor(CurrentVertexPosition.Y);

                if (VX < 0 || VY < 0 || VX >= HeightMap.GetLength(0) - 1 || VY >= HeightMap.GetLength(1) - 1)
                {
                    continue;
                }
                
                Vector3[] QuadVertices = {
                    new Vector3(0,0, HeightMap[VX,VY]* HeighScale),
                    new Vector3(TerrainScale,0, HeightMap[VX+1,VY]* HeighScale),
                    new Vector3(0,TerrainScale, HeightMap[VX,VY+1]* HeighScale),
                    new Vector3(TerrainScale, TerrainScale, HeightMap[VX+1,VY+1]* HeighScale)
                };

                if (!QuadCheck.Contains(new Vector2(VX, VY)))
                {
                    QuadCheck.Add(new Vector2(VX, VY));

                    Vector3 VertexWorldPosition = File.GetPosition() + new Vector3(VX * TerrainScale, VY * TerrainScale, 0);
                    for (int k = 0; k < Indices.Length; k += 3)
                    {
                        Vector3 V0 = VertexWorldPosition + QuadVertices[Indices[k]];
                        Vector3 V1 = VertexWorldPosition + QuadVertices[Indices[k + 1]];
                        Vector3 V2 = VertexWorldPosition + QuadVertices[Indices[k + 2]];
                            
                        if (CollisionUtil.Intersect(V0, V1, V2, Origin, Direction))
                        {
                            TerrainGeometryContainer TerrainGeom = TerrainGeometries.Where(x=>x.WorldFile.FileName == FileManager.CurrentWorldFile.FileName).First();

                            Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, Origin, Direction);
                            CollisionResult Result = new CollisionResult(
                                V0,
                                V1,
                                V2,
                                Origin,
                                CollisionPoint,
                                TerrainGeom.TerrainGeometry);
                            Results.Add(Result);
                        }
                    }
                }
             }
            return Results;
        }
        
        public CollisionResults RayCastTerrain(Vector3 Origin, Vector3 Direction, int LOD, float Range, bool HasCurrentChunkLock)
        {
            if (FileManager.MasteryFile == null)
            {
                return new CollisionResults();
            }

            SettingsContainer Settings = FileManager.MasteryFile.Settings;
            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;

            List<Vector2> QuadCheck = new List<Vector2>();
            List<Vector3> Vertices = new List<Vector3>();

            CollisionResults Results = new CollisionResults();

            int CurrentWorldFileLOD = FileManager.CurrentWorldFile == null ? 0 : FileManager.CurrentWorldFile.LODID;

            if (HasCurrentChunkLock)
            {
                if (FileManager.CurrentWorldFile != null)
                {
                    float TerrainScale = FileManager.CurrentWorldFile.TerrainScale;
                    int[,] HeightMap = FileManager.CurrentWorldFile.HeightMap;

                    float Precision = TerrainScale * 0.5f;

                    for (float j = 0; j < Range; j += Precision)
                    {
                        Vector3 CurrentPosition = Origin + (Direction * j);
                        Vector3 CurrentVertexPosition = (CurrentPosition - FileManager.CurrentWorldFile.GetPosition()) / TerrainScale;

                        int VX = (int)Math.Floor(CurrentVertexPosition.X);
                        int VY = (int)Math.Floor(CurrentVertexPosition.Y);

                        if (VX < 0 || VY < 0 || VX >= HeightMap.GetLength(0) - 1 || VY >= HeightMap.GetLength(1) - 1)
                        {
                            continue;
                        }
                        
                        Vector3[] QuadVertices = {
                        new Vector3(0,0, HeightMap[VX,VY]),
                        new Vector3(TerrainScale,0, HeightMap[VX+1,VY]),
                        new Vector3(0,TerrainScale, HeightMap[VX,VY+1]),
                        new Vector3(TerrainScale, TerrainScale, HeightMap[VX+1,VY+1])
                    };

                        if (!QuadCheck.Contains(new Vector2(VX, VY)))
                        {
                            QuadCheck.Add(new Vector2(VX, VY));

                            Vector3 VertexWorldPosition = FileManager.CurrentWorldFile.GetPosition() + new Vector3(VX * TerrainScale, VY * TerrainScale, 0);
                            for (int k = 0; k < Indices.Length; k+=3)
                            {
                                Vector3 V0 = VertexWorldPosition + QuadVertices[Indices[k]];
                                Vector3 V1 = VertexWorldPosition + QuadVertices[Indices[k+1]];
                                Vector3 V2 = VertexWorldPosition + QuadVertices[Indices[k+2]];

                                //if (CollisionUtil.Intersect(Vertices[i], Vertices[i + 1], Vertices[i + 2], Origin, Direction))
                                //{
                                if (CollisionUtil.Intersect(V0, V1, V2, Origin, Direction))
                                {
                                    TerrainGeometryContainer TerrainGeom = TerrainGeometries.Where(x => x.WorldFile.FileName == FileManager.CurrentWorldFile.FileName).First();

                                    Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, Origin, Direction);
                                    CollisionResult Result = new CollisionResult(
                                        V0,
                                        V1,
                                        V2,
                                        Origin,
                                        CollisionPoint,
                                        TerrainGeom.TerrainGeometry);
                                    Results.Add(Result);
                                }

                                //Vertices.Add(VertexWorldPosition + QuadVertices[Indices[k]]);
                            }
                        }
                    }
                }
            }
            else
            {
                int PX = (int)Math.Floor(Origin.X / Settings.ChunkSize);
                int PY = (int)Math.Floor(Origin.Y / Settings.ChunkSize);

                for (int i = 0; i < ActiveWorldFiles.Count; i++)
                {
                    int ValidLOD = CurrentWorldFileLOD == -1 ? ActiveWorldFiles[i].LODID : CurrentWorldFileLOD;

                    if (Math.Abs(ActiveWorldFiles[i].IDX - PX) <= Math.Ceiling(Settings.ChunkDistance / Range) && Math.Abs(ActiveWorldFiles[i].IDY - PY) <= Math.Ceiling(Settings.ChunkDistance / Range) && ActiveWorldFiles[i].LODID == ValidLOD)
                    {
                        float TerrainScale = ActiveWorldFiles[i].TerrainScale;
                        float HeightScale = ActiveWorldFiles[i].HeightScale;
                        int[,] HeightMap = ActiveWorldFiles[i].HeightMap;

                        float Precision = TerrainScale * 0.5f;

                        for (float j = 0; j < Range; j += Precision)
                        {
                            Vector3 CurrentPosition = Origin + (Direction * j);
                            Vector3 CurrentVertexPosition = (CurrentPosition - ActiveWorldFiles[i].GetPosition()) / TerrainScale;

                            int VX = (int)Math.Floor(CurrentVertexPosition.X);
                            int VY = (int)Math.Floor(CurrentVertexPosition.Y);

                            if (VX < 0 || VY < 0 || VX >= HeightMap.GetLength(0) - 1 || VY >= HeightMap.GetLength(1) - 1)
                            {
                                continue;
                            }
                            Vector3[] QuadVertices = {
                                new Vector3(0,0, HeightMap[VX,VY] * HeightScale),
                                new Vector3(TerrainScale,0, HeightMap[VX+1,VY]* HeightScale),
                                new Vector3(0,TerrainScale, HeightMap[VX,VY+1]* HeightScale),
                                new Vector3(TerrainScale, TerrainScale, HeightMap[VX+1,VY+1]* HeightScale)
                            };

                            if (!QuadCheck.Contains(new Vector2(VX, VY)))
                            {
                                QuadCheck.Add(new Vector2(VX, VY));

                                Vector3 VertexWorldPosition = ActiveWorldFiles[i].GetPosition() + new Vector3(VX * TerrainScale, VY * TerrainScale, 0);
                                for (int k = 0; k < Indices.Length; k+=3)
                                {
                                    Vector3 V0 = VertexWorldPosition + QuadVertices[Indices[k]];
                                    Vector3 V1 = VertexWorldPosition + QuadVertices[Indices[k + 1]];
                                    Vector3 V2 = VertexWorldPosition + QuadVertices[Indices[k + 2]];
                                    
                                    if (CollisionUtil.Intersect(V0, V1, V2, Origin, Direction))
                                    {

                                        TerrainGeometryContainer TerrainGeom = TerrainGeometries.Where(x => x.WorldFile.FileName == FileManager.CurrentWorldFile.FileName).First();

                                        Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, Origin, Direction);
                                        CollisionResult Result = new CollisionResult(
                                            V0,
                                            V1,
                                            V2,
                                            Origin,
                                            CollisionPoint,
                                            TerrainGeom.TerrainGeometry);
                                        Results.Add(Result);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Results;
        }

        public CollisionResults RayCastWater(Vector3 Origin, Vector3 Direction, float Range, WorldFile File)
        {

            if (FileManager.MasteryFile == null)
            {
                return new CollisionResults();
            }

            SettingsContainer Settings = FileManager.MasteryFile.Settings;
            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;

            List<Vector2> QuadCheck = new List<Vector2>();
            CollisionResults Results = new CollisionResults();

            int LOD = FileManager.CurrentWorldFile.LODID;

            if (FileManager.CurrentWorldFile != null)
            {
                float TerrainScale = File.TerrainScale;
                float HeightScale = File.HeightScale;
                int[,] HeightMap = File.HeightMap;
                int[,] WaterHeightMap = File.WaterHeightMap;
                float Precision = TerrainScale * 0.5f;

                for (float j = 0; j < Range; j += Precision)
                {
                    Vector3 CurrentPosition = Origin + (Direction * j);
                    Vector3 CurrentVertexPosition = (CurrentPosition - File.GetPosition()) / TerrainScale;

                    int VX = (int)Math.Floor(CurrentVertexPosition.X);
                    int VY = (int)Math.Floor(CurrentVertexPosition.Y);

                    if (VX < 0 || VY < 0 || VX >= HeightMap.GetLength(0) - 1 || VY >= HeightMap.GetLength(1) - 1)
                    {
                        continue;
                    }

                    float MidPoint =
                             (HeightMap[VX, VY] + HeightMap[VX + 1, VY] + HeightMap[VX, VY + 1] + HeightMap[VX + 1, VY + 1] +
                             WaterHeightMap[VX, VY] + WaterHeightMap[VX + 1, VY] + WaterHeightMap[VX, VY + 1] + WaterHeightMap[VX + 1, VY + 1]) / 4;

                    Vector3[] QuadVertices = {
                            new Vector3(0,0, (WaterHeightMap[VX,VY] + HeightMap[VX,VY]) * HeightScale),
                            new Vector3(TerrainScale,0,(WaterHeightMap[VX+1,VY]+ HeightMap[VX+1,VY]) * HeightScale),
                            new Vector3(0,TerrainScale, (WaterHeightMap[VX,VY+1] + HeightMap[VX,VY+1]) * HeightScale),
                            new Vector3(TerrainScale, TerrainScale, (WaterHeightMap[VX+1,VY+1]+ HeightMap[VX+1,VY+1]) * HeightScale)
                        };

                    if (!QuadCheck.Contains(new Vector2(VX, VY)))
                    {
                        QuadCheck.Add(new Vector2(VX, VY));

                        Vector3 VertexWorldPosition = File.GetPosition() + new Vector3(VX * TerrainScale, VY * TerrainScale, 0);
                        for (int k = 0; k < Indices.Length; k += 3)
                        {
                            Vector3 V0 = VertexWorldPosition + QuadVertices[Indices[k]];
                            Vector3 V1 = VertexWorldPosition + QuadVertices[Indices[k + 1]];
                            Vector3 V2 = VertexWorldPosition + QuadVertices[Indices[k + 2]];
                            
                            if (CollisionUtil.Intersect(V0, V1, V2, Origin, Direction))
                            {
                                TerrainWaterContainer WaterGeom = WaterGeometries.Where(x => x.WorldFile.FileName == FileManager.CurrentWorldFile.FileName).First();

                                Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, Origin, Direction);
                                CollisionResult Result = new CollisionResult(
                                    V0,
                                    V1,
                                    V2,
                                    Origin,
                                    CollisionPoint,
                                    WaterGeom.Geom);
                                Results.Add(Result);
                            }
                        }
                    }
                }
            }
            return Results;

        }

        public CollisionResults RayCastWater(Vector3 Origin, Vector3 Direction, float Range)
        {
            return RayCastWater(Origin,Direction,Range, false);
        }

        public CollisionResults RayCastWater(Vector3 Origin, Vector3 Direction, float Range, bool HasCurrentChunkLock)
        {
            if (FileManager.MasteryFile == null)
            {
                return new CollisionResults();
            }

            int LOD = FileManager.CurrentWorldFile.LODID;

            SettingsContainer Settings = FileManager.MasteryFile.Settings;
            List<WorldFile> ActiveWorldFiles = FileManager.MasteryFile.ActiveWorldFiles;

            CollisionResults Results = new CollisionResults();

            List<Vector2> QuadCheck = new List<Vector2>();

            if (HasCurrentChunkLock)
            {
                if (FileManager.CurrentWorldFile != null)
                {
                    float TerrainScale = FileManager.CurrentWorldFile.TerrainScale;
                    float HeightScale = FileManager.CurrentWorldFile.HeightScale;
                    int[,] HeightMap = FileManager.CurrentWorldFile.HeightMap;
                    int[,] WaterHeightMap = FileManager.CurrentWorldFile.WaterHeightMap;

                    float Precision = TerrainScale * 0.5f;

                    for (float j = 0; j < Range; j += Precision)
                    {
                        Vector3 CurrentPosition = Origin + (Direction * j);
                        Vector3 CurrentVertexPosition = (CurrentPosition - FileManager.CurrentWorldFile.GetPosition()) / TerrainScale;

                        int VX = (int)Math.Floor(CurrentVertexPosition.X);
                        int VY = (int)Math.Floor(CurrentVertexPosition.Y);

                        if (VX < 0 || VY < 0 || VX >= HeightMap.GetLength(0) - 1 || VY >= HeightMap.GetLength(1) - 1)
                        {
                            continue;
                        }

                        float MidPoint =
                            (HeightMap[VX, VY] + HeightMap[VX + 1, VY] + HeightMap[VX, VY + 1] + HeightMap[VX + 1, VY + 1] +
                            WaterHeightMap[VX, VY] + WaterHeightMap[VX + 1, VY] + WaterHeightMap[VX, VY + 1] + WaterHeightMap[VX + 1, VY + 1]) / 4;

                        Vector3[] QuadVertices = {
                            new Vector3(0,0, (WaterHeightMap[VX,VY] + HeightMap[VX,VY]) * HeightScale),
                            new Vector3(TerrainScale,0,(WaterHeightMap[VX+1,VY]+ HeightMap[VX+1,VY]) * HeightScale),
                            new Vector3(0,TerrainScale, (WaterHeightMap[VX,VY+1] + HeightMap[VX,VY+1]) * HeightScale),
                            new Vector3(TerrainScale, TerrainScale, (WaterHeightMap[VX+1,VY+1]+ HeightMap[VX+1,VY+1]) * HeightScale)
                        };

                        if (!QuadCheck.Contains(new Vector2(VX, VY)))
                        {
                            QuadCheck.Add(new Vector2(VX, VY));

                            Vector3 VertexWorldPosition = FileManager.CurrentWorldFile.GetPosition() + new Vector3(VX * TerrainScale, VY * TerrainScale, 0);
                            
                            for (int k = 0; k < Indices.Length; k += 3)
                            {
                                Vector3 V0 = VertexWorldPosition + QuadVertices[Indices[k]];
                                Vector3 V1 = VertexWorldPosition + QuadVertices[Indices[k + 1]];
                                Vector3 V2 = VertexWorldPosition + QuadVertices[Indices[k + 2]];

                                if (CollisionUtil.Intersect(V0, V1, V2, Origin, Direction))
                                {
                                    TerrainWaterContainer WaterGeom = WaterGeometries.Where(x => x.WorldFile.FileName == FileManager.CurrentWorldFile.FileName).First();

                                    Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, Origin, Direction);
                                    CollisionResult Result = new CollisionResult(
                                        V0,
                                        V1,
                                        V2,
                                        Origin,
                                        CollisionPoint,
                                        WaterGeom.Geom);
                                    Results.Add(Result);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                int PX = (int)Math.Floor(Origin.X / Settings.ChunkSize);
                int PY = (int)Math.Floor(Origin.Y / Settings.ChunkSize);

                for (int i = 0; i < ActiveWorldFiles.Count; i++)
                {
                    int ValidLOD = LOD == -1 ? ActiveWorldFiles[i].LODID : LOD;
                    if (Math.Abs(ActiveWorldFiles[i].IDX - PX) <= Settings.ChunkSelectionRadius && Math.Abs(ActiveWorldFiles[i].IDY - PY) < Settings.ChunkSelectionRadius && ActiveWorldFiles[i].LODID == ValidLOD)
                    {
                        float TerrainScale = ActiveWorldFiles[i].TerrainScale;
                        float HeightScale = ActiveWorldFiles[i].HeightScale;
                        int[,] HeightMap = ActiveWorldFiles[i].HeightMap;
                        int[,] WaterHeightMap = FileManager.CurrentWorldFile.WaterHeightMap;

                        float Precision = TerrainScale * 0.5f;

                        for (float j = 0; j < Range; j += Precision)
                        {
                            Vector3 CurrentPosition = Origin + (Direction * j);
                            Vector3 CurrentVertexPosition = (CurrentPosition - ActiveWorldFiles[i].GetPosition()) / TerrainScale;

                            int VX = (int)Math.Floor(CurrentVertexPosition.X);
                            int VY = (int)Math.Floor(CurrentVertexPosition.Y);

                            if (VX < 0 || VY < 0 || VX >= HeightMap.GetLength(0) - 1 || VY >= HeightMap.GetLength(1) - 1)
                            {
                                continue;
                            }
                            float MidPoint =
                            (HeightMap[VX, VY] + HeightMap[VX + 1, VY] + HeightMap[VX, VY + 1] + HeightMap[VX + 1, VY + 1] +
                            WaterHeightMap[VX, VY] + WaterHeightMap[VX + 1, VY] + WaterHeightMap[VX, VY + 1] + WaterHeightMap[VX + 1, VY + 1]) / 4;

                            Vector3[] QuadVertices = {
                                new Vector3(0,0, (WaterHeightMap[VX,VY] + HeightMap[VX,VY]) * HeightScale),
                                new Vector3(TerrainScale,0,(WaterHeightMap[VX+1,VY]+ HeightMap[VX+1,VY]) * HeightScale),
                                new Vector3(0,TerrainScale, (WaterHeightMap[VX,VY+1] + HeightMap[VX,VY+1]) * HeightScale),
                                new Vector3(TerrainScale, TerrainScale, (WaterHeightMap[VX+1,VY+1]+ HeightMap[VX+1,VY+1]) * HeightScale)
                            };

                            if (!QuadCheck.Contains(new Vector2(VX, VY)))
                            {
                                QuadCheck.Add(new Vector2(VX, VY));

                                Vector3 VertexWorldPosition = ActiveWorldFiles[i].GetPosition() + new Vector3(VX * TerrainScale, VY * TerrainScale, 0);
                               
                                for (int k = 0; k < Indices.Length; k += 3)
                                {
                                    Vector3 V0 = VertexWorldPosition + QuadVertices[Indices[k]];
                                    Vector3 V1 = VertexWorldPosition + QuadVertices[Indices[k + 1]];
                                    Vector3 V2 = VertexWorldPosition + QuadVertices[Indices[k + 2]];

                                    if (CollisionUtil.Intersect(V0, V1, V2, Origin, Direction))
                                    {
                                        Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, Origin, Direction);

                                        TerrainWaterContainer WaterGeom = WaterGeometries.Where(x => x.WorldFile.FileName == FileManager.CurrentWorldFile.FileName).First();
                                        
                                        CollisionResult Result = new CollisionResult(
                                            V0,
                                            V1,
                                            V2,
                                            Origin,
                                            CollisionPoint,
                                            WaterGeom.Geom);
                                        Results.Add(Result);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return Results;
        }
        
        public void ClearBrushHighLight()
        {
            for (int i = 0; i < TerrainGeometries.Count; i++)
            {
                TerrainGeometries[i].SetHighLight(new Vector3(), new Vector4(), 0, 0);
            }
            for (int i = 0; i < WaterGeometries.Count; i++)
            {
                WaterGeometries[i].SetHighLight( new Vector3(), new Vector4(), 0, 0);
            }
        }
        
        public void UpdateBrushHighLight(Vector3 Origin, Vector3 Direction, Vector4 Color, int ToolGroup, int Shape, float BrushRadius, float Radius)
        {
            if (FileManager.CurrentWorldFile == null || FileManager.MasteryFile == null)
            {
                return;
            }
            CollisionResults Results = new CollisionResults();
            switch (ToolGroup)
            {
                case 0:
                    Results.AddRange(RayCastTerrain(Origin, Direction, Radius));
                    break;
                case 1:
                    
                    Results.AddRange( RayCastWater(Origin, Direction, Radius));
                    break;
                case 2:
                    Results.AddRange(RayCastWater(Origin, Direction, Radius));
                    Results.AddRange(RayCastTerrain(Origin, Direction, Radius));
                    break;
            }
            if (Results.Count == 0)
            {
                return;
            }

            CollisionResult Result = Results.GetClosest();
            int SelectedLOD = FileManager.CurrentWorldFile.LODID;
            
            //Editing Bounds
            Vector2 A1 = new Vector2(Result.CollisionPoint.X - Radius, Result.CollisionPoint.Y - BrushRadius);
            Vector2 A2 = new Vector2(Result.CollisionPoint.X + Radius, Result.CollisionPoint.Y + BrushRadius);
            
            for ( int i = 0; i < TerrainGeometries.Count; i++)
            {
                if (TerrainGeometries[i].WorldFile.LODID != SelectedLOD)
                {
                    continue;
                }
                Vector2 B1 = TerrainGeometries[i].WorldFile.GetA();
                Vector2 B2 = TerrainGeometries[i].WorldFile.GetB();

                if (A1.X < B2.X && A2.X > B1.X && A1.Y < B2.Y && A2.Y > B1.Y)
                {
                    if (Result.GeometryName.Contains("Water"))
                    {
                        WaterGeometries[i].SetHighLight(Result.CollisionPoint, Color, Shape, BrushRadius);
                    }
                    else if( Result.GeometryName.Contains("Terrain"))
                    {
                        TerrainGeometries[i].SetHighLight(Result.CollisionPoint, Color, Shape, BrushRadius);
                    }
                }
                else
                {
                    TerrainGeometries[i].SetHighLight(Result.CollisionPoint, new Vector4(), Shape, 0);
                    WaterGeometries[i].SetHighLight(Result.CollisionPoint, new Vector4(), Shape, 0);
                }
            }
            
        }


    }


}
