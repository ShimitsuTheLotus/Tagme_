using CommunityToolkit.WinUI.Controls;
using Microsoft.Data.Sqlite;
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

            InitializeTagSuggestionList();
        }

        private void AddItemPage_Loaded(object sender, RoutedEventArgs e)
        {
            pageLoaded = true;
        }

        public static class NewItemProperty
        {
            public static bool IsFolder = false;
            /// <summary>
            /// The tags will be separated with 3 spaces, and the proporty is separated with 2 spaces.
            /// But actually, the tags will be stored as their unique ID
            /// </summary>
            public static string TagIDs;
            public static string Description;
            public static string CreatedTime;
            public static string ModifiedTime;
            /// <summary>
            /// If there's file added, the value will be set to a unique one, or the value will be an empty string.
            /// </summary>
            public static string ItemID = "";
            /// <summary>
            /// Only when IsFolder == true, we can add file content. Else, it can only add path or nothing
            /// True: Add file content; False: Add File Path; Null: Add nothing
            /// </summary>
            public static bool? IsFileContent = false;
            public static string FilePath = "";
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


        //Status
        bool pageLoaded { get; set; } = false;


        //Variables
        public ObservableCollection<string> Tags { get; set; } = new ObservableCollection<string>();
        public List<Tagme_CustomizedCore.Template.TagInputSuggestionTemplate> TagSuggestionList { get; set; } = new List<Tagme_CustomizedCore.Template.TagInputSuggestionTemplate>();
        public string TagTextBox_OldContent = string.Empty;

        public class TagListTemplateItem
        {
            public string Tag { get; set; }
        }

        public static class Data
        {
            public static List<TagListTemplateItem> TagList = new List<TagListTemplateItem>();
        }
        

        //Functions
        private void InitializeTagSuggestionList()
        {
            ///For test
            TagSuggestionList.Add(new Tagme_CustomizedCore.Template.TagInputSuggestionTemplate() {TagName="XAML",ID="2"});
            TagSuggestionList.Add(new Tagme_CustomizedCore.Template.TagInputSuggestionTemplate() { TagName = "C#", ID = "1" });
            TagSuggestionList.Add(new Tagme_CustomizedCore.Template.TagInputSuggestionTemplate() { TagName = "C#/XAML", ID = "3" });
        }


        //Events

        private void TagTextBox_TokenItemAdding(CommunityToolkit.WinUI.Controls.TokenizingTextBox sender, CommunityToolkit.WinUI.Controls.TokenItemAddingEventArgs args)
        {
            //Avoid Tag repeating
            if (!(args.Item == null && !Tags.Contains(args.TokenText)))
            {
                args.Cancel = true;
            }
        }

        private void SetTypeFileButton_Click(object sender, RoutedEventArgs e)
        {
            SetTypeFileButton.BorderThickness = new Thickness(4, 4, 4, 4);
            SetTypeFolderButton.BorderThickness = new Thickness(0, 0, 0, 0);
            NewItemProperty.IsFolder = false;
        }

        private void SetTypeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SetTypeFileButton.BorderThickness = new Thickness(0, 0, 0, 0);
            SetTypeFolderButton.BorderThickness = new Thickness(4, 4, 4, 4);
            NewItemProperty.IsFolder = true;
        }

        private void SetContentPathButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentPathButton.BorderThickness = new Thickness(4, 4, 4, 4);
            SetContentFileButton.BorderThickness = new Thickness(0, 0, 0, 0);
            SetContentNullButton.BorderThickness = new Thickness(0, 0, 0, 0);
            NewItemProperty.IsFileContent = false;
            SelectedFilePathTextBlock.Opacity = 1;
        }

        private void SetContentFileButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentPathButton.BorderThickness = new Thickness(0, 0, 0, 0);
            SetContentFileButton.BorderThickness = new Thickness(4, 4, 4, 4);
            SetContentNullButton.BorderThickness = new Thickness(0, 0, 0, 0);
            NewItemProperty.IsFileContent = true;
            SelectedFilePathTextBlock.Opacity = 1;
        }

        private void SetContentNullButton_Click(object sender, RoutedEventArgs e)
        {
            SetContentPathButton.BorderThickness = new Thickness(0, 0, 0, 0);
            SetContentFileButton.BorderThickness = new Thickness(0, 0, 0, 0);
            SetContentNullButton.BorderThickness = new Thickness(4, 4, 4, 4);
            NewItemProperty.IsFileContent = null;
            SelectedFilePathTextBlock.Opacity = 0.7;
        }

        private async void ChangeSelectedFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                NewItemProperty.FilePath = file.Path;
                SelectedFilePathTextBlock.Text = NewItemProperty.FilePath;
            }
        }

        private void ClearSelectedFileButton_Click(object sender, RoutedEventArgs e)
        {
            NewItemProperty.FilePath = "";
            SelectedFilePathTextBlock.Text = "[" + "Not Selected" + "]";
        }

        private void CalcelCreateItemButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void CreateItemButton_Click(object sender, RoutedEventArgs e)
        {
            //Create Item
            string dbpath = Tagme_CoreUWP.CoreRunningData.Tagme_DataBase.UsingDataBasePath;
            if (File.Exists(dbpath))
            {
                using (SqliteConnection db = new SqliteConnection($"Filename{dbpath}"))
                {
                    db.Open();

                    //Check tag mapping, add unlogged tags
                    SqliteCommand sqliteCommand = new SqliteCommand($"INSERT INTO");

                    //Add Item root

                    //Add item source if a storage item is added, and folder won't have this property

                    //Add a proporty root even it's empty

                    db.Close();
                }
            }
        }

        private void TagTextBox_SuggestionChosen(RichSuggestBox sender, SuggestionChosenEventArgs args)
        {
            if (args.Prefix == "#")
            {
                args.DisplayText = ((Tagme_CustomizedCore.Template.TagInputSuggestionTemplate)args.SelectedItem).TagName;
            }
        }

        private void TagTextBox_SuggestionRequested(RichSuggestBox sender, SuggestionRequestedEventArgs args)
        {
            //Color the tags to let users know whether the tag has been used
            List<Tagme_CustomizedCore.Template.TagInputSuggestionTemplate> coloredSuggestList = TagSuggestionList.Select(
                x => TagTextBox.Tokens.Any(
                    y => ((Tagme_CustomizedCore.Template.TagInputSuggestionTemplate)y.Item).ID == x.ID)
                ? new Tagme_CustomizedCore.Template.TagInputSuggestionTemplate()
                {
                    ID = x.ID,
                    TagName = x.TagName,
                    SignColor = Application.Current.RequestedTheme
                    == ApplicationTheme.Light
                    ? new SolidColorBrush(Windows.UI.Color.FromArgb(0x70, 0x00, 0x00, 0x00))
                    : new SolidColorBrush(Windows.UI.Color.FromArgb(0x90, 0xFF, 0xFF, 0xFF))
                }
                : new Tagme_CustomizedCore.Template.TagInputSuggestionTemplate()
                {
                    ID = x.ID,
                    TagName = x.TagName,
                    SignColor = Application.Current.RequestedTheme
                    == ApplicationTheme.Light
                    ? new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x00, 0x3E, 0x92))
                    : new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x99, 0xEB, 0xFF))
                }
                ).ToList();
            sender.ItemsSource = coloredSuggestList.Where(x => x.TagName.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase));
        }

        private void TagTextBox_TextChanged(RichSuggestBox sender, RoutedEventArgs args)
        {
            //Suggest contents
            string tagTemp = TagTextBox.TextDocument.ToString();
            List<string> tagsList = new List<string>();

            //Ignore spaces from the beginning and end of the string
            //Ignore spaces at the beginning
            if (tagTemp != string.Empty)
            {
                int i = 0;
                for (i = 0; i < tagTemp.Length; i++)
                {
                    if (tagTemp[i] == ' ')
                    {
                        if (i < tagTemp.Length - 1)
                        {
                            i++;
                        }
                        else { break; }
                    }
                    else { break; }
                }
                tagTemp = tagTemp.Substring(i, tagTemp.Length - i);
            }
            //Ignore spaces at the end
            if (tagTemp != string.Empty)
            {
                int i = tagTemp.Length - 1;
                while (i >= 0)
                {
                    if (tagTemp[i] == ' ')
                    {
                        i--;
                    }
                    else { break; }
                }
                tagTemp = tagTemp.Substring(0, i + 1);
            }

            //Get the ending index of the modified tag
            for (int i = 0; i < Math.Min(TagTextBox_OldContent.Length, tagTemp.Length); i++)
            {
                if (TagTextBox_OldContent[i] != tagTemp[i])
                {
                    tagTemp = tagTemp.Substring(0, i + 1);
                    break;
                }
            }
            TagTextBox_OldContent = tagTemp;

            //Get the tag just modified

        }

        private void TagSuggestBox_TextEdited(object sender, EventArgs e)
        {
            int i = 0;
            for (int i1 = 0;i1<2;i1++)
            {
                i += 1;
            }
        }
    }
}
