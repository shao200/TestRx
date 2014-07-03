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

            Center = new Point(
                this.rect2985.Width / 2,
                this.rect2985.Height / 2);
        }

        public bool DrawForce { get;private set; }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            DrawForce = false;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var pm = Mouse.GetPosition(this);
            MX = pm.X;
            MY = pm.Y;
            Notify("MX");
            Notify("MY");

            DrawForce = true;
            this.InvalidateVisual();
        }

        public double Magnitude { get; set; }

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
