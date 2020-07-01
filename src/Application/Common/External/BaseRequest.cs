using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.External
{
    public class BaseRequest<T> : IRequest<T>
    {
        public string AuthCode { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(AuthCode) || !AuthCode.Equals("d2umyeg2020"))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
