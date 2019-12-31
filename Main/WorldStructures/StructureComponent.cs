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

namespace Main.Structures
{
    public class StructureComponent : ICloneable
    {   
        public Vector3 A { get; set; } = new Vector3();
        public Vector3 Rotation { get; set; } = new Vector3();
        public Vector3 Scale { get; set; } = new Vector3();

        public Vector3 WorldPosition { get; set; } = new Vector3();

        public Geometry3D TransGeom { get; set; } = new Geometry3D("TransCompGeom");
        public Geometry3D Geom { get; set; } = new Geometry3D("CompGeom");
        
        public bool NeedsGeomUpdate { get; set; } = true;
        public string Name { get; set; } = "";
        
        public StructureComponent(Vector3 BoundA, Vector3 Angles, Vector3 Size)
        {
            A = BoundA;
            Rotation = Angles;
            Scale = Size;
        }

        public StructureComponent(byte[] Data)
        {

        }
        
        public virtual void Init(byte[] Data)
        {

        }

        protected StructureComponent() { }
        
        public virtual void GenerateGeometry(RenderManager RenderManager)
        {
            Geom = new Geometry3D(Name);
            Geom.SetTextures(new string[] {"Missing"});
            BasicEffect BasicEffect = new BasicEffect(RenderManager.Graphics);
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            Geom.HasCull = false;
            Geom.LocalPosition = A;
            Geom.WorldPosition = WorldPosition;
            Geom.RenderBucket = Geometry3D.RenderQueue.Solid;

            TransGeom = new Geometry3D("Trans"+Name);
            TransGeom.SetTextures(new string[] { "Missing" });
            BasicEffect.TextureEnabled = true;
            Geom.Shader = BasicEffect;
            TransGeom.HasCull = false;
            TransGeom.LocalPosition = A;
            TransGeom.WorldPosition = WorldPosition;
            TransGeom.RenderBucket = Geometry3D.RenderQueue.Transparent;
        }
        
        public virtual void SetWorldPosition(Vector3 NewWorldPosition)
        {
            WorldPosition = NewWorldPosition;
            if (TransGeom != null)
            {
                TransGeom.LocalPosition = A;
                TransGeom.WorldPosition = WorldPosition;
            }
            if (Geom != null)
            {
                Geom.LocalPosition = A;
                Geom.WorldPosition = WorldPosition;
            }
        }
        
        public virtual void SetLocalPosition(Vector3 NewPosition)
        {
            A = NewPosition;
            if (TransGeom != null)
            {
                TransGeom.LocalPosition = A;
                TransGeom.WorldPosition = WorldPosition;
            }
            if (Geom != null)
            {
                Geom.LocalPosition = A;
                Geom.WorldPosition = WorldPosition;
            }
        }
        
        public virtual byte[] ToBytes()
        {
            return new byte[0];
        }

        public virtual void UpdateGeometry(RenderManager RenderManager)
        {
        }
        
        public virtual object Clone()
        {
            StructureComponent NewComponent = new StructureComponent();

            NewComponent.A = new Vector3(A.X, A.Y, A.Z);
            NewComponent.WorldPosition = new Vector3(WorldPosition.X,WorldPosition.Y,WorldPosition.Z);
            NewComponent.Rotation = new Vector3(Rotation.X,Rotation.Y,Rotation.Z);
            NewComponent.Scale = new Vector3(Scale.X, Scale.Y, Scale.Z);
           
            NewComponent.NeedsGeomUpdate = NeedsGeomUpdate;
            NewComponent.Name = Name;

            return NewComponent;
        }

    }
}
