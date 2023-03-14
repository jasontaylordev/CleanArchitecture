using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Common.Contracts;

public interface IAuditableEntity
{
    DateTime Created { get; set; }

    string? CreatedBy { get; set; }

    DateTime? LastModified { get; set; }

    string? LastModifiedBy { get; set; }
}
