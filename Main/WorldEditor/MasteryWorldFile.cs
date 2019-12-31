using Main.Structures;
using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.WorldEditor
{
    public class MasteryWorldFile
    {
        public string WorldName;
        
        public Vector3 PlayerPosition { get; set; } = new Vector3();
        public SettingsContainer Settings { get; set; } = new SettingsContainer();
        
        public List<WorldFile> ActiveWorldFiles { get; private set; } = new List<WorldFile>();

        public List<Prop> Props { get; private set; } = new List<Prop>();
        public List<Structure> Structures { get; private set; } = new List<Structure>();
        
        public MasteryWorldFile()
        {
            
        }

        public MasteryWorldFile(string Name)
        {
            WorldName = Name;
            Update();
        }
        
        public void Close()
        {

        }

        public void Update()
        {
            ImportActiveFiles();
            RemoveInactiveFiles();
        }

        public void AddWorldFile(WorldFile WorldFile)
        {
            ActiveWorldFiles.Add(WorldFile);
        }
        
        public void AddProp(Prop Prop)
        {
            Props.Add(Prop);
        }

        public void ImportActiveFiles()
        {
            DirectoryInfo folder = new DirectoryInfo("WorldFiles/" + WorldName);

            if (folder.Exists)
            {
                FileInfo[] ConfigFiles = folder.GetFiles();

                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Extension.Equals(".WOF"))
                    {
                        if (FileWithinRange(File))
                        {
                            ActiveWorldFiles.Add( new WorldFile(File));
                        }
                    }
                }
            }
        }       
        
        public void RemoveInactiveFiles()
        {
           
        }

        public WorldFile GetWorldFile(int IDX, int IDY, int LOD)
        {
            List<WorldFile> ValidWorldFiles = ActiveWorldFiles.Where(x => x.IDX == IDX && x.IDY == IDY).ToList();
            
            if (ValidWorldFiles.Count == 0)
            {
                return null;
            }

            if (LOD == -1)
            {
                //find lowest LOD
                return ValidWorldFiles.OrderBy(x => x.LODID).First();
            }
            else
            {
                //find LOD
                return ValidWorldFiles.Where(x => x.LODID == LOD).FirstOrDefault();
            }
        }
        
        public bool HasWorldFile(int IDX, int IDY)
        {
            for (int i = 0; i < ActiveWorldFiles.Count; i++)
            {
                if (ActiveWorldFiles.ElementAt(i).IDX == IDX && ActiveWorldFiles.ElementAt(i).IDY == IDY)
                {
                    return true;
                }
            }
            return false;
        }

        public Prop GetProp(string Name, int InstanceID)
        {
            return Props.Where(x=> x.Name == Name).ToList().ElementAt(InstanceID);
        }
        
        public bool HasWorldFile(WorldFile WordFile)
        {
            return ActiveWorldFiles.Contains(WordFile);
        }

        public List<WorldFile> LoadWorldFileChunk(int IDX, int IDY)
        {

            for (int i = 0; i < Settings.ChunkLODCount; i++)
            {
                //check for file
                FileInfo File = new FileInfo("WorldFiles/Chunk-" + IDX + "-" + IDY + "-" + i);
                if (File.Exists)
                {
                    FileStream FileStream = new FileStream(File.FullName, FileMode.Open);
                    //readFile
                    
                }
            }
            return new List<WorldFile>();
        }

        private bool FileWithinRange(FileInfo File)
        {
            FileStream FileStream = new FileStream(File.FullName, FileMode.Open);

            byte[] WorldPositionData = new byte[12];
            byte[] BoundaryCount = new byte[4];
            
            FileStream.Read(WorldPositionData, 0, 12);
            FileStream.Read(WorldPositionData, 11, 4);

            Vector3 WorldPosition = new Vector3( BitConverter.ToSingle(WorldPositionData, 0), BitConverter.ToSingle(WorldPositionData, 4), BitConverter.ToSingle(WorldPositionData, 8));
            List<Vector2> BoundaryPoints = new List<Vector2>();
            
            int BoundaryPointCount = BitConverter.ToInt32(BoundaryCount, 0);
            int ByteCount = 0;
            byte[] DataX = new byte[4];
            byte[] DataY = new byte[4];
            for (int i = 0; i < BoundaryPointCount; i++)
            {
                FileStream.Read(DataX, ByteCount, 4);
                FileStream.Read(DataY, ByteCount + 4, 4);
                
                BoundaryPoints.Add( new Vector2( BitConverter.ToSingle(DataX, 0) + WorldPosition.X, BitConverter.ToSingle(DataY, 0) + WorldPosition.Y));
                ByteCount += 8;
            }
            return WithinRange(PlayerPosition, BoundaryPoints);
        }

        private bool WithinRange( Vector3 Position, List<Vector2> BoundaryPoints )
        {
            for (int i = 0; i < BoundaryPoints.Count; i += 3)
            {
                if (BoundaryPoints[i].X - PlayerPosition.X < Settings.LOD[Settings.LOD.Length-1] || BoundaryPoints[i].Y - PlayerPosition.Y < Settings.LOD[Settings.LOD.Length - 1])
                {
                    return true;
                }
            }
            return false;
        }


    }
}
