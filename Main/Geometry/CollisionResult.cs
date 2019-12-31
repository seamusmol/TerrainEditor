using Main.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Geometry
{
    public class CollisionResult
    {
        public Vector3 P1, P2, P3;

        public int InstanceID { get; set; } = -1;
        public string GeometryName { get; set; }

        public Vector3 Origin { get; set; }
        public Vector3 CollisionPoint { get; set; }
        public Geometry3D Geometry { get; set; }

        public float Distance { get; set; } = 0;

        public CollisionResult(string Name, Vector3 Position1, Vector3 Position2, Vector3 Position3, Vector3 Or, Vector3 CollisionP)
        {
            GeometryName = Name;
            P1 = Position1;
            P2 = Position2;
            P3 = Position3;
            Origin = Or;
            CollisionPoint = CollisionP;
            Distance = Vector3.Distance(CollisionP, Or);
        }

        public CollisionResult(Vector3 Position1, Vector3 Position2, Vector3 Position3, Vector3 Or, Vector3 CollisionP, Geometry3D Geom)
        {
            P1 = Position1;
            P2 = Position2;
            P3 = Position3;
            GeometryName = Geom.Name;
            Origin = Or;
            CollisionPoint = CollisionP;
            Geometry = Geom;
            Distance = Vector3.Distance(CollisionP, Or);
        }
        
        public CollisionResult(Vector3 Position1, Vector3 Position2, Vector3 Position3, int ID, Vector3 Or, Vector3 CollisionP, Geometry3D Geom)
        {
            P1 = Position1;
            P2 = Position2;
            P3 = Position3;

            InstanceID = ID;

            GeometryName = Geom.Name;
            Origin = Or;
            CollisionPoint = CollisionP;
            Geometry = Geom;
            Distance = Vector3.Distance(CollisionP, Or);
        }
        
    }
}
