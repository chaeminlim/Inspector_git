using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;

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

        private MainWindow mainWindow;
        private MouseHook mouseHook;
        

        public SelectorController(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mouseHook = new MouseHook();

            mouseHook.MouseMove += MouseHook_MouseMove;
            mouseHook.LeftButtonDown += MouseHook_LeftButtonDown;
            mouseHook.Install();

            mainWindow.WindowState = WindowState.Minimized;

            aeRemains = null;
        }

        private AutomationElement aeRemains;
        private void MouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            /*
            System.Windows.Point point = new System.Windows.Point();
            point.X = mouseStruct.pt.x;
            point.Y = mouseStruct.pt.y;

            AutomationElement ae1 = AutomationElement.FromPoint(point);
            
            if(ae1 != aeRemains && ae1 != null)
            {
                IntPtr dc = GetDC((IntPtr)ae1.Current.NativeWindowHandle);

                Graphics newGraphics = Graphics.FromHdc(dc);
                //newGraphics.Dispose();
                aeRemains = ae1;

                Rect rect = aeRemains.Current.BoundingRectangle;
                

                newGraphics.DrawRectangle(new Pen(Color.Red, 3),(float)rect.X , (float)rect.Y , (float)rect.Width , (float)rect.Height);
                newGraphics.Dispose();
            }
            */

        }

        private void MouseHook_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            System.Windows.Point point = new System.Windows.Point();
            point.X = mouseStruct.pt.x;
            point.Y = mouseStruct.pt.y;

            mouseHook.Uninstall();
            AutomationElement ae = AutomationElement.FromPoint(point);
            Stack<AutomationElement> automationElements = XmlController.MakeStack(ae);
            String result = XmlController.MakeXmlFile(automationElements, 0, null);
            mainWindow.XmlBox.Text = result;

            mainWindow.Activate();

        }
    }
}
