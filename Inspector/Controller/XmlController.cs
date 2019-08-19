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

            XmlElement xmlElement = tree.CreateElement("Window");
            AutomationElement wae = automationElements.Pop();
            Process p = Process.GetProcessById(wae.Current.ProcessId);
            
            xmlElement.SetAttribute("app", p.MainModule.ModuleName);
            xmlElement.SetAttribute("class", wae.Current.ClassName);

            root.AppendChild(xmlElement);

            while (automationElements.Count > 0)
            {
                AutomationElement ae = automationElements.Pop();
                xmlElement = null;
                xmlElement = tree.CreateElement("element");
                xmlElement.SetAttribute("name", ae.Current.Name);
                xmlElement.SetAttribute("AutomationID", ae.Current.AutomationId);
                xmlElement.SetAttribute("ControlType", ae.Current.ControlType.ProgrammaticName);
                xmlElement.SetAttribute("class", ae.Current.ClassName);


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

    }
}
