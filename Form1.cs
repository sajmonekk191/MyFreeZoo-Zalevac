using Hazdryx.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFreeZooZalevac
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private readonly Color PixelColor = Color.FromArgb(176, 176, 176);
        private Keys AttackKey = Keys.NumPad1;
        private bool Zalevat = false;
        private bool Penize = false;
        private bool Zvirata = false;
        private readonly System.Threading.Timer timer;

        public Form1()
        {
            InitializeComponent();

            timer = new System.Threading.Timer(async _ => await Timer_TickAsync(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        }

        private async Task Timer_TickAsync()
        {
            if ((GetAsyncKeyState(AttackKey) & 0x8000) != 0 && Zalevat)
            {
                var point = FindAndClickPixel(PixelColor);
                if (point != Point.Empty)
                {
                    await ClickAtPoint(point);
                }
            }
        }

        private Dictionary<string, Keys> keyMap = new Dictionary<string, Keys>
        {
            { "Num1", Keys.NumPad1 },
            { "Num2", Keys.NumPad2 },
            { "Num3", Keys.NumPad3 },
            { "Ctrl", Keys.Control },
            { "Alt", Keys.Alt }
        };

        private async Task ClickAtPoint(Point point)
        {
            SetCursorPos(point.X, point.Y - 3);
            await Task.Delay(5);
            mouse_event(MOUSEEVENTF_LEFTDOWN, point.X, point.Y, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, point.X, point.Y, 0, UIntPtr.Zero);
        }

        private Point FindAndClickPixel(Color PixelColor)
        {
            int searchvalueNormal = PixelColor.ToArgb();

            Rectangle rect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap BMP = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
            using (Graphics GFX = Graphics.FromImage(BMP))
            {
                GFX.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }

            using (FastBitmap bitmap = new FastBitmap(BMP))
            {
                for (int i = 0; i < bitmap.Length - 1; i++)
                {
                    int currentPixel = bitmap.GetI(i);
                    if (currentPixel == searchvalueNormal)
                    {
                        return GetPointFromIndex(i, rect.Width);
                    }
                }
            }

            return Point.Empty;
        }
        private Point GetPointFromIndex(int index, int width)
        {
            int row = index / width;
            int col = index % width;
            return new Point(col, row);
        }
        private void ButtonSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ButtonSelector.SelectedItem != null)
            {
                var selectedValue = ButtonSelector.SelectedItem.ToString();
                if (keyMap.ContainsKey(selectedValue))
                {
                    AttackKey = keyMap[selectedValue];
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Zalevat = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Penize = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Zvirata = checkBox3.Checked;
        }
    }
}
