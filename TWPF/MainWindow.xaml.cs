using ReactiveUI;
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

namespace TWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
    {
        /// <summary>
        /// The dependency property declaration for the ViewModel.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(MainWindowViewModel),
                typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();

            // We need to bind the ViewModel property to the DataContext in order to be able to
            // use WPF Bindings. Let's use WPF bindings for the UserName property.
            //this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

            DataContext = ViewModel=new MainWindowViewModel();
            

            this.Bind(ViewModel, x => x.QueryWord);
            //this.Bind(ViewModel, x => x.QueryResults);

            // For the LoginCommand on the other hand, we must use a one way binding and explicitly
            // specify the control and the property being bound to.
            this.OneWayBind(ViewModel, x => x.QueryCommand, x => x.QueryButton.Command);

        }

        /// <summary>
        /// Gets or sets the view's ViewModel.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)this.GetValue(ViewModelProperty); }
            set { this.SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the view's ViewModel.
        /// </summary>
        object IViewFor.ViewModel
        {
            get { return this.ViewModel; }
            set { this.ViewModel = (MainWindowViewModel)value; }
        }

    }
}
