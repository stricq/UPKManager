using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Properties {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainPropertyHeader : ObservableObject {

    #region Private Fields

    private uint index;

    private ObservableCollection<DomainProperty> properties;

    #endregion Private Fields

    #region Properties

    public uint Index {
      get { return index; }
      set { SetField(ref index, value, () => Index); }
    }

    public ObservableCollection<DomainProperty> Properties {
      get { return properties; }
      set { SetField(ref properties, value, () => Properties); }
    }

    #endregion Properties

  }

}
