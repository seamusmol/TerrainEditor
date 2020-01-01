using Main.Geometry;
using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{

    public class WorldFile
    {
        public string FileName { get; set; }

        public int LODID { get; set; }
        public int IDX { get; set; }
        public int IDY { get; set; }

        public int WSX { get; set; }
        public int WSY { get; set; }
        public int SX { get; set; }
        public int SY { get; set; }
        public float TerrainScale { get; set; }
        public float HeightScale { get; set; } = 0.05f;
        public int TerrainDepth { get; set; } = 100;
        public int MaterialDensity { get; set; } = 2;
        public int WaterColorDensity { get; set; } = 8;
        public int DisplacementDensity = 16;
        //Terrrain
        public int[,] HeightMap { get; private set; }
        //material Map
        public byte[,] MaterialMap { get; private set; }
        public byte[,] SecondaryMaterialMap { get; private set; }
        public byte[,] BlendAlphaMap { get; private set; }
        //Material Alpha Maps
        public byte[,] DecalMaterialMap { get; private set; }
        public byte[,] DecalAlphaMap { get; private set; }
        //Water
        public int[,] WaterHeightMap { get; private set; }
        //Wave Data Map
        public byte[,] WaveLengthMap { get; private set; }
        public byte[,] WaveHeightMap { get; private set; }
        //FlowMap
        public byte[,] FlowXMap { get; private set; }
        public byte[,] FlowYMap { get; private set; }
        public byte[,] FlowBackTimeMap { get; private set; }
        public byte[,] FlowPulseSpeedMap { get; private set; }
        //Has Water Map
        public byte[,] WaterMap { get; private set; }
        //Water Color Map
        public byte[,] WaterColorR { get; private set; }
        public byte[,] WaterColorG { get; private set; }
        public byte[,] WaterColorB { get; private set; }
        public byte[,] WaterColorA { get; private set; }
        //Water Color Data Map 1
        public byte[,] WaterNormalMap { get; private set; }
        public byte[,] WaterFresnelMap { get; private set; }
        public byte[,] FoamRampMap0 { get; private set; }
        public byte[,] WaterColorFalloffMap { get; private set; }
        //Terrain Cover
        public byte[,] CoverHeight { get; private set; }
        public byte[,] MaterialLerp { get; private set; }

        public byte[,] VegetationMaterial { get; private set; }
             
        public bool HasTerrainUpdate { get; set; } = false;
        
        public WorldFile(int LOD, int ChunkIDX, int ChunkIDY, int X, int Y, float Scale)
        {
            LODID = LOD;
            IDX = ChunkIDX;
            IDY = ChunkIDY;

            WSX = X;
            WSY = Y;
            
            SX = (int)Math.Round(X / Scale);
            SY = (int)Math.Round(Y / Scale);
            
            TerrainScale = Scale;
            
            HeightMap = new int[SX + 1, SY + 1];
            MaterialMap = new byte[SX * MaterialDensity, SY * MaterialDensity];

            SecondaryMaterialMap = new byte[SX * MaterialDensity, SY * MaterialDensity];
            BlendAlphaMap = new byte[SX * MaterialDensity, SY * MaterialDensity];
            DecalMaterialMap = new byte[SX * MaterialDensity, SY * MaterialDensity];
            DecalAlphaMap = new byte[SX * MaterialDensity, SY * MaterialDensity];
            
            WaterHeightMap = new int[SX + 1, SY + 1];
            WaveLengthMap = new byte[SX + 1, SY + 1];
            WaveHeightMap = new byte[SX + 1, SY + 1];

            FlowXMap = new byte[SX, SY];
            FlowYMap = new byte[SX, SY];
            FlowBackTimeMap = new byte[SX, SY];
            FlowPulseSpeedMap = new byte[SX, SY];

            WaterMap = new byte[SX, SY];

            FoamRampMap0 = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];
            WaterColorFalloffMap = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];

            WaterColorR = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];
            WaterColorG = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];
            WaterColorB = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];
            WaterColorA = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];

            WaterNormalMap = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];
            WaterFresnelMap = new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1];

            CoverHeight = new byte[SX * DisplacementDensity + 1, SY * DisplacementDensity + 1];
            
            VoxelUtil.ReplaceValue(FlowXMap, 0, 128);
            VoxelUtil.ReplaceValue(FlowYMap, 0, 128);
            
            FileName = ChunkIDX + "-" + ChunkIDY + "-" + LODID;
        }

        public WorldFile(byte[] Data)
        {

            int ByteCount = 0;

            LODID = BitConverter.ToInt32(Data,ByteCount += 4);
            IDX = BitConverter.ToInt32(Data, ByteCount += 4);
            IDY = BitConverter.ToInt32(Data, ByteCount += 4);
            SX = BitConverter.ToInt32(Data, ByteCount += 4);
            SY = BitConverter.ToInt32(Data, ByteCount += 4);
            TerrainScale = BitConverter.ToSingle(Data, ByteCount += 4);
            
            int DataLength0 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength1 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength2 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength3 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength4 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength5 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength6 = BitConverter.ToInt32(Data, ByteCount += 4);
            int DataLength7 = BitConverter.ToInt32(Data, ByteCount += 4);
            
            MaterialMap = CompressionUtil.UnPackToByteArray(SX + 1, SY + 1, Data, ByteCount += 4, DataLength1);
            SecondaryMaterialMap = CompressionUtil.UnPackToByteArray(SX + 1, SY + 1, Data, ByteCount += 4, DataLength0);
            DecalMaterialMap = CompressionUtil.UnPackToByteArray(SX + 1, SY + 1, Data, ByteCount += 4, DataLength0);

            BlendAlphaMap = CompressionUtil.UnPackToByteArray(SX + 1, SY + 1, Data, ByteCount += 4, DataLength0);
            DecalAlphaMap = CompressionUtil.UnPackToByteArray(SX + 1, SY + 1, Data, ByteCount += 4, DataLength0);
            
        }
        
        public byte[] ToBytes()
        {
            List<byte> Data = new List<byte>();
            Data.AddRange( BitConverter.GetBytes(LODID));
            Data.AddRange(BitConverter.GetBytes(IDX));
            Data.AddRange(BitConverter.GetBytes(IDY));
            Data.AddRange(BitConverter.GetBytes(SX));
            Data.AddRange(BitConverter.GetBytes(SY));
            Data.AddRange(BitConverter.GetBytes(TerrainScale));
            
            //object[] CompressedMap0 = CompressionUtil.Array2DToCompressedArrays(HeightMap);
            //Terrian Material Maps
            object[] CompressedMap1 = CompressionUtil.Array2DToCompressedArrays(MaterialMap);
            object[] CompressedMap2 = CompressionUtil.Array2DToCompressedArrays(SecondaryMaterialMap);
            object[] CompressedMap3 = CompressionUtil.Array2DToCompressedArrays(DecalMaterialMap);
            //Terrain MaterialAlpha Maps
            object[] CompressedMap4 = CompressionUtil.Array2DToCompressedArrays(BlendAlphaMap);
            object[] CompressedMap5 = CompressionUtil.Array2DToCompressedArrays(DecalAlphaMap);
            //Water Flow Map
            object[] CompressedMap6 = CompressionUtil.Array2DToCompressedArrays(FlowXMap);
            object[] CompressedMap7 = CompressionUtil.Array2DToCompressedArrays(FlowYMap);
            object[] CompressedMap8 = CompressionUtil.Array2DToCompressedArrays(FlowBackTimeMap);
            object[] CompressedMap9 = CompressionUtil.Array2DToCompressedArrays(FlowPulseSpeedMap);
            //Water Color Map
            object[] CompressedMap10 = CompressionUtil.Array2DToCompressedArrays(WaterColorR);
            object[] CompressedMap11 = CompressionUtil.Array2DToCompressedArrays(WaterColorG);
            object[] CompressedMap12 = CompressionUtil.Array2DToCompressedArrays(WaterColorB);
            object[] CompressedMap13 = CompressionUtil.Array2DToCompressedArrays(WaterColorA);
            //Water Color Data 1
            object[] CompressedMap14 = CompressionUtil.Array2DToCompressedArrays(WaterNormalMap);
            object[] CompressedMap15 = CompressionUtil.Array2DToCompressedArrays(WaterFresnelMap);
            object[] CompressedMap16 = CompressionUtil.Array2DToCompressedArrays(FoamRampMap0);
            object[] CompressedMap17 = CompressionUtil.Array2DToCompressedArrays(WaterColorFalloffMap);
            
            //Data.AddRange(CompressionUtil.ListToBytes( (List<int>)CompressedMap0[0]));
            //Data.AddRange(CompressionUtil.ListToBytes( (List<int>)CompressedMap0[0]));
            
            Data.AddRange( (List<byte>)CompressedMap1[0]);
            Data.AddRange( (List<byte>)CompressedMap1[0]);
            Data.AddRange((List<byte>)CompressedMap2[0]);
            Data.AddRange((List<byte>)CompressedMap2[0]);
            Data.AddRange((List<byte>)CompressedMap3[0]);
            Data.AddRange((List<byte>)CompressedMap3[0]);
            
            return Data.ToArray();
        }
        
        
        /*
        public void UpdateTerrainSize(int NX, int NY, float NS)
        {
            SX = NX;
            SY = NY;
            TerrainScale = NS;
            
            HeightMap = VoxelUtil.MergeFields(new int[SX + 1, SY + 1],  HeightMap);
            VoxelUtil.ReplaceValue(HeightMap, 0, (int)Math.Round(TerrainDepth / HeightScale));

            MaterialMap = VoxelUtil.MergeFields(new byte[SX * MaterialDensity, SY * MaterialDensity], MaterialMap);
            SecondaryMaterialMap = VoxelUtil.MergeFields(new byte[SX * MaterialDensity, SY * MaterialDensity], SecondaryMaterialMap);
            BlendAlphaMap = VoxelUtil.MergeFields(new byte[SX * MaterialDensity, SY * MaterialDensity], BlendAlphaMap);
            DecalMaterialMap = VoxelUtil.MergeFields(new byte[SX * MaterialDensity, SY * MaterialDensity], DecalMaterialMap);
            DecalAlphaMap = VoxelUtil.MergeFields(new byte[SX * MaterialDensity, SY * MaterialDensity], DecalAlphaMap);
            
            WaterHeightMap = VoxelUtil.MergeFields(new int[SX + 1, SY + 1], WaterHeightMap);
            
            FlowXMap = VoxelUtil.MergeFields( VoxelUtil.ReturnReplaceValue(new byte[SX, SY], 0, 128), FlowXMap);
            FlowYMap = VoxelUtil.MergeFields( VoxelUtil.ReturnReplaceValue(new byte[SX, SY], 0, 128), FlowYMap);
            FlowBackTimeMap = VoxelUtil.MergeFields( new byte[SX, SY], FlowBackTimeMap);
            FlowPulseSpeedMap = VoxelUtil.MergeFields(new byte[SX, SY], FlowPulseSpeedMap);

            WaterMap = VoxelUtil.MergeFields( new byte[SX, SY], WaterMap);
            WaveLengthMap = VoxelUtil.MergeFields(new byte[SX + 1, SY + 1], WaveLengthMap);
            WaveHeightMap = VoxelUtil.MergeFields(new byte[SX + 1, SY + 1], WaveHeightMap);

            WaterAlphaMap = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterAlphaMap);
            WaterFresnelMap = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterFresnelMap);
            FoamRampMap0 = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], FoamRampMap0);
            WaterColorFalloffMap = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterColorFalloffMap);
           
            WaterColorR = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterColorR);
            WaterColorG = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterColorG);
            WaterColorB = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterColorB);
            WaterColorA = VoxelUtil.MergeFields(new byte[SX * WaterColorDensity + 1, SY * WaterColorDensity + 1], WaterColorA);
            
            //VoxelUtil.ReturnReplaceValue(new byte[SX, SY], 0, 128);
            //VoxelUtil.ReturnReplaceValue(new byte[SX, SY], 0, 128);

            //VoxelUtil.ReplaceValue(FoamPulseSpeedMap, 0, 128);
            //VoxelUtil.ReplaceValue(FoamPulseOffsetMap, 0, 128);
            
            HasTerrainUpdate = true;
        }
        */

        public void MergeEdges(WorldFile OtherFile, bool IsVertical, string MapName, int MergeMode)
        {
            int[,] IntArray = new int[0, 0];
            byte[,] ByteArray = new byte[0, 0];
            float[,] FloatArray = new float[0, 0];

            object Map = GetMap(MapName);
            object OtherMap = OtherFile.GetMap(MapName);
            if (Map != null && OtherMap != null)
            {
                if (Map.GetType() == IntArray.GetType())
                {
                    TerrainUtil.MergeEdges((int[,])Map, (int[,])OtherMap, IsVertical, MergeMode);
                }
                else if (Map.GetType() == ByteArray.GetType())
                {
                    TerrainUtil.MergeEdges((byte[,])Map, (byte[,])OtherMap, IsVertical, MergeMode);
                }
                else if (Map.GetType() == FloatArray.GetType())
                {
                    
                    TerrainUtil.MergeEdges((float[,])Map, (float[,])OtherMap, IsVertical, MergeMode);
                }
            }
        }
        
        public void ModifyMap(int X1, int X2, int Y1, int Y2, float[,] Q, int Shape, int Tool, bool IsPrimary, int Radius, float Flow, string MapName)
        {
            object Map = GetMap(MapName);
            
            int[,] IntArray = new int[,] { };
            byte[,] ByteArray = new byte[,] { };
            float[,] FloatArray = new float[,] { };

            if (Map.GetType() == IntArray.GetType())
            {
                int[,] Array = (int[,])Map;
                TerrainUtil.ApplySquareBrush( X1, X2, Y1, Y2, Q, Array, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
                
            }
            else if (Map.GetType() == ByteArray.GetType())
            {
                byte[,] Array = (byte[,])Map;
                TerrainUtil.ApplySquareBrush( X1, X2, Y1, Y2, Q, Array, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
            }
            else if (Map.GetType() == FloatArray.GetType())
            {
                float[,] Array = (float[,])Map;
                TerrainUtil.ApplySquareBrush(X1, X2, Y1, Y2, Q, Array, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
                
            }
            HasTerrainUpdate = true;
        }

        //Modifies With HeightAdjustMent
        public void ModifyMap(int X1, int X2, int Y1, int Y2, float[,] Q, int Shape, int Tool, bool IsPrimary, int Radius, float Flow, string MapName, string AdjustMapName, bool AdjustHeight)
        {
            object Map = GetMap(MapName);
            object AdjustmentMap = GetMap(AdjustMapName);
            
            int[,] IntArray = new int[,] { };
            byte[,] ByteArray = new byte[,] { };
            float[,] FloatArrray = new float[,] { };
            
            if (Map.GetType() == IntArray.GetType() && AdjustmentMap.GetType() == IntArray.GetType())
            {
                if (AdjustHeight)
                {
                    int[,] Array = (int[,])Map;
                    int[,] AdjustmentArray = (int[,])AdjustmentMap;

                    TerrainUtil.ApplySquareBrush(X1, X2, Y1, Y2, Q, Array, AdjustmentArray, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
                }
                else
                {
                    int[,] Array = (int[,])Map;
                    int[,] AdjustmentArray = (int[,])AdjustmentMap;

                    int[,] OldArray = (int[,])Array.Clone();

                    //apply difference
                    TerrainUtil.ApplySquareBrush(X1, X2, Y1, Y2, Q, Array, AdjustmentArray, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
                    TerrainUtil.AdjustWaterHeight(OldArray, Array, AdjustmentArray);
                }
            }
            else if (Map.GetType() == FloatArrray.GetType() && AdjustmentMap.GetType() == FloatArrray.GetType())
            {
                if (AdjustHeight)
                {
                    float[,] Array = (float[,])Map;
                    float[,] AdjustmentArray = (float[,])AdjustmentMap;
                    TerrainUtil.ApplySquareBrush(X1, X2, Y1, Y2, Q, Array, AdjustmentArray, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
                }
                else
                {
                    float[,] Array = (float[,])Map;
                    float[,] AdjustmentArray = (float[,])AdjustmentMap;
                    float[,] OldArray = (float[,])Array.Clone();
                    //apply difference
                    TerrainUtil.ApplySquareBrush(X1, X2, Y1, Y2, Q, Array, AdjustmentArray, Shape, Tool, Radius, IsPrimary ? 1 : -1, Flow);
                    TerrainUtil.AdjustWaterHeight(OldArray, Array, AdjustmentArray);
                }
            }
            HasTerrainUpdate = true;
        }
        
        public Vector3 GetLocalCenterPoint()
        {
            return new Vector3(HeightMap.GetLength(0) * TerrainScale * 0.5f, HeightMap.GetLength(1) * TerrainScale * 0.5f, 0);
        }

        public Vector3 GetCenterPoint()
        {
            return new Vector3(IDX * SX * TerrainScale , IDY * SY * TerrainScale, 0) + new Vector3( (HeightMap.GetLength(0)-1) * TerrainScale * 0.5f, (HeightMap.GetLength(1) - 1) * TerrainScale * 0.5f, 0);
        }
        
        public WorldFile(FileInfo File)
        {
            Import(File);
        }

        public void Import(FileInfo File)
        {

        }

        public void Export()
        {

        }
        
        public object GetMap(string MapName)
        {
            int[,] IntArray = new int[,] { };
            byte[,] ByteArray = new byte[,] { };
            float[,] FloatArray = new float[,] { };
            PropertyInfo[] properties = GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == MapName)
                {
                    if (properties[i].GetValue(this).GetType() == IntArray.GetType())
                    {
                        int[,] Array = (int[,])properties[i].GetValue(this);
                        return Array;
                    }
                    else if (properties[i].GetValue(this).GetType() == ByteArray.GetType())
                    {
                        byte[,] Array = (byte[,])properties[i].GetValue(this);
                        return Array;
                    }
                    else if (properties[i].GetValue(this).GetType() == FloatArray.GetType())
                    {
                        float[,] Array = (float[,])properties[i].GetValue(this);
                        return Array;
                    }
                }
            }
            return new int[0, 0] { };
        }
        
        public float GetMapValue(string MapName, int PX, int PY)
        {
            int[,] IntArray = new int[,] { };
            byte[,] ByteArray = new byte[,] { };
            float[,] FloatArray = new float[,] { };
            PropertyInfo[] properties = GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == MapName)
                {
                    if (properties[i].GetValue(this).GetType() == IntArray.GetType())
                    {
                        int[,] Array = (int[,])properties[i].GetValue(this);

                        if (PX >= 0 && PY >= 0 && PX < Array.GetLength(1) && PY < Array.GetLength(1))
                        {
                            return Array[PX , PY];
                        }
                    }
                    else if (properties[i].GetValue(this).GetType() == ByteArray.GetType())
                    {
                        byte[,] Array = (byte[,])properties[i].GetValue(this);

                        if (PX >= 0 && PY >= 0 && PX < Array.GetLength(1) && PY < Array.GetLength(1))
                        {
                            return Array[PX, PY];
                        }
                    }
                    else if (properties[i].GetValue(this).GetType() == FloatArray.GetType())
                    {
                        float[,] Array = (float[,])properties[i].GetValue(this);

                        if (PX >= 0 && PY >= 0 && PX < Array.GetLength(1) && PY < Array.GetLength(1))
                        {
                            return Array[PX, PY];
                        }
                    }
                }
            }
            return -1;
        }
        
        public int GetMapSize(string MapName)
        {
            int[,] IntArray = new int[,] { };
            byte[,] ByteArray = new byte[,] { };
            float[,] FloatArray = new float[,] { };
            PropertyInfo[] properties = GetType().GetProperties();
            
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == MapName)
                {
                    if (properties[i].GetValue(this).GetType() == IntArray.GetType())
                    {
                        int[,] Array = (int[,])properties[i].GetValue(this);
                       
                        return Array.GetLength(0);
                    }
                    else if (properties[i].GetValue(this).GetType() == ByteArray.GetType())
                    {
                        byte[,] Array = (byte[,])properties[i].GetValue(this);
                        return Array.GetLength(0);
                    }
                    else if (properties[i].GetValue(this).GetType() == FloatArray.GetType())
                    {
                        float[,] Array = (float[,])properties[i].GetValue(this);
                        return Array.GetLength(0);
                    }
                }
            }
            return 0;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(IDX * SX * TerrainScale, IDY * SY * TerrainScale, 0);
        }

        public Vector2 GetA()
        {
            return new Vector2(IDX * SX * TerrainScale, IDY * SY * TerrainScale);
        }
        public Vector2 GetB()
        {
            return new Vector2(SX * TerrainScale, SY * TerrainScale) + GetA();
        }
    }
}
