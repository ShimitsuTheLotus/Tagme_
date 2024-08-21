# AutoWrapPanel Document

*The document only show customized or important items*

## Properties
- x:Name
- x:UID
- Children
```cs
//Children - AutoWrapPanel.cs

public class AutoWrapPanel : Control
{
    private ObservableCollection<UIElement> _children;
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

    public ObservableCollection<UIElement> Children => _children;

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
}
```

# Functions
- Children.Add()
```cs

```
