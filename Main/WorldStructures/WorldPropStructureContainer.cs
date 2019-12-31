using Main.Geometry;
using Main.Main;
using Main.Structures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static Main.Geometry.GeometryModel;
using Main.Util;
using Main.WorldEditor;

namespace Main.WorldStructures
{
    //All Prop/Structure Data should be read as read-only
    //Generate ALl LOD/Model Batches on load

    public class WorldPropStructureContainer
    {   
        List<Geometry3D> Batches = new List<Geometry3D>();
        
        public WorldPropStructureContainer(RenderManager Render)
        {
            GenerateGeometry(Render);
        }
        
        public void UpdatePreRender(SettingsContainer WorldSettings, Vector3 Position, Vector3 ViewDirection, float Time)
        {
            Vector3 LightPosition = new Vector3(0, 0, 100);

            for (int i = 0; i < Batches.Count; i++)
            {
                Batches[i].Shader.Parameters["LightPosition"].SetValue(WorldSettings.DebugLightPosition);
                Batches[i].Shader.Parameters["CameraPosition"].SetValue(Position);

                Batches[i].Shader.Parameters["LightColor"].SetValue(WorldSettings.DebugDirectLightColor);
                Batches[i].Shader.Parameters["AmbientLightColor"].SetValue(WorldSettings.DebugAmbientLightColor);
            }
        }
        
        public void Update(GraphicsDevice Graphics, List<Prop> PropList, List<Structure> StructureList)
        {
            GenerateInstanceBuffer(Graphics, PropList, StructureList);
        }
        
        public void GenerateGeometry(RenderManager Render)
        {
            Dictionary<string, GeometryModel> Models = Render.Models;

            //Generate 
            foreach (KeyValuePair<string, GeometryModel> entry in Models)
            {
                Geometry3D NewBatch = new Geometry3D(entry.Key);
                NewBatch.SetVertexBuffer( entry.Value.GetBufferData(Render.Graphics), entry.Value.VertexList.ToArray());
                NewBatch.Shader = entry.Value.Shader.Clone();
                NewBatch.DepthShader = entry.Value.DepthShader.Clone();
                NewBatch.SetIndexBuffer( GraphicsUtil.GenerateIndexBuffer(Render.Graphics, entry.Value.VertexList.Count));
                
                //TODO
                //Add Proper System for texture import
                foreach (KeyValuePair<string,string> Textures in entry.Value.Textures)
                {
                    if (NewBatch.Shader.Parameters[Textures.Key] != null)
                    {
                        NewBatch.Shader.Parameters[Textures.Key].SetValue(Render.Textures[Textures.Value]);
                    }
                }

                NewBatch.CurrentRenderMode = Geometry3D.RenderMode.Instanced;
                //set VertexBuffer
                Batches.Add(NewBatch);
            }
        }
        
        public void GenerateInstanceBuffer(GraphicsDevice Graphics, List<Prop> PropList, List<Structure> StructureList)
        {
            if (PropList.Count > 0)
            {
                PropList.OrderBy( x => x.LOD).ThenBy(x=> x.Name);
            }
            if (StructureList.Count > 0)
            {
                //StructureList.OrderBy(x => x.LOD).ThenBy(x => x.Name);
            }

            for (int i = 0; i < Batches.Count;i++)
            {
                List<ModelInstanceVertex> InstanceData = new List<ModelInstanceVertex>();
                List<Matrix> InstanceCollisionData = new List<Matrix>();

                //remove "transparent" 
                foreach ( Prop Prop in PropList)
                {
                    if (Prop.Name == Batches[i].Name.Replace("-Transparent-","") && !Prop.HasCull)
                    {
                        InstanceData.Add( new ModelInstanceVertex(Prop.Position, Prop.Orientation.Forward, Prop.Orientation.Up, Prop.Orientation.Left, Prop.Scale, Prop.HighLight, Prop.TB0, Prop.TB1, Prop.TB2));

                        //Matrix Orientation = Matrix.Identity;
                        //Matrix Rotation = Matrix.Identity;

                        //Rotation *= Matrix.CreateFromAxisAngle(Orientation.Up, MathHelper.ToRadians(Z));
                        //Rotation *= Matrix.CreateFromAxisAngle(Orientation.Right, MathHelper.ToRadians(Y));
                        //Rotation *= Matrix.CreateFromAxisAngle(Orientation.Forward, MathHelper.ToRadians(X));

                        Matrix Rotation = Prop.Orientation;

                        //Matrix Rotation = Matrix.CreateTranslation(0,0,0);
                        //Rotation *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Prop.Angles.Z), MathHelper.ToRadians(Prop.Angles.X), MathHelper.ToRadians(Prop.Angles.Y));

                        //Rotation *= Matrix.CreateRotationZ(MathHelper.ToRadians(Prop.Angles.Y));
                        //Rotation *= Matrix.CreateRotationY(MathHelper.ToRadians(Prop.Angles.Z));
                        //Rotation *= Matrix.CreateRotationX(MathHelper.ToRadians(Prop.Angles.X));

                        //Rotation *= Matrix.CreateScale(Prop.Scale);

                        //Rotation.Translation = Prop.Position;
                        //Rotation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Prop.Angles.Z), MathHelper.ToRadians(Prop.Angles.X), MathHelper.ToRadians(Prop.Angles.Y));

                        Rotation *= Matrix.CreateScale(Prop.Scale);

                        Rotation.Translation = Prop.Position;
                        
                        InstanceCollisionData.Add(Rotation);
                    }
                }
                
                if (InstanceData.Count != 0)
                {
                    VertexBuffer InstanceBuffer = new VertexBuffer(Graphics, ModelInstanceVertex.ModelInstanceVertexDeclaration, InstanceData.Count, BufferUsage.None);
                    
                    InstanceBuffer.SetData(InstanceData.ToArray());
                    Batches[i].SetInstanceBuffer(InstanceBuffer, InstanceCollisionData);
                    
                    Batches[i].InstanceCount = InstanceData.Count;
                    Batches[i].HasCull = false;
                }
                else
                {
                    Batches[i].InstanceCount = 0;
                    Batches[i].HasCull = true;
                }

            }
        }
        
        public List<Geometry3D> GetGeometries(int LOD)
        {
            List<Geometry3D> GeomList = new List<Geometry3D>();
            if (LOD == -1)
            {
                GeomList.AddRange(Batches);
            }
            else
            {
                for (int i = 0; i  < Batches.Count; i++)
                {
                    string[] Split = Batches[i].Name.Split( new char[] { ' ' });
                    int BatchLOD = int.Parse( Split[Split.Length - 1]);
                    if (BatchLOD == LOD)
                    {
                        GeomList.Add(Batches[i]);
                    }
                }

            }
            return GeomList;
        }
        
        public CollisionResults CollideWith(Vector3 Origin, Vector3 Direction, float Range)
        {
            CollisionResults Results = new CollisionResults();

            for (int i = 0; i < Batches.Count;i ++)
            {
                CollisionResults BatchResults = Batches[i].CollideWith( Origin, Direction, Range);

                if (Batches[i].InstanceCount > 0)
                {
                    Results.AddRange( Batches[i].CollideWith(Origin,Direction,Range));
                }
            }
            
            return Results;
        }

    }
    
}
