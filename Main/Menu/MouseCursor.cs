using Main.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Main.Menu
{
    public enum CursorState
    {
        Inactive = 0, Menu = 1, CrossHair = 2
    }

    public class MouseCursor
    {
        public float PX { get; set; }
        public float PY { get; set; }
        public float S { get; set; }
        public float TX { get; set; }
        public float TY { get; set; }
        public float TSX { get; set; }
        public float TSY { get; set; }
        public float Z { get; set; }

        public string Name { get; set; }
        public string TextureName { get; set; } = "StandardMenu";

        public bool IsActive = true;
        public Geometry2D Geometry { get; set; }

        public CursorState CurrentCursorState = CursorState.Menu;

        public MouseCursor(float Mous)
        {
            Name = CurrentCursorState.ToString();
            PX = 0.5f;
            PY = 0.5f;
            S = 48;
            TX = 0.0f;
            TY = 0.0625f;
            TSX = 0.0625f;
            TSY = 0.0625f;
            UpdateGeometry();
        }

        
        public void Update(Point MousePosition)
        {
            if (Geometry.Name != CurrentCursorState.ToString())
            {
                Name = CurrentCursorState.ToString();
                TY = 0.0625f * (int)CurrentCursorState;
            }
            
            switch (CurrentCursorState)
            {
                case CursorState.Inactive:
                    Geometry.HasCull = true;
                    IsActive = false;
                    break;
                case CursorState.Menu:
                    IsActive = true;
                    Geometry.HasCull = false;
                    PX = MousePosition.X;
                    PY = MousePosition.Y;
                    Geometry.PX = PX - (S / 2);
                    Geometry.PY = PY - (S / 2);
                    break;
                case CursorState.CrossHair:
                    PX = 0.5f;
                    PY = 0.5f;
                    Geometry.PX = PX - (S / 2);
                    Geometry.PY = PY - (S / 2);
                    IsActive = false;
                    Geometry.HasCull = false;
                    break;
            }
        }
        
        
        private void UpdateGeometry()
        {
            Geometry = new Geometry2D(Name, TextureName, PX - (S/2), PY - (S/2), S, S, TX, TY, TSX, TSY, 1.0f);
        }
    }
}
