using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenControls.Wpf.DockManager
{
    public static class ExtensionMethods
    {
        private static System.Windows.Forms.Screen[] Screens = System.Windows.Forms.Screen.AllScreens;

        public static System.Windows.Forms.Screen GetCurrentScreen(this System.Windows.Media.Visual visual)
        {
            System.Windows.Forms.Screen currentScreen = Screens[0];

            System.Windows.Point ptOnScreen = visual.PointToScreen(new System.Windows.Point(1, 1));
            foreach (var screen in Screens)
            {
                if (
                        (ptOnScreen.X >= screen.Bounds.X) &&
                        (ptOnScreen.X <= screen.Bounds.Right) &&
                        (ptOnScreen.Y >= screen.Bounds.Y) &&
                        (ptOnScreen.Y <= screen.Bounds.Bottom)
                    )
                {
                    return screen;
                }
            }

            // This should not happen
            return currentScreen;
        }
    }
}
