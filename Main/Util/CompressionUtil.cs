using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Util
{
    static class CompressionUtil
    {



        public static bool[,] UnPackToBool(int ChunkSize, byte[] Data, int StartIndex, int Length)
        {
            int size = (int)Math.Sqrt(Length);
            bool[,] Array = new bool[size, size];

            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                int Quantity = BitConverter.ToInt32(Data, StartIndex + i * 4);
                bool Value = BitConverter.ToBoolean(Data, StartIndex + i * 4 + Length * 4);
                for (int j = 0; j < Quantity; j++)
                {
                    Array[count / size, count % size] = Value;
                    count++;
                }
            }
            return Array;
        }

        
        public static int[,] UnPackToIntArray(int ChunkSize, byte[] Data, int StartIndex, int Length)
        {
            int[,] Array = new int[ChunkSize, ChunkSize];
            
            int count = 0;
            for ( int i = 0; i < Length; i++)
            {
                int Quantity = BitConverter.ToInt32(Data, StartIndex + i *  4);
                int Value = BitConverter.ToInt32(Data, StartIndex + i * 4 + Length * 4);
                
                for (int j = 0; j < Quantity; j++)
                {
                    Array[count / ChunkSize, count % ChunkSize] = Value;
                    count++;
                }
            }
            return Array;
        }

        public static int[,] UnPackToIntArray(int SX, int SY, byte[] Data, int StartIndex, int Length)
        {
            int[,] Array = new int[SX, SY];

            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                int Quantity = BitConverter.ToInt32(Data, StartIndex + i * 4);
                int Value = BitConverter.ToInt32(Data, StartIndex + i * 4 + Length * 4);
                
                for (int j = 0; j < Quantity; j++)
                {
                    Array[count / SY, count % SY] = Value;
                    count++;
                }
            }
            return Array;
        }

        public static byte[,] UnPackToByteArray(int ChunkSize, byte[] Data, int StartIndex, int Length)
        {
            byte[,] Array = new byte[ChunkSize, ChunkSize];

            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                int Quantity = BitConverter.ToInt32(Data, StartIndex + i * 4);
                byte Value = Data[StartIndex + i * 4 + Length];

                for (int j = 0; j < Quantity; j++)
                {
                    Array[count / ChunkSize, count % ChunkSize] = Value;
                    count++;
                }
            }
            return Array;
        }

        public static byte[,] UnPackToByteArray(int SX, int SY, byte[] Data, int StartIndex, int Length)
        {
            byte[,] Array = new byte[SX, SY];

            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                int Quantity = BitConverter.ToInt32(Data, StartIndex + i * 4);
                byte Value = Data[StartIndex + i * 4 + Length];

                for (int j = 0; j < Quantity; j++)
                {
                    Array[count / SY, count % SY] = Value;
                    count++;
                }
            }
            return Array;
        }

        public static byte[] ListToBytes(List<int> List)
        {
            byte[] Data = new byte[List.Count * 4];

            byte[] CurrentData = new byte[4];
            for (int i = 0; i < List.Count; i++)
            {
                CurrentData = BitConverter.GetBytes(List[i]);
                Data[i*4] = CurrentData[0];
                Data[i*4+1] = CurrentData[1];
                Data[i*4+2] = CurrentData[2];
                Data[i*4+3] = CurrentData[3];
            }
            return Data;
        }
        
        
        public static int[,] UnPack(List< List<int>> List)
        {
            int size = (int)Math.Sqrt(List[0].Count);
            int[,] Array = new int[size, size];

            int count = 0;
            for (int i = 0; i < List[0].Count; i++)
            {
                int Value = List[0][i];
                for (int j = 0; j < List[1].Count; j++)
                {
                    Array[count / size, count % size] = Value;
                    count++;
                }
            }
            return Array;
        }

        public static object[] Array2DToCompressedArrays(int[,] Array)
        {
            List<int> FlatList = new List<int>();

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    FlatList.Add(Array[i,j]);
                }
            }
            return CompressRLE(FlatList); 
        }

        public static object[] Array2DToCompressedArrays(byte[,] Array)
        {
            List<int> FlatList = new List<int>();

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    FlatList.Add(Array[i, j]);
                }
            }
            return CompressRLE(FlatList);
        }
        
        public static object[] CompressRLE(List<int> List)
        {
            List<int> Values = new List<int>();
            List<int> Quantities = new List<int>();

            int CurrentValue = List[0];
            int CurrentQuantity = 0;

            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] == CurrentValue)
                {
                    CurrentQuantity++;
                }
                else
                {
                    Values.Add(CurrentValue);
                    Quantities.Add(CurrentQuantity);

                    CurrentValue = List[i];
                    CurrentQuantity = 1;
                }

                if (i == List.Count-1 )
                {
                    Values.Add(CurrentValue);
                    Quantities.Add(CurrentQuantity);
                }
            }
            return new object[] { Values, Quantities};
        }
        
        public static object[] CompressRLE(List<byte> List)
        {
            List<byte> Values = new List<byte>();
            List<int> Quantities = new List<int>();

            byte CurrentValue = List[0];
            int CurrentQuantity = 0;

            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] == CurrentValue)
                {
                    CurrentQuantity++;
                }
                else
                {
                    Values.Add(CurrentValue);
                    Quantities.Add(CurrentQuantity);

                    CurrentValue = List[i];
                    CurrentQuantity = 1;
                }

                if (i == List.Count - 1)
                {
                    Values.Add(CurrentValue);
                    Quantities.Add(CurrentQuantity);
                }
            }
            return new object[] { Quantities, Values };
        }
        

    }
}
