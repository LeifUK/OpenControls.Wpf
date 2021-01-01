using System;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal abstract class ViewContainer : Grid, IViewContainer
    {
        protected System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>> _items;
        public OpenControls.Wpf.DockManager.Controls.TabHeaderControl TabHeaderControl;
        protected UserControl _selectedUserControl;
        protected Border _gap;
        protected Button _listButton;

        protected void CreateTabControl(int row, int column)
        {
            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>>();

            TabHeaderControl = new OpenControls.Wpf.DockManager.Controls.TabHeaderControl();
            TabHeaderControl.SelectionChanged += _tabHeaderControl_SelectionChanged;
            TabHeaderControl.CloseTabRequest += _tabHeaderControl_CloseTabRequest;
            TabHeaderControl.FloatTabRequest += _tabHeaderControl_FloatTabRequest;
            TabHeaderControl.ItemsChanged += _tabHeaderControl_ItemsChanged;
            TabHeaderControl.ItemsSource = _items;
            TabHeaderControl.DisplayMemberPath = "Value.Title";
            Children.Add(TabHeaderControl);
            Grid.SetRow(TabHeaderControl, row);
            Grid.SetColumn(TabHeaderControl, column);
        }

        protected abstract void SetSelectedUserControlGridPosition();

        protected void _tabHeaderControl_SelectionChanged(object sender, System.EventArgs e)
        {
            if ((_selectedUserControl != null) && (Children.Contains(_selectedUserControl)))
            {
                Children.Remove(_selectedUserControl);
            }
            _selectedUserControl = null;

            if ((TabHeaderControl.SelectedIndex > -1) && (TabHeaderControl.SelectedIndex < _items.Count))
            {
                _selectedUserControl = _items[TabHeaderControl.SelectedIndex].Key;
                Children.Add(_selectedUserControl);
                SetSelectedUserControlGridPosition();
            }
            CheckTabCount();

            SelectionChanged?.Invoke(sender, e);
        }

        protected void _tabHeaderControl_ItemsChanged(object sender, System.EventArgs e)
        {
            var items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>>();

            foreach (var item in TabHeaderControl.Items)
            {
                items.Add((System.Collections.Generic.KeyValuePair<UserControl, IViewModel>)item);
            }
            int selectedIndex = (TabHeaderControl.SelectedIndex == -1) ? 0 : TabHeaderControl.SelectedIndex;

            _items = items;
            TabHeaderControl.SelectedIndex = selectedIndex;

            _tabHeaderControl_SelectionChanged(this, null);
        }

        protected abstract System.Windows.Forms.DialogResult UserConfirmClose(string documentTitle);

        protected void _tabHeaderControl_CloseTabRequest(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            System.Collections.Generic.KeyValuePair<UserControl, IViewModel> item = (System.Collections.Generic.KeyValuePair<UserControl, IViewModel>)sender;
            if (item.Value.CanClose)
            {
                if (item.Value.HasChanged)
                {
                    System.Windows.Forms.DialogResult dialogResult = UserConfirmClose(item.Value.Title);

                    if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }

                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        item.Value.Save();
                    }
                }

                int index = _items.IndexOf(item);

                _items.RemoveAt(index);
                TabHeaderControl.ItemsSource = _items;

                if (item.Key == _selectedUserControl)
                {
                    Children.Remove(_selectedUserControl);
                    _selectedUserControl = null;

                    if (_items.Count > 0)
                    {
                        if (index >= _items.Count)
                        {
                            --index;
                        }
                        _selectedUserControl = _items[index].Key;
                        Children.Add(_selectedUserControl);
                    }
                }

                CheckTabCount();

                TabClosed?.Invoke(sender, new Events.TabClosedEventArgs() { UserControl = item.Key });
            }
        }

        protected void _tabHeaderControl_FloatTabRequest(object sender, EventArgs e)
        {
            FloatTabRequest?.Invoke(this, e);
        }

        #region IViewContainer

        public event EventHandler SelectionChanged;
        public event Events.TabClosedEventHandler TabClosed;
        public event EventHandler FloatTabRequest;

        public string Title
        {
            get
            {
                if ((_items.Count == 0) || (TabHeaderControl.SelectedIndex == -1))
                {
                    return null;
                }

                return _items[TabHeaderControl.SelectedIndex].Value.Title;
            }
        }

        public string URL
        {
            get
            {
                if ((_items.Count == 0) || (TabHeaderControl.SelectedIndex == -1))
                {
                    return null;
                }

                return _items[TabHeaderControl.SelectedIndex].Value.URL;
            }
        }

        protected abstract void CheckTabCount();

        public void AddUserControl(UserControl userControl)
        {
            System.Diagnostics.Trace.Assert(userControl != null);
            System.Diagnostics.Trace.Assert(userControl.DataContext is IViewModel);

            _items.Add(new System.Collections.Generic.KeyValuePair<UserControl, IViewModel>(userControl, userControl.DataContext as IViewModel));
            TabHeaderControl.SelectedItem = _items[_items.Count - 1];
        }

        public void InsertUserControl(int index, UserControl userControl)
        {
            System.Diagnostics.Trace.Assert(index > -1);
            System.Diagnostics.Trace.Assert(index <= _items.Count);
            System.Diagnostics.Trace.Assert(userControl != null);
            System.Diagnostics.Trace.Assert(userControl.DataContext is IViewModel);

            _items.Insert(index, new System.Collections.Generic.KeyValuePair<UserControl, IViewModel>(userControl, userControl.DataContext as IViewModel));
            CheckTabCount();
        }

        public UserControl ExtractUserControl(int index)
        {
            if ((index < 0) || (index >= _items.Count))
            {
                return null;
            }

            UserControl userControl = _items[index].Key;
            _items.RemoveAt(index);
            TabHeaderControl.ItemsSource = _items;
            if (Children.Contains(userControl))
            {
                Children.Remove(userControl);
            }
            CheckTabCount();

            return userControl;
        }

        public int GetUserControlCount()
        {
            return _items.Count;
        }

        public int SelectedIndex
        {
            get
            {
                return TabHeaderControl.SelectedIndex;
            }
            set
            {
                TabHeaderControl.SelectedIndex = value;
            }
        }

        public UserControl GetUserControl(int index)
        {
            if ((index < 0) || (index >= _items.Count))
            {
                return null;
            }

            return _items[index].Key;
        }

        public IViewModel GetIViewModel(int index)
        {
            UserControl userControl = GetUserControl(index);
            if (userControl == null)
            {
                return null;
            }

            return userControl.DataContext as IViewModel;
        }

        public void ExtractDocuments(IViewContainer sourceViewContainer)
        {
            System.Diagnostics.Trace.Assert(sourceViewContainer != null);

            while (true)
            {
                UserControl userControl = sourceViewContainer.ExtractUserControl(0);
                if (userControl == null)
                {
                    break;
                }

                AddUserControl(userControl);
            }
        }

        #endregion IViewContainer
    }
}
