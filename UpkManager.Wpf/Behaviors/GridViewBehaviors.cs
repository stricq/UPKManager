using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using STR.Common.Extensions;


namespace UpkManager.Wpf.Behaviors {

  public static class GridViewBehaviors {

    #region AutoResizeColumns Property

    public static readonly DependencyProperty AutoResizeColumnsProperty = DependencyProperty.RegisterAttached("AutoResizeColumns", typeof(bool), typeof(GridViewBehaviors), new FrameworkPropertyMetadata(false, onResizeColumnsPropertyChanged));

    public static bool GetAutoResizeColumns(DependencyObject obj) {
      return (bool)obj.GetValue(AutoResizeColumnsProperty);
    }

    public static void SetAutoResizeColumns(DependencyObject obj, bool value) {
      obj.SetValue(AutoResizeColumnsProperty, value);
    }

    private static void onResizeColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
      ListView view = sender as ListView;

      if (view != null) handleListView(view, args);
    }

    private static void handleListView(ListView list, DependencyPropertyChangedEventArgs args) {
      list.Loaded += (o, eventArgs) => {
        ListView listView = o as ListView;

        int count = VisualTreeHelper.GetChildrenCount(listView);

        Decorator border = null;

        if (count > 0) border = VisualTreeHelper.GetChild(listView, 0) as Decorator;

        ScrollViewer scroller = border?.Child as ScrollViewer;

        if (scroller == null) return;

        if ((bool)args.NewValue) scroller.ScrollChanged += onListViewScrollChanged;
        else scroller.ScrollChanged -= onListViewScrollChanged;
      };
    }

    private static void onListViewScrollChanged(object sender, ScrollChangedEventArgs e) {
      if (!e.ViewportHeightChange.EqualInPercentRange(0.0) || !e.VerticalChange.EqualInPercentRange(0.0) || !e.ExtentHeightChange.EqualInPercentRange(0.0)) {
        ScrollViewer scroller = sender as ScrollViewer;

        if (scroller == null) return;

        Decorator border = VisualTreeHelper.GetParent(scroller) as Decorator;

        if (border == null) return;

        ListView listView = VisualTreeHelper.GetParent(border) as ListView;

        GridView view = listView?.View as GridView;

        if (view == null) return;

        foreach(GridViewColumn column in view.Columns) {
          if (Double.IsNaN(column.Width)) column.Width = column.ActualWidth;

          column.Width = Double.NaN;
        }
      }
    }

    #endregion AutoResizeColumns Property

  }

}
