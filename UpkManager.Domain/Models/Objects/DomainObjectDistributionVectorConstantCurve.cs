using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectDistributionVectorConstantCurve : DomainObjectDistributionFloatUniform {

    #region Overrides

    public override ObjectType ObjectType => ObjectType.DistributionVectorConstantCurve;

    #endregion Overrides

  }

}