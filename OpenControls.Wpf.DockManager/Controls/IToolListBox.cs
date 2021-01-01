using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenControls.Wpf.DockManager.Controls
{
    internal interface IToolListBox
    {
        WindowLocation WindowLocation { get; set; }
        System.Collections.ObjectModel.ObservableCollection<Controls.IToolListBoxItem> ItemsSource { get; set; }
    }
}
