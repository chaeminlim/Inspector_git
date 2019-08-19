using System.Windows.Automation;
using System.Windows.Controls;

namespace Inspector
{
    class PatternInfo
    {
        public int PatternType { get; set; } // 패턴을 구별
        public int MethodType { get; set; } // 패턴 안의 메서드를 구별
        public AutomationElement selectedItem { get; set; }
        public BasePattern PatternObject { get; set; }
        public PatternInfo(int pattype, int mType, AutomationElement item, BasePattern patObj)
        {
            PatternType = pattype;
            MethodType = mType;
            selectedItem = item;
            PatternObject = patObj;
        }

        internal void PatternDistinguisher()
        {
            switch (PatternType)
            {
                case 1:
                    ScrollPatternController();
                    break;
                case 2:
                    DockPatternController();
                    break;
                case 3:
                    ExpandCollapsePatternController();
                    break;
                case 4:
                    GridPatternController();
                    break;
                case 5:
                    break;
                case 6:
                    InvokePatternController();
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    RangeValuePatternController();
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    TogglePatternController();
                    break;
                case 16:
                    TransformPatternController();
                    break;
                case 17:
                    ValuePatternController();
                    break;
                case 18:
                    break;
                case 19:
                    WindowPatternController();
                    break;


            }

        }

        private void WindowPatternController()
        {
            WindowPattern windowPattern = (WindowPattern)PatternObject;

            if (MethodType == 1)
            {
            }
        }

        private void ValuePatternController()
        {
            ValuePattern valuePattern = (ValuePattern)PatternObject;

            if (MethodType == 1)
            {
                valuePattern.SetValue("not implemented");
            }
        }

        private void TransformPatternController()
        {
            TransformPattern transformPattern = (TransformPattern)PatternObject;

            if (MethodType == 1)
            {

            }
            else if (MethodType == 2)
            {

            }
            else if (MethodType == 3)
            {

            }
        }

        private void TogglePatternController()
        {
            TogglePattern togglePattern = (TogglePattern)PatternObject;

            if (MethodType == 1)
            {
                togglePattern.Toggle();
            }
        }

        private void RangeValuePatternController()
        {
            RangeValuePattern rangeValuePattern = (RangeValuePattern)PatternObject;

            if (MethodType == 1)
            {

            }
        }

        private void InvokePatternController()
        {
            InvokePattern invokePattern = (InvokePattern)PatternObject;
            if (MethodType == 1)
            {
                invokePattern.Invoke();
            }
        }

        private void ScrollPatternController()
        {
            if (MethodType == 1)
            {

            }
        }
        private void DockPatternController()
        {
            if (MethodType == 1)
            {

            }
        }
        private void ExpandCollapsePatternController()
        {
            ExpandCollapsePattern expandCollapsePattern = (ExpandCollapsePattern)PatternObject;

            if (MethodType == 1)
            {
                expandCollapsePattern.Expand();
            }
            else if (MethodType == 2)
            {
                expandCollapsePattern.Collapse();
            }
        }
        private void GridPatternController()
        {
            if (MethodType == 1)
            {

            }
        }






    }
    class PatternControl
    {
        public void PatternController(AutomationPattern pat, AutomationElement selectedItem, ListView listView2)
        {
            if (pat == ScrollPattern.Pattern)
            {
                ScrollPattern scrollPattern = selectedItem.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "Scroll - not implemented";
                lvi.Tag = new PatternInfo(1, 1, selectedItem, scrollPattern);
                listView2.Items.Add(lvi);
            }
            else if (pat == DockPattern.Pattern)
            {
                DockPattern dockPattern = selectedItem.GetCurrentPattern(DockPattern.Pattern) as DockPattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "SetDockPosition - not implemented";
                lvi.Tag = new PatternInfo(2, 1, selectedItem, dockPattern);
                listView2.Items.Add(lvi);

            }
            else if (pat == ExpandCollapsePattern.Pattern)
            {
                ExpandCollapsePattern expandCollapsePattern = selectedItem.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

                ListViewItem lvi1 = new ListViewItem();
                lvi1.Content = "Expand";
                lvi1.Tag = new PatternInfo(3, 1, selectedItem, expandCollapsePattern);
                listView2.Items.Add(lvi1);

                ListViewItem lvi2 = new ListViewItem();
                lvi2.Content = "Collapse";
                lvi2.Tag = new PatternInfo(3, 2, selectedItem, expandCollapsePattern);
                listView2.Items.Add(lvi2);
            }
            else if (pat == GridPattern.Pattern)
            {
                GridPattern gridPattern = selectedItem.GetCurrentPattern(GridPattern.Pattern) as GridPattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "GetItem - not implemented";
                lvi.Tag = new PatternInfo(4, 1, selectedItem, gridPattern);
                listView2.Items.Add(lvi);
            }
            else if (pat == GridItemPattern.Pattern)
            {
            }
            else if (pat == InvokePattern.Pattern)
            {
                InvokePattern invokePattern = selectedItem.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "Invoke";
                lvi.Tag = new PatternInfo(6, 1, selectedItem, invokePattern);
                listView2.Items.Add(lvi);
            }
            else if (pat == ItemContainerPattern.Pattern)
            {
            }
            else if (pat == MultipleViewPattern.Pattern)
            {
            }
            else if (pat == RangeValuePattern.Pattern)
            {
                RangeValuePattern rangeValuePattern = selectedItem.GetCurrentPattern(RangeValuePattern.Pattern) as RangeValuePattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "SetValue - not implemented";
                lvi.Tag = new PatternInfo(9, 1, selectedItem, rangeValuePattern);
                listView2.Items.Add(lvi);
            }
            else if (pat == ScrollItemPattern.Pattern)
            {
            }
            else if (pat == SelectionItemPattern.Pattern)
            {
            }
            else if (pat == SelectionPattern.Pattern)
            {
            }
            else if (pat == SynchronizedInputPattern.Pattern)
            {
            }
            else if (pat == TextPattern.Pattern)
            {
            }
            else if (pat == TogglePattern.Pattern)
            {
                TogglePattern togglePattern = selectedItem.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "Toggle";
                lvi.Tag = new PatternInfo(15, 1, selectedItem, togglePattern);
                listView2.Items.Add(lvi);
            }
            else if (pat == TransformPattern.Pattern)
            {
                TransformPattern transformPattern = selectedItem.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "Move - not implemented";
                lvi.Tag = new PatternInfo(16, 1, selectedItem, transformPattern);
                listView2.Items.Add(lvi);

                ListViewItem lvi2 = new ListViewItem();
                lvi2.Content = "Resize - not implemented";
                lvi2.Tag = new PatternInfo(16, 2, selectedItem, transformPattern);
                listView2.Items.Add(lvi2);

                ListViewItem lvi3 = new ListViewItem();
                lvi3.Content = "Rotate - not implemented";
                lvi3.Tag = new PatternInfo(16, 3, selectedItem, transformPattern);
                listView2.Items.Add(lvi3);
            }
            else if (pat == ValuePattern.Pattern)
            {
                ValuePattern valuePattern = selectedItem.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "setValue";
                lvi.Tag = new PatternInfo(17, 1, selectedItem, valuePattern);
                listView2.Items.Add(lvi);
            }
            else if (pat == VirtualizedItemPattern.Pattern)
            {
            }
            else if (pat == WindowPattern.Pattern)
            {
                WindowPattern windowPattern = selectedItem.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

                ListViewItem lvi = new ListViewItem();
                lvi.Content = "SetWindowVisualState - not implemented";
                lvi.Tag = new PatternInfo(19, 1, selectedItem, windowPattern);
                listView2.Items.Add(lvi);
            }
            else
            { listView2.Items.Add("else"); }
        }
    }
}
