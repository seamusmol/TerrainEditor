using Main.Geometry;
using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Structures
{
    public class Prop : StructureComponent
    {
        GeometryModel Model;

        public Prop(Vector3 PA, Vector3 Rot, Vector3 Scale, GeometryModel PropModel) : base(PA, Rot, Scale)
        {
            Model = PropModel;
        }
        
        public override void GenerateGeometry(RenderManager RenderManager)
        {
            Geom = new Geometry3D(Name);
            Geom.Shader = RenderManager.Shaders["Prop"].Clone();
            Geom.Shader.Parameters["Scale"].SetValue(Scale);
            Geom.Shader.Parameters["MaterialMap"].SetValue(RenderManager.Textures["Prop"]);

            Geom.HasCull = false;
            Geom.RenderBucket = Geometry3D.RenderQueue.Solid;
        }

        public override void UpdateGeometry(RenderManager RenderManager)
        {

            //TODO
            //Add System for 
            

        }

    }
}
