using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Structures
{
    public class StructureModelManager
    {
        Dictionary<string, Structure> StructureLibrary = new Dictionary<string, Structure>();
        public StructureModelManager()
        {
            ImportStructures();
        }

        public void ImportStructures()
        {
            DirectoryInfo folder = new DirectoryInfo("StructureModels");

            if (folder.Exists)
            {
                FileInfo[] ConfigFiles = folder.GetFiles();

                foreach (FileInfo File in ConfigFiles)
                {
                    if (File.Extension.Equals(".str"))
                    {
                        //FileStream FileStream = new FileStream(File.FullName.ToString(), FileMode.Open);
                        /*
                        string Name = File.Name.Replace(".str","");

                        object[] Data = StructureIOUtil.ImportStructureModel(Name);
                        
                        Vector3 Position = (Vector3)Data[0];
                        List<StructureComponent> Components = (List<StructureComponent>)Data[1];

                        Structure NewStructure = new Structure(Name, Position, Components.ToArray());
                        StructureLibrary.Add(Name, NewStructure);
                        */
                    }
                }
            }
        }
        
        public bool HasStructure(string Name)
        {
            return StructureLibrary.ContainsKey(Name);
        }

        public Structure GetStructure(string Name)
        {
            return StructureLibrary[Name];
        }

    }
}
