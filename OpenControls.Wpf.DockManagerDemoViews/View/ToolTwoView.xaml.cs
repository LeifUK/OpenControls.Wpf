using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoTwoView.xaml
    /// </summary>
    public partial class ToolTwoView : UserControl
    {
        public ToolTwoView()
        {
            InitializeComponent();
        }

        public OpenControls.Wpf.DockManager.IViewModel IViewModel { get; set; }
    }
}
