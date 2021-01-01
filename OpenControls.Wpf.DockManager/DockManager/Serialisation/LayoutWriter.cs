using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager.Serialisation
{
    internal static class LayoutWriter
    {
        private static void AddAttribute(XmlDocument xmlDocument, XmlElement xmlElement, string name, string value)
        {
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(name);
            xmlAttribute.Value = value;
            xmlElement.Attributes.Append(xmlAttribute);
        }

        private static void AddGuidAttribute(XmlDocument xmlDocument, XmlElement xmlElement, FrameworkElement frameworkElement)
        {
            AddAttribute(xmlDocument, xmlElement, "Guid", frameworkElement.Tag.ToString());
        }

        private static void AddWidthAndHeightAttributes(XmlDocument xmlDocument, XmlElement xmlElement, Grid grid)
        {
            AddAttribute(xmlDocument, xmlElement, "Width", grid.ActualWidth.ToString());
            AddAttribute(xmlDocument, xmlElement, "Height", grid.ActualHeight.ToString());
        }

        private static XmlElement AddToolPaneGroupNode(XmlDocument xmlDocument, XmlNode xmlParentNode, ToolPaneGroup toolPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(toolPaneGroup != null);

            XmlElement xmlToolPaneGroup = xmlDocument.CreateElement("ToolPaneGroup");

            AddGuidAttribute(xmlDocument, xmlToolPaneGroup, toolPaneGroup);
            AddWidthAndHeightAttributes(xmlDocument, xmlToolPaneGroup, toolPaneGroup);

            xmlParentNode.AppendChild(xmlToolPaneGroup);

            int count = toolPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Tool pane group has no tools");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = toolPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                AddToolNode(xmlDocument, xmlToolPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlToolPaneGroup;
        }

        private static XmlElement AddToolNode(XmlDocument xmlDocument, XmlNode xmlParentNode, IViewModel iViewModel, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(iViewModel != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Tool");
            AddAttribute(xmlDocument, xmlElement, "Title", iViewModel.Title);
            AddAttribute(xmlDocument, xmlElement, "ContentId", contentId);

            xmlParentNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private static void AddSplitterPaneNode(XmlDocument xmlDocument, Grid splitterPane, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(splitterPane != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlSplitterPane = xmlDocument.CreateElement("SplitterPane");
            xmlParentNode.AppendChild(xmlSplitterPane);

            GridSplitter gridSplitter = splitterPane.Children.OfType<GridSplitter>().Single();
            System.Diagnostics.Trace.Assert(gridSplitter != null);

            AddGuidAttribute(xmlDocument, xmlSplitterPane, splitterPane);
            AddAttribute(xmlDocument, xmlSplitterPane, "Orientation", (Grid.GetRow(gridSplitter) == 1) ? "Horizontal" : "Vertical");
            AddWidthAndHeightAttributes(xmlDocument, xmlSplitterPane, splitterPane);

            List<FrameworkElement> children = splitterPane.Children.OfType<FrameworkElement>().Where(n => !(n is GridSplitter)).OrderBy(n => Grid.GetRow(n) + Grid.GetColumn(n)).ToList();
            foreach (var childNode in children)
            {
                AddNode(xmlDocument, childNode, xmlSplitterPane);
            }
        }

        private static XmlElement AddDocumentPaneGroupNode(XmlDocument xmlDocument, XmlNode xmlParentNode, DocumentPaneGroup documentPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(documentPaneGroup != null);

            XmlElement xmlDocumentPaneGroup = xmlDocument.CreateElement("DocumentPaneGroup");
            AddGuidAttribute(xmlDocument, xmlDocumentPaneGroup, documentPaneGroup);
            AddWidthAndHeightAttributes(xmlDocument, xmlDocumentPaneGroup, documentPaneGroup);

            xmlParentNode.AppendChild(xmlDocumentPaneGroup);

            int count = documentPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Document pane group has no documents");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = documentPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                AddDocumentNode(xmlDocument, xmlDocumentPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlDocumentPaneGroup;
        }

        private static XmlElement AddDocumentNode(XmlDocument xmlDocument, XmlNode xmlParentNode, IViewModel iViewModel, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(iViewModel != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Document");
            AddAttribute(xmlDocument, xmlElement, "Title", iViewModel.Title);
            AddAttribute(xmlDocument, xmlElement, "ContentId", contentId);
            AddAttribute(xmlDocument, xmlElement, "Url", iViewModel.URL);

            xmlParentNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private static void AddDocumentPanelNode(XmlDocument xmlDocument, DocumentPanel documentPanel, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(documentPanel != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlDocumentPanel = xmlDocument.CreateElement("DocumentPanel");
            AddGuidAttribute(xmlDocument, xmlDocumentPanel, documentPanel);
            AddWidthAndHeightAttributes(xmlDocument, xmlDocumentPanel, documentPanel);
            xmlParentNode.AppendChild(xmlDocumentPanel);

            foreach (var childNode in documentPanel.Children)
            {
                AddNode(xmlDocument, childNode, xmlDocumentPanel);
            }
        }

        private static void AddPositionAndSizeAttributes(XmlDocument xmlDocument, XmlElement xmlElement, FloatingPane floatingPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlElement != null);
            System.Diagnostics.Trace.Assert(floatingPane != null);

            AddAttribute(xmlDocument, xmlElement, "Left", floatingPane.Left.ToString());
            AddAttribute(xmlDocument, xmlElement, "Top", floatingPane.Top.ToString());
            AddAttribute(xmlDocument, xmlElement, "Width", floatingPane.Width.ToString());
            AddAttribute(xmlDocument, xmlElement, "Height", floatingPane.Height.ToString());
        }

        private static XmlElement AddFloatingToolPaneGroupNode(XmlDocument xmlDocument, XmlNode xmlParentNode, FloatingToolPaneGroup floatingToolPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlFloatingToolPaneGroup = xmlDocument.CreateElement("FloatingToolPaneGroup");
            AddPositionAndSizeAttributes(xmlDocument, xmlFloatingToolPaneGroup, floatingToolPaneGroup);
            AddGuidAttribute(xmlDocument, xmlFloatingToolPaneGroup, floatingToolPaneGroup);

            xmlParentNode.AppendChild(xmlFloatingToolPaneGroup);

            int count = floatingToolPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Floating tool pane has no tools");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = floatingToolPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                AddToolNode(xmlDocument, xmlFloatingToolPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlFloatingToolPaneGroup;
        }

        private static XmlElement AddFloatingDocumentPaneGroupNode(XmlDocument xmlDocument, XmlNode xmlParentNode, FloatingDocumentPaneGroup floatingDocumentPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlFloatingDocumentPaneGroup = xmlDocument.CreateElement("FloatingDocumentPaneGroup");
            AddPositionAndSizeAttributes(xmlDocument, xmlFloatingDocumentPaneGroup, floatingDocumentPaneGroup);
            AddGuidAttribute(xmlDocument, xmlFloatingDocumentPaneGroup, floatingDocumentPaneGroup);

            xmlParentNode.AppendChild(xmlFloatingDocumentPaneGroup);

            int count = floatingDocumentPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Floating document pane has no tools");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = floatingDocumentPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                AddDocumentNode(xmlDocument, xmlFloatingDocumentPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlFloatingDocumentPaneGroup;
        }

        private static void AddNode(XmlDocument xmlDocument, Object node, XmlNode xmlParentPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(node != null);
            System.Diagnostics.Trace.Assert(xmlParentPane != null);

            if (node is DocumentPanel)
            {
                AddDocumentPanelNode(xmlDocument, node as DocumentPanel, xmlParentPane);
            }
            else if (node is DocumentPaneGroup)
            {
                AddDocumentPaneGroupNode(xmlDocument, xmlParentPane, node as DocumentPaneGroup);
            }
            else if (node is ToolPaneGroup)
            {
                AddToolPaneGroupNode(xmlDocument, xmlParentPane, node as ToolPaneGroup);
            }
            else if (node is Grid)
            {
                AddSplitterPaneNode(xmlDocument, node as Grid, xmlParentPane);
            }
        }

        private static XmlElement AddUnpinnedToolDataNode(XmlDocument xmlDocument, XmlNode xmlParentNode, UnpinnedToolData unpinnedToolData)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(unpinnedToolData != null);

            XmlElement xmlUnpinnedToolData = xmlDocument.CreateElement("UnpinnedToolData");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Sibling");
            xmlAttribute.Value = unpinnedToolData.SiblingGuid.ToString();
            xmlUnpinnedToolData.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("IsHorizontal");
            xmlAttribute.Value = unpinnedToolData.IsHorizontal.ToString();
            xmlUnpinnedToolData.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("IsFirst");
            xmlAttribute.Value = unpinnedToolData.IsFirst.ToString();
            xmlUnpinnedToolData.Attributes.Append(xmlAttribute);

            AddToolPaneGroupNode(xmlDocument, xmlUnpinnedToolData, unpinnedToolData.ToolPaneGroup);

            xmlParentNode.AppendChild(xmlUnpinnedToolData);
            return xmlUnpinnedToolData;
        }

        public static void SaveLayout(
            XmlDocument xmlDocument, 
            Grid rootGrid, 
            List<IFloatingPane> floatingToolPaneGroups, 
            List<IFloatingPane> floatingDocumentPaneGroups,
            Dictionary<WindowLocation, List<UnpinnedToolData>> dictUnpinnedToolData)
        {
            XmlElement xmlLayoutManager = xmlDocument.CreateElement("LayoutManager");
            xmlDocument.AppendChild(xmlLayoutManager);

            AddNode(xmlDocument, rootGrid, xmlLayoutManager);

            foreach (FloatingToolPaneGroup floatingTool in floatingToolPaneGroups)
            {
                AddFloatingToolPaneGroupNode(xmlDocument, xmlLayoutManager, floatingTool);
            }

            foreach (FloatingDocumentPaneGroup floatingDocumentPaneGroup in floatingDocumentPaneGroups)
            {
                AddFloatingDocumentPaneGroupNode(xmlDocument, xmlLayoutManager, floatingDocumentPaneGroup);
            }

            foreach (var keyValuePair in dictUnpinnedToolData)
            {
                string name = keyValuePair.Key.ToString();
                switch (keyValuePair.Key)
                {
                    case WindowLocation.LeftSide:
                        name = "LeftSide";
                        break;
                    case WindowLocation.TopSide:
                        name = "TopSide";
                        break;
                    case WindowLocation.RightSide:
                        name = "RightSide";
                        break;
                    case WindowLocation.BottomSide:
                        name = "BottomSide";
                        break;
                    default:
                        System.Diagnostics.Trace.Assert(false, "Invalid WindowLocation");
                        break;
                }

                XmlElement xmlElement = xmlDocument.CreateElement(name);
                xmlLayoutManager.AppendChild(xmlElement);

                foreach (var item in keyValuePair.Value)
                {
                    AddUnpinnedToolDataNode(xmlDocument, xmlElement, item);
                }
            }
        }
    }
}
