using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Structures
{
    public static class VoxelPatternManager
    {
        public static Dictionary<string, int[]> VoxelPatterns { get; set; } = new Dictionary<string, int[]>();

        public static void LoadPatterns()
        {
            

        }
        
        public static void SavePattern(string Name, int[] Pattern)
        {
            BinaryWriter Writer;
            try
            {
                string[] Bits = Name.Split(new char[] { ':', '\\', '/' });
                Name = Bits[Bits.Length - 1];

                if (Name.Contains(".str"))
                {
                    Name = Name.Replace(".str", "");
                }
                File.Delete(Name);
                Writer = new BinaryWriter(new FileStream("VoxelPatterns/" + Name + ".vxp", FileMode.Create));




                //Writer.Write(Data.ToArray());

                Writer.Close();
                Writer.Dispose();
            }
            catch (IOException e)
            {

            }
        }

    }
}
