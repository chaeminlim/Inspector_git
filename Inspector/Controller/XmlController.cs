using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using System.Xml;

namespace Inspector
{
    class XmlController
    {
        public static string MakeXmlFile(Stack<AutomationElement> automationElements)
        {
            XmlDocument tree = new XmlDocument();
            XmlElement root = tree.CreateElement("UI");
            tree.AppendChild(root);

            XmlElement xmlElement = tree.CreateElement("Window");
            AutomationElement wae = automationElements.Pop();
            Process p = Process.GetProcessById(wae.Current.ProcessId);

            xmlElement.SetAttribute("App",  p.MainModule.ModuleName);
            xmlElement.SetAttribute("Class", wae.Current.ClassName);

            root.AppendChild(xmlElement);

            while (automationElements.Count > 0)
            {
                AutomationElement ae = automationElements.Pop();
                xmlElement = tree.CreateElement("Element");
                xmlElement.SetAttribute("Name", ae.Current.Name);
                xmlElement.SetAttribute("Class", ae.Current.ClassName);


                root.AppendChild(xmlElement);
            }


            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            tree.WriteTo(tx);

            string strData = sw.ToString();
            return strData;

        }
        public static Stack<AutomationElement> MakeStack(AutomationElement ae)
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
        public (int, AutomationElement) XmlFinder(string xmlData)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            XmlElement root = xml.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;
            Queue<XmlNode> xmlQueue = new Queue<XmlNode>();
            int depth = nodes.Count;

            foreach (XmlNode node in nodes)
            {
                xmlQueue.Enqueue(node);
            }
            Queue<AutomationElement> windowQueue = WindowXMlFinder(xmlQueue.Dequeue(), depth);
            Queue<AutomationElement> elemQueue1 = windowQueue;
            Queue<AutomationElement> elemQueue2 = new Queue<AutomationElement>();
            depth -= 1;

            while (xmlQueue.Count > 0)
            {
                XmlNode xmlNode = xmlQueue.Dequeue();

                elemQueue2 = ElementXMlFinder(xmlNode, elemQueue1, depth);
                elemQueue1 = elemQueue2;
                depth -= 1;

            }

            if (elemQueue1.Count != 1)
                return (elemQueue1.Count, null);
            else
                return (elemQueue1.Count, elemQueue1.Dequeue());
        }
        private Queue<AutomationElement> GetProcessInit()
        {
            Queue<AutomationElement> procQueue = new Queue<AutomationElement>();

            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    AutomationElement ae = AutomationElement.FromHandle(proc.MainWindowHandle);
                    procQueue.Enqueue(ae);
                }
            }
            return procQueue;
        }
        private Queue<AutomationElement> WindowXMlFinder(XmlNode windowNode, int depth)
        {


            Queue<AutomationElement> windowQueue = GetProcessInit();
            Queue<AutomationElement> Filter1 = new Queue<AutomationElement>();
            Queue<AutomationElement> Filter2 = new Queue<AutomationElement>();


            foreach (XmlAttribute attribute in windowNode.Attributes)
            {
                if (attribute.Name == "App")
                {
                    while(windowQueue.Count > 0)
                    {
                        AutomationElement ae = windowQueue.Dequeue();
                        Process p = Process.GetProcessById(ae.Current.ProcessId);

                        if (p.MainModule.ModuleName == attribute.Value)
                        {
                            Filter1.Enqueue(ae);
                        }
                    }
                }
                else if (attribute.Name == "Class")
                {
                    while (Filter1.Count > 0)
                    {
                        AutomationElement ae = Filter1.Dequeue();
                        if (ae.Current.ClassName == attribute.Value)
                        {
                            Filter2.Enqueue(ae);
                        }
                    }
                }
            }

            return FindChild(Filter2, depth);
        }
        private Queue<AutomationElement> FindChild(Queue<AutomationElement> elemQueue, int depth)
        {
            if(depth == 1)
            {
                return elemQueue;
            }
            else
            {
                Queue<AutomationElement> returnQueue = new Queue<AutomationElement>();
                while(elemQueue.Count > 0)
                {
                    AutomationElement ae = elemQueue.Dequeue();
                    AutomationElement child = TreeWalker.RawViewWalker.GetFirstChild(ae);
                    if (child == null)
                    {
                        continue;
                    }
                    else
                    {
                        while (child != null)
                        {
                            returnQueue.Enqueue(child);
                            child = TreeWalker.RawViewWalker.GetNextSibling(child);
                        }
                    }
                }
                
                
                return returnQueue;
            }
            
        }
        private Queue<AutomationElement> ElementXMlFinder(XmlNode elemNode, Queue<AutomationElement> elementQueue, int depth)
        {
            Queue<AutomationElement> Filter1 = new Queue<AutomationElement>();
            Queue<AutomationElement> Filter2 = new Queue<AutomationElement>();

            foreach (XmlAttribute attribute in elemNode.Attributes)
            {

                if (attribute.Name == "Name")
                {
                    while(elementQueue.Count > 0)
                    {
                        AutomationElement ae = elementQueue.Dequeue();
                        if(ae.Current.Name == attribute.Value)
                        {
                            Filter1.Enqueue(ae);
                        }
                    }
                }
                else if (attribute.Name == "Class")
                {
                    while (Filter1.Count > 0)
                    {
                        AutomationElement ae = Filter1.Dequeue();
                        if (ae.Current.ClassName == attribute.Value)
                        {
                            Filter2.Enqueue(ae);
                        }
                    }
                }
            }

            return FindChild(Filter2, depth);
        }
    }
}
