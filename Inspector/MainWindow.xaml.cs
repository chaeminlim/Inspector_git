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

        private MainController MainControllerObject;

        public MainWindow()
        {
            InitializeComponent();
            Loading loading = new Loading();  //트리뷰가 실행되기까지 loading이란 문구가 뜨도록 보여준다.
            loading.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loading.Show();

            MainControllerObject = new MainController(treeView1, listView1);
            MainControllerObject.MakeTree();


            loading.Close();  //'로딩중'이란 창을 닫아준다.

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void GetXmlButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AutomationElement ae = (AutomationElement)((TreeViewItem)treeView1.SelectedItem).Tag;
                SelectedItem = ae;

                Stack<AutomationElement> automationElmements = XmlController.MakeStack(ae);
                String resultString = XmlController.MakeXmlFile(automationElmements);  //스택안의 구조를 xml형태로 바꾸어 string형태로 저장
                showxml.Text = resultString;  //저장된 str을 textbox위치에 출력

            }
            catch
            {
                try
                {
                    Process ps = (Process)((TreeViewItem)treeView1.SelectedItem).Tag;
                    string resultString = ps.ProcessName + "  ";
                    showxml.Text = resultString;

                }
                catch (System.NullReferenceException)  //프로세스를 클릭하지않고 버튼만 클릭한경우F
                {
                    MessageBox.Show("NullReferenceException");
                }
            }

        }
        #region dummy
        ////change into xml버튼을 누른경우
        //private void GetXmlButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {

        //        AutomationElement ae = (AutomationElement)((TreeViewItem)treeView1.SelectedItem).Tag;
        //        SelectedItem = ae;

        //        Stack<AutomationElement> automationElmements = XmlController.MakeStack(ae);
        //        String resultString = XmlController.MakeXmlFile(automationElmements);
        //        showxml.Text = resultString;  //저장된 str을 textbox위치에 출력

        //    }
        //    catch (System.NullReferenceException)
        //    {
        //        MessageBox.Show("NullReferenceException");
        //    }


        //}

        #endregion

        //getprocess버튼을 클릭했을 경우
        


        

        //트리에서 어떠한 프로세스를 클릭한 경우 그 프로세스에 대한 자세한 설명(tag)를 listview1에 출력해준다.
        //에러종류에 따라 error박스가 나타나거나 프로세스에 대한 간략한 설명만 출력해준다
        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MainControllerObject.PrintSelected(e.NewValue as TreeViewItem);
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

        private void MessageSender_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
