using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace TagSuggestBoxTest
{
    public sealed class TagSuggestBox : Control
    {
        public class SuggestItemTemplate
        {
            public string ID {  get; set; }
            public string TagName {  get; set; }
        }

        public TagSuggestBox()
        {
            this.DefaultStyleKey = typeof(TagSuggestBox);
        }


        public static readonly DependencyProperty HeaderProperty 
            = DependencyProperty.Register(
                nameof(Header),
                typeof(string),
                typeof(TagSuggestBox),
                new PropertyMetadata(null));

        public string Header
        {
            get=>(string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderFontSizeProperty
            = DependencyProperty.Register(
                nameof(HeaderFontSize),
                typeof(float),
                typeof(TagSuggestBox),
                new PropertyMetadata(null));

        public string HeaderFontSize
        {
            get =>(string)GetValue(HeaderFontSizeProperty);
            set => SetValue(HeaderFontSizeProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(SuggestItemTemplate),
                typeof(TagSuggestBox),
                new PropertyMetadata(null));

        public List<SuggestItemTemplate> ItemsSource
        {
            get => (List<SuggestItemTemplate>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }


        public event EventHandler<AutoSuggestBoxTextChangedEventArgs> TextChanged;
        public event EventHandler<AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AutoSuggestBox inputBox = GetTemplateChild("InputBox") as AutoSuggestBox;
            if (inputBox != null)
            {
                inputBox.TextChanged += (s, e) => TextChanged?.Invoke(s, e);
                inputBox.QuerySubmitted += (s, e) => QuerySubmitted?.Invoke(s, e);
            }
        }


    }
}
