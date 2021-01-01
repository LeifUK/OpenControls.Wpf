using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Win32;

namespace WpfDockManagerDemo_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ExampleDockManagerViews.ViewModel.MainViewModel();
        }
        
        private string _keyPath = System.Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\OpenControls\WpfDockManagerDemo_2" : @"SOFTWARE\OpenControls\WpfDockManagerDemo_2";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(_keyPath);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(_keyPath);
            }
            else
            {
                Object obj = key.GetValue("Height");
                if (obj != null)
                {
                    Height = Convert.ToDouble(obj);
                }
                obj = key.GetValue("Width");
                if (obj != null)
                {
                    Width = Convert.ToDouble(obj);
                }
                obj = key.GetValue("Top");
                if (obj != null)
                {
                    Top = Convert.ToDouble(obj);
                }
                obj = key.GetValue("Left");
                if (obj != null)
                {
                    Left = Convert.ToDouble(obj);
                }
                obj = key.GetValue("WindowState");
                if (obj != null)
                {
                    if (Enum.TryParse((string)obj, out WindowState windowState))
                    {
                        WindowState = windowState;
                    }
                }
            }

            _layoutManager.Initialise();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(_keyPath, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(_keyPath, true);
            }

            key.SetValue("Height", ActualHeight);
            key.SetValue("Width", ActualWidth);
            key.SetValue("Top", Top);
            key.SetValue("Left", Left);
            key.SetValue("WindowState", WindowState.ToString());

            if (_layoutManager != null)
            {
                _layoutManager.Shutdown();
            }
        }

        private void LoadLayout()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog == null)
            {
                return;
            }

            dialog.Filter = "Layout Files (*.xml)|*.xml";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            ExampleDockManagerViews.ViewModel.MainViewModel mainViewModel = DataContext as ExampleDockManagerViews.ViewModel.MainViewModel;
            System.Diagnostics.Trace.Assert(mainViewModel != null);

            try
            {
                _layoutManager.LoadLayoutFromFile(dialog.FileName);
                mainViewModel.LayoutLoaded = true;
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to load layout: " + exception.Message);
            }
        }

        private void SaveLayout()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog == null)
            {
                return;
            }

            dialog.Filter = "Layout Files (*.xml)|*.xml";
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                _layoutManager.SaveLayoutToFile(dialog.FileName);
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save layout: " + exception.Message);
            }
        }

        private void _buttonCloseTool_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<UserControl, OpenControls.Wpf.DockManager.IViewModel> item = (KeyValuePair<UserControl, OpenControls.Wpf.DockManager.IViewModel>)(sender as Button).DataContext;

            ExampleDockManagerViews.ViewModel.MainViewModel mainViewModel = DataContext as ExampleDockManagerViews.ViewModel.MainViewModel;
            if (mainViewModel.Tools.Contains(item.Value))
            {
                mainViewModel.Tools.Remove(item.Value);
            }
        }

        private void _buttonCloseDocument_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<UserControl, OpenControls.Wpf.DockManager.IViewModel> item = (KeyValuePair<UserControl, OpenControls.Wpf.DockManager.IViewModel>)(sender as Button).DataContext;

            ExampleDockManagerViews.ViewModel.MainViewModel mainViewModel = DataContext as ExampleDockManagerViews.ViewModel.MainViewModel;
            if (mainViewModel.Documents.Contains(item.Value))
            {
                mainViewModel.Documents.Remove(item.Value);
            }
        }

        private void _buttonWindow_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem();
            menuItem.Header = "Save Layout";
            menuItem.IsChecked = false;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { SaveLayout(); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Load Layout";
            menuItem.IsChecked = false;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { LoadLayout(); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private void _buttonTools_Click(object sender, RoutedEventArgs e)
        {
            ExampleDockManagerViews.ViewModel.MainViewModel mainViewModel = DataContext as ExampleDockManagerViews.ViewModel.MainViewModel;
            System.Diagnostics.Trace.Assert(mainViewModel != null);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem();
            menuItem.Header = "Tool One";
            menuItem.IsChecked = mainViewModel.ToolOneVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.ToolOneVisible = !mainViewModel.ToolOneVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Two";
            menuItem.IsChecked = mainViewModel.ToolTwoVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.ToolTwoVisible = !mainViewModel.ToolTwoVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Three";
            menuItem.IsChecked = mainViewModel.ToolThreeVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.ToolThreeVisible = !mainViewModel.ToolThreeVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Four";
            menuItem.IsChecked = mainViewModel.ToolFourVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.ToolFourVisible = !mainViewModel.ToolFourVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Five";
            menuItem.IsChecked = mainViewModel.ToolFiveVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.ToolFiveVisible = !mainViewModel.ToolFiveVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private void _buttonDocuments_Click(object sender, RoutedEventArgs e)
        {
            ExampleDockManagerViews.ViewModel.MainViewModel mainViewModel = DataContext as ExampleDockManagerViews.ViewModel.MainViewModel;
            System.Diagnostics.Trace.Assert(mainViewModel != null);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentOne.URL;
            menuItem.IsChecked = mainViewModel.DocumentOneVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.DocumentOneVisible = !mainViewModel.DocumentOneVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentTwo.URL;
            menuItem.IsChecked = mainViewModel.DocumentTwoVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.DocumentTwoVisible = !mainViewModel.DocumentTwoVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentThree.URL;
            menuItem.IsChecked = mainViewModel.DocumentThreeVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.DocumentThreeVisible = !mainViewModel.DocumentThreeVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentFour.URL;
            menuItem.IsChecked = mainViewModel.DocumentFourVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.DocumentFourVisible = !mainViewModel.DocumentFourVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentFive.URL;
            menuItem.IsChecked = mainViewModel.DocumentFiveVisible;
            menuItem.Command = new OpenControls.Wpf.DockManager.Command(delegate { mainViewModel.DocumentFiveVisible = !mainViewModel.DocumentFiveVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }
    }
}
