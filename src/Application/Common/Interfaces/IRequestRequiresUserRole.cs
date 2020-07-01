using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IRequestRequiresUserRole<out TResponse> : IRequest<TResponse>
    {
        List<string> RequiredRoles { get; }
    }
}
