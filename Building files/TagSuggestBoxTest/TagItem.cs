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
    public sealed class TagItem : Control
    {
        public TagItem()
        {
            this.DefaultStyleKey = typeof(TagItem);
        }

        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(TagItem),
                new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public event EventHandler<RoutedEventArgs> DeleteButtonClicked;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AppBarButton deleteButton = GetTemplateChild("DeleteButton") as AppBarButton;
            if (deleteButton != null)
            {
                deleteButton.Click += (s, e) => DeleteButtonClicked?.Invoke(s, e);
            }
        }
    }
}
