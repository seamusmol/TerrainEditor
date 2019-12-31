using Main.Geometry;
using Main.Main;
using Main.StructureEditor;
using Main.Structures;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{
    //Manage SceneGraph for Active Files in WorldFiles

    //Manage WorldFile Graphical Tools
    //Grid,Highlighted WordFile
    public class WorldFileManager : AppState
    {
        RenderManager Render;

        public MasteryWorldFile MasteryFile { get; private set; }
        public WorldFile CurrentWorldFile { get; set; }

        public Prop SelectedProp { get; set; }
       
        public int SelectedChunkX { get; private set; } = 0;
        public int SelectedChunkY { get; private set; } = 0;

        public Grid WorldFileGrid { get; private set; }
        
        public CollisionQuad ChunkSelectorQuad { get; private set; }

        public Node3D RootNode { get; set; }
        
        public WorldFileManager(RenderManager RenderManager)
        {
            Render = RenderManager;
            RootNode = Render.RootNode;
            InitGrid();
            ChunkSelectorQuad = new CollisionQuad(new Vector3(), new Vector3(), new Vector3(), new Vector3());
        }

        public void InitGrid()
        {
            WorldFileGrid = new Grid(0, 0, 0, 0, 0.05f);
            WorldFileGrid.GenerateGeometry(Render.Graphics);
            WorldFileGrid.GenerateGrid(Render.Graphics);
            RootNode.Attach(WorldFileGrid.Geom);
        }

        public override void Update(GameTime GameTime)
        {

        }
        
        //Updates data for 
        public void MoveToChunk(int PX, int PY)
        {
            //if contains Worldfile
            
            SelectedChunkX = PX >= 0 ? PX : 0;
            SelectedChunkY = PY >= 0 ? PY : 0;

            float ChunkSize = MasteryFile.Settings.ChunkSize;
            Vector3 ChunkPosition = new Vector3(SelectedChunkX * ChunkSize, SelectedChunkY * ChunkSize, 0);
            
            WorldFileGrid.SetPosition(Render.Graphics, ChunkPosition);
            ChunkSelectorQuad.SetPosition(ChunkPosition);

            //change CurrentWordFile
            CurrentWorldFile = null;
            if (MasteryFile.HasWorldFile(SelectedChunkX, SelectedChunkY))
            {
                CurrentWorldFile = MasteryFile.GetWorldFile(SelectedChunkX, SelectedChunkY, 0);
            }
        }
        
        public void SwitchActiveWorldFile(WorldFile NewActiveWorldFile)
        {
            if (!MasteryFile.ActiveWorldFiles.Contains(NewActiveWorldFile))
            {
                return;
            }
            CurrentWorldFile = NewActiveWorldFile;
            //CurrentWorldFile.UpdateTerrainSize(NSX, NSY, CurrentWorldFile.TerrainScale);

            //TODO 
            //Change to WorldFile Scale
            WorldFileGrid.ChangeGridSize(CurrentWorldFile.SX, CurrentWorldFile.SY, 0.5f, Render.Graphics);
        }
        
        public void SwitchActiveWorldFileLOD(int LOD)
        {
            if (CurrentWorldFile != null)
            {
                if (CurrentWorldFile.LODID != LOD)
                {
                    WorldFile NewFile = MasteryFile.GetWorldFile(CurrentWorldFile.IDX, CurrentWorldFile.IDY, LOD);
                    if (NewFile != null)
                    {
                        CurrentWorldFile = NewFile;
                    }
                }
            }
        }

        //For Chunk Selector
        public void SetSelectedChunk(Vector3 Position, Vector3 RayDirection)
        {
            CollisionResults Collision = ChunkSelectorQuad.CollideWith(Position, RayDirection);
            if (Collision == null)
            {
                return;
            }

            if (MasteryFile != null && Collision.GetClosest() != null)
            {
                float ChunkSize = MasteryFile.Settings.ChunkSize;
                SelectedChunkX = (int)Math.Floor(Collision.GetClosest().CollisionPoint.X / ChunkSize);
                SelectedChunkY = (int)Math.Floor(Collision.GetClosest().CollisionPoint.Y / ChunkSize);

                SelectedChunkX = SelectedChunkX >= 0 ? SelectedChunkX : 0;
                SelectedChunkY = SelectedChunkY >= 0 ? SelectedChunkY : 0;

                Vector3 ChunkPosition = new Vector3(SelectedChunkX * ChunkSize, SelectedChunkY * ChunkSize, 0 );
                WorldFileGrid.SetPosition(Render.Graphics, ChunkPosition);
                ChunkSelectorQuad.SetPosition(ChunkPosition);

                //change CurrentWordFile
                CurrentWorldFile = null;
                if (MasteryFile.HasWorldFile(SelectedChunkX, SelectedChunkY))
                {
                    CurrentWorldFile = MasteryFile.GetWorldFile(SelectedChunkX, SelectedChunkY, 0);
                }
                
            }
        }
        
        public void ToggleGridVisibility(bool Val)
        {
            WorldFileGrid.ToggleVisibility(Val);
        }
        
        public float GetDataValue(Vector2 WorldPosition, Vector2 CenterPosition, string MapName)
        {
            int IDX = (int)Math.Floor(WorldPosition.X / (MasteryFile.Settings.ChunkSize));
            int IDY = (int)Math.Floor(WorldPosition.Y / (MasteryFile.Settings.ChunkSize));

            List<WorldFile> ActiveWorldFiles = MasteryFile.ActiveWorldFiles;

            for (int i = 0; i < ActiveWorldFiles.Count; i++)
            {
                if (ActiveWorldFiles[i].IDX == IDX && ActiveWorldFiles[i].IDY == IDY)
                {
                    int ArraySize = ActiveWorldFiles[i].GetMapSize(MapName);
                    float ChunkSize = MasteryFile.Settings.ChunkSize;
                    float Scale = (float)(ChunkSize / ArraySize);

                    Vector2 A = ActiveWorldFiles[i].GetA();
                    int LX = (int)Math.Round((WorldPosition.X - A.X) / Scale);
                    int LY = (int)Math.Round((WorldPosition.Y - A.Y) / Scale);
                    
                    float Value = ActiveWorldFiles[i].GetMapValue(MapName, LX, LY);

                    return Value;
                }
            }
            //Find closest chunk to CenterPosition
            
            int CX = (int)Math.Round(CenterPosition.X / (MasteryFile.Settings.ChunkSize));
            int CY = (int)Math.Round(CenterPosition.Y / (MasteryFile.Settings.ChunkSize));
            
            //Replace above system
            WorldFile ClosestFile = ActiveWorldFiles.Where(x => Math.Abs(CX - x.IDX) < 2 && Math.Abs(CY - x.IDY) < 2).OrderBy( y => Vector2.Distance(new Vector2(IDX, IDY), new Vector2(y.IDX, y.IDY))).Reverse().ToList().First();
            if (ClosestFile != null)
            {
                Vector2 A = ClosestFile.GetA();
               
                int ArraySize = ClosestFile.GetMapSize(MapName);
                float ChunkSize = MasteryFile.Settings.ChunkSize;
                float Scale = (float)(ChunkSize / ArraySize);

                int LX = (int)Math.Round((WorldPosition.X - A.X) / Scale) >= 0 ? (int)Math.Round((WorldPosition.X - A.X) / Scale) : 0;
                int LY = (int)Math.Round((WorldPosition.Y - A.Y) / Scale) >= 0 ? (int)Math.Round((WorldPosition.Y - A.Y) / Scale) : 0;

                LX = LX < ArraySize ? LX : ArraySize - 1;
                LY = LY < ArraySize ? LY : ArraySize - 1;
                
                return ClosestFile.GetMapValue(MapName,LX,LY);
            }
            
            return 0;
        }
        
        /**
         * V3----V4
         * |     |
         * |     |
         * V1----V2
        **/
        
        public void ModifyMultipleChunks(Vector3 CollisionPosition, string MapName, int Shape, int Tool, bool IsPrimary, float Radius, float Flow, float Value, int EdgeSmoothMode)
        {
            ModifyMultipleChunks( CollisionPosition, MapName, "", Shape, Tool, IsPrimary, Radius, Flow, Value, EdgeSmoothMode, false);
        }
            //EdheSmoothMode: 0-None,1-flatten,2-Average
        public void ModifyMultipleChunks(Vector3 CollisionPosition, string MapName, string AdjustMapName, int Shape, int Tool, bool IsPrimary, float Radius, float Flow, float Value, int EdgeSmoothMode, bool AdjustWaterHeight)
        {
            if (MapName == "")
            {
                return;
            }
            
            int SelectedLOD = CurrentWorldFile.LODID;

            //Editing Bounds
            Vector2 A1 = new Vector2(CollisionPosition.X - Radius, CollisionPosition.Y - Radius);
            Vector2 A2 = new Vector2(CollisionPosition.X + Radius, CollisionPosition.Y + Radius);
            float V1 = GetDataValue(new Vector2(A1.X, A1.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), MapName);
            float V2 = GetDataValue(new Vector2(A2.X, A1.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), MapName);
            float V3 = GetDataValue(new Vector2(A1.X, A2.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), MapName);
            float V4 = GetDataValue(new Vector2(A2.X, A2.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), MapName);
            List<WorldFile> ActiveWorldFiles = MasteryFile.ActiveWorldFiles;

            List<object[]> MergeFiles = new List<object[]>();

            //check to see if inside bounding area
            for (int i = 0; i < ActiveWorldFiles.Count; i++)
            {
                if (ActiveWorldFiles[i].LODID != SelectedLOD)
                {
                    continue;
                }
                
                Vector2 B1 = ActiveWorldFiles[i].GetA();
                Vector2 B2 = ActiveWorldFiles[i].GetB();

                int ArraySize = ActiveWorldFiles[i].GetMapSize(MapName);
                float ChunkSize = MasteryFile.Settings.ChunkSize;
                float Scale = (float)(ChunkSize / ArraySize);
                
                
                if (ArraySize == 0)
                {
                    continue;
                }

                if (A1.X < B2.X + Scale && A2.X > B1.X - Scale && A1.Y < B2.Y + Scale && A2.Y > B1.Y - Scale)
                {   
                    int MinX = (int)Math.Floor((A1.X - B1.X) / Scale) >= 0 ? (int)Math.Floor((A1.X - B1.X) / Scale) : 0;
                    int MinY = (int)Math.Floor((A1.Y - B1.Y) / Scale) >= 0 ? (int)Math.Floor((A1.Y - B1.Y) / Scale) : 0;

                    int MaxX = (int)Math.Floor((A2.X - B1.X) / Scale) < ArraySize ? (int)Math.Floor((A2.X - B1.X) / Scale) : ArraySize-1;
                    int MaxY = (int)Math.Floor((A2.Y - B1.Y) / Scale) < ArraySize ? (int)Math.Floor((A2.Y - B1.Y) / Scale) : ArraySize-1;
                    MinX = MinX <= MaxX ? MinX : MaxX;
                    MinY = MinY <= MaxY ? MinY : MaxY;

                    float[,] Q = null;
                    if (Tool == 0 || Tool == 1)
                    {
                        float Val = GetDataValue(new Vector2(CollisionPosition.X, CollisionPosition.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), MapName);
                        /*
                        if (Val < 0)
                        {
                            continue;
                        }
                        */
                        Q = new float[1, 1] { { Val } };
                    }
                    else if (Tool == 2)
                    {
                        float X1 = MinX * Scale + B1.X < A1.X ? A1.X : MinX * Scale + B1.X;
                        float X2 = MaxX * Scale + B1.X > A2.X ? A2.X : MaxX * Scale + B1.X;
                        float Y1 = MinY * Scale + B1.Y < A1.Y ? A1.Y : MinY * Scale + B1.Y;
                        float Y2 = MaxY * Scale + B1.X > A2.Y ? A1.Y : MaxY * Scale + B1.Y;
                        
                        float Q11 = MathUtil.BiLerp(X1, Y1, A1.X, A2.X, A1.Y, A2.Y, V1, V2, V3, V4);
                        float Q21 = MathUtil.BiLerp(X2, Y1, A1.X, A2.X, A1.Y, A2.Y, V1, V2, V3, V4);
                        float Q12 = MathUtil.BiLerp(X1, Y2, A1.X, A2.X, A1.Y, A2.Y, V1, V2, V3, V4);
                        float Q22 = MathUtil.BiLerp(X2, Y2, A1.X, A2.X, A1.Y, A2.Y, V1, V2, V3, V4);
                        
                        /*
                        if (Q11 < 0.0f || Q21 < 0.0f || Q12 < 0.0f || Q22 < 0.0f)
                        {
                            continue;
                        }
                        */
                        Q = new float[2, 2] { { Q11, Q21 }, { Q12, Q22 } };
                    }
                    else if (Tool == 3)
                    {
                        /*
                        if (Value < 0)
                        {
                            continue;
                        }
                        */
                        Q = new float[1, 1] { { Value } };
                    }
                    else if (Tool == 4)
                    {
                        //TODO
                        //Add Pattern import system
                    }
                    else if (Tool == 5)
                    {
                        float Val1 = GetDataValue(new Vector2(CollisionPosition.X, CollisionPosition.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), MapName);
                        float Val2 = GetDataValue(new Vector2(CollisionPosition.X, CollisionPosition.Y), new Vector2(CollisionPosition.X, CollisionPosition.Y), AdjustMapName);

                        /*
                        if (Val1 + Val2 < 0)
                        {
                            continue;
                        }
                        */
                        Q = new float[1, 1] { { Val1 + Val2 } };
                    }
                    
                    if (AdjustMapName != "")
                    {
                        ActiveWorldFiles[i].ModifyMap(MinX, MaxX, MinY, MaxY, Q, Shape, Tool, IsPrimary, (int)Math.Round(Radius / Scale), Flow, MapName, AdjustMapName, AdjustWaterHeight);
                    }
                    else
                    {
                        ActiveWorldFiles[i].ModifyMap(MinX, MaxX, MinY, MaxY, Q, Shape, Tool, IsPrimary, (int)Math.Round(Radius / Scale), Flow, MapName);
                    }

                    if (EdgeSmoothMode != 0)
                    {
                        if (MinX == 0)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX - 1, ActiveWorldFiles[i].IDY, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                ActiveWorldFiles[i].MergeEdges(OtherFile, true, MapName, EdgeSmoothMode);
                            }
                        }
                        if (MinY == 0)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX, ActiveWorldFiles[i].IDY - 1, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                OtherFile.MergeEdges(ActiveWorldFiles[i], false, MapName, EdgeSmoothMode);
                            }
                        }
                        if (MaxX == ArraySize)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX + 1, ActiveWorldFiles[i].IDY, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                ActiveWorldFiles[i].MergeEdges(OtherFile, true, MapName, EdgeSmoothMode);
                            }
                        }
                        if (MaxY == ArraySize)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX, ActiveWorldFiles[i].IDY + 1, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                OtherFile.MergeEdges(ActiveWorldFiles[i], false, MapName, EdgeSmoothMode);
                            }
                        }
                    }

                    if (EdgeSmoothMode != 0)
                    {
                        
                        if (MinX == 0)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX - 1, ActiveWorldFiles[i].IDY, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                MergeFiles.Add( new object[] { ActiveWorldFiles[i], OtherFile, true});
                            }
                        }
                        if (MinY == 0)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX, ActiveWorldFiles[i].IDY - 1, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                MergeFiles.Add(new object[] { OtherFile, ActiveWorldFiles[i], false });
                            }
                        }
                        if (MaxX == ArraySize-1)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX+1, ActiveWorldFiles[i].IDY, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                MergeFiles.Add(new object[] { ActiveWorldFiles[i], OtherFile, true });
                            }
                        }
                        if (MaxY == ArraySize-1)
                        {
                            WorldFile OtherFile = MasteryFile.GetWorldFile(ActiveWorldFiles[i].IDX, ActiveWorldFiles[i].IDY + 1, ActiveWorldFiles[i].LODID);
                            if (OtherFile != null)
                            {
                                MergeFiles.Add(new object[] { OtherFile, ActiveWorldFiles[i],false });
                            }
                        }
                    }

                    
                }
            }
            for (int i = 0; i < MergeFiles.Count; i++)
            {
                WorldFile FileA = (WorldFile)MergeFiles[i][0];
                WorldFile FileB = (WorldFile)MergeFiles[i][1];
                bool Isvertical = (bool)MergeFiles[i][2];

                FileA.MergeEdges(FileB, Isvertical, MapName, EdgeSmoothMode);
            }
        }
        
        public void CreateMasteryFile()
        {
            /*
            if (MasteryFile != null)
            {
            //MasteryFile.Close();

            }
            */

            if (MasteryFile != null)
            {
                return;
            }
            MasteryFile = new MasteryWorldFile();
            float ChunkSize = MasteryFile.Settings.ChunkSize;
            float MinDistance = -ChunkSize * MasteryFile.Settings.ChunkSelectionRadius;
            float MaxDistance = ChunkSize * (MasteryFile.Settings.ChunkSelectionRadius + 1);
            ChunkSelectorQuad.SetVertices(new Vector3(MinDistance, MinDistance, 0), new Vector3(MaxDistance, MinDistance, 0), new Vector3(MinDistance, MaxDistance, 0), new Vector3(MaxDistance, MaxDistance, 0));
        }

        public void LoadMasteryFile(string FileName)
        {
            if (MasteryFile != null)
            {
                if (MasteryFile.WorldName != FileName)
                {
                    MasteryFile.Close();
                    MasteryFile = new MasteryWorldFile(FileName);
                    
                }
            }
            else
            {
                MasteryFile = new MasteryWorldFile(FileName);
            }
        }

        public void SaveMasteryFile(string Name)
        {
            if (MasteryFile != null)
            {
                if (Name != "")
                {
                    MasteryFile.WorldName = Name;
                    //export MasteryFile

                }
                else if (MasteryFile.WorldName != "")
                {
                    //export MasteryFile

                }
            }
        }
        
        public void CreateProp(Vector3 ClickPosition, string PropName, bool SetToActive, float SX,float SY, float SZ, float RX, float RY, float RZ)
        {
            if (MasteryFile != null)
            {
                //create new Prop

                //load Obj file

                if (SelectedProp != null)
                {
                   
                }
                else
                {
                    if (Render.Models.ContainsKey(PropName))
                    {
                        Prop Prop = new Prop(PropName);
                        
                        Prop.Position = ClickPosition;
                        Prop.Scale = new Vector3(SX, SY, SZ);
                        
                        Prop.SetRotation(RX,RY,RZ);

                        Prop.Position = new Vector3(ClickPosition.X, ClickPosition.Y, ClickPosition.Z);

                        MasteryFile.AddProp(Prop);
                        
                        if (SetToActive)
                        {
                            SelectedProp = Prop;
                            SelectedProp.HighLight = 1;
                        }
                    }
                }
            }
        }

        public void CreateStructure(string StructureName)
        {
            if (MasteryFile != null)
            {
                


            }
        }

        public void CreateWorldFile()
        {
            if (MasteryFile != null)
            {
                if (MasteryFile.HasWorldFile(SelectedChunkX, SelectedChunkY))
                {
                    return;
                }
                
                //Create 5 LODs
                WorldFile NewWorldFile0 = new WorldFile(0, SelectedChunkX, SelectedChunkY, MasteryFile.Settings.ChunkSize, MasteryFile.Settings.ChunkSize, 0.5f);
                WorldFile NewWorldFile1 = new WorldFile(1, SelectedChunkX, SelectedChunkY, MasteryFile.Settings.ChunkSize, MasteryFile.Settings.ChunkSize, 1.0f);
                WorldFile NewWorldFile2 = new WorldFile(2, SelectedChunkX, SelectedChunkY, MasteryFile.Settings.ChunkSize, MasteryFile.Settings.ChunkSize, 4.0f);
                WorldFile NewWorldFile3 = new WorldFile(3, SelectedChunkX, SelectedChunkY, MasteryFile.Settings.ChunkSize, MasteryFile.Settings.ChunkSize, 8.0f);
                WorldFile NewWorldFile4 = new WorldFile(4, SelectedChunkX, SelectedChunkY, MasteryFile.Settings.ChunkSize, MasteryFile.Settings.ChunkSize, 16.0f);
                
                //check for worldfile occupying area

                MasteryFile.AddWorldFile(NewWorldFile0);
                MasteryFile.AddWorldFile(NewWorldFile1);
                MasteryFile.AddWorldFile(NewWorldFile2);
                MasteryFile.AddWorldFile(NewWorldFile3);
                MasteryFile.AddWorldFile(NewWorldFile4);

                WorldFileGrid.ChangeGridSize(MasteryFile.Settings.ChunkSize, MasteryFile.Settings.ChunkSize, 0.5f, Render.Graphics);

                CurrentWorldFile = NewWorldFile0;
            }
        }
        
        public string GetMasteryFileName()
        {
            return MasteryFile != null ? MasteryFile.WorldName : "No File";
        }
        
    }
}
