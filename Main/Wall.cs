using Main.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Structures
{

    public class Wall : StructureComponent, ICloneable
    {
        public enum VoxelPattern
        {
            MarchingSquare = 0, Square = 1
        }
        
        public Wall(Vector3 PA, Vector3 Rot, Vector3 Scale) : base(PA, Rot, Scale)
        {

        }

        protected Wall(){ }

        public override void UpdateGeometry(RenderManager RenderManager)
        {
            Geom.LocalPosition = A;
            Geom.WorldPosition = WorldPosition;
            TransGeom.LocalPosition = A;
            TransGeom.WorldPosition = WorldPosition;
            WallGenerator.GenerateWall(RenderManager.Graphics, this);
        }
    
        public override object Clone()
        {
            Wall NewComponent = new Wall();

            NewComponent.A = new Vector3(A.X,A.Y,A.Z);
            NewComponent.Rotation = new Vector3(Rotation.X,Rotation.Y,Rotation.Z);
            NewComponent.Scale = new Vector3(Scale.X, Scale.Y, Scale.Z);

            NewComponent.NeedsGeomUpdate = NeedsGeomUpdate;
            NewComponent.Name = Name;
            
            return NewComponent;
        }

    }


}
