using System;
using System.Collections.Generic;

namespace OpenControls.Wpf.DockManager
{
    /*
     * Stores information for a ToolPane that has been unpinned
     */
    internal class UnpinnedToolData
    {
        public UnpinnedToolData()
        {
            Items = new List<ToolListBoxItem>();
        }

        public ToolPaneGroup ToolPaneGroup { get; set; }
        public Guid SiblingGuid { get; set; }

        public List<ToolListBoxItem> Items;

        // These define the original location relative to the Sibling pane
        public bool IsHorizontal { get; set; }
        public bool IsFirst { get; set; }
    }
}
