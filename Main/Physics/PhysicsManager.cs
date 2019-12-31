using Main.Main;
using Main.WorldEditor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Physics
{
    /**
     * 
     * Processes Physics interactions
     * 
     *  Terrain
     *  Props
     *  Structures
     * 
     **/
    public class PhysicsManager : AppState
    {
        public List<PhysicsControl> PhysicsControl = new List<PhysicsControl>();

        public  MasteryWorldFile MasteryFile { get; private set; }
        
        public PhysicsManager()
        {

        }

        public override void Update(GameTime GameTime)
        {
           
        }


    }
}
