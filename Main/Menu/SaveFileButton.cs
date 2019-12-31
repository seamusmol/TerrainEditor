using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main.Menu
{
    class SaveFileButton : Button
    {
        public string FileName { get; set; } = "";
        public string StartFolder { get; set; } = "WorldMap";

        public SaveFileButton()
        {

        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);
            StartFolder = Data[11];
        }

        public override object[] GetValue()
        {
            return new object[] { FileName };
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
            SaveFileDialog SaveDialog = new SaveFileDialog();
            DirectoryInfo DirectoryFolder = new DirectoryInfo(StartFolder);

            SaveDialog.InitialDirectory = DirectoryFolder.FullName;

            DialogResult result = SaveDialog.ShowDialog(); // Show the dialog.   

            if (result == DialogResult.OK) // Test result.
            {

                FileName = SaveDialog.FileName;
                
                SaveDialog.Dispose();
            }
            else
            {
                return;
            }
        }
    }
}
