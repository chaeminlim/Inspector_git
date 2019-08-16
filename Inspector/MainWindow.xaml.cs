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
            try
            {

                AutomationElement ae = (AutomationElement)((TreeViewItem)treeView1.SelectedItem).Tag;
                SelectedItem = ae;

                Stack<AutomationElement> automationElmements = XmlController.MakeStack(ae);
                String resultString = XmlController.MakeXmlFile(automationElmements);
                showxml.Text = resultString;  //저장된 str을 textbox위치에 출력

            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("NullReferenceException");
            }


        }
        //getprocess버튼을 클릭했을 경우
        private void GetProcButton_Click(object sender, RoutedEventArgs e)
        {
            Loading loading = new Loading();  //트리뷰가 실행되기까지 loading이란 문구가 뜨도록 보여준다.
            loading.Show();
            MainWindowSetter.GetProcessInit(treeView1);
            

            loading.Close();  //'로딩중'이란 창을 닫아준다.
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
