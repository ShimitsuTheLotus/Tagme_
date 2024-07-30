﻿using CommunityToolkit.WinUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tagme_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddItemPage : Page
    {
        public AddItemPage()
        {
            this.InitializeComponent();

            SizeChanged += AddItemPage_SizeChanged;
            Loaded += AddItemPage_Loaded;
        }

        private void AddItemPage_Loaded(object sender, RoutedEventArgs e)
        {
            pageLoaded = true;
        }

        private void AddItemPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (pageLoaded)
            {
                /*DependencyObject startNode = TagTextBox.ItemsPanelRoot;
                int count = VisualTreeHelper.GetChildrenCount(startNode);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(startNode, i);
                    TokenizingTextBoxItem item = TagTextBox.ContainerFromItem(child) as TokenizingTextBoxItem;
                    if (item != null)
                    {
                        item.MaxWidth = TagTextBox.ActualWidth;
                    }
                }
                foreach (TokenizingTextBoxItem item in TagTextBox.ItemsPanelRoot.Children)
                {
                    item.Width = TagTextBox.ActualWidth;
                    DependencyObject startNode = item;
                    int count = VisualTreeHelper.GetChildrenCount(startNode);
                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(startNode, i);
                        TokenizingTextBoxItem item2 = TagTextBox.ContainerFromItem(child) as TokenizingTextBoxItem;
                        if (child is Grid)
                        {
                            Grid grid = child as Grid;
                            grid.MaxWidth = TagTextBox.ActualWidth;
                        }
                        if (child is TextBlock)
                        {
                            TextBlock textBlock = child as TextBlock;
                            textBlock.MaxWidth = TagTextBox.ActualWidth;
                        }
                        if (item2 != null)
                        {
                            item2.MaxWidth = TagTextBox.ActualWidth;
                        }
                    }

                }*/
            }
        }

        //status
        bool pageLoaded { get; set; } = false;

        //Variables
        public ObservableCollection<string> Tags { get; set; } = new ObservableCollection<string>();
        

        public class TagListTemplateItem
        {
            public string Tag { get; set; }
        }

        public static class Data
        {
            public static List<TagListTemplateItem> TagList = new List<TagListTemplateItem>();
        }
        

        private void TagTextBox_TokenItemAdding(CommunityToolkit.WinUI.Controls.TokenizingTextBox sender, CommunityToolkit.WinUI.Controls.TokenItemAddingEventArgs args)
        {
            //Avoid Tag repeating
            if (!(args.Item == null && !Tags.Contains(args.TokenText)))
            {
                args.Cancel = true;
            }
        }

        private void TagTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }
    }
}
