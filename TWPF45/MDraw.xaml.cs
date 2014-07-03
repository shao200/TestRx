using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TWPF45
{
    /// <summary>
    /// Interaction logic for MDraw.xaml
    /// </summary>
    public partial class MDraw : UserControl ,INotifyPropertyChanged
    {
        public MDraw()
        {
            InitializeComponent();
            g3809.DataContext = this;
            Center = new Point(
                this.rect2985.Width / 2,
                this.rect2985.Height / 2);
        }

        public bool DrawForce { get;private set; }

        double MDot(double xv, double yv)
        {
            var res = xv * MDX + yv * MDY;
            if (res < minOpacity) res = minOpacity;
            else if (res > 0.989) res = 0.99;
            return res;
        }

        void ProcessMouse(MouseButtonState mbs)
        {
            if (mbs == MouseButtonState.Pressed)
            {
                var pm = Mouse.GetPosition(this);
                MX = pm.X;
                MY = pm.Y;
                MDX = (MX - Center.X) / Center.X;
                MDY = (Center.Y - MY) / Center.Y;

                MRHMagnitude = MDot(-1, 1);
                MRLMagnitude = MDot(-1, -1);

                MLHMagnitude = MDot(1, 1);
                MLLMagnitude = MDot(1, -1);

                //MagnitudePercent = Math.Max(Math.Abs(MDX), Math.Abs(MDY));

                Notify("MX");
                Notify("MY");
                Notify("MDX");
                Notify("MDY");

                //Notify("MagnitudePercent");
                DrawForce = true;
                this.InvalidateVisual();
            }
            else
            {
                if (DrawForce == true)
                {
                    DrawForce = false;
                    MRHMagnitude =
                    MRLMagnitude =

                    MLHMagnitude =
                    MLLMagnitude = minOpacity;
                    this.InvalidateVisual();
                }
            }           
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            ProcessMouse(e.LeftButton);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            ProcessMouse(e.LeftButton);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            DrawForce = false;
            ProcessMouse(e.LeftButton);
        }

        const double minOpacity = 0.2;

        public static readonly DependencyProperty MagnitudePercentProperty =
    DependencyProperty.Register("MagnitudePercent", typeof(double), typeof(MDraw), new FrameworkPropertyMetadata(minOpacity, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MagnitudePercent {
            get { return (double)this.GetValue(MagnitudePercentProperty); }
            set { this.SetValue(MagnitudePercentProperty, value); }
        }

        /// <summary>
        /// Right High Channel
        /// </summary>
        public static readonly DependencyProperty MRHMagnitudeProperty =
    DependencyProperty.Register("MRHMagnitude", typeof(
double), typeof(MDraw), new FrameworkPropertyMetadata(minOpacity, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MRHMagnitude
        {
            get { return (double)this.GetValue(MRHMagnitudeProperty); }
            set { this.SetValue(MRHMagnitudeProperty, value); }
        }


        /// <summary>
        /// Right Low Channel
        /// </summary>
        public static readonly DependencyProperty MRLMagnitudeProperty =
    DependencyProperty.Register("MRLMagnitude", typeof(
double), typeof(MDraw), new FrameworkPropertyMetadata(minOpacity, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MRLMagnitude
        {
            get { return (double)this.GetValue(MRLMagnitudeProperty); }
            set { this.SetValue(MRLMagnitudeProperty, value); }
        }

        /// <summary>
        /// Left High Channel
        /// </summary>
        public static readonly DependencyProperty MLHMagnitudeProperty =
    DependencyProperty.Register("MLHMagnitude", typeof(
double), typeof(MDraw), new FrameworkPropertyMetadata(minOpacity, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MLHMagnitude
        {
            get { return (double)this.GetValue(MLHMagnitudeProperty); }
            set { this.SetValue(MLHMagnitudeProperty, value); }
        }


        /// <summary>
        /// Left Low Channel
        /// </summary>
        public static readonly DependencyProperty MLLMagnitudeProperty =
    DependencyProperty.Register("MLLMagnitude", typeof(
double), typeof(MDraw), new FrameworkPropertyMetadata(minOpacity, FrameworkPropertyMetadataOptions.AffectsRender));

        public double MLLMagnitude
        {
            get { return (double)this.GetValue(MLLMagnitudeProperty); }
            set { this.SetValue(MLLMagnitudeProperty, value); }
        }


        public double Magnitude { get; set; }



        public double MDX { get; set; }
        public double MDY { get; set; }
        public double MX { get; set; }
        public double MY { get; set; }
        Point Center;
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!DrawForce) return;

            Pen pen = new Pen(
            (SolidColorBrush)FindResource("AccentColorBrush"), 2);
            var ptc = new Point(MX, MY);
            var ptx = new Point(MX, Center.Y);
            var pty = new Point(Center.X, MY);

            drawingContext.DrawLine(pen, ptx, Center);

            drawingContext.DrawLine(pen, pty, Center);

            drawingContext.DrawLine(pen, ptc, Center);
        
        }

        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
