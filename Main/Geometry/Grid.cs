using Main.Geometry;
using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.StructureEditor
{
    public class Grid
    {
        public struct GridVertex
        {
            private Vector3 Position;
            private Vector2 TexCoord;

            public GridVertex(Vector3 Position, Vector2 TexCoord)
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

        public float SX { get; set; } = 0;
        public float SY { get; set; } = 0;
        public float SZ { get; set; } = 0;
        public float Scale = 0.25f;
        float Thickness = 0.05f;

        public Vector3 Position { get; private set; } = new Vector3();
        public Geometry3D Geom { get; set; }

        public Grid(int NSX, int NSY, int NSZ, float S, float T)
        {
            SX = NSX;
            SY = NSY;
            SZ = NSZ;
            Scale = S;
            Thickness = T;
        }

        public void ToggleVisibility(bool Val)
        {
            if (Geom != null)
            {
                Geom.HasCull = !Val;
            }
        }

        public void SetPosition(GraphicsDevice Graphics, Vector3 NewPosition)
        {
            Position = NewPosition;
            GenerateGrid(Graphics);
        }

        public void ChangeGridSize(float NSX, float NSY, float NScale, GraphicsDevice Graphics)
        {
            SX = NSX;
            SY = NSY;
            Scale = NScale;
            GenerateGrid(Graphics);
        }

        /*         v6---------------v7
                   /|               /|
                  / |              / |
                 /  |             /  |
               v2----------------v3  |
                |   |            |   |
                |  v4----------- |---v5
                |  /             |  /
                | /              | /
                |/               |/
                v0---------------v1
                
        */

        public void GenerateGeometry(GraphicsDevice Graphics)
        {
            Geom = new Geometry3D("GridGeom");
            //Geom.SetVertexBuffer( new VertexPositionTexture[0]);

            Geom.SetTextures(new string[] { "GridColors" });

            BasicEffect BasicEffect = new BasicEffect(Graphics);
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            Geom.HasCull = false;
            Geom.RenderBucket = Geometry3D.RenderQueue.Solid;
        }

        public void GenerateGrid(GraphicsDevice Graphics)
        {
            List<Vector3> Positions = new List<Vector3>();
            List<Color> ColorValues = new List<Color>();

            //draw axis lines

            Vector2 Red = new Vector2(0.1875f, 0.1875f);
            Vector2 Green = new Vector2(0.1875f, 0.375f);
            Vector2 Blue = new Vector2(0.375f, 0.1875f);
            Vector2 Gray = new Vector2(0.375f, 0.375f);

            List<GridVertex> GridVertices = new List<GridVertex>();

            //front,back,left,right,top,bottom
            int[] Indices = new int[] { 2, 0, 1, 1, 3, 2, 7, 5, 4, 4, 6, 7, 6, 4, 0, 0, 2, 6, 3, 1, 5, 5, 7, 3, 6, 2, 3, 3, 7, 6, 0, 4, 5, 5, 1, 0 };

            Vector3[] LineLookup = new Vector3[24]
            {
                //X Axis
                new Vector3(0,0,0),
                new Vector3(SX,0,0),
                new Vector3(0,0,Thickness),
                new Vector3(SX,0,Thickness),

                new Vector3(0,Thickness,0),
                new Vector3(SX,Thickness,0),
                new Vector3(0,Thickness,Thickness),
                new Vector3(SX,Thickness,Thickness),
                //Y Axis
                new Vector3(0,0,0),
                new Vector3(Thickness,0,0),
                new Vector3(0,0,Thickness),
                new Vector3(Thickness,0,Thickness),

                new Vector3(0,SY,0),
                new Vector3(Thickness,SY,0),
                new Vector3(0,SY,Thickness),
                new Vector3(Thickness,SY,Thickness),
                //Z Axis
                new Vector3(0,0,0),
                new Vector3(Thickness,0,0),
                new Vector3(0,0,SZ),
                new Vector3(Thickness,0,SZ),

                new Vector3(0,Thickness,0),
                new Vector3(Thickness,Thickness,0),
                new Vector3(0,Thickness, SZ),
                new Vector3(Thickness,Thickness,SZ)
            };

            //X Axis
            for (int i = 0; i < Indices.Length; i++)
            {
                GridVertices.Add(new GridVertex(LineLookup[Indices[i]] + Position, Red));
            }
            //Y Axis
            for (int i = 0; i < Indices.Length; i++)
            {
                GridVertices.Add(new GridVertex(LineLookup[Indices[i] + 8] + Position, Green));
            }
            //Z Axis
            for (int i = 0; i < Indices.Length; i++)
            {
                GridVertices.Add(new GridVertex(LineLookup[Indices[i] + 16] + Position, Blue));
            }

            for (float i = Scale; i < SY; i += Scale)
            {
                Vector3 Offset = new Vector3(0 + Thickness, i, 0);
                for (int index = 0; index < Indices.Length; index++)
                {
                    GridVertices.Add(new GridVertex(LineLookup[Indices[index]] + Offset + Position, Gray));
                }
            }

            for (float i = Scale; i < SX; i += Scale)
            {
                Vector3 Offset = new Vector3(i, 0 + Thickness, 0);
                for (int index = 0; index < Indices.Length; index++)
                {
                    GridVertices.Add(new GridVertex(LineLookup[Indices[index] + 8] + Offset + Position, Gray));
                }
            }
            GridVertices.Reverse();

            VertexBuffer Buffer = new VertexBuffer(Graphics, GridVertex.VertexDeclaration, GridVertices.Count, BufferUsage.WriteOnly);
            Buffer.SetData(GridVertices.ToArray());
            Geom.SetVertexBuffer(Buffer);
        }

    }
}
