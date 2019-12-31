using Main.Main;
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
    public class FileLoaderButton : Button
    {
        public string FileName { get; set; } = "";

        public string StartFolder { get; set; } = "";

        public FileLoaderButton()
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
            base.OnClick(MX, MY);
            if (HasFocus)
            {
                LoadFile();
            }
        }

        private void LoadFile()
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Filter = "All files (*.*)|*.*";
            FileDialog.FilterIndex = 2;
            DirectoryInfo DirectoryFolder = new DirectoryInfo(StartFolder);
            FileDialog.InitialDirectory = DirectoryFolder.FullName;
            FileDialog.Title = "Open File";
            FileDialog.Multiselect = true;

            DialogResult result = FileDialog.ShowDialog(); // Show the dialog.   
            if (result == DialogResult.OK) // Test result.
            {
                FileName = FileDialog.SafeFileName.Remove(FileDialog.SafeFileName.Length-4,4);
                FileDialog.Dispose();
            }
            else
            {
                return;
            }
        }

    }
}
