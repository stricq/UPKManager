using System.Collections.Generic;
using System.Linq;

using UpkManager.Domain.Models.UpkFile;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Extensions {

  internal static class ListExtensions {

    public static DomainNameTableEntry AddDomainNameTableEntry(this List<DomainNameTableEntry> nameTable, string value) {
      DomainString valueString = new DomainString();

      valueString.SetString(value);

      int index = nameTable.Max(nt => nt.TableIndex) + 1;

      DomainNameTableEntry entry = new DomainNameTableEntry();

      entry.SetNameTableEntry(valueString, 0x0007001000000000, index);

      nameTable.Add(entry);

      return entry;
    }

  }

}
