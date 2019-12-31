using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Physics
{
    public class PhysicsControl
    {
        public Geometry3D Geometry { get; set; }

        public bool[,,] Particles { get; set; }
        public Vector3[] Bounds { get; set; }

        public float Precision = 0.25f;

        public PhysicsControl(Geometry3D Geom)
        {
            Geometry = Geom;
            GenerateBounds();
        }
        
        public void GenerateBounds()
        {
            Bounds = Geometry.GetBoundsAB();

            List<Vector3> VertexList = new List<Vector3>();
            for (int i = 0; i < Geometry.CollisionBuffer.Length; i++)
            {
                //VertexList.Add(Geometry.CollisionBuffer[i].Position);
            }
            
            Vector3 Direction = new Vector3(0,1,0);

            int SizeX = (int)((Bounds[1].X - Bounds[0].X) / Precision);
            int SizeY = (int)((Bounds[1].Y - Bounds[0].Y) / Precision);
            int SizeZ = (int)((Bounds[1].Z - Bounds[0].Z) / Precision);

            Particles = new bool[ SizeX, SizeY, SizeZ];

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeZ; k++)
                    {
                        Vector3 Position = new Vector3(i,j,k) * Precision + Bounds[0];
                        int Count = CollisionUtil.GetCollisionCount(VertexList, Position, Direction);
                        Particles[i, j, k] = Count % 2 == 1;
                    }
                }
            }
        }

    }
}
