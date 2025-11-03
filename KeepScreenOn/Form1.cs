using System.Runtime.InteropServices;

namespace KeepScreenOn
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const byte VK_SHIFT = 0x10;

        static bool enableClicks = false;
        static bool enableKeyboardInput = true;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Text = "Executing...";
            btnStart.Enabled = btnExit.Enabled = false;
            this.WindowState = FormWindowState.Minimized;
            await Task.Run(() => RunMouseMover());
            this.WindowState = FormWindowState.Normal;
            btnStart.Text = "Start";
            btnStart.Enabled = btnExit.Enabled = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RunMouseMover()
        {
            int[,] movementPattern = { { 10, 0 }, { 10, 10 }, { 0, 10 }, { 0, 0 } };
            int step = 0;

            SetCursorPos(0, 0);
            GetCursorPos(out POINT lastPhysicalPos);
            int expectedX = 0, expectedY = 0;

            while (true)
            {
                Thread.Sleep(1000);

                GetCursorPos(out POINT currentPos);

                if (currentPos.X != expectedX || currentPos.Y != expectedY)
                {
                    break;
                }

                expectedX = movementPattern[step % 4, 0];
                expectedY = movementPattern[step % 4, 1];
                SetCursorPos(expectedX, expectedY);

                if (enableClicks && step % 4 == 0)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(50);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                }

                if (enableKeyboardInput && step % 4 == 2)
                {
                    keybd_event(VK_SHIFT, 0, 0, 0);
                    Thread.Sleep(50);
                    keybd_event(VK_SHIFT, 0, 2, 0);
                }

                step++;
            }
        }
    }
}