using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Main;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Menu
{
    public class ToggleButton : Button
    {
        public bool IsToggled { get; set; } = false;
        public ToggleButton()
        {

        }
        
        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data,Width,Height,Shaders);
            
        }

        public override object[] GetValue()
        {
            return new object[]{ IsToggled};
        }

        public void Toggle(bool Val)
        {
            IsToggled = Val;
            if (IsToggled)
            {
                foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
                {
                    entry.Value.TextureName = OnFocusTextureName;
                }
            }
            else
            {
                foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
                {
                    entry.Value.TextureName = TextureName;
                }
            }

        }

        public override void OnClick(int MX, int MY)
        {
            base.OnClick(MX, MY);
            if (HasFocus)
            {
                IsToggled = !IsToggled;
            }

            if (IsToggled)
            {
                foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
                {
                    entry.Value.TextureName = OnFocusTextureName;
                }
            }
            else
            {
                foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
                {
                    entry.Value.TextureName = TextureName;
                }
            }
        }

    }
}
