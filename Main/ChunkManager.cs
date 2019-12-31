using Main.GamePlay;
using Main.Geometry;
using Main.Main;
using Main.Structures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Main.WorldManager
{
    public class ChunkManager
    {
        public WorldMap Map { get; set; }

        List<TerrainChunk> ChunkList = new List<TerrainChunk>();
        List<Structure> StructureList = new List<Structure>();
        
        int ChunkDistance = 10;
        Node3D TerrainChunkNode = new Node3D("TerrainChunkNode");

        GeometryBatch TerrainBatch { get; set; }
        
        Node3D RootNode;
        Node3D CollisionNode;
        Node3D TransCollisionNode;

        StructureModelManager ModelManager;
        RenderManager Render;

        public ChunkManager(RenderManager RenderManager)
        {
            Map = new WorldMap("Test", false);
            Render = RenderManager;
            RootNode = RenderManager.RootNode;
            CollisionNode = new Node3D("CollisionNode");
            TransCollisionNode = new Node3D("TransCollisionNode");

            TerrainBatch = new GeometryBatch("TerrainBatch", Render.Graphics, "Test");
            RootNode.Attach(TerrainBatch.Geom);

            ModelManager = new StructureModelManager();
        }
        
        public void Update(Player Player, GameTime GameTime)
        {
            for (int i = 0; i < Player.Actions.Count; i++)
            {
                switch (Player.Actions[i])
                {
                    case "Remove":
                        ModifyWall( Player, 0);
                        break;
                    case "Fill":
                        ModifyWall(Player, Player.PlaceMaterial);
                        break;
                }
            }
            
            int IDX = (int)Player.Position.X / Map.StructureSize;
            int IDY = (int)Player.Position.Y / Map.StructureSize;
            int WorldChunkSize = (Map.WorldSize / Map.StructureSize);
            
            bool HasUpdate = false;
            for (int i = -ChunkDistance+1; i < ChunkDistance; i++)
            {
                for (int j = -ChunkDistance+1; j < ChunkDistance; j++)
                {
                    if (i + IDX < 0 || i + IDX >= WorldChunkSize || i + IDY < 0 || i + IDY >= WorldChunkSize)
                    {
                        continue;
                    }

                    bool HasChunk = false;
                    for (int k = 0; k < ChunkList.Count; k++)
                    {
                        if (ChunkList[k].IDX == IDX + i && ChunkList[k].IDY == IDY + j)
                        {
                            HasChunk = true;
                            break;
                        }
                    }
                    
                    if (!HasChunk)
                    {
                        TerrainChunk NewChunk = TerrainChunkGenerator.GenerateChunk(Map, IDX + i, IDY + j, Map.StructureSize);
                        NewChunk.GenerateGeometry(Render.Graphics);
                        ChunkList.Add(NewChunk);
                        TerrainBatch.Add(NewChunk.Geom);
                        
                        int StructureID = Map.StructureMap[IDX + i, IDY + j];
                        if (StructureID != 0)
                        {
                            if (ModelManager.HasStructure("Tile-" + StructureID))
                            {
                                Structure NewStructure = (Structure)ModelManager.GetStructure("Tile-" + StructureID).Clone();

                                NewStructure.Name = (IDX + i) + "-" + (IDY + j);
                                NewStructure.InitBatches(Render);
                                NewStructure.UpdatePosition( new Vector3((IDX + i) * Map.StructureSize, (IDY + j) * Map.StructureSize, 9));
                                
                                StructureList.Add(NewStructure);

                                RootNode.Attach(NewStructure.GeomBatch.Geom);
                                RootNode.Attach(NewStructure.TransGeomBatch.Geom);
                                RootNode.Attach(NewStructure.SolidPropGeomBatch.Geom);
                                RootNode.Attach(NewStructure.TransPropGeomBatch.Geom);

                                NewStructure.AttachToCollisionNode(CollisionNode, TransCollisionNode);
                            }
                        }
                        HasUpdate = true;
                    }
                }
            }
            UpdateStructures();

            if (HasUpdate)
            {
                TerrainBatch.GenerateVertexBuffer(Render.Graphics);
            }
            RemoveInactiveStructures(IDX,IDY);
            RemoveInactiveChunks(IDX, IDY);
        }

        public void UpdateStructures()
        {
            for (int i = 0; i < StructureList.Count; i++)
            {
                StructureList[i].Update(Render);
            }

        }

        public void RemoveInactiveStructures(int IDX, int IDY)
        {
            for (int i = 0; i < StructureList.Count; i++)
            {
                int SX = (int)StructureList[i].Position.X / Map.StructureSize;
                int SY = (int)StructureList[i].Position.Y / Map.StructureSize;
                
                int DX = Math.Abs(SX - IDX);
                int DY = Math.Abs(SY - IDY);

                if (DX >= ChunkDistance || DY >= ChunkDistance)
                {
                    RootNode.DetachGeometry(StructureList[i].GeomBatch.Geom);
                    RootNode.DetachGeometry(StructureList[i].TransGeomBatch.Geom);
                    RootNode.DetachGeometry(StructureList[i].SolidPropGeomBatch.Geom);
                    RootNode.DetachGeometry(StructureList[i].TransPropGeomBatch.Geom);

                    StructureList[i].DetachFromCollisionNode(CollisionNode, TransCollisionNode);

                    StructureList.Remove(StructureList[i]);
                }
            }
        }
        
        public void RemoveInactiveChunks(int IDX, int IDY)
        {
            for (int i = 0; i < ChunkList.Count; i++)
            {
                int DX = Math.Abs(ChunkList[i].IDX - IDX);
                int DY = Math.Abs(ChunkList[i].IDY - IDY);

                if (DX >= ChunkDistance || DY >= ChunkDistance)
                {
                    TerrainBatch.Remove(ChunkList[i].IDX + "-" + ChunkList[i].IDY);
                    ChunkList.RemoveAt(i);
                    
                }
            }
        }

        public StructureComponent GetStructureComponentByGeometry(Geometry3D Geom)
        {
            for (int i = 0 ; i < StructureList.Count; i++)
            {
                if (StructureList[i].GetComponentByGeometry(Geom) != null)
                {
                    return StructureList[i].GetComponentByGeometry(Geom);
                }
            }
            return null;
        }
        
        public void ModifyWall(Player Player, int Material)
        {
            CollisionResults Results = CollisionNode.CollideWith(Player.Camera.Position, Player.Camera.ViewDirection);
            Results.AddRange( TransCollisionNode.CollideWith(Player.Camera.Position, Player.Camera.ViewDirection));
            
            if (Results.Count > 0)
            {
                CollisionResult Closest = Results.GetClosest();
                StructureComponent Component = GetStructureComponentByGeometry(Closest.Geometry);
                
                if (Component != null)
                {
                    if ( Component.GetType() == typeof(SingleWall))
                    {
                        SingleWall SingleWall = (SingleWall)Component;
                        SingleWall.Modify( Closest.CollisionPoint, Material);

                        return;
                    }

                }
            }
        }
        


    }
}
