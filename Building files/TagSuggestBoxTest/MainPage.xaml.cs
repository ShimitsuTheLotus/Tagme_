using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TagSuggestBoxTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private void TagSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<TagSuggestBox.SuggestItemTemplate> list = new List<TagSuggestBox.SuggestItemTemplate>();
                list.Add(new TagSuggestBox.SuggestItemTemplate() { ID = "1", TagName = "tag1" });
                SuggestBox.ItemsSource = list;
            }
        }

        private void SuggestBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HelperBlock.Text = WrapPanel.ActualWidth.ToString();
            new Grid().Children.Clear();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            AutoWrapPanel autoWrapPanel = WrapPanel;

            Button button = new Button
            {
                Height = new Random().Next(20, 50),
                Width = new Random().Next(50, 100),
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x91, 0xCA, 0xE8)),
            };

            autoWrapPanel.Children.Add(button);
        }
    }
}
