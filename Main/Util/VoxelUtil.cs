using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Util
{
    public static class VoxelUtil
    {
        public static byte[,] ReturnReplaceValue(byte[,] Array, byte OldVal, byte NewVal)
        {
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    if (Array[i, j] == OldVal)
                    {
                        Array[i, j] = NewVal;
                    }
                }
            }
            return Array;
        }

        public static int[,] AddValues(int[,] ArrayA, int[,] ArrayB)
        {
            int[,] Array = new int[ArrayA.GetLength(0), ArrayA.GetLength(1)];
            for (int i = 0; i < ArrayA.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayA.GetLength(1); j++)
                {
                    Array[i, j] = ArrayA[i, j] + ArrayB[i, j];
                }
            }
            return Array;
        }

        public static void ReplaceValue(byte[,] Array, byte OldVal, byte NewVal)
        {
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    if ( Array[i,j] == OldVal)
                    {
                        Array[i, j] = NewVal;
                    }
                }
            }
        }

        public static void ReplaceValue(int[,] Array, int OldVal, int NewVal)
        {
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    if (Array[i, j] == OldVal)
                    {
                        Array[i, j] = NewVal;
                    }
                }
            }
        }

        public static short[] FlattenArray(short[,] OldArray)
        {
            short[] NewArray = new short[OldArray.GetLength(0) * OldArray.GetLength(1)];
            for (int i = 0; i < OldArray.GetLength(0); i++)
            {
                for (int j = 0; j < OldArray.GetLength(1); j++)
                {
                    NewArray[j * OldArray.GetLength(0) + j] = OldArray[i, j];
                }
            }
            return NewArray; 
        }

        public static ushort[,] DownScaleAverage(ushort[,] OldArray, float Scale)
        {
            int SquareSize = (int)(1.0f / Scale);

            ushort[,] NewArray = new ushort[(int)(OldArray.GetLength(0) * Scale), (int)(OldArray.GetLength(1) * Scale)];

            for (int i = 0; i < NewArray.GetLength(0); i++)
            {
                for (int j = 0; j < NewArray.GetLength(0); j++)
                {
                    int Val = 0;
                    for (int x = 0; x < SquareSize; x++)
                    {
                        for (int y = 0; y < SquareSize; y++)
                        {
                            Val += OldArray[i * SquareSize + x, j * SquareSize + y];
                        }
                    }
                    Val /= SquareSize * SquareSize;
                    NewArray[i, j] = (ushort)Val;
                }
            }
            return NewArray;
        }
        
        public static int[,] DownScaleAverage(int[,] OldArray, float Scale)
        {
            int SquareSize = (int)(1.0f / Scale);

            int[,] NewArray = new int[(int)(OldArray.GetLength(0) * Scale), (int)(OldArray.GetLength(1) * Scale)];
            
            for (int i = 0; i < NewArray.GetLength(0); i++)
            {
                for (int j = 0; j < NewArray.GetLength(0); j++)
                {
                    int Val = 0;
                    for (int x = 0; x < SquareSize; x++)
                    {
                        for (int y = 0; y < SquareSize; y++)
                        {
                            Val += OldArray[i * SquareSize + x, j * SquareSize + y];
                        }
                    }
                    Val /= SquareSize * SquareSize;
                    NewArray[i, j] = Val;
                }
            }
            return NewArray;
        }

        public static byte[,,] DownScaleMost(byte[,,] OldArray, float Scale)
        {
            int SquareSize = (int)(1.0f / Scale);

            byte[,,] NewArray = new byte[(int)(OldArray.GetLength(0) * Scale), (int)(OldArray.GetLength(1) * Scale), OldArray.GetLength(2)];

            List<byte> SampleSquareList = new List<byte>();

            for (int i = 0; i < NewArray.GetLength(0); i++)
            {
                for (int j = 0; j < NewArray.GetLength(0); j++)
                {

                    for (int x = 0; x < SquareSize; x++)
                    {
                        for (int y = 0; y < SquareSize; y++)
                        {
                            for (int z = 0; z < OldArray.GetLength(2); z++)
                            {
                                SampleSquareList.Add(OldArray[i * SquareSize + x, j * SquareSize + y, z]);
                            }

                        }
                    }


                    for (int k = 0; k < OldArray.GetLength(2); k++)
                    {
                        NewArray[i, j, k] = SampleSquareList.GroupBy(Val => Val).OrderBy(Group => Group.Count()).Last().Key;
                    }
                    
                    SampleSquareList.Clear();
                }
            }

            return NewArray;
        }

        public static int[,] DownScaleMost(int[,] OldArray, float Scale)
        {
            int SquareSize = (int)(1.0f / Scale);
            
            int[,] NewArray = new int[ (int)(OldArray.GetLength(0) * Scale), (int)(OldArray.GetLength(1) * Scale)];

            List<int> SampleSquareList = new List<int>();

            for (int i = 0; i < NewArray.GetLength(0); i++)
            {
                for (int j = 0; j < NewArray.GetLength(0); j++)
                {
                    for (int x = 0; x < SquareSize; x++)
                    {
                        for (int y = 0; y < SquareSize; y++)
                        {
                            SampleSquareList.Add(OldArray[i*SquareSize + x, j * SquareSize + y]);
                        }
                    }
                    NewArray[i, j] = SampleSquareList.GroupBy(Val => Val).OrderBy(Group => Group.Count()).Last().Key;

                    SampleSquareList.Clear();
                }
            }

            return NewArray;
        }


        public static int[,] ScaleVoxels(int[,] TargetField, int[,] Merger)
        {
            int[,] NewVoxels = (int[,])TargetField.Clone();
            for (float i = 0; i < TargetField.GetLength(0); i++)
            {
                for (float j = 0; j < TargetField.GetLength(1); j++)
                {
                    int TX = (int)(i / (TargetField.GetLength(0)) * Merger.GetLength(0));
                    int TY = (int)(j / (TargetField.GetLength(1)) * Merger.GetLength(1));

                    TX = TX < Merger.GetLength(0) ? TX : Merger.GetLength(0) - 1;
                    TY = TY < Merger.GetLength(1) ? TY : Merger.GetLength(1) - 1;

                    NewVoxels[ (int)i, (int)j] = Merger[TX, TY];
                }
            }
            return NewVoxels;
        }


        public static byte[,,] MergeFields(byte[,,] TargetField, byte[,,] Merger)
        {
            byte[,,] NewVoxels = (byte[,,])TargetField.Clone();
            for (int i = 0; i < TargetField.GetLength(0) && i < Merger.GetLength(0); i++)
            {
                for (int j = 0; j < TargetField.GetLength(1) && j < Merger.GetLength(1); j++)
                {
                    for (int k = 0; k < TargetField.GetLength(2) && k < Merger.GetLength(2); k++)
                    {
                        NewVoxels[i, j, k] = Merger[i, j, k];
                    }
                }
            }
            return NewVoxels;
        }

        public static byte[,] MergeFields(byte[,] TargetField, byte[,] Merger)
        {
            byte[,] NewVoxels = (byte[,])TargetField.Clone();
            for (int i = 0; i < TargetField.GetLength(0) && i < Merger.GetLength(0); i++)
            {
                for (int j = 0; j < TargetField.GetLength(1) && j < Merger.GetLength(1); j++)
                {
                    NewVoxels[i, j] = Merger[i, j];
                }
            }
            return NewVoxels;
        }

        public static short[,] MergeFields(short[,] TargetField, short[,] Merger)
        {
            short[,] NewVoxels = (short[,])TargetField.Clone();
            for (int i = 0; i < TargetField.GetLength(0) && i < Merger.GetLength(0); i++)
            {
                for (int j = 0; j < TargetField.GetLength(1) && j < Merger.GetLength(1); j++)
                {
                    NewVoxels[i, j] = Merger[i, j];
                }
            }
            return NewVoxels;
        }

        public static ushort[,] MergeFields(ushort[,] TargetField, ushort[,] Merger)
        {
            ushort[,] NewVoxels = (ushort[,])TargetField.Clone();
            for (int i = 0; i < TargetField.GetLength(0) && i < Merger.GetLength(0); i++)
            {
                for (int j = 0; j < TargetField.GetLength(1) && j < Merger.GetLength(1); j++)
                {
                    NewVoxels[i, j] = Merger[i, j];
                }
            }
            return NewVoxels;
        }

        public static int[,] MergeFields(int[,] TargetField, int[,] Merger)
        {
            int[,] NewVoxels = (int[,])TargetField.Clone();
            for (int i = 0; i < TargetField.GetLength(0) && i < Merger.GetLength(0); i++)
            {
                for (int j = 0; j < TargetField.GetLength(1) && j < Merger.GetLength(1); j++)
                {
                    NewVoxels[i, j] = Merger[i, j];
                }
            }
            return NewVoxels;
        }

        public static List<int[,]> SplitVoxels(int[,] Voxels, int TransTolerance)
        {
            int[,] NormVoxels = new int[Voxels.GetLength(0), Voxels.GetLength(1)];
            int[,] TransVoxels = new int[Voxels.GetLength(0), Voxels.GetLength(1)];

            for (int i = 0; i < Voxels.GetLength(0); i++)
            {
                for (int j = 0; j < Voxels.GetLength(1); j++)
                {
                    if (Voxels[i, j] != 0)
                    {
                        if (Voxels[i, j] < TransTolerance)
                        {
                            NormVoxels[i, j] = Voxels[i, j];
                        }
                        else
                        {
                            TransVoxels[i, j] = Voxels[i, j];
                        }
                    }

                }
            }
            List<int[,]> Data = new List<int[,]>();
            Data.Add(NormVoxels);
            Data.Add(TransVoxels);
            return Data;
        }

        public static void GetClosestMaterialVertex(Vector3 P1, Vector3 P2, Vector3 P3, int[,] Voxels, Vector3 Scale, Vector2 T1, Vector2 T2, Vector2 T3)
        {
            Vector3 C = (P1 + P2 + P3) * Scale / 3;

            float MX = (Voxels[(int)Math.Round(C.X), (int)Math.Round(C.Y)] / 16) * 0.0625f;
            float MY = (Voxels[(int)Math.Round(C.X), (int)Math.Round(C.Y)] % 16) * 0.0625f;

            T1 = new Vector2((P1.X / Scale.X) * 0.0625f + MX, (P1.Z / Scale.Z) * 0.0625f + MY);
            T2 = new Vector2((P2.X / Scale.X) * 0.0625f + MX, (P2.Z / Scale.Z) * 0.0625f + MY);
            T3 = new Vector2((P3.X / Scale.X) * 0.0625f + MX, (P3.Z / Scale.Z) * 0.0625f + MY);
        }
        
        public static int GetClosestMaterialVertex(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 Scale, int[,] Voxels)
        {
            Vector2 C = (P1 + P2 + P3) * Scale  / 3;

            float MX = (Voxels[(int)Math.Round(C.X), (int)Math.Round(C.Y)] / 16);
            float MY = (Voxels[(int)Math.Round(C.X), (int)Math.Round(C.Y)] % 16);
            
            int Material = Voxels[(int)Math.Round(C.X), (int)Math.Round(C.Y)];
            Material = Material == 0 ? Voxels[(int)Math.Round(P1.X * Scale.X), (int)Math.Round(P1.Y * Scale.Y)] : Material;
            Material = Material == 0 ? Voxels[(int)Math.Round(P2.X * Scale.X), (int)Math.Round(P2.Y * Scale.Y)] : Material;
            Material = Material == 0 ? Voxels[(int)Math.Round(P3.X * Scale.X), (int)Math.Round(P3.Y * Scale.Y)] : Material;
            

            return Material;
        }
    }
}
