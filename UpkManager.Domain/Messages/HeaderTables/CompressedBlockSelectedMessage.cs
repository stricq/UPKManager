using STR.Common.Messages;

using UpkManager.Domain.Models.Compression;


namespace UpkManager.Domain.Messages.HeaderTables {

  public class CompressedBlockSelectedMessage : MessageBase {

    public CompressedBlockSelectedMessage() : base(true) { }

    public DomainCompressedChunkBlock Block { get; set; }

  }

}
