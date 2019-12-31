using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Main.Main.Geometry3D;
using static Main.Structures.StructureComponent;

namespace Main.Structures
{

    //TODO
    //Add Structure Position to component Geometry for collision Nodes
    public class Structure : ICloneable
    {
        public GeometryBatch SolidPropGeomBatch { get; set; } = new GeometryBatch();
        public GeometryBatch TransPropGeomBatch { get; set; } = new GeometryBatch();

        public GeometryBatch GeomBatch { get; set; } = new GeometryBatch();
        public GeometryBatch TransGeomBatch { get; set; } = new GeometryBatch();

        public string Name { get; set; } = "";

        public bool NeedsUpdate { get; set; } = true;
        public Vector3 Position { get; set; } = new Vector3();

        public List<StructureComponent> Components = new List<StructureComponent>();

        public Structure(string StructureName, Vector3 Pos, StructureComponent[] StructureComponents)
        {
            Name = StructureName;
            Position = Pos;

            if (StructureComponents.Length > 0)
            {
                Components.AddRange(StructureComponents);
            }
        }
        
        protected Structure(){}

        public void InitBatches(RenderManager RenderManager)
        {
            SolidPropGeomBatch = new GeometryBatch(Name + "-SolidProp", RenderManager.Graphics, "Prop");
            TransPropGeomBatch = new GeometryBatch(Name + "-TransProp", RenderManager.Graphics, "Prop");

            GeomBatch = new GeometryBatch(Name + "-SolidStructure", RenderManager.Graphics, "Structure");
            TransGeomBatch = new GeometryBatch(Name + "-TransStructure", RenderManager.Graphics, "Structure");

            SolidPropGeomBatch.Geom.RenderBucket = RenderQueue.Solid;
            TransPropGeomBatch.Geom.RenderBucket = RenderQueue.Transparent;
            GeomBatch.Geom.RenderBucket = RenderQueue.Solid;
            TransGeomBatch.Geom.RenderBucket = RenderQueue.Transparent; 
            
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Name = Name + "-" + i;
                Components[i].GenerateGeometry(RenderManager);
                Components[i].SetWorldPosition(Position);
                AddGeometry( Components[i]);
            }
        }

        public void UpdatePosition(Vector3 NewPosition)
        {
            Position = NewPosition;

            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].SetWorldPosition(Position);
            }
        }

        public void Update(RenderManager RenderManager)
        {
            UpdateGeometries(RenderManager);
        }

        public void UpdateGeometries(RenderManager RenderManager)
        {
            bool HasBatchUpdate = false;
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].NeedsGeomUpdate)
                {
                    Components[i].UpdateGeometry(RenderManager);
                    Components[i].NeedsGeomUpdate = false;
                    HasBatchUpdate = true;
                }
            }

            if (HasBatchUpdate)
            {
                SolidPropGeomBatch.GenerateVertexBuffer(RenderManager.Graphics);
                TransPropGeomBatch.GenerateVertexBuffer(RenderManager.Graphics);
                GeomBatch.GenerateVertexBuffer(RenderManager.Graphics);
                TransGeomBatch.GenerateVertexBuffer(RenderManager.Graphics);
            }
            
            SolidPropGeomBatch.Geom.WorldPosition = Position;
            TransPropGeomBatch.Geom.WorldPosition = Position;
            GeomBatch.Geom.WorldPosition = Position;
            TransGeomBatch.Geom.WorldPosition = Position;
        }
        
        public void AddGeometry(StructureComponent NewComponent)
        {
            if (NewComponent.GetType() != typeof(Prop) )
            {
                GeomBatch.Add(NewComponent.Geom);
                TransGeomBatch.Add(NewComponent.TransGeom);
            }
            else
            {
                SolidPropGeomBatch.Add(NewComponent.Geom);
                TransPropGeomBatch.Add(NewComponent.TransGeom);
            }
        }

        public void Add( StructureComponent NewComponent)
        {
            Components.Add(NewComponent);
            AddGeometry(NewComponent);
        }
        
        public void AttachToCollisionNode(Node3D CollisionNode, Node3D TransCollisionNode)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                CollisionNode.Attach(Components[i].Geom);
                TransCollisionNode.Attach(Components[i].TransGeom);
            }

        }

        public void DetachFromCollisionNode(Node3D CollisionNode, Node3D TransCollisionNode)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                CollisionNode.DetachGeometry(Components[i].Geom);
                TransCollisionNode.DetachGeometry(Components[i].TransGeom);
            }
        }

        public StructureComponent GetComponentByGeometry(Geometry3D Geom)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Geom.Equals(Geom) || Components[i].TransGeom.Equals(Geom))
                {
                    return Components[i];
                }
            }
            return null;
        }

        public virtual object Clone()
        {
            Structure NewStructure = new Structure();
            NewStructure.SolidPropGeomBatch = (GeometryBatch)SolidPropGeomBatch.Clone();
            NewStructure.TransPropGeomBatch = (GeometryBatch)TransPropGeomBatch.Clone();
            NewStructure.GeomBatch = (GeometryBatch)GeomBatch.Clone();
            NewStructure.TransGeomBatch = (GeometryBatch)TransGeomBatch.Clone();

            NewStructure.Name = (string)Name.Clone();
            NewStructure.Position = new Vector3(Position.X, Position.Y, Position.Z);
            NewStructure.Components = new List<StructureComponent>();
            
            for (int i = 0; i < Components.Count; i++)
            {
                NewStructure.Components.Add( (StructureComponent)Components[i].Clone());

            }
            return NewStructure;
    }


    }


}
