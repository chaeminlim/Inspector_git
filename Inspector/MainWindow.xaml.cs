using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inspector
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public AutomationElement SelectedItem
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private void GetProcButton_Click(object sender, RoutedEventArgs e)
        {


            Loading loading = new Loading();
            loading.Show();

            Process[] processes = Process.GetProcesses();
            foreach(Process proc in processes)
            {
                if(proc.MainWindowHandle != IntPtr.Zero)
                {
                    TreeViewItem tvi = new TreeViewItem() { Header = proc.ProcessName, Tag = proc };
                    tvi.ExpandSubtree();
                    TreeViewWrapper wrapper = new TreeViewWrapper(tvi);
                    treeView1.Items.Add(wrapper.Node);
                    UIAutomationElementFinder(wrapper);
                }
            }
            
            loading.Close();
        }

        private void UIAutomationElementFinder(TreeViewWrapper wrapper)
        {
            Process proc = (Process)wrapper.Node.Tag;

            AutomationElement ae = AutomationElement.FromHandle(proc.MainWindowHandle);
            AutomationElementWrapper aew = new AutomationElementWrapper(ae);

            wrapper.Add(aew.Node);
            wrapper.Node.ExpandSubtree();


            TreeWalker walker = TreeWalker.RawViewWalker; // make tree walker
            TraverseElement(walker, aew);
        }

        private void TraverseElement(TreeWalker walker, AutomationElementWrapper automationElementWrapper)
        {
            AutomationElement child = walker.GetFirstChild(automationElementWrapper.AE);
            while(child != null)
            {
                AutomationElementWrapper caew = new AutomationElementWrapper(child);
                automationElementWrapper.Add(caew);
                TraverseElement(walker, caew);
                child = walker.GetNextSibling(child);
            }
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
            try
            {
                AutomationElement ae = (AutomationElement)((TreeViewItem)e.NewValue).Tag;
                SelectedItem = ae;

                listView1.Items.Clear();
                listView1.Items.Add("요소명: "+ ae.Current.Name);
                listView1.Items.Add("가속화 키: " + ae.Current.AcceleratorKey);
                listView1.Items.Add("액세스 키: "+ ae.Current.AccessKey);
                listView1.Items.Add("자동화 요소 ID: " + ae.Current.AutomationId);
                listView1.Items.Add("사각영역: " + ae.Current.BoundingRectangle);
                listView1.Items.Add("클래스 이름: " + ae.Current.ClassName);
                listView1.Items.Add("컨트롤 유형: " + ae.Current.ControlType.ProgrammaticName);
                listView1.Items.Add("Framework ID: " + ae.Current.FrameworkId);
                listView1.Items.Add("포커스 소유 : " + ae.Current.HasKeyboardFocus);
                listView1.Items.Add("도움말: " + ae.Current.HelpText);
                listView1.Items.Add("컨텐츠 여부: " + ae.Current.IsContentElement);
                listView1.Items.Add("컨트롤 여부: " + ae.Current.IsControlElement);
                listView1.Items.Add("활성화 여부: " + ae.Current.IsEnabled);
                listView1.Items.Add("포커스 소유 가능 여부: " + ae.Current.IsKeyboardFocusable);
                listView1.Items.Add("화면 비표시 여부: " + ae.Current.IsOffscreen);
                listView1.Items.Add("내용 보화(패스워드) 여부: " + ae.Current.IsPassword);
                listView1.Items.Add("IsRequiredForForm: " + ae.Current.IsRequiredForForm);
                listView1.Items.Add("아이템 상태: " + ae.Current.ItemStatus);
                listView1.Items.Add("아이템 형식: " + ae.Current.ItemType);
                if (ae.Current.LabeledBy != null)
                {
                    listView1.Items.Add("LabledBy 이름: "+ ae.Current.LabeledBy.Current.Name);
                }
                listView1.Items.Add("윈도우 창 핸들: " + ae.Current.NativeWindowHandle);
                listView1.Items.Add("컨트롤 방향: " + ae.Current.Orientation);
                listView1.Items.Add("프로세스 ID: " + ae.Current.ProcessId);
                //

                SelectedItemController();

                //
                IntPtr desktopPtr = GetDC(IntPtr.Zero);
                Graphics g = Graphics.FromHdc(desktopPtr);

                System.Drawing.Pen b = new System.Drawing.Pen(System.Drawing.Color.White);
                System.Drawing.Rectangle Rect = new System.Drawing.Rectangle(
                    (int)ae.Current.BoundingRectangle.X,
                    (int)ae.Current.BoundingRectangle.Y,
                    (int)ae.Current.BoundingRectangle.Width,
                    (int)ae.Current.BoundingRectangle.Height);

                g.DrawRectangle(b, Rect);
                g.Dispose();

                ReleaseDC(IntPtr.Zero, desktopPtr);

            }
            catch (Exception)
            {
                try
                {
                    Process ps = (Process)((TreeViewItem)e.NewValue).Tag;
                    listView1.Items.Clear();
                    listView1.Items.Add("프로세스 id: " + ps.Id);
                    listView1.Items.Add("프로세스 이름: " + ps.ProcessName);
                    listView1.Items.Add("메인 창 이름: " + ps.MainWindowTitle);
                }
                catch(Exception)
                {
                    MessageBox.Show("error");
                }
            }
        }

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);


        private void CallSelector_Click(object sender, RoutedEventArgs e)
        {
            SelectorPage selectorPage = new SelectorPage();
            this.WindowState = WindowState.Minimized;
            selectorPage.ShowDialog();
            
            selectorPage.Close();
            this.WindowState = WindowState.Normal;

        }

        
        private void SelectedItemController()
        {
            AutomationPattern[] patterns = SelectedItem.GetSupportedPatterns();

            listView2.Items.Clear();
            foreach (AutomationPattern pattern in patterns)
            {
            //    listView2.Items.Add("ProgrammaticName: " + pattern.ProgrammaticName);
            //    listView2.Items.Add("PatternName: " + Automation.PatternName(pattern));

                PatternControl pc = new PatternControl();
                pc.PatternController(pattern, SelectedItem, listView2);
                
            }
            
        }

        public ListViewItem selectedListItem
        {
            get; set;
        }
        private void ListView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedListItem = e.AddedItems[0] as ListViewItem;
        }
        private void CallHandler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListViewItem lvi = selectedListItem;
                PatternInfo pi = (PatternInfo)lvi.Tag;
                pi.PatternDistinguisher();

                TreeViewItem tvi = (TreeViewItem)treeView1.SelectedItem;
                AutomationElementWrapper aew = new AutomationElementWrapper(tvi.Tag as AutomationElement);

                TreeWalker walker = TreeWalker.RawViewWalker; // make tree walker
                TraverseElement(walker, aew);
            }
            catch (Exception)
            {
                MessageBox.Show("error!\n프로세스를 불러온 후 자동화 요소를 먼저 선택하세요");
            }
        }

    }
}
