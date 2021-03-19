using System;
using System.Runtime.Serialization;

namespace CleanArchitecture.Application.Common.Exceptions
{
    [Serializable]
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base() { }

        protected ForbiddenAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
