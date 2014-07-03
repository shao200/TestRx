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

namespace TWPF45
{
    /// <summary>
    /// Interaction logic for MDraw.xaml
    /// </summary>
    public partial class MDraw : UserControl
    {
        public MDraw()
        {
            InitializeComponent();
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

            DrawForce = true;
            this.InvalidateVisual();
        }

        double MX { get; set; }
        double MY { get; set; }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!DrawForce) return;

            Pen pen = new Pen(
            (SolidColorBrush)FindResource("AccentColorBrush"), 2);
            Point pt1 = new Point(this.rect2985.Width / 2, this.rect2985.Height / 2);
            Point pt2 = new Point(MX, MY);
            drawingContext.DrawLine(pen, pt1, pt2);
        }
    }
}
