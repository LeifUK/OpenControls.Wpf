using System;
using System.Windows;
using System.Text.RegularExpressions;

namespace OpenControls.Wpf.Utilities
{
    public static class Utilities
    {
        public static bool Parse(string text, out CornerRadius cornerRadius)
        {
            cornerRadius = new CornerRadius();

            Match match = Regex.Match(text, @"(\d),(\d),(\d),(\d)");
            if (!match.Success)
            {
                return false;
            }

            cornerRadius = new System.Windows.CornerRadius(
                System.Convert.ToDouble(match.Groups[1].Value),
                    System.Convert.ToDouble(match.Groups[2].Value),
                    System.Convert.ToDouble(match.Groups[3].Value),
                    System.Convert.ToDouble(match.Groups[4].Value)
                    );

            return true;
        }

        public static bool Parse(string text, out Thickness thickness)
        {
            thickness = new Thickness();

            Match match = Regex.Match(text, @"(\d),(\d),(\d),(\d)");
            if (!match.Success)
            {
                return false;
            }

            thickness = new System.Windows.Thickness(
                System.Convert.ToDouble(match.Groups[1].Value),
                    System.Convert.ToDouble(match.Groups[2].Value),
                    System.Convert.ToDouble(match.Groups[3].Value),
                    System.Convert.ToDouble(match.Groups[4].Value)
                    );

            return true;
        }

        public static string GetAppDataFolder()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = System.IO.Path.Combine(path, System.AppDomain.CurrentDomain.FriendlyName);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
