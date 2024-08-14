using CommunityToolkit.WinUI.Controls;
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

namespace Tagme_
{
    public class TagSuggestBox : Control
    {
        //Control name consts
        private const string N_TextBox = "TagTextBox";

        //Non-public structure
        private readonly List<string> _tags;

        //Controls and locks
        private TextBox _textBox;
        private readonly object _textLock;

        //Non-public properties
        private int _selectedIndex;

        public TagSuggestBox()
        {
            this.DefaultStyleKey = typeof(TagSuggestBox);


            _tags = new List<string>();

            //DefaultStyleKey = typeof(TextBlock);

            LostFocus += TagSuggestBox_LostFocus;
            Loaded += TagSuggestBox_Loaded;
        }

        private void TagSuggestBox_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void TagSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Hide suggestions
        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TagSuggestBox),
            new PropertyMetadata(null,OnTextEdited));


        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(TagSuggestBox),
            new PropertyMetadata(null));


        /// <summary>
        /// The text content in text edit area
        /// </summary>
        public string Text
        {
            get { return(string)GetValue(TextProperty); }
            set { SetValue(TextProperty,value); }
        }

        /// <summary>
        /// Header to show what the control is for
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public event EventHandler TextEdited;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextChangedEventHandler textChangedEventHandler = TagSuggestBox_TextChanged;
        }

        private void TagSuggestBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private static void OnTextEdited(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TagSuggestBox target = d as TagSuggestBox;
            string oldValue = (string)e.OldValue;
            string newValue = (string)e.NewValue;
            if(oldValue != newValue)
            {
                target.OnTextChanged(oldValue,newValue);
            }
        }

        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            //this.Text = "uwu";
        }

        /// <summary>
        /// Clear text in inputbox
        /// </summary>
        public void ClearText()
        {
            lock (_textLock)
            {
                _textBox.Text = string.Empty;
            }
        }
    }
}
