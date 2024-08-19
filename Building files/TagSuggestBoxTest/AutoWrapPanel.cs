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
        private object _canvasLock;
        private ObservableCollection<UIElement> _children;
        private _childrenVerticalAlignment _verticalAlignmentMode = _childrenVerticalAlignment.Top;
        /// <summary>
        /// row index, column index, element
        /// </summary>
        private List<(int,int,UIElement)> _childrenList = new List<(int, int, UIElement)>();
        /// <summary>
        /// List index as row index, height
        /// </summary>
        private List<double> _maxRowHeightList = new List<double>();
        /// <summary>
        /// Summary of control width
        /// </summary>
        private List<double> _rowControlWidthList = new List<double>();

        private float _horizontalItemSpacing = 10;
        private float _verticalItemSpacing = 10;

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
            _children.CollectionChanged -= OnChildrenChanged;
            _children.CollectionChanged += OnChildrenChanged;
            _canvas = GetTemplateChild("MainContainer") as Canvas;
            foreach (var child in _children)
            {
                lock (_canvasLock)
                {
                    _canvas.Children.Add(child);
                }
                lock (_canvasLock)
                {
                    OnChildAdded(child);
                }
            }
        }

        private void OnChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_canvas == null)
            {
                _canvas = GetTemplateChild("MainContainer") as Canvas;
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (UIElement newChild in e.NewItems)
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
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
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
                new PropertyMetadata(100));

        public double Width
        {
            set => SetValue(WidthProperty, value);
        }

        public static readonly DependencyProperty HeightProperty
            = DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(AutoWrapPanel),
                new PropertyMetadata(100));

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
                new PropertyMetadata(_childrenVerticalAlignment.Top));

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
                //Make DesiredSize measured
                newChild.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                _canvas.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                //Expand the width of the panel if a single control desire width more than the panel.
                if (newChild.DesiredSize.Width + 2 * _horizontalItemSpacing >= _canvas.ActualWidth)
                {
                    //Expand width, 2* since there's space at the control's left and right.
                    _canvas.Width = _canvas.ActualWidth + newChild.DesiredSize.Width + 2 * _horizontalItemSpacing;
                    //Expand height
                    _canvas.Height = _canvas.ActualHeight + newChild.DesiredSize.Height + _verticalItemSpacing;

                    //Get row index
                    int? maxRowIndex = _maxRowHeightList.Count() > 1 ? (int?)_maxRowHeightList.Count() - 1 : null;


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
                        Canvas.SetTop(newChild, _maxRowHeightList.Sum() + ((double)maxRowIndex + 1) * _verticalItemSpacing);
                    }

                    //Add item
                    _childrenList.Add((maxRowIndex == null || _childrenList.Count == 0 ? 0 : (int)maxRowIndex + 1, 0, newChild));
                }
                //The element is not more wide than the panel
                else
                {
                    //Get index
                    int? maxRowIndex = _maxRowHeightList.Count() > 0 ? (int?)_maxRowHeightList.Count() - 1 : null;
                    //Get width
                    double lastRowElementLength =
                        maxRowIndex == null
                        ? 0
                        : _rowControlWidthList[_rowControlWidthList.Count - 1] 
                        + Math.Max(_childrenList.Where(x => x.Item1 == maxRowIndex).Count() * _horizontalItemSpacing, 2 * _horizontalItemSpacing);
                    //Add in next row
                    if (lastRowElementLength + newChild.DesiredSize.Width + _horizontalItemSpacing > _canvas.ActualWidth)
                    {
                        _canvas.Height = _canvas.ActualHeight + _verticalItemSpacing + newChild.DesiredSize.Height;

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
                            Canvas.SetTop(newChild, _maxRowHeightList.Sum() - _maxRowHeightList[_maxRowHeightList.Count() - 1] + ((double)maxRowIndex + 2) * _verticalItemSpacing);
                        }

                        _childrenList.Add((maxRowIndex == null || _childrenList.Count == 0 ? 0 : (int)maxRowIndex + 1, 0, newChild));
                    }
                    //Add in this row
                    else
                    {
                        List<double> historyMaxRowHeight = _maxRowHeightList.Where(x=>true).ToList();
                        if (maxRowIndex == null)
                        {
                            _canvas.Height = _canvas.ActualHeight + _verticalItemSpacing + newChild.DesiredSize.Height;
                        }
                        else
                        {
                            //Expand if width is greater than maximum
                            if (newChild.DesiredSize.Height > (double)_maxRowHeightList[(int)maxRowIndex])
                            {
                                _canvas.Height = _canvas.ActualHeight + newChild.DesiredSize.Height - _maxRowHeightList[(int)maxRowIndex];
                                //Update height
                                _maxRowHeightList[(int)maxRowIndex] = newChild.DesiredSize.Height;
                            }
                        }


                        //Set left position
                        Canvas.SetLeft(newChild, maxRowIndex == null ? (double)_horizontalItemSpacing : (double)((_childrenList.Where(x => x.Item1 == maxRowIndex).Count() + 1) * _horizontalItemSpacing) + (double)_rowControlWidthList[(int)maxRowIndex]);
                        //Set top position, the position will be infected by the ChildrenVerticalAlignment
                        switch (ChildrenVerticalAlignment)
                        {
                            case _childrenVerticalAlignment.Top:
                                {
                                    if (maxRowIndex == null)
                                    {
                                        Canvas.SetTop(newChild, _verticalItemSpacing);
                                    }
                                    else
                                    {
                                        Canvas.SetTop(newChild, _maxRowHeightList.Sum() - _maxRowHeightList[(int)maxRowIndex] + ((double)maxRowIndex + 1) * _verticalItemSpacing);
                                    }
                                }
                                break;
                            case _childrenVerticalAlignment.Bottom://In fact, these childrens should be repositioned in case Bottom and Center
                                {
                                    if (maxRowIndex == null)
                                    {
                                        Canvas.SetTop(newChild, _verticalItemSpacing);
                                    }
                                    else
                                    {
                                        if (newChild.DesiredSize.Height > historyMaxRowHeight[(int)maxRowIndex])
                                        {
                                            _maxRowHeightList[_maxRowHeightList.Count - 1] = newChild.DesiredSize.Height;
                                            foreach (var item in _childrenList.Where(x => x.Item1 == (int)maxRowIndex))
                                            {
                                                Canvas.SetTop(item.Item3, _maxRowHeightList.Sum() - _maxRowHeightList[(int)maxRowIndex] + ((double)maxRowIndex + 1) * _verticalItemSpacing
                                                    + _maxRowHeightList[_maxRowHeightList.Count - 1] - item.Item3.DesiredSize.Height);
                                            }
                                        }
                                        Canvas.SetTop(newChild, _maxRowHeightList.Sum() - _maxRowHeightList[(int)maxRowIndex] + ((double)maxRowIndex + 1) * _verticalItemSpacing
                                            + _maxRowHeightList[_maxRowHeightList.Count - 1] - newChild.DesiredSize.Height);
                                    }
                                }
                                break;
                            case _childrenVerticalAlignment.Center:
                                {
                                    if (maxRowIndex == null)
                                    {
                                        Canvas.SetTop(newChild, _verticalItemSpacing);
                                    }
                                    else
                                    {
                                        if (newChild.DesiredSize.Height > historyMaxRowHeight[(int)maxRowIndex])
                                        {
                                            _maxRowHeightList[_maxRowHeightList.Count - 1] = newChild.DesiredSize.Height;
                                            int targetRowIndex = (int)maxRowIndex;//For easier implantation, make it can update any row of UIElements
                                            foreach (var item in _childrenList.Where(x => x.Item1 == targetRowIndex))
                                            {
                                                Canvas.SetTop(item.Item3, _maxRowHeightList.Sum() - _maxRowHeightList[(int)maxRowIndex] + ((double)maxRowIndex + 1) * _verticalItemSpacing
                                                    + (double)0.5 * (_maxRowHeightList[_maxRowHeightList.Count - 1] - item.Item3.DesiredSize.Height));
                                            }
                                        }
                                        Canvas.SetTop(newChild, _maxRowHeightList.Sum() - _maxRowHeightList[(int)maxRowIndex] + ((double)maxRowIndex + 1) * _verticalItemSpacing
                                            + (double)0.5 * (_maxRowHeightList[_maxRowHeightList.Count - 1] - newChild.DesiredSize.Height));
                                    }
                                }
                                break;
                        }

                        int? newChildColumnIndex = maxRowIndex == null ? null : (int?)(_childrenList.Where(x => x.Item1 == maxRowIndex).Count());

                        _childrenList.Add((maxRowIndex == null || _childrenList.Count == 0 ? 0 : (int)maxRowIndex, newChildColumnIndex == null ? 0 : (int)newChildColumnIndex, newChild));

                        //Log item height and width
                        if (maxRowIndex == null)
                        {
                            _maxRowHeightList.Add(newChild.DesiredSize.Height);
                            _rowControlWidthList.Add(newChild.DesiredSize.Width);
                        }
                        else
                        {
                            _rowControlWidthList[_maxRowHeightList.Count() - 1] += newChild.DesiredSize.Width;
                        }
                    }
                }
            }
        }

        protected virtual void OnChildRemoved(UIElement child)
        {
            if (child != null)
            {
                var deletedChildMapping = _childrenList.Where(x => x.Item3 == child).FirstOrDefault();
                var childrenList = _childrenList.Where(x => true).ToList();
                foreach (var item in childrenList.Where(x => (x.Item1 == deletedChildMapping.Item1 && x.Item2 >= deletedChildMapping.Item2) || x.Item1 > deletedChildMapping.Item1))
                {
                    //Item was at the same row with deleted item
                    if (item.Item1 == deletedChildMapping.Item1)
                    {
                        if (item != deletedChildMapping)
                        {
                            Canvas.SetLeft(item.Item3, (double)(_childrenList.Where(x => x.Item1 == item.Item1 && x.Item2 < item.Item2 && x != deletedChildMapping).Count() + 1) * _horizontalItemSpacing
                                + (double)_childrenList.Where(x => x.Item1 == item.Item1 && x.Item2 < item.Item2 && x != deletedChildMapping).Sum(x => x.Item3.DesiredSize.Width));
                        }

                        //update _maxRowHeightList and _rowControlWidthList
                        if (_childrenList.Where(x => x.Item1 == item.Item1 && x != deletedChildMapping).Count() > 0)
                        {
                            _maxRowHeightList[item.Item1] = _childrenList.Where(x => x.Item1 == item.Item1 && x != deletedChildMapping).Max(x => x.Item3.DesiredSize.Height);
                            _rowControlWidthList[item.Item1] = _childrenList.Where(x => x.Item1 == item.Item1 && x != deletedChildMapping).Sum(x => x.Item3.DesiredSize.Width);
                        }
                        else
                        {
                            _maxRowHeightList.RemoveAt(item.Item1);
                            _rowControlWidthList.RemoveAt(item.Item1);
                        }

                        _childrenList[_childrenList.IndexOf(item)] = (item.Item1, item.Item2 - 1, item.Item3);
                    }
                    //update maxrowindex


                }
                //Remove target element in the list
                _childrenList.Remove(_childrenList.Where(x => x.Item3 == child).FirstOrDefault());//Interstingly, using deletedChildMapping won't delete any target in the List.
            }
        }

        protected virtual void OnChildReplaced(UIElement newChild)
        {

        }
    }
}
