using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

using STR.MvvmCommon;


namespace UpkManager.Wpf.Behaviors {

  public static class HyperlinkBehaviors {

    public static readonly DependencyProperty RequestNavigationCommandProperty = DependencyProperty.RegisterAttached("RequestNavigationCommand", typeof(ICommand), typeof(HyperlinkBehaviors), new FrameworkPropertyMetadata(null, onRequestNavigationCommandPropertyChanged));

    public static ICommand GetRequestNavigationCommand(DependencyObject obj) {
      return obj.GetValue(RequestNavigationCommandProperty) as ICommand;
    }

    public static void SetRequestNavigationCommand(DependencyObject obj, ICommand value) {
      obj.SetValue(RequestNavigationCommandProperty, value);
    }

    private static void onRequestNavigationCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
      Hyperlink link = sender as Hyperlink;

      if (link == null) return;

      if (args.NewValue != null && args.OldValue == null) {
        link.RequestNavigate += onLinkRequestNavigate;
      }
      else if (args.NewValue == null && args.OldValue != null) {
        link.RequestNavigate -= onLinkRequestNavigate;
      }
    }

    private static void onLinkRequestNavigate(object sender, RequestNavigateEventArgs args) {
      if (!ReferenceEquals(sender, args.OriginalSource)) return;

      if (ObservableObject.IsInDesignMode) return;

      Hyperlink link = sender as Hyperlink;

      ICommand command = link?.GetValue(RequestNavigationCommandProperty) as ICommand;

      if (command != null && command.CanExecute(args)) command.Execute(args);
    }

  }

}
