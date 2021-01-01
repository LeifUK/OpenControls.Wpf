using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class ToolThreeView : UserControl
    {
        public ToolThreeView()
        {
            InitializeComponent();
        }

        public OpenControls.Wpf.DockManager.IViewModel IViewModel { get; set; }
    }
}
