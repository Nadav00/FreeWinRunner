using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

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

        public static WindowsFormsHost SetProcessAsChildOfPanelControl(Process processToBeChild, int width, int height)
        {
            while (processToBeChild.MainWindowHandle == IntPtr.Zero)
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
    }
}
