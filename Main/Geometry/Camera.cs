using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Geometry
{
    public class Camera
    {
        public float Fov { get; set; } = MathHelper.PiOver4;
        public float FarPlaneClip { get; set; } = 1000.0f;
        public float NearPlaneClip { get; set; } = 1.0f;
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 ViewDirection { get; set; } = new Vector3();

        public float ResolutionX { get; private set; } = 0;
        public float ResolutionY { get; private set; } = 0;

        public Matrix ViewMatrix { get; set; } = new Matrix();
        public Matrix ProjectionMatrix { get; private set; } = new Matrix();

        public bool IsOrthogonal { get; private set; } = false;

        public Camera(Vector3 pos, float Width, float Height, float farPlane, float nearPlane, float fov)
        {
            Position = pos;
            Fov = fov;
            FarPlaneClip = farPlane;
            NearPlaneClip = nearPlane;
        }
        
        public Camera(Vector3 pos, float Width, float Height, float farPlane, float nearPlane)
        {
            Position = pos;
            FarPlaneClip = farPlane;
            NearPlaneClip = nearPlane;
            ResolutionX = Width;
            ResolutionY = Height;
            
        }
        
        public void GenerateMatrices(Vector3 NewViewDirection)
        {
            ViewDirection = Vector3.Normalize(NewViewDirection);
            ViewMatrix = Matrix.CreateLookAt(Position, Position + ViewDirection * 10.0f, Vector3.UnitZ);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Fov, ResolutionX / ResolutionY, NearPlaneClip, FarPlaneClip);
        }

        public void GenerateMatrices(Vector3 NewViewDirection, Vector3 UpVector)
        {
            ViewDirection = Vector3.Normalize(NewViewDirection);
            ViewMatrix = Matrix.CreateLookAt(Position, Position + ViewDirection * 10.0f, UpVector);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Fov, ResolutionX / ResolutionY, NearPlaneClip, FarPlaneClip);
        }
        
        public void GenerateLookAtViewMatrix(Vector3 Pos, Vector3 LookAtPos)
        {
            Position = Pos;

            ViewDirection = LookAtPos - Pos;
            ViewDirection = Vector3.Normalize(ViewDirection);
            ViewMatrix = Matrix.CreateLookAt(Pos, LookAtPos, Vector3.UnitZ);
            
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Fov, ResolutionX / ResolutionY, NearPlaneClip, FarPlaneClip);
        }

        
        public void GenerateLookAtViewMatrix(Vector3 Pos, Vector3 LookAtPos, Vector3 UpVector)
        {
            Position = Pos;

            ViewDirection = LookAtPos - Pos;
            ViewDirection = Vector3.Normalize(ViewDirection);
            ViewMatrix = Matrix.CreateLookAt(Pos, LookAtPos, UpVector);

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Fov, ResolutionX / ResolutionY, NearPlaneClip, FarPlaneClip);
            IsOrthogonal = false;
        }

        public void GenerateOrthogonalViewMatrix(Vector3 Pos, Vector3 LookAtPos, Vector3 UpVector)
        {
            Position = Pos;
            ViewMatrix = Matrix.Identity;

            //ViewMatrix = new Matrix(new Vector4(0,1,0,0), new Vector4(1,0,0,0), new Vector4(0,0,1,0), new Vector4(0,0,0,0));

            //ViewMatrix = Matrix.CreateOrthographic(ResolutionX, ResolutionY, NearPlaneClip, FarPlaneClip);
            
            ViewMatrix = Matrix.CreateLookAt(Pos, LookAtPos, UpVector);

            //ViewMatrix = Matrix.Identity;
            
            ProjectionMatrix = Matrix.CreateOrthographic(ResolutionX, ResolutionY, NearPlaneClip, FarPlaneClip);
            IsOrthogonal = true;
        }

        public void SetResolution(float Width, float Height)
        {
            ResolutionX = Width;
            ResolutionY = Height;
        }
        
    }
}
