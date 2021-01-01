using System;
using System.Windows;

namespace OpenControls.Wpf.DockManager.Themes
{
    public abstract class Theme : DependencyObject
    {
        public Theme()
        {

        }

        public abstract Uri Uri
        {
            get;
        }
    }
}
