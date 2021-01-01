using System.Collections.Generic;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System;

namespace OpenControls.Wpf.DockManager.Serialisation
{
    internal class LayoutReader
    {
        private static string GetStringAttribute(XmlElement xmlElement, string name)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem(name) as XmlAttribute;
            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a " + name + " attribute");
            return xmlAttribute.Value;
        }

        private static double GetDoubleAttribute(XmlElement xmlElement, string name)
        {
            return System.Convert.ToDouble(GetStringAttribute(xmlElement, name));
        }

        private static bool GetBooleanAttribute(XmlElement xmlElement, string name)
        {
            string value = GetStringAttribute(xmlElement, name);
            bool.TryParse(value, out bool isHorizontal);
            return isHorizontal;
        }

        private static void SetWidthOrHeight(XmlElement xmlElement, FrameworkElement parentFrameworkElement, bool isParentHorizontal, int row, int column)
        {
            Grid grid = parentFrameworkElement as Grid;
            if (grid != null)
            {
                if (isParentHorizontal)
                {
                    if (row < grid.RowDefinitions.Count)
                    {
                        double height = GetDoubleAttribute(xmlElement, "Height");
                        grid.RowDefinitions[row].Height = new GridLength(height, GridUnitType.Star);
                    }
                }
                else
                {
                    if (column < grid.ColumnDefinitions.Count)
                    {
                        double width = GetDoubleAttribute(xmlElement, "Width");
                        grid.ColumnDefinitions[column].Width = new GridLength(width, GridUnitType.Star);
                    }
                }
            }
        }

        private static void SetLocationAndSize(XmlElement xmlElement, Window window)
        {
            window.Left = GetDoubleAttribute(xmlElement, "Left");
            window.Top = GetDoubleAttribute(xmlElement, "Top");
            window.Width = GetDoubleAttribute(xmlElement, "Width");
            window.Height = GetDoubleAttribute(xmlElement, "Height");
        }

        private static void LoadTools(Dictionary<string, UserControl> viewsMap, XmlElement xmlToolPaneGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlChild in xmlToolPaneGroup.ChildNodes)
            {
                if (xmlChild is XmlElement)
                {
                    if ((xmlChild as XmlElement).Name == "Tool")
                    {
                        XmlElement xmlToolElement = xmlChild as XmlElement;

                        string ContentId = GetStringAttribute(xmlToolElement, "ContentId");

                        if (viewsMap.ContainsKey(ContentId))
                        {
                            iViewContainer.AddUserControl(viewsMap[ContentId]);
                            viewsMap.Remove(ContentId);
                        }
                    }
                }
            }
        }

        private static void LoadDocuments(ILayoutFactory iLayoutFactory, Dictionary<string, UserControl> viewsMap, XmlElement xmlDocumentPaneGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlChild in xmlDocumentPaneGroup.ChildNodes)
            {
                if (xmlChild is XmlElement)
                {
                    if ((xmlChild as XmlElement).Name == "Document")
                    {
                        XmlElement xmlToolElement = xmlChild as XmlElement;

                        string contentId = GetStringAttribute(xmlToolElement, "ContentId");
                        string url = GetStringAttribute(xmlToolElement, "Url");
                        string key = iLayoutFactory.MakeDocumentKey(contentId, url);

                        if (viewsMap.ContainsKey(key))
                        {
                            iViewContainer.AddUserControl(viewsMap[key]);
                            viewsMap.Remove(contentId);
                        }
                    }
                }
            }
        }

        private static Guid GetGuid(XmlElement xmlElement)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("Guid") as XmlAttribute;
            return new Guid(xmlAttribute.Value);
        }

        private static void LoadUnPinnedToolDataNodes(ILayoutFactory iLayoutFactory, Dictionary<string, UserControl> viewsMap, WindowLocation windowLocation, XmlElement xmlParentElement)
        {
            foreach (var xmlChildNode in xmlParentElement.ChildNodes)
            {
                if (xmlChildNode is XmlElement)
                {
                    if ((xmlChildNode as XmlElement).Name == "UnpinnedToolData")
                    {
                        XmlElement xmlUnpinnedToolData = xmlChildNode as XmlElement;

                        string guid = GetStringAttribute(xmlUnpinnedToolData, "Sibling");
                        bool isHorizontal = GetBooleanAttribute(xmlUnpinnedToolData, "IsHorizontal");
                        bool isFirst = GetBooleanAttribute(xmlUnpinnedToolData, "IsFirst");

                        foreach (var xmlUnpinnedToolDataChildNode in xmlUnpinnedToolData.ChildNodes)
                        {
                            if (xmlUnpinnedToolDataChildNode is XmlElement)
                            {
                                if ((xmlUnpinnedToolDataChildNode as XmlElement).Name == "ToolPaneGroup")
                                {
                                    ToolPaneGroup toolPaneGroup = iLayoutFactory.MakeToolPaneGroup();
                                    XmlElement xmlToolPaneGroup = xmlUnpinnedToolDataChildNode as XmlElement;
                                    LoadTools(viewsMap, xmlToolPaneGroup, toolPaneGroup.IViewContainer);
                                    iLayoutFactory.MakeUnpinnedToolPaneGroup(windowLocation, toolPaneGroup, guid, isHorizontal, isFirst);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void LoadNode(ILayoutFactory iLayoutFactory, Dictionary<string, UserControl> viewsMap, FrameworkElement rootFrameworkElement, FrameworkElement parentFrameworkElement, XmlNode xmlParentElement, bool isParentHorizontal)
        {
            int row = 0;
            int rowIncrement = isParentHorizontal ? 2 : 0;
            int column = 0;
            int columnIncrement = isParentHorizontal ? 0 : 2;

            foreach (var xmlChildNode in xmlParentElement.ChildNodes)
            {
                if (xmlChildNode is XmlElement)
                {
                    if ((xmlChildNode as XmlElement).Name == "SplitterPane")
                    {
                        XmlElement xmlSplitterPane = xmlChildNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlSplitterPane.Attributes.GetNamedItem("Orientation") as XmlAttribute;

                        System.Diagnostics.Trace.Assert(xmlAttribute != null, "SplitterPane element does not have an orientation attribute");

                        bool isChildHorizontal = xmlAttribute.Value == "Horizontal";

                        SplitterPane newGrid = iLayoutFactory.MakeSplitterPane(isChildHorizontal);
                        newGrid.Tag = GetGuid(xmlSplitterPane);

                        if (parentFrameworkElement == rootFrameworkElement)
                        {
                            iLayoutFactory.SetRootPane(newGrid, out row, out column);
                        }
                        else
                        {
                            System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                            parentElement.AddChild(newGrid);
                            Grid.SetRow(newGrid, row);
                            Grid.SetColumn(newGrid, column);
                        }
                        SetWidthOrHeight(xmlSplitterPane, parentFrameworkElement, isParentHorizontal, row, column);

                        row += rowIncrement;
                        column += columnIncrement;

                        LoadNode(iLayoutFactory, viewsMap, rootFrameworkElement, newGrid, xmlSplitterPane, isChildHorizontal);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "DocumentPanel")
                    {
                        DocumentPanel documentPanel = iLayoutFactory.MakeDocumentPanel();

                        if (parentFrameworkElement == rootFrameworkElement)
                        {
                            iLayoutFactory.SetRootPane(documentPanel, out row, out column);
                        }
                        else
                        {
                            System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                            parentElement.AddChild(documentPanel);
                            Grid.SetRow(documentPanel, row);
                            Grid.SetColumn(documentPanel, column);
                        }

                        XmlElement xmlDocumentPanel = xmlChildNode as XmlElement;
                        SetWidthOrHeight(xmlDocumentPanel, parentFrameworkElement, isParentHorizontal, row, column);

                        row += rowIncrement;
                        column += columnIncrement;

                        LoadNode(iLayoutFactory, viewsMap, rootFrameworkElement, documentPanel, xmlDocumentPanel, true);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "DocumentPaneGroup")
                    {
                        DocumentPaneGroup documentPaneGroup = iLayoutFactory.MakeDocumentPaneGroup();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(documentPaneGroup);

                        XmlElement xmlDocumentGroup = xmlChildNode as XmlElement;

                        documentPaneGroup.Tag = GetGuid(xmlDocumentGroup); ;
                        SetWidthOrHeight(xmlDocumentGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadDocuments(iLayoutFactory, viewsMap, xmlDocumentGroup, documentPaneGroup.IViewContainer);
                        Grid.SetRow(documentPaneGroup, row);
                        Grid.SetColumn(documentPaneGroup, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "ToolPaneGroup")
                    {
                        ToolPaneGroup toolPaneGroup = iLayoutFactory.MakeToolPaneGroup();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(toolPaneGroup);

                        XmlElement xmlToolPaneGroup = xmlChildNode as XmlElement;

                        toolPaneGroup.Tag = GetGuid(xmlToolPaneGroup);
                        SetWidthOrHeight(xmlToolPaneGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadTools(viewsMap, xmlToolPaneGroup, toolPaneGroup.IViewContainer);
                        Grid.SetRow(toolPaneGroup, row);
                        Grid.SetColumn(toolPaneGroup, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "FloatingToolPaneGroup")
                    {
                        FloatingToolPaneGroup floatingToolPaneGroup = iLayoutFactory.MakeFloatingToolPaneGroup();
                        XmlElement xmlfloatingTool = xmlChildNode as XmlElement;
                        floatingToolPaneGroup.Tag = GetGuid(xmlfloatingTool);
                        SetLocationAndSize(xmlfloatingTool, floatingToolPaneGroup);
                        LoadTools(viewsMap, xmlfloatingTool, floatingToolPaneGroup.IViewContainer);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "FloatingDocumentPaneGroup")
                    {
                        FloatingDocumentPaneGroup floatingDocumentPaneGroup = iLayoutFactory.MakeFloatingDocumentPaneGroup();
                        XmlElement xmlfloatingDocument = xmlChildNode as XmlElement;
                        floatingDocumentPaneGroup.Tag = GetGuid(xmlfloatingDocument);
                        SetLocationAndSize(xmlfloatingDocument, floatingDocumentPaneGroup);
                        LoadDocuments(iLayoutFactory, viewsMap, xmlfloatingDocument, floatingDocumentPaneGroup.IViewContainer);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "LeftSide")
                    {
                        XmlElement xmlLeftSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolDataNodes(iLayoutFactory, viewsMap, WindowLocation.LeftSide, xmlLeftSide);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "TopSide")
                    {
                        XmlElement xmlTopSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolDataNodes(iLayoutFactory, viewsMap, WindowLocation.TopSide, xmlTopSide);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "RightSide")
                    {
                        XmlElement xmlRightSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolDataNodes(iLayoutFactory, viewsMap, WindowLocation.RightSide, xmlRightSide);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "BottomSide")
                    {
                        XmlElement xmlBottomSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolDataNodes(iLayoutFactory, viewsMap, WindowLocation.BottomSide, xmlBottomSide);
                    }
                }

                if (parentFrameworkElement != rootFrameworkElement)
                {
                    if ((row > 2) || (column > 2))
                    {
                        // we can only have two child elements (plus a splitter) in each grid
                        break;
                    }
                }
            }
        }
    }
}

