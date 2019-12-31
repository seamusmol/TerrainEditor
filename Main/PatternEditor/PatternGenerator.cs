using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.PatternEditor
{
    public static class PatternGenerator
    {
        /*
            V2---V7---v3
            | \       |
            |  \      |  
            v5   v8   v6
            |      \  |     
            |       \ |
            V0---v4---v1
         */
         
        public static int[][] MarchingSquaresIndices = new int[16][]{
            new int[]{ },
            new int[]{ 5,0,4},
            new int[]{ 4,1,6},
            new int[]{ 5,0,4,4,1,6,5,4,6},
            new int[]{ 2,5,7},
            new int[]{ 2,0,4,4,7,2},
            new int[]{ 2,5,7,5,6,7,5,4,6,4,1,6},
            new int[]{ 2,5,7,5,6,7,5,0,4,4,1,6,5,4,6},
            new int[]{ 7,6,3},
            new int[]{ 5,0,4,5,4,6,5,6,7,6,3,7},
            new int[]{ 7,4,1,1,3,7},
            new int[]{ 5,0,4,4,1,6,5,4,6,5,6,7,6,3,7},
            new int[]{ 2,5,7,5,6,7,6,3,7},
            new int[]{ 2,5,7,5,6,7,6,3,7,5,4,6,5,0,4},
            new int[]{ 2,5,7,5,6,7,6,3,7,5,4,6,4,1,6},
            new int[]{ 2,5,8,8,7,2,7,8,6,6,3,7,5,0,4,4,8,5,8,4,1,1,6,8}
        };
        
        public static void GeneratePatternImage(int [,] Voxels, float ScaleX, float ScaleY, LockBitMap PatternImage, LockBitMap TextureLookup)
        {
            float VoxelSizeX = PatternImage.Width / (Voxels.GetLength(0));
            float VoxelSizeY = PatternImage.Height / (Voxels.GetLength(1));

            int ISX = TextureLookup.Width / 16;
            int ISY = TextureLookup.Height / 16;

            Vector2[] Position = new Vector2[]
            {
                new Vector2(0,0),
                new Vector2(VoxelSizeX,0),
                new Vector2(0,VoxelSizeY),
                new Vector2(VoxelSizeX,VoxelSizeY),
                new Vector2(VoxelSizeX/2,0),
                new Vector2(0,VoxelSizeY/2),
                new Vector2(VoxelSizeX, VoxelSizeY/2),
                new Vector2(VoxelSizeX/2,VoxelSizeY),
                new Vector2(VoxelSizeX/2,VoxelSizeY/2)
            };
            
            PatternImage.Clear();

            List<int[,]> SplitVoxelFields = VoxelUtil.SplitVoxels(Voxels, 239);

            Vector2 Scale = new Vector2( 1.0f/VoxelSizeX,1.0f/ VoxelSizeY);

            for (int index = 0; index < SplitVoxelFields.Count; index++)
            {
                int[,] VoxelField = SplitVoxelFields[index];

                for (int i = 0; i < VoxelField.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < VoxelField.GetLength(1) - 1; j++)
                    {
                        int Val = 0;
                        Val += VoxelField[i, j] != 0 ? 1 : 0;
                        Val += VoxelField[i + 1, j] != 0 ? 2 : 0;
                        Val += VoxelField[i, j + 1] != 0 ? 4 : 0;
                        Val += VoxelField[i + 1, j + 1] != 0 ? 8 : 0;
                        
                        //int Material = MathUtil.CalculateMost(new int[] { VoxelField[i, j], VoxelField[i + 1, j], VoxelField[i, j + 1], VoxelField[i + 1, j + 1] });
                        
                        Vector2 VoxelPosition = new Vector2(i * VoxelSizeX, j * VoxelSizeY);
                        

                        for (int k = 0; k < MarchingSquaresIndices[Val].Length; k += 3)
                        {
                            Vector2 P1 = Position[MarchingSquaresIndices[Val][k]] + VoxelPosition;
                            Vector2 P2 = Position[MarchingSquaresIndices[Val][k + 1]] + VoxelPosition;
                            Vector2 P3 = Position[MarchingSquaresIndices[Val][k + 2]] + VoxelPosition;
                            
                            int Material = VoxelUtil.GetClosestMaterialVertex(P1,P2,P3, Scale, VoxelField);

                            int MX = (Material / 16);
                            int MY = (Material % 16);

                            FillTriangle(VoxelSizeX, VoxelSizeY, ISX, ISY, MX, MY, ScaleX, ScaleY, P1, P2, P3, PatternImage, TextureLookup);
                        }
                    }
                }
            }
            
        }

        private static void FillTriangle( float VoxelSizeX, float VoxelSizeY, int ISX, int ISY, int MX, int MY, float ScaleX, float ScaleY, Vector2 P1, Vector2 P2, Vector2 P3, LockBitMap ShadeMap, LockBitMap TextureLookup)
        {
            int MaxX = (int)Math.Max(P1.X, Math.Max(P2.X, P3.X));
            int MaxY = (int)Math.Max(P1.Y, Math.Max(P2.Y, P3.Y));
            int MinX = (int)Math.Min(P1.X, Math.Min(P2.X, P3.X));
            int MinY = (int)Math.Min(P1.Y, Math.Min(P2.Y, P3.Y));

            Vector2 Pos = new Vector2();
            for (int i = MinX; i <= MaxX; i++)
            {
                for (int j = MinY; j <= MaxY; j++)
                {
                    Pos.X = i;
                    Pos.Y = j;
                    
                    int TX = (int)((i % VoxelSizeX) / VoxelSizeX * ISX);
                    int TY = (int)((j % VoxelSizeY) / VoxelSizeY * ISY);
                    
                    if (CollisionUtil.InsideTriangle(Pos, P1, P2, P3))
                    {
                        ShadeMap.SetPixel( i, j, TextureLookup.GetPixel( TX + MX * ISX, TY + ISY * MY));
                    }
                }
            }
        }
    }
}
