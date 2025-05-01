using System;
using System.Windows;
using System.Drawing;//Color, Bitmap, Graphics
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Runtime.InteropServices; //User32.dll (and dll import)

namespace Pixel_Bot
{

    public partial class MainWindow : Window
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002; //click
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004; //release
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);
        private void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0,0,0,0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0,0,0,0);
        }

        private void DoubleClick(int PosX, int PosY)
        {
            SetCursorPos(PosX, PosY);
            Click();
            System.Threading.Thread.Sleep(250);
            Click();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnButtonSearchPixelClick(object sender, RoutedEventArgs e)
        {
            string inputHexColorCode = TextBoxColor.Text;
            SearchPixel(inputHexColorCode);
        }
        
        private bool SearchPixel(string hexCode)
        {
            //Creating bitmap size of screen
            Bitmap bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            //Creating a new graphics objects that can capture the screen
            Graphics graphics = Graphics.FromImage(bitmap as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            //e.g translate #ffffff to a color object
            Color desiredPixelColor = ColorTranslator.FromHtml(hexCode);

            for(int x = 0; x < SystemInformation.VirtualScreen.Width; x++)
            {
                for(int y = 0; y < SystemInformation.VirtualScreen.Height; y++)
                {
                    //Get the current pixel color
                    Color currentPixelColor = bitmap.GetPixel(x, y);
                    if(desiredPixelColor == currentPixelColor)
                    {
                        MessageBox.Show(String.Format("Found pixel {0},{1} - Now set mouse cursor", x, y));
                        //TODO set mouse cursor and double click
                        DoubleClick(x, y);
                        return true;
                    }
                }
            }
            return false;
        }

        private void TextBoxColor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
