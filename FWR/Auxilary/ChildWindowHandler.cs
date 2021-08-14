using FWR.Engine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms.Integration;
using System.Drawing;
using System.Drawing.Imaging;

namespace FWR.UI_Aux
{
    public static class ChildWindowHandler
    {
        const int WS_BORDER = 8388608;
        const int WS_DLGFRAME = 4194304;
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;
        const int WS_SYSMENU = 524288;
        const int WS_THICKFRAME = 262144;
        const int WS_MINIMIZE = 536870912;
        const int WS_MAXIMIZEBOX = 65536;
        const long GWL_STYLE = -16L;
        const long GWL_EXSTYLE = -20L;
        const long WS_EX_DLGMODALFRAME = 0x1L;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_FRAMECHANGED = 0x20;
        const uint MF_BYPOSITION = 0x400;
        const uint MF_REMOVE = 0x1000;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private const int WmPaint = 0x000F;

        public static void ForcePaint(IntPtr mainWindowHandle)
        {
            SendMessage(mainWindowHandle, WmPaint, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        public static extern Int64 SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static WindowsFormsHost SetProcessAsChildOfPanelControl(Process processToBeChild, int width, int height, Test test)
        {
            while (processToBeChild?.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Yield();
            }

            var _appWin = processToBeChild.MainWindowHandle;

            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();

            windowsFormsHost.Child = panel;

            SetParent(_appWin, panel.Handle);
            MakeExternalWindowBorderless(processToBeChild.MainWindowHandle);
            MoveWindow(_appWin, 0, 0, width, height, true);

            test.WindowsFormsHostControl = windowsFormsHost;
            return windowsFormsHost;
        }

        public static void MakeExternalWindowBorderless(IntPtr MainWindowHandle)
        {
            int Style = 0;
            Style = GetWindowLong(MainWindowHandle, (int)GWL_STYLE);
            Style = Style & ~WS_CAPTION;
            Style = Style & ~WS_SYSMENU;
            Style = Style & ~WS_THICKFRAME;
            Style = Style & ~WS_MINIMIZE;
            Style = Style & ~WS_MAXIMIZEBOX;
            SetWindowLong(MainWindowHandle, (int)GWL_STYLE, Style);
            Style = GetWindowLong(MainWindowHandle, (int)GWL_EXSTYLE);
            SetWindowLong(MainWindowHandle, (int)GWL_EXSTYLE, (int)Style | (int)WS_EX_DLGMODALFRAME);
            SetWindowPos(MainWindowHandle, new IntPtr(0), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
        }

        public static void MakeExternalWindowHiddenOrNot(Process process, bool toHidden)
        {
            var hWnd = process.MainWindowHandle.ToInt32();
            if (toHidden)
                ShowWindow(hWnd, SW_HIDE);
            else
                ShowWindow(hWnd, SW_SHOW);
        }


        public class ScreenCapture
        {

            public Image CaptureScreen()
            {
                return CaptureWindow(User32.GetDesktopWindow());
            }

            public Image CaptureWindow(IntPtr handle)
            {
                IntPtr hdcSrc = User32.GetWindowDC(handle);
                User32.RECT windowRect = new User32.RECT();
                User32.GetWindowRect(handle, ref windowRect);
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;

                IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
                IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
                IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
                GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);

                GDI32.SelectObject(hdcDest, hOld);
                GDI32.DeleteDC(hdcDest);
                User32.ReleaseDC(handle, hdcSrc);
                Image img = Image.FromHbitmap(hBitmap);
                GDI32.DeleteObject(hBitmap);
                return img;
            }

            public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
            {
                Image img = CaptureWindow(handle);
                img.Save(filename, format);
            }

            public Image CaptureWindowToImage(IntPtr handle)
            {
                Image img = CaptureWindow(handle);
                return img;
            }

            public void CaptureScreenToFile(string filename, ImageFormat format)
            {
                Image img = CaptureScreen();
                img.Save(filename, format);
            }

            private class GDI32
            {

                public const int SRCCOPY = 0x00CC0020; 
                [DllImport("gdi32.dll")]
                public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                    int nWidth, int nHeight, IntPtr hObjectSource,
                    int nXSrc, int nYSrc, int dwRop);
                [DllImport("gdi32.dll")]
                public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                    int nHeight);
                [DllImport("gdi32.dll")]
                public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
                [DllImport("gdi32.dll")]
                public static extern bool DeleteDC(IntPtr hDC);
                [DllImport("gdi32.dll")]
                public static extern bool DeleteObject(IntPtr hObject);
                [DllImport("gdi32.dll")]
                public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
            }

            private class User32
            {
                [StructLayout(LayoutKind.Sequential)]
                public struct RECT
                {
                    public int left;
                    public int top;
                    public int right;
                    public int bottom;
                }
                [DllImport("user32.dll")]
                public static extern IntPtr GetDesktopWindow();
                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowDC(IntPtr hWnd);
                [DllImport("user32.dll")]
                public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            }
        }

        public static Image CaptureWindowToImage(IntPtr mainWindowHandle)
        {
            ScreenCapture sc = new ScreenCapture();
            if (sc != null)
                return sc.CaptureWindowToImage(mainWindowHandle);
            else
                return null;
        }

    }
}
