using Main.Main;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menu
{
    public class LayeredImageButton : Button
    {
        public LayeredImageButton() : base()
        {

        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);

            ScreenWidth = Width;
            ScreenHeight = Height;
        }
        
        public void ToggleLayer(string LayerName)
        {
            if (GeometryList.ContainsKey(LayerName))
            {
                GeometryList[LayerName].HasCull = !GeometryList[LayerName].HasCull;
            }
        }
        
        public void AddLayer(string LayerName, string LayerTexture)
        {
            //geometry
            if (GeometryList.ContainsKey(LayerName))
            {
                GeometryList.Remove(LayerName);
            }
            GeometryList.Add( LayerName, new Geometry2D(LayerName, LayerTexture, PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX, TY, TSX, TSY, Z));
        }

        public override void GenerateGeometry()
        {
        }

        public void RemoveLayer(string LayerName)
        {
            
            GeometryList.Remove(LayerName);

        }
        
    }
}
