using Main.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Menu
{
    public class TransitionButton : Button
    {
        string TransitionName { get; set; }
        string Text { get; set; } = "";

        public TransitionButton() : base()
        {
        }
        
        public override void OnClick(int MX, int MY)
        {
            base.OnClick(MX, MY);
            if (HasFocus)
            {
                MenuManager.MenuTransition(TransitionName);
            }
        }

        public override void OnFocus(int MX, int MY)
        {
            base.OnFocus(MX, MY);
            if (HasFocus)
            { 
                foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
                {
                    if (entry.Key == Name)
                    {
                        entry.Value.TextureName = "OnFocusMenu";
                    }
                    else
                    {
                        entry.Value.TextureName = "OnFocusText";
                    }
                }
                return;
            }
            foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
            {
                if (entry.Key == Name)
                {
                    entry.Value.TextureName = "StandardMenu";
                }
                else
                {
                    entry.Value.TextureName = "StandardText";
                }
            }
        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);
            TransitionName = Data[11];
            
            for (int i = 12; i < Data.Length; i++)
            {
                Text += Data[i] + " ";
            }
            
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
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            GeometryList.Clear();
            GeometryList.Add(Name, new Geometry2D(Name, "StandardMenu", PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX, TY, TSX, TSY, Z));
            
            if (Text.Length > 0)
            {
                float Buffer = SX * 0.1f;
                float DX = (SX - Buffer) / Text.Length;

                for (int i = 0; i < Text.Length; i++)
                {
                    int Index = Text[i] - 1;
                    string Bit = Text[i].ToString();

                    GeometryList.Add(Name + i, new Geometry2D(Name + i, "StandardText", (PX + Buffer) * ScreenWidth + (DX * i * ScreenWidth), PY * ScreenHeight, DX * ScreenWidth, SY * ScreenHeight, 0.0625f * (Index % 16), 0.0625f * (Index / 16), 0.0625f, 0.0625f, Z + 0.1f));
                }
            }
            NeedsNodeUpdate = true;
        }


    }
}
