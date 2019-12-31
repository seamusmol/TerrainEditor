using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO.Compression;
using Main.Util;


namespace Main.WorldManager
{
    
    public class WorldMap
    {
        public int[,] HeightMap { get; set; } // 1 meter resolution n+1
        public int[,] MaterialMap { get; set; } // 8 meter resolution 
        public int[,] StructureMap { get; set; }// 16 meter resolution
        public bool[,] ValidBuildMap { get; set; }//1 meter resolution

        public string Name { get; set; }
        public int WorldSize { get; set; }
        public int StructureSize { get; set; }
        public int MaterialSize { get; set; }
        
        public WorldMap(string worldName, int worldSize, int structureSize, int materialSize)
        {
            Name = worldName;
            WorldSize = worldSize;
            StructureSize = structureSize;
            MaterialSize = materialSize;
           
            HeightMap = new int[WorldSize + 1, WorldSize + 1];
            MaterialMap = new int[WorldSize, WorldSize];
            StructureMap = new int[WorldSize / StructureSize, WorldSize / StructureSize];
            ValidBuildMap = new bool[WorldSize, WorldSize];

            LoadHeightMap();
            GenerateValidBuildingMap();
        }

        
        public WorldMap(string WorldName, bool LoadMaps)
        {
            Name = WorldName;

            bool HasImport = ImportMap();

            if (LoadMaps && HasImport)
            {
                LoadHeightMap();
                GenerateValidBuildingMap();
                GenerateStructureMap();
            }
        }
        
        
        public void SetValue(string MapName, int Value, int PX, int PY)
        {
            switch (MapName)
            {
                case "Material":

                    if (ValidBuildMap[PX * MaterialSize, PY * MaterialSize])
                    {
                        for (int i = 0; i < MaterialSize; i++)
                        {
                            for (int j = 0; j < MaterialSize; j++)
                            {
                                MaterialMap[PX * MaterialSize + i, PY * MaterialSize + j] = Value;
                            }
                        }
                        DrawOnMap(PX * MaterialSize, PY * MaterialSize, Value, MapName, MaterialSize);
                    }
                    break;

                case "Structure":
                    //
                    if (ValidBuildMap[PX * StructureSize, PY * StructureSize])
                    {
                        StructureMap[PX, PY] = Value;
                        DrawOnMap(PX * StructureSize, PY * StructureSize, Value, MapName, StructureSize);
                    }
                    break;
                    
            }
        }
        
        public void ExportMap()
        {
            object[] CompressedHeightMap = CompressionUtil.Array2DToCompressedArrays(HeightMap);
            object[] CompressedMaterialMap = CompressionUtil.Array2DToCompressedArrays(MaterialMap);
            object[] CompressedStuctureMap = CompressionUtil.Array2DToCompressedArrays(StructureMap);

            List<int> HeightMapQuantities = (List<int>)CompressedHeightMap[0];
            List<int> MaterialMapQuantities = (List<int>)CompressedHeightMap[0];
            List<int> StructureMapQuantities = (List<int>)CompressedHeightMap[0];

            List<int> HeightMapValues = (List<int>)CompressedHeightMap[1];
            List<int> MaterialMapValues = (List<int>)CompressedHeightMap[1];
            List<int> StructureMapValue = (List<int>)CompressedHeightMap[1];

            int HeightMapLength = HeightMapQuantities.Count;
            int MaterialMapLength = HeightMapQuantities.Count;
            int StructureMapLength = HeightMapQuantities.Count;
            
            bool HasTerrainData = false;
            
            BinaryWriter Writer;
            try
            {
                File.Delete( "WorldMap/" + Name + "/WordSave.dat");
                Writer = new BinaryWriter( new FileStream("WorldMap/" + Name + "/WorldSave.dat", FileMode.Create));
                Writer.Write(HeightMapLength);
                Writer.Write(MaterialMapLength);
                Writer.Write(StructureMapLength);
                Writer.Write(WorldSize);
                Writer.Write(StructureSize);
                Writer.Write(MaterialSize);

                Writer.Write(CompressionUtil.ListToBytes(HeightMapQuantities));
                Writer.Write(CompressionUtil.ListToBytes(MaterialMapQuantities));

                Writer.Write(CompressionUtil.ListToBytes(MaterialMapQuantities));
                Writer.Write(CompressionUtil.ListToBytes(MaterialMapValues));

                Writer.Write(CompressionUtil.ListToBytes(StructureMapQuantities));
                Writer.Write(CompressionUtil.ListToBytes(StructureMapValue));

                Writer.Write(HasTerrainData);

                Writer.Close();
                Writer.Dispose();
            }
            catch (IOException e)
            {
            }

        }

        public bool ImportMap()
        {
            if (!File.Exists("WorldMap/" + Name + "/WorldSave.dat"))
            {
                return false;
            }

            // List<byte> Data = new List<byte>();
            byte[] Data = null;
            try
            {
                Data = File.ReadAllBytes("WorldMap/" + Name + "/WorldSave.dat");
            }
            catch (IOException e)
            {

            }

            int ByteCount = 0;

            int HeightMapLength = BitConverter.ToInt32( Data, 0);
            ByteCount += 4;

            int MaterialMapLength = BitConverter.ToInt32( Data, ByteCount);
            ByteCount += 4;

            int StructureMapLength = BitConverter.ToInt32( Data, ByteCount);
            ByteCount += 4;

            WorldSize = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;

            StructureSize = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;

            MaterialSize = BitConverter.ToInt32(Data, ByteCount);
            ByteCount += 4;
            
            HeightMap = CompressionUtil.UnPackToIntArray(WorldSize+1, Data, ByteCount, HeightMapLength);
            ByteCount += HeightMapLength * 8;
            MaterialMap = CompressionUtil.UnPackToIntArray(WorldSize, Data, ByteCount, MaterialMapLength);
            ByteCount += MaterialMapLength * 8;
            StructureMap = CompressionUtil.UnPackToIntArray(WorldSize/StructureSize, Data, ByteCount, StructureMapLength);
            ByteCount += StructureMapLength * 8;
            
            bool HasTerrainData = Convert.ToBoolean( Data[ByteCount]);
            return true;
        }

        public void LoadHeightMap()
        {
            FileStream FileStream = new FileStream("WorldMap/" + Name + "/HeightMap.png", System.IO.FileMode.Open);
            Bitmap HeightBitmap = new Bitmap(FileStream);
            FileStream.Close();
            FileStream.Dispose();
            
            for (int i = 0; i < HeightMap.GetLength(0); i++)
            {
                for (int j = 0; j < HeightMap.GetLength(1); j++)
                {
                    HeightMap[i, j] = HeightBitmap.GetPixel(i, j).R;
                }
            }
            HeightBitmap.Dispose();
        }
        public void GenerateValidBuildingMap()
        {
            int WaterHeight = 32;
            ValidBuildMap = new bool[HeightMap.GetLength(0) - 1, HeightMap.GetLength(1) - 1];

            for (int i = 0; i < StructureMap.GetLength(0); i++)
            {
                for (int j = 0; j < StructureMap.GetLength(1); j++)
                {
                    bool IsValid = true;
                    for (int x = 0; x < StructureSize; x++)
                    {
                        for (int y = 0; y < StructureSize; y++)
                        {
                            if (HeightMap[i * StructureSize + x, j * StructureSize + y] <= WaterHeight)
                            {
                                IsValid = false;
                                break;
                            }
                        }
                    }

                    if (IsValid)
                    {
                        for (int x = 0; x < StructureSize; x++)
                        {
                            for (int y = 0; y < StructureSize; y++)
                            {
                                ValidBuildMap[i * StructureSize + x, j * StructureSize + y] = true;
                            }
                        }
                    }
                }
            }

            Bitmap ValidBuildingBitmap = new Bitmap(HeightMap.GetLength(0) - 1, HeightMap.GetLength(1) - 1);

            LockBitMap ValidBuildingLockBitMap = new LockBitMap(ValidBuildingBitmap);
            ValidBuildingLockBitMap.LockBits();

            Color ValidColor = Color.FromArgb(128, 0, 255, 0);
            Color InValidColor = Color.FromArgb(128, 255, 0, 0);

            for (int i = 0; i < ValidBuildMap.GetLength(0); i++)
            {
                for (int j = 0; j < ValidBuildMap.GetLength(1); j++)
                {
                    //ValidBuildingBitmap.SetPixel(i, j, ValidBuildMap[i, j] ? ValidColor : InValidColor);
                    ValidBuildingLockBitMap.SetPixel(i, j, ValidBuildMap[i, j] ? ValidColor : InValidColor);
                }
            }
            ValidBuildingLockBitMap.UnlockBits();

            FileInfo File = new FileInfo("WorldMap/" + Name + "/ValidBuilding.png");
            ValidBuildingBitmap.Save(File.FullName);
            ValidBuildingBitmap.Dispose();
        }
        
        public void GenerateStructureMap()
        {
            DirectoryInfo Folder = new DirectoryInfo(Name);
            FileInfo StructureFile = new FileInfo("WorldMap/" + Name + "/Structure.png");
            Bitmap NewStructureMap = new Bitmap(HeightMap.GetLength(0), HeightMap.GetLength(1));
            
            LockBitMap StructureLockBitMap = new LockBitMap(NewStructureMap);
            StructureLockBitMap.LockBits();

            DirectoryInfo TextureAtlasFolder = new DirectoryInfo("GFX");
            Bitmap TextureAtlas = new Bitmap(TextureAtlasFolder.FullName + "/StructureTiles.png");
            LockBitMap AtlasLookup = new LockBitMap( TextureAtlas);
            AtlasLookup.LockBits();

            int AtlasX = 8;
            int AtlasY = 32;
            int TexSize = TextureAtlas.Width;
            int TexSizeX = (TextureAtlas.Width / AtlasX);
            int TexSizeY = (TextureAtlas.Width / AtlasX);
            
            for (int i = 0; i < StructureMap.GetLength(0); i++)
            {
                for (int j = 0; j < StructureMap.GetLength(1); j++)
                {
                    int StructureID = StructureMap[i, j];
                    
                    for (float x = 0; x < StructureSize; x++)
                    {
                        for (float y = 0; y < StructureSize; y++)
                        {
                            int tx = (int)(x / StructureSize * TexSizeX) + TexSizeX * (StructureID / AtlasY);
                            int ty = (int)(y / StructureSize * TexSizeY) + TexSizeY * (StructureID % AtlasY);

                            //NewStructureMap.SetPixel((int)(i*StructureSize + x), (int)(j * StructureSize + y), TextureAtlas.GetPixel(tx, ty));
                            StructureLockBitMap.SetPixel((int)(i * StructureSize + x), (int)(j * StructureSize + y), AtlasLookup.GetPixel(tx, ty));
                        }
                    }
                }
            }
            StructureLockBitMap.SetPixel(0,0, Color.Red);

            StructureLockBitMap.UnlockBits();
            AtlasLookup.UnlockBits();
            
            if (StructureFile.Exists)
            {
                File.Delete(StructureFile.FullName);
            }

            NewStructureMap.Save(StructureFile.FullName);
            NewStructureMap.Dispose();
        }
        

        public void DrawOnMap(int PX, int PY, int Value, string MapName, int TileScale)
        {
            DirectoryInfo Folder = new DirectoryInfo(Name);
            FileInfo StructureFile = new FileInfo("WorldMap/" + Name + "/" + MapName + ".png");
            
            FileStream FileStream = new FileStream(StructureFile.FullName, System.IO.FileMode.Open);
            Bitmap StructureBitmap = new Bitmap(FileStream);
            FileStream.Close();
            FileStream.Dispose();
            LockBitMap StructureLockBitMap = new LockBitMap(StructureBitmap);
            StructureLockBitMap.LockBits();

            DirectoryInfo TextureAtlasFolder = new DirectoryInfo("GFX");
            Bitmap TextureAtlas = new Bitmap(TextureAtlasFolder.FullName + "/" + MapName + "Tiles.png");
            LockBitMap TextureLookup = new LockBitMap(TextureAtlas);
            TextureLookup.LockBits();

            int AtlasX = 8;
            int AtlasY = 32;
            int TexSize = TextureAtlas.Width;
            int TexSizeX = (TextureAtlas.Width / AtlasX);
            int TexSizeY = (TextureAtlas.Width / AtlasX);
            
            for (float i = 0; i < TileScale; i++)
            {
                for (float j = 0; j < TileScale; j++)
                {
                    int tx = (int)(i / TileScale * TexSizeX) + TexSizeX * (Value / AtlasY);
                    int ty = (int)(j / TileScale * TexSizeY) + TexSizeY * (Value % AtlasY);
                    
                    //StructureBitmap.SetPixel( (int)(PX + i), (int)(PY + j), TextureAtlas.GetPixel( tx, ty));
                    StructureLockBitMap.SetPixel((int)(PX + i), (int)(PY + j), TextureLookup.GetPixel(tx, ty));
                }
            }
            TextureLookup.UnlockBits();
            StructureLockBitMap.UnlockBits();

            File.Delete(StructureFile.FullName);

            StructureBitmap.Save(StructureFile.FullName);
            StructureBitmap.Dispose();
        }
        
    }
}
