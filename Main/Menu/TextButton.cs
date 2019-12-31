using Main.Main;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menu
{
    
    public class TextButton : Button
    {
        public string Text;

        public TextButton() : base()
        {

        }

        public override object[] GetValue()
        {
            return new object[] { Text };
        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            for (int i = 11; i < Data.Length; i++)
            {
                Text += Data[i];
            }
           
            ScreenWidth = Width;
            ScreenHeight = Height;

            GenerateGeometry();
            base.Init(Data, Width, Height, Shaders);
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
            GeometryList.Add(Name + "Up", new Geometry2D(Name, "StandardMenu", PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX, TY, TSX, TSY, Z));
            GeometryList.Add(Name + "Down", new Geometry2D(Name, "StandardMenu", PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX, TY, TSX, TSY, Z));

            float Buffer = SX * 0.1f;
            float LetterGap = 0.75f;
            LetterGap = Text.Length > 1 ? LetterGap : 1;
            float DX = (SX - Buffer) / Text.Length;
            float OY = (SY - DX)/2.0f;
            
            float DY = DX;
            DY = DY > SY / 4.0f ? DY : SY / 4.0f;

            for (int i = 0; i < Text.Length; i++)
            {
                int Index = Text[i] - 1;

                string Bit = Text[i].ToString();
                
                /*
                if ( Int32.TryParse(Bit, out int n))
                {
                    Index = 64 + Int32.Parse(Bit);
                }
                */

                GeometryList.Add(Name + i, new Geometry2D(Name + i, "StandardText", (PX + Buffer / 2) * ScreenWidth + (DX * LetterGap * i * ScreenWidth), (PY+OY) * ScreenHeight, DX * (2.0f - LetterGap) * ScreenWidth, DY * ScreenHeight, 0.0625f * (Index % 16), 0.0625f * (Index / 16), 0.0625f, 0.0625f, Z + 0.1f));
            }
            NeedsNodeUpdate = true;
        }
    }
}
