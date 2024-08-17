using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class AutoWrapPanel : Control
    {
        private Canvas _canvas;
        private ObservableCollection<UIElement> _children;
        /// <summary>
        /// row index, column index, element
        /// </summary>
        private List<(ulong,ulong,UIElement)> _childrenList;
        /// <summary>
        /// List index as row index, height
        /// </summary>
        private List<double> _maxRowHeightList;
        /// <summary>
        /// Summary of control width
        /// </summary>
        private List<double> _rowControlWidthList;

        private float _horizontalItemSpacing = 4;
        private float _verticalItemSpacing = 4;

        public enum _childrenVerticalAlignment
        {
            Top,
            Bottom,
            Center
        }

        public AutoWrapPanel()
        {
            this.DefaultStyleKey = typeof(AutoWrapPanel);

            _children = new ObservableCollection<UIElement>();
            _children.CollectionChanged += OnChildrenChanged;
            _canvas = GetTemplateChild("MainContainer") as Canvas;
            _children.CollectionChanged += OnChildrenChanged;
            foreach (var child in _children)
            {
                _canvas.Children.Add(child);
                OnChildAdded(child);
            }
        }

        private void OnChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(UIElement newChild in e.NewItems)
                {
                    _canvas.Children.Add(newChild);
                    OnChildAdded(newChild);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (UIElement Child in e.OldItems)
                {
                    _canvas.Children.Remove(Child);
                    OnChildRemoved(Child);
                }
            }
            else if (e.Action== System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                int i = 0;
                foreach (UIElement child in e.OldItems)
                {
                    _canvas.Children.Remove(child);
                    _canvas.Children.Add(e.NewItems[i] as UIElement);
                    OnChildReplaced(child);
                    i++;
                }
            }
        }


        public static readonly DependencyProperty WidthProperty
            = DependencyProperty.Register(
                nameof(Width),
                typeof(double),
                typeof(AutoWrapPanel),
                new PropertyMetadata(null));

        public double Width
        {
            set => SetValue(WidthProperty, value);
        }

        public static readonly DependencyProperty HeightProperty
            = DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(AutoWrapPanel),
                new PropertyMetadata(null));

        public double Height
        {
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty ActualWidthProperty
            = DependencyProperty.Register(
                nameof(ActualWidth),
                typeof(double),
                typeof(AutoWrapPanel),
                new PropertyMetadata(null));
        public double ActualWidth
        {
            get => (double)GetValue(ActualWidthProperty);
            private set => SetValue(ActualWidthProperty, value);
        }

        public static readonly DependencyProperty ActualHeightProperty
            = DependencyProperty.Register(
                nameof(ActualHeight),
                typeof(double),
                typeof(AutoWrapPanel),
                new PropertyMetadata(null));

        public double ActualHeight
        {
            get => (double)GetValue(ActualHeightProperty);
            private set => SetValue(ActualHeightProperty, value);
        }

        public static readonly DependencyProperty ChildrenVerticalAlignmentProperty
            = DependencyProperty.Register(
                nameof(ChildrenVerticalAlignment),
                typeof(_childrenVerticalAlignment),
                typeof(AutoWrapPanel),
                new PropertyMetadata(null));

        public _childrenVerticalAlignment ChildrenVerticalAlignment
        {
            get => (_childrenVerticalAlignment)GetValue(ChildrenVerticalAlignmentProperty);
            set => SetValue(ChildrenVerticalAlignmentProperty, value);
        }


        public event EventHandler<SizeChangedEventArgs> SizeChanged;
        public event EventHandler<SizeChangedEventArgs> ChildrenVerticalAlignmentChanged;


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_canvas != null)
            {
                _canvas.SizeChanged += (s, e) => SizeChanged?.Invoke(s, e);

                _canvas.SizeChanged -= MainContainer_SizeChanged;
                _canvas.SizeChanged += MainContainer_SizeChanged;
                this.ChildrenVerticalAlignmentChanged += AutoWrapPanel_ChildrenVerticalAlignmentChanged;
            }
        }

        private void AutoWrapPanel_ChildrenVerticalAlignmentChanged(object sender, SizeChangedEventArgs e)
        {
            //Reset Alignment
        }

        public ObservableCollection<UIElement> Children => _children;

        private void MainContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ActualWidth = e.NewSize.Width;
            ActualHeight = e.NewSize.Height;
        }

        protected virtual void OnChildAdded(UIElement newChild)
        {
            if (newChild != null)
            {
                //Expand the width of the panel if a single control desire width more than the panel.
                if (newChild.DesiredSize.Width + 2 * _horizontalItemSpacing >= _canvas.Width)
                {
                    //Expand width, 2* since there's space at the control's left and right.
                    _canvas.Width = newChild.DesiredSize.Width + 2 * _horizontalItemSpacing;
                    //Expand height
                    _canvas.Width += newChild.DesiredSize.Height + _verticalItemSpacing;

                    //Get row index
                    ulong? maxRowIndex = _maxRowHeightList.Count() > 1 ? (ulong?)_maxRowHeightList.Count() - 1 : null;


                    //Add item, the item will be added to a new row.
                    //Log item height and width
                    _maxRowHeightList.Add(newChild.DesiredSize.Height);
                    _rowControlWidthList.Add(newChild.DesiredSize.Width);
                    //Set left position
                    Canvas.SetLeft(newChild, _horizontalItemSpacing);
                    //Set top position, since the row have only 1 element, the ChildrenVerticalAlignment dosen't make any difference.
                    if (maxRowIndex == null)
                    {
                        Canvas.SetTop(newChild, _verticalItemSpacing);
                    }
                    else
                    {
                        Canvas.SetTop(newChild, _maxRowHeightList.Sum() + ((ulong)maxRowIndex + 1) * _verticalItemSpacing);
                    }
                    ////not used
                    //switch (ChildrenVerticalAlignment)
                    //{
                    //    case _childrenVerticalAlignment.Top:
                    //        {
                    //            if (maxRowIndex == null)
                    //            {
                    //                Canvas.SetTop(newChild, _verticalItemSpacing);
                    //            }
                    //            else
                    //            {
                    //                Canvas.SetTop(newChild, _maxRowHeightList.Sum() + ((ulong)maxRowIndex + 1) * _verticalItemSpacing);
                    //            }
                    //        }
                    //        break;
                    //    case _childrenVerticalAlignment.Bottom:
                    //        {
                    //            if (maxRowIndex == null)
                    //            {
                    //                Canvas.SetTop(newChild, _verticalItemSpacing);
                    //            }
                    //            else
                    //            {
                    //                Canvas.SetTop(newChild, _maxRowHeightList.Sum() + ((ulong)maxRowIndex + 1) * _verticalItemSpacing);
                    //            }
                    //        }
                    //        break;
                    //    case _childrenVerticalAlignment.Center:
                    //        {
                    //            if (maxRowIndex == null)
                    //            {
                    //                Canvas.SetTop(newChild, _verticalItemSpacing);
                    //            }
                    //            else
                    //            {
                    //                Canvas.SetTop(newChild, _maxRowHeightList.Sum() + ((ulong)maxRowIndex + 1) * _verticalItemSpacing);
                    //            }
                    //        }
                    //        break;
                    //}

                    //Add item
                    _childrenList.Add((maxRowIndex == null ? (ulong)0 : (ulong)maxRowIndex + 1, 0, newChild));
                }
                //The element is not more wide than the panel
                else
                {
                    ulong? maxRowIndex = _maxRowHeightList.Count() > 1 ? (ulong?)_maxRowHeightList.Count() - 1 : null;
                    foreach (var item in _childrenList)
                    {
                        maxRowIndex = maxRowIndex > item.Item1 ? item.Item1 : maxRowIndex;
                    }
                    double lastRowElementLength = (double)_childrenList
                        .Where(x => x.Item1 == maxRowIndex)
                        .Sum(y => (decimal)y.Item3.DesiredSize.Width);
                    if (lastRowElementLength + newChild.DesiredSize.Width + _horizontalItemSpacing > _canvas.Width)
                    {
                        _canvas.Height += _verticalItemSpacing + newChild.DesiredSize.Height;
                        _childrenList.Add((maxRowIndex + 1, 0, newChild));
                    }
                    else
                    {
                        ulong maxColumnIndex = 0;
                        foreach (var item in _childrenList)
                        {
                            maxColumnIndex = maxColumnIndex> item.Item2 ? item.Item2 : maxColumnIndex;
                        }
                        _childrenList.Add((maxRowIndex, maxColumnIndex + 1, newChild));
                    }
                }
            }
        }

        protected virtual void OnChildRemoved(UIElement child)
        {
            if (child != null)
            {
                
            }
        }

        protected virtual void OnChildReplaced(UIElement newChild)
        {

        }
    }
}
