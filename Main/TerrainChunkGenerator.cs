using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldManager
{
    class TerrainChunkGenerator
    {
        public static TerrainChunk GenerateChunk(WorldMap Map, int IDX, int IDY, int ChunkSize)
        {
            int[,] HeightMap = new int[ChunkSize+1, ChunkSize+1];
            int[,] MaterialMap = new int[ChunkSize, ChunkSize];
            
            for (int i = 0; i < ChunkSize; i++)
            {
                for (int j = 0; j < ChunkSize; j++)
                {
                    MaterialMap[i,j] = Map.MaterialMap[IDX * ChunkSize + i, IDY * ChunkSize + j];
                }
            }
            
            for (int i = 0; i < ChunkSize+1; i++)
            {
                for (int j = 0; j < ChunkSize+1; j++)
                {
                    HeightMap[i, j] = Map.HeightMap[IDX * ChunkSize + i, IDY * ChunkSize + j];
                }
            }

            return new TerrainChunk(IDX,IDY,HeightMap,MaterialMap);
        }
        
        public static VertexPositionTexture[] GenerateHeightMapGeometry(int ChunkSize, int[,] MaterialMap, int[,] HeightMap, float HeightPrecision)
        {
            List<VertexPositionTexture> Vertices = new List<VertexPositionTexture>();
            
            float TexSize = 0.0625f;

            for (int i = 0; i < MaterialMap.GetLength(0); i++)
            {
                for (int j = 0; j < MaterialMap.GetLength(1); j++)
                {
                    float TX = 1.0f / (MaterialMap[i, j] / 16);
                    float TY = 1.0f / (MaterialMap[i, j] % 16);

                    TX = float.IsInfinity(TX) ? 0 : TX;
                    TY = float.IsInfinity(TY) ? 0 : TY;
                    
                    Vertices.Add(new VertexPositionTexture(new Vector3(i,j, HeightMap[i, j] * HeightPrecision), new Vector2(TX, TY)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i,j+1, HeightMap[i, j+1] * HeightPrecision), new Vector2(TX, TY + TexSize)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i + 1,j+1, HeightMap[i+1, j+1] * HeightPrecision), new Vector2(TX + TexSize, TY + TexSize)));

                    Vertices.Add(new VertexPositionTexture(new Vector3(i + 1,j+1, HeightMap[i+1, j+1] * HeightPrecision), new Vector2(TX + TexSize, TY + TexSize)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i + 1,j, HeightMap[i+1, j] * HeightPrecision), new Vector2(TX + TexSize, TY)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i,j, HeightMap[i, j] * HeightPrecision), new Vector2(TX, TY)));
                }
            }
            return Vertices.ToArray();
        }

        
        public static VertexPositionTexture[] GenerateHeightMapGeometry(Vector3 Position, int ChunkSize, int[,] MaterialMap, int[,] HeightMap, float HeightPrecision)
        {
            List<VertexPositionTexture> Vertices = new List<VertexPositionTexture>();

            float TexSize = 0.0625f;

            for (int i = 0; i < MaterialMap.GetLength(0); i++)
            {
                for (int j = 0; j < MaterialMap.GetLength(1); j++)
                {
                    float TX = 1.0f / (MaterialMap[i, j] / 16);
                    float TY = 1.0f / (MaterialMap[i, j] % 16);

                    TX = float.IsInfinity(TX) ? 0 : TX;
                    TY = float.IsInfinity(TY) ? 0 : TY;
                    
                    Vertices.Add(new VertexPositionTexture(new Vector3(i, j, HeightMap[i, j] * HeightPrecision) + Position, new Vector2(TX, TY)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i, j + 1, HeightMap[i, j + 1] * HeightPrecision) + Position, new Vector2(TX, TY + TexSize)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i + 1, j + 1, HeightMap[i + 1, j + 1] * HeightPrecision) + Position, new Vector2(TX + TexSize, TY + TexSize)));

                    Vertices.Add(new VertexPositionTexture(new Vector3(i + 1, j + 1, HeightMap[i + 1, j + 1] * HeightPrecision) + Position, new Vector2(TX + TexSize, TY + TexSize)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i + 1, j, HeightMap[i + 1, j] * HeightPrecision) + Position, new Vector2(TX + TexSize, TY)));
                    Vertices.Add(new VertexPositionTexture(new Vector3(i, j, HeightMap[i, j] * HeightPrecision) + Position, new Vector2(TX, TY)));
                }
            }
            return Vertices.ToArray();
        }
    
    }
}
