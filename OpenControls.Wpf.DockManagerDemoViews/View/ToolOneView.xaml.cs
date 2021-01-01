using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class ToolOneView : UserControl
    {
        public ToolOneView()
        {
            InitializeComponent();
        }

        public OpenControls.Wpf.DockManager.IViewModel IViewModel { get; set; }
    }
}
