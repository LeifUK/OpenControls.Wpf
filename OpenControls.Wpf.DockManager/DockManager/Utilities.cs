using System;
using System.Windows;
using System.Text.RegularExpressions;

namespace OpenControls.Wpf.DockManager.Controls
{
    public static class Utilities
    {
        internal static System.Windows.ResourceDictionary GetResourceDictionary()
        {
            return (System.Windows.ResourceDictionary)Application.LoadComponent(new System.Uri("/WpfOpenControls;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
        }

        public const int VK_LBUTTON = 0x01;

        public static bool IsLeftMouseButtonDown()
        {
            return (User32.GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }

        public static void SendMouseButtonPress(IntPtr wndHandle, uint buttonPressCode)
        {
            var input = new User32.INPUT();
            input.Type = User32.MOUSEINPUTTYPE; /// input type mouse
            input.Data.Mouse.Flags = buttonPressCode;

            var inputs = new User32.INPUT[] { input };
            User32.SendInput((uint)inputs.Length, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(User32.INPUT)));
        }

        public static void SendLeftMouseButtonDown(IntPtr wndHandle)
        {
            SendMouseButtonPress(wndHandle, User32.LEFTMOUSEDOWN);
        }

        public static void SendLeftMouseButtonUp(IntPtr wndHandle)
        {
            SendMouseButtonPress(wndHandle, User32.LEFTMOUSEUP);
        }

        // The WPF method does not work properly -> call into User32.dll
        public static Point GetCursorPosition()
        {
            if (User32.GetCursorPos(out User32.POINT point) == false)
            {
                return new Point(0, 0);
            }
            return new Point(point.X, point.Y);
        }

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
