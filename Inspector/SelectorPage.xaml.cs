using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using System.Xml;

namespace Inspector
{
    /// <summary>
    /// SelectorPage.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 


    public partial class SelectorPage : Window
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
        [DllImport("gdi32.dll")]
        static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        public System.Windows.Point Point;
        private MouseHook mouseHook;
        private DispatcherTimer timer;
        private AutomationElement selectedElement { get; set; }


        public SelectorPage()
        {
            InitializeComponent();
            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            mouseHook = new MouseHook();
            timer = new DispatcherTimer();    //객체생성

            

        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            mouseHook.MouseMove += MouseHook_MouseMove;
            mouseHook.LeftButtonDown += MouseHook_LeftButtonDown;
            mouseHook.Install();

            //EventHandler EventHandler1 = new EventHandler();
            //timer.Interval = TimeSpan.FromMilliseconds(1000);    //시간간격 설정
            //timer.Tick += EventHandler1;
            //timer.Start();

        }
       


        private void MouseHook_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            ButtonStop_Click("Hooker", null);
        }


        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mouseHook.Uninstall();

            if(sender as string== "Hooker")
            {
                try
                {
                    AutomationElement ae = AutomationElement.FromPoint(Point);
                    Stack<AutomationElement> automationElements = XmlController.MakeStack(ae);
                    String result = XmlController.MakeXmlFile(automationElements);
                    TextBox1.Text = result;
                    this.WindowState = WindowState.Normal;
                }
                catch (COMException)
                {
                    MessageBox.Show("응용 프로그램이 입력 동기화된 호출을 전달하고 있으므로 나가는 호출을 할 수 없습니다." +
                        "(예외가 발생한 HRESULT: 0x8001010D (RPC_E_CANTCALLOUT_ININPUTSYNCCALL))"); 
                }
                
            }

            

        }

        private void FindFromXml_Click(object sender, RoutedEventArgs e)
        {
            string xmlData = TextBox1.Text;
            AutomationElement returnVal = XmlController.ReadXml(xmlData);
            if(returnVal == null)
            {
                MessageBox.Show(" 못찾았습니다. 혹은 에러입니다. ");
            }
            else
            {
                MessageBox.Show("찾았습니다." + returnVal.Current.Name);
                HighLightElement(returnVal);
            }
        }

        private void HighLightElement(AutomationElement returnVal)
        {
            IntPtr dc = GetDC((IntPtr)returnVal.Current.NativeWindowHandle);
            Rect rect = returnVal.Current.BoundingRectangle;
            Rectangle(dc, (int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);


            ReleaseDC((IntPtr)returnVal.Current.NativeWindowHandle, dc);
        }

        private void DrawBox(object sender, EventArgs e)
        {
            AutomationElement ae = AutomationElement.FromPoint(Point);


            Graphics graphics = Graphics.FromHwnd((IntPtr)ae.Current.NativeWindowHandle);


            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Aqua);

            graphics.DrawRectangle(
                pen,
                (float)ae.Current.BoundingRectangle.X, (float)ae.Current.BoundingRectangle.Y,
                (float)ae.Current.BoundingRectangle.Width, (float)ae.Current.BoundingRectangle.Height);

            graphics.Dispose();


            //SolidBrush semiTransBrush = new SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 255));


            //graphics.Dispose();



            //     이 System.Drawing.Graphics의 System.Drawing.Graphics.GetHdc 메서드에 대한 이전 호출에서 얻은
            //     장치 컨텍스트 핸들을 해제합니다.
            //Graphics g = Graphics.FromHdc(desktopPtr);
            //System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Blue);
            //System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow);
            //System.Drawing.Rectangle Rect = new System.Drawing.Rectangle(
            //    (int)((AutomationElement)ae).Current.BoundingRectangle.X,
            //    (int)((AutomationElement)ae).Current.BoundingRectangle.Y,
            //    (int)((AutomationElement)ae).Current.BoundingRectangle.Width,
            //    (int)((AutomationElement)ae).Current.BoundingRectangle.Height);
            //g.FillRectangle(brush, Rect);
            //g.DrawRectangle(pen, Rect);
            //g.Dispose();

        }
        private void MouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Point.X = mouseStruct.pt.x;
            Point.Y = mouseStruct.pt.y;

        }

        private void ButtonLoadXml_Click(object sender, RoutedEventArgs e)
        {
            // 외부 파일을 불러오는 기능을 구현할 곳
        }
    }
}

