using Main.Geometry;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Main
{
    public class Geometry3D : ICloneable
    {

        public enum RenderMode
        {
            NonIndexed = 0, Indexed = 1, Instanced = 2, NonIndexBufferless = 3
        }

        public enum RenderQueue
        {
            Solid = 0, Transparent = 1
        }
        
        public Effect Shader { get; set; } = null;
        public Effect DepthShader { get; set; } = null;
        public Vector3[] CollisionBuffer { get; private set; } = new Vector3[0];
        public List<Matrix> InstanceCollisionBuffer { get; private set; } = new List<Matrix>();

        public IndexBuffer IndexBuffer { get; private set; }
        public VertexBuffer VertexBuffer { get; private set; }
        public VertexBuffer InstanceBuffer { get; private set; }
        
        public int InstanceCount { get; set; } = 0;
        public int VertexCount = 0;

        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Rotation { get; set; } = new Vector3();

        public List<string> TextureNames { get; private set; } = new List<string>();

        public RenderQueue RenderBucket { get; set; } = RenderQueue.Solid;

        public int PassSetting { get; set; } = -1;
        public int PassMipSetting { get; set; } = -1;

        public RenderMode CurrentRenderMode { get; set; } = RenderMode.NonIndexed;

        public string Name { get; private set; }
        public bool HasCull { get; set; } = false;
        public bool HasTextureUpdate { get; set; } = true;
        
        public Geometry3D(string GeomName)
        {
            Name = GeomName;
        }
        
        public virtual void RenderDepth(GraphicsDevice Graphics, Matrix ViewMatrix, Matrix ProjectionMatrix, Texture2D ClipPlaneMap, Vector3 DepthMapPosition, Vector4 DepthMapRange, float ClipPlaneDirection, float ClipOffset, float FarPlane, bool IsOrthogonal)
        {
            if (DepthShader == null || HasCull)
            {
                return;
            }

            bool HasSet = false;
            
            if (DepthShader.Parameters["ClipPlaneMap"] != null && Shader.Parameters["ClipDirection"] != null && ClipPlaneMap != null)
            {
                DepthShader.Parameters["ClipMapPosition"].SetValue(DepthMapPosition);
                DepthShader.Parameters["ClipMapRange"].SetValue(DepthMapRange);
                
                DepthShader.Parameters["ClipPlaneMap"].SetValue(ClipPlaneMap);
                DepthShader.Parameters["ClipDirection"].SetValue(ClipPlaneDirection);
                DepthShader.Parameters["ClipOffset"].SetValue(ClipOffset);
                
                HasSet = true;
            }
            
            DepthShader.Parameters["IsOrthogonal"].SetValue(IsOrthogonal ? 1 : 0);
            DepthShader.Parameters["WorldViewProjection"].SetValue(ViewMatrix * ProjectionMatrix);
            DepthShader.Parameters["FarPlane"].SetValue(FarPlane);

            if (CurrentRenderMode == RenderMode.Indexed)
            {

            }
            else if (CurrentRenderMode == RenderMode.NonIndexed)
            {
                Graphics.SetVertexBuffer(VertexBuffer);
                DepthShader.CurrentTechnique.Passes[0].Apply();
                Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexBuffer.VertexCount / 3);
            }
            else if (CurrentRenderMode == RenderMode.NonIndexBufferless)
            {
                //Graphics.SetVertexBuffer(VertexBuffer);
                DepthShader.CurrentTechnique.Passes[0].Apply();
                Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexCount / 3);
            }
            else if (CurrentRenderMode == RenderMode.Instanced)
            {
                if (InstanceCount > 0)
                {
                    Graphics.Indices = IndexBuffer;

                    Graphics.SetVertexBuffers(new VertexBufferBinding(VertexBuffer, 0, 0), new VertexBufferBinding(InstanceBuffer, 0, 1));
                    
                    DepthShader.CurrentTechnique.Passes[0].Apply();
                    Graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount / 3, InstanceBuffer.VertexCount);

                    if (IsOrthogonal)
                    {
                    }
                }
            }
          
            DepthShader.Parameters["IsOrthogonal"].SetValue(0);

            if (HasSet)
            {
                DepthShader.Parameters["ClipDirection"].SetValue(0.0f);
                DepthShader.Parameters["ClipOffset"].SetValue(0.0f);
            }
        }

        public virtual void RenderFirstPassColor(GraphicsDevice Graphics, Matrix ViewMatrix, Matrix ProjectionMatrix, Texture2D ClipPlaneMap, Vector3 DepthMapPosition, Vector4 DepthMapRange, float ClipPlaneDirection, float ClipOffset, float FarPlane)
        {
            if (Shader == null || HasCull)
            {
                return;
            }

            bool HasSet = false;
            
            if (Shader.Parameters["ClipPlaneMap"] != null && Shader.Parameters["ClipDirection"] != null && ClipPlaneMap != null)
            {
                Shader.Parameters["ClipMapPosition"].SetValue(DepthMapPosition);
                Shader.Parameters["ClipMapRange"].SetValue(DepthMapRange);
                
                Shader.Parameters["ClipPlaneMap"].SetValue(ClipPlaneMap);
                Shader.Parameters["ClipDirection"].SetValue(ClipPlaneDirection);
                Shader.Parameters["ClipOffset"].SetValue(ClipOffset);
                HasSet = true;
            }

            if (Shader.GetType() == typeof(BasicEffect))
            {
                BasicEffect CastShader = (BasicEffect)Shader;
                CastShader.Projection = ProjectionMatrix;
                CastShader.View = ViewMatrix;
                CastShader.World = Matrix.CreateTranslation(Position);
            }
            else
            {
                Shader.Parameters["WorldViewProjection"].SetValue(ViewMatrix * ProjectionMatrix);
            }

            if (CurrentRenderMode == RenderMode.Indexed)
            {

            }
            else if (CurrentRenderMode == RenderMode.NonIndexed)
            {
                Graphics.SetVertexBuffer(VertexBuffer);
                Shader.CurrentTechnique.Passes[0].Apply();
                Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexBuffer.VertexCount / 3);
            }
            else if (CurrentRenderMode == RenderMode.NonIndexBufferless)
            {
                //Graphics.SetVertexBuffer(VertexBuffer);
                Shader.CurrentTechnique.Passes[0].Apply();
                Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexCount / 3);
            }
            else if (CurrentRenderMode == RenderMode.Instanced)
            {
                if (InstanceCount > 0)
                {
                    Graphics.Indices = IndexBuffer;
                    
                    Graphics.SetVertexBuffers(new VertexBufferBinding(VertexBuffer, 0, 0), new VertexBufferBinding(InstanceBuffer, 0, 1));
                    Shader.CurrentTechnique.Passes[0].Apply();
                    Graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount / 3, InstanceBuffer.VertexCount);
                    
                }
            }
            
            if (HasSet)
            {
                Shader.Parameters["ClipDirection"].SetValue(0.0f);
                Shader.Parameters["ClipOffset"].SetValue(0.0f);
            }
        }

        public virtual void RenderPostProcessColor(GraphicsDevice Graphics, Matrix ViewMatrix, Matrix ProjectionMatrix, Texture2D ScreenDepthCapture, Texture2D ScreenColorCapture,float FarPlane)
        {
            if (Shader == null || HasCull)
            {
                return;
            }

            if (Shader.Parameters["FarPlane"] != null)
            {
                Shader.Parameters["FarPlane"].SetValue(FarPlane);
            }
            
            if (Shader.Parameters["SceneDepthTexture"] != null)
            {
                Shader.Parameters["SceneDepthTexture"].SetValue((Texture2D)ScreenDepthCapture);
            }

            if (Shader.Parameters["SceneTexture"] != null)
            {
                Shader.Parameters["SceneTexture"].SetValue((Texture2D)ScreenColorCapture);
            }

            if (CurrentRenderMode == RenderMode.Indexed)
            {

            }
            else if (CurrentRenderMode == RenderMode.NonIndexed)
            {
                for (int j = 1; j < Shader.CurrentTechnique.Passes.Count; j++)
                {
                    Graphics.SetVertexBuffer(VertexBuffer);
                    Shader.CurrentTechnique.Passes[j].Apply();
                    Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexBuffer.VertexCount / 3);
                }
            }
            else if (CurrentRenderMode == RenderMode.NonIndexBufferless)
            {
                for (int j = 1; j < Shader.CurrentTechnique.Passes.Count; j++)
                {
                    Shader.CurrentTechnique.Passes[j].Apply();
                    Graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexCount / 3);
                }
            }
            else if (CurrentRenderMode == RenderMode.Instanced)
            {

            }
        }
  
        public void SetTextures( string[] Textures)
        {
            TextureNames.Clear();
            TextureNames.AddRange(Textures);
            HasTextureUpdate = true;
        }
        
        public void SetCollisionBuffer(Vector3[] NewCollisionBuffer)
        {
            CollisionBuffer = NewCollisionBuffer;
        }
        
        public void SetName(string NewName)
        {
            Name = NewName;
        }
        
        public void SetIndexBuffer(IndexBuffer NewIndexBuffer)
        {
            if (IndexBuffer != null)
            {
                IndexBuffer.Dispose();
            }
            IndexBuffer = NewIndexBuffer;
        }

        
        public void SetInstanceBuffer(VertexBuffer NewInstanceBuffer, List<Matrix> InstanceCollisionData)
        {
            if (InstanceBuffer != null)
            {
                InstanceBuffer.Dispose();
            }

            if (InstanceCollisionBuffer.Count > 0)
            {
                InstanceCollisionBuffer.Clear();
            }
            
            InstanceCollisionBuffer.AddRange(InstanceCollisionData);
            InstanceBuffer = NewInstanceBuffer;
        }

        public void SetVertexBuffer(VertexBuffer NewVertexBuffer)
        {
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
            }
            VertexBuffer = NewVertexBuffer;
        }

        public void SetVertexBuffer(VertexBuffer NewVertexBuffer, Vector3[] NewCollisionBuffer)
        {
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
            }
            VertexBuffer = NewVertexBuffer;
            CollisionBuffer = (Vector3[])NewCollisionBuffer.Clone();
        }
        
        public Vector3[] GetBoundsAB()
        {
            if (CollisionBuffer.Length == 0)
            {
                return new Vector3[] { new Vector3(), new Vector3()};
            }

            float MinX = 0;
            float MinY = 0;
            float MinZ = 0;
            float MaxX = 0;
            float MaxY = 0;
            float MaxZ = 0;

            for (int i = 0; i < CollisionBuffer.Length; i++)
            {
                MinX = CollisionBuffer[i].X < MinX ? CollisionBuffer[i].X : MinX;
                MinY = CollisionBuffer[i].Y < MinY ? CollisionBuffer[i].Y : MinY;
                MinZ = CollisionBuffer[i].Z < MinZ ? CollisionBuffer[i].Z : MinZ;

                MaxX = CollisionBuffer[i].X > MaxX ? CollisionBuffer[i].X : MaxX;
                MaxY = CollisionBuffer[i].Y > MaxY ? CollisionBuffer[i].Y : MaxY;
                MaxZ = CollisionBuffer[i].Z > MaxZ ? CollisionBuffer[i].Z : MaxZ;
            }
            return new Vector3[] { new Vector3(MinX,MinY,MinZ), new Vector3(MaxX,MaxY,MaxZ)};
        }

        //TODO ADD Prop Scale
        public CollisionResults CollideWith(Vector3 RayPosition, Vector3 RayDirection, float Range)
        {
            CollisionResults Results = new CollisionResults();
            
            if (CurrentRenderMode == RenderMode.Instanced)
            {
                for (int i = 0; i < InstanceCollisionBuffer.Count; i++)
                {
                    Matrix InstanceMatrix = InstanceCollisionBuffer[i];

                    if (Vector3.Distance(RayPosition, Position) <= Range)
                    {

                    }

                    for (int j = 0; j < CollisionBuffer.Length; j += 3)
                    {
                        Vector3 V0 = Vector3.Transform(CollisionBuffer[j], InstanceMatrix);
                        Vector3 V1 = Vector3.Transform(CollisionBuffer[j+1], InstanceMatrix);
                        Vector3 V2 = Vector3.Transform(CollisionBuffer[j+2], InstanceMatrix);

                        Vector3 CollisionPoint = new Vector3();

                        if (CollisionUtil.Intersect(V0, V1, V2, RayPosition, RayDirection, CollisionPoint))
                        {
                            CollisionResult Result = new CollisionResult(
                                V0,
                                V1,
                                V2,
                                i,
                                Position,
                                CollisionPoint,
                                this);

                            /*
                            Debug.WriteLine("----------");
                            Debug.WriteLine(CollisionPoint);
                            Debug.WriteLine(V0);
                            Debug.WriteLine(V1);
                            Debug.WriteLine(V2);
                            */

                            Results.Add(Result);
                        }
                    }

                }
            }
            else
            {
                for (int i = 0; i < CollisionBuffer.Length; i += 3)
                {
                    Vector3 V0 = CollisionBuffer[i] + Position;
                    Vector3 V1 = CollisionBuffer[i + 1] + Position;
                    Vector3 V2 = CollisionBuffer[i + 2] + Position;

                    //if (CollisionUtil.Intersect(V0 + Position, V1 + Position, V2 + Position, RayPosition, RayDirection))
                    //{
                    Vector3 CollisionPoint = new Vector3();

                    if (CollisionUtil.Intersect(V0, V1, V2, RayPosition, RayDirection, CollisionPoint))
                    {
                        CollisionResult Result = new CollisionResult(
                            V0,
                            V1,
                            V2,
                            Position,
                            CollisionPoint,
                            this);

                        Results.Add(Result);
                    }
                }
            }
            return Results;
        }

        public CollisionResults CollideWith(Vector3 RayPosition, Vector3 RayDirection)
        {
            if (CurrentRenderMode == RenderMode.Instanced)
            {
                CollisionResults Results = new CollisionResults();

                for (int i = 0; i < InstanceCollisionBuffer.Count; i++)
                {
                    Matrix InstanceMatrix = InstanceCollisionBuffer[i];

                    for ( int j = 0; j < CollisionBuffer.Length; j += 3)
                    {
                        Vector3 V0 = Vector3.Transform(CollisionBuffer[j], InstanceMatrix);
                        Vector3 V1 = Vector3.Transform(CollisionBuffer[j+1], InstanceMatrix);
                        Vector3 V2 = Vector3.Transform(CollisionBuffer[j+2], InstanceMatrix);

                        //if (CollisionUtil.Intersect(V0 + Position, V1 + Position, V2 + Position, RayPosition, RayDirection))
                        //{
                        if (CollisionUtil.Intersect(V0, V1, V2, RayPosition, RayDirection))
                        {
                            Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, RayPosition, RayDirection);

                            CollisionResult Result = new CollisionResult(
                            V0,
                            V1,
                            V2,
                            i,
                            Position,
                            CollisionPoint,
                            this);

                            Results.Add(Result);
                        }
                    }

                }
            }
            else
            {
                CollisionResults Results = new CollisionResults();

                for (int i = 0; i < CollisionBuffer.Length; i += 3)
                {
                    Vector3 V0 = CollisionBuffer[i] + Position;
                    Vector3 V1 = CollisionBuffer[i+1] + Position;
                    Vector3 V2 = CollisionBuffer[i+2] + Position;

                    //if (CollisionUtil.Intersect(V0 + Position, V1 + Position, V2 + Position, RayPosition, RayDirection))
                    //{
                    if (CollisionUtil.Intersect(V0, V1, V2, RayPosition, RayDirection))
                    {
                        Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(V0, V1, V2, RayPosition, RayDirection);

                        CollisionResult Result = new CollisionResult(
                        V0,
                        V1,
                        V2,
                        Position,
                        CollisionPoint,
                        this);

                        Results.Add(Result);
                    }
                }
                return Results;
            }
            return null;
        }
        
        public virtual object Clone()
        {
            Geometry3D NewGeom = new Geometry3D(Name);
            NewGeom.Shader = Shader == null ? null : (BasicEffect)Shader.Clone();
            NewGeom.CollisionBuffer = CollisionBuffer == null ? null : (Vector3[])CollisionBuffer.Clone();
            NewGeom.Position = new Vector3(Position.X, Position.Y, Position.Z);
            NewGeom.RenderBucket = RenderBucket;
            NewGeom.HasCull = HasCull;
            NewGeom.TextureNames.AddRange(TextureNames);
            return NewGeom;
        }

    }
}
