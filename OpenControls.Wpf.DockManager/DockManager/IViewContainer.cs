using System;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface IViewContainer
    {
        string Title { get; }
        string URL { get; }
        void AddUserControl(UserControl userControl);
        void InsertUserControl(int index, UserControl userControl);
        UserControl ExtractUserControl(int index);
        int GetUserControlCount();
        int SelectedIndex { get; set; }
        UserControl GetUserControl(int index);
        IViewModel GetIViewModel(int index);
        void ExtractDocuments(IViewContainer sourceViewContainer);

        event EventHandler SelectionChanged;
        event Events.TabClosedEventHandler TabClosed;
        event EventHandler FloatTabRequest;
    }
}
