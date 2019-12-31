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
    public class ListButton : Button
    {
        List<string> Lines = new List<string>();
        List<string> ValidLines = new List<string>();

        public string ValidSearch { get; private set; } = "";

        int MaxLineCount = 0;
        int CurrentIndex = 0;
        int CurrentLocalIndex = 0;
        
        public ListButton()
        {

        }

        public override object[] GetValue()
        {
            return new object[] { CurrentIndex + CurrentLocalIndex <= Lines.Count-1 ? Lines[CurrentIndex + CurrentLocalIndex] : "" };
        }

        public void MoveList(int Value)
        {
            CurrentLocalIndex += Value;
            CurrentLocalIndex = CurrentLocalIndex > MaxLineCount-1 ? MaxLineCount - 1 : CurrentLocalIndex;
            CurrentLocalIndex = CurrentLocalIndex > Lines.Count - 1 ? Lines.Count - 1 : CurrentLocalIndex;

            CurrentLocalIndex = CurrentLocalIndex <= 0 ? 0 : CurrentLocalIndex;
            UpdateGeometry();
        }

        public void SetText(List<string> NewList)
        {
            Lines = NewList;
            CurrentIndex = 0;
            UpdateGeometry();
        }

        public void SetValidSearch(string ValidLine)
        {
            ValidSearch = ValidLine;

            ValidLines.Clear();
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].Contains(ValidLine))
                {
                    ValidLines.Add(Lines[i]);
                }
            }

            UpdateGeometry();
        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);

            MaxLineCount = int.Parse(Data[11]);
            
            ScreenWidth = Width;
            ScreenHeight = Height;

            GenerateGeometry();
        }


        //mode Arrows to seperate button
        public override void OnClick(int MX, int MY)
        {
            //up
            if (HasFocus)
            {
                int NewLocalIndex = (int)(((MY - (PY * ScreenHeight)) / (SY * ScreenHeight)) * MaxLineCount);
                CurrentLocalIndex = NewLocalIndex < Lines.Count ? NewLocalIndex : CurrentLocalIndex;
                UpdateGeometry();
            }
        }

        public override void OnFocus(int MX, int MY)
        {
            base.OnFocus(MX, MY);
            if (HasFocus)
            {

            }
        }

        public override void UpdateGeometry()
        {   
            GeometryList.Clear();
            GeometryList.Add(Name + "Background", new Geometry2D(Name + "StandardMenu", "StandardMenu", PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX,TY,TSX,TSY, Z));

            if (Lines.Count > 0)
            {
                float LetterGap = 0.5f;
                float DY = SY / MaxLineCount;
                float DX = DY;

                for (int i = CurrentIndex; i < CurrentIndex + MaxLineCount && i < Lines.Count; i++)
                {
                    string Line = Lines[i];


                    float AllowedLength = SX / (DX * LetterGap);

                    for (int j = 0; j < AllowedLength && j < Line.Length; j++)
                    {

                        //get text texcoord 
                        int Index = Line[j] - 1;
                        GeometryList.Add(Name + i + "-" + j, new Geometry2D(Name + i + "-" + j, "StandardText", (PX + DX * LetterGap * j) * ScreenWidth, (PY + DY * i) * ScreenHeight, DX * ScreenWidth, DY * ScreenHeight, 0.0625f * (Index % 16), 0.0625f * (Index / 16), 0.0625f, 0.0625f, Z + 0.1f));

                        //GeometryList.Add(Name + i, new Geometry2D(Name + i, "StandardText", (PX + DX * j + Buffer / 2) * ScreenWidth + (DX * i * ScreenWidth), (PY + OY) * ScreenHeight, DX * (2.0f - LetterGap) * ScreenWidth, DY * ScreenHeight, 0.0625f * (Index % 16), 0.0625f * (Index / 16), 0.0625f, 0.0625f, Z + 0.1f));

                    }
                }

                GeometryList.Add(Name + "HighLight", new Geometry2D(Name + "HighLight", "StandardMenu", PX * ScreenWidth, (PY + (SY / MaxLineCount * CurrentLocalIndex)) * ScreenHeight, SX * ScreenWidth, SY / MaxLineCount * ScreenHeight, 0.75f, 0, 0.1875f, 0.0625f, Z));
            }
            NeedsNodeUpdate = true;
        }

    }
}
