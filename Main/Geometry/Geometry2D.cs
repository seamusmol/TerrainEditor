using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Main
{
    public class Geometry2D : ICloneable
    {
        public string Name { get; set; }
        public bool HasCull { get; set; } = false;
        public string TextureName { get; set; } = "MapBackDrop";
        public float PX { get; set; }
        public float PY { get; set; }
        public float SX { get; set; }
        public float SY { get; set; }
        public float TX { get; set; }
        public float TY { get; set; }
        public float TSX { get; set; }
        public float TSY { get; set; }
        public float Z { get; set; }

        public Effect Shader;

        public Geometry2D(string name, string textureName, float px, float py, float sx, float sy, float tx, float ty, float tsx, float tsy, float z)
        {
            Name = name;
            TextureName = textureName;
            PX = px;
            PY = py;
            SX = sx;
            SY = sy;
            TX = tx;
            TY = ty;
            TSX = tsx;
            TSY = tsy;
            Z = z;
        }
        
        public Geometry2D(string GeomName)
        {
            Name = GeomName;
        }

        public virtual object Clone()
        {
            Geometry2D NewGeom = new Geometry2D(Name);
            NewGeom.HasCull = HasCull;
            NewGeom.TextureName = (string)TextureName.Clone();
            NewGeom.PX = PX;
            NewGeom.PY = PY;
            NewGeom.SX = PX;
            NewGeom.SY = PY;
            NewGeom.TX = TX;
            NewGeom.TY = TY;
            NewGeom.TSX = TSX;
            NewGeom.TSY = TSY;
            NewGeom.Z = Z;
            return NewGeom;
        }

    }


}
