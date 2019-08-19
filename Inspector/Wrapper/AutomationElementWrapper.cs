using System.Windows.Automation;
using System.Windows.Controls;

namespace Inspector
{
    class AutomationElementWrapper
    {
        public TreeViewItem Node
        {
            get; set;
        }


        public AutomationElementWrapper(AutomationElement ae)
        {
            Node = new TreeViewItem();

            AE = ae;
            if (ae.Current.Name != "")
            {
                Node.Header = ae.Current.Name;

            }
            else
            {
                Node.Header = "No Name";
            }
            Node.Tag = AE;
        }

        public AutomationElement AE
        {
            get;
            private set;
        }

        public void Add(AutomationElementWrapper aew)
        {
            Node.Items.Add(aew.Node);
        }

    }
}
