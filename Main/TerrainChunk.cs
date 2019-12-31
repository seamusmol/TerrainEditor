using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldManager
{
    class TerrainChunk
    {
        public int[,] HeightMap; // n+1
        public int[,] MaterialMap; // n

        public Geometry3D Geom { get; set; }

        public int IDX { get; set; }
        public int IDY { get; set; }
        public int ChunkSize { get; set; }

        public float Scale = 1.0f;
        public float MaterialScale = 1.0f;
        public float HeightScale = 0.25f;

        public TerrainChunk(int PX, int PY, int[,] Height, int[,] Material)
        {
            IDX = PX;
            IDY = PY;
            HeightMap = Height;
            MaterialMap = Material;
            ChunkSize = HeightMap.GetLength(0) - 1;
        }
        
        public void GenerateGeometry(GraphicsDevice Graphics)
        {
            Geom = new Geometry3D(IDX + "-" + IDY);
            //Geom.SetVertexBuffer( TerrainChunkGenerator.GenerateHeightMapGeometry(new Vector3(IDX * ChunkSize, IDY * ChunkSize, 0), ChunkSize, MaterialMap, HeightMap, HeightScale));
            Geom.SetTextures(new string[] { "Test"});
            BasicEffect BasicEffect = new BasicEffect(Graphics);
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            Geom.LocalPosition = new Vector3();
            Geom.HasCull = false;
        }
        
        public void UpdateShader()
        {
          

        }
        

    }
}
