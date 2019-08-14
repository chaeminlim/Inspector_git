using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Inspector
{
    class TreeViewWrapper
    {
        public TreeViewItem Node { get; set; }
        
        public TreeViewWrapper(TreeViewItem tvi)
        {
            Node = tvi;
        }

        public void Add(object o)
        {
            this.Node.Items.Add(o);
        }
    }
}
