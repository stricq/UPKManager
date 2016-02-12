using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectDistributionFloatConstantCurve : DomainObjectDistributionFloatUniform {

    #region Overrides

    public override ObjectType ObjectType => ObjectType.DistributionFloatConstant;

    #endregion Overrides

  }

}
