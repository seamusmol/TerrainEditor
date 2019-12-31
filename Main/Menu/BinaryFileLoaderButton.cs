using Main.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main.Menu
{
    public class BinaryFileLoaderButton : Button
    {
        byte[] Data;
        
        public BinaryFileLoaderButton()
        {

        }

        public override object[] GetValue()
        {
            return new object[] { Data };
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
            FileDialog.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
            FileDialog.Title = "Open Image";
            DialogResult result = FileDialog.ShowDialog(); // Show the dialog.   

            if (result == DialogResult.OK) // Test result.
            {
                using (System.IO.Stream Stream = FileDialog.OpenFile())
                {
                    Data = new byte[Stream.Length];
                    Stream.Read(Data, 0, Data.Length);
                    Stream.Close();
                    Stream.Dispose();
                }
                FileDialog.Dispose();
            }
            else
            {
                return;
            }
        }

    }
}
