using Main.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Main.PatternEditor
{
    public static class PatternIOUtil
    {

        public static void Export(string Name, int[,] Voxels)
        {
            //name
            //VX,VY
            //Voxels
            
            object[] CompressedVoxels = CompressionUtil.Array2DToCompressedArrays(Voxels);
            List<int> Quantities = (List<int>)CompressedVoxels[0];
            List<int> Value = (List<int>)CompressedVoxels[0];
            int VoxelLength = Quantities.Count;

            
            BinaryWriter Writer;
            try
            {
                string[] Bits = Name.Split(new char[] { ':', '\\', '/' });
                Name = Bits[Bits.Length - 1];

                if (Name.Contains(".vxp"))
                {
                    Name = Name.Replace(".vxp", "");
                }
                File.Delete(Name);
                Writer = new BinaryWriter(new FileStream("VoxelPatterns/" + Name + ".vxp", FileMode.Create));
                
                Writer.Write(Voxels.GetLength(0));
                Writer.Write(Voxels.GetLength(1));
                Writer.Write(VoxelLength);

                Writer.Write(CompressionUtil.ListToBytes(Quantities));
                Writer.Write(CompressionUtil.ListToBytes(Value));

                Writer.Close();
                Writer.Dispose();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static int[,] Import(string Name)
        {
            if (!File.Exists("VoxelPatterns/" + Name + ".vxp"))
            {
                return null;
            }


            byte[] Data = null;
            try
            {
                Data = File.ReadAllBytes("VoxelPatterns/" + Name + ".vxp");
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
            }

            int ByteCount = 0;
            
            int VX = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            int VY = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;

            int DataLength = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;

            return CompressionUtil.UnPackToIntArray(VX,VY, Data, ByteCount, DataLength);
        }

    }
}
