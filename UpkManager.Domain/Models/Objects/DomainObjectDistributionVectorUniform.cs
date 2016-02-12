using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectDistributionVectorUniform : DomainObjectDistributionFloatUniform {

    #region Overrides

    public override ObjectType ObjectType => ObjectType.DistributionVectorUniform;

    #endregion Overrides

  }

}