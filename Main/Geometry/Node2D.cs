using Main.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Main
{
    public class Node2D
    {
        string Name;

        bool HasCull { get; set; } = false;
        Dictionary<Node2D, string> Children { get; set; } = new Dictionary<Node2D, string>();
        Dictionary<Geometry2D, string> Geometries { get; set; } = new Dictionary<Geometry2D, string>();

        public Node2D(string name)
        {
            Name = name;
        }

        public List<Geometry2D> GetAllGeometries()
        {
            List<Geometry2D> ChildGeometries = new List<Geometry2D>();
            GetChildrenGeometries(ChildGeometries);
            return ChildGeometries;
        }
        
        public void GetChildrenGeometries( List<Geometry2D> GeometryList)
        {
            if (!HasCull)
            {
                foreach ( KeyValuePair<Geometry2D, string> entry in Geometries)
                {
                    if (!entry.Key.HasCull)
                    {
                        GeometryList.Add(entry.Key);
                    }
                }
                foreach (KeyValuePair<Node2D, string> entry in Children)
                {
                    entry.Key.GetChildrenGeometries(GeometryList);
                }
            }
        }

        public void DetachAllGeometries()
        {
            Geometries.Clear();
        }

        public void DetachChildNamed(string ChildName)
        {
            Node2D RemovalKey = null;

            foreach (KeyValuePair<Node2D, string> entry in Children)
            {
                if (entry.Value == ChildName)
                {
                    RemovalKey = entry.Key;
                    break;
                }
            }

            if (RemovalKey != null)
            {
                Children.Remove(RemovalKey);
            }

        }

        public void Attach(Node2D Node)
        {
            if (!Children.ContainsKey(Node))
            {
                Children.Add(Node, Node.Name);
            }
        }

        
        public void Attach(Geometry2D Geometry)
        {
            if (!Geometries.ContainsKey(Geometry))
            {
                Geometries.Add(Geometry, Geometry.Name);
            }
        }
        
        public void DetachNode(Node2D Node)
        {
            if (Children.ContainsKey(Node))
            {
                Children.Remove(Node);
            }
        }

        public void DetachGeometry(Geometry2D Geometry)
        {
            if (Geometries.ContainsKey(Geometry))
            {
                Geometries.Remove(Geometry);
            }

        }
    }
}
