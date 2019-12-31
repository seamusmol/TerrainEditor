using Main.Geometry;
using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Structures
{
    public class Prop
    {
        public enum RenderMode
        {
            None = 0, Opaque = 1,Transparent = 2, Both = 3
        }
        
        public int LOD { get; set; } = 0;
        public string Name { get; private set; } = "";
        
        public Vector3 Position { get;  set; } = new Vector3();
        public Vector3 Scale { get; set; } = new Vector3();
        public Vector3 Angles { get; private set; } = new Vector3();
        public Matrix Orientation = Matrix.Identity;

        public byte HighLight { get; set; } = 0;
        public byte TB0 { get; set; } = 0;
        public byte TB1 { get; set; } = 0;
        public byte TB2 { get; set; } = 0;

        public bool HasCull { get; set; } = false;
        
        public Prop(string PropName)
        {
            Name = PropName;
        }

        public void SetRotation(Matrix NewRotation)
        {
            Orientation = NewRotation;
        }

        public void SetRotation(Vector3 NewAngles)
        {
            SetRotation(NewAngles.X, NewAngles.Y, NewAngles.Z);
        }

        public void SetRotation(float X, float Y, float Z)
        {

            Orientation = Matrix.Identity;
            
            Quaternion rot = Quaternion.Identity;
            
            rot *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.ToRadians(Z));
            rot *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), MathHelper.ToRadians(Y));
            rot *= Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.ToRadians(X));

            Orientation = Matrix.CreateFromQuaternion(rot);
            
            Angles = new Vector3(X,Y,Z);
        }
    }
}
