using System;
using System.Windows; // WPF
using System.Drawing; // Color, Bitmap, Graphics
using System.Runtime.InteropServices; // User32.dll (and dll import)
using System.Windows.Forms; // Screen height and width
using MessageBox = System.Windows.MessageBox; // Use message box of wpf (not forms)
using System.Diagnostics;

namespace PixelBot
{
    // usecases: clicker bot for game - tell 
    public partial class MainWindow : Window
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);


        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnButtonSearchPixelClick(object sender, RoutedEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("HD-Player");
            IntPtr handle = processes[0].MainWindowHandle;
            RECT Rect = new RECT();
            if (GetWindowRect(handle, ref Rect))
                MoveWindow(handle, 0, 0, Rect.right - Rect.left, Rect.bottom - Rect.top + 50, true);

            // 01. get hex value from input field
            string inputHexColorCode = TextBoxHexColor.Text;
            SearchPixel(inputHexColorCode);
        }

        private bool SearchPixel(string hexcode)
        {
            // Take an image from the screen
            // Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height); // Create an empty bitmap with the size of the current screen 

            Bitmap bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height); // Create an empty bitmap with the size of all connected screen 

            Graphics graphics = Graphics.FromImage(bitmap as Image); // Create a new graphics objects that can capture the screen

            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size); // Screenshot moment → screen content to graphics object

            Color desiredPixelColor = ColorTranslator.FromHtml(hexcode);

            // Go one to the right and then check from top to bottom every pixel (next round -> go one to right and go down again)
            for (int x = 0; x < SystemInformation.VirtualScreen.Width; x++)
            {
                for (int y = 0; y < SystemInformation.VirtualScreen.Height; y++)
                {
                    // Get the current pixels color
                    Color currentPixelColor = bitmap.GetPixel(x, y);

                    // Finally compare the pixels hex color and the desired hex color (if they match we found a pixel)
                    if (desiredPixelColor == currentPixelColor)
                    {
                        MessageBox.Show("Found Pixel - Now set mouse cursor");
                        DoubleClickAtPosition(x, y);
                        return true;
                    }

                }
            }

            MessageBox.Show("Did not find pixel");
            return false;
        }

        private void DoubleClickAtPosition(int posX, int posY)
        {
            SetCursorPos(posX, posY);

            Click();
            System.Threading.Thread.Sleep(250);
            Click();
        }

        private void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }
}
