using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;

namespace OpenControls.Wpf.SurfacePlotterDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, OpenControls.Wpf.SurfacePlot.Model.ILabelFormatter
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenControls.Wpf.SurfacePlotterDemo.ViewModel.MainViewModel mainViewModel = new OpenControls.Wpf.SurfacePlotterDemo.ViewModel.MainViewModel();
            DataContext = mainViewModel;

            mainViewModel.Load();
            _configurationControl.DataContext = new OpenControls.Wpf.SurfacePlot.ViewModel.ConfigurationControlViewModel(mainViewModel.IConfiguration);
            _surfacePlotControl.Initialise(mainViewModel.IConfiguration);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            (DataContext as OpenControls.Wpf.SurfacePlotterDemo.ViewModel.MainViewModel).Save();
            base.OnClosing(e);
        }

        System.Threading.Tasks.Task _task;
        private void _buttonStart_Click(object sender, RoutedEventArgs e)
        {
            OpenControls.Wpf.SurfacePlotterDemo.ViewModel.MainViewModel mainViewModel = (DataContext as OpenControls.Wpf.SurfacePlotterDemo.ViewModel.MainViewModel);

            int algorithm = mainViewModel.SelectedSpeed;
            int sleepIntervalInMSecs = 1000 / mainViewModel.SelectedSpeed;

            mainViewModel.IsRunning = true;
            _task = new System.Threading.Tasks.Task(new System.Action(delegate
            {
                _surfacePlotControl.ILabelFormatter = this;

                const int YCount = 100;
                const int XCount = 100;
                int counter = 0;
                float zMax = 160;
                float zMin = -160;
                float scale = 2f * (float)System.Math.PI / (float)XCount;

                List<List<float>> srcData = new List<List<float>>();
                for (int i = 0; i < XCount; ++i)
                {
                    List<float> list = new List<float>();
                    srcData.Add(list);
                    for (int j = 0; j < YCount; ++j)
                    {
                        list.Add((float)(zMax * System.Math.Sin(scale * i) * System.Math.Sin(scale * j)));
                    }
                }

                _surfacePlotControl.XAxisTitle = "Number of Samples (X)";
                _surfacePlotControl.YAxisTitle = "Population Count (Y)";
                _surfacePlotControl.ZAxisTitle = "Adjusted Sigma Delta (Z)";

                while (mainViewModel.IsRunning == true)
                {
                    List<List<float>> drawData = new List<List<float>>();

                    for (int i = 0; i < XCount; ++i)
                    {
                        int offset = i + counter;
                        while (offset >= XCount)
                        {
                            offset -= XCount;
                        }
                        List<float> list = new List<float>();
                        drawData.Add(list);
                        for (int j = 0; j < YCount; ++j)
                        {
                            list.Add(srcData[offset][j]);
                        }
                    }

                    this.Dispatcher.Invoke(delegate
                    {
                        _surfacePlotControl.SetData(drawData, 0, 10, 11, 0, 100, 11, zMin, zMax, 11);
                    });

                    counter += 1;
                    if (counter >= XCount)
                    {
                        counter = 0;
                    }

                    System.Threading.Thread.Sleep(sleepIntervalInMSecs);
                }
            }));
            _task.Start();
        }

        private void _buttonStop_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as OpenControls.Wpf.SurfacePlotterDemo.ViewModel.MainViewModel).IsRunning = false;
            _task = null;
        }

        #region OpenControls.Wpf.SurfacePlot.Model.ILabelFormatter

        public string XLabel(float x)
        {
            return x.ToString("F1");
        }

        public string YLabel(float y)
        {
            return y.ToString("F1");
        }

        public string ZLabel(float z)
        {
            return z.ToString("E2");
        }

        #endregion OpenControls.Wpf.SurfacePlot.Model.ILabelFormatter
    }
}
