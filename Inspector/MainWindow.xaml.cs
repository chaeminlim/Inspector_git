using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;


namespace Inspector
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainController MainControllerObject;

        public MainWindow()
        {
            InitializeComponent();
            Loading loading = new Loading();  //트리뷰가 실행되기까지 loading이란 문구가 뜨도록 보여준다.
            loading.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loading.Show();

            MainControllerObject = new MainController(this);
            MainControllerObject.MakeTree();

            loading.Close();  //'로딩중'이란 창을 닫아준다.

        }

        //selector버튼을 클릭한 경우
        private void CallSelector_Click(object sender, RoutedEventArgs e)
        {
            SelectorController selector = new SelectorController(this);
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MainControllerObject.PrintSelected(e.NewValue as TreeViewItem);
        }

        private void GetXmlButton_Click(object sender, RoutedEventArgs e)
        {
            AutomationElement ae = (AutomationElement)((TreeViewItem)treeView1.SelectedItem).Tag;
            Stack<AutomationElement> automationElmements = XmlController.MakeStack(ae);
            String resultString = XmlController.MakeXmlFile(automationElmements);  //스택안의 구조를 xml형태로 바꾸어 string형태로 저장
            resultString = resultString.Replace("><", ">\n<");
            XmlBox.Text = resultString;  //저장된 str을 textbox위치에 출력
        }

        private void FindByXml_Click(object sender, RoutedEventArgs e)
        {
            XmlController xmlController = new XmlController();
            String xmlData = XmlBox.Text.Replace(">\n<", "><");
            AutomationElement ae; int count;

            (count, ae) = xmlController.XmlFinder(xmlData);

            if (count == 1)
                MessageBox.Show("찾았습니다");
            else if(count == 0)
                MessageBox.Show("못 찾았습니다");
            else
                MessageBox.Show("여러개를 찾았습니다. ");

        }

        private void Recorder_Click(object sender, RoutedEventArgs e)
        {
            Recorder recorder = new Recorder(this.RecordList);
            recorder.Show();            
        }

        private void MessageSender_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////아래부분 수정해야됨
        /// </summary>

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


        private void ClearRecordList_Click(object sender, RoutedEventArgs e)
        {
            this.RecordList.Items.Clear();

        }
    }
}
