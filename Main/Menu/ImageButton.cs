using Main.Main;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menu
{
    class ImageButton : Button
    {
        
        public ImageButton()
        {

        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            Name = Data[1];

            PX = float.Parse(Data[2]);
            PY = float.Parse(Data[3]);
            SX = float.Parse(Data[4]);
            SY = float.Parse(Data[5]);
            Z = float.Parse(Data[6]);
            TextureName = Data[7];

            ScreenWidth = Width;
            ScreenHeight = Height;
            GenerateGeometry();
        }
        
        public override void GenerateGeometry()
        {
            if (GeometryList.Count > 0)
            {
                GeometryList.Clear();
            }
            GeometryList.Add(Name, new Geometry2D(Name, TextureName, PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, 0, 0, 1.0f, 1.0f, Z));
        }

    }
}
