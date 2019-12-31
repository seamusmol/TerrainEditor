using Main.Geometry;
using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Main
{
    //TODO
    //Add Child Position/Rotatation inheritance
    public class Node3D
    {
        public string Name { get; private set; }

        public bool HasCull { get; private set; } = false;
        public Dictionary<Node3D, string> Children { get; private set; } = new Dictionary<Node3D, string>();
        public Dictionary<Geometry3D, string> Geometries { get; private set; } = new Dictionary<Geometry3D, string>();
        public int Count { get; set; } = 0;

        public Node3D(string name)
        {
            Name = name;
        }

        public List<Geometry3D> GetAllGeometries()
        {
            List<Geometry3D> ChildGeometries = new List<Geometry3D>();
            GetChildrenGeometries(ChildGeometries);
            
            return ChildGeometries;
        }

        //Applies Render order to Geometries
        public List<Geometry3D> GetAllGeometries(Vector3 Position)
        {
            List<Geometry3D> ChildGeometries = new List<Geometry3D>();
            GetChildrenGeometries(ChildGeometries);
            ChildGeometries = ChildGeometries.OrderBy(x => (int)x.RenderBucket).ThenBy(x => -Vector3.Distance(Position, x.Position)).ToList();
            return ChildGeometries;
        }
        
        public bool HasGeometryNamed(string GeometryName)
        {
            return Geometries.ContainsValue(GeometryName);
        }

        public Node3D GetChildNamed(string ChildName)
        {
            foreach (KeyValuePair<Node3D, string> entry in Children)
            {
                if (entry.Value == ChildName)
                {
                    return entry.Key;
                }
            }
            return null;
        }

        public bool HasGeometry(Geometry3D Geometry)
        {
            return Geometries.ContainsKey(Geometry);
        }
        
        public void GetChildrenGeometries(List<Geometry3D> GeometryList)
        {
            if (!HasCull)
            {
                foreach (KeyValuePair<Geometry3D, string> entry in Geometries)
                {
                    if (!entry.Key.HasCull)
                    {
                        GeometryList.Add(entry.Key);
                    }
                }
                foreach (KeyValuePair<Node3D, string> entry in Children)
                {
                    entry.Key.GetChildrenGeometries(GeometryList);
                }
            }
        }

        /*
        public void AttachRange(List<Node3D> NodeList)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                if (!Children.ContainsKey(NodeList[i]))
                {
                    Children.Add(NodeList[i], NodeList[i].Name);
                }
            }
        }
        */

        public void AttachRange(List<Geometry3D> GeometryList)
        {
            for (int i = 0; i < GeometryList.Count; i++)
            {
                if (!Geometries.ContainsKey(GeometryList[i]))
                {
                    Geometries.Add(GeometryList[i], GeometryList[i].Name);
                }
            }
            Count = Geometries.Count;
        }

        public void Attach(Node3D Node)
        {
            if (!Children.ContainsKey(Node))
            {
                Children.Add(Node, Node.Name);
            }
        }
        
        public void Attach(Geometry3D Geometry)
        {
            if (!Geometries.ContainsKey(Geometry))
            {
                Geometries.Add(Geometry, Geometry.Name);
                Count = Geometries.Count;
            }
        }

        public void DetachNode(Node3D Node)
        {
            if (Children.ContainsKey(Node))
            {
                Children.Remove(Node);
            }
        }
        
        public void DetachGeometry(Geometry3D Geometry)
        {
            if (Geometries.ContainsKey(Geometry))
            {
                Geometries.Remove(Geometry);
                Count = Geometries.Count;
            }
        }
        
        public void DetachAllGeometries()
        {
            Geometries.Clear();
        }

        public void DetachAllChildren()
        {
            Children.Clear();
        }

        public CollisionResults CollideWith(Vector3 Position, Vector3 Direction)
        {
            CollisionResults Results = new CollisionResults();

            List<Geometry3D> HitList = GetAllGeometries();

            for (int i = 0; i < HitList.Count; i++)
            {
                Geometry3D Geom = HitList.ElementAt(i);
                CollisionResults NewResults = Geom.CollideWith(Position, Direction);
                
                if (NewResults.Count > 0)
                {
                    Results.AddRange(NewResults);
                }
            }
            return Results;
        }

    }
}
