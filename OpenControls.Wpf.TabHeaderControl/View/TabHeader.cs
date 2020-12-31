using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace OpenControls.Wpf.TabHeaderControl
{
    public class TabHeader : ListBox
    {
        public event EventHandler ItemsChanged;
        public event EventHandler FloatTabRequest;

        private bool _mouseLeftButtonDown = false;
        private int _dragIndex;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            _mouseLeftButtonDown = true;

            // SelectedIndex is yet to be set so we must find it outselves ... 
            Point cursorScreenPosition = OpenControls.Wpf.Utilities.Windows.GetCursorPosition();
            _dragIndex = GetListBoxItemIndex(cursorScreenPosition);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            _mouseLeftButtonDown = false;
        }

        private ListBoxItem GetListBoxItem(int index)
        {
            if (
                (index < 0) ||
                (index >= Items.Count) ||
                (ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated))
            {
                return null;
            }

            return ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        }

        private bool IsMouseOverListBoxItem(Point cursorScreenPosition, int index, out Rect bounds)
        {
            ListBoxItem item = GetListBoxItem(index);
            Point cursorItemPosition = item.PointFromScreen(cursorScreenPosition);
            Point itemTopLeft = item.PointToScreen(new Point(0, 0));
            bounds = new Rect(itemTopLeft.X, itemTopLeft.Y, item.ActualWidth, item.ActualHeight);
            return (cursorItemPosition.X >= 0) && (cursorItemPosition.Y >= 0) && (cursorItemPosition.X <= item.ActualWidth) && (cursorItemPosition.Y <= item.ActualHeight);
        }

        private int GetListBoxItemIndex(Point cursorScreenPosition)
        {
            for (int index = 0; index < Items.Count; ++index)
            {
                if (IsMouseOverListBoxItem(cursorScreenPosition, index, out Rect bounds))
                {
                    //System.Diagnostics.Debug.WriteLine("_listBox_MouseMove: " + _listBox.SelectedIndex + "," + index);
                    return index;
                }
            }

            return -1;
        }

        private Rect GetListBoxItemBounds(int index)
        {
            ListBoxItem item = GetListBoxItem(index);
            if (item == null)
            {
                return new Rect();
            }
            Point itemTopLeft = item.PointToScreen(new Point(0, 0));
            return new Rect(itemTopLeft.X, itemTopLeft.Y, item.ActualWidth, item.ActualHeight);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            e.Handled = true;
            if (!_mouseLeftButtonDown)
            {
                return;
            }

            Point cursorScreenPosition = OpenControls.Wpf.Utilities.Windows.GetCursorPosition();
            int selectedIndex = GetListBoxItemIndex(cursorScreenPosition);

            if ((selectedIndex < 0) || (selectedIndex >= Items.Count) || (selectedIndex == _dragIndex))
            {
                Point topLeftPoint = PointToScreen(new Point(0, 0));
                if (
                        (cursorScreenPosition.Y < (topLeftPoint.Y - 30)) ||
                        (cursorScreenPosition.Y > (topLeftPoint.Y + ActualHeight + 15)) ||
                        (cursorScreenPosition.X < (topLeftPoint.X - 30)) ||
                        (cursorScreenPosition.X > (topLeftPoint.X + ActualWidth + 15))
                   )
                {
                    // Cancel further drag processing until we get the next mouse left button down
                    _mouseLeftButtonDown = false;
                    System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
                    System.Diagnostics.Debug.WriteLine("FloatTabRequest: count = " + Items.Count);
                    FloatTabRequest?.Invoke(this, null);
                }
                return;
            }

            Rect rectSelectedItem = GetListBoxItemBounds(selectedIndex);
            Rect rectDragItem = GetListBoxItemBounds(_dragIndex);

            double selectedWidth = rectSelectedItem.Width;
            double currentWidth = rectDragItem.Width;

            if (_dragIndex > SelectedIndex)
            {
                selectedWidth = -selectedWidth;
            }

            rectDragItem.Offset(selectedWidth, 0);
            if (!rectDragItem.Contains(cursorScreenPosition))
            {
                return;
            }

            // Move the item along to the new index
            var item = Items[_dragIndex];
            Items.Remove(item);
            Items.Insert(selectedIndex, item);
            _dragIndex = selectedIndex;
            SelectedIndex = selectedIndex;

            ItemsChanged?.Invoke(this, null);
        }
    }
}
