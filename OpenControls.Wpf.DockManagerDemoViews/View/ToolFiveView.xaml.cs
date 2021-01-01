using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoFiveView.xaml
    /// </summary>
    public partial class ToolFiveView : UserControl
    {
        public ToolFiveView()
        {
            InitializeComponent();
        }

        public OpenControls.Wpf.DockManager.IViewModel IViewModel { get; set; }
    }
}
