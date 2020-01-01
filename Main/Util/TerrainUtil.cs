using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Util
{
    public static class TerrainUtil
    {
        public static void RemoveWaterBelowTerrain(byte[,] WaterMap, int[,] WaterHeightMap)
        {
            for (int i = 0; i < WaterMap.GetLength(0); i++)
            {
                for (int j = 0; j < WaterMap.GetLength(1); j++)
                {
                    if (WaterHeightMap[i ,j] < 0 && WaterHeightMap[i + 1, j] < 0 && WaterHeightMap[i ,j + 1] < 0 && WaterHeightMap[i + 1, j + 1] < 0)
                    {
                        WaterMap[i, j] = 0;
                    }
                }
            }
        }

        public static void AdjustWaterHeight(int[,] OldHeightMap, int[,] NewHeightMap, int[,] WaterHeightMap)
        {
            for (int i = 0; i < OldHeightMap.GetLength(0); i++)
            {
                for (int j = 0; j < OldHeightMap.GetLength(1); j++)
                {
                    int Difference = OldHeightMap[i, j] - NewHeightMap[i, j];

                    if (Difference != 0)
                    {
                        WaterHeightMap[i, j] = (int)(WaterHeightMap[i, j] + Difference);
                    }
                }
            }
        }

        public static void AdjustWaterHeight(float[,] OldHeightMap, float[,] NewHeightMap, float[,] WaterHeightMap)
        {
            for (int i = 0; i < OldHeightMap.GetLength(0); i++)
            {
                for (int j = 0; j < OldHeightMap.GetLength(1); j++)
                {
                    float Difference = OldHeightMap[i, j] - NewHeightMap[i, j];

                    if (Difference != 0)
                    {
                        WaterHeightMap[i, j] = (float)(WaterHeightMap[i, j] + Difference);
                    }
                }
            }
        }

        public static void MergeEdges(byte[,] ArrayA, byte[,] ArrayB, bool IsVertical, int MergeMode)
        {
            if (MergeMode == 0)
            {

            }
            else if (MergeMode == 1)
            {
                if (IsVertical)
                {
                    for (int j = 0; j < ArrayA.GetLength(1); j++)
                    {
                        ArrayB[ArrayB.GetLength(0) - 1, j] = ArrayA[0, j];
                    }
                }
                else
                {
                    for (int i = 0; i < ArrayA.GetLength(1); i++)
                    {
                        ArrayB[i, 0] = ArrayA[i, ArrayA.GetLength(1) - 1];
                    }
                }

            }
            if (MergeMode == 2)
            {
                if (IsVertical)
                {
                    for (int j = 0; j < ArrayA.GetLength(1); j++)
                    {
                        byte Val = (byte)((ArrayA[0, j] + ArrayB[ArrayB.GetLength(0) - 1, j]) / 2);
                        ArrayA[0, j] = Val;
                        ArrayB[ArrayB.GetLength(0) - 1, j] = Val;
                    }
                }
                else
                {
                    for (int i = 0; i < ArrayA.GetLength(1); i++)
                    {
                        byte Val = (byte)((ArrayA[i, ArrayA.GetLength(1) - 1] + ArrayB[i, 0]) / 2);
                        ArrayA[i, ArrayA.GetLength(1) - 1] = Val;
                        ArrayB[i, 0] = Val;
                    }
                }
            }
        }
        
        public static void MergeEdges(int[,] ArrayA, int[,] ArrayB, bool IsVertical, int MergeMode) 
        {
            if (MergeMode == 0)
            {

            }
            else if (MergeMode == 1)
            {
                if (IsVertical)
                {
                    for (int j = 0; j < ArrayA.GetLength(1); j++)
                    {
                        ArrayB[ArrayB.GetLength(0) - 1, j] = ArrayA[0, j];
                    }
                }
                else
                {
                    for (int i = 0; i < ArrayA.GetLength(1); i++)
                    {
                        ArrayB[i, 0] = ArrayA[i, ArrayA.GetLength(1) - 1];
                    }
                }
            }
            else if (MergeMode == 2)
            {
                if (IsVertical)
                {
                    for (int j = 0; j < ArrayA.GetLength(1); j++)
                    {
                        int Val = (ArrayA[0, j] + ArrayB[ArrayB.GetLength(0) - 1, j]) / 2;
                        ArrayA[0, j] = Val;
                        ArrayB[ArrayB.GetLength(0) - 1, j] = Val;
                    }
                }
                else
                {
                    for (int i = 0; i < ArrayA.GetLength(1); i++)
                    {
                        int Val = (ArrayA[i, ArrayA.GetLength(1) - 1] + ArrayB[i, 0]) / 2;
                        ArrayA[i, ArrayA.GetLength(1) - 1] = Val;
                        ArrayB[i, 0] = Val;
                    }
                }
            }
        }


        public static void MergeEdges(float[,] ArrayA, float[,] ArrayB, bool IsVertical, int MergeMode)
        {
            if (MergeMode == 0)
            {

            }
            else if (MergeMode == 1)
            {
                if (IsVertical)
                {
                    for (int j = 0; j < ArrayA.GetLength(1); j++)
                    {
                        ArrayB[ArrayB.GetLength(0) - 1, j] = ArrayA[0, j];
                    }
                }
                else
                {
                    for (int i = 0; i < ArrayA.GetLength(1); i++)
                    {
                        ArrayB[i, 0] = ArrayA[i, ArrayA.GetLength(1) - 1];
                    }
                }

            }
            if (MergeMode == 2)
            {
                if (IsVertical)
                {
                    for (int j = 0; j < ArrayA.GetLength(1); j++)
                    {
                        float Val = ((ArrayA[0, j] + ArrayB[ArrayB.GetLength(0) - 1, j]) / 2);
                        ArrayA[0, j] = Val;
                        ArrayB[ArrayB.GetLength(0) - 1, j] = Val;
                    }
                }
                else
                {
                    for (int i = 0; i < ArrayA.GetLength(1); i++)
                    {
                        float Val = ((ArrayA[i, ArrayA.GetLength(1) - 1] + ArrayB[i, 0]) / 2);
                        ArrayA[i, ArrayA.GetLength(1) - 1] = Val;
                        ArrayB[i, 0] = Val;
                    }
                }
            }
        }

        public static void ApplySquareBrush(int X1, int X2, int Y1, int Y2, float[,] Q, int[,] AffectedMap, int Shape, int Tool, int Radius, int Val, float Flow)
        {
            ApplySquareBrush( X1, X2, Y1, Y2, Q, AffectedMap, new int[,]{ }, Shape, Tool, Radius, Val, Flow);
        }
        //Tool ID:
        //0: Set
        //1: +/- 
        //2: smooth
        //3: Set to Pattern

        //Shape ID:
        //0: single point
        //1: Square
        public static void ApplySquareBrush(int X1, int X2, int Y1, int Y2, float[,] Q, int[,] AffectedMap, int[,]AdjustmentMap, int Shape, int Tool, int Radius, int Val, float Flow)
        {
            if (Tool == 0 || Tool == 3)
            {
                if (Shape == 0)
                {
                    //single square
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] = (int)Q[0,0];
                    }
                }
                else if (Shape == 1)
                {
                    for (int i = X1; i <= X2; i++)
                    {
                        for (int j = Y1; j <= Y2; j++)
                        {
                            if (i >= 0 && j >= 0 && i < AffectedMap.GetLength(0) && j < AffectedMap.GetLength(1))
                            {
                                AffectedMap[i, j] = (int)Q[0, 0];
                            }
                        }
                    }
                }
            }
            else if (Tool == 1)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] += (int)(Val * Flow);
                    }
                }
                else if (Shape == 1)
                {
                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                if (i >= 0 && j >= 0 && i < AffectedMap.GetLength(0) && j < AffectedMap.GetLength(1))
                                {
                                    AffectedMap[i, j] += (int)(Val * Flow);
                                }
                            }
                        }
                    }
                }
            }
            else if (Tool == 2)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] = Val;
                    }
                }
                else if (Shape == 1)
                {
                    if (Q.GetLength(0) != 2 || Q.GetLength(1) != 2)
                    {
                        return;
                    }

                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                AffectedMap[i, j] = (int)MathUtil.Lerp(Flow, 0, 100, AffectedMap[i, j], MathUtil.BiLerp(i, j, X1, X2, Y1, Y2, Q[0, 0], Q[0, 1], Q[1, 0], Q[1, 1]));
                            }
                        }
                    }
                }
            }
            else if (Tool == 5)
            {
                if (AdjustmentMap.GetLength(0) == AffectedMap.GetLength(0) && AdjustmentMap.GetLength(1) == AffectedMap.GetLength(1))
                {
                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                AffectedMap[i, j] = (int)Q[0, 0] - AdjustmentMap[i, j];
                            }
                        }
                    }
                }
            }
        }

        public static void ApplySquareBrush(int X1, int X2, int Y1, int Y2, float[,] Q, byte[,] AffectedMap, int Shape, int Tool, int Radius, int Val, float Flow)
        {
            ApplySquareBrush(X1, X2, Y1, Y2, Q, AffectedMap, new byte[,] { }, Shape, Tool, Radius, Val, Flow);
        }

        public static void ApplySquareBrush(int X1, int X2, int Y1, int Y2, float[,] Q, byte[,] AffectedMap, byte[,] AdjustmentMap, int Shape, int Tool, int Radius, int Val, float Flow)
        {
            if (Tool == 0 || Tool == 3)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] = (byte)Q[0, 0];
                    }
                }
                else if (Shape == 1)
                {
                    for (int i = X1; i <= X2; i++)
                    {
                        for (int j = Y1; j <= Y2; j++)
                        {
                            if (i >= 0 && j >= 0 && i < AffectedMap.GetLength(0) && j < AffectedMap.GetLength(1))
                            {
                                AffectedMap[i, j] = (byte)Q[0, 0];
                            }
                        }
                    }
                }
            }
            else if (Tool == 1)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] += (byte)(Val * Flow);
                    }
                }
                else if (Shape == 1)
                {
                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                if (i >= 0 && j >= 0 && i < AffectedMap.GetLength(0) && j < AffectedMap.GetLength(1))
                                {
                                    AffectedMap[i, j] += (byte)(Val * Flow);
                                }
                            }
                        }
                    }
                }
            }
            else if (Tool == 2)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] = (byte)Val;
                    }
                }
                else if (Shape == 1)
                {
                    if (Q.GetLength(0) != 2 || Q.GetLength(1) != 2)
                    {
                        return;
                    }

                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                AffectedMap[i, j] = (byte)(int)MathUtil.Lerp(Flow, 0, 100, AffectedMap[i, j], MathUtil.BiLerp(i, j, X1, X2, Y1, Y2, Q[0, 0], Q[0, 1], Q[1, 0], Q[1, 1]));
                            }
                        }
                    }
                }
            }
            else if (Tool == 5)
            {

                if (AdjustmentMap.GetLength(0) == AffectedMap.GetLength(0) && AdjustmentMap.GetLength(1) == AffectedMap.GetLength(1))
                {
                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                AffectedMap[i, j] = (byte)(Q[0, 0] - AdjustmentMap[i, j]);
                            }
                        }
                    }
                }
            }
        }

        public static void ApplySquareBrush(int X1, int X2, int Y1, int Y2, float[,] Q, float[,] AffectedMap, int Shape, int Tool, int Radius, int Val, float Flow)
        {
            ApplySquareBrush(X1, X2, Y1, Y2, Q, AffectedMap, new float[,] { }, Shape, Tool, Radius, Val, Flow);
        }

        public static void ApplySquareBrush(int X1, int X2, int Y1, int Y2, float[,] Q, float[,] AffectedMap, float[,] AdjustmentMap, int Shape, int Tool, int Radius, int Val, float Flow)
        {
            if (Tool == 0 || Tool == 3)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] = Q[0, 0];
                    }
                }
                else if (Shape == 1)
                {
                    for (int i = X1; i <= X2; i++)
                    {
                        for (int j = Y1; j <= Y2; j++)
                        {
                            if (i >= 0 && j >= 0 && i < AffectedMap.GetLength(0) && j < AffectedMap.GetLength(1))
                            {
                                AffectedMap[i, j] = Q[0, 0];
                            }
                        }
                    }
                }
            }
            else if (Tool == 1)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] += (Val * Flow);
                    }
                }
                else if (Shape == 1)
                {
                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                if (i >= 0 && j >= 0 && i < AffectedMap.GetLength(0) && j < AffectedMap.GetLength(1))
                                {
                                    AffectedMap[i, j] += (Val * Flow);
                                }
                            }
                        }
                    }
                }
            }
            else if (Tool == 2)
            {
                if (Shape == 0)
                {
                    if (X1 >= 0 && Y1 >= 0 && X1 < AffectedMap.GetLength(0) && Y1 < AffectedMap.GetLength(1))
                    {
                        AffectedMap[X1, Y1] = (byte)Val;
                    }
                }
                else if (Shape == 1)
                {
                    if (Q.GetLength(0) != 2 || Q.GetLength(1) != 2)
                    {
                        return;
                    }

                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                AffectedMap[i, j] = MathUtil.Lerp(Flow, 0, 100, AffectedMap[i, j], MathUtil.BiLerp(i, j, X1, X2, Y1, Y2, Q[0, 0], Q[0, 1], Q[1, 0], Q[1, 1]));
                            }
                        }
                    }
                }
            }
            else if (Tool == 5)
            {

                if (AdjustmentMap.GetLength(0) == AffectedMap.GetLength(0) && AdjustmentMap.GetLength(1) == AffectedMap.GetLength(1))
                {
                    if (X1 >= 0 && X2 < AffectedMap.GetLength(0) && Y1 >= 0 && Y2 < AffectedMap.GetLength(1))
                    {
                        for (int i = X1; i <= X2; i++)
                        {
                            for (int j = Y1; j <= Y2; j++)
                            {
                                AffectedMap[i, j] = (Q[0, 0] - AdjustmentMap[i, j]);
                            }
                        }
                    }
                }
            }
        }


        public static void ApplyWaterBrush(Vector3 Position, int[,] TerrainHeightMap, int[,] WaterHeightMap, byte[,] WaterMap, int Shape, int Tool, int Radius, byte HasWater, int Val, float Flow)
        {
            int PX = (int)Math.Floor(Position.X);
            int PY = (int)Math.Floor(Position.Y);
            
            if (Tool == 0)
            {
                if (Shape == 0)
                {  
                    if (PX >= 0 && PY >= 0 && PX < WaterMap.GetLength(0) && PY < WaterMap.GetLength(1))
                    {
                        WaterMap[PX, PY] = HasWater;
                    }
                }
                else if (Shape == 1)
                {
                    for (int i = PX - Radius; i <= PX + Radius; i++)
                    {
                        for (int j = PY - Radius; j <= PY + Radius; j++)
                        {
                            if (i >= 0 && j >= 0 && i < WaterMap.GetLength(0) && j < WaterMap.GetLength(1))
                            {
                                WaterHeightMap[i, j] = (short)(WaterHeightMap[i, j] > 0 ? WaterHeightMap[i, j] : 0);
                                WaterMap[i, j] = HasWater;
                            }
                        }
                    }
                }
            }
            else if (Tool == 1)
            {
                //adjust WaterHeight
                if (Shape == 0)
                {
                    int Height =
                        (TerrainHeightMap[PX, PY] + TerrainHeightMap[PX + 1, PY] + TerrainHeightMap[PX, PY + 1] + TerrainHeightMap[PX + 1, PY + 1] +
                        WaterHeightMap[PX, PY] + WaterHeightMap[PX + 1, PY] + WaterHeightMap[PX, PY + 1] + WaterHeightMap[PX + 1, PY + 1]) / 4;

                    if (PX >= 0 && PY >= 0 && PX < WaterHeightMap.GetLength(0) && PY < WaterHeightMap.GetLength(1))
                    {
                        WaterHeightMap[PX, PY] = (short)(Height - TerrainHeightMap[PX, PY]);
                        WaterHeightMap[PX+1, PY] = (short)(Height - TerrainHeightMap[PX+1, PY]);
                        WaterHeightMap[PX, PY+1] = (short)(Height - TerrainHeightMap[PX, PY+1]);
                        WaterHeightMap[PX+1, PY+1] = (short)(Height - TerrainHeightMap[PX+1, PY+1]);
                    }
                }
                else if (Shape == 1)
                {
                    int Height =
                        (TerrainHeightMap[PX, PY] + TerrainHeightMap[PX + 1, PY] + TerrainHeightMap[PX, PY + 1] + TerrainHeightMap[PX + 1, PY + 1] +
                        WaterHeightMap[PX, PY] + WaterHeightMap[PX + 1, PY] + WaterHeightMap[PX, PY + 1] + WaterHeightMap[PX + 1, PY + 1]) / 4;

                    for (int i = PX - Radius; i <= PX + Radius + 1; i++)
                    {
                        for (int j = PY - Radius; j <= PY + Radius + 1; j++)
                        {
                            if (i >= 0 && j >= 0 && i < WaterHeightMap.GetLength(0) && j < WaterHeightMap.GetLength(1))
                            {
                                WaterHeightMap[i, j] = (short)(Height - TerrainHeightMap[i, j]);
                            }
                        }
                    }
                }
            }
            else if (Tool == 2)
            {
                if (Shape == 0)
                {
                    if (PX >= 0 && PY >= 0 && PX < WaterHeightMap.GetLength(0) && PY < WaterHeightMap.GetLength(1))
                    {
                        WaterHeightMap[PX, PY] += (short)(Val * Flow);
                    }
                }
                else if (Shape == 1)
                {
                    for (int i = PX - Radius; i <= PX + Radius + 1; i++)
                    {
                        for (int j = PY - Radius; j <= PY + Radius + 1; j++)
                        {
                            if (i >= 0 && j >= 0 && i < WaterHeightMap.GetLength(0) && j < WaterHeightMap.GetLength(1))
                            {
                                WaterHeightMap[i, j] += (short)(Val * Flow);
                            }
                        }
                    }
                }
            }
        }

       

    }
}
