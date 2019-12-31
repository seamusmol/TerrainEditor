using Main.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Main.Menu
{
    class ImageLoaderButton : Button
    {
        Bitmap Image;

        public ImageLoaderButton()
        {
        }

        public override object[] GetValue()
        {
            return new object[] { Image };
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
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
            FileDialog.Title = "Open Image";
            DialogResult Result = FileDialog.ShowDialog();
            
            if (Result == DialogResult.OK)
            {
                using (System.IO.Stream Stream = FileDialog.OpenFile())
                {
                    byte[] img = new byte[Stream.Length];
                    Stream.Read(img, 0, img.Length);

                    Image = new Bitmap(Stream);
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
