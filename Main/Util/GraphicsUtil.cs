using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Main.Util
{
    public static class GraphicsUtil
    {

        public static IndexBuffer GenerateIndexBuffer(GraphicsDevice Graphics, int Length)
        {
            IndexBuffer NewBuffer = new IndexBuffer(Graphics, IndexElementSize.ThirtyTwoBits, Length, BufferUsage.None);

            int[] Indices = new int[Length];
            for (int i = 0; i < Length; i++)
            {
                Indices[i] = i;
            }
            NewBuffer.SetData(Indices);
            return NewBuffer;
        }

        public static Vector3 GenerateNormal(Vector3 P1, Vector3 P2, Vector3 P3)
        {
            return Vector3.Normalize(Vector3.Cross((P2 - P1), (P3 - P1)));
        }

        public static Vector3[,] GenerateNormals(int[,] HeightMap)
        {
            Vector3[,] NormalArray = new Vector3[HeightMap.GetLength(0), HeightMap.GetLength(1)];

            for (int i = 0; i < HeightMap.GetLength(0)-1; i++)
            {
                for (int j = 0; j < HeightMap.GetLength(0) - 1; j++)
                {
                    NormalArray[i, j] = GenerateNormal( new Vector3(i,j,HeightMap[i,j]), new Vector3(i+1, j, HeightMap[i+1, j]), new Vector3(i, j+1, HeightMap[i, j+1]));
                }
            }

            for (int i = 0; i < HeightMap.GetLength(0) - 1; i++)
            {
                HeightMap[i, HeightMap.GetLength(0) - 1] = HeightMap[i, HeightMap.GetLength(0) - 2];
            }
            for (int j = 0; j < HeightMap.GetLength(1) - 1; j++)
            {
                HeightMap[HeightMap.GetLength(0) - 1, j] = HeightMap[HeightMap.GetLength(0) - 2, j];
            }

            return NormalArray;
        }

        //NOTE
        //Tangents are "Correct" Do Not Touch
        public static List<Vector3> GenerateTangents(List<Vector3> Vertices, List<Vector2> TexCoords, List<Vector3> Normals)
        {
            List<Vector3> Tangents = new List<Vector3>();
            
            for (int i = 0; i < Vertices.Count; i += 3)
            {
                Vector3 n = Normals[i];

                Vector3 v1 = Vertices[i];
                Vector3 v2 = Vertices[i + 1];
                Vector3 v3 = Vertices[i + 2];

                Vector2 w1 = TexCoords[i];
                Vector2 w2 = TexCoords[i+1];
                Vector2 w3 = TexCoords[i+2];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                
                Vector3 Tangent = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 BiTangent = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

               
                Tangent = Vector3.Normalize(Tangent - n * Vector3.Dot(n, Tangent));
                BiTangent = Vector3.Normalize(BiTangent - n * Vector3.Dot(n, BiTangent));

                BiTangent.X = -BiTangent.X;

                Tangents.Add(BiTangent);
                Tangents.Add(BiTangent);
                Tangents.Add(BiTangent);
                
            }
            
            return Tangents;
        }

        public static Vector3 GenerateTangent(Vector3 P0, Vector3 P1, Vector3 P2, Vector2 T0, Vector2 T1, Vector2 T2)
        {
            Vector3 DeltaPos1 = P1 - P0;
            Vector3 DeltaPos2 = P2 - P0;

            Vector2 DeltaUV1 = T1 - T0;
            Vector2 DeltaUV2 = T2 - T0;
            
            float R = 1.0f / (DeltaUV1.X * DeltaUV2.Y - DeltaUV1.Y * DeltaUV2.X);
            
            Vector3 Tangent = (DeltaPos1 * DeltaUV2.Y - DeltaPos2 * DeltaUV1.Y) * R;

            return Vector3.Normalize( Tangent);
        }

        public static void FillTexture(Texture2D Texture, int[,] Array)
        {
            byte[] Data = new byte[Array.GetLength(0) * Array.GetLength(1) * 4];

            byte[] TempBytes = new byte[4];
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    int Index = ((j * Array.GetLength(0)) + i) * 4;
                    
                    Data[Index] = (byte)(Array[i, j] >> 24);
                    Data[Index + 1] = (byte)(Array[i, j] >> 16);
                    Data[Index + 2] = (byte)(Array[i, j] >> 8);
                    Data[Index + 3] = (byte)(Array[i, j]);
                }
            }
            Texture.SetData(Data);
        }

        public static void FillTexture(Texture2D Texture, short[,] ArrayA, byte[,] ArrayB, byte[,] ArrayC)
        {
            byte[] Data = new byte[ArrayA.GetLength(0) * ArrayA.GetLength(1) * 4];

            for (int i = 0; i < ArrayA.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayA.GetLength(1); j++)
                {
                    int Index = ((j * ArrayA.GetLength(0)) + i) * 4;

                    Data[Index] = (byte)(ArrayA[i, j] >> 8);
                    Data[Index + 1] = (byte)(ArrayA[i, j] & 255);

                    Data[Index + 2] = ArrayB[i, j];
                    Data[Index + 3] = ArrayC[i, j];

                }
            }
            Texture.SetData(Data);
        }

        public static void FillTexture(Texture2D Texture, byte[,] ArrayA)
        {
            byte[] Data = new byte[ArrayA.GetLength(0) * ArrayA.GetLength(1)];

            for (int i = 0; i < ArrayA.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayA.GetLength(1); j++)
                {
                    int Index = ((j * ArrayA.GetLength(0)) + i);

                    Data[Index] = ArrayA[i, j];

                }
            }
            Texture.SetData(Data);
        }
        
        public static void FillTexture(Texture2D Texture, short[,] ArrayA, short[,] ArrayB)
        {
            byte[] Data = new byte[ArrayA.GetLength(0) * ArrayA.GetLength(1) * 4];

            for (int i = 0; i < ArrayA.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayA.GetLength(1); j++)
                {
                    int Index = ((j * ArrayA.GetLength(0)) + i) * 4;

                    Data[Index] = (byte)(ArrayA[i, j] >> 8);
                    Data[Index + 1] = (byte)(ArrayA[i, j] & 255);
                    Data[Index + 2] = (byte)(ArrayB[i, j] >> 8);
                    Data[Index + 3] = (byte)(ArrayB[i, j] & 255);

                }
            }
            Texture.SetData(Data);
        }

        public static void FillTexture(Texture2D Texture, byte[,] ArrayA, byte[,] ArrayB, byte[,] ArrayC, byte[,] ArrayD)
        {
            byte[] Data = new byte[ArrayA.GetLength(0) * ArrayA.GetLength(1) * 4];

            for (int i = 0; i < ArrayA.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayA.GetLength(1); j++)
                {
                    int Index = ((j * ArrayA.GetLength(0)) + i) * 4;

                    Data[Index] = ArrayA[i, j];
                    Data[Index + 1] = ArrayB[i, j];
                    Data[Index + 2] = ArrayC[i, j];
                    Data[Index + 3] = ArrayD[i, j];

                }
            }
            Texture.SetData(Data);
        }

        public static void FillTexture(Texture2D Texture, Vector4[,] Array)
        {
            byte[] Data = new byte[Array.GetLength(0) * Array.GetLength(1) * 4];

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    int Index = ((j * Array.GetLength(0)) + i) * 4;
                    byte[] ByteStoreX = BitConverter.GetBytes(Array[i, j].X);
                    byte[] ByteStoreY = BitConverter.GetBytes(Array[i, j].Y);
                    byte[] ByteStoreZ = BitConverter.GetBytes(Array[i, j].Z);
                    byte[] ByteStoreW = BitConverter.GetBytes(Array[i, j].W);

                    Data[Index] = ByteStoreX[0];
                    Data[Index + 1] = ByteStoreY[0];
                    Data[Index + 2] = ByteStoreZ[0];
                    Data[Index + 3] = ByteStoreW[0];

                }
            }
            Texture.SetData(Data);
        }

        public static void FillTexture(Texture2D Texture, Vector3[,] Array)
        {
            byte[] Data = new byte[Array.GetLength(0) * Array.GetLength(1) * 4];

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    int Index = ((j * Array.GetLength(0)) + i) * 4;
                    byte[] ByteStoreX = BitConverter.GetBytes(Array[i, j].X);
                    byte[] ByteStoreY = BitConverter.GetBytes(Array[i, j].Y);
                    byte[] ByteStoreZ = BitConverter.GetBytes(Array[i, j].Z);

                    Data[Index] = ByteStoreX[0];
                    Data[Index + 1] = ByteStoreY[0];
                    Data[Index + 2] = ByteStoreZ[0];
                    Data[Index + 3] = 0;

                }
            }
            Texture.SetData(Data);
        }

        public static void FillTexture(Texture2D Texture, float[,] Array)
        {
            byte[] Data = new byte[Array.GetLength(0) * Array.GetLength(1) * 4];

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    int Index = ((j * Array.GetLength(0)) + i) * 4;
                    byte[] ByteStore = BitConverter.GetBytes(Array[i, j]);

                    Data[Index] = ByteStore[0];
                    Data[Index + 1] = ByteStore[1];
                    Data[Index + 2] = ByteStore[2];
                    Data[Index + 3] = ByteStore[3];

                }
            }
            Texture.SetData(Data);
        }

        //TODO
        //Add support for all texture formats
        public static Texture2D CloneTexture(GraphicsDevice Graphics, Texture2D Texture)
        {
            Texture2D NewTexture = new Texture2D(Graphics, Texture.Width, Texture.Height, false, Texture.Format);

            byte[] Data = null;
            switch (Texture.Format)
            {
                case SurfaceFormat.Color:
                    Data = new byte[Texture.Width * Texture.Height * 4];
                    break;
                case SurfaceFormat.Single:
                    Data = new byte[Texture.Width * Texture.Height * 4];
                    break;
            }

            Texture.GetData(Data, 0, Data.Length);
            NewTexture.SetData(Data);
            return NewTexture;
        }
        

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, byte[,] ArrayA)
        {
            Texture2D NewTexture = new Texture2D(Graphics, ArrayA.GetLength(0), ArrayA.GetLength(1), false, SurfaceFormat.Alpha8);
            FillTexture(NewTexture, ArrayA);

            return NewTexture;
        }

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, byte[,] ArrayA, byte[,] ArrayB, byte[,] ArrayC, byte[,] ArrayD)
        {
            Texture2D NewTexture = new Texture2D(Graphics, ArrayA.GetLength(0), ArrayA.GetLength(1), false, SurfaceFormat.Color);
            FillTexture(NewTexture, ArrayA, ArrayB, ArrayC, ArrayD);

            return NewTexture;
        }

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, short[,] ArrayA, byte[,] ArrayB, byte[,] ArrayC)
        {
            Texture2D NewTexture = new Texture2D(Graphics, ArrayA.GetLength(0), ArrayA.GetLength(1), false, SurfaceFormat.Color);
            FillTexture(NewTexture, ArrayA, ArrayB, ArrayC);

            return NewTexture;
        }

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, short[,] ArrayA, short[,] ArrayB)
        {
            Texture2D NewTexture = new Texture2D(Graphics, ArrayA.GetLength(0), ArrayA.GetLength(1), false, SurfaceFormat.Color);
            FillTexture(NewTexture, ArrayA, ArrayB);

            return NewTexture;
        }

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, int[,] Array)
        {
            Texture2D NewTexture = new Texture2D(Graphics, Array.GetLength(0), Array.GetLength(1), false, SurfaceFormat.Color);
            FillTexture(NewTexture, Array);
            return NewTexture;
        }
        
        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, float[,] Array)
        {
            Texture2D NewTexture = new Texture2D(Graphics, Array.GetLength(0), Array.GetLength(1), false, SurfaceFormat.Single);
            FillTexture(NewTexture, Array);
            return NewTexture;
        }

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, SurfaceFormat Format, float[,] Array)
        {
            Texture2D NewTexture = new Texture2D(Graphics, Array.GetLength(0), Array.GetLength(1), false, Format);
            FillTexture(NewTexture, Array);
            return NewTexture;
        }

        public static Texture2D MaterialMapsToTexture(GraphicsDevice Graphics, SurfaceFormat Format, int[,] Array)
        {
            Texture2D NewTexture = new Texture2D(Graphics, Array.GetLength(0), Array.GetLength(1), false, Format);
            FillTexture(NewTexture, Array);
            return NewTexture;
        }
        
        public static Vector2 NormUV(Vector3 Vec, Vector3 Pos, bool rot)
        {
            Vector2 UV = new Vector2(Pos.X, Pos.Y);

            bool PosX = Pos.X > 0;
            bool PosY = Pos.Y > 0;
            bool PosZ = Pos.Z > 0;

            float absX = Math.Abs(Vec.X);
            float absY = Math.Abs(Vec.Y);
            float absZ = Math.Abs(Vec.Z);

            if (PosX && absX >= absY && absX >= absZ)
            {
                if (rot)
                {
                    UV = new Vector2(Pos.Z, Pos.Y);
                }
                else
                {
                    UV = new Vector2(Pos.Y, Pos.Z);
                }
            }
            else if (!PosX && absX >= absY && absX >= absZ)
            {
                if (rot)
                {
                    UV = new Vector2(Pos.Z, Pos.Y);
                }
                else
                {
                    UV = new Vector2(Pos.Y, Pos.Z);
                }
            }
            else if (PosY && absY >= absX && absY >= absZ)
            {
                if (rot)
                {
                    UV = new Vector2(Pos.X, Pos.Z);
                }
                else
                {
                    UV = new Vector2(Pos.Z, Pos.X);
                }
            }
            else if (!PosY && absY >= absX && absY >= absZ)
            {
                if (rot)
                {
                    UV = new Vector2(Pos.X, Pos.Z);
                }
                else
                {
                    UV = new Vector2(Pos.Z, Pos.X);
                }
            }
            else if (PosZ && absZ >= absX && absZ >= absY)
            {
                if (rot)
                {
                    UV = new Vector2(Pos.X, Pos.Y);
                }
                else
                {
                    UV = new Vector2(Pos.Y, Pos.X);
                }
            }
            else if (!PosZ && absZ >= absX && absZ >= absY)
            {
                if (rot)
                {
                    UV = new Vector2(Pos.X, Pos.Y);
                }
                else
                {
                }
            }
            else
            {
                if (rot)
                {
                    UV = new Vector2(Pos.X, Pos.Z);
                }
                else
                {
                    UV = new Vector2(Pos.Z, Pos.X);
                }
            }

            return UV;
        }

    }
}
