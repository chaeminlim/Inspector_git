using System;
using System.Windows;
using System.Windows.Automation;

namespace Inspector
{
    internal class RecorderController
    {
        private System.Windows.Controls.ListBox RecordList;
        private MouseHook mouseHook;

        public RecorderController(System.Windows.Controls.ListBox recordList)
        {
            RecordList = recordList;
            mouseHook = new MouseHook();
        }

        public void Install()
        {
            mouseHook.Install();
        }

        public void Start()
        {
            mouseHook.LeftButtonDown += MouseHook_LeftButtonDown;
        }

        public void Stop()
        {
            mouseHook.LeftButtonDown -= MouseHook_LeftButtonDown;
        }

        private void MouseHook_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Point point = new Point(mouseStruct.pt.x, mouseStruct.pt.y);
            AutomationElement ae = AutomationElement.FromPoint(point);
            String xmlData = XmlController.MakeXmlFile(XmlController.MakeStack(ae));
            AddList(xmlData);
        }

        private void AddList(string xmlData)
        {
            RecordList.Items.Add(xmlData);
        }

        public void FindRecordedElement()
        {

        }
    }
}