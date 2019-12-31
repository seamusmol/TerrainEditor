using Main.Structures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Main.Structures.StructureComponent;

namespace Main.Structures
{
    class StructureGenerator
    {

        //get structure code

        //structure()
        //Single component Prop(Trees, Street Lights, etc.)

        public static List<Structure> GenerateStructureList(int IDX, int IDY, int ChunkSize, int ID)
        {
            List<Structure> StructureList = new List<Structure>();
            
            if (ID == 0)
            {
                return new List<Structure>();
            }
           
            Vector3 ChunkPosition = new Vector3(IDX * ChunkSize, IDY * ChunkSize, 0);
            
            /*
            for (int i = 0; i < StructureList.Count; i++)
            {
                StructureList[i].Position += ChunkPosition;
            }
            */
            return StructureList;
        }
        
        
    }
}
