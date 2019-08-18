using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Xml;

namespace Inspector
{
    class XmlController
    {
        internal static string MakeXmlFile(Stack<AutomationElement> automationElements)
        {
            XmlDocument tree = new XmlDocument();
            XmlElement root  = tree.CreateElement("UI");
            tree.AppendChild(root);

            XmlElement xmlElement = tree.CreateElement("process");
            Process p = Process.GetProcessById(automationElements.Peek().Current.ProcessId);
            
            xmlElement.SetAttribute("name", p.ProcessName);
            root.AppendChild(xmlElement);

            while (automationElements.Count > 0)
            {
                AutomationElement ae = automationElements.Pop();
                xmlElement = null;
                xmlElement = tree.CreateElement("element");
                xmlElement.SetAttribute("name", ae.Current.Name);
                xmlElement.SetAttribute("role", ae.Current.ClassName);
                root.AppendChild(xmlElement);
            }
            

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            tree.WriteTo(tx);

            string strData = sw.ToString();
            return strData;

        }

        internal static Stack<AutomationElement> MakeStack(AutomationElement ae)
        {

            Stack<AutomationElement> automationElements = new Stack<AutomationElement>();  //계층구조를 저장할 스택선언
            TreeWalker walker = TreeWalker.RawViewWalker;  // treewalker 선언
            automationElements.Push(ae);  //해당노드를 스택에 먼저 push

            AutomationElement parent = walker.GetParent(ae);  //treewalker를 통해 바로위의 부모를 찾는다.
            while (walker.GetParent(parent) != null)  //최상위 부모를 찾을때까지 반복문 실행
            {
                automationElements.Push(parent);  //각 계층구조를 스택에 저장
                parent = walker.GetParent(parent);
            }

            return automationElements;
        }

        internal static AutomationElement ReadXml(string xmlData)
        {
            // xmlData를 xml 형식으로 파싱하고 다른 자료구조에 저장
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlData);
            XmlElement root = xmldoc.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;

            List<Tuple<string, string, string>> treeData = new List<Tuple<string, string, string>>();
            // 1 - 태그 이름 2 - name 값 3 - role 값
            foreach(XmlNode node in nodes)
            {
                String nameVal = "", roleVal= "";
                switch (node.Name)
                {
                    case "process":
                        foreach (XmlAttribute xa in node.Attributes)
                        {
                            if (xa.Name == "name")
                            { nameVal = xa.Value; }
                        }
                        treeData.Add(new Tuple<string, string, string>(node.Name, nameVal, roleVal));
                        break;

                    case "element":
                        foreach (XmlAttribute xa in node.Attributes)
                        {
                            if (xa.Name == "name")
                            { nameVal = xa.Value; }
                            else if (xa.Name == "role")
                            { roleVal = xa.Value; }
                        }
                        treeData.Add(new Tuple<string, string, string>(node.Name, nameVal, roleVal));
                        break;
                }
            }

            // check proess opened
            Process[] processes = Process.GetProcessesByName(treeData[0].Item2);
            if (processes.Length < 1)
            {
                return null; // 에러처리 해야됨. 해당 프로세스가 없을 때
            }

            AutomationElement ae = null;

            foreach (Process p in processes)
            {
                if(p.ProcessName == treeData[0].Item2)
                {
                    ae = AutomationElement.FromHandle(p.MainWindowHandle);
                    break;
                }
            }

            if(ae == null)
            {
                return null; // 예외처리  automationelement가 할당되지 않았을 때
            }
            if(ae.Current.ClassName != treeData[1].Item3)
            {
                return null; // 예외처리 찾은 엘리먼트의 role이 다를때 즉 잘못찾았을떄
            }

            // 프로세스에 관한 정보 제거 element에 관해서만 확인
            treeData.RemoveAt(0);
            treeData.RemoveAt(0); // 메인 윈도우 엘리먼트는 이미 찾았으므로

            AutomationElement returnVal1 = ae;
            AutomationElement returnVal2 = null;
            foreach (Tuple<string, string, string> tuple in treeData)
            {
                returnVal2 = FindAutomationElement(returnVal1, tuple);
                if (returnVal2 == null)
                {
                    return null; // 예외처리 - 못찾을 경우
                }
                returnVal1 = returnVal2;
            }
            
            return returnVal2;
        }

        private static AutomationElement FindAutomationElement(AutomationElement parent, Tuple<string, string, string> tuple)
        {
            string type = tuple.Item1;
            string name = tuple.Item2;
            string role = tuple.Item3;
            
            AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild(parent);
            List<string> list = new List<string>();
            while(child != null)
            {
                if(child.Current.Name == name)
                {
                    if(child.Current.ClassName == role)
                    {
                        return child;
                    }
                }
                else
                {
                    child = TreeWalker.RawViewWalker.GetNextSibling(child);
                }
            }
            return null; // 못찾음
        }



        #region dummy

        //internal static Queue<Tuple<string, string>> ReadXmlTree(string text)
        //{
        //    Queue<Tuple<string, string>> ElementInfoQueue = new Queue<Tuple<string, string>>();

        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.LoadXml(text);
        //    XmlElement root = xmldoc.DocumentElement;
        //    XmlNodeList nodes = root.ChildNodes;
        //    foreach (XmlNode node in nodes)
        //    {
        //        switch (node.Name)
        //        {
        //            case "wnd":
        //                foreach (XmlAttribute xa in node.Attributes)
        //                {
        //                    if (xa.Name == "name")
        //                    {
        //                        ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
        //                    }
        //                    else if (xa.Name == "role")
        //                    {
        //                        ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
        //                    }
        //                }
        //                break;

        //            case "ctrl":
        //                foreach (XmlAttribute xa in node.Attributes)
        //                {
        //                    if (xa.Name == "name")
        //                    {
        //                        ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
        //                    }
        //                    else if (xa.Name == "role")
        //                    {
        //                        ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
        //                    }
        //                }
        //                break;
        //        }
        //    }

        //    return ElementInfoQueue;
        //}

        //internal static AutomationElement FindAutomationElementByQueue(Queue<Tuple<string, string>> elementInfoQueue)
        //{
        //    Tuple<string, string> windowName = elementInfoQueue.Dequeue();
        //    Tuple<string, string> windowRole = elementInfoQueue.Dequeue();

        //    AutomationElement selectedOne = null;
        //    Process[] processes = Process.GetProcesses();
        //    List<AutomationElement> automationElements = new List<AutomationElement>();
        //    foreach (Process process in processes)
        //    {
        //        if(process.MainWindowHandle != IntPtr.Zero)
        //        {
        //            AutomationElement ae = AutomationElement.FromHandle(process.MainWindowHandle);
        //            automationElements.Add(ae);
        //        }
        //    }

        //    foreach(AutomationElement ae in automationElements)
        //    {
        //        if(ae.Current.Name == windowName.Item2)
        //        {
        //            selectedOne = ae;
        //            break;
        //        }
        //        else
        //        {
        //            selectedOne = null;
        //            return null;
        //        }
        //    }

        //    Tuple<AutomationElement, int> tuple = null;
        //    while (true)
        //    {
        //        tuple = FindMatch(selectedOne, elementInfoQueue);
        //        if(tuple.Item2 == 0)
        //        {
        //            break;
        //        }
        //    }
        //    return tuple.Item1;

        //}

        //private static Tuple<AutomationElement, int> FindMatch(AutomationElement ae, Queue<Tuple<string, string>> elementInfoQueue)
        //{
        //    AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild(ae);
        //    string Name = elementInfoQueue.Dequeue().Item2;
        //    elementInfoQueue.Dequeue();
        //    while (child != null)
        //    {
        //        if (child.Current.Name == Name)
        //        {
        //            break;
        //        }

        //        child = TreeWalker.RawViewWalker.GetNextSibling(child);
        //    }

        //    int i = 0;
        //    if(elementInfoQueue.Count == 0)
        //    {
        //        i = 0;
        //    }else if(elementInfoQueue.Count > 0)
        //    {
        //        i = 1;
        //    }
        //    return new Tuple<AutomationElement, int>(child, i);

        //}
        #endregion
    }
}
