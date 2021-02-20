using System;
using System.Windows;

namespace OpenControls.Wpf.Utilities
{
    public static class Windows
    {
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

        public static Point ScaleByDpi(Point point)
        {
            return User32.ConvertPixelsToUnits(point);
        }
    }
}
