using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Inspector
{
    public class SelectorController
    {
        [DllImport("User32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        private static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
        [DllImport("gdi32.dll")]
        static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        public System.Windows.Point Point;
        private MouseHook mouseHook;
        private MainWindow mainWindow;

        public SelectorController(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mouseHook = new MouseHook();

            mouseHook.MouseMove += MouseHook_MouseMove;
            mouseHook.LeftButtonDown += MouseHook_LeftButtonDown;
            mouseHook.Install();

            mainWindow.WindowState = WindowState.Minimized;

        }

        private void MouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Point.X = mouseStruct.pt.x;
            Point.Y = mouseStruct.pt.y;

        }

        private void MouseHook_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {

            mouseHook.Uninstall();
            AutomationElement ae = AutomationElement.FromPoint(Point);
            Stack<AutomationElement> automationElements = XmlController.MakeStack(ae);
            String result = XmlController.MakeXmlFile(automationElements);
            mainWindow.XmlBox.Text = result;

            mainWindow.Activate();

        }
    }
}
