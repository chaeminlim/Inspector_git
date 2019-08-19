using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Xml;

namespace Inspector
{
    class MainController
    {
        public List<Process> Processes { get; set; }
        public TreeView TreeView { get; set; }
        public ListView ListView { get; set; }
        //생성자에서 프로세스 리스트를 불러와 객체에 저장
        public MainController(TreeView treeView1, ListView listView1)
        {
            GetProcessInit();
            TreeView = treeView1;
            ListView = listView1;
        }

        public void GetProcessInit()
        {
            Processes = new List<Process>();
            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    Processes.Add(proc);
                }
            }
        }

        public void MakeTree()
        {
            TreeWalker walker = TreeWalker.RawViewWalker;

            foreach (Process proc in Processes)
            {
                AutomationElement ae = AutomationElement.FromHandle(proc.MainWindowHandle);
                AutomationElementWrapper aew = new AutomationElementWrapper(ae);
                TreeView.Items.Add(aew.Node);
                aew.Node.Header = proc.ProcessName + ", " + ae.Current.Name;
                TraverseElement(walker, aew);
            }


        }

        private static void TraverseElement(TreeWalker walker, AutomationElementWrapper automationElementWrapper)
        {
            AutomationElement child = walker.GetFirstChild(automationElementWrapper.AE);
            while (child != null)
            {
                AutomationElementWrapper caew = new AutomationElementWrapper(child);

                automationElementWrapper.Add(caew);
                TraverseElement(walker, caew);
                child = walker.GetNextSibling(child);
            }
        }

        public void PrintSelected(TreeViewItem treeViewItem)
        {
            AutomationElement ae = (AutomationElement)(treeViewItem).Tag;

            Process p = Process.GetProcessById(ae.Current.ProcessId);

            ListView.Items.Clear();

            ListView.Items.Add("프로세스 이름: " + p.ProcessName);
            ListView.Items.Add("파일 경로" + p.MainModule.FileName);

            ListView.Items.Add("요소명: " + ae.Current.Name);
            ListView.Items.Add("가속화 키: " + ae.Current.AcceleratorKey);
            ListView.Items.Add("액세스 키: " + ae.Current.AccessKey);
            ListView.Items.Add("자동화 요소 ID: " + ae.Current.AutomationId);
            ListView.Items.Add("사각영역: " + ae.Current.BoundingRectangle);
            ListView.Items.Add("클래스 이름: " + ae.Current.ClassName);
            ListView.Items.Add("컨트롤 유형: " + ae.Current.ControlType.ProgrammaticName);
            ListView.Items.Add("Framework ID: " + ae.Current.FrameworkId);
            ListView.Items.Add("포커스 소유 : " + ae.Current.HasKeyboardFocus);
            ListView.Items.Add("도움말: " + ae.Current.HelpText);
            ListView.Items.Add("컨텐츠 여부: " + ae.Current.IsContentElement);
            ListView.Items.Add("컨트롤 여부: " + ae.Current.IsControlElement);
            ListView.Items.Add("활성화 여부: " + ae.Current.IsEnabled);
            ListView.Items.Add("포커스 소유 가능 여부: " + ae.Current.IsKeyboardFocusable);
            ListView.Items.Add("화면 비표시 여부: " + ae.Current.IsOffscreen);
            ListView.Items.Add("내용 보화(패스워드) 여부: " + ae.Current.IsPassword);
            ListView.Items.Add("IsRequiredForForm: " + ae.Current.IsRequiredForForm);
            ListView.Items.Add("아이템 상태: " + ae.Current.ItemStatus);
            ListView.Items.Add("아이템 형식: " + ae.Current.ItemType);

            //SelectedItemController();


            //


        } 
    }
}
