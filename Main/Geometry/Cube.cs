using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Main.Main.Geometry3D;

namespace Main.StructureEditor
{
    public class Cube
    {
        public struct CubeVertex
        {
            private Vector3 Position;
            private Vector2 TexCoord;

            public CubeVertex(Vector3 Position, Vector2 TexCoord)
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

        public Vector3 Position;
        public Vector3 Size;
        public Vector3 Buffer = new Vector3(0.05f, 0.05f, 0.05f);
        public Geometry3D Geom { get; set; }
        
        public string CubeColor { get; set; } = "red";

        public Cube(Vector3 Pos, Vector3 Scale)
        {
            Position = Pos;
            Size = Scale;
        }

        public void UpdateVertexBuffer(GraphicsDevice Graphics, Vector3 Scale)
        {
            int[] Indices = new int[] { 2, 0, 1, 1, 3, 2, 7, 5, 4, 4, 6, 7, 6, 4, 0, 0, 2, 6, 3, 1, 5, 5, 7, 3, 6, 2, 3, 3, 7, 6, 0, 4, 5, 5, 1, 0 };

            Vector3[] LineLookup = new Vector3[8]
            {
                //X Axis
                new Vector3(-1.0f,-1.0f,-1.0f) * (Size + Buffer),
                new Vector3(1.0f,-1.0f,-1.0f) * (Size + Buffer),
                new Vector3(-1.0f,-1.0f,1.0f) * (Size + Buffer),
                new Vector3(1.0f,-1.0f,1.0f) * (Size + Buffer),

                new Vector3(-1.0f,1.0f,-1.0f) * (Size + Buffer),
                new Vector3(1.0f,1.0f,-1.0f) * (Size + Buffer),
                new Vector3(-1.0f,1.0f,1.0f) * (Size + Buffer),
                new Vector3(1.0f,1.0f,1.0f) * (Size + Buffer)
            };
            
            List<VertexPositionTexture> VertexList = new List<VertexPositionTexture>();
            Vector2 Color = new Vector2();

            switch (CubeColor)
            {
                case "red":
                    Color = new Vector2(0.1875f, 0.1875f);
                    break;
                case "green":
                    Color = new Vector2(0.1875f, 0.3125f);
                    break;
                case "blue":
                    Color = new Vector2(0.3125f, 0.1875f);
                    break;
                case "gray":
                    Color = new Vector2(0.3125f, 0.3125f);
                    break;
                default :
                    Color = new Vector2(0.8125f, 0.8125f);
                    break;
            }
            
            for (int i = Indices.Length - 1; i >= 0; i--)
            {
                VertexList.Add(new VertexPositionTexture(LineLookup[Indices[i]], Color));
            }
            //Geom.SetVertexBuffer(VertexList.ToArray());

            VertexBuffer VertexBuffer = new VertexBuffer(Graphics, CubeVertex.VertexDeclaration, VertexList.Count, BufferUsage.None);
            VertexBuffer.SetData(VertexList.ToArray());
            Geom.SetVertexBuffer(VertexBuffer);

        }

        public void SetPosition(Vector3 NewPosition)
        {
            Position = NewPosition;
            Geom.Position = Position;
        }
        
        public void GenerateLookatCube(GraphicsDevice Graphics)
        {
            int[] Indices = new int[] { 2, 0, 1, 1, 3, 2, 7, 5, 4, 4, 6, 7, 6, 4, 0, 0, 2, 6, 3, 1, 5, 5, 7, 3, 6, 2, 3, 3, 7, 6, 0, 4, 5, 5, 1, 0 };

            Vector3[] LineLookup = new Vector3[8]
            {
                //X Axis
                new Vector3(-1.0f,-1.0f,-1.0f) * (Size + Buffer),
                new Vector3(1.0f,-1.0f,-1.0f) * (Size + Buffer),
                new Vector3(-1.0f,-1.0f,1.0f) * (Size + Buffer),
                new Vector3(1.0f,-1.0f,1.0f) *(Size + Buffer),

                new Vector3(-1.0f,1.0f,-1.0f) * (Size + Buffer),
                new Vector3(1.0f,1.0f,-1.0f) * (Size + Buffer),
                new Vector3(-1.0f,1.0f,1.0f) *(Size + Buffer),
                new Vector3(1.0f,1.0f,1.0f) * (Size + Buffer)
            };

            Geom = new Geometry3D("LookatCube");
            Geom.SetTextures(new string[] { "GridColors"});
            BasicEffect BasicEffect = new BasicEffect(Graphics);
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            Geom.Position = Position;
            Geom.HasCull = false;
            Geom.RenderBucket = RenderQueue.Solid;

            List<VertexPositionTexture> VertexList = new List<VertexPositionTexture>();
            Vector2 Color = new Vector2();

            switch (CubeColor)
            {
                case "red":
                    Color = new Vector2(0.1875f, 0.1875f);
                    break;
                case "green":
                    Color = new Vector2(0.1875f, 0.3125f);
                    break;
                case "blue":
                    Color = new Vector2(0.3125f, 0.1875f);
                    break;
                case "gray":
                    Color = new Vector2(0.3125f, 0.3125f);
                    break;
                default:
                    Color = new Vector2(0.8125f, 0.8125f);
                    break;
            }

            for (int i = Indices.Length - 1; i >= 0; i--)
            {
                VertexList.Add(new VertexPositionTexture(LineLookup[Indices[i]], Color));
            }

            VertexBuffer VertexBuffer = new VertexBuffer(Graphics, CubeVertex.VertexDeclaration, VertexList.Count, BufferUsage.WriteOnly);
            VertexBuffer.SetData(VertexList.ToArray());
            Geom.SetVertexBuffer(VertexBuffer);

        }
    }
}
