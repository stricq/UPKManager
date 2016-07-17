using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("StatusBarViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class StatusBarViewModel : ObservableObject {

    #region Private Fields

    private bool jobProgressVisibility;

    private double memory;

    private double jobProgress;

    private string jobProgressText;
    private string      statusText;

    #endregion Private Fields

    #region Properties

    public bool JobProgressVisibility {
      get { return jobProgressVisibility; }
      set { SetField(ref jobProgressVisibility, value, () => JobProgressVisibility); }
    }

    public double Memory {
      get { return memory; }
      set { SetField(ref memory, value, () => Memory); }
    }

    public double JobProgress {
      get { return jobProgress; }
      set { SetField(ref jobProgress, value, () => JobProgress); }
    }

    public string JobProgressText {
      get { return jobProgressText; }
      set { SetField(ref jobProgressText, value, () => JobProgressText); }
    }

    public string StatusText {
      get { return statusText; }
      set { SetField(ref statusText, value, () => StatusText); }
    }

    #endregion Properties

  }

}
