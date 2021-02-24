using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenControls.WinForms.SurfacePlotterDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Controls.Add(_surfacePlotControl);

            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonSettings.Enabled = false;
            _configuration = new OpenControls.Wpf.SurfacePlot.Model.Configuration();
            OpenControls.Wpf.SurfacePlot.Model.ConfigurationSerialiser configurationSerialiser = new Wpf.SurfacePlot.Model.ConfigurationSerialiser();
            configurationSerialiser.CurrentRegistryKey = OpenRegKey();
            _configuration.Load(configurationSerialiser);

            _surfacePlotControl.Initialise(_configuration);
        }

        private Microsoft.Win32.RegistryKey OpenRegKey()
        {
            string path = System.Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\OpenControls.Wpf.SurfacePlotDemo" : @"SOFTWARE\OpenControls.Wpf.SurfacePlotDemo";
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, true);
            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
            }

            if (key == null)
            {
                return null;
            }

            Microsoft.Win32.RegistryKey settingsKey = key.OpenSubKey("RawDataSettings", true);
            if (settingsKey == null)
            {
                settingsKey = key.CreateSubKey("RawDataSettings");
            }

            return settingsKey;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_surfacePlotControl != null)
            {
                _surfacePlotControl.SetBounds(
                    ClientRectangle.X,
                    ClientRectangle.Y,
                    ClientRectangle.Width,
                    ClientRectangle.Height);
            }
        }

        OpenControls.Wpf.SurfacePlot.SurfacePlotControl _surfacePlotControl = new OpenControls.Wpf.SurfacePlot.SurfacePlotControl();
        OpenControls.Wpf.SurfacePlot.Model.Configuration _configuration;
        private void Run()
        {
            buttonSettings.Enabled = true;

            const int YCount = 50;
            const int XCount = 50;
            int counter = 0;
            float zMax = 150;
            float zMin = -150;
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

            _surfacePlotControl.SetData(drawData, -50, 50, 21, -50, 50, 21, zMin, zMax, 21);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            OpenControls.Wpf.SurfacePlot.Exports.ShowConfigurationDialog(_configuration);
        }
    }
}
