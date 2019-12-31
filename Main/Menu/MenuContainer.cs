using Main.Main;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menu
{
    public class MenuContainer
    {
        public string Name { get; set; }
        public Dictionary<string, Button> ButtonList { get; set; } = new Dictionary<string, Button>();
        public MenuManager MenuManager;
        public Node2D GuiNode{get;set; }

        public MenuContainer(string name, MenuManager menuManager)
        {
            GuiNode = new Node2D("MenuContainer");
            MenuManager = menuManager;
            Name = name;

            MenuManager.GuiNode.Attach(GuiNode);

        }

        public void UpdateScreenSize(int ScreenWidth, int ScreenHeight)
        {
            foreach (KeyValuePair<string, Button> entry in ButtonList)
            {
                entry.Value.ScreenWidth = ScreenWidth;
                entry.Value.ScreenHeight = ScreenHeight;
                entry.Value.GenerateGeometry();
            }
        }
        
        
        public void Update(Point MousePosition, int ScrollVal, bool HasClick, bool HasTick, MenuManager MenuManager)
        {
            bool HasUpdate = false;
            foreach (KeyValuePair<string, Button> entry in ButtonList)
            {
                if (entry.Value.IsActive)
                {
                    if (!entry.Value.HasTickRate || HasTick)
                    {
                        entry.Value.OnFocus(MousePosition.X, MousePosition.Y);
                        if (HasClick)
                        {
                            entry.Value.OnClick(MousePosition.X, MousePosition.Y);
                        }
                        if (ScrollVal != 0)
                        {
                            entry.Value.OnScroll(MousePosition.X, MousePosition.Y, ScrollVal);
                        }  
                    }   
                }

                if (entry.Value.NeedsNodeUpdate)
                {
                    entry.Value.NeedsNodeUpdate = false;
                    HasUpdate = true;
                }
            }

            if (HasUpdate)
            {
                GuiNode.DetachAllGeometries();
                AttachToGUI();
            }
        }

        public void DetachButtonGeometry()
        {
            foreach (KeyValuePair<string, Button> entry in ButtonList)
            {
                
            }
        }

        public void DetachFromGUI()
        {
            foreach (KeyValuePair<string, Button> entry in ButtonList)
            {
                foreach (KeyValuePair<string, Geometry2D> geometryEntry in entry.Value.GeometryList)
                {
                    GuiNode.DetachGeometry(geometryEntry.Value);
                }
            }
        }

        public void AttachToGUI()
        {
            foreach (KeyValuePair<string, Button> entry in ButtonList)
            {
                foreach (KeyValuePair<string, Geometry2D> geometryEntry in entry.Value.GeometryList)
                {
                    GuiNode.Attach(geometryEntry.Value);
                }
            }
        }
        
        public void Attach(Button NewButton)
        {
            if (!ButtonList.ContainsValue(NewButton))
            {
                ButtonList.Add(NewButton.Name, NewButton);
            }
        }
        public void Remove(Button RemovalButton)
        {
            if (ButtonList.ContainsValue(RemovalButton))
            {
                ButtonList.Remove(RemovalButton.Name);
            }

            foreach (KeyValuePair<string, Geometry2D> entry in RemovalButton.GeometryList)
            {
                GuiNode.DetachGeometry(entry.Value);
            }
        }
        
    }
}
