using Main.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Menu
{
    public class FolderLoaderButton : Button
    {
        public string FolderName { get; set; } = "";
        public string StartFolder { get; set; } = "WorldMap";

        public FolderLoaderButton()
        {

        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);
            StartFolder = Data[11];
        }
        
        public override object[] GetValue()
        {
            return new object[] { FolderName };
        }

        public override void OnClick(int MX, int MY)
        {
            base.OnClick(MX,MY);
            if (HasFocus)
            {
                LoadFile();
            }
        }

        private void LoadFile()
        {
            FolderBrowserDialog FolderDialog = new FolderBrowserDialog();
            DirectoryInfo DirectoryFolder = new DirectoryInfo(StartFolder);
            
            FolderDialog.SelectedPath = DirectoryFolder.FullName;
            
            DialogResult result = FolderDialog.ShowDialog(); // Show the dialog.   
            
            if (result == DialogResult.OK) // Test result.
            {
                FolderName = FolderDialog.SelectedPath;
                FolderDialog.Dispose();
            }
            else
            {
                return;
            }
        }

    }
}
