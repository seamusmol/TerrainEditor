using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Geometry
{
    public class CollisionQuad 
    {
        public string Name = "CollisionQuad";
        public Vector3 A { get; private set; }
        public Vector3 B { get; private set; }
        public Vector3 C { get; private set; }
        public Vector3 D { get; private set; }
        public Vector3 Position = new Vector3();
        public Vector3[] CollisionBuffer { get; private set; } = new Vector3[0];

        /**
         * A->B->D D->C->A
         **/
        public CollisionQuad(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
        {
            A = new Vector3(P1.X, P1.Y, P1.Z);
            B = new Vector3(P2.X, P2.Y, P2.Z);
            C = new Vector3(P3.X, P3.Y, P3.Z);
            D = new Vector3(P4.X, P4.Y, P4.Z);

            GenerateCollisionBuffer();
        }

        public CollisionQuad(string QuadName, Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
        {
            Name = QuadName;
            A = new Vector3(P1.X, P1.Y, P1.Z);
            B = new Vector3(P2.X, P2.Y, P2.Z);
            C = new Vector3(P3.X, P3.Y, P3.Z);
            D = new Vector3(P4.X, P4.Y, P4.Z);

            GenerateCollisionBuffer();
        }

        private void GenerateCollisionBuffer()
        {
            CollisionBuffer = new Vector3[]
            {
                A,B,D,D,C,A
            };
        }

        public void SetPosition(Vector3 NewPosition)
        {
            Position = new Vector3(NewPosition.X, NewPosition.Y, NewPosition.Z);
            GenerateCollisionBuffer();
        }

        /**
        * A->B->D D->C->A
        **/
        public void SetVertices(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
        {
            A = new Vector3(P1.X, P1.Y, P1.Z);
            B = new Vector3(P2.X, P2.Y, P2.Z);
            C = new Vector3(P3.X, P3.Y, P3.Z);
            D = new Vector3(P4.X, P4.Y, P4.Z);
            GenerateCollisionBuffer();
        }

        public bool HasCollision(Vector3 RayPosition, Vector3 RayDirection)
        {
            CollisionResults Results = new CollisionResults();

            for (int i = 0; i < CollisionBuffer.Length; i += 3)
            {
                if (CollisionUtil.Intersect(CollisionBuffer[i] + Position, CollisionBuffer[i + 1] + Position, CollisionBuffer[i + 2] + Position, RayPosition, RayDirection))
                {
                    Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(CollisionBuffer[i] + Position, CollisionBuffer[i + 1] + Position, CollisionBuffer[i + 2] + Position, RayPosition, RayDirection);

                    return true;
                }
            }
            return false;
        }
        
        public CollisionResults CollideWith(Vector3 RayPosition, Vector3 RayDirection)
        {
            CollisionResults Results = new CollisionResults();
            
            for (int i = 0; i < CollisionBuffer.Length; i += 3)
            {
                if (CollisionUtil.Intersect(CollisionBuffer[i] + Position, CollisionBuffer[i + 1] + Position, CollisionBuffer[i + 2] + Position, RayPosition, RayDirection))
                {
                    Vector3 CollisionPoint = CollisionUtil.GetCollisionPoint(CollisionBuffer[i] + Position, CollisionBuffer[i + 1] + Position, CollisionBuffer[i + 2] + Position, RayPosition, RayDirection);

                    CollisionResult Result = new CollisionResult(
                        Name,
                        CollisionBuffer[i],
                        CollisionBuffer[i + 1],
                        CollisionBuffer[i + 2],
                        Position,
                        CollisionPoint
                        );

                    Results.Add(Result);
                }
            }
            return Results;
        }

    }
}
