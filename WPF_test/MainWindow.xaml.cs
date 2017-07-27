using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System.Windows.Threading;


namespace WPF_test
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    { 
        private Capture capture;
        private CascadeClassifier face_cascade;
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            capture = new Capture();
            string face_cascade_name = @"haarcascade_frontalface_alt_tree.xml";
            try
            {  
                face_cascade = new CascadeClassifier(face_cascade_name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't connect" + face_cascade_name+"\r\n"+ex.Message);             
            }
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Start();

        }
        private void timer_tick(object sender,EventArgs e)
        {
            Image<Bgr,Byte> cam = capture.QueryFrame().ToImage<Bgr,Byte>();
            if (cam!=null)
            {
                Image<Gray, Byte> gcam = cam.Convert<Gray, Byte>();
                var detected_face = face_cascade.DetectMultiScale(gcam);
                foreach(var face in detected_face)
                {
                    cam.Draw(face, new Bgr(0, 255, 0));
                }
                image.Source=ToBitmapSource(cam);
            }
        }
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop
                  .Imaging.CreateBitmapSourceFromHBitmap(
                  ptr,
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
