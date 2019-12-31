using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Main.Structures.Wall;

namespace Main.Structures
{
    //TODO
    //Use GetClosestMaterialVertex for Material lookup
    //Adjust vertices to better match closest material

    public struct WallVertex
    {
        public Vector3 Position { get; set; }
        public Vector2 TexCoord { get; set; }

        public WallVertex(Vector3 Position, Vector2 TexCoord)
        {
            this.Position = Position;
            this.TexCoord = TexCoord;
        }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );
    }

    public static class WallGenerator
    {
        /*         v10-------v15-----v11
                   /|               /|
                  / |              / |
                 /  v13           /  v14
               v2-------v7-------v3  |
                |   |            |   |
                |  v8------v12---|---v9
             v5 |  /             |v6/
                | /              | /
                |/               |/
                v0------v4-------v1
            
            V2------v3
            |\       |
            |  \     |
            |    \   |     
            |      \ |
            V0------v1
         */

        public static int[][] MarchingSquaresIndices = new int[16][]{
            new int[]{ },
            new int[]{ 5,0,4, 12,8,13, 5,4,12, 12,13,5},
            new int[]{ 4,1,6, 14,9,12, 14,12,4, 4,6,14},
            new int[]{ 5,0,1, 1,6,5, 14,9,8, 8,13,14, 13,5,6, 6,14,13},
            new int[]{ 7,5,13 ,13,15,7, 2,5,7, 13,10,15},
            new int[]{ 7,4,12, 12,15,7, 2,0,4, 4,7,2, 15,12,8, 8,10,15},
            new int[]{ 14,12,4,4,6,14,4,1,6,14,9,12,7,5,13,13,15,7,2,5,7,13,10,15},
            new int[]{ 7,6,14,14,15,7,7,2,0,0,1,6,7,0,6,8,10,15,14,9,8,15,14,8},
            new int[]{ 15,14,6,6,7,15,6,3,7,11,14,15},
            new int[]{ 5,0,4,5,4,12,12,13,5,12,8,13,15,14,6,6,7,15,6,3,7,11,14,15},
            new int[]{ 15,12,4,4,7,15,7,4,1,1,3,7,11,9,12,12,15,11},
            new int[]{ 15,13,5,5,7,15,5,0,1,1,3,7,5,1,7,9,8,13,11,9,15,15,9,13},
            new int[]{ 5,13,14,14,6,5,2,5,6,6,3,2,11,14,13,13,10,11},
            new int[]{ 6,4,12,12,14,6,2,0,4,6,3,2,2,4,6,10,11,14,12,8,10,14,12,10},
            new int[]{ 13,12,4,4,5,13,2,5,3,4,1,3,5,4,3,11,9,12,11,12,13,13,10,11},
            new int[]{ 2,0,1,1,3,2,11,9,8,8,10,11}
            };

        
        public static void GenerateWall(GraphicsDevice Graphics, Wall Wall)
        {
            if (Wall.GetType() == typeof(SingleWall))
            {
                SingleWall SingleWall = (SingleWall)Wall;
                List<int[,]> VoxelList = VoxelUtil.SplitVoxels(SingleWall.Voxels, 239);
                
                List<WallVertex> WallVertices = GenerateSingleWall( new Vector3(), SingleWall.Scale, SingleWall.Rotation, VoxelList.ElementAt(0));
                List<WallVertex> TransWallVertices = GenerateSingleWall(new Vector3(), SingleWall.Scale, SingleWall.Rotation, VoxelList.ElementAt(1));

                if (WallVertices.Count != 0)
                {
                    VertexBuffer NewBuffer = new VertexBuffer(Graphics, WallVertex.VertexDeclaration, WallVertices.Count, BufferUsage.WriteOnly);
                    NewBuffer.SetData(WallVertices.ToArray());
                    SingleWall.Geom.SetVertexBuffer(NewBuffer);

                    Vector3[] CollisionBuffer = new Vector3[WallVertices.Count];
                    for (int i = 0; i < WallVertices.Count; i++)
                    {
                        CollisionBuffer[i] = WallVertices[i].Position;
                    }
                    SingleWall.TransGeom.SetCollisionBuffer(CollisionBuffer);
                }
                else
                {
                    SingleWall.Geom.SetVertexBuffer(null);
                }

                if (TransWallVertices.Count != 0)
                {
                    VertexBuffer NewTransBuffer = new VertexBuffer(Graphics, WallVertex.VertexDeclaration, TransWallVertices.Count, BufferUsage.WriteOnly);
                    NewTransBuffer.SetData(TransWallVertices.ToArray());
                    SingleWall.TransGeom.SetVertexBuffer(NewTransBuffer);
                    Vector3[] CollisionBuffer = new Vector3[TransWallVertices.Count];
                    for (int i = 0; i < TransWallVertices.Count; i++)
                    {
                        CollisionBuffer[i] = TransWallVertices[i].Position;
                    }
                    SingleWall.TransGeom.SetCollisionBuffer(CollisionBuffer);
                }
                else
                {
                    SingleWall.TransGeom.SetVertexBuffer(null);
                }
            }
            else if (Wall.GetType() == typeof(DoubleWall))
            {
                DoubleWall DoubleWall = (DoubleWall)Wall;
                List<int[,]> VoxelList = VoxelUtil.SplitVoxels(DoubleWall.Voxels, 239);
                List<int[,]> VoxelList2 = VoxelUtil.SplitVoxels(DoubleWall.Voxels2, 239);
                
                Vector3 Scale1 = new Vector3(DoubleWall.Scale.X, DoubleWall.Scale.Y * DoubleWall.WallWidth, DoubleWall.Scale.Z);
                Vector3 GapScale = new Vector3(DoubleWall.Scale.X, DoubleWall.Scale.Y * DoubleWall.WallGapWidth, DoubleWall.Scale.Z);
                Vector3 Scale2 = new Vector3(DoubleWall.Scale.X, DoubleWall.Scale.Y * DoubleWall.WallWidth2, DoubleWall.Scale.Z);

                Vector3 WallPos2 = new Vector3(0, DoubleWall.Scale.Y * (Scale1.Y + GapScale.Y), 0);

                List<WallVertex> WallVertices = new List<WallVertex>();
                List<WallVertex> TransWallVertices = new List<WallVertex>();
                
                WallVertices.AddRange( GenerateSingleWall( new Vector3(), Scale1, DoubleWall.Rotation, VoxelList.ElementAt(0)));
                TransWallVertices.AddRange( GenerateSingleWall( new Vector3(), Scale1, DoubleWall.Rotation, VoxelList.ElementAt(1)));

                WallVertices.AddRange(GenerateSingleWall(WallPos2, Scale2, DoubleWall.Rotation, VoxelList2.ElementAt(0)));
                TransWallVertices.AddRange( GenerateSingleWall(WallPos2, Scale2, DoubleWall.Rotation, VoxelList2.ElementAt(1)));
                
                if (WallVertices.Count != 0)
                {
                    VertexBuffer NewBuffer = new VertexBuffer(Graphics, WallVertex.VertexDeclaration, WallVertices.Count, BufferUsage.WriteOnly);
                    NewBuffer.SetData(WallVertices.ToArray());
                    DoubleWall.Geom.SetVertexBuffer(NewBuffer);

                    Vector3[] CollisionBuffer = new Vector3[WallVertices.Count];
                    for (int i = 0; i < WallVertices.Count; i++)
                    {
                        CollisionBuffer[i] = WallVertices[i].Position;
                    }
                    DoubleWall.TransGeom.SetCollisionBuffer(CollisionBuffer);
                }
                else
                {
                    DoubleWall.Geom.SetVertexBuffer(null);
                }

                if (TransWallVertices.Count != 0)
                {
                    VertexBuffer NewTransBuffer = new VertexBuffer(Graphics, WallVertex.VertexDeclaration, TransWallVertices.Count, BufferUsage.WriteOnly);
                    NewTransBuffer.SetData(TransWallVertices.ToArray());
                    DoubleWall.TransGeom.SetVertexBuffer(NewTransBuffer);

                    Vector3[] CollisionBuffer = new Vector3[TransWallVertices.Count];
                    for (int i = 0; i < TransWallVertices.Count; i++)
                    {
                        CollisionBuffer[i] = TransWallVertices[i].Position;
                    }
                    DoubleWall.TransGeom.SetCollisionBuffer(CollisionBuffer);
                }
                else
                {
                    DoubleWall.TransGeom.SetVertexBuffer(null);
                }

            }
    
        }
        
    
        public static List<WallVertex> GenerateSingleWall(Vector3 Position, Vector3 Scale, Vector3 Rot, int[,] Voxels)
        {
            float SizeX = Scale.X;
            float SizeY = Scale.Y;
            float SizeZ = Scale.Z;
            
            Vector3[] MarchingSquaresVertices = new Vector3[16]{
            new Vector3(0,0, 0), new Vector3(SizeX,0,0), new Vector3(0,0,SizeZ), new Vector3(SizeX,0,SizeZ),
            new Vector3(SizeX/2.0f,0,0), new Vector3(0,0,SizeZ/2.0f), new Vector3(SizeX,0,SizeZ/2.0f), new Vector3(SizeX/2.0f,0,SizeZ),

            new Vector3(0,SizeY, 0), new Vector3(SizeX,SizeY,0), new Vector3(0,SizeY,SizeZ), new Vector3(SizeX,SizeY,SizeZ),
            new Vector3(SizeX/2.0f,SizeY,0), new Vector3(0,SizeY,SizeZ/2.0f), new Vector3(SizeX,SizeY,SizeZ/2.0f), new Vector3(SizeX/2.0f,SizeY,SizeZ)};
            
            List<WallVertex> NewVertexBuffer = new List<WallVertex>();
            
            Matrix Rotation = new Matrix();
            Rotation += Matrix.CreateRotationZ( MathHelper.ToRadians(Rot.Z));
            Rotation *= Matrix.CreateRotationY( MathHelper.ToRadians(Rot.Y));
            Rotation *= Matrix.CreateRotationX( MathHelper.ToRadians( Rot.X));
            
            for (int i = 0; i < Voxels.GetLength(0)-1; i++)
            {
                for (int j = 0; j < Voxels.GetLength(1)-1; j++)
                {
                    int Val = 0;
                    Val += Voxels[i, j] != 0 ? 1 : 0;
                    Val += Voxels[i+1, j] != 0 ? 2 : 0;
                    Val += Voxels[i, j+1] != 0 ? 4 : 0;
                    Val += Voxels[i+1, j+1] != 0 ? 8 : 0;

                    int Material = MathUtil.CalculateMost(new int[] { Voxels[i, j], Voxels[i + 1, j], Voxels[i, j + 1], Voxels[i + 1, j + 1] });

                    float MX = (Material / 16) * 0.0625f;
                    float MY = (Material % 16) * 0.0625f;
                    
                    Vector3 WorldSpace = new Vector3(i * SizeX, 0, j * SizeZ) + Position;
                    
                    for (int k = MarchingSquaresIndices[Val].Length-1; k >= 0; k-= 3)
                    {
                        Vector3 P1 = MarchingSquaresVertices[MarchingSquaresIndices[Val][k]];
                        Vector3 P2 = MarchingSquaresVertices[MarchingSquaresIndices[Val][k-1]];
                        Vector3 P3 = MarchingSquaresVertices[MarchingSquaresIndices[Val][k-2]];
                        
                        if (P1.Y != P2.Y || P1.Y != P3.Y)
                        {
                            if (P1.Z != P2.Z || P1.Z != P3.Z)
                            {
                                Vector2 T1 = new Vector2(Math.Abs((P1.Y / 1.0f * 0.0625f)) + MX, Math.Abs((P1.Z / 1.0f * 0.0625f)) + MY);
                                Vector2 T2 = new Vector2(Math.Abs((P2.Y / 1.0f * 0.0625f)) + MX, Math.Abs((P2.Z / 1.0f * 0.0625f)) + MY);
                                Vector2 T3 = new Vector2(Math.Abs((P3.Y / 1.0f * 0.0625f)) + MX, Math.Abs((P3.Z / 1.0f * 0.0625f)) + MY);

                                NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P1 + WorldSpace), Rotation), T1));
                                NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P2 + WorldSpace), Rotation), T2));
                                NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P3 + WorldSpace), Rotation), T3));
                            }
                            else
                            {
                                Vector2 T1 = new Vector2(Math.Abs((P1.X / 1.0f * 0.0625f)) + MX, Math.Abs((P1.Y / 1.0f * 0.0625f)) + MY);
                                Vector2 T2 = new Vector2(Math.Abs((P2.X / 1.0f * 0.0625f)) + MX, Math.Abs((P2.Y / 1.0f * 0.0625f)) + MY);
                                Vector2 T3 = new Vector2(Math.Abs((P3.X / 1.0f * 0.0625f)) + MX, Math.Abs((P3.Y / 1.0f * 0.0625f)) + MY);

                                NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P1 + WorldSpace), Rotation), T1));
                                NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P2 + WorldSpace), Rotation), T2));
                                NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P3 + WorldSpace), Rotation), T3));
                            }
                        }
                        else
                        {
                            float OX = (WorldSpace.X % 1.0f) * 0.0625f;
                            float OY = (WorldSpace.Z % 1.0f) * 0.0625f;
                            
                            Vector2 T1 = new Vector2( (P1.X % 1.001f) * 0.0625f + MX + OX, (P1.Z % 1.001f) * 0.0625f + MY + OY);
                            Vector2 T2 = new Vector2( (P2.X % 1.001f) * 0.0625f + MX + OX, (P2.Z % 1.001f) * 0.0625f + MY + OY);
                            Vector2 T3 = new Vector2( (P3.X % 1.001f) * 0.0625f + MX + OX, (P3.Z % 1.001f) * 0.0625f + MY + OY);
                            
                            NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P1 + WorldSpace), Rotation), T1));
                            NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P2 + WorldSpace), Rotation), T2));
                            NewVertexBuffer.Add(new WallVertex(Vector3.Transform((P3 + WorldSpace), Rotation), T3));
                        }
                    }
                }
            }
            
            //wall edges(Top/Bottom)
            Vector3[] HorizontalPlane = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(SizeX/2, 0, 0),
                new Vector3(0, SizeY, 0),
                new Vector3(SizeX/2, SizeY, 0)
            };
            Vector2[] HorizontalTexCoords = new Vector2[]
            {  
                new Vector2(0,0),
                new Vector2(0.03125f * SizeX,0),
                new Vector2(0,0.0625f * SizeY),
                new Vector2(0.03125f * SizeX ,0.0625f * SizeY)
            };
            
            for (int i = 0; i < Voxels.GetLength(0)-1; i++)
            {
                if (Voxels[i, 0] != 0)
                {
                    Vector3 Pos = new Vector3(i * SizeX, 0, 0) + Position;
                    Vector2 Material = new Vector2((Voxels[i, 0] / 16) * 0.0625f + ( ((i*SizeX) % 1.0f) * 0.0625f ), (Voxels[i, 0] % 16) * 0.0625f);

                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[0] + Pos, Rotation), Material + HorizontalTexCoords[0]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[3] + Pos, Rotation), Material + HorizontalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                }
                if (Voxels[i + 1, 0] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[i + 1, 0] / 16) * 0.0625f + ((((i + 0.5f) * SizeX) % 1.0f) * 0.0625f), (Voxels[i + 1, 0] % 16) * 0.0625f);

                    Vector3 Pos = new Vector3(i * SizeX + SizeX / 2, 0, 0) + Position;

                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[0] + Pos, Rotation), Material + HorizontalTexCoords[0]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[3] + Pos, Rotation), Material + HorizontalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                }
                
                if (Voxels[i, Voxels.GetLength(1) - 1] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[i, Voxels.GetLength(1) - 1] / 16) * 0.0625f + (((i * SizeX) % 1.0f) * 0.0625f), (Voxels[i, Voxels.GetLength(1) - 1] % 16) * 0.0625f);
                    Vector3 Pos = new Vector3(i * SizeX, 0, (Voxels.GetLength(1) - 1) * SizeZ) + Position;
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[3] + Pos, Rotation), Material + HorizontalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[0] + Pos, Rotation), Material + HorizontalTexCoords[0]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                }
                if (Voxels[i + 1, Voxels.GetLength(1) - 1] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[i + 1, Voxels.GetLength(1) - 1] / 16) * 0.0625f + ((((i+0.5f) * SizeX) % 1.0f) * 0.0625f), (Voxels[i + 1, Voxels.GetLength(1) - 1] % 16) * 0.0625f);
                    Vector3 Pos = new Vector3(i * SizeX + SizeX/2, 0, (Voxels.GetLength(1) - 1) * SizeZ) + Position;
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[3] + Pos, Rotation), Material + HorizontalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[1] + Pos, Rotation), Material + HorizontalTexCoords[1]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[0] + Pos, Rotation), Material + HorizontalTexCoords[0]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(HorizontalPlane[2] + Pos, Rotation), Material + HorizontalTexCoords[2]));
                }
            }
            
            Vector3[] VerticalPlane = new Vector3[8]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, SizeY, 0),
                new Vector3(0, 0, SizeZ),
                new Vector3(0, SizeY, SizeZ),

                new Vector3(0, 0, 0),
                new Vector3(0, SizeY, 0),
                new Vector3(0, 0, SizeZ/2),
                new Vector3(0, SizeY, SizeZ/2),
            };
            Vector2[] VerticalTexCoords = new Vector2[]
            {
                new Vector2(0,0),

                new Vector2(0.03125f * SizeY,0),
                new Vector2(0.0625f * SizeY,0),

                new Vector2(0,0.03125f * SizeZ),
                new Vector2(0,0.0625f * SizeZ),

                new Vector2(0.0625f * SizeY,0.03125f * SizeZ),
                new Vector2(0.0625f * SizeY,0.0625f * SizeZ),

                new Vector2(0.03125f * SizeY,0.0625f * SizeZ),
            };
            
            for (int j = 0; j < Voxels.GetLength(1) - 1; j++)
            {
                if (Voxels[0, j] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[0, j] / 16) * 0.0625f, (Voxels[0, j] % 16) * 0.0625f + (((j * SizeZ) % 1.0f) * 0.0625f));
                    Vector3 Pos = new Vector3(0, 0, j * SizeZ) + Position;
                    NewVertexBuffer.Add(new WallVertex( Vector3.Transform(VerticalPlane[6] + Pos, Rotation), Material + VerticalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex( Vector3.Transform(VerticalPlane[4] + Pos, Rotation), Material + VerticalTexCoords[0]));
                    NewVertexBuffer.Add(new WallVertex( Vector3.Transform(VerticalPlane[5] + Pos, Rotation), Material + VerticalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex( Vector3.Transform(VerticalPlane[5] + Pos, Rotation), Material + VerticalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex( Vector3.Transform(VerticalPlane[7] + Pos, Rotation), Material + VerticalTexCoords[5]));
                    NewVertexBuffer.Add(new WallVertex( Vector3.Transform(VerticalPlane[6] + Pos, Rotation), Material + VerticalTexCoords[3]));
                }
                if (Voxels[0, j + 1] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[0, j + 1] / 16) * 0.0625f, (Voxels[0, j + 1] % 16) * 0.0625f + (((j * SizeZ) % 1.0f) * 0.0625f));
                    Vector3 Pos = new Vector3(0, 0, j * SizeZ + SizeZ/2) + Position;
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[6] + Pos, Rotation), Material + VerticalTexCoords[4]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[4] + Pos, Rotation), Material + VerticalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[5] + Pos, Rotation), Material + VerticalTexCoords[5]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[5] + Pos, Rotation), Material + VerticalTexCoords[5]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[7] + Pos, Rotation), Material + VerticalTexCoords[6]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[6] + Pos, Rotation), Material + VerticalTexCoords[4]));
                }

                if (Voxels[(Voxels.GetLength(0) - 1), j] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[ Voxels.GetLength(0) - 1, j] / 16) * 0.0625f, (Voxels[Voxels.GetLength(0) - 1, j] % 16) * 0.0625f + (((j* SizeZ) % 1.0f) * 0.0625f));
                    Vector3 Pos = new Vector3((Voxels.GetLength(0) - 1) * SizeX, 0, j * SizeZ) + Position;
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[7] + Pos, Rotation), Material + VerticalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[5] + Pos, Rotation), Material + VerticalTexCoords[0]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[4] + Pos, Rotation), Material + VerticalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[4] + Pos, Rotation), Material + VerticalTexCoords[2]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[6] + Pos, Rotation), Material + VerticalTexCoords[5]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[7] + Pos, Rotation), Material + VerticalTexCoords[3]));
                }
                if (Voxels[(Voxels.GetLength(0) - 1), j + 1] != 0)
                {
                    Vector2 Material = new Vector2((Voxels[ Voxels.GetLength(0) - 1, j + 1] / 16) * 0.0625f, (Voxels[Voxels.GetLength(0) - 1, j + 1] % 16) * 0.0625f +(((j * SizeZ) % 1.0f) * 0.0625f));
                    Vector3 Pos = new Vector3((Voxels.GetLength(0) - 1) * SizeX, 0, j * SizeZ + SizeZ/2) + Position;
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[7] + Pos, Rotation), Material + VerticalTexCoords[4]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[5] + Pos, Rotation), Material + VerticalTexCoords[3]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[4] + Pos, Rotation), Material + VerticalTexCoords[5]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[4] + Pos, Rotation), Material + VerticalTexCoords[5]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[6] + Pos, Rotation), Material + VerticalTexCoords[6]));
                    NewVertexBuffer.Add(new WallVertex(Vector3.Transform(VerticalPlane[7] + Pos, Rotation), Material + VerticalTexCoords[4]));
                }
            }
            return NewVertexBuffer;
        }
        
    }
}
