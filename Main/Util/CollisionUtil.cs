using Main.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Util
{
    public static class CollisionUtil
    {
        public static bool InsideTriangle(Vector2 Point, Vector2 P0, Vector2 P1, Vector2 P2)
        {
            float A = 1.0f / 2.0f * (-P1.Y * P2.X + P0.Y * (-P1.X + P2.X) + P0.X * (P1.Y - P2.Y) + P1.X * P2.Y);
            float sign = A < 0.0f ? -1.0f : 1.0f;
            float s = (P0.Y * P2.X - P0.X * P2.Y + (P2.Y - P0.Y) * Point.X + (P0.X - P2.X) * Point.Y) * sign;
            float t = (P0.X * P1.Y - P0.Y * P1.X + (P0.Y - P1.Y) * Point.X + (P1.X - P0.X) * Point.Y) * sign;

            return s > 0.0f && t > 0.0f && (s + t) < 2.0f * A * sign;
        }
        
        public static Vector3 GetDirection(Vector2 MousePosition, Camera Camera, GraphicsDevice Graphics)
        {
            Vector2 Click2D = new Vector2(MousePosition.X, MousePosition.Y);

            Vector3 CloseClick3D = GetWorldCoordinates(Click2D, 0, Camera, Graphics);
            Vector3 FarClick3D = GetWorldCoordinates(Click2D, 1, Camera, Graphics);

            Vector3 RayDirection = FarClick3D - CloseClick3D;
            RayDirection.Normalize();
            return RayDirection;
        }

        public static Vector3 GetWorldCoordinates(Vector2 MousePosition, float Z, Camera Camera, GraphicsDevice Device)
        {
            Vector3 Point2D = new Vector3(MousePosition.X, MousePosition.Y, Z);
            Vector3 Point3D = Device.Viewport.Unproject(Point2D, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);

            return Device.Viewport.Unproject(Point2D, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
        }
       
        
        public static Vector3 FindWhereClicked(Vector2 MousePosition, Camera Camera, GraphicsDevice Device)
        {
            Vector3 NearPoint2D = new Vector3(MousePosition.X, MousePosition.Y, 0);
            Vector3 FarPoint2D = new Vector3(MousePosition.X, MousePosition.Y, 1);
            Vector3 NearPoint3D = Device.Viewport.Unproject(NearPoint2D, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
            Vector3 FarPoint3D = Device.Viewport.Unproject(FarPoint2D, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);

            Vector3 direction = FarPoint3D - NearPoint3D;

            float ZFactor = -NearPoint3D.Y / direction.Y;
            Vector3 ZeroWorldPoint = NearPoint3D + direction * ZFactor;
            

            return ZeroWorldPoint;
        }
        

        public static int GetCollisionCount(List<Vector3> Vertices, Vector3 Position, Vector3 Direction)
        {
            int Count = 0;
            for (int i = 0; i < Vertices.Count; i += 3)
            {
                Count += Intersect(Vertices[i], Vertices[i + 1], Vertices[i + 2], Position, Direction) ? 1 : 0;
            }
            return Count;
        }

        public static bool Intersect(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 RayPosition, Vector3 RayDirection, Vector3 Result)
        {
            Vector3 E1, E2;

            Vector3 P, Q, T;
            float Det, InvDet, U, V;

            E1 = P2 - P1;
            E2 = P3 - P1;
            P = Vector3.Cross(RayDirection, E2);
            Det = Vector3.Dot(E1, P);

            if (Det > -float.Epsilon && Det < float.Epsilon)
            {
                return false;
            }
            InvDet = 1.0f / Det;

            T = RayPosition - P1;

            U = Vector3.Dot(T, P) * InvDet;

            if (U < 0 || U > 1)
            {
                return false;
            }

            Q = Vector3.Cross(T, E1);
            V = Vector3.Dot(RayDirection, Q) * InvDet;

            if (V < 0 || U + V > 1)
            {
                return false;
            }

            if ((Vector3.Dot(E2, Q) * InvDet) > float.Epsilon)
            {
                Result =  P1 + U * E1 + V * E2;
                return true;
            }
            return false;
        }

        public static bool Intersect(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 RayPosition, Vector3 RayDirection)
        {
            Vector3 E1, E2;

            Vector3 P, Q, T;
            float Det, InvDet, U, V;

            E1 = P2 - P1;
            E2 = P3 - P1;
            P = Vector3.Cross(RayDirection, E2);
            Det = Vector3.Dot(E1, P);

            if (Det > -float.Epsilon && Det < float.Epsilon)
            {
                return false;
            }
            InvDet = 1.0f / Det;

            T = RayPosition - P1;

            U = Vector3.Dot(T, P) * InvDet;

            if (U < 0 || U > 1)
            {
                return false;
            }

            Q = Vector3.Cross(T, E1);
            V = Vector3.Dot(RayDirection, Q) * InvDet;

            if (V < 0 || U + V > 1)
            {
                return false;
            }

            if ((Vector3.Dot(E2, Q) * InvDet) > float.Epsilon)
            {
                return true;
            }
            return false;
        }

        //TODO
        //Add Get Collisionpoint
        public static Vector3 GetCollisionPoint(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 RayPosition, Vector3 RayDirection)
        {
            Vector3 E1, E2;

            Vector3 P, Q, T;
            float Det, InvDet, U, V;

            E1 = P2 - P1;
            E2 = P3 - P1;
            P = Vector3.Cross(RayDirection, E2);
            Det = Vector3.Dot(E1, P);

            if (Det > -float.Epsilon && Det < float.Epsilon)
            {
                return new Vector3(float.NaN, float.NaN, float.NaN);
            }
            InvDet = 1.0f / Det;

            T = RayPosition - P1;

            U = Vector3.Dot(T, P) * InvDet;

            if (U < 0 || U > 1)
            {
                return new Vector3(float.NaN, float.NaN, float.NaN);
            }

            Q = Vector3.Cross(T, E1);
            V = Vector3.Dot(RayDirection, Q) * InvDet;

            if (V < 0 || U + V > 1)
            {
                return new Vector3(float.NaN, float.NaN, float.NaN);
            }

            if ((Vector3.Dot(E2, Q) * InvDet) > float.Epsilon)
            {
                return P1 + U * E1 + V * E2;
            }
            return new Vector3(float.NaN, float.NaN, float.NaN);
        }
        

    }
}
