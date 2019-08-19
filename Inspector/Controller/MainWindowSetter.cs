using System;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Inspector
{
    class MainWindowSetter
    {
        internal static void GetProcessInit(TreeView treeView1)
        {
            treeView1.Items.Clear();  //트리뷰를 우선 초기화한다
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
        }

        //해당 treeviewitem에서 uiautomationelement에 대한 정보를 미리 찾아주는 함수
        private static void UIAutomationElementFinder(TreeViewWrapper wrapper)  //treeviewitem이 담겨있는 treeviewwrapper를 넘겨준다
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
        private static void TraverseElement(TreeWalker walker, AutomationElementWrapper automationElementWrapper)
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
    }
}
