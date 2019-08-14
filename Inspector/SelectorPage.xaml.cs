using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Inspector
{
    /// <summary>
    /// SelectorPage.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 


    public partial class SelectorPage : Window
    {

        public SelectorPage()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            AutomationFocusChangedEventHandler afceh =
                new AutomationFocusChangedEventHandler(FocusChanged);
            Automation.AddAutomationFocusChangedEventHandler(afceh);
            

        }

        public void FocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate {
                AutomationElement ae = AutomationElement.FocusedElement;
                label1.Content = ae.Current.ControlType;
            }));

        }


        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        const uint MOUSEMOVE = 0x0001;      // 마우스 이동
        const uint ABSOLUTEMOVE = 0x8000;   // 전역 위치
        const uint LBUTTONDOWN = 0x0002;    // 왼쪽 마우스 버튼 눌림
        const uint LBUTTONUP = 0x0004;      // 왼쪽 마우스 버튼 떼어짐
        const uint RBUTTONDOWN = 0x0008;    // 오른쪽 마우스 버튼 눌림
        const uint RBUTTONUP = 0x00010;      // 오른쪽 마우스 버튼 떼어짐

        
    }
}
