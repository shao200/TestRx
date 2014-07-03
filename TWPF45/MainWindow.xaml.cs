using MahApps.Metro.Controls;
using ReactiveUI;
using ReactiveUI.Xaml;
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
using TWPF;

namespace TWPF45
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            DataContext =
            ViewModel = new MainWindowViewModel();
            InitializeComponent();

            pg1.SelectedObject = ViewModel;

            this.Bind(ViewModel, x => x.QueryWord);


            this.OneWayBind(ViewModel, x => x.QueryResults, x => x.QueryResults.ItemsSource);

            //QueryButton
            this.OneWayBind(ViewModel, x => x.QueryCommand, x => x.QueryButton.Command);
            this.OneWayBind(ViewModel, x => x.Fetch, x => x.Fetch.Command);

            this.OneWayBind(ViewModel, x => x.IsBusy, x => x.IsBusy.Visibility);

            //this.OneWayBind(ViewModel, x => x.AccentColors, x => x.AccentMenu);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainWindowViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }
    }
}
