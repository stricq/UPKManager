using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectDistributionBase : DomainObjectBase {

    #region Private Fields

    private uint unknown1;

    #endregion Private Fields

    #region Properties

    public uint Unknown1 {
      get { return unknown1; }
      set { SetField(ref unknown1, value, () => Unknown1); }
    }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.DistributionFloatUniform;

    #endregion Overrides

  }

}
