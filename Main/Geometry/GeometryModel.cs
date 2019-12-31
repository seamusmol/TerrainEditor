using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Main.Geometry
{
    /**
     * Stores Data used for Rendering Models
     * */
    public class GeometryModel : ICloneable
    {
        enum ModelType
        {
            OBJ = 0
        }

        public struct ModelInstanceVertex
        {
            public Vector3 Position { get; set; }
            public Vector3 Forward { get; set; }
            public Vector3 Up { get; set; }
            public Vector3 Left { get; set; }
            public Vector3 Scale { get; set; }

            public byte Data0 { get; set; }
            public byte Data1 { get; set; }
            public byte Data2 { get; set; }
            public byte Data3 { get; set; }

            public ModelInstanceVertex(Vector3 Position, Vector3 Forward, Vector3 Up, Vector3 Left, Vector3 Scale, byte TBD0, byte TBD1, byte TBD2, byte TBD3)
            {
                this.Position = Position;
                this.Forward = Forward;
                this.Up = Up;
                this.Left = Left;
                this.Scale = Scale;
                this.Data0 = TBD0;
                this.Data1 = TBD1;
                this.Data2 = TBD2;
                this.Data3 = TBD3;
            }

            public readonly static VertexDeclaration ModelInstanceVertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 1),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Normal, 2),
                new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Normal, 3),
                new VertexElement(48, VertexElementFormat.Vector3, VertexElementUsage.Normal, 4),
                new VertexElement(60, VertexElementFormat.Byte4, VertexElementUsage.Tangent, 0)

            );
        }
        
        public struct ModelVertex
        {
            public Vector3 Position { get; set; }
            public Vector2 TexCoord { get; set; }
            public Vector3 Normal { get; set; }
            
            public ModelVertex(Vector3 Position, Vector2 TexCoord, Vector3 Normal)
            {
                this.Position = Position;
                this.TexCoord = TexCoord;
                this.Normal = Normal;
            }

            public readonly static VertexDeclaration ModelVertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal, 5)
            );
        }

        public List<Vector3> VertexList { get; set; } = new List<Vector3>();
        public List<Vector2> TexcoordList { get; set; } = new List<Vector2>();
        public List<Vector3> NormalList { get; set; } = new List<Vector3>();

        public Dictionary<string, string> Textures = new Dictionary<string, string>();

        public Effect Shader { get; set; }
        public Effect DepthShader { get; set; }

        public string Name { get; set; }

        public GeometryModel()
        {

        }

        public GeometryModel(string ModelName)
        {
            Name = ModelName;
        }

        public GeometryModel(string ModelName,List<Vector3> Vertices, List<Vector2> Texcoords, List<Vector3> Normals, Dictionary<string, string> ModelTextures, Effect ModelMainShader, Effect ModelDepthShader)
        {
            Name = ModelName;
            VertexList = Vertices;
            TexcoordList = Texcoords;
            NormalList = Normals;
            Textures = ModelTextures;
            Shader = ModelMainShader;
            DepthShader = ModelDepthShader;
        }
        
        public VertexBuffer GetBufferData(GraphicsDevice Graphics)
        {
            VertexBuffer VertexBuffer = new VertexBuffer(Graphics, ModelVertex.ModelVertexDeclaration, VertexList.Count, BufferUsage.None);
            
            List<ModelVertex> ModelVertexData = new List<ModelVertex>();
            for (int i = 0; i < VertexList.Count; i++)
            {

                ModelVertexData.Add(new ModelVertex(VertexList[i], TexcoordList[i], NormalList[i]));
            }
            VertexBuffer.SetData(ModelVertexData.ToArray());
            return VertexBuffer;
        }
        
        public void AddTexture(string Key, string TextureName)
        {
            if (!Textures.ContainsKey(Key))
            {
                Textures.Add(Key, TextureName);
            }
        }

        public void RemoveTexture(string Key)
        {
            if (Textures.ContainsKey(Key))
            {
                Textures.Remove(Key);
            }
        }

        public void Normalize()
        {
            AlingToOrigin();
            float[] Bounds = GetBounds();
            
            Vector3 MaxBound = new Vector3(Bounds[3], Bounds[4], Bounds[5]);

            for (int i = 0; i < VertexList.Count; i++)
            {
                VertexList[i] /= MaxBound;
            }
        }

        public void AlingToOrigin()
        {
            float[] Bounds = GetBounds();
            Vector3 BoundOffset = new Vector3( -Bounds[0], -Bounds[1], -Bounds[2]);

            for (int i = 0; i < VertexList.Count; i++)
            {
                VertexList[i] += BoundOffset;
            }
        }

        public float[] GetBounds()
        {
            if (VertexList.Count == 0)
            {
                return new float[] { 0, 0, 0, 0 };
            }
            float MinX = VertexList[0].X;
            float MaxX = VertexList[0].X;
            
            float MinY = VertexList[0].Y;
            float MaxY = VertexList[0].Y;
            
            float MinZ = VertexList[0].Z;
            float MaxZ = VertexList[0].Z;

            for (int i = 0; i < VertexList.Count; i++)
            {
                MinX = VertexList[i].X < MinX ? VertexList[i].X : MinX;
                MinY = VertexList[i].Y < MinY ? VertexList[i].Y : MinY;
                MinZ = VertexList[i].Z < MinZ ? VertexList[i].Z : MinZ;

                MaxX = VertexList[i].X > MinX ? VertexList[i].X : MinX;
                MaxY = VertexList[i].Y > MinY ? VertexList[i].Y : MinY;
                MaxZ = VertexList[i].Z > MinZ ? VertexList[i].Z : MinZ;
            }
            
            return new float[] { MinX, MinY, MinZ, MaxX, MaxY, MaxZ };
        }

        public virtual object Clone()
        {
            GeometryModel NewModel = new GeometryModel(this.Name);
            
            List<Vector3> NewVertexList = new List<Vector3>();
            for (int i = 0; i < VertexList.Count; i++)
            {
                NewVertexList.Add( new Vector3(VertexList[i].X, VertexList[i].Y, VertexList[i].Z));
            }
            List<Vector2> NewTexcoordList = new List<Vector2>();
            for (int i = 0; i < TexcoordList.Count; i++)
            {
                NewTexcoordList.Add( new Vector2(TexcoordList[i].X, TexcoordList[i].Y));
            }
            List<Vector3> NewNormalList= new List<Vector3>();
            for (int i = 0; i < NormalList.Count; i++)
            {
                NewNormalList.Add(new Vector3(NormalList[i].X, NormalList[i].Y, NormalList[i].Z));
            }
            Dictionary<string, string> NewTextureReferences = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in Textures)
            {
                NewTextureReferences.Add(entry.Key, entry.Value);
            }
            
            NewModel.Name = (string)Name.Clone();
            NewModel.VertexList = NewVertexList;
            NewModel.TexcoordList = NewTexcoordList;
            NewModel.NormalList = NewNormalList;

            NewModel.Textures = NewTextureReferences;
            NewModel.Shader = Shader.Clone();
            NewModel.DepthShader = DepthShader.Clone();
            return NewModel;
        }

    }
}
