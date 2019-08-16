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

        //change into xml버튼을 누른경우
        private void GetXmlButton_Click(object sender, RoutedEventArgs e)
        {
            try  //트리뷰의 하위노드를 클릭한경우
            {
                AutomationElement ae = (AutomationElement)((TreeViewItem)treeView1.SelectedItem).Tag;
                SelectedItem = ae;
                string str = "";

                Stack<AutomationElement> automationElements = new Stack<AutomationElement>();  //계층구조를 저장할 스택선언
                TreeWalker walker = TreeWalker.RawViewWalker;  // treewalker 선언
                automationElements.Push(ae);  //해당노드를 스택에 먼저 push

                AutomationElement parent = walker.GetParent(ae);  //treewalker를 통해 바로위의 부모를 찾는다.
                while (walker.GetParent(parent) != null)  //최상위 부모를 찾을때까지 반복문 실행
                {
                    automationElements.Push(parent);  //각 계층구조를 스택에 저장
                    parent = walker.GetParent(parent);
                }

                XmlController xmlController = new XmlController();
                xmlController.MakeXmlFile(automationElements);

            }
            catch (Exception)
            {

            }



                
            //    Process ps = Process.GetProcessById(ae.Current.ProcessId);  //프로세스id로 현재 프로세스를 찾아 관련정보를 ps에저장
            //    if (ps.ProcessName.Equals("chrome") || ps.ProcessName.Equals("explorer"))
            //    {  //인터넷창이 최상위부모인경우
            //        str = "<html app:" + ps.ProcessName + " />\n";  //태그를 html로 시작
            //        int i = 1;
            //        while (automationElements.Count != 0)  //스택이 빌때까지 반복문실행
            //        {
            //            for (int j = 1; j <= i; j++) { str += "  "; }
            //            SelectedItem = (AutomationElement)automationElements.Peek();  //스택에서 한개씩 꺼내서 해당요소이름을 출력
            //            str += "<webctrl " + SelectedItem.Current.Name + "/>\n";
            //            automationElements.Pop(); i++;
            //        }

            //    }
            //    else  //인터넷창이 최상위부모가 아닌경우 wnd로 태그시작
            //    {
            //        str = "<wnd app:" + ps.ProcessName + " />\n";
            //        int i = 1;
            //        while (automationElements.Count() != 0)
            //        {
            //            for (int j = 1; j <= i; j++)
            //            {
            //                str += "  ";
            //            }
            //            SelectedItem = (AutomationElement)(automationElements.Peek());
            //            str += "<ctrl " + SelectedItem.Current.Name + "/>\n";
            //            automationElements.Pop(); i++;
            //        }
            //    }
            //    showxml.Text = str;  //저장된 str을 textbox위치에 출력
            //}
            //catch (Exception)  //최상위 프로세스를 클릭한경우 최상위 프로세스이름만 출력
            //{
            //    try
            //    {
            //        Process ps = (Process)((TreeViewItem)treeView1.SelectedItem).Tag;
            //        string str = "";
            //        if (ps.ProcessName.Equals("chrome") || ps.ProcessName.Equals("explorer"))
            //        {
            //            str = "<html app:" + ps.ProcessName + " /> ";

            //        }
            //        else
            //        {
            //            str = "<wnd app:" + ps.ProcessName + " /> ";
            //        }
            //        showxml.Text = str;
            //    }
            //    catch (Exception)
            //    {
            //        MessageBox.Show("error2");
            //    }
            //}

        }
        //getprocess버튼을 클릭했을 경우
        private void GetProcButton_Click(object sender, RoutedEventArgs e)
        {
            treeView1.Items.Clear();  //트리뷰를 우선 초기화한다

            Loading loading = new Loading();  //트리뷰가 실행되기까지 loading이란 문구가 뜨도록 보여준다.
            loading.Show();

            Process[] processes = Process.GetProcesses();  //현재 모든 실행중인 모든프로세스를 배열형태로 저장
            foreach (Process proc in processes)  //모든 프로세스를 foreach로 돌면서 메인창이 뜨는 프로세스 각각을 treeviewitem객체에 넣는다
            {
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    //header의 경우는 treeview에 프로세스 이름이 출력되도록하는것.
                    //tag는 그 프로세스이름을 클릭 시 해당 정보가 출력되도록하는것.(listview1에)
                    TreeViewItem tvi = new TreeViewItem() { Header = proc.ProcessName, Tag = proc };
                    tvi.ExpandSubtree();  //현재 프로세스 하위 트리가 한단계 펼쳐지도록 해주는 부분.
                    TreeViewWrapper wrapper = new TreeViewWrapper(tvi);  //treevieitem add를 쉽게하기위해 treewrapper안에 treeviewitem객체를 넣는다.
                    treeView1.Items.Add(wrapper.Node);
                    UIAutomationElementFinder(wrapper);  //현재 프로세스안에 있는 uiautomationelement부분을 찾아준다.
                }
            }

            loading.Close();  //'로딩중'이란 창을 닫아준다.
        }

        //해당 treeviewitem에서 uiautomationelement에 대한 정보를 미리 찾아주는 함수
        private void UIAutomationElementFinder(TreeViewWrapper wrapper)  //treeviewitem이 담겨있는 treeviewwrapper를 넘겨준다
        {
            Process proc = (Process)wrapper.Node.Tag;  //넘겨준 treeviewwrapper객체의 상세설명인 tag를 변수 proc에 저장해준다

            AutomationElement ae = AutomationElement.FromHandle(proc.MainWindowHandle);  //proc을 다룰수있는 핸들에 관한 정보를 automationelement 객체에 넣어준다
            AutomationElementWrapper aew = new AutomationElementWrapper(ae);  //마찬가지로 automationelement객체의 add를 쉽게해주기위해 automationelementwrapper에 넣어준다

            wrapper.Add(aew.Node);
            wrapper.Node.ExpandSubtree();  //매개변수로 넘겨진 프로세스의 하위트리를 한단계 펼쳐준다


            TreeWalker walker = TreeWalker.RawViewWalker; // treewalker객체 rawviewwalker로 만들어주어 모든 트리를 순회가능하도록 생성하고, 만들어진 
                                                          //트리를 순회하면서 완성하도록한다.(미리 정보가져오기위함)
            TraverseElement(walker, aew);
        }

        //treewalker객체로 트리완성하는 함수. 매개변수로 treewalker, automationelement가 들어있는 automationelementwrapper에 넣어준다
        private void TraverseElement(TreeWalker walker, AutomationElementWrapper automationElementWrapper)
        {
            AutomationElement child = walker.GetFirstChild(automationElementWrapper.AE); //treewalker를 통해 해당 automationelement의 첫번째 자식을 chlld에 넣어준다
            while (child != null)  //자식이 없을때까지 반복
            {
                AutomationElementWrapper caew = new AutomationElementWrapper(child);  //마찬가지로 자식도 하위 프로세스가 있으므로 add를 쉽게하기위해
                                                                                      // wrapping을 해준다
                automationElementWrapper.Add(caew);
                TraverseElement(walker, caew);  //자식을 다시 매개변수로 넣어 재귀로 트리를 완선시킨다.
                child = walker.GetNextSibling(child);  //한 자식에 대한 정보가 완료된 경우 getnextsibling이란 함수로 다음 자식에대한 정보를 child에 넣고 반복
            }
        }

        //트리에서 어떠한 프로세스를 클릭한 경우 그 프로세스에 대한 자세한 설명(tag)를 listview1에 출력해준다.
        //에러종류에 따라 error박스가 나타나거나 프로세스에 대한 간략한 설명만 출력해준다
        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            try
            {
                AutomationElement ae = (AutomationElement)((TreeViewItem)e.NewValue).Tag;
                SelectedItem = ae;

                listView1.Items.Clear();
                listView1.Items.Add("요소명: " + ae.Current.Name);
                listView1.Items.Add("가속화 키: " + ae.Current.AcceleratorKey);
                listView1.Items.Add("액세스 키: " + ae.Current.AccessKey);
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
                    listView1.Items.Add("LabledBy 이름: " + ae.Current.LabeledBy.Current.Name);
                }
                listView1.Items.Add("윈도우 창 핸들: " + ae.Current.NativeWindowHandle);
                listView1.Items.Add("컨트롤 방향: " + ae.Current.Orientation);
                listView1.Items.Add("프로세스 ID: " + ae.Current.ProcessId);
                //


                SelectedItemController();


                //


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
                catch (Exception)
                {
                    MessageBox.Show("error");
                }
            }
        }



        //selector버튼을 클릭한 경우
        private void CallSelector_Click(object sender, RoutedEventArgs e)
        {

            SelectorPage selectorPage = new SelectorPage();  //selectorPage라는 윈도우창을 하나 만들어서 
            selectorPage.Owner = this;						//그 창의 정보를 가져올 수 있도록한다.
            selectorPage.WindowState = WindowState.Normal;  //selectorPage창의 기본값을 보통창크기로 설정해준다
            selectorPage.ShowDialog();  //해당창을 띄워준다

            Selector.SelectorController(selectorPage);


        }

        private void SelectedItemController()
        {
            AutomationPattern[] patterns = SelectedItem.GetSupportedPatterns();  //주어진 트리노드의 컨트롤유형을 배열형태로 저장


            listView2.Items.Clear();  //listview2부분을 초기화한뒤
            foreach (AutomationPattern pattern in patterns)//각 컨트롤 유형에 대해서 반복문 실행
            {
                //    listView2.Items.Add("ProgrammaticName: " + pattern.ProgrammaticName);
                //    listView2.Items.Add("PatternName: " + Automation.PatternName(pattern));

                PatternControl pc = new PatternControl();  //컨트롤유형과 관련한 객체를 생성해서
                pc.PatternController(pattern, SelectedItem, listView2);  //각 컨트롤유형에 따라 가능한 자동화형태를 listview2에 출력

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

        //call handler를 클릭한경우
        private void CallHandler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListViewItem lvi = selectedListItem;  //Listviewitem객체에 클릭한 노드를 넣어주고
                PatternInfo pi = (PatternInfo)lvi.Tag;  //해당 노드의 tag(listview1의 내용)을 pi에 저장
                pi.PatternDistinguisher();  //해당노드의 tag를 보고 자동화요소의 handler를 실행해준다
                                            //TreeViewItem tvi = (TreeViewItem)treeView1.SelectedItem;
                                            //AutomationElementWrapper aew = new AutomationElementWrapper(tvi.Tag as AutomationElement);

                //TreeWalker walker = TreeWalker.RawViewWalker; // make tree walker
                //TraverseElement(walker, aew);
            }
            catch (Exception)  //프로세스를 클릭하지않고 call handler만 클릭한 에러의 경우 해당메시지박스 출력
            {
                MessageBox.Show("error!\n프로세스를 불러온 후 자동화 요소를 먼저 선택하세요");
            }
        }

    }
}
