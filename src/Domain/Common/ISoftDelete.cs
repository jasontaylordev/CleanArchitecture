using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Common;
/// <summary>
/// Marks an entity as soft-deletable.  A DELETE will be converted into a
/// timestamp, and global query filters will hide those rows.
/// </summary>
public interface ISoftDelete
{
    DateTimeOffset? Deleted { get; set; }
    string? DeletedBy { get; set; }
    bool IsDeleted => Deleted.HasValue;
}
