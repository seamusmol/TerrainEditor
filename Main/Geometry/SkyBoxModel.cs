using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Geometry
{
    public class SkyBoxModel
    {
        public struct SkyBoxVertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;

            public SkyBoxVertex(Vector3 Position, Vector2 TexCoord)
            {
                this.Position = Position;
                this.TexCoord = TexCoord;
            }

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );
        }
        

        public Geometry3D Geom { get; private set; }

        Vector3 Position;
        string Name;
        float SkyBoxDistance = 10.0f;

        public SkyBoxModel(RenderManager Render, int Distance, string SkyBoxName)
        {
            Name = SkyBoxName;
            SkyBoxDistance = Distance;
            GenerateGeometry(Render);
        }

        public void Update(Vector3 NewPosition)
        {
            Position = NewPosition;
            Geom.Shader.Parameters["WorldPosition"].SetValue(Position);
        }

        public void GenerateGeometry(RenderManager Render)
        {

            Vector2[] tex = new Vector2[14]
            {
                new Vector2(0.25f, 0.001f),
                new Vector2(0.5f, 0.001f),
                new Vector2(0, 0.3333f),
                new Vector2(0.25f, 0.3333f),

                new Vector2(0.5f, 0.3333f),
                new Vector2(0.75f, 0.3333f),
                new Vector2(0.999f, 0.3333f),
                new Vector2(0, 0.6666f),

                new Vector2(0.25f, 0.6666f),
                new Vector2(0.5f, 0.6666f),
                new Vector2(0.75f, 0.6666f),
                new Vector2(0.9999f, 0.6666f),

                new Vector2(0.5f, 0.9999f),
                new Vector2(0.5f, 0.9999f),
            };

            Vector3[] pos = new Vector3[8]
            {
                new Vector3(-SkyBoxDistance, -SkyBoxDistance, -SkyBoxDistance),
                new Vector3(-SkyBoxDistance, SkyBoxDistance, -SkyBoxDistance),
                new Vector3(SkyBoxDistance, -SkyBoxDistance, -SkyBoxDistance),
                new Vector3(SkyBoxDistance, SkyBoxDistance, -SkyBoxDistance),

                new Vector3(-SkyBoxDistance, -SkyBoxDistance, SkyBoxDistance),
                new Vector3(-SkyBoxDistance, SkyBoxDistance, SkyBoxDistance),
                new Vector3(SkyBoxDistance, -SkyBoxDistance, SkyBoxDistance),
                new Vector3(SkyBoxDistance, SkyBoxDistance, SkyBoxDistance)
            };

            SkyBoxVertex[] Vertices = new SkyBoxVertex[36]
            {
                //bottom
                new SkyBoxVertex(pos[1], tex[8]),
                new SkyBoxVertex(pos[3], tex[9]),
                new SkyBoxVertex(pos[2], tex[13]),
                new SkyBoxVertex(pos[2], tex[13]),
                new SkyBoxVertex(pos[0], tex[12]),
                new SkyBoxVertex(pos[1], tex[8]),
                //east
                new SkyBoxVertex(pos[3], tex[9]),
                new SkyBoxVertex(pos[7], tex[4]),
                new SkyBoxVertex(pos[6], tex[5]),
                new SkyBoxVertex(pos[6], tex[5]),
                new SkyBoxVertex(pos[2], tex[10]),
                new SkyBoxVertex(pos[3], tex[9]),
                //top
                new SkyBoxVertex(pos[7], tex[4]),
                new SkyBoxVertex(pos[5], tex[3]),
                new SkyBoxVertex(pos[4], tex[0]),
                new SkyBoxVertex(pos[4], tex[0]),
                new SkyBoxVertex(pos[6], tex[1]),
                new SkyBoxVertex(pos[7], tex[4]),
                //west
                new SkyBoxVertex(pos[5], tex[3]),
                new SkyBoxVertex(pos[1], tex[8]),
                new SkyBoxVertex(pos[0], tex[7]),
                new SkyBoxVertex(pos[0], tex[7]),
                new SkyBoxVertex(pos[4], tex[2]),
                new SkyBoxVertex(pos[5], tex[3]),
                //north
                new SkyBoxVertex(pos[3], tex[9]),
                new SkyBoxVertex(pos[1], tex[8]),
                new SkyBoxVertex(pos[5], tex[3]),
                new SkyBoxVertex(pos[5], tex[3]),
                new SkyBoxVertex(pos[7], tex[4]),
                new SkyBoxVertex(pos[3], tex[9]),
                //south
                new SkyBoxVertex(pos[0], tex[11]),
                new SkyBoxVertex(pos[2], tex[10]),
                new SkyBoxVertex(pos[6], tex[5]),
                new SkyBoxVertex(pos[6], tex[5]),
                new SkyBoxVertex(pos[4], tex[6]),
                new SkyBoxVertex(pos[0], tex[11])
            };
            
            //Geom.SetVertexBuffer(VertexList.ToArray());
            Geom = new Geometry3D("LookatCube");
            Geom.Shader = Render.Shaders["SkyBox"].Clone();

            VertexBuffer VertexBuffer = new VertexBuffer(Render.Graphics, SkyBoxVertex.VertexDeclaration, Vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(Vertices);
            Geom.SetVertexBuffer(VertexBuffer);

            Geom.HasCull = false;
            Geom.Position = new Vector3();
            Geom.RenderBucket = Geometry3D.RenderQueue.Solid;
            Geom.Shader.Parameters["DiffuseMap"].SetValue( Render.Textures["SkyMap"]);
        }
        
    }
}
