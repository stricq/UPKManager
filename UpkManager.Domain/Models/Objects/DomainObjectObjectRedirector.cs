using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectObjectRedirector : DomainObjectBase {

    #region Private Fields

    private int objectTableReference;

    #endregion Private Fields

    #region Properties

    public int ObjectTableReference {
      get { return objectTableReference; }
      set { SetField(ref objectTableReference, value, () => ObjectTableReference); }
    }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.ObjectRedirector;

    #endregion Overrides

  }

}
