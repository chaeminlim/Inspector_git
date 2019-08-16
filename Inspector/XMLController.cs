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

            XmlElement xmlElement = null;
            while (automationElements.Count > 0)
            {
                AutomationElement ae = automationElements.Pop();
                
                if(ae.Current.ClassName == "Window")
                {
                    xmlElement = tree.CreateElement("wnd");
                    xmlElement.SetAttribute("name", ae.Current.Name);
                    xmlElement.SetAttribute("role", ae.Current.ClassName);

                }
                else
                {
                    xmlElement = tree.CreateElement("ctrl");
                    xmlElement.SetAttribute("name", ae.Current.Name);
                    xmlElement.SetAttribute("role", ae.Current.ClassName);
                }

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

        internal static Queue<Tuple<string, string>> ReadXmlTree(string text)
        {
            Queue<Tuple<string, string>> ElementInfoQueue = new Queue<Tuple<string, string>>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(text);
            XmlElement root = xmldoc.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                switch (node.Name)
                {
                    case "wnd":
                        foreach (XmlAttribute xa in node.Attributes)
                        {
                            if (xa.Name == "name")
                            {
                                ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
                            }
                            else if (xa.Name == "role")
                            {
                                ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
                            }
                        }
                        break;

                    case "ctrl":
                        foreach (XmlAttribute xa in node.Attributes)
                        {
                            if (xa.Name == "name")
                            {
                                ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
                            }
                            else if (xa.Name == "role")
                            {
                                ElementInfoQueue.Enqueue(new Tuple<string, string>(xa.Name, xa.Value));
                            }
                        }
                        break;
                }
            }

            return ElementInfoQueue;
        }

        internal static AutomationElement FindAutomationElementByQueue(Queue<Tuple<string, string>> elementInfoQueue)
        {
            Tuple<string, string> windowName = elementInfoQueue.Dequeue();
            Tuple<string, string> windowRole = elementInfoQueue.Dequeue();

            AutomationElement selectedOne = null;
            Process[] processes = Process.GetProcesses();
            List<AutomationElement> automationElements = new List<AutomationElement>();
            foreach (Process process in processes)
            {
                if(process.MainWindowHandle != IntPtr.Zero)
                {
                    AutomationElement ae = AutomationElement.FromHandle(process.MainWindowHandle);
                    automationElements.Add(ae);
                }
            }

            foreach(AutomationElement ae in automationElements)
            {
                if(ae.Current.Name == windowName.Item2)
                {
                    selectedOne = ae;
                    break;
                }
                else
                {
                    selectedOne = null;
                    return null;
                }
            }

            Tuple<AutomationElement, int> tuple = null;
            while (true)
            {
                tuple = FindMatch(selectedOne, elementInfoQueue);
                if(tuple.Item2 == 0)
                {
                    break;
                }
            }
            return tuple.Item1;

        }

        private static Tuple<AutomationElement, int> FindMatch(AutomationElement ae, Queue<Tuple<string, string>> elementInfoQueue)
        {
            AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild(ae);
            string Name = elementInfoQueue.Dequeue().Item2;
            elementInfoQueue.Dequeue();
            while (child != null)
            {
                if (child.Current.Name == Name)
                {
                    break;
                }
                
                child = TreeWalker.RawViewWalker.GetNextSibling(child);
            }

            int i = 0;
            if(elementInfoQueue.Count == 0)
            {
                i = 0;
            }else if(elementInfoQueue.Count > 0)
            {
                i = 1;
            }
            return new Tuple<AutomationElement, int>(child, i);
        
        }
    }
}
